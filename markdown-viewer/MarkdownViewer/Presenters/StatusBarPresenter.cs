using MarkdownViewer.Services;
using MarkdownViewer.Views;
using Serilog;

namespace MarkdownViewer.Presenters
{
    /// <summary>
    /// Presenter for StatusBarView.
    /// Handles theme and language selection in the status bar.
    /// </summary>
    public class StatusBarPresenter
    {
        private readonly IStatusBarView _view;
        private readonly IThemeService _themeService;
        private readonly ILocalizationService _localizationService;

        public StatusBarPresenter(
            IStatusBarView view,
            IThemeService themeService,
            ILocalizationService localizationService)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _themeService = themeService ?? throw new ArgumentNullException(nameof(themeService));
            _localizationService = localizationService ?? throw new ArgumentNullException(nameof(localizationService));

            // Subscribe to view events
            _view.ThemeChanged += OnThemeChanged;
            _view.LanguageChanged += OnLanguageChanged;

            Log.Debug("StatusBarPresenter created");
        }

        /// <summary>
        /// Initializes the status bar with available themes and languages.
        /// </summary>
        public void Initialize()
        {
            try
            {
                // Get available themes
                var themes = _themeService.GetAvailableThemes();
                _view.UpdateAvailableThemes(themes);

                // Get available languages
                var languages = _localizationService.GetSupportedLanguages();
                _view.UpdateAvailableLanguages(languages);

                Log.Debug("StatusBar initialized with {ThemeCount} themes and {LanguageCount} languages",
                    themes.Count(), languages.Count());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error initializing StatusBar");
            }
        }

        private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
        {
            Log.Debug("Theme changed in status bar: {Theme}", e.ThemeName);
            // This event will be bubbled up to MainPresenter via MainView
        }

        private void OnLanguageChanged(object? sender, LanguageChangedEventArgs e)
        {
            Log.Debug("Language changed in status bar: {Language}", e.LanguageCode);
            // This event will be bubbled up to MainPresenter via MainView
        }

        /// <summary>
        /// Updates the status message.
        /// </summary>
        public void SetStatus(string message)
        {
            _view.SetStatus(message);
        }

        /// <summary>
        /// Updates the line and column information.
        /// </summary>
        public void SetLineInfo(int lineNumber, int columnNumber)
        {
            _view.SetLineInfo(lineNumber, columnNumber);
        }
    }
}
