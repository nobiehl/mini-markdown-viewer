using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Win32;
using MarkdownViewer.Services;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Status bar control with 5 sections:
    /// 1. Update status (icon with tooltip)
    /// 2. Explorer registration status (icon with tooltip)
    /// 3. Language selector (dropdown)
    /// 4. Info (clickable label)
    /// 5. Help (clickable label)
    ///
    /// StatusBar is hidden by default according to settings.
    /// </summary>
    public class StatusBarControl : StatusStrip
    {
        // Services
        private readonly ILocalizationService _localization;

        // Status items
        private readonly ToolStripStatusLabel _updateStatus;
        private readonly ToolStripStatusLabel _explorerStatus;
        private readonly ToolStripDropDownButton _languageSelector;
        private readonly ToolStripStatusLabel _infoLabel;
        private readonly ToolStripStatusLabel _helpLabel;

        // Events
        public event EventHandler? LanguageChanged;
        public event EventHandler? InfoClicked;
        public event EventHandler? HelpClicked;

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

            // Create status items
            _updateStatus = new ToolStripStatusLabel
            {
                Text = "🔄",
                ToolTipText = _localization.GetString("StatusBarUpToDate"),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true
            };

            _explorerStatus = new ToolStripStatusLabel
            {
                Text = "📁",
                ToolTipText = _localization.GetString("StatusBarExplorerNotRegistered"),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true
            };

            _languageSelector = new ToolStripDropDownButton
            {
                Text = GetLanguageDisplayName(_localization.GetCurrentLanguage()),
                ToolTipText = _localization.GetString("StatusBarLanguage", _localization.GetCurrentLanguage().ToUpper()),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true,
                ShowDropDownArrow = true
            };

            // Populate language dropdown
            PopulateLanguageDropdown();

            _infoLabel = new ToolStripStatusLabel
            {
                Text = _localization.GetString("StatusBarInfo"),
                ToolTipText = _localization.GetString("StatusBarInfo"),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true,
                IsLink = true
            };

            _infoLabel.Click += (s, e) => InfoClicked?.Invoke(this, EventArgs.Empty);

            _helpLabel = new ToolStripStatusLabel
            {
                Text = _localization.GetString("StatusBarHelp"),
                ToolTipText = _localization.GetString("StatusBarHelp"),
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                AutoSize = true,
                IsLink = true
            };

            _helpLabel.Click += (s, e) => HelpClicked?.Invoke(this, EventArgs.Empty);

            // Add items to status bar (left to right)
            this.Items.Add(_updateStatus);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(_explorerStatus);
            this.Items.Add(new ToolStripSeparator());

            // Add spring to push language selector to the center-right
            var spring = new ToolStripStatusLabel
            {
                Spring = true
            };
            this.Items.Add(spring);

            this.Items.Add(_languageSelector);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(_infoLabel);
            this.Items.Add(new ToolStripSeparator());
            this.Items.Add(_helpLabel);

            // Initialize status
            CheckExplorerRegistration();
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
                LanguageChanged?.Invoke(this, EventArgs.Empty);
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
                    _updateStatus.Text = "✅";
                    _updateStatus.ToolTipText = _localization.GetString("StatusBarUpToDate");
                    _updateStatus.ForeColor = Color.Green;
                    break;

                case UpdateStatus.UpdateAvailable:
                    _updateStatus.Text = "🔄";
                    _updateStatus.ToolTipText = _localization.GetString("StatusBarUpdateAvailable", latestVersion ?? "?");
                    _updateStatus.ForeColor = Color.Orange;
                    break;

                case UpdateStatus.Checking:
                    _updateStatus.Text = "⏳";
                    _updateStatus.ToolTipText = "Checking for updates...";
                    _updateStatus.ForeColor = Color.Gray;
                    break;

                case UpdateStatus.Error:
                    _updateStatus.Text = "❌";
                    _updateStatus.ToolTipText = "Update check failed";
                    _updateStatus.ForeColor = Color.Red;
                    break;

                case UpdateStatus.Unknown:
                default:
                    _updateStatus.Text = "❓";
                    _updateStatus.ToolTipText = "Update status unknown";
                    _updateStatus.ForeColor = Color.Gray;
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
                _explorerStatus.Text = "✅📁";
                _explorerStatus.ToolTipText = _localization.GetString("StatusBarExplorerRegistered");
                _explorerStatus.ForeColor = Color.Green;
            }
            else
            {
                _explorerStatus.Text = "❌📁";
                _explorerStatus.ToolTipText = _localization.GetString("StatusBarExplorerNotRegistered");
                _explorerStatus.ForeColor = Color.Gray;
            }
        }

        /// <summary>
        /// Refreshes all UI text with current language.
        /// </summary>
        public void RefreshLanguage()
        {
            // Update language selector
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

            // Update info and help labels
            _infoLabel.Text = _localization.GetString("StatusBarInfo");
            _infoLabel.ToolTipText = _localization.GetString("StatusBarInfo");
            _helpLabel.Text = _localization.GetString("StatusBarHelp");
            _helpLabel.ToolTipText = _localization.GetString("StatusBarHelp");

            // Update status tooltips
            UpdateExplorerStatus();
            SetUpdateStatus(UpdateStatusValue, null); // Refresh without changing status
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
