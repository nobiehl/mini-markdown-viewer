using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MarkdownViewer.Core;
using MarkdownViewer.Core.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownViewer.Avalonia;

public partial class MainWindow : Window
{
    private readonly MarkdownRenderer _renderer;
    private Theme? _currentTheme;

    public MainWindow()
    {
        InitializeComponent();

        _renderer = new MarkdownRenderer();

        // Load default theme
        _currentTheme = LoadDefaultTheme();
    }

    private async void OnOpenFile(object sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Markdown File",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Markdown Files")
                {
                    Patterns = new[] { "*.md", "*.markdown" }
                }
            }
        });

        if (files.Count > 0)
        {
            var file = files[0];
            await LoadMarkdownFile(file.Path.LocalPath);
        }
    }

    private async Task LoadMarkdownFile(string filePath)
    {
        try
        {
            string markdown = await File.ReadAllTextAsync(filePath);
            string html = _renderer.RenderToHtml(markdown, filePath, _currentTheme);

            // OPTION 1: Wenn WebView verfügbar
            // MarkdownWebView.NavigateToString(html);

            // OPTION 2: Fallback - zeige HTML als Text
            HtmlPreview.Text = html;

            Title = $"{Path.GetFileName(filePath)} - Markdown Viewer (Avalonia PoC)";
        }
        catch (Exception ex)
        {
            // TODO: Error dialog
            HtmlPreview.Text = $"Error: {ex.Message}";
        }
    }

    private void OnExit(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnThemeStandard(object sender, RoutedEventArgs e)
    {
        _currentTheme = LoadTheme("standard");
    }

    private void OnThemeDark(object sender, RoutedEventArgs e)
    {
        _currentTheme = LoadTheme("dark");
    }

    private Theme LoadDefaultTheme()
    {
        // Minimal theme for PoC
        return new Theme
        {
            Name = "standard",
            DisplayName = "Standard",
            UI = new UiColors
            {
                FormBackground = "#FFFFFF",
                ControlForeground = "#000000"
            },
            Markdown = new MarkdownColors
            {
                Background = "#FFFFFF",
                Foreground = "#000000",
                CodeBackground = "#F5F5F5"
            }
        };
    }

    private Theme LoadTheme(string name)
    {
        // For dark theme
        if (name == "dark")
        {
            return new Theme
            {
                Name = "dark",
                DisplayName = "Dark",
                UI = new UiColors
                {
                    FormBackground = "#1E1E1E",
                    ControlForeground = "#FFFFFF"
                },
                Markdown = new MarkdownColors
                {
                    Background = "#1E1E1E",
                    Foreground = "#D4D4D4",
                    CodeBackground = "#2D2D30",
                    LinkColor = "#569CD6",
                    HeadingColor = "#DCDCDC"
                }
            };
        }

        return LoadDefaultTheme();
    }
}