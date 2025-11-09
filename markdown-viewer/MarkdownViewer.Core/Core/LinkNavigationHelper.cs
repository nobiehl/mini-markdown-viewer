using System;
using System.IO;
using Serilog;

namespace MarkdownViewer.Core
{
    /// <summary>
    /// Helper class for link navigation logic.
    /// Provides testable methods for path resolution, validation, and link type detection.
    /// </summary>
    public static class LinkNavigationHelper
    {
        /// <summary>
        /// Resolves a potentially relative path to an absolute path based on the current file's directory.
        /// </summary>
        /// <param name="linkPath">The path from the link (relative or absolute)</param>
        /// <param name="currentFilePath">The path of the currently open file</param>
        /// <returns>The resolved absolute path</returns>
        public static string ResolveRelativePath(string linkPath, string currentFilePath)
        {
            if (string.IsNullOrWhiteSpace(linkPath))
                throw new ArgumentException("Link path cannot be null or empty", nameof(linkPath));

            if (string.IsNullOrWhiteSpace(currentFilePath))
                throw new ArgumentException("Current file path cannot be null or empty", nameof(currentFilePath));

            // If already absolute, just return it normalized
            if (Path.IsPathRooted(linkPath))
            {
                return Path.GetFullPath(linkPath);
            }

            // Relative path - resolve relative to current file's directory
            string currentFileDirectory = Path.GetDirectoryName(currentFilePath) ?? ".";
            return Path.GetFullPath(Path.Combine(currentFileDirectory, linkPath));
        }

        /// <summary>
        /// Determines the type of link based on its format.
        /// </summary>
        public enum LinkType
        {
            Unknown,
            ExternalHttp,
            LocalMarkdown,
            Anchor,
            InlineResource
        }

        /// <summary>
        /// Determines what type of link this is.
        /// </summary>
        public static LinkType GetLinkType(string link)
        {
            if (string.IsNullOrWhiteSpace(link))
                return LinkType.Unknown;

            // HTTP/HTTPS
            if (link.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                link.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return LinkType.ExternalHttp;
            }

            // Anchor-only
            if (link.StartsWith("#"))
            {
                return LinkType.Anchor;
            }

            // Local markdown file
            if (link.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                link.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
            {
                return LinkType.LocalMarkdown;
            }

            return LinkType.Unknown;
        }

        /// <summary>
        /// Validates that a file exists and logs the result.
        /// </summary>
        /// <returns>True if file exists, false otherwise</returns>
        public static bool ValidateFileExists(string resolvedPath, string originalPath)
        {
            if (string.IsNullOrWhiteSpace(resolvedPath))
            {
                Log.Warning("ValidateFileExists: Resolved path is null or empty | Original: {OriginalPath}", originalPath);
                return false;
            }

            bool exists = File.Exists(resolvedPath);

            if (!exists)
            {
                Log.Warning("File not found: {ResolvedPath} | Original link: {OriginalPath} | Navigation aborted",
                    resolvedPath, originalPath);
            }
            else
            {
                try
                {
                    var fileInfo = new FileInfo(resolvedPath);
                    Log.Information("File exists: {ResolvedPath} | File size: {Size} bytes", resolvedPath, fileInfo.Length);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "File exists but couldn't get file info: {ResolvedPath}", resolvedPath);
                }
            }

            return exists;
        }

        /// <summary>
        /// Checks if a link is an inline resource (PlantUML, CDN, image, etc).
        /// </summary>
        public static bool IsInlineResource(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return url.Contains("plantuml.com") ||
                   url.Contains("jsdelivr.net") ||
                   url.Contains("cdnjs.cloudflare.com") ||
                   url.Contains("githubusercontent.com") ||
                   url.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".svg", StringComparison.OrdinalIgnoreCase) ||
                   url.EndsWith(".webp", StringComparison.OrdinalIgnoreCase);
        }
    }
}
