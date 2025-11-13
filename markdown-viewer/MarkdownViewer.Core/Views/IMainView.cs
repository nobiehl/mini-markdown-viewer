using MarkdownViewer.Core.Models;

namespace MarkdownViewer.Core.Views
{
    /// <summary>
    /// View interface for MainForm.
    /// Abstracts all UI interactions for testability.
    /// </summary>
    public interface IMainView
    {
        // Properties
        string CurrentFilePath { get; set; }
        string WindowTitle { get; set; }
        bool IsNavigationBarVisible { get; set; }
        bool IsSearchBarVisible { get; set; }

        // Events (View -> Presenter)
        event EventHandler ViewLoaded;
        event EventHandler<ThemeChangedEventArgs> ThemeChangeRequested;
        event EventHandler<LanguageChangedEventArgs> LanguageChangeRequested;
        event EventHandler RefreshRequested;
        event EventHandler SearchRequested;
        event EventHandler NavigateBackRequested;
        event EventHandler NavigateForwardRequested;

        // View Actions (Presenter -> View)
        void DisplayMarkdown(string html);
        void ShowError(string message, string title);
        void ShowInfo(string message, string title);
        void ShowSuccess(string message, string title);
        void UpdateTheme(Theme theme);
        void SetNavigationState(bool canGoBack, bool canGoForward);
        void SetSearchResults(int currentMatch, int totalMatches);
        void ShowSearchBar();
        void HideSearchBar();
        void Close();
    }

    /// <summary>
    /// Event arguments for theme change requests.
    /// </summary>
    public class ThemeChangedEventArgs : EventArgs
    {
        public string ThemeName { get; }
        public ThemeChangedEventArgs(string themeName) => ThemeName = themeName;
    }

    /// <summary>
    /// Event arguments for language change requests.
    /// </summary>
    public class LanguageChangedEventArgs : EventArgs
    {
        public string LanguageCode { get; }
        public LanguageChangedEventArgs(string languageCode) => LanguageCode = languageCode;
    }
}
