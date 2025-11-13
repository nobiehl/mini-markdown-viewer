# Markdown Features

This document demonstrates various Markdown features supported by MarkdownViewer.

[← Back to Index](index.md)

## Headers

# H1 Header
## H2 Header
### H3 Header
#### H4 Header
##### H5 Header
###### H6 Header

## Text Formatting

**Bold text** or __bold text__

*Italic text* or _italic text_

***Bold and italic*** or ___bold and italic___

~~Strikethrough~~

## Lists

### Unordered List

- Item 1
- Item 2
  - Nested item 2.1
  - Nested item 2.2
    - Double nested item 2.2.1
- Item 3

### Ordered List

1. First item
2. Second item
   1. Nested item 2.1
   2. Nested item 2.2
3. Third item

### Task List

- [x] Completed task
- [x] Another completed task
- [ ] Incomplete task
- [ ] Another incomplete task

## Links

### External Links

- [GitHub](https://github.com)
- [Microsoft](https://microsoft.com)
- [Anthropic](https://anthropic.com)

### Internal Links (Anchor Links)

- [Jump to Tables](#tables)
- [Jump to Images](#images)
- [Jump to Blockquotes](#blockquotes)

### Links to Other Markdown Files

- [Mermaid Examples](mermaid-examples.md)
- [PlantUML Examples](plantuml-examples.md)
- [Code Examples](code-examples.md)

## Tables

### Simple Table

| Feature | Status | Notes |
|---------|--------|-------|
| Markdown | ✅ | Full CommonMark |
| Mermaid | ✅ | All diagram types |
| PlantUML | ✅ | HEX encoding |
| Highlighting | ✅ | Via Highlight.js |

### Aligned Table

| Left | Center | Right |
|:-----|:------:|------:|
| A | B | C |
| 1 | 2 | 3 |
| Left aligned | Centered | Right aligned |

## Images

### External Images

![GitHub Logo](https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png)

### Base64 Embedded Images

**Red Pixel (PNG):**

![Red Pixel](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==)

**Blue Pixel (PNG):**

![Blue Pixel](data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==)

**SVG Circle:**

![SVG Circle](data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMTAwIiBoZWlnaHQ9IjEwMCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj4KICA8Y2lyY2xlIGN4PSI1MCIgY3k9IjUwIiByPSI0MCIgc3Ryb2tlPSJncmVlbiIgc3Ryb2tlLXdpZHRoPSI0IiBmaWxsPSJ5ZWxsb3ciIC8+Cjwvc3ZnPg==)

## Blockquotes

### Simple Blockquote

> This is a blockquote.
> It can span multiple lines.

### Nested Blockquote

> First level
>
> > Second level
> >
> > > Third level

### Blockquote with Code

> Example command:
> ```bash
> MarkdownViewer.exe --install
> ```

## Code Blocks

### Inline Code

Use `MarkdownViewer.exe` to open markdown files.

### Fenced Code Block

```
Plain text code block
No syntax highlighting
```

### Code Block with Language

See [Code Examples](code-examples.md) for language-specific examples.

## Horizontal Rules

---

***

___

## Footnotes

Here's a sentence with a footnote[^1].

Here's another with a longer footnote[^longnote].

[^1]: This is the first footnote.

[^longnote]: This is a longer footnote with multiple lines.

    You can have multiple paragraphs in a footnote.

## Definition Lists

Term 1
:   Definition 1

Term 2
:   Definition 2a
:   Definition 2b

## Abbreviations

The HTML specification is maintained by the W3C.

*[HTML]: Hyper Text Markup Language
*[W3C]: World Wide Web Consortium

## Emoji

:smiley: :heart: :rocket: :+1: :tada: :sparkles: :fire:

(Note: Emoji rendering depends on Markdig configuration)

## HTML Elements

<div style="background-color: #f0f0f0; padding: 10px; border-radius: 5px; margin: 10px 0;">
This is a <strong>custom HTML block</strong> with inline styling.
</div>

<details>
<summary>Click to expand</summary>

Hidden content revealed!

- List item 1
- List item 2

```javascript
console.log("Code works in details too!");
```

</details>

## Mathematics (if supported)

Inline math: $E = mc^2$

Block math:

$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

(Note: Math rendering depends on Markdig extensions)

---

**Navigation:**
- [← Previous: Code Examples](code-examples.md)
- [← Back to Index](index.md)
- [Next: Test Suite →](test-diagrams.md)
