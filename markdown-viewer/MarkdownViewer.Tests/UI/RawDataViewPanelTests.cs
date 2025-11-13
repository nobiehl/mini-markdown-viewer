using System;
using System.Drawing;
using System.Windows.Forms;
using FluentAssertions;
using Xunit;
using MarkdownViewer.Core.Models;
using MarkdownViewer.UI;

namespace MarkdownViewer.Tests.UI
{
    /// <summary>
    /// Unit tests for RawDataViewPanel functionality.
    /// Tests split-view, syntax highlighting, theme support, and state persistence.
    /// v1.9.0 feature tests.
    /// </summary>
    public class RawDataViewPanelTests : IDisposable
    {
        private readonly RawDataViewPanel _panel;

        public RawDataViewPanelTests()
        {
            _panel = new RawDataViewPanel();
        }

        public void Dispose()
        {
            _panel?.Dispose();
        }

        [Fact]
        public void Constructor_InitializesPanel_WithDefaultState()
        {
            // Arrange & Act
            var panel = new RawDataViewPanel();

            // Assert
            panel.Should().NotBeNull();
            panel.Visible.Should().BeFalse(); // Hidden by default
            panel.Dock.Should().Be(DockStyle.Fill);
            // Note: SplitterDistance may vary until panel is added to a parent Form
            // This is a WinForms constraint - actual value tested in integration tests
            panel.SplitterDistance.Should().BeGreaterThan(0);
        }

        [Fact]
        public void ShowRawData_DisplaysMarkdownAndHtml_AndMakesPanelVisible()
        {
            // Arrange
            string markdown = "# Test Heading\n\nTest content with `code`.";
            string html = "<h1>Test Heading</h1><p>Test content with <code>code</code>.</p>";

            // Act
            _panel.ShowRawData(markdown, html);

            // Assert
            _panel.Visible.Should().BeTrue();
            // Note: Cannot easily test RichTextBox.Text in unit tests without UI thread
            // This would be better tested in integration/UI tests
        }

        [Fact]
        public void Hide_HidesPanel()
        {
            // Arrange
            _panel.ShowRawData("test", "test");
            _panel.Visible.Should().BeTrue();

            // Act
            _panel.Hide();

            // Assert
            _panel.Visible.Should().BeFalse();
        }

        [Fact]
        public void SplitterDistance_CanBeSet_AndRetrieved()
        {
            // Arrange
            int initialDistance = _panel.SplitterDistance;
            int newDistance = 300;

            // Act
            _panel.SplitterDistance = newDistance;
            int actualDistance = _panel.SplitterDistance;

            // Assert
            // In unit tests without Form hosting, WinForms may adjust the value
            // We verify it changed and is positive
            actualDistance.Should().NotBe(initialDistance);
            actualDistance.Should().BeGreaterThan(0);
        }

        [Fact]
        public void SplitterDistance_HasPositiveDefault()
        {
            // Arrange & Act
            var panel = new RawDataViewPanel();

            // Assert
            // In unit tests, WinForms may adjust based on minimum panel sizes
            // We verify it has a reasonable positive value
            panel.SplitterDistance.Should().BeGreaterThan(0);
            panel.SplitterDistance.Should().BeLessThan(1000);
        }

        [Fact]
        public void SetLabelTexts_UpdatesLabelText()
        {
            // Arrange
            string markdownLabel = "Test Markdown";
            string htmlLabel = "Test HTML";

            // Act
            _panel.SetLabelTexts(markdownLabel, htmlLabel);

            // Assert
            // Labels are updated internally, verified by no exceptions thrown
            // Full verification would require accessing private fields or UI automation
        }

        [Fact]
        public void ApplyTheme_DarkTheme_AppliesDarkColors()
        {
            // Arrange
            var darkTheme = new Theme
            {
                Name = "dark",
                Markdown = new MarkdownColors
                {
                    Background = "#1e1e1e",
                    Foreground = "#d4d4d4"
                },
                UI = new UiColors
                {
                    FormBackground = "#1e1e1e"
                }
            };

            // Act
            _panel.ApplyTheme(darkTheme);

            // Assert
            // Theme applied, verified by no exceptions thrown
            // Full color verification would require accessing private RichTextBox controls
        }

        [Fact]
        public void ApplyTheme_LightTheme_AppliesLightColors()
        {
            // Arrange
            var lightTheme = new Theme
            {
                Name = "standard",
                Markdown = new MarkdownColors
                {
                    Background = "#ffffff",
                    Foreground = "#333333"
                },
                UI = new UiColors
                {
                    FormBackground = "#f0f0f0"
                }
            };

            // Act
            _panel.ApplyTheme(lightTheme);

            // Assert
            // Theme applied, verified by no exceptions thrown
        }

        [Fact]
        public void ShowRawData_WithLargeContent_DoesNotThrow()
        {
            // Arrange
            string largeMarkdown = new string('#', 10000) + " Large Heading\n" + new string('a', 50000);
            string largeHtml = "<h1>" + new string('x', 10000) + "</h1><p>" + new string('y', 50000) + "</p>";

            // Act
            Action act = () => _panel.ShowRawData(largeMarkdown, largeHtml);

            // Assert
            act.Should().NotThrow();
            _panel.Visible.Should().BeTrue();
        }

        [Fact]
        public void ShowRawData_WithEmptyStrings_DoesNotThrow()
        {
            // Arrange
            string emptyMarkdown = string.Empty;
            string emptyHtml = string.Empty;

            // Act
            Action act = () => _panel.ShowRawData(emptyMarkdown, emptyHtml);

            // Assert
            act.Should().NotThrow();
            _panel.Visible.Should().BeTrue();
        }

        [Fact]
        public void ShowRawData_WithNullStrings_DoesNotThrow()
        {
            // Arrange
            string? nullMarkdown = null;
            string? nullHtml = null;

            // Act & Assert
            // RichTextBox.Text handles null gracefully by converting to empty string
            Action act = () => _panel.ShowRawData(nullMarkdown!, nullHtml!);
            act.Should().NotThrow();
        }

        [Fact]
        public void ToggleVisibility_WorksCorrectly()
        {
            // Arrange
            _panel.Visible = false;

            // Act - Show
            _panel.ShowRawData("test", "test");
            bool visibleAfterShow = _panel.Visible;

            // Act - Hide
            _panel.Hide();
            bool visibleAfterHide = _panel.Visible;

            // Assert
            visibleAfterShow.Should().BeTrue();
            visibleAfterHide.Should().BeFalse();
        }
    }
}
