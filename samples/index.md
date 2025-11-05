# MarkdownViewer Sample Documents

Welcome to the MarkdownViewer sample document collection! This index provides navigation to all sample files demonstrating different features.

## 📚 Sample Categories

### Diagram Examples

1. **[Mermaid Examples](mermaid-examples.md)**
   - Flowcharts
   - Sequence diagrams
   - Class diagrams
   - State diagrams
   - Gantt charts
   - Pie charts
   - Git graphs

2. **[PlantUML Examples](plantuml-examples.md)**
   - Class diagrams
   - Sequence diagrams
   - Use case diagrams
   - Activity diagrams
   - Component diagrams
   - State diagrams
   - Deployment diagrams
   - Object diagrams

### Code and Syntax

3. **[Code Examples](code-examples.md)**
   - C# (with full application example)
   - Python (file processing)
   - JavaScript (async/await)
   - TypeScript (interfaces and classes)
   - Bash/Shell scripting
   - SQL (database schema)
   - JSON configuration
   - YAML configuration

### Mathematical Formulas

4. **[Mathematical Formulas](math-examples.md)**
   - Inline math ($E = mc^2$)
   - Display math (centered equations)
   - Fractions, subscripts, superscripts
   - Greek letters
   - Matrices
   - Summations and integrals
   - Limits and derivatives
   - Complex formulas (Maxwell's equations, etc.)

### Markdown Features

5. **[Markdown Features](markdown-features.md)**
   - Headers (H1-H6)
   - Text formatting (bold, italic, strikethrough)
   - Lists (ordered, unordered, task lists)
   - Links (external, internal, inter-document)
   - Tables (simple and aligned)
   - Images (external and base64 embedded)
   - Blockquotes (simple and nested)
   - Code blocks (inline and fenced)
   - Horizontal rules
   - Footnotes
   - HTML elements

### Test Files

6. **[Test Diagrams](test-diagrams.md)**
   - Combined Mermaid and PlantUML tests
   - Original test file

7. **[Test Features](test-features.md)**
   - Comprehensive feature test
   - Images, links, tables, code blocks
   - All in one file

8. **[Test Math](test-math.md)**
   - Comprehensive mathematical formula tests
   - All formula types and edge cases
   - Inline, block, matrices, Greek letters

## 🚀 Quick Navigation

### By Feature Type

**Diagrams:**
- [Mermaid Examples](mermaid-examples.md) | [PlantUML Examples](plantuml-examples.md)

**Code:**
- [Code Examples](code-examples.md)

**Math:**
- [Mathematical Formulas](math-examples.md)

**Markdown:**
- [Markdown Features](markdown-features.md)

**Testing:**
- [Test Diagrams](test-diagrams.md) | [Test Features](test-features.md) | [Test Math](test-math.md)

## 🔍 Feature Matrix

| Feature | Sample File | Status |
|---------|-------------|--------|
| Mermaid Flowchart | [mermaid-examples.md](mermaid-examples.md) | ✅ |
| Mermaid Sequence | [mermaid-examples.md](mermaid-examples.md) | ✅ |
| PlantUML Class | [plantuml-examples.md](plantuml-examples.md) | ✅ |
| PlantUML Sequence | [plantuml-examples.md](plantuml-examples.md) | ✅ |
| C# Syntax | [code-examples.md](code-examples.md) | ✅ |
| Python Syntax | [code-examples.md](code-examples.md) | ✅ |
| JavaScript Syntax | [code-examples.md](code-examples.md) | ✅ |
| TypeScript Syntax | [code-examples.md](code-examples.md) | ✅ |
| Tables | [markdown-features.md](markdown-features.md) | ✅ |
| Base64 Images | [markdown-features.md](markdown-features.md) | ✅ |
| Task Lists | [markdown-features.md](markdown-features.md) | ✅ |
| Blockquotes | [markdown-features.md](markdown-features.md) | ✅ |
| Inline Math | [math-examples.md](math-examples.md) | ✅ |
| Display Math | [math-examples.md](math-examples.md) | ✅ |
| Complex Formulas | [test-math.md](test-math.md) | ✅ |

## 📖 Reading Order

If you're new to MarkdownViewer, we recommend reading the samples in this order:

1. **Start here:** [Markdown Features](markdown-features.md) - Learn basic Markdown syntax
2. **Next:** [Code Examples](code-examples.md) - See syntax highlighting in action
3. **Then:** [Mathematical Formulas](math-examples.md) - Learn math formula syntax
4. **After that:** [Mermaid Examples](mermaid-examples.md) - Explore Mermaid diagrams
5. **Finally:** [PlantUML Examples](plantuml-examples.md) - Discover PlantUML capabilities

## 🛠️ Testing Files

For testing specific features:

- **Diagram Testing:** [test-diagrams.md](test-diagrams.md)
- **Feature Testing:** [test-features.md](test-features.md)
- **Math Testing:** [test-math.md](test-math.md)
- **PlantUML Encoding Test:** [test-plantuml-encoding.html](test-plantuml-encoding.html) (open in browser)

## 💡 Tips

### Navigation
- Use `[link text](filename.md)` to link to other markdown files
- Use `[link text](#anchor)` for internal page navigation
- Click links to navigate between documents

### File Watching
- Edit any markdown file while viewing
- MarkdownViewer automatically reloads on save
- Perfect for live editing workflow

### Windows Integration
After running `MarkdownViewer.exe --install`:
- Double-click any .md file to open
- Right-click → "Open with Markdown Viewer"
- Right-click → "Send to" → "Markdown Viewer"

## 📝 Document Structure

```
samples/
├── index.md                       ← You are here
├── mermaid-examples.md            ← Mermaid diagrams
├── plantuml-examples.md           ← PlantUML diagrams
├── code-examples.md               ← Code syntax highlighting
├── math-examples.md               ← Mathematical formulas
├── markdown-features.md           ← Markdown features
├── test-diagrams.md               ← Combined diagram tests
├── test-features.md               ← Feature tests
├── test-math.md                   ← Mathematical formula tests
└── test-plantuml-encoding.html    ← PlantUML encoding test
```

## 🔗 External Resources

- [Markdown Guide](https://www.markdownguide.org/)
- [Mermaid Documentation](https://mermaid.js.org/)
- [PlantUML Documentation](https://plantuml.com/)
- [KaTeX Documentation](https://katex.org/)
- [Highlight.js Languages](https://highlightjs.org/static/demo/)

## 🎯 What to Test

When testing MarkdownViewer, verify:

1. ✅ All Mermaid diagram types render
2. ✅ All PlantUML diagram types render (not placeholders!)
3. ✅ Code syntax highlighting works for all languages
4. ✅ Tables render with correct alignment
5. ✅ Base64 images display correctly
6. ✅ Links between markdown files work
7. ✅ Live reload works when editing files
8. ✅ Copy buttons appear on code blocks
9. ✅ Inline math formulas render correctly
10. ✅ Display math formulas are centered and formatted
11. ✅ Complex math formulas (matrices, Greek letters, etc.) work

---

**Version:** 1.0.5
**Last Updated:** 2025-11-05
**Generated with:** Claude Code

**Happy Markdown Viewing!** 🚀
