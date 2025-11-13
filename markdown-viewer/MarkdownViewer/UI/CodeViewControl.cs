using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Custom text viewer control with flicker-free row highlighting.
    /// Draws text and highlighting in a single paint cycle.
    /// </summary>
    public class CodeViewControl : Control
    {
        private string _text = string.Empty;
        private int _scrollOffset = 0;
        private int _mouseOverLine = -1;
        private int _cursorLine = 0;
        private VScrollBar _vScrollBar;

        private Color _mouseOverColor = Color.FromArgb(25, 100, 150, 200);
        private Color _cursorLineColor = Color.FromArgb(80, 100, 150, 200); // Deutlich stärker
        private Font _textFont = new Font("Consolas", 10F);

        private bool _showLineNumbers = true;
        private Color _lineNumberBackColor = Color.FromArgb(240, 240, 240);
        private Color _lineNumberForeColor = Color.Gray;
        private const int LINE_NUMBER_WIDTH = 50;

        public CodeViewControl()
        {
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw,
                true);

            // Create vertical scrollbar
            _vScrollBar = new VScrollBar
            {
                Dock = DockStyle.Right,
                Visible = false
            };
            _vScrollBar.Scroll += OnScrollBarScroll;
            this.Controls.Add(_vScrollBar);

            this.MouseMove += OnMouseMoveHandler;
            this.MouseLeave += OnMouseLeaveHandler;
            this.MouseClick += OnMouseClickHandler;
        }

        public new string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                UpdateScrollBar();
                this.Invalidate();
            }
        }

        public void SetHighlightColors(Color mouseOverColor, Color cursorLineColor)
        {
            _mouseOverColor = mouseOverColor;
            _cursorLineColor = cursorLineColor;
            this.Invalidate();
        }

        public void SetLineNumberColors(Color backColor, Color foreColor)
        {
            _lineNumberBackColor = backColor;
            _lineNumberForeColor = foreColor;
            this.Invalidate();
        }

        public bool ShowLineNumbers
        {
            get => _showLineNumbers;
            set
            {
                _showLineNumbers = value;
                this.Invalidate();
            }
        }

        public new Font Font
        {
            get => _textFont;
            set
            {
                _textFont = value;
                UpdateScrollBar();
                this.Invalidate();
            }
        }

        private void OnScrollBarScroll(object? sender, ScrollEventArgs e)
        {
            _scrollOffset = e.NewValue;
            this.Invalidate();
        }

        private void OnMouseMoveHandler(object? sender, MouseEventArgs e)
        {
            int line = GetLineFromPoint(e.Location);
            if (line != _mouseOverLine)
            {
                _mouseOverLine = line;
                this.Invalidate();
            }
        }

        private void OnMouseLeaveHandler(object? sender, EventArgs e)
        {
            if (_mouseOverLine != -1)
            {
                _mouseOverLine = -1;
                this.Invalidate();
            }
        }

        private void OnMouseClickHandler(object? sender, MouseEventArgs e)
        {
            int line = GetLineFromPoint(e.Location);
            if (line >= 0)
            {
                _cursorLine = line;
                this.Invalidate();
            }
        }

        private int GetLineFromPoint(Point point)
        {
            float lineHeight = _textFont.GetHeight();
            int line = (int)((point.Y / lineHeight) + _scrollOffset);

            string[] lines = _text.Split('\n');
            if (line >= 0 && line < lines.Length)
                return line;

            return -1;
        }

        private void UpdateScrollBar()
        {
            if (string.IsNullOrEmpty(_text))
            {
                _vScrollBar.Visible = false;
                return;
            }

            string[] lines = _text.Split('\n');
            float lineHeight = _textFont.GetHeight();
            int visibleLines = (int)(this.ClientSize.Height / lineHeight);

            if (lines.Length > visibleLines)
            {
                _vScrollBar.Visible = true;
                _vScrollBar.Minimum = 0;
                _vScrollBar.Maximum = lines.Length - 1;
                _vScrollBar.LargeChange = visibleLines;
                _vScrollBar.SmallChange = 1;
            }
            else
            {
                _vScrollBar.Visible = false;
                _scrollOffset = 0;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(this.BackColor);

            if (string.IsNullOrEmpty(_text))
                return;

            string[] lines = _text.Split('\n');
            float lineHeight = _textFont.GetHeight();
            int visibleLines = (int)(this.ClientSize.Height / lineHeight) + 1;
            int textStartX = _showLineNumbers ? LINE_NUMBER_WIDTH : 0;
            int totalWidth = _vScrollBar.Visible ? this.ClientSize.Width - _vScrollBar.Width : this.ClientSize.Width;

            using (SolidBrush textBrush = new SolidBrush(this.ForeColor))
            using (SolidBrush mouseOverBrush = new SolidBrush(_mouseOverColor))
            using (SolidBrush cursorBrush = new SolidBrush(_cursorLineColor))
            using (SolidBrush lineNumberBgBrush = new SolidBrush(_lineNumberBackColor))
            using (SolidBrush lineNumberFgBrush = new SolidBrush(_lineNumberForeColor))
            {
                // Draw line number background area
                if (_showLineNumbers)
                {
                    g.FillRectangle(lineNumberBgBrush, 0, 0, LINE_NUMBER_WIDTH, this.ClientSize.Height);
                }

                for (int i = 0; i < visibleLines && (i + _scrollOffset) < lines.Length; i++)
                {
                    int lineIndex = i + _scrollOffset;
                    float y = i * lineHeight;

                    // Draw highlighting (full width including line numbers)
                    RectangleF highlightRect = new RectangleF(
                        0,
                        y,
                        totalWidth,
                        lineHeight
                    );

                    if (lineIndex == _cursorLine)
                    {
                        g.FillRectangle(cursorBrush, highlightRect);
                    }
                    else if (lineIndex == _mouseOverLine)
                    {
                        g.FillRectangle(mouseOverBrush, highlightRect);
                    }

                    // Draw line numbers
                    if (_showLineNumbers)
                    {
                        string lineNum = (lineIndex + 1).ToString();
                        SizeF lineNumSize = g.MeasureString(lineNum, _textFont);
                        float lineNumX = LINE_NUMBER_WIDTH - lineNumSize.Width - 5; // Right-aligned with 5px padding
                        g.DrawString(lineNum, _textFont, lineNumberFgBrush, lineNumX, y);
                    }

                    // Draw text
                    string line = lines[lineIndex].TrimEnd('\r');
                    g.DrawString(line, _textFont, textBrush, textStartX + 2, y);
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateScrollBar();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (_vScrollBar.Visible)
            {
                int newValue = _scrollOffset - (e.Delta / 120) * 3; // 3 lines per wheel tick
                newValue = Math.Max(_vScrollBar.Minimum, Math.Min(_vScrollBar.Maximum - _vScrollBar.LargeChange + 1, newValue));

                if (newValue != _scrollOffset)
                {
                    _scrollOffset = newValue;
                    _vScrollBar.Value = _scrollOffset;
                    this.Invalidate();
                }
            }
        }

        // Compatibility properties for RichTextBox-like interface
        public bool ReadOnly { get; set; } = true;
        public bool WordWrap { get; set; } = false;
        public BorderStyle BorderStyle { get; set; } = BorderStyle.None;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _vScrollBar?.Dispose();
                _textFont?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
