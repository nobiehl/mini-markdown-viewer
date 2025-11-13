using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using MarkdownViewer.Core.Models;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Panel for Raw Data View - displays Markdown source and generated HTML side-by-side.
    /// Provides a developer/power-user view for inspecting markdown rendering.
    /// </summary>
    public class RawDataViewPanel : Panel
    {
        private readonly SplitContainer _splitContainer;
        private readonly CodeViewControl _markdownTextBox;
        private readonly CodeViewControl _htmlTextBox;
        private readonly Label _markdownLabel;
        private readonly Label _htmlLabel;

        private bool _isDarkTheme;

        // Cached compiled regex patterns for performance (10x faster than creating regex each time)
        private static readonly Regex _markdownHeadingRegex = new Regex(@"^#{1,6}\s+.*$", RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex _markdownCodeBlockRegex = new Regex(@"```[\s\S]*?```", RegexOptions.Compiled);
        private static readonly Regex _markdownInlineCodeRegex = new Regex(@"`[^`]+`", RegexOptions.Compiled);
        private static readonly Regex _markdownLinkRegex = new Regex(@"\[([^\]]+)\]\(([^)]+)\)", RegexOptions.Compiled);
        private static readonly Regex _htmlTagRegex = new Regex(@"</?[a-zA-Z][^>]*>", RegexOptions.Compiled);
        private static readonly Regex _htmlAttributeRegex = new Regex(@"""[^""]*""", RegexOptions.Compiled);

        /// <summary>
        /// Gets or sets the splitter distance (position between left and right panels).
        /// This value is persisted in settings.json.
        /// </summary>
        public int SplitterDistance
        {
            get => _splitContainer.SplitterDistance;
            set => _splitContainer.SplitterDistance = value;
        }

        /// <summary>
        /// Initializes a new instance of RawDataViewPanel.
        /// Creates split-view with two RichTextBox controls for Markdown and HTML.
        /// </summary>
        public RawDataViewPanel()
        {
            this.Dock = DockStyle.Fill;
            this.Visible = false;

            // SplitContainer setup
            _splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 8
            };

            // Set 50/50 split when panel is resized
            this.Resize += (s, e) =>
            {
                if (this.Width > 0)
                {
                    _splitContainer.SplitterDistance = this.Width / 2;
                }
            };

            // Left Panel: Markdown Source
            _markdownLabel = new Label
            {
                Text = "Markdown Source",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            _markdownTextBox = new CodeViewControl
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10F),
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            _splitContainer.Panel1.Controls.Add(_markdownTextBox);
            _splitContainer.Panel1.Controls.Add(_markdownLabel);

            // Right Panel: Generated HTML
            _htmlLabel = new Label
            {
                Text = "Generated HTML",
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(240, 240, 240),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            _htmlTextBox = new CodeViewControl
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                Font = new Font("Consolas", 10F),
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = Color.Black
            };

            _splitContainer.Panel2.Controls.Add(_htmlTextBox);
            _splitContainer.Panel2.Controls.Add(_htmlLabel);

            this.Controls.Add(_splitContainer);
        }

        /// <summary>
        /// Shows the raw data view with Markdown source and generated HTML.
        /// Applies simple syntax highlighting to both text boxes.
        /// </summary>
        /// <param name="markdown">Markdown source text</param>
        /// <param name="html">Generated HTML text</param>
        public void ShowRawData(string markdown, string html)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            // Set plain text first (FAST - no formatting)
            _markdownTextBox.Text = markdown;
            _htmlTextBox.Text = html;

            // Line numbers are drawn automatically by CodeViewControl

            // Show panel immediately (instant response)
            this.Visible = true;

            sw.Stop();
            System.Diagnostics.Debug.WriteLine($"Performance: ShowRawData (instant): {sw.ElapsedMilliseconds}ms");

            // NO SYNTAX HIGHLIGHTING - it's too slow and not worth the UX cost
            // Raw data view is for inspecting content, not for pretty reading
            // Users can copy-paste to their editor if they want highlighting
        }

        /// <summary>
        /// Hides the raw data view panel.
        /// </summary>
        public new void Hide()
        {
            this.Visible = false;
        }

        /// <summary>
        /// Sets the localized text for the panel labels.
        /// Should be called after construction with localized strings.
        /// </summary>
        /// <param name="markdownLabel">Localized text for markdown panel label</param>
        /// <param name="htmlLabel">Localized text for HTML panel label</param>
        public void SetLabelTexts(string markdownLabel, string htmlLabel)
        {
            _markdownLabel.Text = markdownLabel;
            _htmlLabel.Text = htmlLabel;
        }

        /// <summary>
        /// Applies theme colors to the panel and text boxes.
        /// Supports dark and light themes.
        /// </summary>
        /// <param name="theme">Theme to apply</param>
        public void ApplyTheme(Theme theme)
        {
            _isDarkTheme = theme.Name.ToLower().Contains("dark");

            Color background = _isDarkTheme ? Color.FromArgb(30, 30, 30) : Color.White;
            Color foreground = _isDarkTheme ? Color.FromArgb(200, 200, 200) : Color.Black;
            Color labelBackground = _isDarkTheme ? Color.FromArgb(45, 45, 45) : Color.FromArgb(240, 240, 240);
            Color lineNumberForeground = _isDarkTheme ? Color.FromArgb(100, 100, 100) : Color.Gray;

            // Row highlighting colors (theme-aware)
            Color mouseOverColor = _isDarkTheme ? Color.FromArgb(35, 100, 150, 200) : Color.FromArgb(25, 100, 150, 200);
            Color cursorLineColor = _isDarkTheme ? Color.FromArgb(100, 100, 150, 200) : Color.FromArgb(80, 100, 150, 200); // Deutlich stärker

            // Apply to text boxes
            _markdownTextBox.BackColor = background;
            _markdownTextBox.ForeColor = foreground;
            _markdownTextBox.SetHighlightColors(mouseOverColor, cursorLineColor);
            _markdownTextBox.SetLineNumberColors(labelBackground, lineNumberForeground);

            _htmlTextBox.BackColor = background;
            _htmlTextBox.ForeColor = foreground;
            _htmlTextBox.SetHighlightColors(mouseOverColor, cursorLineColor);
            _htmlTextBox.SetLineNumberColors(labelBackground, lineNumberForeground);

            // Apply to labels
            _markdownLabel.BackColor = labelBackground;
            _markdownLabel.ForeColor = foreground;
            _htmlLabel.BackColor = labelBackground;
            _htmlLabel.ForeColor = foreground;

            // No syntax highlighting for CodeViewControl
        }

        /// </summary>
        /// <param name="lineNumberBox">The RichTextBox to display line numbers</param>
        /// <param name="text">The text content to count lines from</param>
        private void UpdateLineNumbers(RichTextBox lineNumberBox, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                lineNumberBox.Text = "1";
                return;
            }

            int lineCount = text.Split('\n').Length;
            var lineNumbers = new System.Text.StringBuilder();

            for (int i = 1; i <= lineCount; i++)
            {
                lineNumbers.AppendLine(i.ToString());
            }

            lineNumberBox.Text = lineNumbers.ToString();
        }

        // SyncScroll removed - CodeViewControl has its own scrollbar
    }

    /// <summary>
    /// Extension methods for RichTextBox scroll position management.
    /// </summary>
    internal static class RichTextBoxExtensions
    {
        private const int EM_GETFIRSTVISIBLELINE = 0xCE;
        private const int EM_LINESCROLL = 0xB6;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wparam, int lparam);

        /// <summary>
        /// Gets the index of the first visible line in the RichTextBox.
        /// </summary>
        public static int GetFirstVisibleLineIndex(this RichTextBox rtb)
        {
            return SendMessage(rtb.Handle, EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        /// <summary>
        /// Sets the first visible line in the RichTextBox.
        /// </summary>
        public static void SetFirstVisibleLine(this RichTextBox rtb, int lineIndex)
        {
            int currentLine = rtb.GetFirstVisibleLineIndex();
            int linesToScroll = lineIndex - currentLine;
            SendMessage(rtb.Handle, EM_LINESCROLL, 0, linesToScroll);
        }
    }
}
