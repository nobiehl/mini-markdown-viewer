using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Xunit;
using Xunit.Abstractions;
using MarkdownViewer.UI;

namespace MarkdownViewer.Tests.Performance
{
    /// <summary>
    /// Performance tests for RawDataViewPanel syntax highlighting.
    /// Measures time to highlight different file sizes.
    /// </summary>
    public class SyntaxHighlightingPerformanceTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private readonly RawDataViewPanel _panel;
        private readonly Form _form; // Needed for proper WinForms sizing

        public SyntaxHighlightingPerformanceTests(ITestOutputHelper output)
        {
            _output = output;

            // Create a parent form for accurate measurements
            _form = new Form { Width = 1024, Height = 768 };
            _panel = new RawDataViewPanel();
            _form.Controls.Add(_panel);
        }

        public void Dispose()
        {
            _panel?.Dispose();
            _form?.Dispose();
        }

        [Fact]
        public void PerformanceTest_SmallFile_1KB()
        {
            // Arrange
            string markdown = GenerateMarkdown(1000); // ~1KB
            string html = GenerateHtml(1000);

            // Act
            var sw = Stopwatch.StartNew();
            _panel.ShowRawData(markdown, html);
            sw.Stop();

            // Assert
            _output.WriteLine($"Small file (1KB): {sw.ElapsedMilliseconds}ms");
            Assert.True(sw.ElapsedMilliseconds < 100, $"Expected < 100ms, got {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void PerformanceTest_MediumFile_10KB()
        {
            // Arrange
            string markdown = GenerateMarkdown(10000); // ~10KB
            string html = GenerateHtml(10000);

            // Act
            var sw = Stopwatch.StartNew();
            _panel.ShowRawData(markdown, html);
            sw.Stop();

            // Assert
            _output.WriteLine($"Medium file (10KB): {sw.ElapsedMilliseconds}ms");
            Assert.True(sw.ElapsedMilliseconds < 500, $"Expected < 500ms, got {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void PerformanceTest_LargeFile_50KB()
        {
            // Arrange
            string markdown = GenerateMarkdown(50000); // ~50KB
            string html = GenerateHtml(50000);

            // Act
            var sw = Stopwatch.StartNew();
            _panel.ShowRawData(markdown, html);
            sw.Stop();

            // Assert
            _output.WriteLine($"Large file (50KB): {sw.ElapsedMilliseconds}ms");
            Assert.True(sw.ElapsedMilliseconds < 1000, $"Expected < 1000ms, got {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void PerformanceTest_VeryLargeFile_100KB()
        {
            // Arrange
            string markdown = GenerateMarkdown(100000); // ~100KB
            string html = GenerateHtml(100000);

            // Act
            var sw = Stopwatch.StartNew();
            _panel.ShowRawData(markdown, html);
            sw.Stop();

            // Assert
            _output.WriteLine($"Very large file (100KB): {sw.ElapsedMilliseconds}ms");
            // Should skip highlighting automatically for files > 100KB
            Assert.True(sw.ElapsedMilliseconds < 500, $"Expected < 500ms (no highlighting), got {sw.ElapsedMilliseconds}ms");
        }

        [Fact]
        public void PerformanceTest_RealReadmeFile()
        {
            // Arrange
            string readmePath = Path.Combine(
                Path.GetDirectoryName(typeof(SyntaxHighlightingPerformanceTests).Assembly.Location)!,
                "..", "..", "..", "..", "..", "..", "README.md");

            if (File.Exists(readmePath))
            {
                string markdown = File.ReadAllText(readmePath);
                string html = $"<html><body>{markdown}</body></html>"; // Simple HTML

                // Act
                var sw = Stopwatch.StartNew();
                _panel.ShowRawData(markdown, html);
                sw.Stop();

                // Assert
                _output.WriteLine($"Real README.md ({markdown.Length} bytes): {sw.ElapsedMilliseconds}ms");
                Assert.True(sw.ElapsedMilliseconds < 2000, $"Expected < 2000ms, got {sw.ElapsedMilliseconds}ms");
            }
            else
            {
                _output.WriteLine("README.md not found, skipping test");
            }
        }

        /// <summary>
        /// Generates realistic markdown with headings, code blocks, links, etc.
        /// </summary>
        private string GenerateMarkdown(int targetSize)
        {
            var sb = new System.Text.StringBuilder();

            while (sb.Length < targetSize)
            {
                sb.AppendLine("# Main Heading");
                sb.AppendLine();
                sb.AppendLine("This is a paragraph with some **bold text** and *italic text*.");
                sb.AppendLine();
                sb.AppendLine("## Subheading");
                sb.AppendLine();
                sb.AppendLine("Here's a [link](https://example.com) and some `inline code`.");
                sb.AppendLine();
                sb.AppendLine("```csharp");
                sb.AppendLine("public void Method()");
                sb.AppendLine("{");
                sb.AppendLine("    Console.WriteLine(\"Hello World\");");
                sb.AppendLine("}");
                sb.AppendLine("```");
                sb.AppendLine();
                sb.AppendLine("### Level 3 Heading");
                sb.AppendLine();
                sb.AppendLine("- List item 1");
                sb.AppendLine("- List item 2");
                sb.AppendLine("- List item 3");
                sb.AppendLine();
            }

            return sb.ToString().Substring(0, Math.Min(sb.Length, targetSize));
        }

        /// <summary>
        /// Generates realistic HTML with tags and attributes.
        /// </summary>
        private string GenerateHtml(int targetSize)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head><title>Test</title></head>");
            sb.AppendLine("<body>");

            while (sb.Length < targetSize - 100)
            {
                sb.AppendLine("<h1>Main Heading</h1>");
                sb.AppendLine("<p>This is a paragraph with <strong>bold</strong> and <em>italic</em> text.</p>");
                sb.AppendLine("<h2>Subheading</h2>");
                sb.AppendLine("<p>Here's a <a href=\"https://example.com\">link</a> and some <code>inline code</code>.</p>");
                sb.AppendLine("<pre><code class=\"language-csharp\">");
                sb.AppendLine("public void Method()");
                sb.AppendLine("{");
                sb.AppendLine("    Console.WriteLine(\"Hello World\");");
                sb.AppendLine("}");
                sb.AppendLine("</code></pre>");
                sb.AppendLine("<h3>Level 3 Heading</h3>");
                sb.AppendLine("<ul>");
                sb.AppendLine("<li>List item 1</li>");
                sb.AppendLine("<li>List item 2</li>");
                sb.AppendLine("<li>List item 3</li>");
                sb.AppendLine("</ul>");
            }

            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString().Substring(0, Math.Min(sb.Length, targetSize));
        }
    }
}
