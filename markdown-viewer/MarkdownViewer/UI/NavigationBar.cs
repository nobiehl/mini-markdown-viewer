using System;
using System.Windows.Forms;
using MarkdownViewer.Core;
using MarkdownViewer.Services;
using Serilog;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Navigation toolbar with Back/Forward buttons.
    /// Integrates with NavigationManager for history navigation.
    /// </summary>
    public class NavigationBar
    {
        private readonly ToolStrip _toolbar;
        private readonly ToolStripButton _backButton;
        private readonly ToolStripButton _forwardButton;
        private readonly NavigationManager _navigationManager;
        private readonly ILocalizationService _localization;

        /// <summary>
        /// Gets the underlying ToolStrip control.
        /// </summary>
        public ToolStrip Control => _toolbar;

        /// <summary>
        /// Gets or sets whether the navigation bar is visible.
        /// </summary>
        public bool Visible
        {
            get => _toolbar.Visible;
            set => _toolbar.Visible = value;
        }

        /// <summary>
        /// Initializes a new instance of the NavigationBar.
        /// </summary>
        /// <param name="navigationManager">Navigation manager for history tracking</param>
        /// <param name="localization">Localization service for tooltips</param>
        public NavigationBar(NavigationManager navigationManager, ILocalizationService localization)
        {
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
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

            // Subscribe to navigation state changes
            _navigationManager.NavigationChanged += OnNavigationChanged;

            // Initialize button states
            UpdateButtonStates();

            Log.Debug("NavigationBar initialized");
        }

        private void OnBackButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Back button clicked");
            _navigationManager.GoBack();
        }

        private void OnForwardButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Forward button clicked");
            _navigationManager.GoForward();
        }

        private void OnNavigationChanged(object? sender, EventArgs e)
        {
            Log.Debug("Navigation state changed, updating button states");
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            _backButton.Enabled = _navigationManager.CanGoBack;
            _forwardButton.Enabled = _navigationManager.CanGoForward;

            Log.Debug("Button states updated: Back={BackEnabled}, Forward={ForwardEnabled}",
                _backButton.Enabled, _forwardButton.Enabled);
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
