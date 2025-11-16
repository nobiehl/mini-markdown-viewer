using System;
using System.Drawing;
using System.Windows.Forms;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Code viewer control with line numbers and text selection.
    /// Uses a RichTextBox for text display with built-in selection and copy support.
    /// </summary>
    public class CodeViewControl : UserControl
    {
        private LineNumberPanel _lineNumbers = null!;
        private RichTextBox _textBox = null!;

        private Color _lineNumberBackColor = Color.FromArgb(240, 240, 240);
        private Color _lineNumberForeColor = Color.Gray;
        private bool _showLineNumbers = true;

        public CodeViewControl()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.SuspendLayout();

            // Create line number panel (left side)
            _lineNumbers = new LineNumberPanel
            {
                Dock = DockStyle.Left,
                Width = LineNumberPanel.DefaultWidth
            };

            // Create RichTextBox (main text area)
            _textBox = new RichTextBox
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                WordWrap = false,
                BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 10F),
                BackColor = Color.White,
                ForeColor = Color.Black,
                ScrollBars = RichTextBoxScrollBars.Both,
                DetectUrls = false,
                HideSelection = false // Keep selection visible when control loses focus
            };

            // Connect line numbers to text box
            _lineNumbers.AttachToTextBox(_textBox);

            // Build control hierarchy (line numbers left, text box fills rest)
            this.Controls.Add(_textBox);
            this.Controls.Add(_lineNumbers);

            this.ResumeLayout(false);
        }

        /// <summary>
        /// Gets or sets the text content.
        /// </summary>
        public new string Text
        {
            get => _textBox.Text;
            set => _textBox.Text = value ?? string.Empty;
        }

        /// <summary>
        /// Sets the colors for the line number panel.
        /// </summary>
        public void SetLineNumberColors(Color backColor, Color foreColor)
        {
            _lineNumberBackColor = backColor;
            _lineNumberForeColor = foreColor;
            _lineNumbers.SetColors(backColor, foreColor);
        }

        /// <summary>
        /// Gets or sets whether line numbers are shown.
        /// </summary>
        public bool ShowLineNumbers
        {
            get => _showLineNumbers;
            set
            {
                _showLineNumbers = value;
                _lineNumbers.Visible = value;
            }
        }

        /// <summary>
        /// Gets or sets the font for the text.
        /// </summary>
        public new Font Font
        {
            get => _textBox.Font;
            set
            {
                _textBox.Font = value;
                _lineNumbers.Font = value;
            }
        }

        /// <summary>
        /// Gets or sets the background color of the text area.
        /// </summary>
        public new Color BackColor
        {
            get => _textBox.BackColor;
            set => _textBox.BackColor = value;
        }

        /// <summary>
        /// Gets or sets the foreground color (text color).
        /// </summary>
        public new Color ForeColor
        {
            get => _textBox.ForeColor;
            set => _textBox.ForeColor = value;
        }

        /// <summary>
        /// Gets or sets whether the control is read-only.
        /// </summary>
        public bool ReadOnly
        {
            get => _textBox.ReadOnly;
            set => _textBox.ReadOnly = value;
        }

        /// <summary>
        /// Gets or sets whether word wrap is enabled.
        /// </summary>
        public bool WordWrap
        {
            get => _textBox.WordWrap;
            set => _textBox.WordWrap = value;
        }

        /// <summary>
        /// Gets or sets the border style (compatibility property).
        /// </summary>
        public new BorderStyle BorderStyle
        {
            get => base.BorderStyle;
            set => base.BorderStyle = value;
        }

        /// <summary>
        /// Gets the underlying RichTextBox (for advanced scenarios).
        /// </summary>
        public RichTextBox TextBox => _textBox;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _lineNumbers?.Dispose();
                _textBox?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
