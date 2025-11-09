namespace MarkdownViewer.Views
{
    /// <summary>
    /// View interface for SearchBar control.
    /// Abstracts search bar interactions for testability.
    /// </summary>
    public interface ISearchBarView
    {
        // Properties
        string SearchText { get; set; }
        bool Visible { get; set; }
        bool IsCaseSensitive { get; set; }

        // Events (View -> Presenter)
        event EventHandler<SearchRequestedEventArgs> SearchRequested;
        event EventHandler FindNextRequested;
        event EventHandler FindPreviousRequested;
        event EventHandler CloseRequested;

        // View Actions (Presenter -> View)
        void UpdateResults(int currentMatch, int totalMatches);
        void ClearSearch();
        void Focus();
    }

    /// <summary>
    /// Event arguments for search requested event.
    /// </summary>
    public class SearchRequestedEventArgs : EventArgs
    {
        public string SearchText { get; }
        public bool CaseSensitive { get; }

        public SearchRequestedEventArgs(string searchText, bool caseSensitive)
        {
            SearchText = searchText;
            CaseSensitive = caseSensitive;
        }
    }
}
