using MarkdownViewer.Core.Models;
using MarkdownViewer.Core.Views;

namespace MarkdownViewer.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IMainView for testing Presenters.
    /// </summary>
    public class MockMainView : IMainView
    {
        // Properties
        public string CurrentFilePath { get; set; } = string.Empty;
        public string WindowTitle { get; set; } = string.Empty;
        public bool IsNavigationBarVisible { get; set; }
        public bool IsSearchBarVisible { get; set; }

        // Events
        public event EventHandler? ViewLoaded;
        public event EventHandler<ThemeChangedEventArgs>? ThemeChangeRequested;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChangeRequested;
#pragma warning disable CS0067
        public event EventHandler<string>? FileLoadRequested;
#pragma warning restore CS0067
        public event EventHandler? RefreshRequested;
        public event EventHandler? SearchRequested;
#pragma warning disable CS0067
        public event EventHandler? NavigateBackRequested;
        public event EventHandler? NavigateForwardRequested;
        public event EventHandler? CloseRequested;
#pragma warning restore CS0067

        // Tracking properties for assertions
        public string? LastDisplayedHtml { get; private set; }
        public string? LastErrorMessage { get; private set; }
        public string? LastInfoMessage { get; private set; }
        public Theme? LastAppliedTheme { get; private set; }
        public int SetSearchResultsCallCount { get; private set; }
        public int ShowSearchBarCallCount { get; private set; }
        public int HideSearchBarCallCount { get; private set; }
        public bool WasClosed { get; private set; }

        // View Actions (Presenter -> View)
        public void DisplayMarkdown(string html)
        {
            LastDisplayedHtml = html;
        }

        public void ShowError(string message, string title)
        {
            LastErrorMessage = message;
        }

        public void ShowInfo(string message, string title)
        {
            LastInfoMessage = message;
        }

        public void ShowSuccess(string message, string title)
        {
            LastInfoMessage = message;
        }

        public void UpdateTheme(Theme theme)
        {
            LastAppliedTheme = theme;
        }

        public void SetNavigationState(bool canGoBack, bool canGoForward)
        {
            // Track if needed for tests
        }

        public void SetSearchResults(int currentMatch, int totalMatches)
        {
            SetSearchResultsCallCount++;
        }

        public void ShowSearchBar()
        {
            IsSearchBarVisible = true;
            ShowSearchBarCallCount++;
        }

        public void HideSearchBar()
        {
            IsSearchBarVisible = false;
            HideSearchBarCallCount++;
        }

        public void Close()
        {
            WasClosed = true;
        }

        // Helper methods to trigger events (for testing event flow)
        public void TriggerViewLoaded()
        {
            ViewLoaded?.Invoke(this, EventArgs.Empty);
        }

        public void TriggerThemeChange(string themeName)
        {
            ThemeChangeRequested?.Invoke(this, new ThemeChangedEventArgs(themeName));
        }

        public void TriggerLanguageChange(string languageCode)
        {
            LanguageChangeRequested?.Invoke(this, new LanguageChangedEventArgs(languageCode));
        }

        public void TriggerRefresh()
        {
            RefreshRequested?.Invoke(this, EventArgs.Empty);
        }

        public void TriggerSearch()
        {
            SearchRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
