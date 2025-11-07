using MarkdownViewer.Views;
using Serilog;

namespace MarkdownViewer.Presenters
{
    /// <summary>
    /// Presenter for SearchBarView.
    /// Handles search functionality in the markdown viewer.
    /// </summary>
    public class SearchBarPresenter
    {
        private readonly ISearchBarView _view;
        private readonly IWebViewAdapter _webView;

        private int _currentMatch;
        private int _totalMatches;

        public SearchBarPresenter(
            ISearchBarView view,
            IWebViewAdapter webView)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));

            // Subscribe to view events
            _view.SearchRequested += OnSearchRequested;
            _view.FindNextRequested += OnFindNextRequested;
            _view.FindPreviousRequested += OnFindPreviousRequested;
            _view.CloseRequested += OnCloseRequested;

            Log.Debug("SearchBarPresenter created");
        }

        private async void OnSearchRequested(object? sender, SearchRequestedEventArgs e)
        {
            try
            {
                Log.Debug("Search requested: {SearchText}, CaseSensitive: {CaseSensitive}",
                    e.SearchText, e.CaseSensitive);

                if (string.IsNullOrEmpty(e.SearchText))
                {
                    ClearSearch();
                    return;
                }

                // Execute search in WebView
                var script = $@"
                    window.find('{EscapeJavaScript(e.SearchText)}',
                        {e.CaseSensitive.ToString().ToLower()},
                        false,
                        true,
                        false,
                        true,
                        false);
                ";

                await _webView.ExecuteScriptAsync(script);

                // Update results (simplified - would need proper counting)
                _currentMatch = 1;
                _totalMatches = 1; // Would need proper implementation
                _view.UpdateResults(_currentMatch, _totalMatches);

                Log.Information("Search executed: {SearchText}", e.SearchText);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during search");
            }
        }

        private async void OnFindNextRequested(object? sender, EventArgs e)
        {
            try
            {
                var searchText = _view.SearchText;
                if (string.IsNullOrEmpty(searchText))
                    return;

                var script = $@"
                    window.find('{EscapeJavaScript(searchText)}',
                        {_view.IsCaseSensitive.ToString().ToLower()},
                        false,
                        true,
                        false,
                        true,
                        false);
                ";

                await _webView.ExecuteScriptAsync(script);

                if (_currentMatch < _totalMatches)
                    _currentMatch++;

                _view.UpdateResults(_currentMatch, _totalMatches);

                Log.Debug("Find next: {CurrentMatch}/{TotalMatches}", _currentMatch, _totalMatches);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during find next");
            }
        }

        private async void OnFindPreviousRequested(object? sender, EventArgs e)
        {
            try
            {
                var searchText = _view.SearchText;
                if (string.IsNullOrEmpty(searchText))
                    return;

                var script = $@"
                    window.find('{EscapeJavaScript(searchText)}',
                        {_view.IsCaseSensitive.ToString().ToLower()},
                        true,
                        true,
                        false,
                        true,
                        false);
                ";

                await _webView.ExecuteScriptAsync(script);

                if (_currentMatch > 1)
                    _currentMatch--;

                _view.UpdateResults(_currentMatch, _totalMatches);

                Log.Debug("Find previous: {CurrentMatch}/{TotalMatches}", _currentMatch, _totalMatches);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error during find previous");
            }
        }

        private void OnCloseRequested(object? sender, EventArgs e)
        {
            ClearSearch();
            _view.Visible = false;
            Log.Debug("Search bar closed");
        }

        private void ClearSearch()
        {
            _view.ClearSearch();
            _currentMatch = 0;
            _totalMatches = 0;
            Log.Debug("Search cleared");
        }

        private static string EscapeJavaScript(string text)
        {
            return text.Replace("'", "\\'")
                      .Replace("\"", "\\\"")
                      .Replace("\n", "\\n")
                      .Replace("\r", "\\r");
        }
    }
}
