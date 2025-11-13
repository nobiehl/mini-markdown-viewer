using MarkdownViewer.Core.Services;
using MarkdownViewer.Services;

namespace MarkdownViewer.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of IDialogService for testing.
    /// </summary>
    public class MockDialogService : IDialogService
    {
        public string? LastErrorMessage { get; private set; }
        public string? LastInfoMessage { get; private set; }
        public string? LastWarningMessage { get; private set; }
        public int ShowErrorCallCount { get; private set; }
        public int ShowInfoCallCount { get; private set; }
        public int ShowWarningCallCount { get; private set; }

        // Configure dialog results for testing
        public ServiceDialogResult ConfirmationResult { get; set; } = ServiceDialogResult.OK;
        public ServiceDialogResult YesNoResult { get; set; } = ServiceDialogResult.Yes;

        public void ShowError(string message, string title = "Error")
        {
            LastErrorMessage = message;
            ShowErrorCallCount++;
        }

        public void ShowInfo(string message, string title = "Information")
        {
            LastInfoMessage = message;
            ShowInfoCallCount++;
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            LastWarningMessage = message;
            ShowWarningCallCount++;
        }

        public ServiceDialogResult ShowConfirmation(string message, string title = "Confirm")
        {
            return ConfirmationResult;
        }

        public ServiceDialogResult ShowYesNo(string message, string title = "Question")
        {
            return YesNoResult;
        }
    }
}
