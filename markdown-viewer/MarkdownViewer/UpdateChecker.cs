using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Serilog;
using MarkdownViewer.Configuration;
using MarkdownViewer.Models;

namespace MarkdownViewer
{
    /// <summary>
    /// Manages application updates from GitHub Releases.
    /// Supports:
    /// - Automatic update checks (once per day)
    /// - Manual update checks via --update parameter
    /// - Download and installation of updates
    /// - Test mode for development (no real downloads)
    /// </summary>
    public class UpdateChecker
    {
        private readonly UpdateConfiguration _config;
        private const string LastCheckFile = "last-update-check.txt";
        private const string PendingUpdateFile = "pending-update.exe";

        /// <summary>
        /// Initializes a new UpdateChecker with the current configuration
        /// </summary>
        public UpdateChecker()
        {
            _config = UpdateConfiguration.Instance;
        }

        /// <summary>
        /// Checks GitHub API for available updates.
        /// Compares current version with latest release version.
        /// Returns UpdateInfo with details about available update (if any).
        /// </summary>
        /// <param name="currentVersion">Current application version (e.g., "1.0.5")</param>
        /// <returns>UpdateInfo object with update availability and details</returns>
        public async Task<UpdateInfo> CheckForUpdatesAsync(string currentVersion)
        {
            Log.Information("Checking for updates (current: v{Version}, mode: {Mode})",
                currentVersion, _config.IsTestMode ? "Test" : "Production");
            Log.Debug("Configuration: {Config}", _config.ToString());

            try
            {
                string response;

                if (_config.IsTestMode)
                {
                    // Test mode: Read from local JSON files
                    string testFilePath = Path.Combine(_config.TestDataPath, "api-responses", $"{_config.TestScenario}.json");
                    Log.Debug("Reading test data from: {Path}", testFilePath);

                    if (!File.Exists(testFilePath))
                    {
                        Log.Error("Test data file not found: {Path}", testFilePath);
                        return new UpdateInfo
                        {
                            UpdateAvailable = false,
                            Error = $"Test data file not found: {testFilePath}"
                        };
                    }

                    response = await File.ReadAllTextAsync(testFilePath);
                    Log.Debug("Test data loaded ({Length} bytes)", response.Length);
                }
                else
                {
                    // Production mode: Fetch from GitHub API
                    string apiUrl = _config.GetLatestReleaseUrl();
                    Log.Debug("Fetching release info from: {Url}", apiUrl);

                    using var client = new HttpClient();
                    client.DefaultRequestHeaders.Add("User-Agent", "MarkdownViewer");
                    client.Timeout = TimeSpan.FromSeconds(30);

                    response = await client.GetStringAsync(apiUrl);
                    Log.Debug("API Response received ({Length} bytes)", response.Length);
                }

                // Parse JSON response
                var release = JsonSerializer.Deserialize<GitHubRelease>(response);

                if (release == null || string.IsNullOrEmpty(release.TagName))
                {
                    Log.Warning("Invalid release response: missing tag_name");
                    return new UpdateInfo
                    {
                        UpdateAvailable = false,
                        Error = "Invalid release format"
                    };
                }

                Log.Information("Latest release found: {Tag} (prerelease: {Prerelease})",
                    release.TagName, release.Prerelease);

                // Compare versions
                Version current = ParseVersion(currentVersion);
                Version latest = ParseVersion(release.TagName);

                Log.Debug("Version comparison: current={Current}, latest={Latest}",
                    current, latest);

                bool updateAvailable = latest > current;
                Log.Information("Update available: {Available}", updateAvailable);

                if (!updateAvailable)
                {
                    return new UpdateInfo
                    {
                        UpdateAvailable = false,
                        CurrentVersion = currentVersion,
                        LatestVersion = release.TagName
                    };
                }

                // Find exe asset
                var exeAsset = release.Assets?.FirstOrDefault(a =>
                    a.Name.Equals("MarkdownViewer.exe", StringComparison.OrdinalIgnoreCase));

                if (exeAsset == null)
                {
                    Log.Warning("No MarkdownViewer.exe asset found in release");
                    return new UpdateInfo
                    {
                        UpdateAvailable = false,
                        Error = "No executable found in release"
                    };
                }

                Log.Information("Update asset found: {Name} ({Size} bytes)",
                    exeAsset.Name, exeAsset.Size);

                return new UpdateInfo
                {
                    UpdateAvailable = true,
                    CurrentVersion = currentVersion,
                    LatestVersion = release.TagName,
                    ReleaseNotes = release.Body ?? "No release notes available.",
                    DownloadUrl = exeAsset.BrowserDownloadUrl,
                    FileSize = exeAsset.Size,
                    IsPrerelease = release.Prerelease
                };
            }
            catch (HttpRequestException ex)
            {
                Log.Warning("Update check failed (network error): {Message}", ex.Message);
                return new UpdateInfo
                {
                    UpdateAvailable = false,
                    Error = $"Network error: {ex.Message}"
                };
            }
            catch (JsonException ex)
            {
                Log.Warning("Update check failed (invalid JSON): {Message}", ex.Message);
                return new UpdateInfo
                {
                    UpdateAvailable = false,
                    Error = $"Invalid response format: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error during update check");
                return new UpdateInfo
                {
                    UpdateAvailable = false,
                    Error = $"Unexpected error: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Checks if an update check should be performed.
        /// Returns true if:
        /// - No previous check recorded (first run)
        /// - Last check was on a different day
        /// </summary>
        /// <returns>True if update check should be performed</returns>
        public bool ShouldCheckForUpdates()
        {
            try
            {
                string lastCheckPath = GetLastCheckFilePath();

                if (!File.Exists(lastCheckPath))
                {
                    Log.Debug("No previous update check recorded");
                    return true;
                }

                string lastCheck = File.ReadAllText(lastCheckPath).Trim();
                string today = DateTime.Now.ToString("yyyy-MM-dd");

                bool shouldCheck = lastCheck != today;
                Log.Debug("Last check: {LastCheck}, Today: {Today}, Should check: {ShouldCheck}",
                    lastCheck, today, shouldCheck);

                return shouldCheck;
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Error reading last update check file, assuming check is needed");
                return true;
            }
        }

        /// <summary>
        /// Records that an update check was performed today.
        /// Writes current date to last-update-check.txt in logs folder.
        /// </summary>
        public void RecordUpdateCheck()
        {
            try
            {
                string lastCheckPath = GetLastCheckFilePath();
                string logsFolder = Path.GetDirectoryName(lastCheckPath);

                // Ensure logs folder exists
                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                string today = DateTime.Now.ToString("yyyy-MM-dd");
                File.WriteAllText(lastCheckPath, today);

                Log.Debug("Update check recorded: {Date}", today);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to record update check");
            }
        }

        /// <summary>
        /// Downloads an update from the specified URL.
        /// Saves to pending-update.exe in application directory.
        /// </summary>
        /// <param name="url">Download URL from GitHub release</param>
        /// <returns>True if download succeeded</returns>
        public async Task<bool> DownloadUpdateAsync(string url)
        {
            string targetPath = GetPendingUpdatePath();

            try
            {
                Log.Information("Downloading update from: {Url}", url);
                Log.Information("Target path: {Path}", targetPath);

                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromMinutes(5); // 5 minute timeout for download

                var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long? contentLength = response.Content.Headers.ContentLength;
                Log.Debug("Content length: {Length} bytes", contentLength ?? 0);

                using var fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write, FileShare.None);
                await response.Content.CopyToAsync(fileStream);

                long downloadedSize = new FileInfo(targetPath).Length;
                Log.Information("Update downloaded successfully: {Size} bytes", downloadedSize);

                // Verify file size matches (if content-length was provided)
                if (contentLength.HasValue && downloadedSize != contentLength.Value)
                {
                    Log.Warning("Downloaded size ({Downloaded}) doesn't match expected size ({Expected})",
                        downloadedSize, contentLength.Value);
                }

                return true;
            }
            catch (HttpRequestException ex)
            {
                Log.Error(ex, "Download failed (network error)");
                TryDeleteFile(targetPath);
                return false;
            }
            catch (IOException ex)
            {
                Log.Error(ex, "Download failed (file I/O error)");
                TryDeleteFile(targetPath);
                return false;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Download failed (unexpected error)");
                TryDeleteFile(targetPath);
                return false;
            }
        }

        /// <summary>
        /// Applies a pending update if one exists.
        /// Called at application startup before MainForm is created.
        /// Steps:
        /// 1. Check if pending-update.exe exists
        /// 2. Create backup of current .exe
        /// 3. Replace current .exe with pending update
        /// 4. Delete backup
        /// 5. Restart application
        /// </summary>
        public static void ApplyPendingUpdate()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                string exeDir = Path.GetDirectoryName(exePath);
                string updatePath = Path.Combine(exeDir, PendingUpdateFile);

                if (!File.Exists(updatePath))
                {
                    // No pending update
                    return;
                }

                Log.Information("=== Applying pending update ===");
                Log.Information("Current exe: {CurrentExe}", exePath);
                Log.Information("Update file: {UpdateFile}", updatePath);

                // Create backup
                string backupPath = exePath + ".backup";
                if (File.Exists(backupPath))
                {
                    Log.Debug("Removing old backup");
                    File.Delete(backupPath);
                }

                Log.Information("Creating backup: {BackupPath}", backupPath);
                File.Move(exePath, backupPath);

                // Apply update
                Log.Information("Moving update to exe location");
                File.Move(updatePath, exePath);

                // Delete backup on success
                Log.Information("Deleting backup");
                File.Delete(backupPath);

                Log.Information("Update applied successfully!");

                // Restart application with same arguments
                var args = Environment.GetCommandLineArgs().Skip(1);
                string arguments = string.Join(" ", args.Select(a => $"\"{a}\""));

                Log.Information("Restarting application: {Exe} {Args}", exePath, arguments);

                Process.Start(new ProcessStartInfo
                {
                    FileName = exePath,
                    Arguments = arguments,
                    UseShellExecute = true
                });

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply pending update");

                // Attempt rollback
                try
                {
                    string exePath = Application.ExecutablePath;
                    string backupPath = exePath + ".backup";

                    if (File.Exists(backupPath) && !File.Exists(exePath))
                    {
                        Log.Information("Rolling back to backup");
                        File.Move(backupPath, exePath);
                        Log.Information("Rollback successful");
                    }
                }
                catch (Exception rollbackEx)
                {
                    Log.Error(rollbackEx, "Rollback also failed!");
                }

                MessageBox.Show(
                    $"Update installation failed:\n{ex.Message}\n\nThe application may need to be reinstalled.",
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        /// <summary>
        /// Parses a version string into a Version object.
        /// Handles formats: "1.0.5", "v1.0.5", "1.0.5-beta"
        /// </summary>
        private static Version ParseVersion(string versionString)
        {
            try
            {
                // Remove 'v' prefix if present
                string cleaned = versionString.TrimStart('v', 'V');

                // Remove prerelease suffix if present (e.g., "-beta", "-alpha")
                int dashIndex = cleaned.IndexOf('-');
                if (dashIndex > 0)
                {
                    cleaned = cleaned.Substring(0, dashIndex);
                }

                return new Version(cleaned);
            }
            catch (Exception ex)
            {
                Log.Warning("Failed to parse version '{Version}': {Error}", versionString, ex.Message);
                return new Version(0, 0, 0);
            }
        }

        /// <summary>
        /// Gets the full path to the last-update-check.txt file
        /// </summary>
        private static string GetLastCheckFilePath()
        {
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            string logsFolder = Path.Combine(exeDir, "logs");
            return Path.Combine(logsFolder, LastCheckFile);
        }

        /// <summary>
        /// Gets the full path to the pending-update.exe file
        /// </summary>
        private static string GetPendingUpdatePath()
        {
            string exeDir = Path.GetDirectoryName(Application.ExecutablePath);
            return Path.Combine(exeDir, PendingUpdateFile);
        }

        /// <summary>
        /// Tries to delete a file, ignoring errors
        /// </summary>
        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // Ignore deletion errors
            }
        }
    }
}
