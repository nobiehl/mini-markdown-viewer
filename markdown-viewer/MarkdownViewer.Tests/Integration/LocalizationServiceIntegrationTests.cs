using System;
using System.Linq;
using Xunit;
using MarkdownViewer.Core.Services;

namespace MarkdownViewer.Tests.Integration
{
    /// <summary>
    /// Integration tests for LocalizationService with real embedded resources.
    /// Tests actual string loading from resource files.
    /// </summary>
    public class LocalizationServiceIntegrationTests
    {
        [Fact]
        public void GetSupportedLanguages_ReturnsAll8Languages()
        {
            // Arrange
            var localization = new LocalizationService("en");

            // Act
            var languages = localization.GetSupportedLanguages();

            // Assert
            Assert.NotNull(languages);
            Assert.Equal(8, languages.Length);
            Assert.Contains("en", languages);
            Assert.Contains("de", languages);
            Assert.Contains("mn", languages);
            Assert.Contains("fr", languages);
            Assert.Contains("es", languages);
            Assert.Contains("ja", languages);
            Assert.Contains("zh", languages);
            Assert.Contains("ru", languages);
        }

        [Theory]
        [InlineData("en")]
        [InlineData("de")]
        [InlineData("mn")]
        [InlineData("fr")]
        [InlineData("es")]
        [InlineData("ja")]
        [InlineData("zh")]
        [InlineData("ru")]
        public void SetLanguage_ValidLanguageCode_ChangesCurrentLanguage(string langCode)
        {
            // Arrange
            var localization = new LocalizationService("en");

            // Act
            localization.SetLanguage(langCode);
            var current = localization.GetCurrentLanguage();

            // Assert
            Assert.Equal(langCode, current);
        }

        [Theory]
        [InlineData("en", "StatusBarLanguage")]
        [InlineData("de", "StatusBarLanguage")]
        [InlineData("mn", "StatusBarLanguage")]
        [InlineData("fr", "StatusBarLanguage")]
        [InlineData("es", "StatusBarLanguage")]
        [InlineData("ja", "StatusBarLanguage")]
        [InlineData("zh", "StatusBarLanguage")]
        [InlineData("ru", "StatusBarLanguage")]
        public void GetString_AllLanguages_ReturnsLocalizedString(string langCode, string key)
        {
            // Arrange
            var localization = new LocalizationService(langCode);

            // Act
            var value = localization.GetString(key);

            // Assert
            Assert.NotNull(value);
            Assert.NotEmpty(value);
            Assert.DoesNotContain("[", value); // Should not return bracketed key
        }

        [Fact]
        public void GetString_English_ReturnsEnglishStrings()
        {
            // Arrange
            var localization = new LocalizationService("en");

            // Act
            var languageLabel = localization.GetString("LanguageEnglish");
            var themeStandard = localization.GetString("ThemeStandard");

            // Assert
            Assert.Equal("English", languageLabel);
            Assert.Equal("Standard (Enhanced)", themeStandard);
        }

        [Fact]
        public void GetString_German_ReturnsGermanStrings()
        {
            // Arrange
            var localization = new LocalizationService("de");

            // Act
            var languageLabel = localization.GetString("LanguageGerman");
            var themeStandard = localization.GetString("ThemeStandard");

            // Assert
            Assert.Equal("Deutsch", languageLabel);
            Assert.Equal("Standard (Erweitert)", themeStandard);
        }

        [Fact]
        public void GetString_Japanese_ReturnsJapaneseStrings()
        {
            // Arrange
            var localization = new LocalizationService("ja");

            // Act
            var languageLabel = localization.GetString("LanguageJapanese");

            // Assert
            Assert.Equal("日本語", languageLabel);
        }

        [Fact]
        public void GetString_Chinese_ReturnsChineseStrings()
        {
            // Arrange
            var localization = new LocalizationService("zh");

            // Act
            var languageLabel = localization.GetString("LanguageChinese");

            // Assert
            Assert.Equal("简体中文", languageLabel);
        }

        [Fact]
        public void GetString_WithFormatParameters_FormatsCorrectly()
        {
            // Arrange
            var localization = new LocalizationService("en");

            // Act - Test with UpdateAvailable which has a {0} placeholder
            var result = localization.GetString("UpdateAvailable", "1.8.0");

            // Assert
            Assert.Contains("1.8.0", result);
            Assert.Contains("Update", result);
        }

        [Fact]
        public void GetString_InvalidKey_ReturnsBracketedKey()
        {
            // Arrange
            var localization = new LocalizationService("en");

            // Act
            var result = localization.GetString("NonExistentKey");

            // Assert
            Assert.Equal("[NonExistentKey]", result);
        }

        [Fact]
        public void SetLanguage_System_UsesSystemLanguage()
        {
            // Arrange
            var localization = new LocalizationService("system");

            // Act
            var current = localization.GetCurrentLanguage();

            // Assert
            Assert.NotNull(current);
            Assert.NotEqual("system", current); // Should resolve to actual language
        }

        [Fact]
        public void GetString_AllThemes_AreLocalized()
        {
            // Arrange
            var localization = new LocalizationService("en");
            var themes = new[] { "ThemeDark", "ThemeStandard", "ThemeSolarized", "ThemeDraeger" };

            // Act & Assert
            foreach (var themeKey in themes)
            {
                var value = localization.GetString(themeKey);
                Assert.NotNull(value);
                Assert.NotEmpty(value);
                Assert.DoesNotContain("[", value);
            }
        }

        [Fact]
        public void GetString_AllLanguageNames_AreLocalized()
        {
            // Arrange
            var localization = new LocalizationService("en");
            var languageKeys = new[]
            {
                "LanguageEnglish", "LanguageGerman", "LanguageMongolian",
                "LanguageFrench", "LanguageSpanish", "LanguageJapanese",
                "LanguageChinese", "LanguageRussian"
            };

            // Act & Assert
            foreach (var langKey in languageKeys)
            {
                var value = localization.GetString(langKey);
                Assert.NotNull(value);
                Assert.NotEmpty(value);
                Assert.DoesNotContain("[", value);
            }
        }

        [Fact]
        public void LanguageSwitching_MultipleChanges_WorksCorrectly()
        {
            // Arrange
            var localization = new LocalizationService("en");

            // Act & Assert - Switch between multiple languages
            localization.SetLanguage("de");
            Assert.Equal("de", localization.GetCurrentLanguage());
            Assert.Equal("Deutsch", localization.GetString("LanguageGerman"));

            localization.SetLanguage("ja");
            Assert.Equal("ja", localization.GetCurrentLanguage());
            Assert.Equal("日本語", localization.GetString("LanguageJapanese"));

            localization.SetLanguage("en");
            Assert.Equal("en", localization.GetCurrentLanguage());
            Assert.Equal("English", localization.GetString("LanguageEnglish"));
        }

        [Fact]
        public void GetString_StatusBarKeys_AllLanguages_AreComplete()
        {
            // Arrange
            var statusBarKeys = new[]
            {
                "StatusBarLanguage",
                "StatusBarUpToDate",
                "StatusBarUpdateAvailable",
                "StatusBarExplorerRegistered",
                "StatusBarExplorerNotRegistered",
                "StatusBarInfo",
                "StatusBarHelp"
            };

            // Act & Assert - Test all languages have all status bar strings
            foreach (var langCode in new[] { "en", "de", "mn", "fr", "es", "ja", "zh", "ru" })
            {
                var localization = new LocalizationService(langCode);

                foreach (var key in statusBarKeys)
                {
                    var value = localization.GetString(key);
                    Assert.NotNull(value);
                    Assert.NotEmpty(value);
                    Assert.DoesNotContain("[", value);
                }
            }
        }

        [Fact]
        public void GetString_SearchKeys_AllLanguages_AreComplete()
        {
            // Arrange
            var searchKeys = new[]
            {
                "SearchPlaceholder",
                "SearchNoResults",
                "SearchResults",
                "SearchPrevious",
                "SearchNext",
                "SearchClose"
            };

            // Act & Assert - Test all languages have all search strings
            foreach (var langCode in new[] { "en", "de", "mn", "fr", "es", "ja", "zh", "ru" })
            {
                var localization = new LocalizationService(langCode);

                foreach (var key in searchKeys)
                {
                    var value = localization.GetString(key);
                    Assert.NotNull(value);
                    Assert.NotEmpty(value);
                    Assert.DoesNotContain("[", value);
                }
            }
        }
    }
}
