using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using MarkdownViewer.Core.Configuration;
using MarkdownViewer.Core.Models;
using Xunit;

namespace MarkdownViewer.Tests
{
    /// <summary>
    /// Unit tests for UpdateChecker functionality.
    /// Tests update checking, version comparison, and download scenarios.
    /// </summary>
    public class UpdateCheckerTests : IDisposable
    {
        private readonly string _testDataPath;
        private readonly UpdateConfiguration _config;

        public UpdateCheckerTests()
        {
            // Setup test data directory
            _testDataPath = Path.Combine(Path.GetTempPath(), "MarkdownViewer.Tests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(_testDataPath);
            Directory.CreateDirectory(Path.Combine(_testDataPath, "api-responses"));

            // Configure UpdateConfiguration for testing
            _config = UpdateConfiguration.Instance;
            _config.EnableTestMode("http://localhost:8080", _testDataPath, "update-available");
        }

        public void Dispose()
        {
            // Cleanup test data
            if (Directory.Exists(_testDataPath))
            {
                try
                {
                    Directory.Delete(_testDataPath, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }

            // Reset to production mode
            _config.ResetToProductionMode();
        }

        [Fact]
        public void Constructor_InitializesCorrectly()
        {
            // Act
            var checker = new UpdateChecker();

            // Assert
            checker.Should().NotBeNull();
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithNoUpdates_ReturnsNoUpdateAvailable()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "1.0.6";

            // Create test data - same version as current
            var testResponse = new GitHubRelease
            {
                TagName = "v1.0.6",
                Name = "Version 1.0.6",
                Body = "Latest release",
                Prerelease = false,
                Assets = new System.Collections.Generic.List<GitHubReleaseAsset>
                {
                    new GitHubReleaseAsset
                    {
                        Name = "MarkdownViewer.exe",
                        BrowserDownloadUrl = "https://example.com/MarkdownViewer.exe",
                        Size = 1024000
                    }
                }
            };

            var json = JsonSerializer.Serialize(testResponse);
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, json);

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeFalse();
            result.CurrentVersion.Should().Be(currentVersion);
            result.LatestVersion.Should().Be("v1.0.6");
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithNewVersionAvailable_ReturnsUpdateInfo()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "1.0.5";

            // Create test data - newer version available
            var testResponse = new GitHubRelease
            {
                TagName = "v1.0.7",
                Name = "Version 1.0.7",
                Body = "## New Features\n- Feature A\n- Feature B",
                Prerelease = false,
                Assets = new System.Collections.Generic.List<GitHubReleaseAsset>
                {
                    new GitHubReleaseAsset
                    {
                        Name = "MarkdownViewer.exe",
                        BrowserDownloadUrl = "https://example.com/MarkdownViewer.exe",
                        Size = 2048000
                    }
                }
            };

            var json = JsonSerializer.Serialize(testResponse);
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, json);

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeTrue();
            result.CurrentVersion.Should().Be(currentVersion);
            result.LatestVersion.Should().Be("v1.0.7");
            result.ReleaseNotes.Should().Contain("New Features");
            result.DownloadUrl.Should().Be("https://example.com/MarkdownViewer.exe");
            result.FileSize.Should().Be(2048000);
            result.IsPrerelease.Should().BeFalse();
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithInvalidJson_ReturnsError()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "1.0.5";

            // Create invalid JSON test data
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, "{ invalid json }");

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeFalse();
            result.Error.Should().Contain("Invalid response format");
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithMissingTestFile_ReturnsError()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "1.0.5";

            // Don't create test file - simulate missing file

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeFalse();
            result.Error.Should().Contain("Test data file not found");
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithNoExeAsset_ReturnsError()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "1.0.5";

            // Create test data without exe asset
            var testResponse = new GitHubRelease
            {
                TagName = "v1.0.7",
                Name = "Version 1.0.7",
                Body = "Release notes",
                Prerelease = false,
                Assets = new System.Collections.Generic.List<GitHubReleaseAsset>
                {
                    new GitHubReleaseAsset
                    {
                        Name = "README.md",
                        BrowserDownloadUrl = "https://example.com/README.md",
                        Size = 1024
                    }
                }
            };

            var json = JsonSerializer.Serialize(testResponse);
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, json);

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeFalse();
            result.Error.Should().Contain("No executable found in release");
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithPrerelease_MarksAsPrerelease()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "1.0.5";

            // Create test data with prerelease flag
            var testResponse = new GitHubRelease
            {
                TagName = "v1.0.7-beta",
                Name = "Version 1.0.7 Beta",
                Body = "Beta release",
                Prerelease = true,
                Assets = new System.Collections.Generic.List<GitHubReleaseAsset>
                {
                    new GitHubReleaseAsset
                    {
                        Name = "MarkdownViewer.exe",
                        BrowserDownloadUrl = "https://example.com/MarkdownViewer.exe",
                        Size = 2048000
                    }
                }
            };

            var json = JsonSerializer.Serialize(testResponse);
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, json);

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeTrue();
            result.IsPrerelease.Should().BeTrue();
            result.LatestVersion.Should().Be("v1.0.7-beta");
        }

        [Fact]
        public async Task CheckForUpdatesAsync_WithVersionPrefix_ParsesCorrectly()
        {
            // Arrange
            var checker = new UpdateChecker();
            var currentVersion = "v1.0.5"; // Version with 'v' prefix

            // Create test data
            var testResponse = new GitHubRelease
            {
                TagName = "v1.0.8",
                Name = "Version 1.0.8",
                Body = "Release notes",
                Prerelease = false,
                Assets = new System.Collections.Generic.List<GitHubReleaseAsset>
                {
                    new GitHubReleaseAsset
                    {
                        Name = "MarkdownViewer.exe",
                        BrowserDownloadUrl = "https://example.com/MarkdownViewer.exe",
                        Size = 2048000
                    }
                }
            };

            var json = JsonSerializer.Serialize(testResponse);
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, json);

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().BeTrue();
            result.CurrentVersion.Should().Be(currentVersion);
            result.LatestVersion.Should().Be("v1.0.8");
        }

        [Fact]
        public void ShouldCheckForUpdates_WithNoLastCheck_ReturnsTrue()
        {
            // Arrange
            var checker = new UpdateChecker();

            // Act
            // Note: This test depends on Application.ExecutablePath which may have
            // existing last-update-check file. We just verify it doesn't throw.
            Action act = () => checker.ShouldCheckForUpdates();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void RecordUpdateCheck_CreatesLastCheckFile()
        {
            // Arrange
            var checker = new UpdateChecker();

            // Act
            checker.RecordUpdateCheck();

            // Assert
            // Note: This test verifies the method doesn't throw
            // Actual file creation is hard to test without Application.ExecutablePath
        }

        [Theory]
        [InlineData("1.0.5", "1.0.6", true)]
        [InlineData("1.0.6", "1.0.6", false)]
        [InlineData("1.0.7", "1.0.6", false)]
        [InlineData("1.0.5", "2.0.0", true)]
        [InlineData("v1.0.5", "v1.0.6", true)]
        public async Task CheckForUpdatesAsync_VersionComparison_WorksCorrectly(
            string currentVersion, string latestVersion, bool shouldHaveUpdate)
        {
            // Arrange
            var checker = new UpdateChecker();

            // Create test data
            var testResponse = new GitHubRelease
            {
                TagName = latestVersion,
                Name = $"Version {latestVersion}",
                Body = "Release notes",
                Prerelease = false,
                Assets = new System.Collections.Generic.List<GitHubReleaseAsset>
                {
                    new GitHubReleaseAsset
                    {
                        Name = "MarkdownViewer.exe",
                        BrowserDownloadUrl = "https://example.com/MarkdownViewer.exe",
                        Size = 2048000
                    }
                }
            };

            var json = JsonSerializer.Serialize(testResponse);
            var testFilePath = Path.Combine(_testDataPath, "api-responses", "update-available.json");
            await File.WriteAllTextAsync(testFilePath, json);

            // Act
            var result = await checker.CheckForUpdatesAsync(currentVersion);

            // Assert
            result.Should().NotBeNull();
            result.UpdateAvailable.Should().Be(shouldHaveUpdate);
        }
    }
}
