using MarkdownViewer.Core.Views;
using MarkdownViewer.Views;

namespace MarkdownViewer.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IWebViewAdapter for testing.
    /// </summary>
    public class MockWebViewAdapter : IWebViewAdapter
    {
        public bool IsInitialized { get; set; } = true;
        public bool CanGoBack { get; set; }
        public bool CanGoForward { get; set; }

        // Events
        public event EventHandler? Initialized;
        public event EventHandler<NavigationStartingEventArgs>? NavigationStarting;
        public event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;

        // Tracking properties
        public string? LastNavigatedHtml { get; private set; }
        public string? LastExecutedScript { get; private set; }
        public int GoBackCallCount { get; private set; }
        public int GoForwardCallCount { get; private set; }
        public int ReloadCallCount { get; private set; }

        public Task NavigateToStringAsync(string html)
        {
            LastNavigatedHtml = html;
            return Task.CompletedTask;
        }

        public Task<string> ExecuteScriptAsync(string script)
        {
            LastExecutedScript = script;
            return Task.FromResult("{}"); // Return empty JSON by default
        }

        public void GoBack()
        {
            GoBackCallCount++;
            if (CanGoBack)
            {
                // Simulate navigation
            }
        }

        public void GoForward()
        {
            GoForwardCallCount++;
            if (CanGoForward)
            {
                // Simulate navigation
            }
        }

        public void Reload()
        {
            ReloadCallCount++;
        }

        // Helper methods to trigger events
        public void TriggerInitialized()
        {
            IsInitialized = true;
            Initialized?.Invoke(this, EventArgs.Empty);
        }

        public void TriggerNavigationStarting(string uri)
        {
            NavigationStarting?.Invoke(this, new NavigationStartingEventArgs(uri));
        }

        public void TriggerWebMessageReceived(string message)
        {
            WebMessageReceived?.Invoke(this, new WebMessageReceivedEventArgs(message));
        }
    }
}
