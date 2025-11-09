namespace MarkdownViewer.Core.Models
{
    /// <summary>
    /// Theme definition for both Markdown rendering and WinForms UI.
    /// Loaded from JSON files in the Themes/ folder.
    /// </summary>
    public class Theme
    {
        /// <summary>
        /// Internal theme identifier (e.g., "dark", "solarized")
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Display name shown in UI (e.g., "Dark", "Solarized Light")
        /// </summary>
        public string DisplayName { get; set; } = "";

        /// <summary>
        /// Color scheme for Markdown content rendering
        /// </summary>
        public MarkdownColors Markdown { get; set; } = new();

        /// <summary>
        /// Color scheme for WinForms UI elements
        /// </summary>
        public UiColors UI { get; set; } = new();
    }

    /// <summary>
    /// Color definitions for Markdown content rendering (WebView2 CSS).
    /// All colors must be valid CSS color values (hex, rgb, named colors).
    /// </summary>
    public class MarkdownColors
    {
        /// <summary>
        /// Page background color
        /// </summary>
        public string Background { get; set; } = "#ffffff";

        /// <summary>
        /// Main text color
        /// </summary>
        public string Foreground { get; set; } = "#333333";

        /// <summary>
        /// Code block background color
        /// </summary>
        public string CodeBackground { get; set; } = "#f5f5f5";

        /// <summary>
        /// Hyperlink color
        /// </summary>
        public string LinkColor { get; set; } = "#4A90E2";

        /// <summary>
        /// Heading (h1-h6) text color
        /// </summary>
        public string HeadingColor { get; set; } = "#000000";

        /// <summary>
        /// Blockquote left border color
        /// </summary>
        public string BlockquoteBorder { get; set; } = "#4A90E2";

        /// <summary>
        /// Table header background color
        /// </summary>
        public string TableHeaderBackground { get; set; } = "#f5f5f5";

        /// <summary>
        /// Table border color
        /// </summary>
        public string TableBorder { get; set; } = "#dddddd";

        /// <summary>
        /// Inline code background color
        /// </summary>
        public string InlineCodeBackground { get; set; } = "#f5f5f5";

        /// <summary>
        /// Inline code text color
        /// </summary>
        public string InlineCodeForeground { get; set; } = "#c7254e";
    }

    /// <summary>
    /// Color definitions for WinForms UI elements.
    /// All colors must be valid CSS color values for parsing with ColorTranslator.
    /// </summary>
    public class UiColors
    {
        /// <summary>
        /// Main form background color
        /// </summary>
        public string FormBackground { get; set; } = "#f0f0f0";

        /// <summary>
        /// Control (button, textbox) background color
        /// </summary>
        public string ControlBackground { get; set; } = "#ffffff";

        /// <summary>
        /// Control text color
        /// </summary>
        public string ControlForeground { get; set; } = "#000000";

        /// <summary>
        /// StatusBar background color
        /// </summary>
        public string StatusBarBackground { get; set; } = "#e0e0e0";

        /// <summary>
        /// StatusBar text/icon color
        /// </summary>
        public string StatusBarForeground { get; set; } = "#000000";

        /// <summary>
        /// Menu and context menu background color
        /// </summary>
        public string MenuBackground { get; set; } = "#ffffff";

        /// <summary>
        /// Menu text color
        /// </summary>
        public string MenuForeground { get; set; } = "#000000";
    }
}
