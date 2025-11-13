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
        public void SetHighlightColors_DoesNotThrow()
        {
            // Arrange
            Color mouseOver = Color.FromArgb(25, 100, 150, 200);
            Color cursorLine = Color.FromArgb(80, 100, 150, 200);

            // Act & Assert
            var exception = Record.Exception(() => _control.SetHighlightColors(mouseOver, cursorLine));
            Assert.Null(exception);
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

        public void Dispose()
        {
            _control?.Dispose();
            _testForm?.Dispose();
        }
    }
}
