using System;
using System.Drawing;
using System.Windows.Forms;
using MarkdownViewer.Core;
using MarkdownViewer.Services;
using Serilog;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Search toolbar with text input and navigation buttons.
    /// Integrates with SearchManager for in-page search functionality.
    /// </summary>
    public class SearchBar
    {
        private readonly ToolStrip _toolbar;
        private readonly ToolStripTextBox _searchTextBox;
        private readonly ToolStripLabel _resultsLabel;
        private readonly ToolStripButton _previousButton;
        private readonly ToolStripButton _nextButton;
        private readonly ToolStripButton _closeButton;
        private readonly SearchManager _searchManager;
        private readonly ILocalizationService _localization;

        /// <summary>
        /// Gets the underlying ToolStrip control.
        /// </summary>
        public ToolStrip Control => _toolbar;

        /// <summary>
        /// Gets or sets whether the search bar is visible.
        /// </summary>
        public bool Visible
        {
            get => _toolbar.Visible;
            set
            {
                _toolbar.Visible = value;
                if (value)
                {
                    _searchTextBox.Focus();
                }
            }
        }

        /// <summary>
        /// Event raised when the search bar is closed.
        /// </summary>
        public event EventHandler? Closed;

        /// <summary>
        /// Initializes a new instance of the SearchBar.
        /// </summary>
        public SearchBar(SearchManager searchManager, ILocalizationService localization)
        {
            _searchManager = searchManager ?? throw new ArgumentNullException(nameof(searchManager));
            _localization = localization ?? throw new ArgumentNullException(nameof(localization));

            // Create text box
            _searchTextBox = new ToolStripTextBox
            {
                Width = 200,
                ToolTipText = _localization.GetString("SearchPlaceholder")
            };
            _searchTextBox.TextChanged += OnSearchTextChanged;
            _searchTextBox.KeyDown += OnSearchTextBoxKeyDown;

            // Create results label
            _resultsLabel = new ToolStripLabel
            {
                Text = "",
                AutoSize = true
            };

            // Create previous button
            _previousButton = new ToolStripButton
            {
                Text = "▲",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ToolTipText = _localization.GetString("SearchPrevious"),
                Enabled = false
            };
            _previousButton.Click += OnPreviousButtonClick;

            // Create next button
            _nextButton = new ToolStripButton
            {
                Text = "▼",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ToolTipText = _localization.GetString("SearchNext"),
                Enabled = false
            };
            _nextButton.Click += OnNextButtonClick;

            // Create close button
            _closeButton = new ToolStripButton
            {
                Text = "✕",
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                ToolTipText = _localization.GetString("SearchClose")
            };
            _closeButton.Click += OnCloseButtonClick;

            // Create toolbar
            _toolbar = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                Visible = false // Hidden by default
            };

            _toolbar.Items.Add(new ToolStripLabel("🔍"));
            _toolbar.Items.Add(_searchTextBox);
            _toolbar.Items.Add(_resultsLabel);
            _toolbar.Items.Add(_previousButton);
            _toolbar.Items.Add(_nextButton);
            _toolbar.Items.Add(new ToolStripSeparator());
            _toolbar.Items.Add(_closeButton);

            // Subscribe to search results
            _searchManager.SearchResultsChanged += OnSearchResultsChanged;

            Log.Debug("SearchBar initialized");
        }

        private async void OnSearchTextChanged(object? sender, EventArgs e)
        {
            string searchTerm = _searchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                await _searchManager.ClearSearchAsync();
            }
            else
            {
                // Delay search to avoid too many requests while typing
                await System.Threading.Tasks.Task.Delay(300);

                // Check if text is still the same
                if (_searchTextBox.Text == searchTerm)
                {
                    await _searchManager.SearchAsync(searchTerm);
                }
            }
        }

        private void OnSearchTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            // Enter: Next match
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                _ = _searchManager.NextMatchAsync();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            // Shift+Enter: Previous match
            else if (e.KeyCode == Keys.Enter && e.Shift)
            {
                _ = _searchManager.PreviousMatchAsync();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            // Esc: Close search bar
            else if (e.KeyCode == Keys.Escape)
            {
                Close();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private async void OnPreviousButtonClick(object? sender, EventArgs e)
        {
            await _searchManager.PreviousMatchAsync();
        }

        private async void OnNextButtonClick(object? sender, EventArgs e)
        {
            await _searchManager.NextMatchAsync();
        }

        private void OnCloseButtonClick(object? sender, EventArgs e)
        {
            Close();
        }

        private void OnSearchResultsChanged(object? sender, SearchResultsEventArgs e)
        {
            // Update results label
            if (e.TotalMatches == 0)
            {
                _resultsLabel.Text = string.IsNullOrWhiteSpace(_searchTextBox.Text)
                    ? ""
                    : _localization.GetString("SearchNoResults");
                _resultsLabel.ForeColor = Color.Gray;
            }
            else
            {
                _resultsLabel.Text = _localization.GetString("SearchResults", e.CurrentMatch, e.TotalMatches);
                _resultsLabel.ForeColor = Color.Black;
            }

            // Update button states
            _previousButton.Enabled = e.TotalMatches > 0;
            _nextButton.Enabled = e.TotalMatches > 0;
        }

        /// <summary>
        /// Shows the search bar and focuses the text box.
        /// </summary>
        public void Show()
        {
            Log.Debug("Showing SearchBar");
            _toolbar.Visible = true;
            _searchTextBox.Focus();
            _searchTextBox.SelectAll();
        }

        /// <summary>
        /// Hides the search bar and clears the search.
        /// </summary>
        public async void Close()
        {
            Log.Debug("Closing SearchBar");
            await _searchManager.ClearSearchAsync();
            _searchTextBox.Text = "";
            _toolbar.Visible = false;

            Closed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Toggles the search bar visibility.
        /// </summary>
        public void Toggle()
        {
            if (_toolbar.Visible)
            {
                Close();
            }
            else
            {
                Show();
            }
        }

        /// <summary>
        /// Refreshes the localized strings.
        /// </summary>
        public void RefreshLanguage()
        {
            _searchTextBox.ToolTipText = _localization.GetString("SearchPlaceholder");
            _previousButton.ToolTipText = _localization.GetString("SearchPrevious");
            _nextButton.ToolTipText = _localization.GetString("SearchNext");
            _closeButton.ToolTipText = _localization.GetString("SearchClose");

            // Refresh results label if needed
            if (_searchManager.TotalMatches > 0)
            {
                _resultsLabel.Text = _localization.GetString("SearchResults", _searchManager.CurrentMatchIndex, _searchManager.TotalMatches);
            }

            Log.Debug("SearchBar language refreshed");
        }
    }
}
