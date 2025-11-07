namespace MarkdownViewer.Views
{
    /// <summary>
    /// View interface for StatusBarControl.
    /// Abstracts status bar interactions for testability.
    /// </summary>
    public interface IStatusBarView
    {
        // Properties
        string CurrentTheme { get; set; }
        string CurrentLanguage { get; set; }
        bool Visible { get; set; }

        // Events (View -> Presenter)
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;
        event EventHandler<LanguageChangedEventArgs> LanguageChanged;

        // View Actions (Presenter -> View)
        void UpdateAvailableThemes(IEnumerable<string> themeNames);
        void UpdateAvailableLanguages(IEnumerable<string> languageCodes);
        void SetStatus(string message);
        void SetLineInfo(int lineNumber, int columnNumber);
    }
}
