namespace MarkdownViewer.Core.Views
{
    /// <summary>
    /// View interface for NavigationBar control.
    /// Abstracts navigation bar interactions for testability.
    /// </summary>
    public interface INavigationBarView
    {
        // Properties
        bool Visible { get; set; }
        bool CanGoBack { get; set; }
        bool CanGoForward { get; set; }

        // Events (View -> Presenter)
        event EventHandler BackRequested;
        event EventHandler ForwardRequested;

        // View Actions (Presenter -> View)
        void UpdateNavigationState(bool canGoBack, bool canGoForward);
        void SetCurrentPath(string filePath);
    }
}
