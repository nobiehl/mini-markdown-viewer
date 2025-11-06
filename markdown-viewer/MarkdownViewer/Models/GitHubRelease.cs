using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MarkdownViewer.Models
{
    /// <summary>
    /// Model for GitHub Release API response.
    /// Represents a release from GitHub's REST API v3.
    /// Documentation: https://docs.github.com/en/rest/releases/releases
    /// </summary>
    public class GitHubRelease
    {
        /// <summary>
        /// Tag name of the release (e.g., "v1.0.5")
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        /// <summary>
        /// Display name of the release (e.g., "v1.0.5: Math Support")
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Release description/notes in Markdown format
        /// </summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        /// <summary>
        /// Publication timestamp (ISO 8601 format)
        /// </summary>
        [JsonPropertyName("published_at")]
        public string? PublishedAt { get; set; }

        /// <summary>
        /// Whether this is a prerelease version
        /// </summary>
        [JsonPropertyName("prerelease")]
        public bool Prerelease { get; set; }

        /// <summary>
        /// List of downloadable assets (binaries, archives, etc.)
        /// </summary>
        [JsonPropertyName("assets")]
        public List<GitHubReleaseAsset>? Assets { get; set; }
    }

    /// <summary>
    /// Model for GitHub Release Asset (downloadable file).
    /// </summary>
    public class GitHubReleaseAsset
    {
        /// <summary>
        /// File name (e.g., "MarkdownViewer.exe")
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Direct download URL
        /// </summary>
        [JsonPropertyName("browser_download_url")]
        public string? BrowserDownloadUrl { get; set; }

        /// <summary>
        /// File size in bytes
        /// </summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }

        /// <summary>
        /// MIME type (e.g., "application/x-msdownload")
        /// </summary>
        [JsonPropertyName("content_type")]
        public string? ContentType { get; set; }
    }

    /// <summary>
    /// Result of an update check operation.
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// Whether a newer version is available
        /// </summary>
        public bool UpdateAvailable { get; set; }

        /// <summary>
        /// Latest version tag (e.g., "v1.0.6")
        /// </summary>
        public string LatestVersion { get; set; } = string.Empty;

        /// <summary>
        /// Current installed version
        /// </summary>
        public string CurrentVersion { get; set; } = string.Empty;

        /// <summary>
        /// Release notes (Markdown)
        /// </summary>
        public string ReleaseNotes { get; set; } = string.Empty;

        /// <summary>
        /// Direct download URL for the new version
        /// </summary>
        public string DownloadUrl { get; set; } = string.Empty;

        /// <summary>
        /// File size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Error message if update check failed
        /// </summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is a prerelease version
        /// </summary>
        public bool IsPrerelease { get; set; }
    }
}
