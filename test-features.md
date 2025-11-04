# MarkdownViewer Feature Test

This file tests all major features of the Markdown viewer.

## Table of Contents
- [Images](#images)
- [Base64 Images](#base64-images)
- [Links](#links)
- [Tables](#tables)
- [Code Blocks](#code-blocks)
- [Lists](#lists)
- [Blockquotes](#blockquotes)

## Images

### External Image (requires internet)
![GitHub Logo](https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png)

## Base64 Images

### Small Test Image (base64 embedded)

This is a tiny 1x1 red pixel in base64:

![Red Pixel](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==)

### Blue Pixel (base64)

![Blue Pixel](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==)

### SVG Image (base64)

![SVG Circle](data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KICA8Y2lyY2xlIGN4PSI1MCIgY3k9IjUwIiByPSI0MCIgc3Ryb2tlPSJncmVlbiIgc3Ryb2tlLXdpZHRoPSI0IiBmaWxsPSJ5ZWxsb3ciIC8+Cjwvc3ZnPg==)

## Links

### External Links (open in browser)

- [GitHub](https://github.com)
- [Microsoft](https://microsoft.com)
- [Google](https://google.com)

### Internal Links (anchor links)

- [Jump to Images](#images)
- [Jump to Tables](#tables)
- [Jump to Code Blocks](#code-blocks)

### Reference Links

This is a [reference link][1] and another [reference link][2].

[1]: https://en.wikipedia.org/wiki/Markdown
[2]: https://daringfireball.net/projects/markdown/

## Tables

### Simple Table

| Feature | Status | Notes |
|---------|--------|-------|
| Markdown | ✅ | Full CommonMark support |
| Syntax Highlighting | ✅ | Via Highlight.js |
| Mermaid | ✅ | All diagram types |
| PlantUML | ✅ | Server-based |
| Base64 Images | ✅ | Embedded images work |

### Aligned Table

| Left Aligned | Center Aligned | Right Aligned |
|:-------------|:--------------:|--------------:|
| Left | Center | Right |
| A | B | C |
| 1 | 2 | 3 |

## Code Blocks

### C# Code with Syntax Highlighting

```csharp
public class MarkdownViewer
{
    private WebView2 webView;
    private string filePath;

    public MarkdownViewer(string path)
    {
        this.filePath = path;
        InitializeWebView();
        LoadMarkdown();
    }

    private void LoadMarkdown()
    {
        var markdown = File.ReadAllText(filePath);
        var html = ConvertToHtml(markdown);
        webView.NavigateToString(html);
    }
}
```

### Python Code

```python
def hello_world():
    message = "Hello, Markdown Viewer!"
    print(message)
    return message

if __name__ == "__main__":
    hello_world()
```

### JavaScript Code

```javascript
function renderMarkdown(text) {
    const html = marked.parse(text);
    document.getElementById('content').innerHTML = html;
    hljs.highlightAll();
}
```

### Inline Code

Use `MarkdownViewer.exe --install` to install Windows Explorer integration.

## Lists

### Unordered List

- First item
- Second item
  - Nested item 1
  - Nested item 2
    - Double nested
- Third item

### Ordered List

1. First step
2. Second step
   1. Sub-step A
   2. Sub-step B
      1. Sub-sub-step
3. Third step

### Task List

- [x] Markdown rendering
- [x] Syntax highlighting
- [x] Mermaid diagrams
- [x] PlantUML diagrams
- [x] Base64 images
- [ ] Dark mode (future feature)
- [ ] Multi-tab support (future feature)

## Blockquotes

### Simple Quote

> This is a blockquote. It can contain **bold text**, *italic text*, and `inline code`.

### Nested Blockquote

> This is the first level of quoting.
>
> > This is nested blockquote.
> >
> > > This is a third level blockquote.

### Quote with Code

> Here's a quote with some code:
> ```bash
> MarkdownViewer.exe test.md
> ```

## Horizontal Rules

---

***

___

## Text Formatting

### Emphasis

*This is italic text*

_This is also italic text_

**This is bold text**

__This is also bold text__

***This is bold and italic***

~~This is strikethrough~~

### Combinations

**Bold with *italic* inside**

*Italic with **bold** inside*

## Footnotes

Here's a sentence with a footnote[^1].

[^1]: This is the footnote content. It appears at the bottom of the document.

## Emojis

:smiley: :heart: :rocket: :+1: :tada:

## HTML Elements

<div style="background-color: #f0f0f0; padding: 10px; border-radius: 5px;">
This is a <strong>custom HTML block</strong> with inline styling.
</div>

<details>
<summary>Click to expand</summary>

This content is hidden by default and revealed when clicked.

- Can contain
- Markdown
- Elements

```javascript
console.log("Code works too!");
```

</details>

## Mixed Content

This paragraph contains **bold**, *italic*, `code`, [a link](https://example.com), and an image: ![alt](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==)

---

**Test completed successfully!** ✅
