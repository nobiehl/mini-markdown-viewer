using System;
using System.Windows.Forms;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Core.Views;
using Serilog;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Navigation toolbar with Back/Forward buttons.
    /// Implements INavigationBarView for MVP pattern (v2.0.0).
    /// </summary>
    public class NavigationBar : INavigationBarView
    {
        private readonly ToolStrip _toolbar;
        private readonly ToolStripButton _backButton;
        private readonly ToolStripButton _forwardButton;
        private readonly ILocalizationService _localization;

        private bool _canGoBack;
        private bool _canGoForward;

        /// <summary>
        /// Gets the underlying ToolStrip control.
        /// </summary>
        public ToolStrip Control => _toolbar;

        #region INavigationBarView Implementation

        /// <summary>
        /// Gets or sets whether the navigation bar is visible.
        /// </summary>
        public bool Visible
        {
            get => _toolbar.Visible;
            set => _toolbar.Visible = value;
        }

        /// <summary>
        /// Gets or sets whether the back button is enabled.
        /// </summary>
        public bool CanGoBack
        {
            get => _canGoBack;
            set
            {
                _canGoBack = value;
                _backButton.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the forward button is enabled.
        /// </summary>
        public bool CanGoForward
        {
            get => _canGoForward;
            set
            {
                _canGoForward = value;
                _forwardButton.Enabled = value;
            }
        }

        // Events (View → Presenter)
        public event EventHandler? BackRequested;
        public event EventHandler? ForwardRequested;

        #endregion

        /// <summary>
        /// Initializes a new instance of the NavigationBar.
        /// </summary>
        /// <param name="localization">Localization service for tooltips</param>
        public NavigationBar(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Create back button
            _backButton = new ToolStripButton
            {
                Text = "←",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                ToolTipText = _localization.GetString("NavigationBack"),
                Enabled = false,
                AutoSize = true
            };
            _backButton.Click += OnBackButtonClick;

            // Create forward button
            _forwardButton = new ToolStripButton
            {
                Text = "→",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold),
                ToolTipText = _localization.GetString("NavigationForward"),
                Enabled = false,
                AutoSize = true
            };
            _forwardButton.Click += OnForwardButtonClick;

            // Create toolbar
            _toolbar = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                Visible = false // Hidden by default
            };

            _toolbar.Items.Add(_backButton);
            _toolbar.Items.Add(new ToolStripSeparator());
            _toolbar.Items.Add(_forwardButton);

            Log.Debug("NavigationBar initialized (MVP pattern)");
        }

        private void OnBackButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Back button clicked, raising BackRequested event");
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnForwardButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Forward button clicked, raising ForwardRequested event");
            ForwardRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the navigation state (button enabled/disabled state).
        /// Called by presenter when navigation state changes.
        /// </summary>
        public void UpdateNavigationState(bool canGoBack, bool canGoForward)
        {
            CanGoBack = canGoBack;
            CanGoForward = canGoForward;

            Log.Debug("Navigation state updated: CanGoBack={CanGoBack}, CanGoForward={CanGoForward}",
                canGoBack, canGoForward);
        }

        /// <summary>
        /// Sets the current file path (for display purposes).
        /// </summary>
        public void SetCurrentPath(string filePath)
        {
            // Currently not displayed in UI, but interface requires it
            // Could be implemented as a label in the toolbar in future
            Log.Debug("Current path set: {FilePath}", filePath);
        }

        /// <summary>
        /// Shows the navigation bar.
        /// </summary>
        public void Show()
        {
            Log.Debug("Showing NavigationBar");
            _toolbar.Visible = true;
        }

        /// <summary>
        /// Hides the navigation bar.
        /// </summary>
        public void Hide()
        {
            Log.Debug("Hiding NavigationBar");
            _toolbar.Visible = false;
        }

        /// <summary>
        /// Toggles the navigation bar visibility.
        /// </summary>
        public void Toggle()
        {
            _toolbar.Visible = !_toolbar.Visible;
            Log.Debug("NavigationBar toggled: Visible={Visible}", _toolbar.Visible);
        }

        /// <summary>
        /// Refreshes the localized strings (tooltips).
        /// </summary>
        public void RefreshLanguage()
        {
            _backButton.ToolTipText = _localization.GetString("NavigationBack");
            _forwardButton.ToolTipText = _localization.GetString("NavigationForward");

            Log.Debug("NavigationBar language refreshed");
        }
    }
}
