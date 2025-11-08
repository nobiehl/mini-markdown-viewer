using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Win32;
using MarkdownViewer.Services;
using MarkdownViewer.Views;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Status bar control with 7 sections.
    /// Implements IStatusBarView for MVP pattern (v2.0.0).
    ///
    /// Sections:
    /// 1. Update status (icon with tooltip)
    /// 2. Explorer registration status (icon with tooltip)
    /// 3. Spring (pushes items to the right)
    /// 4. Theme selector (dropdown)
    /// 5. Language selector (dropdown)
    /// 6. Info (clickable label)
    /// 7. Help (clickable label)
    ///
    /// StatusBar is hidden by default according to settings.
    /// </summary>
    public class StatusBarControl : StatusStrip, IStatusBarView
    {
        // Services
        private readonly ILocalizationService _localization;

        // Status items
        private readonly ToolStripStatusLabel _updateStatus;
        private readonly ToolStripStatusLabel _explorerStatus;
        private readonly ToolStripDropDownButton _themeSelector;
        private readonly ToolStripDropDownButton _languageSelector;
        private readonly ToolStripStatusLabel _infoLabel;
        private readonly ToolStripStatusLabel _helpLabel;

        // Cached icons for status changes
        private readonly Bitmap? _iconRefreshCw;
        private readonly Bitmap? _iconCheckCircle;
        private readonly Bitmap? _iconAlertCircle;
        private readonly Bitmap? _iconFolder;
        private readonly Bitmap? _iconGlobe;
        private readonly Bitmap? _iconInfo;
        private readonly Bitmap? _iconHelpCircle;

        // Additional events (not in IStatusBarView, but useful)
        public event EventHandler? UpdateClicked;
        public event EventHandler? ExplorerClicked;
        public event EventHandler? InfoClicked;
        public event EventHandler? HelpClicked;

        #region IStatusBarView Implementation

        /// <summary>
        /// Gets or sets the current theme name.
        /// </summary>
        public string CurrentTheme { get; set; } = "standard";

        /// <summary>
        /// Gets or sets the current language code.
        /// </summary>
        public string CurrentLanguage
        {
            get => _localization.GetCurrentLanguage();
            set => _localization.SetLanguage(value);
        }

        // Events required by IStatusBarView
        public event EventHandler<ThemeChangedEventArgs>? ThemeChanged;
        public event EventHandler<LanguageChangedEventArgs>? LanguageChanged;

        #endregion

        /// <summary>
        /// Gets or sets the current update status.
        /// </summary>
        public UpdateStatus UpdateStatusValue { get; private set; } = UpdateStatus.Unknown;

        /// <summary>
        /// Gets or sets the current explorer registration status.
        /// </summary>
        public bool IsExplorerRegistered { get; private set; } = false;

        /// <summary>
        /// Initializes a new instance of the StatusBarControl.
        /// </summary>
        /// <param name="localization">Localization service for multi-language support</param>
        public StatusBarControl(ILocalizationService localization)
        {
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // StatusBar configuration
            this.SizingGrip = false;
            this.Dock = DockStyle.Bottom;
            this.ShowItemToolTips = true; // Enable tooltips for status items!

            // Load Feather Icons (20x20, dark gray color for better visibility)
            var iconColor = Color.FromArgb(64, 64, 64); // Dark gray
            _iconRefreshCw = IconHelper.LoadIcon("refresh-cw", 20, iconColor);
            _iconCheckCircle = IconHelper.LoadIcon("check-circle", 20, iconColor);
            _iconAlertCircle = IconHelper.LoadIcon("alert-circle", 20, iconColor);
            _iconFolder = IconHelper.LoadIcon("folder", 20, iconColor);
            _iconGlobe = IconHelper.LoadIcon("globe", 20, iconColor);
            _iconInfo = IconHelper.LoadIcon("info", 20, iconColor);
            _iconHelpCircle = IconHelper.LoadIcon("help-circle", 20, iconColor);

            // Create status items
            _updateStatus = new ToolStripStatusLabel
            {
                Image = _iconRefreshCw,
                ToolTipText = _localization.GetString("StatusBarUpToDate"),
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                AutoSize = true,
                IsLink = true,
                Padding = new Padding(5, 0, 5, 0)
            };

            _updateStatus.Click += (s, e) => UpdateClicked?.Invoke(this, EventArgs.Empty);

            _explorerStatus = new ToolStripStatusLabel
            {
                Image = _iconFolder,
                ToolTipText = _localization.GetString("StatusBarExplorerNotRegistered"),
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                AutoSize = true,
                IsLink = true,
                Padding = new Padding(5, 0, 5, 0)
            };

            _explorerStatus.Click += (s, e) => ExplorerClicked?.Invoke(this, EventArgs.Empty);

            // NEW: Theme selector
            _themeSelector = new ToolStripDropDownButton
            {
                Text = "Theme: Standard",
                ToolTipText = "Select theme for Markdown rendering and UI",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true,
                ShowDropDownArrow = true,
                Padding = new Padding(5, 0, 5, 0)
            };

            // Populate theme dropdown (will be populated from MainForm)
            // Empty for now, will be filled via PopulateThemeDropdown()

            _languageSelector = new ToolStripDropDownButton
            {
                Image = _iconGlobe,
                Text = GetLanguageDisplayName(_localization.GetCurrentLanguage()),
                ToolTipText = _localization.GetString("StatusBarLanguage", _localization.GetCurrentLanguage().ToUpper()),
                DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                AutoSize = true,
                ShowDropDownArrow = true,
                Padding = new Padding(5, 0, 5, 0)
            };

            // Populate language dropdown
            PopulateLanguageDropdown();

            _infoLabel = new ToolStripStatusLabel
            {
                Image = _iconInfo,
                ToolTipText = _localization.GetString("StatusBarInfo"),
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                AutoSize = true,
                IsLink = true,
                Padding = new Padding(5, 0, 5, 0)
            };

            _infoLabel.Click += (s, e) => InfoClicked?.Invoke(this, EventArgs.Empty);

            _helpLabel = new ToolStripStatusLabel
            {
                Image = _iconHelpCircle,
                ToolTipText = _localization.GetString("StatusBarHelp"),
                DisplayStyle = ToolStripItemDisplayStyle.Image,
                AutoSize = true,
                IsLink = true,
                Padding = new Padding(5, 0, 5, 0)
            };

            _helpLabel.Click += (s, e) => HelpClicked?.Invoke(this, EventArgs.Empty);

            // Add items to status bar (left to right)
            this.Items.Add(_updateStatus);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(_explorerStatus);
            this.Items.Add(new ToolStripSeparator());

            // Add spring to push theme/language selector to the center-right
            var spring = new ToolStripStatusLabel
            {
                Spring = true
            };
            this.Items.Add(spring);

            // NEW: Add theme selector (right side, before language)
            this.Items.Add(_themeSelector);
            this.Items.Add(new ToolStripSeparator());

            this.Items.Add(_languageSelector);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(_infoLabel);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(_helpLabel);

            // Initialize status
            CheckExplorerRegistration();
        }

        /// <summary>
        /// Populates the theme dropdown with available themes.
        /// </summary>
        /// <param name="availableThemes">List of theme names (e.g., "dark", "standard")</param>
        /// <param name="currentTheme">Currently selected theme name</param>
        public void PopulateThemeDropdown(System.Collections.Generic.List<string> availableThemes, string currentTheme)
        {
            _themeSelector.DropDownItems.Clear();
            CurrentTheme = currentTheme;

            foreach (string themeName in availableThemes)
            {
                string displayName = GetThemeDisplayName(themeName);
                var item = new ToolStripMenuItem(displayName)
                {
                    Tag = themeName,
                    Checked = themeName == currentTheme
                };

                item.Click += OnThemeItemClick;
                _themeSelector.DropDownItems.Add(item);
            }

            // Update theme selector text
            _themeSelector.Text = $"Theme: {GetThemeDisplayName(currentTheme)}";
        }

        /// <summary>
        /// Handles theme selection from dropdown.
        /// </summary>
        private void OnThemeItemClick(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is string themeName)
            {
                // Update current theme
                CurrentTheme = themeName;

                // Update UI
                foreach (ToolStripMenuItem menuItem in _themeSelector.DropDownItems)
                {
                    menuItem.Checked = menuItem.Tag as string == themeName;
                }

                _themeSelector.Text = $"Theme: {GetThemeDisplayName(themeName)}";

                // Raise event for MainForm to handle
                ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(themeName));
            }
        }

        /// <summary>
        /// Gets the display name for a theme name.
        /// </summary>
        private string GetThemeDisplayName(string themeName)
        {
            return themeName switch
            {
                "dark" => "Dark",
                "standard" => "Standard",
                "solarized" => "Solarized",
                "draeger" => "Dräger",
                _ => char.ToUpper(themeName[0]) + themeName.Substring(1)
            };
        }

        /// <summary>
        /// Populates the language dropdown with all supported languages.
        /// </summary>
        private void PopulateLanguageDropdown()
        {
            _languageSelector.DropDownItems.Clear();

            foreach (string langCode in _localization.GetSupportedLanguages())
            {
                string displayName = GetLanguageDisplayName(langCode);
                var item = new ToolStripMenuItem(displayName)
                {
                    Tag = langCode,
                    Checked = langCode == _localization.GetCurrentLanguage()
                };

                item.Click += OnLanguageItemClick;
                _languageSelector.DropDownItems.Add(item);
            }
        }

        /// <summary>
        /// Handles language selection from dropdown.
        /// </summary>
        private void OnLanguageItemClick(object? sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is string langCode)
            {
                // Update localization
                _localization.SetLanguage(langCode);

                // Update UI
                RefreshLanguage();

                // Raise event
                LanguageChanged?.Invoke(this, new LanguageChangedEventArgs(langCode));
            }
        }

        /// <summary>
        /// Gets the display name for a language code.
        /// Uses the localized language name from resources.
        /// </summary>
        private string GetLanguageDisplayName(string langCode)
        {
            return langCode.ToLower() switch
            {
                "en" => _localization.GetString("LanguageEnglish"),
                "de" => _localization.GetString("LanguageGerman"),
                "mn" => _localization.GetString("LanguageMongolian"),
                "fr" => _localization.GetString("LanguageFrench"),
                "es" => _localization.GetString("LanguageSpanish"),
                "ja" => _localization.GetString("LanguageJapanese"),
                "zh" => _localization.GetString("LanguageChinese"),
                "ru" => _localization.GetString("LanguageRussian"),
                _ => langCode.ToUpper()
            };
        }

        /// <summary>
        /// Updates the update status icon and tooltip.
        /// </summary>
        /// <param name="status">Current update status</param>
        /// <param name="latestVersion">Latest version if update is available</param>
        public void SetUpdateStatus(UpdateStatus status, string? latestVersion = null)
        {
            UpdateStatusValue = status;

            switch (status)
            {
                case UpdateStatus.UpToDate:
                    _updateStatus.Image = _iconCheckCircle;
                    _updateStatus.ToolTipText = _localization.GetString("StatusBarUpToDate") + " (Click to check)";
                    break;

                case UpdateStatus.UpdateAvailable:
                    _updateStatus.Image = _iconRefreshCw;
                    _updateStatus.ToolTipText = _localization.GetString("StatusBarUpdateAvailable", latestVersion ?? "?") + " (Click to download)";
                    break;

                case UpdateStatus.Checking:
                    _updateStatus.Image = _iconRefreshCw;
                    _updateStatus.ToolTipText = "Checking for updates...";
                    break;

                case UpdateStatus.Error:
                    _updateStatus.Image = _iconAlertCircle;
                    _updateStatus.ToolTipText = "Update check failed (Click to retry)";
                    break;

                case UpdateStatus.Unknown:
                default:
                    _updateStatus.Image = _iconRefreshCw;
                    _updateStatus.ToolTipText = "Update status unknown (Click to check)";
                    break;
            }
        }

        /// <summary>
        /// Checks and updates the Windows Explorer registration status.
        /// </summary>
        public void CheckExplorerRegistration()
        {
            try
            {
                // Check if .md file association exists in HKCU
                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\.md");
                IsExplorerRegistered = key != null && key.GetValue("") as string == "MarkdownViewer.Document";

                UpdateExplorerStatus();
            }
            catch (Exception)
            {
                IsExplorerRegistered = false;
                UpdateExplorerStatus();
            }
        }

        /// <summary>
        /// Updates the explorer status icon and tooltip.
        /// </summary>
        private void UpdateExplorerStatus()
        {
            if (IsExplorerRegistered)
            {
                _explorerStatus.Image = _iconCheckCircle;
                _explorerStatus.ToolTipText = _localization.GetString("StatusBarExplorerRegistered") + " (Click to uninstall)";
            }
            else
            {
                _explorerStatus.Image = _iconFolder;
                _explorerStatus.ToolTipText = _localization.GetString("StatusBarExplorerNotRegistered") + " (Click to install)";
            }
        }

        /// <summary>
        /// Refreshes all UI text with current language.
        /// </summary>
        public void RefreshLanguage()
        {
            // Update language selector (globe icon is set as Image property)
            string currentLang = _localization.GetCurrentLanguage();
            _languageSelector.Text = GetLanguageDisplayName(currentLang);
            _languageSelector.ToolTipText = _localization.GetString("StatusBarLanguage", currentLang.ToUpper());

            // Update dropdown items
            foreach (ToolStripMenuItem item in _languageSelector.DropDownItems)
            {
                if (item.Tag is string langCode)
                {
                    item.Text = GetLanguageDisplayName(langCode);
                    item.Checked = langCode == currentLang;
                }
            }

            // Info and help labels keep their icons, only update tooltips
            _infoLabel.ToolTipText = _localization.GetString("StatusBarInfo");
            _helpLabel.ToolTipText = _localization.GetString("StatusBarHelp");

            // Update status tooltips
            UpdateExplorerStatus();
            SetUpdateStatus(UpdateStatusValue, null); // Refresh without changing status
        }

        #region IStatusBarView Methods

        /// <summary>
        /// Updates the available themes in the theme selector dropdown.
        /// Called by presenter.
        /// </summary>
        public void UpdateAvailableThemes(IEnumerable<string> themeNames)
        {
            var themes = themeNames?.ToList() ?? new List<string>();
            PopulateThemeDropdown(themes, CurrentTheme);
        }

        /// <summary>
        /// Updates the available languages in the language selector dropdown.
        /// Called by presenter.
        /// </summary>
        public void UpdateAvailableLanguages(IEnumerable<string> languageCodes)
        {
            // Language dropdown is populated in constructor
            // This method allows dynamic update if needed in future
            var languages = languageCodes?.ToList() ?? new List<string>();

            _languageSelector.DropDownItems.Clear();

            foreach (var langCode in languages)
            {
                var menuItem = new ToolStripMenuItem
                {
                    Text = GetLanguageDisplayName(langCode),
                    Tag = langCode,
                    Checked = langCode == CurrentLanguage
                };

                menuItem.Click += OnLanguageItemClick;
                _languageSelector.DropDownItems.Add(menuItem);
            }
        }

        /// <summary>
        /// Sets a general status message.
        /// Currently not displayed (reserved for future use).
        /// </summary>
        public void SetStatus(string message)
        {
            // TODO: Could add a general status label in future
            // For now, this is a no-op
        }

        /// <summary>
        /// Sets the line and column information.
        /// Currently not displayed (reserved for future use).
        /// </summary>
        public void SetLineInfo(int lineNumber, int columnNumber)
        {
            // TODO: Could add line/column indicator in future (like VS Code)
            // For now, this is a no-op
        }

        #endregion

        /// <summary>
        /// Applies theme colors to icons in the StatusBar.
        /// Reloads all icons with the foreground color from the theme.
        /// </summary>
        /// <param name="theme">Theme to apply</param>
        public void ApplyTheme(Models.Theme theme)
        {
            try
            {
                // Parse icon color from theme
                var iconColor = System.Drawing.ColorTranslator.FromHtml(theme.UI.StatusBarForeground);

                // Reload icons with theme color
                var newRefreshIcon = IconHelper.LoadIcon("refresh-cw", 20, iconColor);
                var newCheckIcon = IconHelper.LoadIcon("check-circle", 20, iconColor);
                var newAlertIcon = IconHelper.LoadIcon("alert-circle", 20, iconColor);
                var newFolderIcon = IconHelper.LoadIcon("folder", 20, iconColor);
                var newGlobeIcon = IconHelper.LoadIcon("globe", 20, iconColor);
                var newInfoIcon = IconHelper.LoadIcon("info", 20, iconColor);
                var newHelpIcon = IconHelper.LoadIcon("help-circle", 20, iconColor);

                // Update status items with new icons
                if (newRefreshIcon != null) _updateStatus.Image = newRefreshIcon;
                if (newFolderIcon != null) _explorerStatus.Image = newFolderIcon;
                if (newGlobeIcon != null) _languageSelector.Image = newGlobeIcon;
                if (newInfoIcon != null) _infoLabel.Image = newInfoIcon;
                if (newHelpIcon != null) _helpLabel.Image = newHelpIcon;

                // Update icons based on current status
                switch (UpdateStatusValue)
                {
                    case UpdateStatus.Checking:
                        if (newRefreshIcon != null) _updateStatus.Image = newRefreshIcon;
                        break;
                    case UpdateStatus.UpToDate:
                        if (newCheckIcon != null) _updateStatus.Image = newCheckIcon;
                        break;
                    case UpdateStatus.UpdateAvailable:
                        if (newRefreshIcon != null) _updateStatus.Image = newRefreshIcon;
                        break;
                    case UpdateStatus.Error:
                        if (newAlertIcon != null) _updateStatus.Image = newAlertIcon;
                        break;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                System.Diagnostics.Debug.WriteLine($"Failed to apply theme to StatusBar icons: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Enum representing update status states.
    /// </summary>
    public enum UpdateStatus
    {
        Unknown,
        Checking,
        UpToDate,
        UpdateAvailable,
        Error
    }
}
