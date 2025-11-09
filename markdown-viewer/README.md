# Markdown Viewer

A lightweight, feature-rich Windows desktop application for viewing Markdown files with live preview, syntax highlighting, and more.

![Version](https://img.shields.io/badge/version-1.7.4-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![License](https://img.shields.io/badge/license-MIT-green)

## ✨ Features

- **📝 Live Markdown Preview** - Real-time rendering with WebView2
- **🔄 Auto-Reload** - Automatically reloads when file changes
- **🎨 Syntax Highlighting** - Code blocks with highlight.js support
- **🔢 Math Support** - LaTeX equations with KaTeX
- **📊 Diagrams** - Mermaid and PlantUML support
- **🎭 Themes** - Multiple built-in themes (Dark, Light, Solarized, Draeger)
- **🌍 Multi-Language** - 8 languages (English, German, Spanish, French, Japanese, Chinese, Russian, Mongolian)
- **🔍 Search** - Find text within your documents (Ctrl+F)
- **🧭 Navigation** - Back/Forward buttons for browsing history
- **🔗 Link Handling** - Navigate between markdown files
- **📎 Windows Integration** - Open .md files from Explorer
- **📱 Status Bar** - Language selector, theme switcher, update checker
- **🆕 Update Checker** - Automatic update notifications
- **📋 MarkdownDialog** - Beautiful info & release notes display with scrollbar
- **ℹ️ App Info** - Comprehensive application information via info button

## 🚀 Installation

### Requirements

- Windows 10 or later
- .NET 8.0 Runtime (included in installer)
- WebView2 Runtime (usually pre-installed on Windows 11)

### Download

Download the latest release from the [Releases page](https://github.com/nobiehl/mini-markdown-viewer/releases/latest).

### Windows Explorer Integration

To open `.md` files directly from Windows Explorer:

```bash
MarkdownViewer.exe --install
```

To remove the integration:

```bash
MarkdownViewer.exe --uninstall
```

## 📖 Usage

### Opening Files

**From Command Line:**
```bash
MarkdownViewer.exe path/to/file.md
```

**From Windows Explorer:**
- Right-click any `.md` file
- Select "Open with Markdown Viewer"

**Drag & Drop:**
- Drag a markdown file onto the application window

### Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `F5` | Reload current file |
| `Ctrl+F` | Open search |
| `Ctrl+O` | Open file dialog |
| `Alt+Left` | Navigate back |
| `Alt+Right` | Navigate forward |
| `Esc` | Close search bar |

## 🎨 Themes

Choose from multiple built-in themes:
- **Standard** - Clean light theme
- **Dark** - Eye-friendly dark theme
- **Solarized** - Popular Solarized color scheme
- **Draeger** - Custom professional theme

Switch themes via the status bar dropdown.

## 🌍 Languages

Available in 8 languages:
- 🇬🇧 English
- 🇩🇪 Deutsch (German)
- 🇪🇸 Español (Spanish)
- 🇫🇷 Français (French)
- 🇯🇵 日本語 (Japanese)
- 🇨🇳 中文 (Chinese)
- 🇷🇺 Русский (Russian)
- 🇲🇳 Монгол (Mongolian)

## 🔧 Development

### Building from Source

**Requirements:**
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code

**Clone & Build:**
```bash
git clone https://github.com/nobiehl/mini-markdown-viewer.git
cd mini-markdown-viewer
dotnet build
```

**Run:**
```bash
cd MarkdownViewer
dotnet run
```

**Create Release Build:**
```bash
cd MarkdownViewer
dotnet build -c Release
```

### Project Structure

```
markdown-viewer/
├── MarkdownViewer/          # Main WinForms application
├── MarkdownViewer.Core/     # Core business logic & services
├── MarkdownViewer.Tests/    # Unit & UI automation tests
└── .github/workflows/       # CI/CD pipeline
```

### Architecture

The application follows the **MVP (Model-View-Presenter)** pattern:
- **Model**: Data classes in `MarkdownViewer.Core.Models`
- **View**: WinForms UI in `MarkdownViewer/UI`
- **Presenter**: Business logic in `MarkdownViewer.Core.Presenters`

### Running Tests

**All Tests:**
```bash
cd MarkdownViewer.Tests
dotnet test
```

**Unit Tests Only:**
```bash
dotnet test --filter "FullyQualifiedName!~UIAutomation"
```

**UI Automation Tests:**
```bash
dotnet test --filter "FullyQualifiedName~UIAutomation"
```

**With Coverage:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

Current test coverage: **69%** (730/1057 lines)

## 🛠️ Technologies

- **.NET 8.0** - Modern cross-platform framework
- **WinForms** - Native Windows UI
- **WebView2** - Chromium-based web rendering
- **Markdig** - Fast Markdown parser
- **KaTeX** - Math rendering
- **Mermaid** - Diagram generation
- **Highlight.js** - Syntax highlighting
- **FlaUI** - UI automation testing
- **xUnit** - Unit testing framework
- **Serilog** - Structured logging

## 📊 Statistics

- **Lines of Code**: ~10,000+
- **Test Coverage**: 69%
- **Unit Tests**: 209
- **UI Tests**: 19
- **Total Tests**: 228
- **Languages**: 8
- **Themes**: 4

## 🐛 Known Issues

- PlantUML requires Java Runtime for server-side rendering
- Very large markdown files (>10MB) may load slowly

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Credits

- [Markdig](https://github.com/xoofx/markdig) - Markdown processor
- [KaTeX](https://katex.org/) - Math typesetting
- [Mermaid](https://mermaid.js.org/) - Diagram rendering
- [Highlight.js](https://highlightjs.org/) - Syntax highlighting
- [WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) - Web rendering
- [Feather Icons](https://feathericons.com/) - UI icons

## 📮 Support

- **Issues**: [GitHub Issues](https://github.com/nobiehl/mini-markdown-viewer/issues)
- **Discussions**: [GitHub Discussions](https://github.com/nobiehl/mini-markdown-viewer/discussions)

## 🗺️ Roadmap

- [ ] Export to PDF
- [ ] Custom theme editor
- [ ] Plugin system
- [ ] Table of contents sidebar
- [ ] Presentation mode

---

**Made with ❤️ using .NET and WebView2**
