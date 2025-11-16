using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Panel that displays line numbers synchronized with a RichTextBox.
    /// </summary>
    public class LineNumberPanel : Control
    {
        private RichTextBox? _targetTextBox;
        private Color _backColor = Color.FromArgb(240, 240, 240);
        private Color _foreColor = Color.Gray;
        private Color _highlightBackColor = Color.FromArgb(220, 220, 220);
        private Font _font = new Font("Consolas", 10F);
        private int _highlightedLine = -1;

        public const int DefaultWidth = 50;

        public LineNumberPanel()
        {
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            this.DoubleBuffered = true;
            this.Width = DefaultWidth;
            this.Dock = DockStyle.Left;
        }

        /// <summary>
        /// Attaches this line number panel to a RichTextBox.
        /// </summary>
        public void AttachToTextBox(RichTextBox textBox)
        {
            if (_targetTextBox != null)
            {
                _targetTextBox.VScroll -= OnTextBoxScroll;
                _targetTextBox.TextChanged -= OnTextBoxTextChanged;
                _targetTextBox.Resize -= OnTextBoxResize;
            }

            _targetTextBox = textBox;

            if (_targetTextBox != null)
            {
                _targetTextBox.VScroll += OnTextBoxScroll;
                _targetTextBox.TextChanged += OnTextBoxTextChanged;
                _targetTextBox.Resize += OnTextBoxResize;
            }

            this.Invalidate();
        }

        /// <summary>
        /// Sets the line number colors.
        /// </summary>
        public void SetColors(Color backColor, Color foreColor)
        {
            _backColor = backColor;
            _foreColor = foreColor;
            _highlightBackColor = Color.FromArgb(
                Math.Max(0, backColor.R - 20),
                Math.Max(0, backColor.G - 20),
                Math.Max(0, backColor.B - 20)
            );
            this.Invalidate();
        }

        /// <summary>
        /// Sets the font for line numbers.
        /// </summary>
        public new Font Font
        {
            get => _font;
            set
            {
                _font = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Highlights a specific line number.
        /// </summary>
        public void HighlightLine(int lineNumber)
        {
            if (_highlightedLine != lineNumber)
            {
                _highlightedLine = lineNumber;
                this.Invalidate();
            }
        }

        private void OnTextBoxScroll(object? sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void OnTextBoxTextChanged(object? sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void OnTextBoxResize(object? sender, EventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(_backColor);

            if (_targetTextBox == null || string.IsNullOrEmpty(_targetTextBox.Text))
                return;

            // Get first visible line
            int firstVisibleLine = _targetTextBox.GetLineFromCharIndex(_targetTextBox.GetCharIndexFromPosition(new Point(0, 0)));

            // Get line height
            float lineHeight = _font.GetHeight(g);

            // Calculate how many lines are visible
            int visibleLines = (int)Math.Ceiling(this.Height / lineHeight) + 1;

            // Get total line count
            int totalLines = _targetTextBox.Lines.Length;

            using (SolidBrush bgBrush = new SolidBrush(_backColor))
            using (SolidBrush highlightBgBrush = new SolidBrush(_highlightBackColor))
            using (SolidBrush fgBrush = new SolidBrush(_foreColor))
            using (StringFormat format = new StringFormat())
            {
                format.Alignment = StringAlignment.Far; // Right-aligned
                format.LineAlignment = StringAlignment.Near;

                for (int i = 0; i < visibleLines; i++)
                {
                    int lineNumber = firstVisibleLine + i;
                    if (lineNumber >= totalLines)
                        break;

                    float y = i * lineHeight;

                    // Draw highlight background if this is the highlighted line
                    if (lineNumber == _highlightedLine)
                    {
                        g.FillRectangle(highlightBgBrush, 0, y, this.Width, lineHeight);
                    }

                    // Draw line number (1-based)
                    string lineNum = (lineNumber + 1).ToString();
                    RectangleF rect = new RectangleF(0, y, this.Width - 5, lineHeight); // 5px right padding
                    g.DrawString(lineNum, _font, fgBrush, rect, format);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_targetTextBox != null)
                {
                    _targetTextBox.VScroll -= OnTextBoxScroll;
                    _targetTextBox.TextChanged -= OnTextBoxTextChanged;
                    _targetTextBox.Resize -= OnTextBoxResize;
                }
                _font?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
