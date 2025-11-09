using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.DependencyInjection;
using MarkdownViewer.Core;
using MarkdownViewer.Core.Configuration;
using MarkdownViewer.Core.Models;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Core.Views;
using MarkdownViewer.Core.Presenters;
using MarkdownViewer.Services;
using MarkdownViewer.Views;

namespace MarkdownViewer
{
    /// <summary>
    /// Entry point for the MarkdownViewer application.
    /// Handles command-line arguments, Windows Explorer integration, and application startup.
    /// </summary>
    static class Program
    {
        private const string AppName = "MarkdownViewer";
        private const string Version = "1.7.4";

        /// <summary>
        /// Main entry point for the application.
        /// Supports multiple command-line modes:
        /// - No args: Opens file selection dialog
        /// - File path: Opens specified .md file
        /// - --install: Registers Windows Explorer integration
        /// - --uninstall: Removes Windows Explorer integration
        /// - --update: Check for updates manually
        /// - --test-update: Run update test (test mode only)
        /// - --version: Shows version information
        /// - --help: Shows usage information
        /// - --log-level [Debug|Information|Warning|Error]: Sets logging verbosity
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        [STAThread]
        static void Main(string[] args)
        {
            // FIRST: Apply any pending update before anything else
            // This must be done before logging is initialized
            UpdateChecker.ApplyPendingUpdate();

            // SECOND: Clean up old backup from previous update (if any)
            // This is safe to do even if there's no backup
            UpdateChecker.CleanupOldBackup();

            // Parse log level from command line (default: Information)
            LogEventLevel logLevel = LogEventLevel.Information;
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("--log-level", StringComparison.OrdinalIgnoreCase))
                {
                    if (Enum.TryParse<LogEventLevel>(args[i + 1], true, out LogEventLevel parsedLevel))
                    {
                        logLevel = parsedLevel;
                    }
                    break;
                }
            }

            // Parse update flags
            bool forceUpdateCheck = args.Any(arg => arg.Equals("--update", StringComparison.OrdinalIgnoreCase));
            bool testUpdateMode = args.Any(arg => arg.Equals("--test-update", StringComparison.OrdinalIgnoreCase));

            // Handle --test-update mode (for testing update mechanism)
            if (testUpdateMode)
            {
                string? scenario = GetArgValue(args, "--test-scenario");
                RunUpdateTest(scenario ?? "update-available", logLevel);
                return;
            }

            // Handle command-line arguments
            if (args.Length > 0)
            {
                string firstArg = args[0].ToLower();

                if (firstArg == "--version" || firstArg == "-v")
                {
                    MessageBox.Show($"Markdown Viewer v{Version}", "Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (firstArg == "--help" || firstArg == "-h")
                {
                    string help = @"Markdown Viewer - Lightweight Windows Markdown Viewer

Usage:
  MarkdownViewer.exe <file.md>                    Open a markdown file
  MarkdownViewer.exe <file.md> --log-level Debug  Open with debug logging
  MarkdownViewer.exe --install                     Register as default .md viewer
  MarkdownViewer.exe --uninstall                   Unregister file association
  MarkdownViewer.exe --update                      Check for updates
  MarkdownViewer.exe --version                     Show version
  MarkdownViewer.exe --help                        Show this help

Update Behavior:
  - Automatic check once per day on first start
  - Silent background check (non-blocking)
  - Manual check with --update parameter

Test Mode (Development):
  MarkdownViewer.exe --test-update --test-scenario <name>
  Scenarios: update-available, no-update, network-error, rate-limit

Log Levels:
  Debug        Verbose logging (all operations)
  Information  Normal logging (default)
  Warning      Only warnings and errors
  Error        Only errors";

                    MessageBox.Show(help, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (firstArg == "--install")
                {
                    InstallFileAssociation();
                    return;
                }

                if (firstArg == "--uninstall")
                {
                    UninstallFileAssociation();
                    return;
                }

                // Check if file exists
                string filePath = args[0];
                if (!File.Exists(filePath))
                {
                    MessageBox.Show($"File not found: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!filePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Only .md files are supported", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open the file
                ApplicationConfiguration.Initialize();

                // Build DI container and resolve MainForm
                using var serviceProvider = BuildServiceProvider(filePath, logLevel);
                var form = serviceProvider.GetRequiredService<MainForm>();

                // Start async update check if needed
                if (forceUpdateCheck || new UpdateChecker().ShouldCheckForUpdates())
                {
                    _ = Task.Run(async () => await CheckForUpdatesAsync(form, forceUpdateCheck));
                }

                Application.Run(form);
            }
            else
            {
                // No arguments - show file open dialog
                ApplicationConfiguration.Initialize();

                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Markdown files (*.md)|*.md|All files (*.*)|*.*";
                    openFileDialog.Title = "Open Markdown File";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        // Build DI container and resolve MainForm
                        using var serviceProvider = BuildServiceProvider(openFileDialog.FileName, logLevel);
                        var form = serviceProvider.GetRequiredService<MainForm>();

                        // Start async update check if needed
                        if (forceUpdateCheck || new UpdateChecker().ShouldCheckForUpdates())
                        {
                            _ = Task.Run(async () => await CheckForUpdatesAsync(form, forceUpdateCheck));
                        }

                        Application.Run(form);
                    }
                }
            }
        }

        /// <summary>
        /// Installs full Windows Explorer integration for .md files.
        /// Creates registry entries in HKEY_CURRENT_USER (no admin rights required):
        /// 1. Default file association (double-click opens MarkdownViewer)
        /// 2. Context menu entry ("Open with Markdown Viewer")
        /// 3. "Open With" dialog integration
        /// 4. "Send To" menu shortcut
        /// </summary>
        public static void InstallFileAssociation()
        {
            try
            {
                string exePath = Application.ExecutablePath;

                // 1. Default file association (.md extension)
                // HKEY_CURRENT_USER\Software\Classes\.md
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\.md"))
                {
                    key?.SetValue("", "MarkdownViewer.Document");
                }

                // HKEY_CURRENT_USER\Software\Classes\MarkdownViewer.Document
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\MarkdownViewer.Document"))
                {
                    key?.SetValue("", "Markdown Document");
                }

                // HKEY_CURRENT_USER\Software\Classes\MarkdownViewer.Document\shell\open\command
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\MarkdownViewer.Document\shell\open\command"))
                {
                    key?.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                // 2. Context menu integration (appears even if another program is default)
                // HKEY_CURRENT_USER\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer"))
                {
                    key?.SetValue("", "Open with Markdown Viewer");
                    key?.SetValue("Icon", exePath);
                }

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer\command"))
                {
                    key?.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                // 3. "Open With" list integration
                // HKEY_CURRENT_USER\Software\Classes\Applications\MarkdownViewer.exe
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\MarkdownViewer.exe"))
                {
                    key?.SetValue("FriendlyAppName", "Markdown Viewer");
                }

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\MarkdownViewer.exe\SupportedTypes"))
                {
                    key?.SetValue(".md", "");
                    key?.SetValue(".markdown", "");
                }

                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\Applications\MarkdownViewer.exe\shell\open\command"))
                {
                    key?.SetValue("", $"\"{exePath}\" \"%1\"");
                }

                // 4. "Send To" menu integration
                string sendToPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.SendTo),
                    "Markdown Viewer.lnk"
                );

                CreateShortcut(exePath, sendToPath);

                MessageBox.Show(
                    "Markdown Viewer successfully registered!\n\n" +
                    "Integration points:\n" +
                    "• Default program for .md files\n" +
                    "• Right-click context menu\n" +
                    "• 'Open With' dialog\n" +
                    "• 'Send To' menu",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to register file association:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Creates a Windows shortcut (.lnk file) using WScript.Shell COM object.
        /// Used to create "Send To" menu entry.
        /// </summary>
        /// <param name="targetPath">Full path to MarkdownViewer.exe</param>
        /// <param name="shortcutPath">Full path where .lnk file should be created</param>
        private static void CreateShortcut(string targetPath, string shortcutPath)
        {
            try
            {
                // Use IWshRuntimeLibrary to create shortcut
                Type? shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null) return;

                object? shell = Activator.CreateInstance(shellType);
                if (shell == null) return;

                dynamic shortcut = shell.GetType().InvokeMember(
                    "CreateShortcut",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    shell,
                    new object[] { shortcutPath })!;

                var targetPathProperty = shortcut.GetType().GetProperty("TargetPath");
                targetPathProperty?.SetValue(shortcut, targetPath);

                shortcut.GetType().InvokeMember(
                    "Save",
                    System.Reflection.BindingFlags.InvokeMethod,
                    null,
                    shortcut,
                    null);

                System.Runtime.InteropServices.Marshal.ReleaseComObject(shortcut);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(shell);
            }
            catch
            {
                // Silently fail if shortcut creation fails
            }
        }

        /// <summary>
        /// Removes all Windows Explorer integration created by InstallFileAssociation().
        /// Deletes registry entries and "Send To" shortcut.
        /// Safe to call even if --install was never executed (uses throwOnMissingSubKey: false).
        /// </summary>
        public static void UninstallFileAssociation()
        {
            try
            {
                // Remove registry keys
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\.md", false);
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\MarkdownViewer.Document", false);
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer", false);
                Registry.CurrentUser.DeleteSubKeyTree(@"Software\Classes\Applications\MarkdownViewer.exe", false);

                // Remove Send To shortcut
                string sendToPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.SendTo),
                    "Markdown Viewer.lnk"
                );

                if (File.Exists(sendToPath))
                {
                    File.Delete(sendToPath);
                }

                MessageBox.Show(
                    "Markdown Viewer successfully unregistered!\n\n" +
                    "All integration points have been removed.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to unregister file association:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Gets the value of a command-line argument.
        /// Example: --test-scenario update-available returns "update-available"
        /// </summary>
        private static string? GetArgValue(string[] args, string argName)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals(argName, StringComparison.OrdinalIgnoreCase))
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        /// <summary>
        /// Runs an update test with the specified scenario.
        /// Used for testing the update mechanism without real GitHub releases.
        /// </summary>
        private static void RunUpdateTest(string scenario, LogEventLevel logLevel)
        {
            Console.WriteLine($"=== Testing Update Scenario: {scenario} ===");

            // Enable test mode
            string testDataPath = Environment.GetEnvironmentVariable("MDVIEWER_TEST_DATA") ?? "./test-data";
            UpdateConfiguration.Instance.EnableTestMode("http://localhost:8080", testDataPath, scenario);

            Console.WriteLine($"Configuration: {UpdateConfiguration.Instance}");
            Console.WriteLine();

            var checker = new UpdateChecker();
            var updateInfo = checker.CheckForUpdatesAsync(Version).Result;

            Console.WriteLine($"Update Available: {updateInfo.UpdateAvailable}");
            Console.WriteLine($"Current Version: {updateInfo.CurrentVersion ?? Version}");
            Console.WriteLine($"Latest Version: {updateInfo.LatestVersion ?? "N/A"}");
            Console.WriteLine($"Error: {updateInfo.Error ?? "None"}");
            Console.WriteLine($"Is Prerelease: {updateInfo.IsPrerelease}");

            if (updateInfo.UpdateAvailable)
            {
                Console.WriteLine($"Download URL: {updateInfo.DownloadUrl}");
                Console.WriteLine($"File Size: {updateInfo.FileSize / 1024.0 / 1024.0:F2} MB");
                if (!string.IsNullOrEmpty(updateInfo.ReleaseNotes))
                {
                    int noteLength = Math.Min(100, updateInfo.ReleaseNotes.Length);
                    Console.WriteLine($"Release Notes: {updateInfo.ReleaseNotes.Substring(0, noteLength)}...");
                }
            }

            Console.WriteLine();
            Console.WriteLine(updateInfo.UpdateAvailable ? "[PASS] Test PASS: Update detected" : "[PASS] Test PASS: No update");

            Environment.Exit(updateInfo.UpdateAvailable ? 0 : 1);
        }

        /// <summary>
        /// Checks for updates asynchronously after MainForm has started.
        /// Runs in background thread, does not block UI.
        /// Shows dialog if update is available.
        /// </summary>
        private static async Task CheckForUpdatesAsync(MainForm form, bool forcedByUser)
        {
            try
            {
                var checker = new UpdateChecker();

                // Check for updates
                var updateInfo = await checker.CheckForUpdatesAsync(Version);

                // Handle errors - DO NOT record check on error!
                if (!string.IsNullOrEmpty(updateInfo.Error))
                {
                    Log.Warning("Update check failed with error: {Error} - Will retry on next start", updateInfo.Error);

                    if (forcedByUser)
                    {
                        form.Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                $"Update-Prüfung fehlgeschlagen:\n{updateInfo.Error}\n\nWird beim nächsten Start erneut versucht.",
                                "Update-Fehler",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning
                            );
                        }));
                    }
                    return;
                }

                // Success! Record that we successfully checked
                checker.RecordUpdateCheck();
                Log.Debug("Update check successful, recorded timestamp");

                // No update available
                if (!updateInfo.UpdateAvailable)
                {
                    Log.Information("No update available (latest: {Latest})", updateInfo.LatestVersion);

                    if (forcedByUser)
                    {
                        form.Invoke(new Action(() =>
                        {
                            MessageBox.Show(
                                $"Sie verwenden bereits die neueste Version ({Version}).",
                                "Kein Update verfügbar",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                        }));
                    }
                    return;
                }

                // Update is available!
                Log.Information("Update available: {Latest} (current: {Current})",
                    updateInfo.LatestVersion, Version);

                // Show update dialog on UI thread
                form.Invoke(new Action(async () =>
                {
                    // Show release notes in MarkdownDialog
                    string releaseNotesMarkdown = $@"# Update verfügbar: v{updateInfo.LatestVersion}

**Aktuelle Version:** v{Version}
**Größe:** {updateInfo.FileSize / 1024.0 / 1024.0:F1} MB

## Release Notes

{updateInfo.ReleaseNotes}

---

**Möchten Sie das Update jetzt herunterladen?**";

                    var renderer = new MarkdownRenderer();
                    MarkdownViewer.UI.MarkdownDialog.ShowDialog(form, "Update verfügbar", releaseNotesMarkdown, renderer);

                    // Ask for confirmation
                    var result = MessageBox.Show(
                        "Jetzt herunterladen?",
                        "Update verfügbar",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        Log.Information("User accepted update download");

                        // Download update
                        string exeDir = Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;
                        string updatePath = Path.Combine(exeDir, "pending-update.exe");

                        bool downloaded = await checker.DownloadUpdateAsync(updateInfo.DownloadUrl);

                        if (!downloaded)
                        {
                            MessageBox.Show(
                                "Download fehlgeschlagen.\nBitte versuchen Sie es später erneut.",
                                "Download-Fehler",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                            return;
                        }

                        // Ask about restart
                        var restartResult = MessageBox.Show(
                            "Update wurde heruntergeladen.\n\n" +
                            "Das Update wird beim nächsten Start installiert.\n\n" +
                            "Jetzt neu starten?",
                            "Update heruntergeladen",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (restartResult == DialogResult.Yes)
                        {
                            Log.Information("User requested restart to apply update");
                            UpdateChecker.ApplyPendingUpdate();
                        }
                        else
                        {
                            Log.Information("User postponed restart, update will apply on next start");
                        }
                    }
                    else
                    {
                        Log.Information("User declined update download");
                    }
                }));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error in CheckForUpdatesAsync");

                if (forcedByUser)
                {
                    form.Invoke(new Action(() =>
                    {
                        MessageBox.Show(
                            $"Unerwarteter Fehler bei der Update-Prüfung:\n{ex.Message}",
                            "Fehler",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }));
                }
            }
        }

        /// <summary>
        /// Truncates a string to a maximum length, adding "..." if truncated
        /// </summary>
        private static string TruncateString(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text ?? "";

            return text.Substring(0, maxLength) + "...";
        }

        /// <summary>
        /// Builds and configures the dependency injection service provider.
        /// Registers all services, presenters, and views for MVP pattern.
        /// </summary>
        private static ServiceProvider BuildServiceProvider(string filePath, LogEventLevel logLevel)
        {
            var services = new ServiceCollection();

            // Register Application Services (Singleton)
            services.AddSingleton<ISettingsService, SettingsService>();
            services.AddSingleton<IThemeService, ThemeService>();
            services.AddSingleton<ILocalizationService>(sp =>
            {
                var settingsService = sp.GetRequiredService<ISettingsService>();
                var settings = settingsService.Load();
                return new LocalizationService(settings.Language ?? "en");
            });
            services.AddSingleton<IDialogService, WinFormsDialogService>();

            // Register Core Components (Transient)
            services.AddTransient<MarkdownRenderer>();
            services.AddTransient<FileWatcherManager>();

            // Register WebView2 Adapter (Transient - created per form)
            services.AddTransient<IWebViewAdapter>(sp =>
            {
                var webView = new Microsoft.Web.WebView2.WinForms.WebView2();
                return new WebView2Adapter(webView);
            });

            // Register Presenters (Transient)
            services.AddTransient<MainPresenter>();
            services.AddTransient<StatusBarPresenter>();
            services.AddTransient<SearchBarPresenter>();
            services.AddTransient<NavigationPresenter>();

            // Register MainForm (Transient)
            services.AddTransient<MainForm>(sp =>
            {
                // Create MainForm with DI (will need constructor refactoring)
                return new MainForm(filePath, logLevel);
            });

            return services.BuildServiceProvider();
        }
    }
}
