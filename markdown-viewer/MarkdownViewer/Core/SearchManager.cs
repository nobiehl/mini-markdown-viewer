using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Web.WebView2.WinForms;
using Serilog;

namespace MarkdownViewer.Core
{
    /// <summary>
    /// Manages in-page search functionality using mark.js for highlighting.
    /// Provides search term highlighting and match navigation.
    /// </summary>
    public class SearchManager
    {
        private readonly WebView2 _webView;
        private string _currentSearchTerm = string.Empty;
        private int _currentMatchIndex = 0;
        private int _totalMatches = 0;
        private bool _markJsLoaded = false;

        /// <summary>
        /// Event raised when search results change.
        /// </summary>
        public event EventHandler<SearchResultsEventArgs>? SearchResultsChanged;

        /// <summary>
        /// Gets the current search term.
        /// </summary>
        public string CurrentSearchTerm => _currentSearchTerm;

        /// <summary>
        /// Gets the current match index (1-based).
        /// </summary>
        public int CurrentMatchIndex => _currentMatchIndex + 1;

        /// <summary>
        /// Gets the total number of matches.
        /// </summary>
        public int TotalMatches => _totalMatches;

        /// <summary>
        /// Gets whether there are any search results.
        /// </summary>
        public bool HasResults => _totalMatches > 0;

        /// <summary>
        /// Initializes a new instance of the SearchManager.
        /// </summary>
        /// <param name="webView">The WebView2 control to search in</param>
        public SearchManager(WebView2 webView)
        {
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));

            // Subscribe to WebView2 messages for search results
            _webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (e.IsSuccess && _webView.CoreWebView2 != null)
                {
                    _webView.CoreWebView2.WebMessageReceived += OnWebMessageReceived;
                    Log.Debug("SearchManager registered for WebView2 messages");
                }
            };
        }

        private void OnWebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            try
            {
                string json = e.TryGetWebMessageAsString();

                // Parse search result message
                if (json.Contains("\"type\":\"search-results\""))
                {
                    // Simple JSON parsing (for production, use System.Text.Json)
                    var totalStart = json.IndexOf("\"total\":") + 8;
                    var totalEnd = json.IndexOf("}", totalStart);
                    if (totalEnd == -1) totalEnd = json.Length;

                    var totalStr = json.Substring(totalStart, totalEnd - totalStart).Trim().TrimEnd('}');

                    if (int.TryParse(totalStr, out int total))
                    {
                        _totalMatches = total;
                        _currentMatchIndex = total > 0 ? 0 : -1;

                        Log.Information("Search results: {Total} matches for '{Term}'", total, _currentSearchTerm);

                        // Raise event
                        SearchResultsChanged?.Invoke(this, new SearchResultsEventArgs(_totalMatches, _currentMatchIndex + 1));

                        // Auto-scroll to first match if any
                        if (_totalMatches > 0)
                        {
                            _ = ScrollToMatchAsync(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error processing search results message");
            }
        }

        /// <summary>
        /// Performs a search for the specified term.
        /// </summary>
        public async Task SearchAsync(string searchTerm)
        {
            if (_webView.CoreWebView2 == null)
            {
                Log.Warning("Cannot search: WebView2 not initialized");
                return;
            }

            _currentSearchTerm = searchTerm;
            _currentMatchIndex = 0;
            _totalMatches = 0;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                await ClearSearchAsync();
                return;
            }

            // Ensure mark.js is loaded
            await EnsureMarkJsLoadedAsync();

            // Escape search term for JavaScript
            string escapedTerm = EscapeJavaScript(searchTerm);

            // Perform search with mark.js
            var script = $@"
                (function() {{
                    try {{
                        var instance = new Mark(document.body);
                        instance.unmark();
                        instance.mark('{escapedTerm}', {{
                            'className': 'search-highlight',
                            'done': function(totalMatches) {{
                                window.chrome.webview.postMessage({{
                                    type: 'search-results',
                                    total: totalMatches
                                }});
                            }}
                        }});
                    }} catch (e) {{
                        console.error('Search error:', e);
                    }}
                }})();
            ";

            await _webView.CoreWebView2.ExecuteScriptAsync(script);
            Log.Debug("Search executed for term: {Term}", searchTerm);
        }

        /// <summary>
        /// Navigates to the next search match.
        /// </summary>
        public async Task NextMatchAsync()
        {
            if (_totalMatches == 0) return;

            _currentMatchIndex = (_currentMatchIndex + 1) % _totalMatches;
            await ScrollToMatchAsync(_currentMatchIndex);

            SearchResultsChanged?.Invoke(this, new SearchResultsEventArgs(_totalMatches, _currentMatchIndex + 1));
            Log.Debug("Next match: {Current}/{Total}", _currentMatchIndex + 1, _totalMatches);
        }

        /// <summary>
        /// Navigates to the previous search match.
        /// </summary>
        public async Task PreviousMatchAsync()
        {
            if (_totalMatches == 0) return;

            _currentMatchIndex = (_currentMatchIndex - 1 + _totalMatches) % _totalMatches;
            await ScrollToMatchAsync(_currentMatchIndex);

            SearchResultsChanged?.Invoke(this, new SearchResultsEventArgs(_totalMatches, _currentMatchIndex + 1));
            Log.Debug("Previous match: {Current}/{Total}", _currentMatchIndex + 1, _totalMatches);
        }

        /// <summary>
        /// Clears the current search and removes all highlighting.
        /// </summary>
        public async Task ClearSearchAsync()
        {
            if (_webView.CoreWebView2 == null) return;

            _currentSearchTerm = string.Empty;
            _currentMatchIndex = 0;
            _totalMatches = 0;

            var script = @"
                (function() {
                    try {
                        if (typeof Mark !== 'undefined') {
                            var instance = new Mark(document.body);
                            instance.unmark();
                        }
                    } catch (e) {
                        console.error('Clear search error:', e);
                    }
                })();
            ";

            await _webView.CoreWebView2.ExecuteScriptAsync(script);

            SearchResultsChanged?.Invoke(this, new SearchResultsEventArgs(0, 0));
            Log.Debug("Search cleared");
        }

        private async Task ScrollToMatchAsync(int index)
        {
            if (_webView.CoreWebView2 == null) return;

            var script = $@"
                (function() {{
                    try {{
                        var marks = document.querySelectorAll('.search-highlight');
                        if (marks && marks[{index}]) {{
                            // Remove 'current' class from all marks
                            marks.forEach(function(m) {{ m.classList.remove('current'); }});

                            // Add 'current' class to active mark
                            marks[{index}].classList.add('current');

                            // Scroll into view
                            marks[{index}].scrollIntoView({{ behavior: 'smooth', block: 'center' }});
                        }}
                    }} catch (e) {{
                        console.error('Scroll error:', e);
                    }}
                }})();
            ";

            await _webView.CoreWebView2.ExecuteScriptAsync(script);
        }

        private async Task EnsureMarkJsLoadedAsync()
        {
            if (_markJsLoaded || _webView.CoreWebView2 == null) return;

            // Inject mark.js library from CDN
            var script = @"
                (function() {
                    if (typeof Mark === 'undefined') {
                        var script = document.createElement('script');
                        script.src = 'https://cdn.jsdelivr.net/npm/mark.js@8.11.1/dist/mark.min.js';
                        script.onload = function() { console.log('mark.js loaded'); };
                        document.head.appendChild(script);
                    }

                    // Inject search highlight CSS
                    if (!document.getElementById('search-highlight-css')) {
                        var style = document.createElement('style');
                        style.id = 'search-highlight-css';
                        style.textContent = `
                            .search-highlight {
                                background-color: yellow;
                                color: black;
                            }
                            .search-highlight.current {
                                background-color: orange;
                                color: white;
                            }
                        `;
                        document.head.appendChild(style);
                    }
                })();
            ";

            await _webView.CoreWebView2.ExecuteScriptAsync(script);
            _markJsLoaded = true;

            // Wait a bit for mark.js to load
            await Task.Delay(300);

            Log.Information("mark.js library loaded");
        }

        private static string EscapeJavaScript(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;

            return text
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }

    /// <summary>
    /// Event arguments for search results changes.
    /// </summary>
    public class SearchResultsEventArgs : EventArgs
    {
        public int TotalMatches { get; }
        public int CurrentMatch { get; }

        public SearchResultsEventArgs(int totalMatches, int currentMatch)
        {
            TotalMatches = totalMatches;
            CurrentMatch = currentMatch;
        }
    }
}
