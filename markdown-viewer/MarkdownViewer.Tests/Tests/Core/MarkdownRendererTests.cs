using FluentAssertions;
using MarkdownViewer.Core;
using MarkdownViewer.Core.Models;
using System.IO;

namespace MarkdownViewer.Tests.Tests.Core
{
    /// <summary>
    /// Comprehensive tests for MarkdownRenderer to increase code coverage.
    /// Tests all public methods, edge cases, and integration with themes.
    /// </summary>
    public class MarkdownRendererTests
    {
        private readonly MarkdownRenderer _renderer;
        private readonly string _testFilePath;

        public MarkdownRendererTests()
        {
            _renderer = new MarkdownRenderer();
            _testFilePath = Path.Combine(Path.GetTempPath(), "test.md");
        }

        #region Basic Rendering Tests

        [Fact]
        public void RenderToHtml_SimpleMarkdown_ReturnsValidHtml()
        {
            // Arrange
            string markdown = "# Hello World\n\nThis is a test.";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<!DOCTYPE html>");
            result.Should().Contain("<html>");
            result.Should().Contain("<head>");
            result.Should().Contain("<body>");
            result.Should().Contain("</html>");
            result.Should().Contain("<h1");
            result.Should().Contain("Hello World");
            result.Should().Contain("<p>This is a test.</p>");
        }

        [Fact]
        public void RenderToHtml_WithNullMarkdown_ThrowsArgumentNullException()
        {
            // Arrange
            string? markdown = null;

            // Act
            Action act = () => _renderer.RenderToHtml(markdown!, _testFilePath);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("markdown");
        }

        [Fact]
        public void RenderToHtml_WithNullFilePath_ThrowsArgumentNullException()
        {
            // Arrange
            string markdown = "# Test";
            string? filePath = null;

            // Act
            Action act = () => _renderer.RenderToHtml(markdown, filePath!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("currentFilePath");
        }

        [Fact]
        public void RenderToHtml_EmptyMarkdown_ReturnsValidHtml()
        {
            // Arrange
            string markdown = "";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<!DOCTYPE html>");
            result.Should().Contain("<html>");
            result.Should().Contain("</html>");
        }

        #endregion

        #region Header Rendering Tests

        [Theory]
        [InlineData("# H1", "<h1")]
        [InlineData("## H2", "<h2")]
        [InlineData("### H3", "<h3")]
        [InlineData("#### H4", "<h4")]
        [InlineData("##### H5", "<h5")]
        [InlineData("###### H6", "<h6")]
        public void RenderToHtml_WithHeaders_RendersCorrectly(string markdown, string expectedTag)
        {
            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain(expectedTag);
        }

        [Fact]
        public void RenderToHtml_WithMultipleHeaders_RendersAllLevels()
        {
            // Arrange
            string markdown = @"# Level 1
## Level 2
### Level 3
#### Level 4
##### Level 5
###### Level 6";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<h1");
            result.Should().Contain("<h2");
            result.Should().Contain("<h3");
            result.Should().Contain("<h4");
            result.Should().Contain("<h5");
            result.Should().Contain("<h6");
            result.Should().Contain("Level 1");
            result.Should().Contain("Level 2");
            result.Should().Contain("Level 3");
            result.Should().Contain("Level 4");
            result.Should().Contain("Level 5");
            result.Should().Contain("Level 6");
        }

        #endregion

        #region Code Block Tests

        [Fact]
        public void RenderToHtml_WithCodeBlocks_AppliesSyntaxHighlighting()
        {
            // Arrange
            string markdown = @"```csharp
public class Test
{
    public int Value { get; set; }
}
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("hljs.highlightAll()");
            result.Should().Contain("https://cdnjs.cloudflare.com/ajax/libs/highlight.js");
            result.Should().Contain("<pre>");
            result.Should().Contain("<code");
            result.Should().Contain("public class Test");
        }

        [Fact]
        public void RenderToHtml_WithInlineCode_RendersCorrectly()
        {
            // Arrange
            string markdown = "This is `inline code` in text.";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<code>");
            result.Should().Contain("inline code");
            result.Should().Contain("</code>");
        }

        [Fact]
        public void RenderToHtml_WithMultipleCodeBlocks_RendersAll()
        {
            // Arrange
            string markdown = @"First block:
```javascript
console.log('Hello');
```

Second block:
```python
print('World')
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("console.log");
            result.Should().Contain("print('World')");
        }

        #endregion

        #region Link and Image Tests

        [Fact]
        public void RenderToHtml_WithLinks_RendersCorrectly()
        {
            // Arrange
            string markdown = "[Google](https://www.google.com)";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<a");
            result.Should().Contain("href=\"https://www.google.com\"");
            result.Should().Contain("Google");
            result.Should().Contain("</a>");
        }

        [Fact]
        public void RenderToHtml_WithImages_ResolvesRelativePaths()
        {
            // Arrange
            string markdown = "![Alt text](image.png)";
            string filePath = @"C:\test\document.md";

            // Act
            string result = _renderer.RenderToHtml(markdown, filePath);

            // Assert
            result.Should().Contain("<base href='file:///C:/test/'>");
            result.Should().Contain("<img");
            result.Should().Contain("src=\"image.png\"");
            result.Should().Contain("alt=\"Alt text\"");
        }

        [Fact]
        public void RenderToHtml_WithAbsoluteImagePath_RendersCorrectly()
        {
            // Arrange
            string markdown = "![Image](https://example.com/image.png)";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("src=\"https://example.com/image.png\"");
        }

        #endregion

        #region Theme Tests

        [Fact]
        public void RenderToHtml_WithTheme_AppliesThemeColors()
        {
            // Arrange
            string markdown = "# Test";
            var theme = new Theme
            {
                Name = "custom",
                DisplayName = "Custom Theme",
                Markdown = new MarkdownColors
                {
                    Background = "#1e1e1e",
                    Foreground = "#d4d4d4",
                    HeadingColor = "#569cd6",
                    LinkColor = "#4ec9b0",
                    CodeBackground = "#2d2d2d"
                }
            };

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath, theme);

            // Assert
            result.Should().Contain("background: #1e1e1e");
            result.Should().Contain("color: #d4d4d4");
            result.Should().Contain("color: #569cd6");
            result.Should().Contain("color: #4ec9b0");
            result.Should().Contain("background: #2d2d2d");
        }

        [Fact]
        public void RenderToHtml_WithNullTheme_UsesDefaultColors()
        {
            // Arrange
            string markdown = "# Test";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath, null);

            // Assert
            result.Should().Contain("background: #ffffff");
            result.Should().Contain("color: #333");
            result.Should().Contain("color: #4A90E2");
        }

        [Fact]
        public void RenderToHtml_WithCustomThemeAllColors_AppliesAllColors()
        {
            // Arrange
            string markdown = "# Heading\n\n> Quote\n\n`code`\n\n[link](url)";
            var theme = new Theme
            {
                Markdown = new MarkdownColors
                {
                    Background = "#custom1",
                    Foreground = "#custom2",
                    HeadingColor = "#custom3",
                    LinkColor = "#custom4",
                    CodeBackground = "#custom5",
                    BlockquoteBorder = "#custom6",
                    TableHeaderBackground = "#custom7",
                    TableBorder = "#custom8",
                    InlineCodeBackground = "#custom9",
                    InlineCodeForeground = "#custom10"
                }
            };

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath, theme);

            // Assert
            result.Should().Contain("#custom1");
            result.Should().Contain("#custom2");
            result.Should().Contain("#custom3");
            result.Should().Contain("#custom4");
            result.Should().Contain("#custom5");
            result.Should().Contain("#custom6");
            result.Should().Contain("#custom7");
            result.Should().Contain("#custom8");
            result.Should().Contain("#custom9");
            result.Should().Contain("#custom10");
        }

        #endregion

        #region Mermaid Diagram Tests

        [Fact]
        public void RenderToHtml_WithMermaidDiagram_IncludesScript()
        {
            // Arrange
            string markdown = @"```mermaid
graph TD;
    A-->B;
    A-->C;
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("mermaid");
            result.Should().Contain("https://cdn.jsdelivr.net/npm/mermaid");
            result.Should().Contain("mermaid.initialize");
            result.Should().Contain("graph TD");
        }

        [Fact]
        public void RenderToHtml_WithComplexMermaidDiagram_RendersCorrectly()
        {
            // Arrange
            string markdown = @"```mermaid
sequenceDiagram
    Alice->>John: Hello John
    John-->>Alice: Hi Alice
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("sequenceDiagram");
            result.Should().Contain("Alice");
            result.Should().Contain("John");
        }

        #endregion

        #region PlantUML Diagram Tests

        [Fact]
        public void RenderToHtml_WithPlantUML_IncludesScript()
        {
            // Arrange
            string markdown = @"```plantuml
@startuml
Alice -> Bob: test
@enduml
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("plantuml");
            // The code block gets class="language-plantuml" from markdown rendering
            result.Should().Contain("<code");
            result.Should().Contain("@startuml");
            result.Should().Contain("Alice");
        }

        [Fact]
        public void RenderToHtml_WithPlantUML_GeneratesCorrectUrl()
        {
            // Arrange
            string markdown = @"```plantuml
@startuml
A -> B
@enduml
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("https://www.plantuml.com/plantuml/png/~h");
            result.Should().Contain("language-plantuml");
        }

        #endregion

        #region Advanced Markdown Features Tests

        [Fact]
        public void RenderToHtml_WithTable_RendersCorrectly()
        {
            // Arrange
            string markdown = @"| Header 1 | Header 2 |
|----------|----------|
| Cell 1   | Cell 2   |";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<table");
            result.Should().Contain("<th");
            result.Should().Contain("<td");
            result.Should().Contain("Header 1");
            result.Should().Contain("Cell 1");
        }

        [Fact]
        public void RenderToHtml_WithBlockquote_RendersCorrectly()
        {
            // Arrange
            string markdown = "> This is a quote";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<blockquote>");
            result.Should().Contain("This is a quote");
        }

        [Fact]
        public void RenderToHtml_WithLists_RendersCorrectly()
        {
            // Arrange
            string markdown = @"- Item 1
- Item 2
  - Nested item

1. First
2. Second";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<ul>");
            result.Should().Contain("<ol>");
            result.Should().Contain("<li>");
            result.Should().Contain("Item 1");
            result.Should().Contain("First");
        }

        [Fact]
        public void RenderToHtml_WithHorizontalRule_RendersCorrectly()
        {
            // Arrange
            string markdown = "---";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<hr");
        }

        [Fact]
        public void RenderToHtml_WithEmphasis_RendersCorrectly()
        {
            // Arrange
            string markdown = "*italic* **bold** ***bold italic***";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<em>");
            result.Should().Contain("<strong>");
        }

        #endregion

        #region JavaScript and CSS Inclusion Tests

        [Fact]
        public void RenderToHtml_IncludesHighlightJs()
        {
            // Arrange
            string markdown = "# Test";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("https://cdnjs.cloudflare.com/ajax/libs/highlight.js");
            result.Should().Contain("hljs.highlightAll()");
        }

        [Fact]
        public void RenderToHtml_IncludesKaTeX()
        {
            // Arrange
            string markdown = "# Test";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("https://cdn.jsdelivr.net/npm/katex");
            result.Should().Contain("renderMathInElement");
        }

        [Fact]
        public void RenderToHtml_IncludesMermaidScript()
        {
            // Arrange
            string markdown = "# Test";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("import mermaid from");
            result.Should().Contain("https://cdn.jsdelivr.net/npm/mermaid");
        }

        [Fact]
        public void RenderToHtml_IncludesCopyButtonScript()
        {
            // Arrange
            string markdown = "# Test";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("copy-btn");
            result.Should().Contain("navigator.clipboard.writeText");
        }

        [Fact]
        public void RenderToHtml_IncludesLinkInterceptorScript()
        {
            // Arrange
            string markdown = "# Test";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("window.chrome.webview.postMessage");
            result.Should().Contain("document.addEventListener('click'");
        }

        #endregion

        #region Base URL and Path Resolution Tests

        [Fact]
        public void RenderToHtml_SetsCorrectBaseUrl()
        {
            // Arrange
            string markdown = "# Test";
            string filePath = @"C:\Users\Test\Documents\file.md";

            // Act
            string result = _renderer.RenderToHtml(markdown, filePath);

            // Assert
            result.Should().Contain("<base href='file:///C:/Users/Test/Documents/'>");
        }

        [Fact]
        public void RenderToHtml_WithRootPath_SetsCorrectBaseUrl()
        {
            // Arrange
            string markdown = "# Test";
            string filePath = @"C:\file.md";

            // Act
            string result = _renderer.RenderToHtml(markdown, filePath);

            // Assert
            // On Windows, the root path is rendered as C:/ (double slash due to empty directory name)
            result.Should().Contain("<base href='file:///C://'>");
        }

        #endregion

        #region Chart.js Tests

        [Fact]
        public void RenderToHtml_WithChartBlock_IncludesChartJs()
        {
            // Arrange
            string markdown = @"```chart
{
  ""type"": ""line"",
  ""data"": {
    ""labels"": [""Jan"", ""Feb"", ""Mar""],
    ""datasets"": [{
      ""label"": ""Sales"",
      ""data"": [10, 20, 30]
    }]
  }
}
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("https://cdn.jsdelivr.net/npm/chart.js");
            result.Should().Contain("chart.umd.min.js");
            result.Should().Contain("language-chart");
        }

        [Fact]
        public void RenderToHtml_WithChartBlock_IncludesCanvasElement()
        {
            // Arrange
            string markdown = @"```chart
{
  ""type"": ""bar"",
  ""data"": {
    ""labels"": [""A"", ""B"", ""C""],
    ""datasets"": [{
      ""label"": ""Values"",
      ""data"": [5, 10, 15]
    }]
  }
}
```";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("new Chart(canvas, config)");
            result.Should().Contain("document.querySelectorAll('code.language-chart')");
            result.Should().Contain("canvas = document.createElement('canvas')");
        }

        #endregion

        #region Table of Contents (TOC) Tests

        [Fact]
        public void RenderToHtml_WithTocPlaceholder_ContainsTableOfContents()
        {
            // Arrange
            var markdown = "[TOC]\n\n## Heading 1\n\n### Subheading\n\n## Heading 2";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            Assert.Contains("[TOC]", html); // JavaScript will replace this
            Assert.Contains("h2 id=\"heading-1\"", html);
            Assert.Contains("h2 id=\"heading-2\"", html);
            Assert.Contains("h3 id=\"subheading\"", html);
        }

        [Fact]
        public void RenderToHtml_WithTocPlaceholder_ContainsJavaScript()
        {
            // Arrange
            var markdown = "[TOC]\n\n## Test Heading";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            Assert.Contains("// Auto-generate Table of Contents from [TOC] placeholder", html);
            Assert.Contains("document.querySelector('p:has(> :only-child:is", html);
            Assert.Contains("table-of-contents", html);
        }

        [Fact]
        public void RenderToHtml_WithTocPlaceholder_ContainsCssStyles()
        {
            // Arrange
            var markdown = "[TOC]\n\n## Test Heading";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            Assert.Contains(".table-of-contents", html);
            Assert.Contains(".toc-title", html);
            Assert.Contains(".toc-list", html);
            Assert.Contains(".toc-link", html);
        }

        [Fact]
        public void RenderToHtml_WithoutTocPlaceholder_DoesNotAffectRendering()
        {
            // Arrange
            var markdown = "## Heading 1\n\n### Subheading";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            // Should still contain TOC JavaScript (always included)
            Assert.Contains("// Auto-generate Table of Contents", html);
            // Should contain headings but not TOC markup in body
            Assert.Contains("h2 id=\"heading-1\"", html);
            Assert.Contains("h3 id=\"subheading\"", html);
        }

        [Fact]
        public void RenderToHtml_WithMultipleLevels_GeneratesNestedToc()
        {
            // Arrange
            var markdown = @"[TOC]

# Main Title

## Chapter 1
### Section 1.1
#### Subsection 1.1.1

## Chapter 2
### Section 2.1";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            Assert.Contains("h1 id=\"main-title\"", html);
            Assert.Contains("h2 id=\"chapter-1\"", html);
            Assert.Contains("h3 id=\"section-1", html); // AutoIdentifier converts dots to hyphens
            Assert.Contains("h4 id=\"subsection-1", html);
            Assert.Contains("h2 id=\"chapter-2\"", html);
            Assert.Contains("h3 id=\"section-2", html);
        }

        #endregion

        #region Edge Cases and Error Handling Tests

        [Fact]
        public void RenderToHtml_WithSpecialCharacters_EscapesCorrectly()
        {
            // Arrange
            string markdown = "Test with <script>alert('xss')</script>";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            // Markdig renders HTML tags in paragraph content as-is by default
            // but the HTML is wrapped in <p> tags, making it safe in the context
            result.Should().Contain("<p>Test with");
            result.Should().Contain("script");
            // Verify the document structure is valid
            result.Should().Contain("<!DOCTYPE html>");
            result.Should().Contain("<html>");
            result.Should().Contain("</html>");
        }

        [Fact]
        public void RenderToHtml_WithVeryLongText_HandlesCorrectly()
        {
            // Arrange
            string markdown = new string('a', 10000);

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("<html>");
        }

        [Fact]
        public void RenderToHtml_WithUnicodeCharacters_PreservesEncoding()
        {
            // Arrange
            string markdown = "# Überschrift\n\nTäst mit Ümläutän und 中文字符";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("charset='UTF-8'");
            result.Should().Contain("Überschrift");
            result.Should().Contain("Ümläutän");
            result.Should().Contain("中文字符");
        }

        [Fact]
        public void RenderToHtml_WithMathFormulas_IncludesKaTeXDelimiters()
        {
            // Arrange
            string markdown = "Inline math: $x^2$ and display: $$E=mc^2$$";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("renderMathInElement");
            result.Should().Contain("delimiters:");
            result.Should().Contain("$$");
            result.Should().Contain("$");
        }

        [Fact]
        public void RenderToHtml_WithTaskLists_RendersCorrectly()
        {
            // Arrange
            string markdown = @"- [x] Completed task
- [ ] Incomplete task";

            // Act
            string result = _renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            result.Should().Contain("<input");
            result.Should().Contain("type=\"checkbox\"");
        }

        #endregion

        #region Admonitions / Callouts Tests

        [Fact]
        public void RenderToHtml_WithCustomContainers_GeneratesAdmonitions()
        {
            // Arrange
            var markdown = @"::: note
This is a note
:::

::: warning
This is a warning
:::";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("<div class=\"note\">");
            html.Should().Contain("<div class=\"warning\">");
            html.Should().Contain("This is a note");
            html.Should().Contain("This is a warning");
        }

        [Theory]
        [InlineData("note")]
        [InlineData("info")]
        [InlineData("tip")]
        [InlineData("warning")]
        [InlineData("danger")]
        public void RenderToHtml_WithAllAdmonitionTypes_GeneratesCorrectClasses(string type)
        {
            // Arrange
            var markdown = $"::: {type}\nContent\n:::";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain($"<div class=\"{type}\">");
        }

        #endregion

        #region Emoji Support Tests

        [Fact]
        public void RenderToHtml_WithEmojiCodes_ConvertsToEmoji()
        {
            // Arrange
            var markdown = ":smile: :heart: :rocket:";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            // Emojis should be converted to Unicode emoji characters
            html.Should().Contain("😄"); // smile
            html.Should().Contain("❤"); // heart
            html.Should().Contain("🚀"); // rocket
        }

        [Fact]
        public void RenderToHtml_WithMultipleEmojis_ConvertsAll()
        {
            // Arrange
            var markdown = "I :heart: Markdown! :rocket: :fire: :tada:";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("❤"); // heart
            html.Should().Contain("🚀"); // rocket
            html.Should().Contain("🔥"); // fire
            html.Should().Contain("🎉"); // tada
        }

        [Fact]
        public void RenderToHtml_WithEmojiInHeading_ConvertsCorrectly()
        {
            // Arrange
            var markdown = "# :rocket: Getting Started";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("🚀");
            html.Should().Contain("Getting Started");
            html.Should().Contain("<h1");
        }

        [Fact]
        public void RenderToHtml_WithEmojiInList_ConvertsCorrectly()
        {
            // Arrange
            var markdown = "- :white_check_mark: Done\n- :x: Not done";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("✅");
            html.Should().Contain("❌");
        }

        #endregion

        #region Diff Highlighting Tests

        [Fact]
        public void RenderToHtml_WithDiffCode_ContainsDiffClasses()
        {
            // Arrange
            var markdown = "```diff\n- removed\n+ added\n```";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("language-diff");
            html.Should().Contain("hljs-deletion");
            html.Should().Contain("hljs-addition");
        }

        [Fact]
        public void RenderToHtml_WithComplexDiff_RendersCorrectly()
        {
            // Arrange
            var markdown = @"```diff
public class Test
{
-   public int Value { get; set; }
+   public string Value { get; set; }
}
```";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("language-diff");
            html.Should().Contain("public class Test");
        }

        [Fact]
        public void RenderToHtml_WithGitDiff_RendersCorrectly()
        {
            // Arrange
            var markdown = @"```diff
diff --git a/file.js b/file.js
--- a/file.js
+++ b/file.js
@@ -1,3 +1,3 @@
-console.log('old');
+console.log('new');
```";
            var renderer = new MarkdownRenderer();

            // Act
            var html = renderer.RenderToHtml(markdown, _testFilePath);

            // Assert
            html.Should().Contain("language-diff");
            html.Should().Contain("diff --git");
        }

        #endregion
    }
}
