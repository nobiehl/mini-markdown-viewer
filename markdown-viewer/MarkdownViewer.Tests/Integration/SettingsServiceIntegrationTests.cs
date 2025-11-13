using System;
using System.IO;
using Xunit;
using MarkdownViewer.Core.Services;
using MarkdownViewer.Core.Models;

namespace MarkdownViewer.Tests.Integration
{
    /// <summary>
    /// Integration tests for SettingsService with real file system.
    /// Tests actual save/load operations with temporary directories.
    /// </summary>
    public class SettingsServiceIntegrationTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly SettingsService _settingsService;

        public SettingsServiceIntegrationTests()
        {
            // Create a temporary test directory
            _testDirectory = Path.Combine(Path.GetTempPath(), $"MarkdownViewerTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);

            // Override APPDATA path for testing
            Environment.SetEnvironmentVariable("APPDATA", _testDirectory, EnvironmentVariableTarget.Process);

            _settingsService = new SettingsService();
        }

        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void Load_NoSettingsFile_ReturnsDefaultSettings()
        {
            // Arrange - No settings file exists yet

            // Act
            var settings = _settingsService.Load();

            // Assert
            Assert.NotNull(settings);
            Assert.Equal("standard", settings.Theme);
            Assert.Equal("system", settings.Language);
            Assert.Equal("1.5.3", settings.Version); // Current default version in AppSettings.cs
        }

        [Fact]
        public void Save_CreatesSettingsFile()
        {
            // Arrange
            var settings = new AppSettings
            {
                Theme = "dark",
                Language = "de"
            };

            // Act
            _settingsService.Save(settings);

            // Assert - Use the actual path from SettingsService
            var settingsPath = _settingsService.GetSettingsPath();
            Assert.True(File.Exists(settingsPath), $"Settings file should exist at: {settingsPath}");
        }

        [Fact]
        public void SaveAndLoad_RoundTrip_PreservesAllSettings()
        {
            // Arrange
            var originalSettings = new AppSettings
            {
                Theme = "dark",
                Language = "ja",
                Version = "1.5.4"
            };

            originalSettings.UI.StatusBar.Visible = false;
            originalSettings.UI.NavigationBar.Visible = true;
            originalSettings.UI.Search.CaseSensitive = true;
            originalSettings.Updates.CheckOnStartup = false;

            // Act
            _settingsService.Save(originalSettings);
            var loadedSettings = _settingsService.Load();

            // Assert
            Assert.NotNull(loadedSettings);
            Assert.Equal(originalSettings.Theme, loadedSettings.Theme);
            Assert.Equal(originalSettings.Language, loadedSettings.Language);
            Assert.Equal(originalSettings.Version, loadedSettings.Version);
            Assert.Equal(originalSettings.UI.StatusBar.Visible, loadedSettings.UI.StatusBar.Visible);
            Assert.Equal(originalSettings.UI.NavigationBar.Visible, loadedSettings.UI.NavigationBar.Visible);
            Assert.Equal(originalSettings.UI.Search.CaseSensitive, loadedSettings.UI.Search.CaseSensitive);
            Assert.Equal(originalSettings.Updates.CheckOnStartup, loadedSettings.Updates.CheckOnStartup);
        }

        [Fact]
        public void Save_MultipleThemes_PersistsLastSaved()
        {
            // Arrange & Act
            var settings1 = new AppSettings { Theme = "dark" };
            _settingsService.Save(settings1);

            var settings2 = new AppSettings { Theme = "solarized" };
            _settingsService.Save(settings2);

            var loaded = _settingsService.Load();

            // Assert
            Assert.Equal("solarized", loaded.Theme);
        }

        [Fact]
        public void Save_InvalidPath_HandlesGracefully()
        {
            // Arrange
            Environment.SetEnvironmentVariable("APPDATA", "/invalid/path/that/does/not/exist/and/cannot/be/created", EnvironmentVariableTarget.Process);
            var service = new SettingsService();
            var settings = new AppSettings();

            // Act & Assert - Should not throw
            service.Save(settings);
        }

        [Fact]
        public void Load_CorruptedFile_ReturnsDefaultSettings()
        {
            // Arrange
            var settingsPath = _settingsService.GetSettingsPath();
            Directory.CreateDirectory(Path.GetDirectoryName(settingsPath)!);
            File.WriteAllText(settingsPath, "{ invalid json content }");

            // Act
            var settings = _settingsService.Load();

            // Assert - Should return defaults instead of crashing
            Assert.NotNull(settings);
            Assert.Equal("standard", settings.Theme);
        }

        [Fact]
        public void Save_AllLanguages_Persists()
        {
            // Arrange
            var languages = new[] { "en", "de", "mn", "fr", "es", "ja", "zh", "ru" };

            foreach (var lang in languages)
            {
                // Act
                var settings = new AppSettings { Language = lang };
                _settingsService.Save(settings);
                var loaded = _settingsService.Load();

                // Assert
                Assert.Equal(lang, loaded.Language);
            }
        }

        [Fact]
        public void Save_AllThemes_Persists()
        {
            // Arrange
            var themes = new[] { "dark", "standard", "solarized", "draeger" };

            foreach (var theme in themes)
            {
                // Act
                var settings = new AppSettings { Theme = theme };
                _settingsService.Save(settings);
                var loaded = _settingsService.Load();

                // Assert
                Assert.Equal(theme, loaded.Theme);
            }
        }

        [Fact]
        public void Save_NestedSettings_PreservesStructure()
        {
            // Arrange
            var settings = new AppSettings();
            settings.UI.StatusBar.Visible = false;
            settings.UI.NavigationBar.Visible = true;
            settings.Navigation.MaxHistorySize = 50;
            settings.UI.Search.CaseSensitive = true;
            settings.Updates.CheckIntervalDays = 14;
            settings.Explorer.RegisterFileAssociation = true;

            // Act
            _settingsService.Save(settings);
            var loaded = _settingsService.Load();

            // Assert
            Assert.False(loaded.UI.StatusBar.Visible);
            Assert.True(loaded.UI.NavigationBar.Visible);
            Assert.Equal(50, loaded.Navigation.MaxHistorySize);
            Assert.True(loaded.UI.Search.CaseSensitive);
            Assert.Equal(14, loaded.Updates.CheckIntervalDays);
            Assert.True(loaded.Explorer.RegisterFileAssociation);
        }
    }
}
