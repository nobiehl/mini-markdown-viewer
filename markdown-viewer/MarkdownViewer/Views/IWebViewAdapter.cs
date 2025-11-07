namespace MarkdownViewer.Views
{
    /// <summary>
    /// Adapter interface for WebView2 control.
    /// Allows testing without actual WebView2 instance.
    /// </summary>
    public interface IWebViewAdapter
    {
        bool IsInitialized { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }

        event EventHandler Initialized;
        event EventHandler<NavigationStartingEventArgs> NavigationStarting;
        event EventHandler<WebMessageReceivedEventArgs> WebMessageReceived;

        Task NavigateToStringAsync(string html);
        Task<string> ExecuteScriptAsync(string script);
        void GoBack();
        void GoForward();
        void Reload();
    }

    /// <summary>
    /// Event arguments for navigation starting event.
    /// </summary>
    public class NavigationStartingEventArgs : EventArgs
    {
        public string Uri { get; }
        public bool Cancel { get; set; }
        public NavigationStartingEventArgs(string uri) => Uri = uri;
    }

    /// <summary>
    /// Event arguments for web message received event.
    /// </summary>
    public class WebMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }
        public WebMessageReceivedEventArgs(string message) => Message = message;
    }
}
