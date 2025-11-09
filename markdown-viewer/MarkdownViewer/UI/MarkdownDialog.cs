using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using MarkdownViewer.Core;
using Serilog;

namespace MarkdownViewer.UI
{
    /// <summary>
    /// Modal dialog for displaying Markdown content using WebView2.
    /// Used for displaying release notes and other markdown-formatted information.
    /// </summary>
    public class MarkdownDialog : Form
    {
        private WebView2? _webView;
        private Button? _okButton;
        private readonly MarkdownRenderer _renderer;
        private readonly string _markdownContent;
        private readonly string _dialogTitle;

        /// <summary>
        /// Creates a new MarkdownDialog.
        /// </summary>
        /// <param name="title">Dialog title</param>
        /// <param name="markdownContent">Markdown content to display</param>
        /// <param name="renderer">MarkdownRenderer instance for rendering HTML</param>
        public MarkdownDialog(string title, string markdownContent, MarkdownRenderer renderer)
        {
            _dialogTitle = title ?? "Information";
            _markdownContent = markdownContent ?? string.Empty;
            _renderer = renderer ?? throw new ArgumentNullException(nameof(renderer));

            InitializeComponents();
        }

        /// <summary>
        /// Initializes the dialog components.
        /// </summary>
        private void InitializeComponents()
        {
            // Dialog settings
            this.Text = _dialogTitle;
            this.Size = new Size(700, 500);
            this.MinimumSize = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // Create WebView2
            _webView = new WebView2
            {
                Dock = DockStyle.Fill
            };

            // Create OK button
            _okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                Size = new Size(100, 30),
                Location = new Point(this.ClientSize.Width - 110, this.ClientSize.Height - 40)
            };

            // Add controls to form
            this.Controls.Add(_webView);
            this.Controls.Add(_okButton);

            // Set accept button
            this.AcceptButton = _okButton;

            // Handle WebView2 initialization
            _webView.CoreWebView2InitializationCompleted += OnWebViewInitialized;

            // Use separate user data folder to avoid conflicts with main window
            var userDataFolder = Path.Combine(Path.GetTempPath(), "MarkdownViewerDialog");
            Directory.CreateDirectory(userDataFolder);

            // Initialize WebView2 with separate environment
            InitializeWebView2Async(userDataFolder);

            Log.Debug("MarkdownDialog initialized with title: {Title}", _dialogTitle);
        }

        /// <summary>
        /// Initializes WebView2 with a separate user data folder.
        /// </summary>
        private async void InitializeWebView2Async(string userDataFolder)
        {
            try
            {
                var environment = await Microsoft.Web.WebView2.Core.CoreWebView2Environment.CreateAsync(
                    userDataFolder: userDataFolder
                );
                await _webView!.EnsureCoreWebView2Async(environment);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize WebView2 environment");
            }
        }

        /// <summary>
        /// Called when WebView2 initialization is complete.
        /// </summary>
        private void OnWebViewInitialized(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                Log.Debug("WebView2 initialized successfully in MarkdownDialog");
                RenderMarkdown();
            }
            else
            {
                Log.Error(e.InitializationException, "WebView2 initialization failed in MarkdownDialog");
                MessageBox.Show(
                    $"Failed to initialize markdown viewer:\n{e.InitializationException?.Message ?? "Unknown error"}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
            }
        }

        /// <summary>
        /// Renders the markdown content to HTML and displays it.
        /// </summary>
        private void RenderMarkdown()
        {
            try
            {
                // Use a temporary file path for base URL resolution
                // Since this is just for release notes, we don't need real file paths
                string tempPath = System.IO.Path.Combine(
                    System.IO.Path.GetTempPath(),
                    "markdown-dialog-temp.md");

                // Render markdown to HTML
                string html = _renderer.RenderToHtml(_markdownContent, tempPath);

                // Display in WebView2
                if (_webView?.CoreWebView2 != null)
                {
                    _webView.CoreWebView2.NavigateToString(html);
                    Log.Debug("Markdown content rendered successfully");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to render markdown in dialog");
                MessageBox.Show(
                    $"Failed to render markdown:\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Clean up resources when the dialog is disposed.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _webView?.Dispose();
                _okButton?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Shows the dialog modally and returns the dialog result.
        /// </summary>
        /// <param name="owner">Owner form</param>
        /// <param name="title">Dialog title</param>
        /// <param name="markdownContent">Markdown content to display</param>
        /// <param name="renderer">MarkdownRenderer instance</param>
        /// <returns>DialogResult</returns>
        public static DialogResult ShowDialog(IWin32Window owner, string title, string markdownContent, MarkdownRenderer renderer)
        {
            using var dialog = new MarkdownDialog(title, markdownContent, renderer);
            return dialog.ShowDialog(owner);
        }
    }
}
