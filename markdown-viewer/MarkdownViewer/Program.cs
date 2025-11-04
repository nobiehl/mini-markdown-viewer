using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace MarkdownViewer
{
    /// <summary>
    /// Entry point for the MarkdownViewer application.
    /// Handles command-line arguments, Windows Explorer integration, and application startup.
    /// </summary>
    static class Program
    {
        private const string AppName = "MarkdownViewer";
        private const string Version = "1.0.1";

        /// <summary>
        /// Main entry point for the application.
        /// Supports multiple command-line modes:
        /// - No args: Opens file selection dialog
        /// - File path: Opens specified .md file
        /// - --install: Registers Windows Explorer integration
        /// - --uninstall: Removes Windows Explorer integration
        /// - --version: Shows version information
        /// - --help: Shows usage information
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        [STAThread]
        static void Main(string[] args)
        {
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
                    string help = @"Markdown Viewer - Simple Windows Markdown Viewer

Usage:
  MarkdownViewer.exe <file.md>     Open a markdown file
  MarkdownViewer.exe --install      Register as default .md viewer
  MarkdownViewer.exe --uninstall    Unregister file association
  MarkdownViewer.exe --version      Show version
  MarkdownViewer.exe --help         Show this help";

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
                Application.Run(new MainForm(filePath));
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
                        Application.Run(new MainForm(openFileDialog.FileName));
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
        private static void InstallFileAssociation()
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
                Type shellType = Type.GetTypeFromProgID("WScript.Shell");
                if (shellType == null) return;

                dynamic shell = Activator.CreateInstance(shellType);
                var shortcut = shell.CreateShortcut(shortcutPath);
                shortcut.TargetPath = targetPath;
                shortcut.Save();

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
        private static void UninstallFileAssociation()
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
    }
}
