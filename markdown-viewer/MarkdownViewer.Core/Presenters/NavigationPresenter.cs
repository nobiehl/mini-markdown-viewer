using MarkdownViewer.Core.Views;
using Serilog;

namespace MarkdownViewer.Core.Presenters
{
    /// <summary>
    /// Presenter for NavigationBarView.
    /// Handles navigation controls (back, forward, refresh, home).
    /// </summary>
    public class NavigationPresenter
    {
        private readonly INavigationBarView _view;
        private readonly IWebViewAdapter _webView;

        private string _currentPath;

        public NavigationPresenter(
            INavigationBarView view,
            IWebViewAdapter webView)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));

            _currentPath = string.Empty;

            // Subscribe to view events
            _view.BackRequested += OnBackRequested;
            _view.ForwardRequested += OnForwardRequested;
            _view.RefreshRequested += OnRefreshRequested;
            _view.HomeRequested += OnHomeRequested;

            Log.Debug("NavigationPresenter created");
        }

        /// <summary>
        /// Updates the navigation state based on WebView state.
        /// </summary>
        public void UpdateNavigationState()
        {
            var canGoBack = _webView.CanGoBack;
            var canGoForward = _webView.CanGoForward;

            _view.UpdateNavigationState(canGoBack, canGoForward);

            Log.Debug("Navigation state updated: Back={CanGoBack}, Forward={CanGoForward}",
                canGoBack, canGoForward);
        }

        /// <summary>
        /// Sets the current file path.
        /// </summary>
        public void SetCurrentPath(string filePath)
        {
            _currentPath = filePath;
            _view.SetCurrentPath(filePath);
            Log.Debug("Current path set: {FilePath}", filePath);
        }

        private void OnBackRequested(object? sender, EventArgs e)
        {
            if (_webView.CanGoBack)
            {
                _webView.GoBack();
                UpdateNavigationState();
                Log.Debug("Navigated back");
            }
        }

        private void OnForwardRequested(object? sender, EventArgs e)
        {
            if (_webView.CanGoForward)
            {
                _webView.GoForward();
                UpdateNavigationState();
                Log.Debug("Navigated forward");
            }
        }

        private void OnRefreshRequested(object? sender, EventArgs e)
        {
            _webView.Reload();
            Log.Debug("Page refreshed");
        }

        private void OnHomeRequested(object? sender, EventArgs e)
        {
            // Home functionality would navigate to a default or home file
            Log.Debug("Home requested (not implemented)");
        }
    }
}
