# Code Syntax Highlighting Examples

This document demonstrates syntax highlighting for various programming languages.

[← Back to Index](index.md)

## C# Code

```csharp
using System;
using System.IO;
using System.Windows.Forms;

namespace MarkdownViewer
{
    public class MainForm : Form
    {
        private const string Version = "1.0.3";
        private WebView2 webView;

        public MainForm(string filePath)
        {
            InitializeComponents();
            LoadMarkdownFile(filePath);
        }

        private void LoadMarkdownFile(string filePath)
        {
            try
            {
                string markdown = File.ReadAllText(filePath);
                string html = ConvertMarkdownToHtml(markdown);
                webView.NavigateToString(html);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
```

## Python Code

```python
import os
import sys
from pathlib import Path

class MarkdownProcessor:
    def __init__(self, file_path):
        self.file_path = Path(file_path)
        self.content = None

    def read_file(self):
        """Read markdown file content"""
        try:
            with open(self.file_path, 'r', encoding='utf-8') as f:
                self.content = f.read()
            return True
        except FileNotFoundError:
            print(f"Error: File not found: {self.file_path}")
            return False
        except Exception as e:
            print(f"Error reading file: {e}")
            return False

    def process(self):
        """Process markdown content"""
        if self.content is None:
            raise ValueError("No content loaded. Call read_file() first.")

        # Simple processing example
        lines = self.content.split('\n')
        return '\n'.join(f"> {line}" for line in lines)

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python markdown_processor.py <file.md>")
        sys.exit(1)

    processor = MarkdownProcessor(sys.argv[1])
    if processor.read_file():
        result = processor.process()
        print(result)
```

## JavaScript Code

```javascript
class MarkdownViewer {
    constructor(containerId) {
        this.container = document.getElementById(containerId);
        this.currentFile = null;
    }

    async loadFile(filePath) {
        try {
            const response = await fetch(filePath);
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const markdown = await response.text();
            this.render(markdown);
        } catch (error) {
            console.error('Error loading file:', error);
            this.showError(error.message);
        }
    }

    render(markdown) {
        // Convert markdown to HTML
        const html = marked.parse(markdown);
        this.container.innerHTML = html;

        // Syntax highlighting
        hljs.highlightAll();

        // Render Mermaid diagrams
        mermaid.init(undefined, '.language-mermaid');
    }

    showError(message) {
        this.container.innerHTML = `
            <div class="error">
                <strong>Error:</strong> ${message}
            </div>
        `;
    }
}

// Initialize viewer
const viewer = new MarkdownViewer('content');
viewer.loadFile('index.md');
```

## TypeScript Code

```typescript
interface MarkdownFile {
    path: string;
    content: string;
    lastModified: Date;
}

interface RenderOptions {
    syntaxHighlight: boolean;
    renderDiagrams: boolean;
    theme: 'light' | 'dark';
}

class MarkdownRenderer {
    private options: RenderOptions;

    constructor(options: RenderOptions) {
        this.options = options;
    }

    async render(file: MarkdownFile): Promise<string> {
        let html = this.convertMarkdown(file.content);

        if (this.options.syntaxHighlight) {
            html = await this.applySyntaxHighlighting(html);
        }

        if (this.options.renderDiagrams) {
            html = await this.renderDiagrams(html);
        }

        return this.applyTheme(html, this.options.theme);
    }

    private convertMarkdown(markdown: string): string {
        // Markdown conversion logic
        return markdown; // placeholder
    }

    private async applySyntaxHighlighting(html: string): Promise<string> {
        // Syntax highlighting logic
        return html;
    }

    private async renderDiagrams(html: string): Promise<string> {
        // Diagram rendering logic
        return html;
    }

    private applyTheme(html: string, theme: 'light' | 'dark'): string {
        const themeClass = theme === 'dark' ? 'theme-dark' : 'theme-light';
        return `<div class="${themeClass}">${html}</div>`;
    }
}

// Usage
const renderer = new MarkdownRenderer({
    syntaxHighlight: true,
    renderDiagrams: true,
    theme: 'light'
});

const file: MarkdownFile = {
    path: 'index.md',
    content: '# Hello World',
    lastModified: new Date()
};

renderer.render(file).then(html => {
    document.getElementById('content')!.innerHTML = html;
});
```

## Bash/Shell Script

```bash
#!/bin/bash

# MarkdownViewer build script

VERSION="1.0.3"
OUTPUT_DIR="bin-single"
PROJECT_DIR="markdown-viewer/MarkdownViewer"

echo "Building MarkdownViewer v${VERSION}..."

# Clean previous builds
echo "Cleaning previous builds..."
cd "$PROJECT_DIR"
dotnet clean

# Build release
echo "Building release..."
dotnet publish \
    -c Release \
    -r win-x64 \
    --self-contained false \
    -p:PublishSingleFile=true \
    -o "../../${OUTPUT_DIR}"

if [ $? -eq 0 ]; then
    echo "✅ Build successful!"
    echo "Output: ${OUTPUT_DIR}/MarkdownViewer.exe"

    # Get file size
    FILE_SIZE=$(du -h "../../${OUTPUT_DIR}/MarkdownViewer.exe" | cut -f1)
    echo "File size: ${FILE_SIZE}"
else
    echo "❌ Build failed!"
    exit 1
fi

cd ../..

# Test executable
echo ""
echo "Testing executable..."
"${OUTPUT_DIR}/MarkdownViewer.exe" --version

echo ""
echo "Build complete! Run './bin-single/MarkdownViewer.exe --help' for usage."
```

## SQL

```sql
-- Database schema for markdown document management

CREATE TABLE documents (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    file_path TEXT NOT NULL UNIQUE,
    title TEXT,
    content TEXT,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    updated_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE tags (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL UNIQUE
);

CREATE TABLE document_tags (
    document_id INTEGER,
    tag_id INTEGER,
    PRIMARY KEY (document_id, tag_id),
    FOREIGN KEY (document_id) REFERENCES documents(id) ON DELETE CASCADE,
    FOREIGN KEY (tag_id) REFERENCES tags(id) ON DELETE CASCADE
);

-- Indexes for better query performance
CREATE INDEX idx_documents_path ON documents(file_path);
CREATE INDEX idx_documents_updated ON documents(updated_at DESC);
CREATE INDEX idx_tags_name ON tags(name);

-- View for documents with their tags
CREATE VIEW documents_with_tags AS
SELECT
    d.id,
    d.file_path,
    d.title,
    d.updated_at,
    GROUP_CONCAT(t.name, ', ') as tags
FROM documents d
LEFT JOIN document_tags dt ON d.id = dt.document_id
LEFT JOIN tags t ON dt.tag_id = t.id
GROUP BY d.id;

-- Sample queries
SELECT * FROM documents_with_tags WHERE tags LIKE '%tutorial%';
SELECT COUNT(*) as total_docs FROM documents;
SELECT name, COUNT(*) as usage FROM tags
    JOIN document_tags ON tags.id = document_tags.tag_id
    GROUP BY name
    ORDER BY usage DESC;
```

## JSON

```json
{
    "project": "MarkdownViewer",
    "version": "1.0.3",
    "description": "Lightweight Windows Desktop Markdown Viewer",
    "author": "nobiehl",
    "license": "MIT",
    "dependencies": {
        "markdig": "0.37.0",
        "webview2": "1.0.2420.47"
    },
    "features": [
        "Markdown rendering",
        "Mermaid diagrams",
        "PlantUML diagrams",
        "Syntax highlighting",
        "Live reload",
        "Windows Explorer integration"
    ],
    "configuration": {
        "window": {
            "width": 1024,
            "height": 768,
            "title": "Markdown Viewer v{version}"
        },
        "rendering": {
            "theme": "github",
            "syntaxHighlighting": true,
            "diagramRendering": {
                "mermaid": {
                    "enabled": true,
                    "theme": "default"
                },
                "plantuml": {
                    "enabled": true,
                    "encoding": "hex",
                    "format": "png"
                }
            }
        },
        "fileWatcher": {
            "enabled": true,
            "debounceMs": 100
        }
    }
}
```

## YAML

```yaml
# MarkdownViewer Configuration

project:
  name: MarkdownViewer
  version: 1.0.3
  description: Lightweight Windows Desktop Markdown Viewer
  author: nobiehl
  license: MIT

dependencies:
  - name: Markdig
    version: 0.37.0
    purpose: Markdown parsing
  - name: WebView2
    version: 1.0.2420.47
    purpose: HTML rendering

features:
  - id: markdown-rendering
    name: Markdown Rendering
    enabled: true
    description: Full CommonMark support with extensions

  - id: mermaid-diagrams
    name: Mermaid Diagrams
    enabled: true
    config:
      theme: default
      cdn: https://cdn.jsdelivr.net/npm/mermaid@10

  - id: plantuml-diagrams
    name: PlantUML Diagrams
    enabled: true
    config:
      server: https://www.plantuml.com/plantuml
      encoding: hex
      format: png

  - id: syntax-highlighting
    name: Syntax Highlighting
    enabled: true
    config:
      library: highlight.js
      version: 11.9.0
      theme: github

window:
  width: 1024
  height: 768
  title: "Markdown Viewer v{version}"
  centerOnScreen: true

fileWatcher:
  enabled: true
  debounceMs: 100
  watchFilters:
    - "*.md"
    - "*.markdown"
```

## Inline Code

You can also use inline code: `const version = "1.0.3";`

Execute with: `MarkdownViewer.exe index.md`

Install with: `MarkdownViewer.exe --install`

---

**Navigation:**
- [← Previous: PlantUML Examples](plantuml-examples.md)
- [← Back to Index](index.md)
- [Next: Markdown Features →](markdown-features.md)
