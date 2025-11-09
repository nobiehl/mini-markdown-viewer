using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using Serilog;
using MarkdownViewer.Core.Views;

namespace MarkdownViewer.Views
{
    /// <summary>
    /// Adapter for WebView2 control that implements IWebViewAdapter.
    /// Wraps a real WebView2 instance and exposes it through the interface for testability.
    /// </summary>
    public class WebView2Adapter : IWebViewAdapter
    {
        private readonly WebView2 _webView;

        public WebView2Adapter(WebView2 webView)
        {
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));

            // Subscribe to WebView2 events and forward them
            _webView.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
            _webView.NavigationStarting += OnNavigationStarting;
            _webView.WebMessageReceived += OnWebMessageReceived;
        }

        #region IWebViewAdapter Properties

        public bool IsInitialized => _webView.CoreWebView2 != null;

        public bool CanGoBack => IsInitialized && (_webView.CoreWebView2?.CanGoBack ?? false);

        public bool CanGoForward => IsInitialized && (_webView.CoreWebView2?.CanGoForward ?? false);

        #endregion

        #region IWebViewAdapter Events

        public event EventHandler? Initialized;
        public event EventHandler<NavigationStartingEventArgs>? NavigationStarting;
        public event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;

        #endregion

        #region IWebViewAdapter Methods

        public async Task NavigateToStringAsync(string html)
        {
            if (!IsInitialized)
            {
                Log.Warning("Cannot navigate: WebView2 not initialized");
                return;
            }

            try
            {
                _webView.CoreWebView2!.NavigateToString(html);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error navigating to string");
                throw;
            }
        }

        public async Task<string> ExecuteScriptAsync(string script)
        {
            if (!IsInitialized)
            {
                Log.Warning("Cannot execute script: WebView2 not initialized");
                return string.Empty;
            }

            try
            {
                var result = await _webView.CoreWebView2!.ExecuteScriptAsync(script);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error executing script: {Script}", script);
                throw;
            }
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                _webView.CoreWebView2!.GoBack();
            }
            else
            {
                Log.Debug("Cannot go back: CanGoBack is false");
            }
        }

        public void GoForward()
        {
            if (CanGoForward)
            {
                _webView.CoreWebView2!.GoForward();
            }
            else
            {
                Log.Debug("Cannot go forward: CanGoForward is false");
            }
        }

        public void Reload()
        {
            if (IsInitialized)
            {
                _webView.CoreWebView2!.Reload();
            }
            else
            {
                Log.Debug("Cannot reload: WebView2 not initialized");
            }
        }

        #endregion

        #region Event Handlers

        private void OnCoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                Log.Information("WebView2 initialization completed successfully");
                Initialized?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                Log.Error(e.InitializationException, "WebView2 initialization failed");
            }
        }

        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            var args = new NavigationStartingEventArgs(e.Uri);
            NavigationStarting?.Invoke(this, args);

            if (args.Cancel)
            {
                e.Cancel = true;
            }
        }

        private void OnWebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var args = new WebMessageReceivedEventArgs(e.WebMessageAsJson);
            WebMessageReceived?.Invoke(this, args);
        }

        #endregion
    }
}
