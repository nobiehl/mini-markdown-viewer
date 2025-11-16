using System;
using System.Drawing;
using System.Windows.Forms;
using Xunit;
using MarkdownViewer.UI;

namespace MarkdownViewer.Tests.UI
{
    public class CodeViewControlTests : IDisposable
    {
        private readonly Form _testForm;
        private readonly CodeViewControl _control;

        public CodeViewControlTests()
        {
            _testForm = new Form { Width = 800, Height = 600 };
            _control = new CodeViewControl { Dock = DockStyle.Fill };
            _testForm.Controls.Add(_control);
        }

        [Fact]
        public void Constructor_CreatesControlWithDefaults()
        {
            // Assert
            Assert.NotNull(_control);
            Assert.True(_control.ReadOnly);
            Assert.False(_control.WordWrap);
            Assert.Equal(string.Empty, _control.Text);
            Assert.True(_control.ShowLineNumbers);
        }

        [Fact]
        public void Text_CanBeSetAndRetrieved()
        {
            // Arrange
            string testText = "Line 1\nLine 2\nLine 3";

            // Act
            _control.Text = testText;

            // Assert
            Assert.Equal(testText, _control.Text);
        }

        [Fact]
        public void Text_NullBecomesEmptyString()
        {
            // Act
            _control.Text = null!;

            // Assert
            Assert.Equal(string.Empty, _control.Text);
        }

        [Fact]
        public void Font_CanBeChanged()
        {
            // Arrange
            var newFont = new Font("Arial", 12F);

            // Act
            _control.Font = newFont;

            // Assert
            Assert.Equal(newFont, _control.Font);
        }

        [Fact]
        public void ShowLineNumbers_CanBeToggledOn()
        {
            // Act
            _control.ShowLineNumbers = true;

            // Assert
            Assert.True(_control.ShowLineNumbers);
        }

        [Fact]
        public void ShowLineNumbers_CanBeToggledOff()
        {
            // Act
            _control.ShowLineNumbers = false;

            // Assert
            Assert.False(_control.ShowLineNumbers);
        }

        [Fact]
        public void SetLineNumberColors_DoesNotThrow()
        {
            // Arrange
            Color backColor = Color.FromArgb(240, 240, 240);
            Color foreColor = Color.Gray;

            // Act & Assert
            var exception = Record.Exception(() => _control.SetLineNumberColors(backColor, foreColor));
            Assert.Null(exception);
        }

        [Fact]
        public void BackColor_CanBeChanged()
        {
            // Arrange
            Color newColor = Color.FromArgb(30, 30, 30);

            // Act
            _control.BackColor = newColor;

            // Assert
            Assert.Equal(newColor, _control.BackColor);
        }

        [Fact]
        public void ForeColor_CanBeChanged()
        {
            // Arrange
            Color newColor = Color.FromArgb(200, 200, 200);

            // Act
            _control.ForeColor = newColor;

            // Assert
            Assert.Equal(newColor, _control.ForeColor);
        }

        [Fact]
        public void Control_CanBeAddedToForm()
        {
            // Arrange
            var form = new Form();
            var control = new CodeViewControl();

            // Act
            form.Controls.Add(control);

            // Assert
            Assert.True(form.Controls.Contains(control));
            form.Dispose();
            control.Dispose();
        }

        [Fact]
        public void Control_WithLargeText_DoesNotThrow()
        {
            // Arrange
            var lines = new string[1000];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = $"Line {i + 1}: This is a test line with some content";
            }
            string largeText = string.Join("\n", lines);

            // Act & Assert
            var exception = Record.Exception(() => _control.Text = largeText);
            Assert.Null(exception);
        }

        [Fact]
        public void Control_WithEmptyText_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => _control.Text = string.Empty);
            Assert.Null(exception);
        }

        [Fact]
        public void Control_Paint_DoesNotThrowWithText()
        {
            // Arrange
            _control.Text = "Line 1\nLine 2\nLine 3";
            _testForm.Show();

            // Act & Assert - Paint should be triggered by Show()
            var exception = Record.Exception(() => _control.Refresh());
            Assert.Null(exception);
        }

        [Fact]
        public void Control_Paint_DoesNotThrowWithEmptyText()
        {
            // Arrange
            _control.Text = string.Empty;
            _testForm.Show();

            // Act & Assert
            var exception = Record.Exception(() => _control.Refresh());
            Assert.Null(exception);
        }

        [Fact]
        public void ReadOnly_DefaultsToTrue()
        {
            // Assert
            Assert.True(_control.ReadOnly);
        }

        [Fact]
        public void WordWrap_DefaultsToFalse()
        {
            // Assert
            Assert.False(_control.WordWrap);
        }

        [Fact]
        public void BorderStyle_DefaultsToNone()
        {
            // Assert
            Assert.Equal(BorderStyle.None, _control.BorderStyle);
        }

        // ===== TEXT SELECTION & COPY TESTS (v1.9.1) =====

        [Fact]
        public void TextBox_Property_ExposesUnderlyingRichTextBox()
        {
            // Act
            var textBox = _control.TextBox;

            // Assert
            Assert.NotNull(textBox);
            Assert.IsType<RichTextBox>(textBox);
        }

        [Fact]
        public void TextBox_IsReadOnly_ByDefault()
        {
            // Act
            var textBox = _control.TextBox;

            // Assert
            Assert.True(textBox.ReadOnly);
        }

        [Fact]
        public void TextBox_SupportsTextSelection()
        {
            // Arrange
            _control.Text = "Line 1\nLine 2\nLine 3";
            var textBox = _control.TextBox;

            // Act - Select text programmatically
            textBox.Select(0, 6); // Select "Line 1"

            // Assert
            Assert.Equal(0, textBox.SelectionStart);
            Assert.Equal(6, textBox.SelectionLength);
            Assert.Equal("Line 1", textBox.SelectedText);
        }

        [Fact]
        public void TextBox_HideSelection_IsFalse()
        {
            // Arrange
            var textBox = _control.TextBox;

            // Act & Assert
            Assert.False(textBox.HideSelection); // Selection should be visible even when control loses focus
        }

        [Fact]
        public void TextBox_SelectAll_SelectsAllText()
        {
            // Arrange
            string testText = "Line 1\nLine 2\nLine 3";
            _control.Text = testText;
            var textBox = _control.TextBox;

            // Act
            textBox.SelectAll();

            // Assert
            Assert.Equal(testText.Length, textBox.SelectionLength);
            Assert.Equal(testText, textBox.SelectedText);
        }

        [Fact]
        public void TextBox_SelectedText_CanBeRetrieved()
        {
            // Arrange
            _control.Text = "First Second Third";
            var textBox = _control.TextBox;
            textBox.Select(6, 6); // Select "Second"

            // Act
            string selectedText = textBox.SelectedText;

            // Assert
            Assert.Equal("Second", selectedText);
        }

        [Fact]
        public void TextBox_SelectionAcrossMultipleLines_Works()
        {
            // Arrange
            _control.Text = "Line 1\nLine 2\nLine 3";
            var textBox = _control.TextBox;

            // Act - Select from "Line 1" to "Line 2"
            textBox.Select(0, 13); // "Line 1\nLine 2"

            // Assert
            Assert.Equal(13, textBox.SelectionLength);
            Assert.Equal("Line 1\nLine 2", textBox.SelectedText);
        }

        [Fact]
        public void TextBox_EmptySelection_ReturnsEmptyString()
        {
            // Arrange
            _control.Text = "Some text";
            var textBox = _control.TextBox;
            textBox.Select(5, 0); // No selection

            // Act
            string selectedText = textBox.SelectedText;

            // Assert
            Assert.Equal(string.Empty, selectedText);
        }

        [Fact]
        public void Control_WithLineNumbers_DoesNotIncludeLineNumbersInText()
        {
            // Arrange
            _control.ShowLineNumbers = true;
            _control.Text = "Line 1\nLine 2\nLine 3";

            // Act
            string actualText = _control.Text;

            // Assert - Line numbers should NOT be in the text
            Assert.Equal("Line 1\nLine 2\nLine 3", actualText);
            Assert.DoesNotContain("1", actualText.Substring(0, 1)); // First character is not a line number
        }

        [Fact]
        public void TextBox_Selection_DoesNotIncludeLineNumbers()
        {
            // Arrange
            _control.ShowLineNumbers = true;
            _control.Text = "Line 1\nLine 2\nLine 3";
            var textBox = _control.TextBox;

            // Act - Select all
            textBox.SelectAll();
            string selectedText = textBox.SelectedText;

            // Assert - Selected text should NOT contain line numbers
            Assert.Equal("Line 1\nLine 2\nLine 3", selectedText);
            Assert.DoesNotContain("1 ", selectedText); // No "1 " line number prefix
            Assert.DoesNotContain("2 ", selectedText); // No "2 " line number prefix
            Assert.DoesNotContain("3 ", selectedText); // No "3 " line number prefix
        }

        public void Dispose()
        {
            _control?.Dispose();
            _testForm?.Dispose();
        }
    }
}
