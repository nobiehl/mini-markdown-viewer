namespace MarkdownViewer.Services
{
    /// <summary>
    /// Service interface for displaying dialogs to the user.
    /// Allows mocking of MessageBox calls for testability.
    /// </summary>
    public interface IDialogService
    {
        void ShowError(string message, string title = "Error");
        void ShowInfo(string message, string title = "Information");
        void ShowWarning(string message, string title = "Warning");
        ServiceDialogResult ShowConfirmation(string message, string title = "Confirm");
        ServiceDialogResult ShowYesNo(string message, string title = "Question");
    }

    /// <summary>
    /// Dialog result enum (platform-independent).
    /// </summary>
    public enum ServiceDialogResult
    {
        OK,
        Cancel,
        Yes,
        No,
        Abort,
        Retry,
        Ignore
    }
}
