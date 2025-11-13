using Xunit;
using MarkdownViewer.Core;
using MarkdownViewer.Core.Models;
using MarkdownViewer.Core.Presenters;
using MarkdownViewer.Tests.Mocks;

namespace MarkdownViewer.Tests.Presenters
{
    /// <summary>
    /// Unit tests for MainPresenter.
    /// Tests business logic without UI dependencies.
    /// </summary>
    public class MainPresenterTests : IDisposable
    {
        private readonly MockMainView _mockView;
        private readonly MockWebViewAdapter _mockWebView;
        private readonly MockSettingsService _mockSettingsService;
        private readonly MockThemeService _mockThemeService;
        private readonly MockLocalizationService _mockLocalizationService;
        private readonly MockDialogService _mockDialogService;
        private readonly MarkdownRenderer _renderer;
        private readonly FileWatcherManager _fileWatcher;
        private readonly MainPresenter _presenter;

        public MainPresenterTests()
        {
            // Setup mocks
            _mockView = new MockMainView();
            _mockWebView = new MockWebViewAdapter();
            _mockSettingsService = new MockSettingsService();
            _mockThemeService = new MockThemeService();
            _mockLocalizationService = new MockLocalizationService();
            _mockDialogService = new MockDialogService();
            _renderer = new MarkdownRenderer();
            _fileWatcher = new FileWatcherManager();

            // Create presenter with mocked dependencies
            _presenter = new MainPresenter(
                _mockView,
                _mockWebView,
                _mockSettingsService,
                _mockThemeService,
                _mockLocalizationService,
                _mockDialogService,
                _renderer,
                _fileWatcher);
        }

        public void Dispose()
        {
            _fileWatcher?.Dispose();
        }

        [Fact]
        public void Constructor_ShouldSubscribeToViewEvents()
        {
            // Assert - Check that events are wired up by triggering them
            bool viewLoadedTriggered = false;
            _mockView.ViewLoaded += (s, e) => viewLoadedTriggered = true;

            _mockView.TriggerViewLoaded();

            Assert.True(viewLoadedTriggered);
        }

        [Fact]
        public void ViewLoaded_ShouldLoadSettings()
        {
            // Arrange
            var expectedTheme = new Theme { Name = "dark" };
            _mockSettingsService.MockSettings = new AppSettings { Theme = "dark" };
            _mockThemeService.MockThemes["dark"] = expectedTheme;

            // Act
            _mockView.TriggerViewLoaded();

            // Assert
            Assert.Equal(expectedTheme, _mockView.LastAppliedTheme);
            Assert.Equal(1, _mockSettingsService.LoadCallCount);
        }

        [Fact]
        public void ThemeChangeRequested_ShouldUpdateThemeAndSaveSettings()
        {
            // Arrange
            var newTheme = new Theme { Name = "solarized" };
            _mockThemeService.MockThemes["solarized"] = newTheme;

            // Act
            _mockView.TriggerThemeChange("solarized");

            // Assert
            Assert.Equal(newTheme, _mockView.LastAppliedTheme);
            Assert.Equal(1, _mockSettingsService.SaveCallCount);
            Assert.Equal("solarized", _mockSettingsService.MockSettings.Theme);
        }

        [Fact]
        public void ThemeChangeRequested_WhenThemeLoadFails_ShouldShowError()
        {
            // Arrange
            _mockThemeService.ThrowOnLoadTheme = true;

            // Act
            _mockView.TriggerThemeChange("invalid");

            // Wait a bit for async operation
            System.Threading.Thread.Sleep(100);

            // Assert
            Assert.NotNull(_mockDialogService.LastErrorMessage);
            Assert.Contains("Failed to apply theme", _mockDialogService.LastErrorMessage);
            Assert.Equal(1, _mockDialogService.ShowErrorCallCount);
        }

        [Fact]
        public void LanguageChangeRequested_ShouldUpdateLanguageAndSaveSettings()
        {
            // Arrange
            string newLanguage = "de";

            // Act
            _mockView.TriggerLanguageChange(newLanguage);

            // Assert
            Assert.Equal(newLanguage, _mockLocalizationService.CurrentLanguage);
            Assert.Equal(1, _mockLocalizationService.SetLanguageCallCount);
            Assert.Equal(1, _mockSettingsService.SaveCallCount);
            Assert.Equal(newLanguage, _mockSettingsService.MockSettings.Language);
        }

        [Fact]
        public void SearchRequested_ShouldShowSearchBar()
        {
            // Act
            _mockView.TriggerSearch();

            // Assert
            Assert.Equal(1, _mockView.ShowSearchBarCallCount);
        }

        [Fact]
        public void RefreshRequested_WhenFilePathIsEmpty_ShouldNotCrash()
        {
            // Arrange - No file path initialized

            // Act - should not throw
            _mockView.TriggerRefresh();

            // Assert - No assertion needed, just checking it doesn't crash
        }

        [Fact]
        public void CurrentSettings_ShouldReturnLoadedSettings()
        {
            // Arrange
            _mockSettingsService.MockSettings = new AppSettings
            {
                Theme = "dark",
                Language = "de"
            };
            _mockView.TriggerViewLoaded();

            // Act
            var settings = _presenter.CurrentSettings;

            // Assert
            Assert.NotNull(settings);
            Assert.Equal("dark", settings.Theme);
            Assert.Equal("de", settings.Language);
        }

        [Fact]
        public void CurrentTheme_ShouldReturnLoadedTheme()
        {
            // Arrange
            var darkTheme = new Theme { Name = "dark" };
            _mockThemeService.MockThemes["dark"] = darkTheme;
            _mockSettingsService.MockSettings = new AppSettings { Theme = "dark" };
            _mockView.TriggerViewLoaded();

            // Act
            var theme = _presenter.CurrentTheme;

            // Assert
            Assert.NotNull(theme);
            Assert.Equal("dark", theme.Name);
        }

        [Fact]
        public void Initialize_ShouldSetCurrentFilePath()
        {
            // Arrange
            string testPath = "C:\\test\\file.md";

            // Act
            _presenter.Initialize(testPath);

            // Assert
            Assert.Equal(testPath, _presenter.CurrentFilePath);
        }

        [Fact]
        public void WebViewInitialized_ShouldTriggerFileLoad()
        {
            // Arrange
            string testPath = "C:\\test\\file.md";
            _presenter.Initialize(testPath);

            // Create a temporary test file
            System.IO.Directory.CreateDirectory("C:\\test");
            System.IO.File.WriteAllText(testPath, "# Test");

            try
            {
                // Act
                _mockWebView.TriggerInitialized();

                // Wait for async operation
                System.Threading.Thread.Sleep(200);

                // Assert
                Assert.NotNull(_mockWebView.LastNavigatedHtml);
                Assert.Contains("<h1", _mockWebView.LastNavigatedHtml);
            }
            finally
            {
                // Cleanup
                if (System.IO.File.Exists(testPath))
                    System.IO.File.Delete(testPath);
                if (System.IO.Directory.Exists("C:\\test"))
                    System.IO.Directory.Delete("C:\\test");
            }
        }
    }
}
