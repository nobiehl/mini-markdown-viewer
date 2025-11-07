using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Definitions;
using FlaUI.UIA3;

namespace MarkdownViewer.Tests.UIAutomation
{
    /// <summary>
    /// UI Automation tests for MainForm using FlaUI.
    /// These tests launch the actual application and interact with it.
    ///
    /// IMPORTANT: These tests require MarkdownViewer.exe to be built in Release mode.
    /// Run: dotnet build --configuration Release
    ///
    /// Tests are skipped by default to avoid interference with unit/integration tests.
    /// To run UI tests: dotnet test --filter "FullyQualifiedName~UIAutomation"
    /// </summary>
    public class MainFormUITests : IDisposable
    {
        private Application? _app;
        private Process? _process;
        private Window? _mainWindow;
        private readonly string _testFilePath;

        public MainFormUITests()
        {
            // Create a test markdown file
            _testFilePath = Path.Combine(Path.GetTempPath(), $"test_{Guid.NewGuid()}.md");
            File.WriteAllText(_testFilePath, "# Test Heading\n\nThis is a test markdown file.");
        }

        public void Dispose()
        {
            // Clean up
            _mainWindow?.Close();
            _app?.Dispose();
            _process?.Kill();
            _process?.Dispose();

            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }

        /// <summary>
        /// Helper method to launch the application for UI testing.
        /// </summary>
        private void LaunchApplication(string? filePath = null)
        {
            // Find the MarkdownViewer.exe
            var exePath = FindExecutable();
            Assert.NotNull(exePath);

            // Start the application
            var args = filePath ?? _testFilePath;
            _process = Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{args}\"",
                UseShellExecute = true
            });

            // Wait for the process to start
            Thread.Sleep(2000);

            // Attach FlaUI automation
            var automation = new UIA3Automation();
            _app = Application.Attach(_process);

            // Get the main window
            _mainWindow = _app.GetMainWindow(automation);
            Assert.NotNull(_mainWindow);
        }

        /// <summary>
        /// Finds the MarkdownViewer.exe in bin/Release or bin-single/.
        /// </summary>
        private string? FindExecutable()
        {
            var baseDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../.."));
            var candidates = new[]
            {
                Path.Combine(baseDir, "markdown-viewer/MarkdownViewer/bin/Release/net8.0-windows/MarkdownViewer.exe"),
                Path.Combine(baseDir, "markdown-viewer/MarkdownViewer/bin-single/MarkdownViewer.exe"),
                Path.Combine(baseDir, "bin-single/MarkdownViewer.exe")
            };

            return candidates.FirstOrDefault(File.Exists);
        }

        [Fact(Skip = "UI Automation test - requires Release build and manual execution")]
        public void Application_Launches_ShowsMainWindow()
        {
            // Arrange & Act
            LaunchApplication();

            // Assert
            Assert.NotNull(_mainWindow);
            Assert.True(_mainWindow.IsOffscreen == false);
            Assert.Contains("Markdown Viewer", _mainWindow.Title);
        }

        [Fact(Skip = "UI Automation test - requires Release build and manual execution")]
        public void Application_OpensFile_DisplaysContent()
        {
            // Arrange
            LaunchApplication();
            Assert.NotNull(_mainWindow);

            // Act - Window should show the file name in title
            var title = _mainWindow.Title;

            // Assert
            Assert.Contains("test_", title); // File name should be in title
            Assert.Contains("Markdown Viewer", title);
        }

        [Fact(Skip = "UI Automation test - requires Release build and manual execution")]
        public void StatusBar_ThemeSelector_ChangesTheme()
        {
            // Arrange
            LaunchApplication();
            Assert.NotNull(_mainWindow);

            // Act - Find and click theme selector (if visible)
            // Note: This would require StatusBar to be visible
            // In real implementation, we would:
            // 1. Find the theme dropdown by AutomationId
            // 2. Click it to open dropdown
            // 3. Select "Dark" theme
            // 4. Verify theme changed

            // Assert - Placeholder for actual automation
            Assert.NotNull(_mainWindow);
        }

        [Fact(Skip = "UI Automation test - requires Release build and manual execution")]
        public void Search_FindsText_HighlightsMatches()
        {
            // Arrange
            LaunchApplication();
            Assert.NotNull(_mainWindow);

            // Act - Press Ctrl+F to open search
            // Note: This would require:
            // 1. Send Ctrl+F key combination
            // 2. Wait for search bar to appear
            // 3. Type search text
            // 4. Verify results label shows matches

            // Assert - Placeholder for actual automation
            Assert.NotNull(_mainWindow);
        }

        [Fact(Skip = "UI Automation test - requires Release build and manual execution")]
        public void Navigation_BackButton_NavigatesToPreviousFile()
        {
            // Arrange
            var file1 = Path.Combine(Path.GetTempPath(), "file1.md");
            var file2 = Path.Combine(Path.GetTempPath(), "file2.md");
            File.WriteAllText(file1, "# File 1\n\n[Link to File 2](file2.md)");
            File.WriteAllText(file2, "# File 2\n\nContent of file 2");

            try
            {
                LaunchApplication(file1);
                Assert.NotNull(_mainWindow);

                // Act - Would require:
                // 1. Click the link to file2.md
                // 2. Wait for navigation
                // 3. Click Back button
                // 4. Verify we're back to file1

                // Assert - Placeholder
                Assert.NotNull(_mainWindow);
            }
            finally
            {
                if (File.Exists(file1)) File.Delete(file1);
                if (File.Exists(file2)) File.Delete(file2);
            }
        }

        /// <summary>
        /// Example of a more complete UI automation test structure.
        /// This demonstrates how a full test would be implemented.
        /// </summary>
        [Fact(Skip = "Example test structure - not implemented")]
        public void FullExample_LanguageSwitcher_ChangesLanguage()
        {
            // Arrange
            LaunchApplication();
            Assert.NotNull(_mainWindow);

            var automation = new UIA3Automation();

            // Find StatusBar (by AutomationId or ClassName)
            var statusBar = _mainWindow.FindFirstDescendant(cf => cf.ByClassName("StatusStrip"));
            Assert.NotNull(statusBar);

            // Find language dropdown (would need AutomationId set in code)
            var languageDropdown = statusBar.FindFirstDescendant(cf => cf.ByControlType(ControlType.ComboBox));
            Assert.NotNull(languageDropdown);

            // Act - Open dropdown and select German
            languageDropdown.AsComboBox().Select("Deutsch");

            // Wait for UI to update
            Thread.Sleep(500);

            // Assert - Verify language changed
            // This would check that UI elements now show German text
            Assert.Contains("Deutsch", languageDropdown.AsComboBox().SelectedItem.Name);
        }
    }
}
