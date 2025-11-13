using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Web.WebView2.WinForms;
using Moq;
using Xunit;
using MarkdownViewer.Core.Models;
using MarkdownViewer.Services;

namespace MarkdownViewer.Tests
{
    /// <summary>
    /// Unit tests for ThemeService functionality.
    /// Tests theme loading, validation, and availability checks.
    /// Note: These are unit tests, not integration tests.
    /// </summary>
    public class ThemeServiceTests : IDisposable
    {
        private readonly ThemeService _themeService;

        public ThemeServiceTests()
        {
            _themeService = new ThemeService();
        }

        public void Dispose()
        {
            // Cleanup if needed
        }

        [Fact]
        public void Constructor_InitializesWithStandardTheme()
        {
            // Arrange & Act
            var service = new ThemeService();
            var currentTheme = service.GetCurrentTheme();

            // Assert
            currentTheme.Should().NotBeNull();
            currentTheme.Name.Should().Be("standard");
        }

        [Fact]
        public void GetCurrentTheme_ReturnsLoadedTheme()
        {
            // Act
            var theme = _themeService.GetCurrentTheme();

            // Assert
            theme.Should().NotBeNull();
            theme.Should().BeOfType<Theme>();
            theme.Name.Should().NotBeNullOrEmpty();
        }

        [Theory]
        [InlineData("standard")]
        [InlineData("dark")]
        [InlineData("solarized")]
        [InlineData("draeger")]
        public void LoadTheme_WithValidThemeName_ReturnsTheme(string themeName)
        {
            // Act
            var theme = _themeService.LoadTheme(themeName);

            // Assert
            theme.Should().NotBeNull();
            theme.Name.Should().Be(themeName);
            theme.DisplayName.Should().NotBeNullOrEmpty();
            theme.Markdown.Should().NotBeNull();
            theme.UI.Should().NotBeNull();
        }

        [Fact]
        public void LoadTheme_WithInvalidThemeName_FallsBackToStandard()
        {
            // Act
            var theme = _themeService.LoadTheme("nonexistent-theme");

            // Assert
            theme.Should().NotBeNull();
            theme.Name.Should().Be("standard");
        }

        [Fact]
        public void LoadTheme_UpdatesCurrentTheme()
        {
            // Arrange
            var initialTheme = _themeService.GetCurrentTheme();

            // Act
            var newTheme = _themeService.LoadTheme("dark");
            var currentTheme = _themeService.GetCurrentTheme();

            // Assert
            currentTheme.Should().Be(newTheme);
            currentTheme.Name.Should().Be("dark");
        }

        [Fact]
        public void GetAvailableThemes_ReturnsListOfThemes()
        {
            // Act
            var themes = _themeService.GetAvailableThemes();

            // Assert
            themes.Should().NotBeNull();
            themes.Should().NotBeEmpty();
            themes.Should().Contain("standard");
        }

        [Fact]
        public void GetAvailableThemes_ReturnsExpectedThemes()
        {
            // Act
            var themes = _themeService.GetAvailableThemes();

            // Assert
            themes.Should().Contain(new[] { "standard", "dark", "solarized", "draeger" });
        }

        [Fact]
        public void GetAvailableThemes_ReturnsSortedList()
        {
            // Act
            var themes = _themeService.GetAvailableThemes();

            // Assert
            themes.Should().BeInAscendingOrder();
        }

        [Fact]
        public void LoadTheme_ValidatesMarkdownColors()
        {
            // Act
            var theme = _themeService.LoadTheme("standard");

            // Assert
            theme.Markdown.Should().NotBeNull();
            theme.Markdown.Background.Should().NotBeNullOrEmpty();
            theme.Markdown.Foreground.Should().NotBeNullOrEmpty();
            theme.Markdown.CodeBackground.Should().NotBeNullOrEmpty();
            theme.Markdown.LinkColor.Should().NotBeNullOrEmpty();
            theme.Markdown.HeadingColor.Should().NotBeNullOrEmpty();
            theme.Markdown.BlockquoteBorder.Should().NotBeNullOrEmpty();
            theme.Markdown.TableHeaderBackground.Should().NotBeNullOrEmpty();
            theme.Markdown.TableBorder.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void LoadTheme_ValidatesUIColors()
        {
            // Act
            var theme = _themeService.LoadTheme("standard");

            // Assert
            theme.UI.Should().NotBeNull();
            theme.UI.FormBackground.Should().NotBeNullOrEmpty();
            theme.UI.ControlBackground.Should().NotBeNullOrEmpty();
            theme.UI.ControlForeground.Should().NotBeNullOrEmpty();
            theme.UI.StatusBarBackground.Should().NotBeNullOrEmpty();
            theme.UI.StatusBarForeground.Should().NotBeNullOrEmpty();
            theme.UI.MenuBackground.Should().NotBeNullOrEmpty();
            theme.UI.MenuForeground.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void LoadTheme_DarkTheme_HasDarkColors()
        {
            // Act
            var theme = _themeService.LoadTheme("dark");

            // Assert
            theme.Name.Should().Be("dark");
            // Dark theme should have dark background
            theme.Markdown.Background.Should().NotBe("#ffffff");
        }

        [Fact]
        public void LoadTheme_MultipleLoads_DoesNotFail()
        {
            // Act & Assert - should not throw
            for (int i = 0; i < 5; i++)
            {
                var theme = _themeService.LoadTheme("standard");
                theme.Should().NotBeNull();
            }
        }

        [Fact]
        public void LoadTheme_DifferentThemes_ReturnsDifferentObjects()
        {
            // Act
            var standardTheme = _themeService.LoadTheme("standard");
            var darkTheme = _themeService.LoadTheme("dark");

            // Assert
            standardTheme.Should().NotBeSameAs(darkTheme);
            standardTheme.Name.Should().NotBe(darkTheme.Name);
        }

        [Fact]
        public async Task ApplyThemeAsync_WithNullWebView_DoesNotThrow()
        {
            // Arrange
            var theme = _themeService.LoadTheme("standard");
            var mockForm = new Mock<System.Windows.Forms.Form>();

            // Act
            Func<Task> act = async () => await _themeService.ApplyThemeAsync(theme, mockForm.Object, null!);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public void GetCurrentTheme_AfterLoadingMultipleThemes_ReturnsLatest()
        {
            // Act
            _themeService.LoadTheme("standard");
            _themeService.LoadTheme("dark");
            var solarizedTheme = _themeService.LoadTheme("solarized");
            var currentTheme = _themeService.GetCurrentTheme();

            // Assert
            currentTheme.Should().Be(solarizedTheme);
            currentTheme.Name.Should().Be("solarized");
        }

        [Fact]
        public void LoadTheme_ColorFormats_AreValidHexFormat()
        {
            // Act
            var theme = _themeService.LoadTheme("standard");

            // Assert - all colors should be in hex format
            theme.Markdown.Background.Should().MatchRegex(@"^#[0-9a-fA-F]{6}$");
            theme.Markdown.Foreground.Should().MatchRegex(@"^#[0-9a-fA-F]{6}$");
            theme.Markdown.CodeBackground.Should().MatchRegex(@"^#[0-9a-fA-F]{6}$");
            theme.Markdown.LinkColor.Should().MatchRegex(@"^#[0-9a-fA-F]{6}$");
            theme.UI.FormBackground.Should().MatchRegex(@"^#[0-9a-fA-F]{6}$");
        }
    }
}
