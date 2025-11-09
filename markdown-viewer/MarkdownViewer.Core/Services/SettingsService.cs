using System;
using System.IO;
using System.Text.Json;
using MarkdownViewer.Core.Models;
using Serilog;

namespace MarkdownViewer.Core.Services
{
    /// <summary>
    /// Interface for settings management operations
    /// </summary>
    public interface ISettingsService
    {
        /// <summary>
        /// Loads settings from JSON file.
        /// Returns default settings if file doesn't exist or is invalid.
        /// </summary>
        AppSettings Load();

        /// <summary>
        /// Saves settings to JSON file.
        /// Creates directory if it doesn't exist.
        /// </summary>
        void Save(AppSettings settings);

        /// <summary>
        /// Gets the full path to the settings file
        /// </summary>
        string GetSettingsPath();
    }

    /// <summary>
    /// Settings service implementation using JSON file storage.
    /// Settings are stored at: %APPDATA%/MarkdownViewer/settings.json
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private readonly string _settingsPath;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public SettingsService()
        {
            // Settings location: %APPDATA%/MarkdownViewer/settings.json
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "MarkdownViewer");
            _settingsPath = Path.Combine(appFolder, "settings.json");

            Log.Information("SettingsService initialized. Path: {Path}", _settingsPath);
        }

        /// <summary>
        /// Constructor for testing with custom path
        /// </summary>
        internal SettingsService(string customPath)
        {
            _settingsPath = customPath;
            Log.Information("SettingsService initialized with custom path: {Path}", _settingsPath);
        }

        public AppSettings Load()
        {
            try
            {
                if (!File.Exists(_settingsPath))
                {
                    Log.Information("Settings file not found, using defaults: {Path}", _settingsPath);
                    return new AppSettings();
                }

                var json = File.ReadAllText(_settingsPath);
                var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);

                if (settings == null)
                {
                    Log.Warning("Failed to deserialize settings, using defaults");
                    return new AppSettings();
                }

                Log.Information("Settings loaded successfully from: {Path}", _settingsPath);
                return settings;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load settings from: {Path}", _settingsPath);
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(_settingsPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                    Log.Information("Created settings directory: {Directory}", directory);
                }

                // Serialize to JSON with indentation
                var json = JsonSerializer.Serialize(settings, _jsonOptions);

                // Write to file
                File.WriteAllText(_settingsPath, json);

                Log.Information("Settings saved successfully to: {Path}", _settingsPath);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to save settings to: {Path}", _settingsPath);
                throw;
            }
        }

        public string GetSettingsPath()
        {
            return _settingsPath;
        }
    }
}
