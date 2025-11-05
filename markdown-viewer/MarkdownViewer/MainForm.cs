using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Markdig;

namespace MarkdownViewer
{
    /// <summary>
    /// Main window form for displaying Markdown files.
    /// Uses WebView2 for HTML rendering with embedded CSS, JavaScript for syntax highlighting,
    /// Mermaid diagrams, and PlantUML diagrams.
    /// Implements live-reload functionality via FileSystemWatcher.
    /// </summary>
    public class MainForm : Form
    {
        private const string Version = "1.0.3";
        private WebView2 webView;
        private string currentFilePath;
        private FileSystemWatcher fileWatcher;

        /// <summary>
        /// Initializes the main form with a specified Markdown file.
        /// Sets up WebView2, loads the Markdown file, and starts file watching for live-reload.
        /// </summary>
        /// <param name="filePath">Full path to the .md file to display</param>
        public MainForm(string filePath)
        {
            InitializeComponents();
            currentFilePath = filePath;
            LoadMarkdownFile(filePath);
            SetupFileWatcher(filePath);
        }

        /// <summary>
        /// Initializes all UI components and WebView2.
        /// Sets up window properties, WebView2 control, custom cache folder,
        /// and configures navigation behavior (external links open in browser).
        /// </summary>
        private void InitializeComponents()
        {
            // Window setup
            this.Text = $"Markdown Viewer v{Version}";
            this.Size = new System.Drawing.Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Icon is embedded in EXE via ApplicationIcon property

            // WebView2 setup with custom cache folder
            webView = new WebView2
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(webView);

            // Set WebView2 data folder to .cache next to EXE
            string exeFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
            string userDataFolder = System.IO.Path.Combine(exeFolder, ".cache");

            var env = Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                null,
                userDataFolder
            ).Result;

            webView.EnsureCoreWebView2Async(env);

            // Wait for WebView2 to be ready
            webView.CoreWebView2InitializationCompleted += (s, e) =>
            {
                if (e.IsSuccess)
                {
                    webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = false;

                    // Handle navigation - open external links in browser
                    webView.CoreWebView2.NavigationStarting += (sender, args) =>
                    {
                        if (args.Uri != null && args.Uri.StartsWith("http"))
                        {
                            args.Cancel = true;
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = args.Uri,
                                UseShellExecute = true
                            });
                        }
                    };
                }
            };

            webView.EnsureCoreWebView2Async(null);
        }

        /// <summary>
        /// Loads and displays a Markdown file.
        /// Reads the file, converts to HTML, updates window title, and renders in WebView2.
        /// Handles WebView2 initialization states (already ready vs. waiting for ready).
        /// </summary>
        /// <param name="filePath">Full path to the .md file to load</param>
        private void LoadMarkdownFile(string filePath)
        {
            try
            {
                string markdown = File.ReadAllText(filePath);
                string html = ConvertMarkdownToHtml(markdown);

                // Update window title with filename and version
                this.Text = $"{Path.GetFileName(filePath)} - Markdown Viewer v{Version}";

                if (webView.CoreWebView2 != null)
                {
                    webView.CoreWebView2.NavigateToString(html);
                }
                else
                {
                    // Wait for WebView2 to be ready
                    webView.CoreWebView2InitializationCompleted += (s, e) =>
                    {
                        if (e.IsSuccess)
                        {
                            webView.CoreWebView2.NavigateToString(html);
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Converts Markdown text to a complete HTML document.
        /// Uses Markdig with advanced extensions for rich Markdown support.
        /// Generates HTML with embedded:
        /// - CSS for styling (GitHub-like theme)
        /// - Highlight.js for syntax highlighting
        /// - Mermaid.js for Mermaid diagrams (client-side rendering)
        /// - PlantUML rendering via plantuml.com server
        /// - Copy buttons for code blocks
        /// </summary>
        /// <param name="markdown">Raw Markdown text</param>
        /// <returns>Complete HTML document as string</returns>
        private string ConvertMarkdownToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .Build();

            string content = Markdown.ToHtml(markdown, pipeline);

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github.min.css'>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
            max-width: 900px;
            margin: 0 auto;
            padding: 2rem;
            color: #333;
            background: #fff;
        }}
        h1, h2, h3, h4, h5, h6 {{
            margin-top: 1.5rem;
            margin-bottom: 1rem;
            font-weight: 600;
        }}
        h1 {{ font-size: 2rem; border-bottom: 2px solid #eee; padding-bottom: 0.5rem; }}
        h2 {{ font-size: 1.5rem; border-bottom: 1px solid #eee; padding-bottom: 0.3rem; }}
        h3 {{ font-size: 1.25rem; }}
        code {{
            background: #f5f5f5;
            padding: 0.2rem 0.4rem;
            border-radius: 3px;
            font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
            font-size: 0.9em;
        }}
        pre {{
            background: #f5f5f5;
            padding: 1rem;
            border-radius: 6px;
            overflow-x: auto;
            position: relative;
        }}
        pre code {{
            background: none;
            padding: 0;
        }}
        /* Copy button for code blocks */
        .copy-btn {{
            position: absolute;
            top: 0.5rem;
            right: 0.5rem;
            padding: 0.25rem 0.75rem;
            background: #4A90E2;
            color: white;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 0.85rem;
            opacity: 0;
            transition: opacity 0.2s;
        }}
        pre:hover .copy-btn {{
            opacity: 1;
        }}
        .copy-btn:hover {{
            background: #357ABD;
        }}
        blockquote {{
            border-left: 4px solid #4A90E2;
            padding-left: 1rem;
            margin: 1rem 0;
            color: #666;
        }}
        a {{
            color: #4A90E2;
            text-decoration: none;
        }}
        a:hover {{
            text-decoration: underline;
        }}
        img {{
            max-width: 100%;
            height: auto;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 1rem 0;
        }}
        table th, table td {{
            border: 1px solid #ddd;
            padding: 0.5rem;
        }}
        table th {{
            background: #f5f5f5;
            font-weight: 600;
        }}
        ul, ol {{
            padding-left: 2rem;
            margin-bottom: 1rem;
        }}
        /* Mermaid diagram styling */
        .mermaid {{
            margin: 1rem 0;
            text-align: center;
        }}
    </style>
</head>
<body>
    {content}
    <script src='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js'></script>
    <script type='module'>
        import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
        mermaid.initialize({{ startOnLoad: true, theme: 'default' }});
    </script>
    <script>
        // Syntax highlighting
        hljs.highlightAll();

        // Process PlantUML diagrams using HEX encoding (~h method)
        document.querySelectorAll('code.language-plantuml').forEach((block) => {{
            const pre = block.parentElement;
            const code = block.textContent;

            // Create image element
            const img = document.createElement('img');

            // PlantUML HEX encoding: Convert each character to hex
            const hex = Array.from(code)
                .map(c => c.charCodeAt(0).toString(16).padStart(2, '0'))
                .join('');

            // Use ~h prefix for HEX encoding (simpler and more reliable than deflate)
            img.src = `https://www.plantuml.com/plantuml/png/~h${{hex}}`;
            img.style.maxWidth = '100%';
            img.style.margin = '1rem 0';
            img.style.border = '1px solid #ddd';
            img.style.borderRadius = '4px';
            img.style.padding = '10px';
            img.style.backgroundColor = '#fff';

            // Add error handling
            img.onerror = function() {{
                const errorDiv = document.createElement('div');
                errorDiv.style.border = '1px solid #f00';
                errorDiv.style.padding = '10px';
                errorDiv.style.backgroundColor = '#fee';
                errorDiv.style.borderRadius = '4px';
                errorDiv.innerHTML = '<strong>PlantUML Error:</strong> Could not render diagram. Check your PlantUML syntax or internet connection.';
                pre.replaceWith(errorDiv);
            }};

            // Replace code block with image
            pre.replaceWith(img);
        }});

        // Add copy buttons to code blocks (except mermaid and plantuml)
        document.querySelectorAll('pre code').forEach((block) => {{
            if (block.className.includes('language-mermaid') || block.className.includes('language-plantuml')) {{
                return; // Skip diagram blocks
            }}

            const pre = block.parentElement;
            const button = document.createElement('button');
            button.className = 'copy-btn';
            button.textContent = 'Copy';
            button.onclick = () => {{
                navigator.clipboard.writeText(block.textContent).then(() => {{
                    button.textContent = 'Copied!';
                    setTimeout(() => {{ button.textContent = 'Copy'; }}, 2000);
                }});
            }};
            pre.appendChild(button);
        }});
    </script>
</body>
</html>";
        }

        /// <summary>
        /// Sets up a FileSystemWatcher for live-reload functionality.
        /// Watches for changes to the currently displayed Markdown file
        /// and automatically reloads it when modified.
        /// Includes a small delay to avoid multiple triggers.
        /// </summary>
        /// <param name="filePath">Full path to the .md file to watch</param>
        private void SetupFileWatcher(string filePath)
        {
            try
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                fileWatcher = new FileSystemWatcher(directory)
                {
                    Filter = fileName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                };

                fileWatcher.Changed += (s, e) =>
                {
                    // Reload on file change (with small delay to avoid multiple triggers)
                    System.Threading.Thread.Sleep(100);
                    this.Invoke(new Action(() => LoadMarkdownFile(currentFilePath)));
                };

                fileWatcher.EnableRaisingEvents = true;
            }
            catch
            {
                // Ignore file watcher errors
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                fileWatcher?.Dispose();
                webView?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
