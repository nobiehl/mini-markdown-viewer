using MarkdownViewer.Core.Models;
using MarkdownViewer.Core.Services;

namespace MarkdownViewer.Tests.Mocks
{
    /// <summary>
    /// Mock implementation of ISettingsService for testing.
    /// </summary>
    public class MockSettingsService : ISettingsService
    {
        public AppSettings MockSettings { get; set; } = new AppSettings();
        public int LoadCallCount { get; private set; }
        public int SaveCallCount { get; private set; }

        public AppSettings Load()
        {
            LoadCallCount++;
            return MockSettings;
        }

        public void Save(AppSettings settings)
        {
            SaveCallCount++;
            MockSettings = settings;
        }

        public string GetSettingsPath()
        {
            return "C:\\mock\\settings.json";
        }
    }

    /// <summary>
    /// Mock implementation of IThemeService for testing.
    /// </summary>
    public class MockThemeService : IThemeService
    {
        public Dictionary<string, Theme> MockThemes { get; set; } = new Dictionary<string, Theme>();
        public bool ThrowOnLoadTheme { get; set; }
        public int LoadThemeCallCount { get; private set; }
        public int GetAvailableThemesCallCount { get; private set; }

        private Theme _currentTheme;

        public MockThemeService()
        {
            // Add default themes
            MockThemes["dark"] = new Theme { Name = "dark" };
            MockThemes["standard"] = new Theme { Name = "standard" };
            MockThemes["solarized"] = new Theme { Name = "solarized" };
            MockThemes["draeger"] = new Theme { Name = "draeger" };

            _currentTheme = MockThemes["standard"];
        }

        public Theme GetCurrentTheme()
        {
            return _currentTheme;
        }

        public Theme LoadTheme(string themeName)
        {
            LoadThemeCallCount++;
            if (ThrowOnLoadTheme)
                throw new Exception($"Mock exception for theme: {themeName}");

            var theme = MockThemes.ContainsKey(themeName)
                ? MockThemes[themeName]
                : new Theme { Name = themeName };

            _currentTheme = theme;
            return theme;
        }

        public List<string> GetAvailableThemes()
        {
            GetAvailableThemesCallCount++;
            return MockThemes.Keys.ToList();
        }

        public Task ApplyThemeAsync(Theme theme, System.Windows.Forms.Form form, Microsoft.Web.WebView2.WinForms.WebView2 webView)
        {
            // Mock implementation - does nothing
            _currentTheme = theme;
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Mock implementation of ILocalizationService for testing.
    /// </summary>
    public class MockLocalizationService : ILocalizationService
    {
        public string CurrentLanguage { get; private set; } = "en";
        public int SetLanguageCallCount { get; private set; }
        public int GetSupportedLanguagesCallCount { get; private set; }

        private readonly Dictionary<string, string> _mockStrings = new Dictionary<string, string>
        {
            ["AppName"] = "Markdown Viewer",
            ["Error"] = "Error",
            ["Success"] = "Success"
        };

        public string GetString(string key)
        {
            return _mockStrings.ContainsKey(key) ? _mockStrings[key] : $"[{key}]";
        }

        public string GetString(string key, params object[] args)
        {
            var format = GetString(key);
            return string.Format(format, args);
        }

        public void SetLanguage(string languageCode)
        {
            SetLanguageCallCount++;
            CurrentLanguage = languageCode;
        }

        public string GetCurrentLanguage()
        {
            return CurrentLanguage;
        }

        public string[] GetSupportedLanguages()
        {
            GetSupportedLanguagesCallCount++;
            return new[] { "en", "de", "mn", "fr", "es", "ja", "zh", "ru" };
        }
    }
}
