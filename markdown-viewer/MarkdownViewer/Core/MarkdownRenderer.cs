using System.IO;
using Markdig;
using MarkdownViewer.Models;

namespace MarkdownViewer.Core
{
    /// <summary>
    /// Renders Markdown to HTML with embedded CSS and JavaScript.
    /// Supports:
    /// - Syntax highlighting (Highlight.js)
    /// - Math formulas (KaTeX)
    /// - Mermaid diagrams
    /// - PlantUML diagrams
    /// - Copy buttons for code blocks
    /// - Theme support
    /// </summary>
    public class MarkdownRenderer
    {
        private readonly MarkdownPipeline _pipeline;

        public MarkdownRenderer()
        {
            _pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions() // Includes AutoIdentifiers for heading IDs
                .UseMathematics()
                .Build();
        }

        /// <summary>
        /// Converts Markdown text to a complete HTML document.
        /// </summary>
        /// <param name="markdown">Raw Markdown text</param>
        /// <param name="currentFilePath">Path to current file (for relative link resolution)</param>
        /// <param name="theme">Optional theme for styling (uses default if null)</param>
        /// <returns>Complete HTML document as string</returns>
        public string RenderToHtml(string markdown, string currentFilePath, Theme theme = null)
        {
            string content = Markdown.ToHtml(markdown, _pipeline);

            // Generate base URL for relative link resolution
            string currentDir = Path.GetDirectoryName(currentFilePath) ?? ".";
            string baseUrl = $"file:///{currentDir.Replace("\\", "/")}/";

            // Use theme colors if provided, otherwise use defaults
            string backgroundColor = theme?.Markdown.Background ?? "#ffffff";
            string foregroundColor = theme?.Markdown.Foreground ?? "#333";
            string codeBackground = theme?.Markdown.CodeBackground ?? "#f5f5f5";
            string linkColor = theme?.Markdown.LinkColor ?? "#4A90E2";
            string headingColor = theme?.Markdown.HeadingColor ?? "#000";
            string blockquoteBorder = theme?.Markdown.BlockquoteBorder ?? "#4A90E2";
            string tableHeaderBg = theme?.Markdown.TableHeaderBackground ?? "#f5f5f5";
            string tableBorder = theme?.Markdown.TableBorder ?? "#ddd";
            string inlineCodeBg = theme?.Markdown.InlineCodeBackground ?? "#f5f5f5";
            string inlineCodeFg = theme?.Markdown.InlineCodeForeground ?? "#c7254e";

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <base href='{baseUrl}'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/styles/github.min.css'>
    <link rel='stylesheet' href='https://cdn.jsdelivr.net/npm/katex@0.16.9/dist/katex.min.css' integrity='sha384-n8MVd4RsNIU0tAv4ct0nTaAbDJwPJzDEaqSD1odI+WdtXRGWt2kTvGFasHpSy3SV' crossorigin='anonymous'>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
            line-height: 1.6;
            max-width: 900px;
            margin: 0 auto;
            padding: 2rem;
            color: {foregroundColor};
            background: {backgroundColor};
        }}
        h1, h2, h3, h4, h5, h6 {{
            margin-top: 1.5rem;
            margin-bottom: 1rem;
            font-weight: 600;
            color: {headingColor};
        }}
        h1 {{ font-size: 2rem; border-bottom: 2px solid #eee; padding-bottom: 0.5rem; }}
        h2 {{ font-size: 1.5rem; border-bottom: 1px solid #eee; padding-bottom: 0.3rem; }}
        h3 {{ font-size: 1.25rem; }}
        code {{
            background: {inlineCodeBg};
            color: {inlineCodeFg};
            padding: 0.2rem 0.4rem;
            border-radius: 3px;
            font-family: 'Consolas', 'Monaco', 'Courier New', monospace;
            font-size: 0.9em;
        }}
        pre {{
            background: {codeBackground};
            padding: 1rem;
            border-radius: 6px;
            overflow-x: auto;
            position: relative;
        }}
        pre code {{
            background: none;
            padding: 0;
            color: inherit;
        }}
        /* Copy button for code blocks */
        .copy-btn {{
            position: absolute;
            top: 0.5rem;
            right: 0.5rem;
            padding: 0.25rem 0.75rem;
            background: {linkColor};
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
            filter: brightness(0.9);
        }}
        blockquote {{
            border-left: 4px solid {blockquoteBorder};
            padding-left: 1rem;
            margin: 1rem 0;
            opacity: 0.8;
        }}
        a {{
            color: {linkColor};
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
            border: 1px solid {tableBorder};
            padding: 0.5rem;
        }}
        table th {{
            background: {tableHeaderBg};
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
        /* Math formula styling */
        .math {{
            overflow-x: auto;
            overflow-y: hidden;
        }}
        .math.math-display {{
            display: block;
            margin: 1rem 0;
            text-align: center;
        }}
        .math.math-inline {{
            display: inline;
        }}
    </style>
</head>
<body>
    {content}
    <script src='https://cdnjs.cloudflare.com/ajax/libs/highlight.js/11.9.0/highlight.min.js'></script>
    <script defer src='https://cdn.jsdelivr.net/npm/katex@0.16.9/dist/katex.min.js' integrity='sha384-XjKyOOlGwcjNTAIQHIpgOno0Hl1YQqzUOEleOLALmuqehneUG+vnGctmUb0ZY0l8' crossorigin='anonymous'></script>
    <script defer src='https://cdn.jsdelivr.net/npm/katex@0.16.9/dist/contrib/auto-render.min.js' integrity='sha384-+VBxd3r6XgURycqtZ117nYw44OOcIax56Z4dCRWbxyPt0Koah1uHoK0o4+/RRE05' crossorigin='anonymous'></script>
    <script type='module'>
        import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
        mermaid.initialize({{ startOnLoad: true, theme: 'default' }});
    </script>
    <script>
        // Syntax highlighting
        hljs.highlightAll();

        // Render mathematical formulas with KaTeX
        document.addEventListener('DOMContentLoaded', function() {{
            renderMathInElement(document.body, {{
                delimiters: [
                    {{left: '$$', right: '$$', display: true}},
                    {{left: '$', right: '$', display: false}},
                    {{left: '\\[', right: '\\]', display: true}},
                    {{left: '\\(', right: '\\)', display: false}}
                ],
                throwOnError: false,
                errorColor: '#cc0000'
            }});
        }});

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

                // Ignore empty hrefs
                if (!href) {{
                    return;
                }}

                // Ignore data: URIs
                if (href.startsWith('data:')) {{
                    return;
                }}

                // Prevent default navigation (including anchor links)
                e.preventDefault();

                // Send to C# for handling (including anchor links)
                console.log('Intercepted link click:', href);
                window.chrome.webview.postMessage(href);
            }}
        }}, true);
    </script>
</body>
</html>";
        }
    }
}
