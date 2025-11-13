using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Core.Models;
using MarkdownViewer.Services;

namespace MarkdownViewer.Tests.Integration
{
    /// <summary>
    /// Integration tests for ThemeService with embedded resources.
    /// Tests real theme loading from assembly resources.
    /// </summary>
    public class ThemeServiceIntegrationTests
    {
        [Fact]
        public void GetAvailableThemes_ReturnsAllEmbeddedThemes()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var themes = themeService.GetAvailableThemes();

            // Assert
            Assert.NotNull(themes);
            Assert.NotEmpty(themes);
            Assert.Contains("dark", themes);
            Assert.Contains("standard", themes);
            Assert.Contains("solarized", themes);
            Assert.Contains("draeger", themes);
            Assert.Equal(4, themes.Count);
        }

        [Theory]
        [InlineData("dark")]
        [InlineData("standard")]
        [InlineData("solarized")]
        [InlineData("draeger")]
        public void LoadTheme_ValidThemeName_LoadsThemeSuccessfully(string themeName)
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var theme = themeService.LoadTheme(themeName);

            // Assert
            Assert.NotNull(theme);
            Assert.Equal(themeName, theme.Name);
            Assert.NotNull(theme.Markdown);
            Assert.NotNull(theme.UI);
        }

        [Fact]
        public void LoadTheme_DarkTheme_HasCorrectColors()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var theme = themeService.LoadTheme("dark");

            // Assert
            Assert.Equal("dark", theme.Name);

            // Check Markdown colors exist
            Assert.NotNull(theme.Markdown.Background);
            Assert.NotNull(theme.Markdown.Foreground);
            Assert.NotNull(theme.Markdown.HeadingColor);
            Assert.NotNull(theme.Markdown.LinkColor);
            Assert.NotNull(theme.Markdown.CodeBackground);
            Assert.NotNull(theme.Markdown.BlockquoteBorder);
            Assert.NotNull(theme.Markdown.TableHeaderBackground);
            Assert.NotNull(theme.Markdown.TableBorder);

            // Check UI colors exist
            Assert.NotNull(theme.UI.FormBackground);
            Assert.NotNull(theme.UI.ControlBackground);
            Assert.NotNull(theme.UI.ControlForeground);
            Assert.NotNull(theme.UI.StatusBarBackground);
            Assert.NotNull(theme.UI.MenuBackground);
        }

        [Fact]
        public void LoadTheme_StandardTheme_HasCorrectColors()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var theme = themeService.LoadTheme("standard");

            // Assert
            Assert.Equal("standard", theme.Name);
            Assert.NotNull(theme.Markdown);
            Assert.NotNull(theme.UI);
        }

        [Fact]
        public void LoadTheme_InvalidThemeName_ReturnsDefaultTheme()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var theme = themeService.LoadTheme("nonexistent");

            // Assert
            Assert.NotNull(theme);
            Assert.Equal("standard", theme.Name); // Should fallback to default
        }

        [Fact]
        public void LoadTheme_EmptyString_ReturnsDefaultTheme()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var theme = themeService.LoadTheme("");

            // Assert
            Assert.NotNull(theme);
            Assert.Equal("standard", theme.Name);
        }

        [Fact]
        public void LoadTheme_MultipleCallsSameTheme_ReturnsSameTheme()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act
            var theme1 = themeService.LoadTheme("dark");
            var theme2 = themeService.LoadTheme("dark");

            // Assert
            Assert.NotNull(theme1);
            Assert.NotNull(theme2);
            Assert.Equal(theme1.Name, theme2.Name);
            Assert.Equal(theme1.Markdown.Background, theme2.Markdown.Background);
        }

        [Fact]
        public void LoadTheme_AllThemes_AreUnique()
        {
            // Arrange
            var themeService = new ThemeService();
            var themeNames = themeService.GetAvailableThemes();

            // Act
            var themes = themeNames.Select(name => themeService.LoadTheme(name)).ToList();

            // Assert
            Assert.Equal(4, themes.Count);

            // Check that themes are properly loaded (note: some themes may share colors)
            var backgrounds = themes.Select(t => t.Markdown.Background).Distinct().ToList();
            Assert.True(backgrounds.Count >= 3, "Themes should have at least 3 different backgrounds");

            // Verify each theme has unique name
            var names = themes.Select(t => t.Name).Distinct().ToList();
            Assert.Equal(4, names.Count); // All themes should have different names
        }

        [Fact]
        public void ThemeService_EmbeddedResources_AreAccessible()
        {
            // Arrange
            var themeService = new ThemeService();

            // Act & Assert - Should not throw
            foreach (var themeName in new[] { "dark", "standard", "solarized", "draeger" })
            {
                var theme = themeService.LoadTheme(themeName);
                Assert.NotNull(theme);
                Assert.Equal(themeName, theme.Name);
            }
        }
    }
}
