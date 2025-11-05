using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Markdig;
using Serilog;
using Serilog.Events;

namespace MarkdownViewer
{
    /// <summary>
    /// Main window form for displaying Markdown files.
    /// Uses WebView2 for HTML rendering with embedded CSS, JavaScript for syntax highlighting,
    /// Mermaid diagrams, and PlantUML diagrams.
    /// Implements live-reload functionality via FileSystemWatcher.
    /// Includes comprehensive logging via Serilog.
    /// </summary>
    public class MainForm : Form
    {
        private const string Version = "1.0.4";
        private WebView2 webView;
        private string currentFilePath;
        private FileSystemWatcher fileWatcher;

        /// <summary>
        /// Initializes the main form with a specified Markdown file.
        /// Sets up WebView2, loads the Markdown file, and starts file watching for live-reload.
        /// </summary>
        /// <param name="filePath">Full path to the .md file to display</param>
        /// <param name="logLevel">Minimum log level (default: Information)</param>
        public MainForm(string filePath, LogEventLevel logLevel = LogEventLevel.Information)
        {
            InitializeLogging(logLevel);
            Log.Information("=== MarkdownViewer v{Version} Starting (LogLevel: {LogLevel}) ===", Version, logLevel);
            Log.Information("Opening file: {FilePath}", filePath);

            InitializeComponents();
            currentFilePath = filePath;
            LoadMarkdownFile(filePath);
            SetupFileWatcher(filePath);
        }

        /// <summary>
        /// Initializes Serilog logging with rolling file output.
        /// Logs are stored in ./logs/ directory with daily rotation.
        /// </summary>
        /// <param name="logLevel">Minimum log level to record</param>
        private void InitializeLogging(LogEventLevel logLevel)
        {
            try
            {
                // Create logs directory next to executable
                string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
                string logsFolder = Path.Combine(exeFolder, "logs");

                if (!Directory.Exists(logsFolder))
                {
                    Directory.CreateDirectory(logsFolder);
                }

                // Configure Serilog with rolling daily logs
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Is(logLevel)
                    .WriteTo.File(
                        path: Path.Combine(logsFolder, "viewer-.log"),
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,  // Keep last 7 days
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                    .CreateLogger();
            }
            catch (Exception ex)
            {
                // If logging setup fails, show message but continue
                MessageBox.Show($"Warning: Could not initialize logging:\n{ex.Message}",
                    "Logging Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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
                    Log.Information("WebView2 initialized successfully");

                    webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = true;
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = false;

                    // Capture JavaScript console messages (console.log, console.error, etc.)
                    webView.CoreWebView2.WebMessageReceived += (sender, args) =>
                    {
                        string message = args.TryGetWebMessageAsString();
                        Log.Debug("WebMessage received: {Message}", message);
                        HandleLinkClick(message);
                    };

                    // Log JavaScript console output
                    webView.CoreWebView2.WebResourceResponseReceived += (sender, args) =>
                    {
                        Log.Debug("WebResource loaded: {Uri} (Status: {StatusCode})",
                            args.Request.Uri, args.Response.StatusCode);
                    };

                    // Capture browser console messages
                    webView.CoreWebView2.ProcessFailed += (sender, args) =>
                    {
                        Log.Error("WebView2 process failed: {Reason}", args.Reason);
                        MessageBox.Show($"WebView2 process failed: {args.Reason}\nCheck logs for details.",
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    };

                    // Handle navigation - .md files open in viewer, external links in browser
                    // Note: Most navigation is now handled via JavaScript interceptor + WebMessage
                    // This handler is kept for edge cases and external resources
                    webView.CoreWebView2.NavigationStarting += (sender, args) =>
                    {
                        Log.Debug("NavigationStarting event: {Uri}", args.Uri);

                        // Skip null, empty, or initial navigation
                        if (string.IsNullOrEmpty(args.Uri) || args.Uri == "about:blank")
                            return;

                        // Skip data URIs (base64 images)
                        if (args.Uri.StartsWith("data:"))
                            return;

                        // Handle HTTP/HTTPS links
                        if (args.Uri.StartsWith("http://") || args.Uri.StartsWith("https://"))
                        {
                            // Check if it's an inline resource that should load
                            if (IsInlineResource(args.Uri))
                            {
                                Log.Debug("Allowing inline resource: {Uri}", args.Uri);
                                return; // Allow CDN resources, images, PlantUML to load
                            }

                            // External links open in browser
                            Log.Information("Opening external link in browser: {Uri}", args.Uri);
                            args.Cancel = true;
                            try
                            {
                                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                                {
                                    FileName = args.Uri,
                                    UseShellExecute = true
                                });
                            }
                            catch (Exception ex)
                            {
                                Log.Error(ex, "Failed to open link: {Uri}", args.Uri);
                                MessageBox.Show($"Failed to open link: {ex.Message}\n\nCheck logs for details.", "Error",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                            return;
                        }

                        // Handle local file:// links
                        if (args.Uri.StartsWith("file://"))
                        {
                            Log.Debug("Handling file:// link: {Uri}", args.Uri);
                            Uri uri = new Uri(args.Uri);
                            string localPath = Uri.UnescapeDataString(uri.LocalPath);
                            string fragment = uri.Fragment; // #anchor

                            // Check if it's a markdown file
                            if (localPath.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                                localPath.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
                            {
                                args.Cancel = true;

                                // Check if it's the same file (anchor-only navigation)
                                string currentFullPath = Path.GetFullPath(currentFilePath);
                                string linkedFullPath = Path.GetFullPath(localPath);

                                if (currentFullPath.Equals(linkedFullPath, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Same file, just anchor navigation
                                    if (!string.IsNullOrEmpty(fragment))
                                    {
                                        Log.Information("Anchor navigation: {Anchor}", fragment);
                                        // Execute JavaScript to scroll to anchor (after event completes)
                                        string anchorId = fragment.TrimStart('#');
                                        this.BeginInvoke(new Action(() =>
                                        {
                                            webView.ExecuteScriptAsync(
                                                $"document.getElementById('{anchorId}')?.scrollIntoView({{behavior: 'smooth'}}) || " +
                                                $"document.querySelector('a[name=\"{anchorId}\"]')?.scrollIntoView({{behavior: 'smooth'}});");
                                        }));
                                    }
                                    return;
                                }

                                // Different file - navigate
                                if (File.Exists(localPath))
                                {
                                    Log.Information("Navigating to file (via NavigationStarting): {FilePath}", localPath);
                                    // IMPORTANT: Must navigate AFTER the NavigationStarting event completes
                                    // Otherwise WebView2 can't start new navigation while canceling old one
                                    string pathToLoad = localPath;
                                    string anchorToScroll = fragment;

                                    this.BeginInvoke(new Action(() =>
                                    {
                                        currentFilePath = pathToLoad;
                                        LoadMarkdownFile(pathToLoad);
                                        SetupFileWatcher(pathToLoad);

                                        // If there's an anchor, scroll after load
                                        if (!string.IsNullOrEmpty(anchorToScroll))
                                        {
                                            Log.Debug("Will scroll to anchor after load: {Anchor}", anchorToScroll);
                                            string anchorId = anchorToScroll.TrimStart('#');
                                            System.Threading.Tasks.Task.Delay(200).ContinueWith(_ =>
                                            {
                                                this.Invoke(new Action(() =>
                                                {
                                                    webView.ExecuteScriptAsync(
                                                        $"document.getElementById('{anchorId}')?.scrollIntoView({{behavior: 'smooth'}});");
                                                }));
                                            });
                                        }
                                    }));
                                }
                                else
                                {
                                    Log.Warning("File not found (via NavigationStarting): {FilePath}", localPath);
                                    MessageBox.Show($"File not found:\n{localPath}", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                }
                            }
                            // Other local files (images, etc.) - allow loading
                        }
                    };
                }
                else
                {
                    Log.Error("WebView2 initialization failed: {ErrorCode}", e.InitializationException?.Message ?? "Unknown error");
                    MessageBox.Show($"Failed to initialize WebView2:\n{e.InitializationException?.Message ?? "Unknown error"}\n\nCheck logs for details.",
                        "Initialization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
        }

        /// <summary>
        /// Determines if a URL is an inline resource (CDN, images, etc.) that should load in the viewer.
        /// </summary>
        private bool IsInlineResource(string url)
        {
            // CDN and external resources that should load inline
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

        /// <summary>
        /// Handles link clicks from JavaScript via WebMessage
        /// </summary>
        private void HandleLinkClick(string url)
        {
            Log.Debug("HandleLinkClick: {Url}", url);

            try
            {
                // Handle HTTP/HTTPS links - open in browser
                if (url.StartsWith("http://") || url.StartsWith("https://"))
                {
                    Log.Information("Opening external URL in browser: {Url}", url);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                    return;
                }

                // Handle relative paths - convert to absolute
                string absolutePath;
                if (Path.IsPathRooted(url))
                {
                    absolutePath = url;
                    Log.Debug("Link is absolute path: {Path}", absolutePath);
                }
                else
                {
                    // Resolve relative to current file's directory
                    string currentDir = Path.GetDirectoryName(currentFilePath) ?? ".";
                    absolutePath = Path.GetFullPath(Path.Combine(currentDir, url));
                    Log.Debug("Resolved relative link: {RelativeUrl} -> {AbsolutePath}", url, absolutePath);
                }

                // Check if it's a markdown file
                if (absolutePath.EndsWith(".md", StringComparison.OrdinalIgnoreCase) ||
                    absolutePath.EndsWith(".markdown", StringComparison.OrdinalIgnoreCase))
                {
                    if (File.Exists(absolutePath))
                    {
                        Log.Information("Navigating to markdown file: {FilePath}", absolutePath);
                        currentFilePath = absolutePath;
                        LoadMarkdownFile(absolutePath);
                        SetupFileWatcher(absolutePath);
                    }
                    else
                    {
                        Log.Warning("Markdown file not found: {FilePath}", absolutePath);
                        MessageBox.Show($"File not found:\n{absolutePath}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    // Other files - try to open with default program
                    if (File.Exists(absolutePath))
                    {
                        Log.Information("Opening file with default program: {FilePath}", absolutePath);
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = absolutePath,
                            UseShellExecute = true
                        });
                    }
                    else
                    {
                        Log.Warning("File not found: {FilePath}", absolutePath);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error handling link: {Url}", url);
                MessageBox.Show($"Error handling link:\n{ex.Message}\n\nCheck logs for details.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads and displays a Markdown file.
        /// Reads the file, converts to HTML, updates window title, and renders in WebView2.
        /// Handles WebView2 initialization states (already ready vs. waiting for ready).
        /// </summary>
        /// <param name="filePath">Full path to the .md file to load</param>
        private void LoadMarkdownFile(string filePath)
        {
            Log.Debug("LoadMarkdownFile: {FilePath}", filePath);

            try
            {
                string markdown = File.ReadAllText(filePath);
                Log.Debug("Read {Bytes} bytes from {FilePath}", markdown.Length, filePath);

                string html = ConvertMarkdownToHtml(markdown);
                Log.Debug("Converted markdown to HTML ({HtmlLength} characters)", html.Length);

                // Update window title with filename and version
                this.Text = $"{Path.GetFileName(filePath)} - Markdown Viewer v{Version}";

                if (webView.CoreWebView2 != null)
                {
                    Log.Debug("Navigating WebView2 to HTML content");
                    webView.CoreWebView2.NavigateToString(html);
                }
                else
                {
                    Log.Debug("WebView2 not ready, queueing navigation");
                    // Wait for WebView2 to be ready
                    webView.CoreWebView2InitializationCompleted += (s, e) =>
                    {
                        if (e.IsSuccess)
                        {
                            Log.Debug("WebView2 ready, navigating to queued HTML");
                            webView.CoreWebView2.NavigateToString(html);
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error loading file: {FilePath}", filePath);
                MessageBox.Show($"Error loading file: {ex.Message}\n\nCheck logs for details.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Generate base URL for relative link resolution
            string currentDir = Path.GetDirectoryName(currentFilePath) ?? ".";
            string baseUrl = $"file:///{currentDir.Replace("\\", "/")}/";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <base href='{baseUrl}'>
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

        // Intercept link clicks and send to C# for handling
        document.addEventListener('click', function(e) {{
            // Find the clicked link (could be nested in other elements)
            let target = e.target;
            while (target && target.tagName !== 'A') {{
                target = target.parentElement;
            }}

            if (target && target.tagName === 'A') {{
                const href = target.getAttribute('href');

                // Ignore anchor-only links (handled by browser)
                if (!href || href.startsWith('#')) {{
                    return; // Let browser handle it
                }}

                // Ignore data: URIs
                if (href.startsWith('data:')) {{
                    return;
                }}

                // Prevent default navigation
                e.preventDefault();

                // Send to C# for handling
                console.log('Intercepted link click:', href);
                window.chrome.webview.postMessage(href);
            }}
        }}, true);
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
            Log.Debug("SetupFileWatcher: {FilePath}", filePath);

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                fileWatcher?.Dispose(); // Dispose old watcher if exists

                fileWatcher = new FileSystemWatcher(directory)
                {
                    Filter = fileName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                };

                fileWatcher.Changed += (s, e) =>
                {
                    Log.Information("File changed, reloading: {FilePath}", e.FullPath);
                    // Reload on file change (with small delay to avoid multiple triggers)
                    System.Threading.Thread.Sleep(100);
                    this.Invoke(new Action(() => LoadMarkdownFile(currentFilePath)));
                };

                fileWatcher.EnableRaisingEvents = true;
                Log.Debug("FileWatcher enabled for: {Directory}/{FileName}", directory, fileName);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to setup file watcher for: {FilePath}", filePath);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Log.Information("MarkdownViewer disposing");
                fileWatcher?.Dispose();
                webView?.Dispose();
                Log.CloseAndFlush(); // Ensure all logs are written
            }
            base.Dispose(disposing);
        }
    }
}
