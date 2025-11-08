using System;
using System.Drawing;
using System.Windows.Forms;
using MarkdownViewer.Services;
using MarkdownViewer.Views;
using Serilog;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Search toolbar with text input and navigation buttons.
    /// Implements ISearchBarView for MVP pattern (v2.0.0).
    /// </summary>
    public class SearchBar : ISearchBarView
    {
        private readonly ToolStrip _toolbar;
        private readonly ToolStripTextBox _searchTextBox;
        private readonly ToolStripLabel _resultsLabel;
        private readonly ToolStripButton _previousButton;
        private readonly ToolStripButton _nextButton;
        private readonly ToolStripButton _closeButton;
        private readonly ILocalizationService _localization;

        private bool _isCaseSensitive;
        private int _currentMatch;
        private int _totalMatches;

        /// <summary>
        /// Gets the underlying ToolStrip control.
        /// </summary>
        public ToolStrip Control => _toolbar;

        #region ISearchBarView Implementation

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string SearchText
        {
            get => _searchTextBox.Text;
            set => _searchTextBox.Text = value;
        }

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
        /// Gets or sets whether the search is case-sensitive.
        /// </summary>
        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set => _isCaseSensitive = value;
        }

        // Events (View → Presenter)
        public event EventHandler<SearchRequestedEventArgs>? SearchRequested;
        public event EventHandler? FindNextRequested;
        public event EventHandler? FindPreviousRequested;
        public event EventHandler? CloseRequested;

        #endregion

        /// <summary>
        /// Initializes a new instance of the SearchBar.
        /// </summary>
        public SearchBar(ILocalizationService localization)
        {
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

            Log.Debug("SearchBar initialized (MVP pattern)");
        }

        private async void OnSearchTextChanged(object? sender, EventArgs e)
        {
            string searchTerm = _searchTextBox.Text;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // Clear search - raise event
                Log.Debug("Search text cleared, raising SearchRequested event with empty text");
                SearchRequested?.Invoke(this, new SearchRequestedEventArgs("", _isCaseSensitive));
            }
            else
            {
                // Delay search to avoid too many requests while typing (debouncing)
                await System.Threading.Tasks.Task.Delay(300);

                // Check if text is still the same
                if (_searchTextBox.Text == searchTerm)
                {
                    Log.Debug("Search text changed (after debounce): {SearchTerm}, raising SearchRequested event", searchTerm);
                    SearchRequested?.Invoke(this, new SearchRequestedEventArgs(searchTerm, _isCaseSensitive));
                }
            }
        }

        private void OnSearchTextBoxKeyDown(object? sender, KeyEventArgs e)
        {
            // Enter: Next match
            if (e.KeyCode == Keys.Enter && !e.Shift)
            {
                Log.Debug("Enter pressed, raising FindNextRequested event");
                FindNextRequested?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            // Shift+Enter: Previous match
            else if (e.KeyCode == Keys.Enter && e.Shift)
            {
                Log.Debug("Shift+Enter pressed, raising FindPreviousRequested event");
                FindPreviousRequested?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
            // Esc: Close search bar
            else if (e.KeyCode == Keys.Escape)
            {
                Log.Debug("Escape pressed, raising CloseRequested event");
                CloseRequested?.Invoke(this, EventArgs.Empty);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void OnPreviousButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Previous button clicked, raising FindPreviousRequested event");
            FindPreviousRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnNextButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Next button clicked, raising FindNextRequested event");
            FindNextRequested?.Invoke(this, EventArgs.Empty);
        }

        private void OnCloseButtonClick(object? sender, EventArgs e)
        {
            Log.Debug("Close button clicked, raising CloseRequested event");
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Updates the search results display.
        /// Called by presenter when search results change.
        /// </summary>
        public void UpdateResults(int currentMatch, int totalMatches)
        {
            // Store for language refresh
            _currentMatch = currentMatch;
            _totalMatches = totalMatches;

            // Update results label
            if (totalMatches == 0)
            {
                _resultsLabel.Text = string.IsNullOrWhiteSpace(_searchTextBox.Text)
                    ? ""
                    : _localization.GetString("SearchNoResults");
                _resultsLabel.ForeColor = Color.Gray;
            }
            else
            {
                _resultsLabel.Text = _localization.GetString("SearchResults", currentMatch, totalMatches);
                _resultsLabel.ForeColor = Color.Black;
            }

            // Update button states
            _previousButton.Enabled = totalMatches > 0;
            _nextButton.Enabled = totalMatches > 0;

            Log.Debug("Search results updated: {CurrentMatch}/{TotalMatches}", currentMatch, totalMatches);
        }

        /// <summary>
        /// Clears the search text and results.
        /// Called by presenter.
        /// </summary>
        public void ClearSearch()
        {
            _searchTextBox.Text = "";
            _resultsLabel.Text = "";
            _previousButton.Enabled = false;
            _nextButton.Enabled = false;
            Log.Debug("Search cleared");
        }

        /// <summary>
        /// Focuses the search text box.
        /// Called by presenter.
        /// </summary>
        public void Focus()
        {
            _searchTextBox.Focus();
            _searchTextBox.SelectAll();
            Log.Debug("Search bar focused");
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
        public void Close()
        {
            Log.Debug("Closing SearchBar");
            _searchTextBox.Text = "";
            _toolbar.Visible = false;

            // Raise CloseRequested so presenter can clear search
            CloseRequested?.Invoke(this, EventArgs.Empty);
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
            if (_totalMatches > 0)
            {
                _resultsLabel.Text = _localization.GetString("SearchResults", _currentMatch, _totalMatches);
            }

            Log.Debug("SearchBar language refreshed");
        }
    }
}
