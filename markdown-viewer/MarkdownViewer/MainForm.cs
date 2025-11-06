using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Serilog;
using Serilog.Events;
using MarkdownViewer.Core;
using MarkdownViewer.Models;
using MarkdownViewer.Services;
using MarkdownViewer.UI;

namespace MarkdownViewer
{
    /// <summary>
    /// Main window form for displaying Markdown files.
    /// Refactored in v1.2.0 for layered architecture with services.
    /// Enhanced in v1.3.0 with localization and status bar.
    /// Uses WebView2 for HTML rendering with theme support.
    /// </summary>
    public class MainForm : Form
    {
        private const string Version = "1.3.0";

        // UI Components
        private WebView2 _webView;
        private StatusBarControl? _statusBar;

        // Services
        private readonly LocalizationService _localizationService;
        private readonly SettingsService _settingsService;
        private readonly ThemeService _themeService;
        private readonly MarkdownRenderer _renderer;
        private readonly FileWatcherManager _fileWatcher;

        // State
        private string _currentFilePath;
        private AppSettings _settings;
        private Theme _currentTheme;

        /// <summary>
        /// Initializes the main form with a specified Markdown file.
        /// </summary>
        /// <param name="filePath">Full path to the .md file to display</param>
        /// <param name="logLevel">Minimum log level (default: Information)</param>
        public MainForm(string filePath, LogEventLevel logLevel = LogEventLevel.Information)
        {
            InitializeLogging(logLevel);
            Log.Information("=== MarkdownViewer v{Version} Starting (LogLevel: {LogLevel}) ===", Version, logLevel);
            Log.Information("Opening file: {FilePath}", filePath);

            // Initialize services
            _settingsService = new SettingsService();
            _themeService = new ThemeService();
            _renderer = new MarkdownRenderer();
            _fileWatcher = new FileWatcherManager();

            // Load settings and theme
            LoadSettings();

            // Initialize localization service with language from settings
            _localizationService = new LocalizationService(_settings.Language);

            // Initialize UI
            InitializeComponents();

            // Initialize StatusBar if enabled
            if (_settings.UI.StatusBar.Visible)
            {
                InitializeStatusBar();
            }

            // Load initial file
            _currentFilePath = filePath;
            LoadMarkdownFile(filePath);

            // Setup file watching for live reload
            SetupFileWatcher(filePath);
        }

        private void LoadSettings()
        {
            try
            {
                _settings = _settingsService.Load();
                _currentTheme = _themeService.LoadTheme(_settings.Theme);
                Log.Information("Settings loaded: Theme={Theme}, Language={Language}", _settings.Theme, _settings.Language);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load settings, using defaults");
                _settings = new AppSettings();
                _currentTheme = _themeService.LoadTheme("standard");
            }
        }

        /// <summary>
        /// Initializes Serilog logging with rolling file output.
        /// </summary>
        private void InitializeLogging(LogEventLevel logLevel)
        {
            try
            {
                string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
                string logsFolder = Path.Combine(exeFolder, "logs");

                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(logLevel)
                    .WriteTo.File(
                        path: Path.Combine(logsFolder, "viewer-.log"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Warning: Could not initialize logging:\n{ex.Message}",
                    "Logging Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Initializes all UI components and WebView2.
        /// </summary>
        private void InitializeComponents()
        {
            // Window setup
            this.Text = $"Markdown Viewer v{Version}";
            this.Size = new System.Drawing.Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;

            // WebView2 setup
            _webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(_webView);

            // Set WebView2 data folder to .cache next to EXE
            string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
            string userDataFolder = Path.Combine(exeFolder, ".cache");

            var env = Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                null,
                userDataFolder
            ).Result;

            _webView.EnsureCoreWebView2Async(env);

            // Setup WebView2 event handlers
            _webView.CoreWebView2InitializationCompleted += OnWebView2Initialized;
        }

        private void OnWebView2Initialized(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                Log.Information("WebView2 initialized successfully");

                _webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                _webView.CoreWebView2.Settings.AreDevToolsEnabled = false;

                // Handle JavaScript messages (link clicks)
                _webView.CoreWebView2.WebMessageReceived += (s, args) =>
                {
                    string message = args.TryGetWebMessageAsString();
                    Log.Debug("WebMessage received: {Message}", message);
                    HandleLinkClick(message);
                };

                // Handle navigation
                _webView.CoreWebView2.NavigationStarting += OnNavigationStarting;

                // Handle errors
                _webView.CoreWebView2.ProcessFailed += (s, args) =>
                {
                    Log.Error("WebView2 process failed: {Reason}", args.Reason);
                    MessageBox.Show($"WebView2 process failed: {args.Reason}\nCheck logs for details.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                // Apply theme to UI
                ApplyThemeToUI();
            }
            else
            {
                Log.Error("WebView2 initialization failed: {ErrorCode}", e.InitializationException?.Message ?? "Unknown error");
                MessageBox.Show($"Failed to initialize WebView2:\n{e.InitializationException?.Message ?? "Unknown error"}\n\nCheck logs for details.",
                    "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyThemeToUI()
        {
            try
            {
                // Apply theme to WinForms (will be more comprehensive in v1.3.0 with StatusBar)
                this.BackColor = System.Drawing.ColorTranslator.FromHtml(_currentTheme.UI.FormBackground);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to apply theme to UI");
            }
        }

        private void OnNavigationStarting(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Log.Debug("NavigationStarting: {Uri}", args.Uri);

            // Skip null, empty, or initial navigation
            if (string.IsNullOrEmpty(args.Uri) || args.Uri == "about:blank")
                return;

            // Skip data URIs
            if (args.Uri.StartsWith("data:"))
                return;

            // Handle HTTP/HTTPS links
            if (args.Uri.StartsWith("http://") || args.Uri.StartsWith("https://"))
            {
                if (IsInlineResource(args.Uri))
                {
                    Log.Debug("Allowing inline resource: {Uri}", args.Uri);
                    return;
                }

                // External links open in browser
                Log.Information("Opening external link in browser: {Uri}", args.Uri);
                args.Cancel = true;
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = args.Uri,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to open link: {Uri}", args.Uri);
                    MessageBox.Show($"Failed to open link: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                return;
            }

            // Handle local file:// links
            if (args.Uri.StartsWith("file://"))
            {
                HandleLocalFileNavigation(args);
            }
        }

        private void HandleLocalFileNavigation(Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Log.Debug("Handling file:// link: {Uri}", args.Uri);
            Uri uri = new Uri(args.Uri);
            string localPath = Uri.UnescapeDataString(uri.LocalPath);
            string fragment = uri.Fragment;

            if (localPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                localPath.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
            {
                args.Cancel = true;

                string currentFullPath = Path.GetFullPath(_currentFilePath);
                string linkedFullPath = Path.GetFullPath(localPath);

                if (currentFullPath.Equals(linkedFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    // Same file, anchor navigation
                    if (!string.IsNullOrEmpty(fragment))
                    {
                        Log.Information("Anchor navigation: {Anchor}", fragment);
                        ScrollToAnchor(fragment);
                    }
                    return;
                }

                // Different file - navigate
                if (File.Exists(localPath))
                {
                    Log.Information("Navigating to file: {FilePath}", localPath);
                    this.BeginInvoke(new Action(() =>
                    {
                        _currentFilePath = localPath;
                        LoadMarkdownFile(localPath);
                        SetupFileWatcher(localPath);

                        if (!string.IsNullOrEmpty(fragment))
                        {
                            System.Threading.Tasks.Task.Delay(200).ContinueWith(_ =>
                            {
                                this.Invoke(new Action(() => ScrollToAnchor(fragment)));
                            });
                        }
                    }));
                }
                else
                {
                    Log.Warning("File not found: {FilePath}", localPath);
                    MessageBox.Show($"File not found:\n{localPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ScrollToAnchor(string fragment)
        {
            string anchorId = fragment.TrimStart('#');
            _webView.ExecuteScriptAsync(
                $"document.getElementById('{anchorId}')?.scrollIntoView({{behavior: 'smooth'}}) || " +
                $"document.querySelector('a[name=\"{anchorId}\"]')?.scrollIntoView({{behavior: 'smooth'}});");
        }

        private bool IsInlineResource(string url)
        {
            return url.Contains("plantuml.com") ||
                   url.Contains("jsdelivr.net") ||
                   url.Contains("cdnjs.cloudflare.com") ||
                   url.Contains("githubusercontent.com") ||
                   url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase);
        }

        private void HandleLinkClick(string url)
        {
            Log.Debug("HandleLinkClick: {Url}", url);

            if (string.IsNullOrWhiteSpace(url))
                return;

            // Check if it's a local .md file
            if (File.Exists(url) && (url.EndsWith(".md") || url.EndsWith(".markdown")))
            {
                Log.Information("Opening local file via WebMessage: {FilePath}", url);
                _currentFilePath = url;
                LoadMarkdownFile(url);
                SetupFileWatcher(url);
            }
            else if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                if (!IsInlineResource(url))
                {
                    Log.Information("Opening external link via WebMessage: {Url}", url);
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to open link: {Url}", url);
                    }
                }
            }
        }

        private void LoadMarkdownFile(string filePath)
        {
            Log.Debug("LoadMarkdownFile: {FilePath}", filePath);

            try
            {
                string markdown = File.ReadAllText(filePath);
                Log.Debug("Read {Bytes} bytes from {FilePath}", markdown.Length, filePath);

                string html = _renderer.RenderToHtml(markdown, filePath, _currentTheme);
                Log.Debug("Rendered markdown to HTML ({HtmlLength} characters)", html.Length);

                // Update window title
                this.Text = $"{Path.GetFileName(filePath)} - Markdown Viewer v{Version}";

                if (_webView.CoreWebView2 != null)
                {
                    Log.Debug("Navigating WebView2 to HTML content");
                    _webView.CoreWebView2.NavigateToString(html);
                }
                else
                {
                    Log.Debug("WebView2 not ready, queueing navigation");
                    _webView.CoreWebView2InitializationCompleted += (s, e) =>
                    {
                        if (e.IsSuccess)
                        {
                            Log.Debug("WebView2 ready, navigating to queued HTML");
                            _webView.CoreWebView2.NavigateToString(html);
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading file: {FilePath}", filePath);
                MessageBox.Show($"Error loading file: {ex.Message}\n\nCheck logs for details.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupFileWatcher(string filePath)
        {
            Log.Debug("SetupFileWatcher: {FilePath}", filePath);

            // Subscribe to file change events
            _fileWatcher.FileChanged -= OnFileChanged; // Unsubscribe first to avoid duplicates
            _fileWatcher.FileChanged += OnFileChanged;

            // Start watching
            _fileWatcher.Watch(filePath);
        }

        private void OnFileChanged(object sender, string filePath)
        {
            // File changed - reload (already on UI thread via Invoke in FileWatcherManager)
            this.Invoke(new Action(() =>
            {
                Log.Information("File changed, reloading: {FilePath}", filePath);
                LoadMarkdownFile(_currentFilePath);
            }));
        }

        /// <summary>
        /// Initializes and configures the status bar.
        /// </summary>
        private void InitializeStatusBar()
        {
            Log.Debug("Initializing StatusBar");

            try
            {
                _statusBar = new StatusBarControl(_localizationService);

                // Wire up event handlers
                _statusBar.LanguageChanged += OnLanguageChanged;
                _statusBar.InfoClicked += OnInfoClicked;
                _statusBar.HelpClicked += OnHelpClicked;

                // Add to form
                this.Controls.Add(_statusBar);

                // Check initial statuses
                _statusBar.CheckExplorerRegistration();
                _statusBar.SetUpdateStatus(UpdateStatus.Unknown);

                Log.Information("StatusBar initialized successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize StatusBar");
            }
        }

        /// <summary>
        /// Handles language change from status bar.
        /// Refreshes all UI text.
        /// </summary>
        private void OnLanguageChanged(object? sender, EventArgs e)
        {
            Log.Information("Language changed to: {Language}", _localizationService.GetCurrentLanguage());

            try
            {
                // Update settings with new language
                _settings.Language = _localizationService.GetCurrentLanguage();
                _settingsService.Save(_settings);

                // Refresh status bar text
                _statusBar?.RefreshLanguage();

                // Note: Full UI localization will be implemented when all UI strings are migrated to resources
                // For now, only StatusBar is fully localized
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to handle language change");
            }
        }

        /// <summary>
        /// Handles info button click in status bar.
        /// Shows application info dialog.
        /// </summary>
        private void OnInfoClicked(object? sender, EventArgs e)
        {
            Log.Debug("Info button clicked");

            string info = $"Markdown Viewer v{Version}\n\n" +
                          $"Language: {_localizationService.GetCurrentLanguage().ToUpper()}\n" +
                          $"Theme: {_settings.Theme}\n" +
                          $"Settings: {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\MarkdownViewer\\settings.json";

            MessageBox.Show(info, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles help button click in status bar.
        /// Shows help information.
        /// </summary>
        private void OnHelpClicked(object? sender, EventArgs e)
        {
            Log.Debug("Help button clicked");

            string help = $"Markdown Viewer v{Version}\n\n" +
                          "Keyboard Shortcuts:\n" +
                          "• F5 - Reload file\n" +
                          "• Ctrl+Mouse Wheel - Zoom in/out\n\n" +
                          "Features:\n" +
                          "• Live file reload\n" +
                          "• Syntax highlighting\n" +
                          "• Math formulas (KaTeX)\n" +
                          "• Mermaid diagrams\n" +
                          "• PlantUML diagrams\n" +
                          "• 4 Themes\n" +
                          "• 8 Languages\n\n" +
                          "GitHub: github.com/nobiehl/mini-markdown-viewer";

            MessageBox.Show(help, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log.Information("MainForm disposing");
                _fileWatcher?.Dispose();
                Log.CloseAndFlush();
            }
            base.Dispose(disposing);
        }
    }
}
