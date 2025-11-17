using System;
using System.Drawing;
using System.Windows.Forms;
using MarkdownViewer.Core.Services;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Notification bar shown when the currently viewed file is deleted.
    /// Provides options to save content or keep viewing.
    /// Similar to UpdateNotificationBar but for file deletion events.
    /// </summary>
    public class FileDeletedNotificationBar : Panel
    {
        private readonly ILocalizationService _localizationService;
        private Label? _messageLabel;
        private Button? _saveButton;
        private Button? _closeButton;
        private System.Windows.Forms.Timer? _autoHideTimer;

        public event EventHandler<string>? SaveRequested; // Provides file path
        public event EventHandler? CloseRequested;

        public FileDeletedNotificationBar(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            InitializeComponents();
            InitializeAutoHideTimer();
        }

        private void InitializeAutoHideTimer()
        {
            _autoHideTimer = new System.Windows.Forms.Timer();
            _autoHideTimer.Interval = 3000; // 3 seconds
            _autoHideTimer.Tick += (s, e) =>
            {
                _autoHideTimer.Stop();
                this.Hide();
            };
        }

        private void InitializeComponents()
        {
            // Panel setup
            this.Dock = DockStyle.Top;
            this.Height = 50;
            this.Padding = new Padding(10);
            this.Visible = false;

            // Message label
            _messageLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font(SystemFonts.DefaultFont.FontFamily, 10F, FontStyle.Regular)
            };

            // Save button
            _saveButton = new Button
            {
                Text = "💾 Save",
                AutoSize = true,
                Dock = DockStyle.Right,
                Margin = new Padding(5, 0, 5, 0),
                Padding = new Padding(15, 5, 15, 5)
            };
            _saveButton.Click += (s, e) => OnSaveClicked();

            // Close button
            _closeButton = new Button
            {
                Text = "✕",
                Width = 30,
                Dock = DockStyle.Right,
                FlatStyle = FlatStyle.Flat
            };
            _closeButton.Click += (s, e) => OnCloseClicked();

            // Add controls
            this.Controls.Add(_messageLabel);
            this.Controls.Add(_saveButton);
            this.Controls.Add(_closeButton);
        }

        public void Show(string filePath)
        {
            // Stop auto-hide timer if running
            _autoHideTimer?.Stop();

            string fileName = System.IO.Path.GetFileName(filePath);
            _messageLabel!.Text = $"⚠️  File \"{fileName}\" was deleted. Content is still visible in the viewer.";

            // Show buttons for deleted mode
            _saveButton!.Visible = true;
            _closeButton!.Visible = true;

            this.Visible = true;
            this.BringToFront();
        }

        public void ShowRecreated(string filePath)
        {
            // Stop any running timer
            _autoHideTimer?.Stop();

            string fileName = System.IO.Path.GetFileName(filePath);
            _messageLabel!.Text = $"✅  File \"{fileName}\" was recreated and reloaded.";

            // Hide buttons for recreated mode (info only)
            _saveButton!.Visible = false;
            _closeButton!.Visible = false;

            // Apply green theme for recreated
            this.BackColor = Color.FromArgb(212, 237, 218); // Light green
            _messageLabel!.ForeColor = Color.FromArgb(21, 87, 36); // Dark green

            this.Visible = true;
            this.BringToFront();

            // Start auto-hide timer (3 seconds)
            _autoHideTimer?.Start();
        }

        public new void Hide()
        {
            _autoHideTimer?.Stop();
            this.Visible = false;
        }

        private void OnSaveClicked()
        {
            // Get file path from parent form
            var mainForm = this.FindForm() as MainForm;
            if (mainForm != null)
            {
                SaveRequested?.Invoke(this, mainForm.CurrentFilePath);
            }
        }

        private void OnCloseClicked()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
            this.Hide();
        }

        public void ApplyTheme(bool isDarkTheme)
        {
            if (isDarkTheme)
            {
                this.BackColor = Color.FromArgb(60, 60, 60);
                _messageLabel!.ForeColor = Color.White;
                _saveButton!.BackColor = Color.FromArgb(80, 80, 80);
                _saveButton.ForeColor = Color.White;
                _closeButton!.BackColor = Color.FromArgb(80, 80, 80);
                _closeButton.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = Color.FromArgb(255, 243, 205); // Light yellow
                _messageLabel!.ForeColor = Color.Black;
                _saveButton!.BackColor = Color.FromArgb(240, 240, 240);
                _saveButton.ForeColor = Color.Black;
                _closeButton!.BackColor = Color.FromArgb(240, 240, 240);
                _closeButton.ForeColor = Color.Black;
            }
        }
    }
}
