using System;
using Microsoft.Web.WebView2.WinForms;
using Serilog;

namespace MarkdownViewer.Core
{
    /// <summary>
    /// Manages WebView2 navigation history (Back/Forward).
    /// Provides navigation state tracking and history manipulation.
    /// </summary>
    public class NavigationManager
    {
        private readonly WebView2 _webView;
        private bool _isInitialized;

        /// <summary>
        /// Gets whether the WebView can navigate back.
        /// </summary>
        public bool CanGoBack => _isInitialized && (_webView.CoreWebView2?.CanGoBack ?? false);

        /// <summary>
        /// Gets whether the WebView can navigate forward.
        /// </summary>
        public bool CanGoForward => _isInitialized && (_webView.CoreWebView2?.CanGoForward ?? false);

        /// <summary>
        /// Event raised when navigation state changes (history changed).
        /// </summary>
        public event EventHandler? NavigationChanged;

        /// <summary>
        /// Initializes a new instance of the NavigationManager.
        /// </summary>
        /// <param name="webView">The WebView2 control to manage navigation for</param>
        public NavigationManager(WebView2 webView)
        {
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));

            // Subscribe to CoreWebView2 initialization
            _webView.CoreWebView2InitializationCompleted += OnCoreWebView2Initialized;
        }

        private void OnCoreWebView2Initialized(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess && _webView.CoreWebView2 != null)
            {
                _isInitialized = true;

                // Subscribe to history changes
                _webView.CoreWebView2.HistoryChanged += OnHistoryChanged;

                Log.Debug("NavigationManager initialized with WebView2");
            }
            else
            {
                Log.Warning("NavigationManager initialization failed: {Error}",
                    e.InitializationException?.Message ?? "Unknown error");
            }
        }

        private void OnHistoryChanged(object? sender, object e)
        {
            Log.Debug("Navigation history changed. CanGoBack={CanGoBack}, CanGoForward={CanGoForward}",
                CanGoBack, CanGoForward);

            // Notify subscribers that navigation state changed
            NavigationChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Navigates back in the WebView2 history if possible.
        /// </summary>
        public void GoBack()
        {
            if (CanGoBack)
            {
                Log.Information("Navigating back");
                _webView.CoreWebView2.GoBack();
            }
            else
            {
                Log.Debug("Cannot navigate back - no history available");
            }
        }

        /// <summary>
        /// Navigates forward in the WebView2 history if possible.
        /// </summary>
        public void GoForward()
        {
            if (CanGoForward)
            {
                Log.Information("Navigating forward");
                _webView.CoreWebView2.GoForward();
            }
            else
            {
                Log.Debug("Cannot navigate forward - no forward history available");
            }
        }

        /// <summary>
        /// Clears the navigation history.
        /// Note: WebView2 does not support clearing history directly.
        /// This method is provided for future compatibility.
        /// </summary>
        public void ClearHistory()
        {
            Log.Warning("ClearHistory() called but not supported by WebView2");
            // WebView2 does not support clearing history
            // Future: Could be implemented by recreating WebView2 control
        }
    }
}
