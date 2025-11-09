using System;
using System.Globalization;
using System.Resources;

namespace MarkdownViewer.Core.Services
{
    /// <summary>
    /// Interface for localization service.
    /// Provides access to localized strings for the UI.
    /// </summary>
    public interface ILocalizationService
    {
        /// <summary>
        /// Gets a localized string by key.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localized string or key if not found</returns>
        string GetString(string key);

        /// <summary>
        /// Gets a localized string by key with format parameters.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Formatted localized string or key if not found</returns>
        string GetString(string key, params object[] args);

        /// <summary>
        /// Sets the current UI culture for localization.
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "en", "de", "mn")</param>
        void SetLanguage(string languageCode);

        /// <summary>
        /// Gets the current language code.
        /// </summary>
        string GetCurrentLanguage();

        /// <summary>
        /// Gets an array of supported language codes.
        /// </summary>
        string[] GetSupportedLanguages();
    }

    /// <summary>
    /// Service for managing localization using .NET resource files (.resx).
    /// Supports 8 languages: English (en), German (de), Mongolian (mn),
    /// French (fr), Spanish (es), Japanese (ja), Chinese (zh), Russian (ru).
    /// </summary>
    public class LocalizationService : ILocalizationService
    {
        private readonly ResourceManager _resourceManager;
        private CultureInfo _currentCulture = CultureInfo.InvariantCulture;

        private static readonly string[] SupportedLanguages = new[]
        {
            "en", // English
            "de", // German
            "mn", // Mongolian
            "fr", // French
            "es", // Spanish
            "ja", // Japanese
            "zh", // Chinese (Simplified)
            "ru"  // Russian
        };

        /// <summary>
        /// Initializes a new instance of the LocalizationService.
        /// Loads the resource manager for the Strings resource file.
        /// </summary>
        /// <param name="defaultLanguage">Default language code (defaults to "en" if not specified or invalid)</param>
        public LocalizationService(string defaultLanguage = "en")
        {
            // Initialize ResourceManager to load from Resources/Strings.resx
            _resourceManager = new ResourceManager(
                "MarkdownViewer.Resources.Strings",
                typeof(LocalizationService).Assembly
            );

            // Set initial culture
            SetLanguage(defaultLanguage);
        }

        /// <summary>
        /// Gets a localized string by key.
        /// Falls back to English if the key is not found in the current language.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <returns>Localized string or key surrounded by brackets if not found</returns>
        public string GetString(string key)
        {
            try
            {
                // Try to get string in current culture
                string? value = _resourceManager.GetString(key, _currentCulture);

                if (!string.IsNullOrEmpty(value))
                    return value;

                // Fallback to English
                if (_currentCulture.TwoLetterISOLanguageName != "en")
                {
                    value = _resourceManager.GetString(key, CultureInfo.GetCultureInfo("en"));
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }

                // Key not found - return key with brackets
                return $"[{key}]";
            }
            catch (Exception)
            {
                // Error accessing resource - return key with brackets
                return $"[{key}]";
            }
        }

        /// <summary>
        /// Gets a localized string by key with format parameters.
        /// </summary>
        /// <param name="key">Resource key</param>
        /// <param name="args">Format arguments</param>
        /// <returns>Formatted localized string or key if not found</returns>
        public string GetString(string key, params object[] args)
        {
            string format = GetString(key);

            try
            {
                return string.Format(format, args);
            }
            catch (FormatException)
            {
                // Format failed - return unformatted string
                return format;
            }
        }

        /// <summary>
        /// Sets the current UI culture for localization.
        /// If the language code is invalid or not supported, falls back to English.
        /// </summary>
        /// <param name="languageCode">Language code (e.g., "en", "de", "mn", "system")</param>
        public void SetLanguage(string languageCode)
        {
            try
            {
                // Handle "system" language code by using current system culture
                if (languageCode == "system")
                {
                    var systemCulture = CultureInfo.CurrentUICulture;
                    string systemLang = systemCulture.TwoLetterISOLanguageName;

                    // Check if system language is supported
                    if (Array.Exists(SupportedLanguages, lang => lang == systemLang))
                    {
                        _currentCulture = CultureInfo.GetCultureInfo(systemLang);
                    }
                    else
                    {
                        // System language not supported, use English
                        _currentCulture = CultureInfo.GetCultureInfo("en");
                    }
                }
                // Check if requested language is supported
                else if (Array.Exists(SupportedLanguages, lang => lang == languageCode))
                {
                    _currentCulture = CultureInfo.GetCultureInfo(languageCode);
                }
                else
                {
                    // Language not supported, fallback to English
                    _currentCulture = CultureInfo.GetCultureInfo("en");
                }

                // Set thread culture for consistency
                CultureInfo.CurrentUICulture = _currentCulture;
            }
            catch (CultureNotFoundException)
            {
                // Invalid culture code, fallback to English
                _currentCulture = CultureInfo.GetCultureInfo("en");
                CultureInfo.CurrentUICulture = _currentCulture;
            }
        }

        /// <summary>
        /// Gets the current language code.
        /// </summary>
        /// <returns>Two-letter ISO language code (e.g., "en", "de")</returns>
        public string GetCurrentLanguage()
        {
            return _currentCulture.TwoLetterISOLanguageName;
        }

        /// <summary>
        /// Gets an array of supported language codes.
        /// </summary>
        /// <returns>Array of two-letter ISO language codes</returns>
        public string[] GetSupportedLanguages()
        {
            return (string[])SupportedLanguages.Clone();
        }
    }
}
