using System.Collections.Generic;
using MarkdownViewer.Core.Models;

namespace MarkdownViewer.Core.Services
{
    /// <summary>
    /// Interface for theme management operations.
    /// Platform-independent interface for loading and managing themes.
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the currently loaded theme
        /// </summary>
        Theme GetCurrentTheme();

        /// <summary>
        /// Loads a theme by name from embedded resources
        /// </summary>
        Theme LoadTheme(string themeName);

        /// <summary>
        /// Gets list of available theme names
        /// </summary>
        List<string> GetAvailableThemes();
    }
}
