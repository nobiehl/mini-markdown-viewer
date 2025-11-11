using System;
using System.Drawing;
using System.Windows.Forms;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Services;
using Serilog;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Notification bar for displaying update availability.
    /// Positioned above StatusBar with 3 action buttons.
    /// Replaces the previous two-dialog system (MarkdownDialog + MessageBox).
    /// </summary>
    public class UpdateNotificationBar : Panel
    {
        // Services
        private readonly ILocalizationService _localization;

        // UI Components
        private PictureBox _iconBox = null!;
        private Label _messageLabel = null!;
        private Button _showButton = null!;
        private Button _updateButton = null!;
        private Button _ignoreButton = null!;

        // State
        private string _latestVersion = string.Empty;
        private string _releaseNotes = string.Empty;

        /// <summary>
        /// Raised when user clicks "Show Release Notes" button.
        /// MainForm should load release notes as markdown in main viewer.
        /// </summary>
        public event EventHandler<ReleaseNotesEventArgs>? ShowRequested;

        /// <summary>
        /// Raised when user clicks "Update Now" button.
        /// MainForm should start download and ask for restart confirmation.
        /// </summary>
        public event EventHandler<UpdateEventArgs>? UpdateRequested;

        /// <summary>
        /// Raised when user clicks "Ignore" button.
        /// MainForm should hide the notification bar.
        /// </summary>
        public event EventHandler? IgnoreRequested;

        /// <summary>
        /// Initializes a new UpdateNotificationBar.
        /// </summary>
        /// <param name="localization">Localization service for multi-language support</param>
        public UpdateNotificationBar(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));
            InitializeComponents();
            Log.Debug("UpdateNotificationBar initialized");
        }

        /// <summary>
        /// Initializes all UI components.
        /// </summary>
        private void InitializeComponents()
        {
            // Panel configuration
            this.Dock = DockStyle.Bottom;
            this.Height = 50;
            this.Visible = false; // Hidden by default
            this.BackColor = Color.FromArgb(255, 250, 205); // Light yellow (warning color)
            this.Padding = new Padding(10, 5, 10, 5);

            // Icon (download icon)
            _iconBox = new PictureBox
            {
                Size = new Size(24, 24),
                Location = new Point(10, 13),
                SizeMode = PictureBoxSizeMode.Zoom
            };

            // Try to load download icon
            try
            {
                var downloadIcon = IconHelper.LoadIcon("download", 24, Color.FromArgb(64, 64, 64));
                if (downloadIcon != null)
                {
                    _iconBox.Image = downloadIcon;
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to load download icon for UpdateNotificationBar");
            }

            // Message label
            _messageLabel = new Label
            {
                Text = _localization.GetString("UpdateAvailable"),
                AutoSize = true,
                Location = new Point(45, 15),
                Font = new Font("Segoe UI", 9.75F, FontStyle.Regular),
                ForeColor = Color.FromArgb(64, 64, 64)
            };

            // Show Release Notes button
            _showButton = new Button
            {
                Text = _localization.GetString("UpdateShowReleaseNotes"),
                AutoSize = true,
                Location = new Point(300, 10),
                Height = 30,
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                Cursor = Cursors.Hand,
                Name = "ShowReleaseNotesButton" // For UI Automation
            };
            _showButton.Click += OnShowButtonClick;

            // Update Now button
            _updateButton = new Button
            {
                Text = _localization.GetString("UpdateInstall"),
                AutoSize = true,
                Location = new Point(480, 10),
                Height = 30,
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.FromArgb(0, 120, 215), // Windows blue
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Name = "UpdateNowButton" // For UI Automation
            };
            _updateButton.Click += OnUpdateButtonClick;

            // Ignore button
            _ignoreButton = new Button
            {
                Text = _localization.GetString("UpdateIgnore"),
                AutoSize = true,
                Location = new Point(640, 10),
                Height = 30,
                FlatStyle = FlatStyle.Standard,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(64, 64, 64),
                Cursor = Cursors.Hand,
                Name = "IgnoreButton" // For UI Automation
            };
            _ignoreButton.Click += OnIgnoreButtonClick;

            // Add controls to panel
            this.Controls.Add(_iconBox);
            this.Controls.Add(_messageLabel);
            this.Controls.Add(_showButton);
            this.Controls.Add(_updateButton);
            this.Controls.Add(_ignoreButton);

            // Handle resize to keep buttons aligned
            this.Resize += OnResize;
        }

        /// <summary>
        /// Handles panel resize to reposition buttons.
        /// </summary>
        private void OnResize(object? sender, EventArgs e)
        {
            // Keep buttons right-aligned with some spacing
            int rightMargin = 10;
            int buttonSpacing = 10;

            _ignoreButton.Left = this.ClientSize.Width - _ignoreButton.Width - rightMargin;
            _updateButton.Left = _ignoreButton.Left - _updateButton.Width - buttonSpacing;
            _showButton.Left = _updateButton.Left - _showButton.Width - buttonSpacing;
        }

        /// <summary>
        /// Shows the notification bar with update information.
        /// </summary>
        /// <param name="latestVersion">Latest version number (e.g., "1.8.1")</param>
        /// <param name="releaseNotes">Release notes in markdown format</param>
        public void Show(string latestVersion, string releaseNotes)
        {
            _latestVersion = latestVersion;
            _releaseNotes = releaseNotes;

            // Format: "Update v1.8.0 available"
            _messageLabel.Text = string.Format(_localization.GetString("UpdateAvailable"), latestVersion);
            this.Visible = true;
            this.BringToFront(); // Ensure bar is above StatusBar

            Log.Information("UpdateNotificationBar shown for version {Version}", latestVersion);
        }

        /// <summary>
        /// Hides the notification bar.
        /// </summary>
        public new void Hide()
        {
            this.Visible = false;
            Log.Debug("UpdateNotificationBar hidden");
        }

        /// <summary>
        /// Applies theme-aware colors to the notification bar.
        /// Uses warning colors that stand out but remain readable in both light and dark themes.
        /// </summary>
        /// <param name="isDarkTheme">True if dark theme is active, false otherwise</param>
        public void ApplyTheme(bool isDarkTheme)
        {
            if (isDarkTheme)
            {
                // Dark theme: darker orange/amber background with light text
                this.BackColor = Color.FromArgb(70, 50, 20); // Dark amber
                _messageLabel.ForeColor = Color.FromArgb(255, 220, 150); // Light amber text
                _showButton.BackColor = Color.FromArgb(90, 70, 30);
                _showButton.ForeColor = Color.FromArgb(255, 220, 150);
                _updateButton.BackColor = Color.FromArgb(0, 120, 215); // Keep Windows blue
                _updateButton.ForeColor = Color.White;
                _ignoreButton.BackColor = Color.FromArgb(60, 60, 60);
                _ignoreButton.ForeColor = Color.FromArgb(200, 200, 200);
            }
            else
            {
                // Light theme: light yellow/amber background with dark text
                this.BackColor = Color.FromArgb(255, 250, 205); // Light yellow
                _messageLabel.ForeColor = Color.FromArgb(64, 64, 64); // Dark gray
                _showButton.BackColor = Color.White;
                _showButton.ForeColor = Color.FromArgb(64, 64, 64);
                _updateButton.BackColor = Color.FromArgb(0, 120, 215); // Keep Windows blue
                _updateButton.ForeColor = Color.White;
                _ignoreButton.BackColor = Color.White;
                _ignoreButton.ForeColor = Color.FromArgb(64, 64, 64);
            }

            Log.Debug("UpdateNotificationBar theme applied: IsDarkTheme={IsDarkTheme}", isDarkTheme);
        }

        /// <summary>
        /// Handles "Show Release Notes" button click.
        /// </summary>
        private void OnShowButtonClick(object? sender, EventArgs e)
        {
            Log.Information("Show Release Notes button clicked");
            ShowRequested?.Invoke(this, new ReleaseNotesEventArgs(_latestVersion, _releaseNotes));
        }

        /// <summary>
        /// Handles "Update Now" button click.
        /// </summary>
        private void OnUpdateButtonClick(object? sender, EventArgs e)
        {
            Log.Information("Update Now button clicked");
            UpdateRequested?.Invoke(this, new UpdateEventArgs(_latestVersion));
        }

        /// <summary>
        /// Handles "Ignore" button click.
        /// </summary>
        private void OnIgnoreButtonClick(object? sender, EventArgs e)
        {
            Log.Information("Ignore button clicked");
            IgnoreRequested?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Event args for showing release notes.
    /// </summary>
    public class ReleaseNotesEventArgs : EventArgs
    {
        public string Version { get; }
        public string ReleaseNotes { get; }

        public ReleaseNotesEventArgs(string version, string releaseNotes)
        {
            Version = version;
            ReleaseNotes = releaseNotes;
        }
    }

    /// <summary>
    /// Event args for update request.
    /// </summary>
    public class UpdateEventArgs : EventArgs
    {
        public string Version { get; }

        public UpdateEventArgs(string version)
        {
            Version = version;
        }
    }
}
