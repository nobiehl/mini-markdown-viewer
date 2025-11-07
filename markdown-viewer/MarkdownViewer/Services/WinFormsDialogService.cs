using System.Windows.Forms;

namespace MarkdownViewer.Services
{
    /// <summary>
    /// WinForms implementation of IDialogService.
    /// Uses MessageBox for displaying dialogs.
    /// </summary>
    public class WinFormsDialogService : IDialogService
    {
        public void ShowError(string message, string title = "Error")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ShowInfo(string message, string title = "Information")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ShowWarning(string message, string title = "Warning")
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public ServiceDialogResult ShowConfirmation(string message, string title = "Confirm")
        {
            var result = MessageBox.Show(message, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            return ConvertDialogResult(result);
        }

        public ServiceDialogResult ShowYesNo(string message, string title = "Question")
        {
            var result = MessageBox.Show(message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return ConvertDialogResult(result);
        }

        private static ServiceDialogResult ConvertDialogResult(System.Windows.Forms.DialogResult winFormsResult)
        {
            return winFormsResult switch
            {
                System.Windows.Forms.DialogResult.OK => ServiceDialogResult.OK,
                System.Windows.Forms.DialogResult.Cancel => ServiceDialogResult.Cancel,
                System.Windows.Forms.DialogResult.Yes => ServiceDialogResult.Yes,
                System.Windows.Forms.DialogResult.No => ServiceDialogResult.No,
                System.Windows.Forms.DialogResult.Abort => ServiceDialogResult.Abort,
                System.Windows.Forms.DialogResult.Retry => ServiceDialogResult.Retry,
                System.Windows.Forms.DialogResult.Ignore => ServiceDialogResult.Ignore,
                _ => ServiceDialogResult.Cancel
            };
        }
    }
}
