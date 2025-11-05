using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using MarkdownViewer.Models;
using Serilog;

namespace MarkdownViewer.Services
{
    /// <summary>
    /// Interface for theme management operations
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the currently loaded theme
        /// </summary>
        Theme GetCurrentTheme();

        /// <summary>
        /// Loads a theme by name from the Themes folder
        /// </summary>
        Theme LoadTheme(string themeName);

        /// <summary>
        /// Gets list of available theme names
        /// </summary>
        List<string> GetAvailableThemes();

        /// <summary>
        /// Applies a theme to the WinForms UI and WebView2 content
        /// </summary>
        Task ApplyThemeAsync(Theme theme, Form form, WebView2 webView);
    }

    /// <summary>
    /// Theme service implementation.
    /// Loads themes from JSON files in Themes/ folder and applies them to UI.
    /// </summary>
    public class ThemeService : IThemeService
    {
        private Theme _currentTheme;
        private readonly string _themesPath;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ThemeService()
        {
            // Themes folder next to executable
            var exePath = Application.ExecutablePath;
            var exeDir = Path.GetDirectoryName(exePath);
            _themesPath = Path.Combine(exeDir, "Themes");

            Log.Information("ThemeService initialized. Themes path: {Path}", _themesPath);

            // Load default theme
            _currentTheme = LoadTheme("standard");
        }

        /// <summary>
        /// Constructor for testing with custom path
        /// </summary>
        internal ThemeService(string customThemesPath)
        {
            _themesPath = customThemesPath;
            _currentTheme = new Theme { Name = "standard", DisplayName = "Standard" };
            Log.Information("ThemeService initialized with custom path: {Path}", _themesPath);
        }

        public Theme GetCurrentTheme()
        {
            return _currentTheme;
        }

        public Theme LoadTheme(string themeName)
        {
            try
            {
                var themeFile = Path.Combine(_themesPath, $"{themeName}.json");

                if (!File.Exists(themeFile))
                {
                    Log.Warning("Theme file not found: {File}, falling back to standard", themeFile);

                    // If standard theme doesn't exist, return default theme object
                    if (themeName == "standard")
                    {
                        Log.Warning("Standard theme not found, using built-in defaults");
                        return CreateDefaultTheme();
                    }

                    return LoadTheme("standard");
                }

                var json = File.ReadAllText(themeFile);
                var theme = JsonSerializer.Deserialize<Theme>(json, _jsonOptions);

                if (theme == null)
                {
                    Log.Warning("Failed to deserialize theme: {File}", themeFile);
                    return LoadTheme("standard");
                }

                _currentTheme = theme;
                Log.Information("Theme loaded: {Name} ({DisplayName})", theme.Name, theme.DisplayName);
                return theme;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to load theme: {ThemeName}", themeName);
                return CreateDefaultTheme();
            }
        }

        public List<string> GetAvailableThemes()
        {
            try
            {
                if (!Directory.Exists(_themesPath))
                {
                    Log.Warning("Themes directory not found: {Path}", _themesPath);
                    return new List<string> { "standard" };
                }

                var themeFiles = Directory.GetFiles(_themesPath, "*.json");
                var themeNames = themeFiles
                    .Select(f => Path.GetFileNameWithoutExtension(f))
                    .OrderBy(n => n)
                    .ToList();

                Log.Information("Found {Count} themes", themeNames.Count);
                return themeNames;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to get available themes");
                return new List<string> { "standard" };
            }
        }

        public async Task ApplyThemeAsync(Theme theme, Form form, WebView2 webView)
        {
            try
            {
                Log.Information("Applying theme: {Name}", theme.Name);

                // Apply to WinForms UI
                ApplyThemeToWinForms(theme, form);

                // Apply to WebView2 content (CSS injection)
                if (webView != null && webView.CoreWebView2 != null)
                {
                    await ApplyThemeToWebView2Async(theme, webView);
                }

                _currentTheme = theme;
                Log.Information("Theme applied successfully: {Name}", theme.Name);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply theme: {Name}", theme.Name);
                throw;
            }
        }

        private void ApplyThemeToWinForms(Theme theme, Form form)
        {
            try
            {
                // Apply to main form
                form.BackColor = ColorTranslator.FromHtml(theme.UI.FormBackground);

                // Apply to all controls recursively
                ApplyThemeToControl(form, theme);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply theme to WinForms");
            }
        }

        private void ApplyThemeToControl(Control control, Theme theme)
        {
            // StatusStrip
            if (control is StatusStrip statusBar)
            {
                statusBar.BackColor = ColorTranslator.FromHtml(theme.UI.StatusBarBackground);
                statusBar.ForeColor = ColorTranslator.FromHtml(theme.UI.StatusBarForeground);
            }
            // ToolStrip (navigation bar)
            else if (control is ToolStrip toolStrip)
            {
                toolStrip.BackColor = ColorTranslator.FromHtml(theme.UI.ControlBackground);
                toolStrip.ForeColor = ColorTranslator.FromHtml(theme.UI.ControlForeground);
            }
            // MenuStrip
            else if (control is MenuStrip menuStrip)
            {
                menuStrip.BackColor = ColorTranslator.FromHtml(theme.UI.MenuBackground);
                menuStrip.ForeColor = ColorTranslator.FromHtml(theme.UI.MenuForeground);
            }
            // ContextMenuStrip
            else if (control is ContextMenuStrip contextMenu)
            {
                contextMenu.BackColor = ColorTranslator.FromHtml(theme.UI.MenuBackground);
                contextMenu.ForeColor = ColorTranslator.FromHtml(theme.UI.MenuForeground);
            }
            // Panels (search bar, etc.)
            else if (control is Panel panel)
            {
                panel.BackColor = ColorTranslator.FromHtml(theme.UI.ControlBackground);
            }
            // Regular controls
            else if (control is TextBox || control is Button || control is Label)
            {
                if (!(control is WebView2)) // Don't apply to WebView2
                {
                    control.BackColor = ColorTranslator.FromHtml(theme.UI.ControlBackground);
                    control.ForeColor = ColorTranslator.FromHtml(theme.UI.ControlForeground);
                }
            }

            // Recursively apply to child controls
            foreach (Control child in control.Controls)
            {
                ApplyThemeToControl(child, theme);
            }
        }

        private async Task ApplyThemeToWebView2Async(Theme theme, WebView2 webView)
        {
            try
            {
                var css = GenerateThemeCSS(theme.Markdown);
                var script = $@"
                    (function() {{
                        // Remove existing theme style
                        var existingStyle = document.getElementById('theme-style');
                        if (existingStyle) {{
                            existingStyle.remove();
                        }}

                        // Add new theme style
                        var style = document.createElement('style');
                        style.id = 'theme-style';
                        style.innerHTML = `{EscapeForJavaScript(css)}`;
                        document.head.appendChild(style);
                    }})();
                ";

                await webView.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to apply theme to WebView2");
            }
        }

        private string GenerateThemeCSS(MarkdownColors colors)
        {
            return $@"
                body {{
                    background: {colors.Background} !important;
                    color: {colors.Foreground} !important;
                }}

                code {{
                    background: {colors.InlineCodeBackground} !important;
                    color: {colors.InlineCodeForeground} !important;
                    padding: 2px 4px;
                    border-radius: 3px;
                }}

                pre {{
                    background: {colors.CodeBackground} !important;
                    border-radius: 5px;
                    padding: 15px;
                }}

                pre code {{
                    background: transparent !important;
                    color: inherit !important;
                }}

                a {{
                    color: {colors.LinkColor} !important;
                }}

                h1, h2, h3, h4, h5, h6 {{
                    color: {colors.HeadingColor} !important;
                }}

                blockquote {{
                    border-left: 4px solid {colors.BlockquoteBorder} !important;
                    padding-left: 15px;
                    color: {colors.Foreground} !important;
                    opacity: 0.8;
                }}

                table {{
                    border-collapse: collapse;
                }}

                table th {{
                    background: {colors.TableHeaderBackground} !important;
                    border: 1px solid {colors.TableBorder} !important;
                    padding: 8px;
                }}

                table td {{
                    border: 1px solid {colors.TableBorder} !important;
                    padding: 8px;
                }}
            ";
        }

        private string EscapeForJavaScript(string text)
        {
            return text
                .Replace("\\", "\\\\")
                .Replace("`", "\\`")
                .Replace("$", "\\$")
                .Replace("\r\n", " ")
                .Replace("\n", " ")
                .Replace("\r", " ");
        }

        private Theme CreateDefaultTheme()
        {
            return new Theme
            {
                Name = "standard",
                DisplayName = "Standard (Enhanced)",
                Markdown = new MarkdownColors
                {
                    Background = "#ffffff",
                    Foreground = "#2c3e50",
                    CodeBackground = "#f8f9fa",
                    LinkColor = "#3498db",
                    HeadingColor = "#1a252f",
                    BlockquoteBorder = "#3498db",
                    TableHeaderBackground = "#ecf0f1",
                    TableBorder = "#bdc3c7",
                    InlineCodeBackground = "#f8f9fa",
                    InlineCodeForeground = "#e74c3c"
                },
                UI = new UiColors
                {
                    FormBackground = "#ecf0f1",
                    ControlBackground = "#ffffff",
                    ControlForeground = "#2c3e50",
                    StatusBarBackground = "#3498db",
                    StatusBarForeground = "#ffffff",
                    MenuBackground = "#ffffff",
                    MenuForeground = "#2c3e50"
                }
            };
        }
    }
}
