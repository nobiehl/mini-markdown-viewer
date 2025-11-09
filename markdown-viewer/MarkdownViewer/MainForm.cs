using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Serilog;
using Serilog.Events;
using MarkdownViewer.Core;
using MarkdownViewer.Core.Models;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Core.Views;
using MarkdownViewer.Core.Presenters;
using MarkdownViewer.Services;
using MarkdownViewer.UI;

namespace MarkdownViewer
{
    /// <summary>
    /// Main window form for displaying Markdown files.
    /// Refactored in v1.2.0 for layered architecture with services.
    /// Enhanced in v1.3.0 with localization and status bar.
    /// Enhanced in v1.4.0 with navigation and search.
    /// Finalized in v1.5.0 with polish, testing, and documentation.
    /// Refactored in v2.0.0 for MVP pattern with full UI testability.
    /// Uses WebView2 for HTML rendering with theme support.
    /// Implements IMainView for presenter communication.
    /// </summary>
    public class MainForm : Form, IMainView
    {
        private const string Version = "1.7.4";

        // UI Components
        private WebView2 _webView = null!;
        private StatusBarControl? _statusBar;
        private NavigationBar? _navigationBar;
        private SearchBar? _searchBar;
        private ContextMenuStrip? _themeContextMenu;

        // Core Services
        private readonly NavigationManager _navigationManager;
        private readonly SearchManager _searchManager;
        private readonly FileWatcherManager _fileWatcher;
        private readonly MarkdownRenderer _renderer;

        // Application Services
        private readonly LocalizationService _localizationService;
        private readonly SettingsService _settingsService;
        private readonly ThemeService _themeService;

        // State
        private string _currentFilePath = null!;
        private AppSettings _settings = null!;
        private Theme? _currentTheme;

        #region IMainView Implementation

        // IMainView Properties
        public string CurrentFilePath
        {
            get => _currentFilePath;
            set => _currentFilePath = value;
        }

        public string WindowTitle
        {
            get => this.Text;
            set => this.Text = value;
        }

        public bool IsNavigationBarVisible
        {
            get => _navigationBar?.Visible ?? false;
            set
            {
                if (_navigationBar != null)
                    _navigationBar.Visible = value;
            }
        }

        public bool IsSearchBarVisible
        {
            get => _searchBar?.Visible ?? false;
            set
            {
                if (_searchBar != null)
                    _searchBar.Visible = value;
            }
        }

        // IMainView Events (View -> Presenter)
        public event EventHandler? ViewLoaded;
        public event EventHandler<ThemeChangedEventArgs>? ThemeChangeRequested;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChangeRequested;
        public event EventHandler? RefreshRequested;
        public event EventHandler? SearchRequested;
        public event EventHandler? NavigateBackRequested;
        public event EventHandler? NavigateForwardRequested;

        // IMainView Methods (Presenter -> View)
        public void DisplayMarkdown(string html)
        {
            if (_webView.CoreWebView2 != null)
            {
                _webView.CoreWebView2.NavigateToString(html);
            }
        }

        public void ShowError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowInfo(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowSuccess(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void UpdateTheme(Theme theme)
        {
            _currentTheme = theme;
            ApplyThemeToUI(); // Apply theme to WinForms UI
        }

        public void SetNavigationState(bool canGoBack, bool canGoForward)
        {
            // TODO: Update navigation bar state when NavigationBar implements INavigationBarView
            // _navigationBar?.UpdateNavigationState(canGoBack, canGoForward);
        }

        public void SetSearchResults(int currentMatch, int totalMatches)
        {
            // TODO: Update search results when SearchBar implements ISearchBarView
            // _searchBar?.UpdateResults(currentMatch, totalMatches);
        }

        public void ShowSearchBar()
        {
            if (_searchBar != null)
            {
                _searchBar.Visible = true;
                _searchBar.Show(); // Use existing Show() method
            }
        }

        public void HideSearchBar()
        {
            if (_searchBar != null)
            {
                _searchBar.Visible = false;
            }
        }

        #endregion

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

            // Initialize application services
            _settingsService = new SettingsService();
            _themeService = new ThemeService();
            _renderer = new MarkdownRenderer();
            _fileWatcher = new FileWatcherManager();

            // Load settings and theme
            LoadSettings();

            // Initialize localization service with language from settings
            _localizationService = new LocalizationService(_settings.Language ?? "en");

            // Initialize UI components
            InitializeComponents();

            // Apply theme to UI immediately (after form is initialized)
            ApplyThemeToUI();

            // Initialize core managers (require WebView2)
            _navigationManager = new NavigationManager(_webView!);
            _searchManager = new SearchManager(_webView!);

            // Initialize NavigationBar if enabled
            if (_settings.UI.NavigationBar.Visible)
            {
                InitializeNavigationBar();
            }

            // Initialize SearchBar (always available via Ctrl+F, hidden by default)
            InitializeSearchBar();

            // Initialize StatusBar (always visible)
            InitializeStatusBar();

            // Initialize Theme Context Menu
            InitializeThemeContextMenu();

            // Load initial file
            _currentFilePath = filePath;
            LoadMarkdownFile(filePath);

            // Setup file watching for live reload
            SetupFileWatcher(filePath);

            // Wire up Form Load event to trigger ViewLoaded
            this.Load += OnFormLoad;
        }

        /// <summary>
        /// Handles form load event and triggers ViewLoaded for presenter.
        /// </summary>
        private void OnFormLoad(object? sender, EventArgs e)
        {
            Log.Debug("Form loaded, triggering ViewLoaded event");
            ViewLoaded?.Invoke(this, EventArgs.Empty);
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
                string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;
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
            this.KeyPreview = true; // CRITICAL: Enable keyboard shortcuts even when WebView2 has focus

            // WebView2 setup
            _webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(_webView);

            // Setup WebView2 event handlers
            _webView.CoreWebView2InitializationCompleted += OnWebView2Initialized;

            // Initialize WebView2 asynchronously with separate user data folder
            string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? Environment.CurrentDirectory;
            string userDataFolder = Path.Combine(exeFolder, ".cache");
            Directory.CreateDirectory(userDataFolder);

            InitializeWebView2Async(userDataFolder);
        }

        /// <summary>
        /// Initializes WebView2 asynchronously with a separate user data folder.
        /// </summary>
        private async void InitializeWebView2Async(string userDataFolder)
        {
            try
            {
                Log.Debug("Starting WebView2 initialization with user data folder: {UserDataFolder}", userDataFolder);
                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                    browserExecutableFolder: null,
                    userDataFolder: userDataFolder
                );
                await _webView!.EnsureCoreWebView2Async(environment);
                Log.Debug("WebView2 environment created successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize WebView2 environment");
                MessageBox.Show(
                    $"Failed to initialize WebView2:\n{ex.Message}\n\nPlease ensure Microsoft Edge WebView2 Runtime is installed.",
                    "Initialization Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void OnWebView2Initialized(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
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

        private async void ApplyThemeToUI()
        {
            try
            {
                Log.Information("Applying theme to Form: {ThemeName}", _currentTheme?.Name ?? "null");

                // Apply theme to WinForms
                if (_currentTheme == null)
                {
                    Log.Warning("Theme is null, cannot apply theme to UI");
                    return;
                }

                this.BackColor = System.Drawing.ColorTranslator.FromHtml(_currentTheme.UI.FormBackground);

                // Apply theme to WebView2 background (visible while loading)
                if (_webView != null)
                {
                    _webView.BackColor = System.Drawing.ColorTranslator.FromHtml(_currentTheme.UI.FormBackground);
                }

                // Apply theme to all UI components (StatusBar, etc.) using ThemeService
                if (_webView != null)
                {
                    await _themeService.ApplyThemeAsync(_currentTheme, this, _webView);
                }

                Log.Information("Theme applied to Form successfully: BackColor={BackColor}", _currentTheme.UI.FormBackground);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to apply theme to UI");
            }
        }

        private void OnNavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
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
                    Log.Information("HandleLocalFileNavigation: File exists | Path: {FilePath} | File size: {Size} bytes",
                        localPath, new FileInfo(localPath).Length);
                    this.BeginInvoke(new Action(() =>
                    {
                        _currentFilePath = localPath;
                        LoadMarkdownFile(localPath);
                        SetupFileWatcher(localPath);
                        Log.Information("Successfully navigated to file: {FilePath}", localPath);

                        if (!string.IsNullOrEmpty(fragment))
                        {
                            Log.Information("Scheduling anchor scroll after navigation | Fragment: {Fragment}", fragment);
                            System.Threading.Tasks.Task.Delay(200).ContinueWith(_ =>
                            {
                                this.Invoke(new Action(() => ScrollToAnchor(fragment)));
                            });
                        }
                    }));
                }
                else
                {
                    Log.Warning("HandleLocalFileNavigation: File not found | Path: {FilePath} | Navigation aborted", localPath);
                }
            }
        }

        private void ScrollToAnchor(string fragment)
        {
            string anchorId = fragment.TrimStart('#');
            Log.Information("ScrollToAnchor: Scrolling to anchor | Fragment: {Fragment} | Anchor ID: {AnchorId}", fragment, anchorId);

            string script = $"document.getElementById('{anchorId}')?.scrollIntoView({{behavior: 'smooth'}}) || " +
                           $"document.querySelector('a[name=\"{anchorId}\"]')?.scrollIntoView({{behavior: 'smooth'}});";

            _webView.ExecuteScriptAsync(script);
            Log.Debug("ScrollToAnchor: JavaScript executed for anchor: {AnchorId}", anchorId);
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
            Log.Debug("HandleLinkClick called with URL: {Url}", url);

            if (string.IsNullOrWhiteSpace(url))
            {
                Log.Warning("HandleLinkClick called with null or empty URL");
                return;
            }

            // Handle external HTTP/HTTPS links
            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                if (!IsInlineResource(url))
                {
                    Log.Information("Link type: External HTTP/HTTPS | URL: {Url}", url);
                    try
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = url,
                            UseShellExecute = true
                        });
                        Log.Information("Successfully opened external link in browser: {Url}", url);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Failed to open external link: {Url}", url);
                    }
                }
                else
                {
                    Log.Debug("Skipping inline resource: {Url}", url);
                }
                return;
            }

            // Handle anchor-only links (should be handled by JavaScript, but log if we get them)
            if (url.StartsWith("#"))
            {
                Log.Information("Link type: Anchor | Fragment: {Fragment}", url);
                ScrollToAnchor(url);
                return;
            }

            // Handle local file links (.md or .markdown)
            if (url.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                url.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
            {
                Log.Information("Link type: Local Markdown File | Original path: {Path}", url);

                // Resolve relative paths relative to the current file's directory
                string resolvedPath = url;

                // Check if path is relative (not rooted)
                if (!Path.IsPathRooted(url))
                {
                    string currentFileDirectory = Path.GetDirectoryName(_currentFilePath) ?? Environment.CurrentDirectory;
                    resolvedPath = Path.GetFullPath(Path.Combine(currentFileDirectory, url));
                    Log.Information("Path resolution: Relative path detected | Base directory: {BaseDir} | Resolved to: {ResolvedPath}",
                        currentFileDirectory, resolvedPath);
                }
                else
                {
                    resolvedPath = Path.GetFullPath(url);
                    Log.Information("Path resolution: Absolute path detected | Resolved to: {ResolvedPath}", resolvedPath);
                }

                // Validate file existence BEFORE attempting navigation
                if (!File.Exists(resolvedPath))
                {
                    Log.Warning("File not found: {ResolvedPath} | Original link: {OriginalPath} | Navigation aborted",
                        resolvedPath, url);
                    return;
                }

                Log.Information("File exists: {ResolvedPath} | File size: {Size} bytes",
                    resolvedPath, new FileInfo(resolvedPath).Length);

                // Navigate to the file
                try
                {
                    _currentFilePath = resolvedPath;
                    LoadMarkdownFile(resolvedPath);
                    SetupFileWatcher(resolvedPath);
                    Log.Information("Successfully navigated to local file: {ResolvedPath}", resolvedPath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to navigate to local file: {ResolvedPath}", resolvedPath);
                }
            }
            else
            {
                Log.Debug("Unhandled link type or extension: {Url}", url);
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
                            _webView.CoreWebView2!.NavigateToString(html);
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

        private void OnFileChanged(object? sender, string filePath)
        {
            // File changed - reload (already on UI thread via Invoke in FileWatcherManager)
            this.Invoke(new Action(() =>
            {
                Log.Information("File changed, reloading: {FilePath}", filePath);
                LoadMarkdownFile(_currentFilePath);
            }));
        }

        /// <summary>
        /// Initializes and configures the navigation bar.
        /// </summary>
        private void InitializeNavigationBar()
        {
            Log.Debug("Initializing NavigationBar");

            try
            {
                _navigationBar = new NavigationBar(_localizationService);

                // Wire up navigation bar events (MVP pattern)
                _navigationBar.BackRequested += (s, e) => _navigationManager?.GoBack();
                _navigationBar.ForwardRequested += (s, e) => _navigationManager?.GoForward();

                // Subscribe to navigation manager state changes
                if (_navigationManager != null)
                {
                    _navigationManager.NavigationChanged += (s, e) =>
                    {
                        _navigationBar.UpdateNavigationState(
                            _navigationManager.CanGoBack,
                            _navigationManager.CanGoForward);
                    };
                }

                // Add to form (docked at top)
                this.Controls.Add(_navigationBar.Control);

                Log.Information("NavigationBar initialized successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize NavigationBar");
            }
        }

        /// <summary>
        /// Initializes and configures the search bar.
        /// </summary>
        private void InitializeSearchBar()
        {
            Log.Debug("Initializing SearchBar");

            try
            {
                _searchBar = new SearchBar(_localizationService);

                // Wire up search bar events (MVP pattern)
                _searchBar.SearchRequested += async (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(e.SearchText))
                    {
                        await _searchManager!.ClearSearchAsync();
                    }
                    else
                    {
                        await _searchManager!.SearchAsync(e.SearchText);
                    }
                };

                _searchBar.FindNextRequested += async (s, e) => await _searchManager!.NextMatchAsync();
                _searchBar.FindPreviousRequested += async (s, e) => await _searchManager!.PreviousMatchAsync();
                _searchBar.CloseRequested += (s, e) =>
                {
                    _searchBar?.ClearSearch();
                    _ = _searchManager?.ClearSearchAsync();
                };

                // Subscribe to search manager results
                if (_searchManager != null)
                {
                    _searchManager.SearchResultsChanged += (s, e) =>
                    {
                        _searchBar?.UpdateResults(e.CurrentMatch, e.TotalMatches);
                    };
                }

                // Add to form (docked at top, hidden by default)
                this.Controls.Add(_searchBar.Control);

                Log.Information("SearchBar initialized successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize SearchBar");
            }
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
                _statusBar.ThemeChanged += OnThemeChanged;  // NEW
                _statusBar.UpdateClicked += OnUpdateClicked;
                _statusBar.ExplorerClicked += OnExplorerClicked;
                _statusBar.InfoClicked += OnInfoClicked;
                _statusBar.HelpClicked += OnHelpClicked;

                // Add to form
                this.Controls.Add(_statusBar);

                // Initialize theme dropdown with available themes
                var availableThemes = _themeService.GetAvailableThemes();
                var currentTheme = _currentTheme?.Name ?? "standard";
                _statusBar.PopulateThemeDropdown(availableThemes, currentTheme);

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
        private void OnLanguageChanged(object? sender, LanguageChangedEventArgs e)
        {
            var newLanguage = e.LanguageCode;
            Log.Information("Language changed to: {Language}", newLanguage);

            // Trigger IMainView event
            LanguageChangeRequested?.Invoke(this, new LanguageChangedEventArgs(newLanguage));

            try
            {
                // Update settings with new language
                _settings.Language = newLanguage;
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
        /// Handles theme change from status bar.
        /// Loads and applies the new theme to UI and Markdown rendering.
        /// </summary>
        private async void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            var newThemeName = e.ThemeName;
            Log.Information("Theme changed to: {Theme}", newThemeName);

            // Trigger IMainView event
            ThemeChangeRequested?.Invoke(this, new ThemeChangedEventArgs(newThemeName));

            try
            {
                // Load the new theme
                var newTheme = _themeService.LoadTheme(newThemeName);
                _currentTheme = newTheme;

                // Update settings with new theme
                _settings.Theme = newThemeName;
                _settingsService.Save(_settings);

                // Apply theme to UI and WebView2
                await _themeService.ApplyThemeAsync(newTheme, this, _webView);

                Log.Information("Theme applied successfully: {Theme}", newThemeName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply theme: {Theme}", newThemeName);
                MessageBox.Show(
                    $"Failed to apply theme '{newThemeName}':\n{ex.Message}",
                    "Theme Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles info button click in status bar.
        /// Shows application info dialog.
        /// </summary>
        private void OnInfoClicked(object? sender, EventArgs e)
        {
            Log.Debug("Info button clicked");

            string appInfo = BuildApplicationInfoMarkdown();

            // Show in MarkdownDialog for beautiful formatting
            using var dialog = new MarkdownDialog("About Markdown Viewer", appInfo, _renderer);
            dialog.ShowDialog(this);
        }

        /// <summary>
        /// Builds comprehensive application information in Markdown format.
        /// This will be displayed in the MarkdownDialog once available.
        /// </summary>
        private string BuildApplicationInfoMarkdown()
        {
            string markdown = $@"# Markdown Viewer v{Version}

## About

A lightweight, fast Windows application for viewing Markdown files with live preview and advanced rendering capabilities.

## Features

- **Live File Reload** - Automatically updates when file changes
- **Syntax Highlighting** - Code blocks with language-specific highlighting
- **Math Support** - Render mathematical formulas using KaTeX
- **Mermaid Diagrams** - Flowcharts, sequence diagrams, and more
- **PlantUML Support** - UML diagrams and technical drawings
- **Multiple Themes** - Choose from 4 beautiful themes (Dark, Standard, Solarized, Dräger)
- **Multi-Language** - Supports 8 languages (EN, DE, MN, FR, ES, JA, ZH, RU)
- **Navigation** - Back/forward navigation through document history
- **Search** - Find text within documents (Ctrl+F)
- **Windows Integration** - Double-click .md files, context menu, Send To

## Current Configuration

- **Version**: {Version}
- **Language**: {_localizationService.GetCurrentLanguage().ToUpper()}
- **Theme**: {char.ToUpper(_settings.Theme[0]) + _settings.Theme.Substring(1)}
- **Settings**: `{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\MarkdownViewer\settings.json`

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `F5` | Reload current file |
| `Ctrl+F` | Open search |
| `F3` | Next search result |
| `Shift+F3` | Previous search result |
| `Alt+Left` | Navigate back |
| `Alt+Right` | Navigate forward |
| `Ctrl+Mouse Wheel` | Zoom in/out |
| `Esc` | Close search |

## Credits

Developed with:
- **Markdig** - Markdown parsing and rendering
- **KaTeX** - Mathematical formula rendering
- **Mermaid** - Diagram generation
- **Highlight.js** - Syntax highlighting for code blocks
- **WebView2** - Modern web rendering engine
- **Feather Icons** - Beautiful UI icons

## Links

- **GitHub Repository**: [github.com/nobiehl/mini-markdown-viewer](https://github.com/nobiehl/mini-markdown-viewer)
- **Report Issues**: [github.com/nobiehl/mini-markdown-viewer/issues](https://github.com/nobiehl/mini-markdown-viewer/issues)
- **Documentation**: View README.md on GitHub

---

*Built with care for the Markdown community*
";
            return markdown;
        }

        /// <summary>
        /// Converts Markdown to plain text for MessageBox display.
        /// This is a temporary helper until MarkdownDialog is available.
        /// </summary>
        private string ConvertMarkdownToPlainText(string markdown)
        {
            // Simple conversion: remove markdown formatting for MessageBox
            string text = markdown
                .Replace("# ", "")
                .Replace("## ", "")
                .Replace("**", "")
                .Replace("- ", "• ")
                .Replace("`", "\"")
                .Replace("---", "")
                .Replace("*", "");

            return text.Trim();
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
                          "• Ctrl+F - Open search\n" +
                          "• F3 / Shift+F3 - Next/Previous match\n" +
                          "• Alt+Left / Alt+Right - Navigate back/forward\n" +
                          "• Ctrl+Mouse Wheel - Zoom in/out\n" +
                          "• Right-click - Theme menu\n\n" +
                          "Features:\n" +
                          "• Live file reload\n" +
                          "• Syntax highlighting\n" +
                          "• Math formulas (KaTeX)\n" +
                          "• Mermaid diagrams\n" +
                          "• PlantUML diagrams\n" +
                          "• 4 Themes\n" +
                          "• 8 Languages\n" +
                          "• Navigation & Search\n" +
                          "• StatusBar (always visible)\n\n" +
                          "GitHub: github.com/nobiehl/mini-markdown-viewer";

            MessageBox.Show(help, "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Handles update status icon click in status bar.
        /// Triggers manual update check.
        /// </summary>
        private async void OnUpdateClicked(object? sender, EventArgs e)
        {
            Log.Debug("Update icon clicked");

            try
            {
                // Show checking status
                _statusBar?.SetUpdateStatus(UpdateStatus.Checking);

                // Check for updates
                var checker = new UpdateChecker();
                var updateInfo = await checker.CheckForUpdatesAsync(Version);

                if (updateInfo.UpdateAvailable)
                {
                    _statusBar?.SetUpdateStatus(UpdateStatus.UpdateAvailable, updateInfo.LatestVersion);

                    // Show release notes in MarkdownDialog
                    string releaseNotesMarkdown = $@"# Update Available: v{updateInfo.LatestVersion}

**Current version:** v{Version}

## Release Notes

{updateInfo.ReleaseNotes}

---

**Would you like to download and install this update now?**";

                    MarkdownDialog.ShowDialog(this, "Update Available", releaseNotesMarkdown, _renderer);

                    // Ask for confirmation
                    var result = MessageBox.Show(
                        "Download and install now?",
                        "Update Available",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        // Download update
                        var success = await checker.DownloadUpdateAsync(updateInfo.DownloadUrl);

                        if (success)
                        {
                            MessageBox.Show(
                                "Update downloaded successfully!\n\n" +
                                "The update will be installed when you restart the application.",
                                "Update Downloaded",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
                        }
                        else
                        {
                            _statusBar?.SetUpdateStatus(UpdateStatus.Error);
                            MessageBox.Show(
                                "Failed to download update.\n\nPlease try again later or download manually from GitHub.",
                                "Download Failed",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                    }
                }
                else
                {
                    _statusBar?.SetUpdateStatus(UpdateStatus.UpToDate);
                    MessageBox.Show(
                        $"You are running the latest version (v{Version}).",
                        "Up to Date",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to check for updates");
                _statusBar?.SetUpdateStatus(UpdateStatus.Error);
                MessageBox.Show(
                    $"Failed to check for updates:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles explorer status icon click in status bar.
        /// Shows install/uninstall prompt.
        /// </summary>
        private void OnExplorerClicked(object? sender, EventArgs e)
        {
            Log.Debug("Explorer icon clicked");

            try
            {
                if (_statusBar?.IsExplorerRegistered == true)
                {
                    // Already registered - offer to uninstall
                    var result = MessageBox.Show(
                        "Windows Explorer integration is currently installed.\n\n" +
                        "This includes:\n" +
                        "• Double-click .md files to open in MarkdownViewer\n" +
                        "• Right-click context menu\n" +
                        "• 'Send To' menu integration\n\n" +
                        "Would you like to uninstall the integration?",
                        "Uninstall Explorer Integration",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Program.UninstallFileAssociation();
                        _statusBar?.CheckExplorerRegistration();

                        MessageBox.Show(
                            "Windows Explorer integration has been removed successfully.",
                            "Uninstall Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
                else
                {
                    // Not registered - offer to install
                    var result = MessageBox.Show(
                        "Windows Explorer integration is not currently installed.\n\n" +
                        "Installing will enable:\n" +
                        "• Double-click .md files to open in MarkdownViewer\n" +
                        "• Right-click context menu\n" +
                        "• 'Send To' menu integration\n\n" +
                        "No admin rights required (uses HKEY_CURRENT_USER).\n\n" +
                        "Would you like to install the integration?",
                        "Install Explorer Integration",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        Program.InstallFileAssociation();
                        _statusBar?.CheckExplorerRegistration();

                        MessageBox.Show(
                            "Windows Explorer integration has been installed successfully!\n\n" +
                            "You can now double-click .md files to open them in MarkdownViewer.",
                            "Install Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to handle explorer registration");
                MessageBox.Show(
                    $"Failed to update Explorer integration:\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Initializes the theme context menu.
        /// Allows right-click theme switching.
        /// </summary>
        private void InitializeThemeContextMenu()
        {
            Log.Debug("Initializing Theme Context Menu");

            try
            {
                _themeContextMenu = new ContextMenuStrip();

                // Get available themes
                var themes = _themeService.GetAvailableThemes();

                // Create menu items for each theme
                foreach (string themeId in themes)
                {
                    string displayName = GetThemeDisplayName(themeId);
                    var menuItem = new ToolStripMenuItem(displayName)
                    {
                        Tag = themeId,
                        Checked = themeId == _settings.Theme
                    };

                    menuItem.Click += OnThemeSelected;
                    _themeContextMenu.Items.Add(menuItem);
                }

                // Assign context menu to the form
                this.ContextMenuStrip = _themeContextMenu;

                Log.Information("Theme Context Menu initialized with {Count} themes", themes.Count);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize Theme Context Menu");
            }
        }

        /// <summary>
        /// Gets the localized display name for a theme.
        /// </summary>
        private string GetThemeDisplayName(string themeId)
        {
            return themeId.ToLower() switch
            {
                "dark" => _localizationService.GetString("ThemeDark"),
                "solarized" => _localizationService.GetString("ThemeSolarized"),
                "draeger" => _localizationService.GetString("ThemeDraeger"),
                "standard" => _localizationService.GetString("ThemeStandard"),
                _ => themeId
            };
        }

        /// <summary>
        /// Handles theme selection from context menu.
        /// Applies the selected theme and reloads the current file.
        /// </summary>
        private void OnThemeSelected(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is string themeId)
            {
                Log.Information("Theme changed to: {Theme}", themeId);

                try
                {
                    // Update current theme
                    _currentTheme = _themeService.LoadTheme(themeId);

                    // Update settings and save
                    _settings.Theme = themeId;
                    _settingsService.Save(_settings);

                    // Update menu item checked states
                    if (_themeContextMenu != null)
                    {
                        foreach (ToolStripMenuItem item in _themeContextMenu.Items)
                        {
                            item.Checked = item.Tag as string == themeId;
                        }
                    }

                    // Apply theme to UI
                    ApplyThemeToUI();

                    // Reload current file to apply theme to markdown
                    LoadMarkdownFile(_currentFilePath);

                    Log.Information("Theme applied successfully: {Theme}", themeId);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to apply theme: {Theme}", themeId);
                    MessageBox.Show($"Failed to apply theme: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Handles keyboard shortcuts for navigation and search.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Alt+Left: Navigate back
            if (keyData == (Keys.Alt | Keys.Left))
            {
                Log.Debug("Alt+Left pressed");
                NavigateBackRequested?.Invoke(this, EventArgs.Empty); // Trigger IMainView event
                _navigationManager.GoBack();
                return true;
            }

            // Alt+Right: Navigate forward
            if (keyData == (Keys.Alt | Keys.Right))
            {
                Log.Debug("Alt+Right pressed");
                NavigateForwardRequested?.Invoke(this, EventArgs.Empty); // Trigger IMainView event
                _navigationManager.GoForward();
                return true;
            }

            // Ctrl+F: Open search
            if (keyData == (Keys.Control | Keys.F))
            {
                Log.Debug("Ctrl+F pressed");
                SearchRequested?.Invoke(this, EventArgs.Empty); // Trigger IMainView event
                _searchBar?.Show();
                return true;
            }

            // F5: Refresh
            if (keyData == Keys.F5)
            {
                Log.Debug("F5 pressed");
                RefreshRequested?.Invoke(this, EventArgs.Empty); // Trigger IMainView event
                LoadMarkdownFile(_currentFilePath); // Reload current file
                return true;
            }

            // F3: Next search result
            if (keyData == Keys.F3)
            {
                Log.Debug("F3 pressed");
                _ = _searchManager.NextMatchAsync();
                return true;
            }

            // Shift+F3: Previous search result
            if (keyData == (Keys.Shift | Keys.F3))
            {
                Log.Debug("Shift+F3 pressed");
                _ = _searchManager.PreviousMatchAsync();
                return true;
            }

            // Esc: Close search (handled in SearchBar.OnSearchTextBoxKeyDown)

            return base.ProcessCmdKey(ref msg, keyData);
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
