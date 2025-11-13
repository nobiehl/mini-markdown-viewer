using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Custom RichTextBox with row highlighting for mouse-over and cursor position.
    /// Draws highlighting in background to avoid flickering.
    /// </summary>
    public class HighlightedRichTextBox : RichTextBox
    {
        private int _mouseOverLine = -1;
        private int _cursorLine = -1;
        private Color _mouseOverColor = Color.FromArgb(20, 100, 150, 200);
        private Color _cursorLineColor = Color.FromArgb(40, 100, 150, 200);

        private const int WM_ERASEBKGND = 0x0014;
        private const int WM_PAINT = 0x000F;

        public HighlightedRichTextBox()
        {
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.MouseMove += OnMouseMoveHandler;
            this.SelectionChanged += OnSelectionChangedHandler;
            this.MouseLeave += OnMouseLeaveHandler;
        }

        // Internal accessor for extension methods (compatibility)
        internal RichTextBox InternalTextBox => this;

        /// <summary>
        /// Sets the highlight colors for mouse-over and cursor line.
        /// </summary>
        public void SetHighlightColors(Color mouseOverColor, Color cursorLineColor)
        {
            _mouseOverColor = mouseOverColor;
            _cursorLineColor = cursorLineColor;
            this.Invalidate();
        }

        private void OnMouseMoveHandler(object? sender, MouseEventArgs e)
        {
            if (!this.ReadOnly) return;

            int charIndex = this.GetCharIndexFromPosition(e.Location);
            int lineIndex = this.GetLineFromCharIndex(charIndex);

            if (lineIndex != _mouseOverLine)
            {
                _mouseOverLine = lineIndex;
                this.Invalidate();
            }
        }

        private void OnSelectionChangedHandler(object? sender, EventArgs e)
        {
            if (!this.ReadOnly) return;

            int cursorLine = this.GetLineFromCharIndex(this.SelectionStart);

            if (cursorLine != _cursorLine)
            {
                _cursorLine = cursorLine;
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

        protected override void WndProc(ref Message m)
        {
            // Let RichTextBox do its normal painting first
            base.WndProc(ref m);

            // AFTER the text is painted, draw highlighting on top
            if (m.Msg == WM_PAINT && this.ReadOnly && (_mouseOverLine >= 0 || _cursorLine >= 0))
            {
                using (Graphics g = Graphics.FromHwnd(this.Handle))
                {
                    // Use alpha blending for smooth highlighting
                    g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

                    // Draw mouse-over line highlight (lighter)
                    if (_mouseOverLine >= 0 && _mouseOverLine != _cursorLine)
                    {
                        DrawLineHighlight(g, _mouseOverLine, _mouseOverColor);
                    }

                    // Draw cursor line highlight (stronger)
                    if (_cursorLine >= 0)
                    {
                        DrawLineHighlight(g, _cursorLine, _cursorLineColor);
                    }
                }
            }
        }

        private void DrawLineHighlight(Graphics g, int lineIndex, Color color)
        {
            try
            {
                int charIndex = this.GetFirstCharIndexFromLine(lineIndex);
                if (charIndex < 0) return;

                Point linePos = this.GetPositionFromCharIndex(charIndex);
                float lineHeight = this.Font.GetHeight();

                Rectangle highlightRect = new Rectangle(
                    0,
                    linePos.Y,
                    this.ClientSize.Width,
                    (int)Math.Ceiling(lineHeight)
                );

                if (highlightRect.Y >= 0 && highlightRect.Y < this.ClientSize.Height)
                {
                    using (SolidBrush brush = new SolidBrush(color))
                    {
                        g.FillRectangle(brush, highlightRect);
                    }
                }
            }
            catch
            {
                // Ignore errors
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.MouseMove -= OnMouseMoveHandler;
                this.SelectionChanged -= OnSelectionChangedHandler;
                this.MouseLeave -= OnMouseLeaveHandler;
            }
            base.Dispose(disposing);
        }
    }
}
