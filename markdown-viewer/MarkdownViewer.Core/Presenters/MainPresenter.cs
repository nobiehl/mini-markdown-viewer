using MarkdownViewer.Core;
using MarkdownViewer.Core.Models;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Core.Views;
using Serilog;

namespace MarkdownViewer.Core.Presenters
{
    /// <summary>
    /// Presenter for MainView.
    /// Contains all testable business logic for the main window.
    /// Follows the MVP (Model-View-Presenter) pattern.
    /// </summary>
    public class MainPresenter
    {
        private readonly IMainView _view;
        private readonly IWebViewAdapter _webView;
        private readonly ISettingsService _settingsService;
        private readonly IThemeService _themeService;
        private readonly ILocalizationService _localizationService;
        private readonly IDialogService _dialogService;
        private readonly MarkdownRenderer _renderer;
        private readonly FileWatcherManager _fileWatcher;

        private AppSettings _settings;
        private Theme _currentTheme;
        private string _currentFilePath;

        public MainPresenter(
            IMainView view,
            IWebViewAdapter webView,
            ISettingsService settingsService,
            IThemeService themeService,
            ILocalizationService localizationService,
            IDialogService dialogService,
            MarkdownRenderer renderer,
            FileWatcherManager fileWatcher)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));
            _dialogService = dialogService ?? throw new ArgumentNullException(nameof(dialogService));
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));
            _fileWatcher = fileWatcher ?? throw new ArgumentNullException(nameof(fileWatcher));

            _settings = new AppSettings();
            _currentTheme = new Theme();
            _currentFilePath = string.Empty;

            // Subscribe to view events
            _view.ViewLoaded += OnViewLoaded;
            _view.ThemeChangeRequested += OnThemeChangeRequested;
            _view.LanguageChangeRequested += OnLanguageChangeRequested;
            _view.FileLoadRequested += OnFileLoadRequested;
            _view.RefreshRequested += OnRefreshRequested;
            _view.SearchRequested += OnSearchRequested;
            _view.NavigateBackRequested += OnNavigateBackRequested;
            _view.NavigateForwardRequested += OnNavigateForwardRequested;

            // Subscribe to WebView events
            _webView.Initialized += OnWebViewInitialized;
            _webView.NavigationStarting += OnNavigationStarting;

            // Subscribe to file watcher
            _fileWatcher.FileChanged += OnFileChanged;

            Log.Debug("MainPresenter created");
        }

        /// <summary>
        /// Initializes the presenter with the initial file path.
        /// </summary>
        public void Initialize(string initialFilePath)
        {
            _currentFilePath = initialFilePath;
            Log.Information("MainPresenter initialized with file: {FilePath}", initialFilePath);
        }

        #region View Event Handlers

        private void OnViewLoaded(object? sender, EventArgs e)
        {
            try
            {
                Log.Information("View loaded, initializing...");

                // Load settings
                _settings = _settingsService.Load();
                _currentTheme = _themeService.LoadTheme(_settings.Theme);
                _localizationService.SetLanguage(_settings.Language);

                // Apply initial theme to view
                _view.UpdateTheme(_currentTheme);

                // Apply UI settings
                _view.IsNavigationBarVisible = _settings.UI.NavigationBar.Visible;
                _view.IsSearchBarVisible = false; // Initially hidden

                Log.Information("View initialization complete. Theme: {Theme}, Language: {Language}",
                    _settings.Theme, _settings.Language);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during view initialization");
                _dialogService.ShowError(
                    $"Error during initialization:\n{ex.Message}",
                    "Initialization Error");
            }
        }

        private async void OnThemeChangeRequested(object? sender, ThemeChangedEventArgs e)
        {
            try
            {
                Log.Information("Theme change requested: {Theme}", e.ThemeName);

                // Load new theme
                var newTheme = _themeService.LoadTheme(e.ThemeName);
                _currentTheme = newTheme;

                // Update settings
                _settings.Theme = e.ThemeName;
                _settingsService.Save(_settings);

                // Apply to view
                _view.UpdateTheme(newTheme);

                // Reload current file to apply theme to markdown
                if (!string.IsNullOrEmpty(_currentFilePath))
                {
                    await LoadMarkdownFileAsync(_currentFilePath);
                }

                Log.Information("Theme applied successfully: {Theme}", e.ThemeName);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply theme: {Theme}", e.ThemeName);
                _dialogService.ShowError(
                    $"Failed to apply theme '{e.ThemeName}':\n{ex.Message}",
                    "Theme Error");
            }
        }

        private void OnLanguageChangeRequested(object? sender, LanguageChangedEventArgs e)
        {
            try
            {
                Log.Information("Language change requested: {Language}", e.LanguageCode);

                _localizationService.SetLanguage(e.LanguageCode);

                // Update settings
                _settings.Language = e.LanguageCode;
                _settingsService.Save(_settings);

                // Note: View should refresh its UI text by calling localization service
                Log.Information("Language changed successfully: {Language}", e.LanguageCode);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to change language: {Language}", e.LanguageCode);
                _dialogService.ShowError(
                    $"Failed to change language: {ex.Message}",
                    "Language Error");
            }
        }

        private async void OnFileLoadRequested(object? sender, string filePath)
        {
            await LoadMarkdownFileAsync(filePath);
        }

        private async void OnRefreshRequested(object? sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_currentFilePath))
            {
                Log.Information("Refresh requested for file: {FilePath}", _currentFilePath);
                await LoadMarkdownFileAsync(_currentFilePath);
            }
        }

        private void OnSearchRequested(object? sender, EventArgs e)
        {
            _view.ShowSearchBar();
            Log.Debug("Search bar shown");
        }

        private void OnNavigateBackRequested(object? sender, EventArgs e)
        {
            if (_webView.CanGoBack)
            {
                _webView.GoBack();
                UpdateNavigationState();
                Log.Debug("Navigated back");
            }
        }

        private void OnNavigateForwardRequested(object? sender, EventArgs e)
        {
            if (_webView.CanGoForward)
            {
                _webView.GoForward();
                UpdateNavigationState();
                Log.Debug("Navigated forward");
            }
        }

        #endregion

        #region WebView Event Handlers

        private async void OnWebViewInitialized(object? sender, EventArgs e)
        {
            try
            {
                Log.Information("WebView initialized");

                // Load initial file
                if (!string.IsNullOrEmpty(_currentFilePath))
                {
                    await LoadMarkdownFileAsync(_currentFilePath);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error after WebView initialization");
                _dialogService.ShowError(
                    $"Error loading initial file:\n{ex.Message}",
                    "Load Error");
            }
        }

        private void OnNavigationStarting(object? sender, NavigationStartingEventArgs e)
        {
            Log.Debug("Navigation starting: {Uri}", e.Uri);
            // Handle navigation logic here (e.g., open external links, load local files)
            // For now, we'll let the default behavior handle it
        }

        #endregion

        #region FileWatcher Event Handlers

        private void OnFileChanged(object? sender, string filePath)
        {
            Log.Information("File changed, reloading: {FilePath}", filePath);
            _ = LoadMarkdownFileAsync(_currentFilePath);
        }

        #endregion

        #region Business Logic

        /// <summary>
        /// Loads and renders a markdown file.
        /// </summary>
        private async Task LoadMarkdownFileAsync(string filePath)
        {
            try
            {
                Log.Debug("LoadMarkdownFile: {FilePath}", filePath);

                if (!File.Exists(filePath))
                {
                    Log.Warning("File not found: {FilePath}", filePath);
                    _dialogService.ShowError(
                        $"File not found: {filePath}",
                        "File Not Found");
                    return;
                }

                // Read markdown file
                string markdown = await File.ReadAllTextAsync(filePath);

                // Render to HTML with current theme
                string html = _renderer.RenderToHtml(markdown, filePath, _currentTheme);

                // Update view
                await _webView.NavigateToStringAsync(html);
                _view.WindowTitle = $"{Path.GetFileName(filePath)} - Markdown Viewer";
                _currentFilePath = filePath;

                // Setup file watching
                _fileWatcher.Watch(filePath);

                // Update navigation state
                UpdateNavigationState();

                Log.Information("File loaded successfully: {FilePath}", filePath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading file: {FilePath}", filePath);
                _dialogService.ShowError(
                    $"Error loading file:\n{ex.Message}",
                    "Load Error");
            }
        }

        /// <summary>
        /// Updates the navigation state in the view.
        /// </summary>
        private void UpdateNavigationState()
        {
            _view.SetNavigationState(_webView.CanGoBack, _webView.CanGoForward);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the current file path.
        /// </summary>
        public string CurrentFilePath => _currentFilePath;

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        public Theme CurrentTheme => _currentTheme;

        /// <summary>
        /// Gets the current settings.
        /// </summary>
        public AppSettings CurrentSettings => _settings;

        #endregion
    }
}
