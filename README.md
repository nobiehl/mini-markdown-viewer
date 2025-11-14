# MarkdownViewer - Lightweight Windows Desktop Markdown Viewer

Lightweight Windows desktop viewer for Markdown files with themes, localization, navigation, and search. Full Windows Explorer integration included.

![Version](https://img.shields.io/badge/version-1.9.1-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Size](https://img.shields.io/badge/size-3.3_MB-green)
![Languages](https://img.shields.io/badge/languages-8-orange)
![Themes](https://img.shields.io/badge/themes-4-purple)
![Tests](https://img.shields.io/badge/tests-283_passing-success)

## Features

### Core Features
- ✅ **Markdown Rendering** with full CommonMark support
- ✅ **Syntax Highlighting** for code blocks (via Highlight.js)
- ✅ **Mathematical Formulas** with LaTeX syntax (via KaTeX)
  - Inline math: `$E = mc^2$`
  - Display math: `$$\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}$$`
  - Full LaTeX support: matrices, Greek letters, fractions, summations, integrals, etc.
- ✅ **Auto Table of Contents** - Generate TOC from `[TOC]` placeholder
- ✅ **Emoji Support** - Convert emoji codes (`:smile:`, `:rocket:`) to Unicode emojis
- ✅ **Code Diff Highlighting** - Highlight added (+) and removed (-) lines in diff blocks
- ✅ **Admonitions/Callouts** - Styled boxes for notes, tips, warnings, and danger alerts
- ✅ **Charts** - Chart.js for data visualization (line, bar, pie, doughnut, radar, polar area)
- ✅ **Mermaid Diagrams** (flowcharts, sequence, class, state, gantt, ER, etc.)
- ✅ **PlantUML Diagrams** (class, sequence, use case, activity, component, etc.)
- ✅ **Images** (including base64-embedded)
- ✅ **Tables, Lists, Blockquotes** (via Markdig extensions)
- ✅ **Live Reload** on file changes (FileSystemWatcher)
- ✅ **Copy Buttons** for code blocks
- ✅ **Link Navigation**: Navigate between .md files, external links open in browser
- ✅ **Anchor Links**: Jump to headings with # anchors
- ✅ **Professional Logging**: Serilog with rolling daily logs and configurable levels
- ✅ **Automatic Updates**: Check for updates once every 7 days, manual check with `--update`
  - Background check (non-blocking, automatic retry on failure)
  - Release notes display
  - Safe installation with backup and rollback
  - Test mode for development

### Advanced Features

#### 🎨 Themes (v1.2.0)
- **4 Built-in Themes**: Dark, Solarized Light, Dräger, Standard
- **Theme Switcher**: StatusBar dropdown or right-click context menu
- **Instant Application**: No restart required
- **Settings Persistence**: Theme saved to `settings.json`
- **Theme-aware Icons** (v1.6.0): Dynamic icon generation adapts to theme
- **Custom Themes**: Add your own in `Themes/` folder

#### 🌍 Localization (v1.3.0)
- **8 Languages**: English, Deutsch, Монгол, Français, Español, 日本語, 简体中文, Русский
- **Instant Language Switching**: via StatusBar dropdown
- **Localized UI**: All interface elements translated
- **Fallback Support**: Missing translations fall back to English

#### 📊 StatusBar (v1.3.0)
- **5 Sections**: Update status, Explorer registration, Language selector, Info, Help
- **Real-time Status**: Shows current state of all features
- **Always Visible**: Displayed at bottom of window

#### 🧭 Navigation (v1.4.0)
- **Back/Forward Buttons**: Navigate document history (Alt+Left/Right)
- **WebView2 History**: Seamless integration with browser navigation
- **Navigation Bar**: Optional toolbar (hidden by default)
- **Keyboard Shortcuts**: Alt+Left (back), Alt+Right (forward)

#### 🔍 In-Page Search (v1.4.0)
- **Real-time Highlighting**: Yellow for matches, orange for current
- **Match Navigation**: F3 (next), Shift+F3 (previous)
- **Results Counter**: "X of Y" display
- **Keyboard Shortcuts**: Ctrl+F (open), Enter/Shift+Enter (navigate), Esc (close)
- **mark.js Integration**: Fast highlighting with smooth scrolling

#### 🎨 Theme-Aware Icons (v1.6.0)
- **Dynamic Icon Generation**: Icons automatically adapt to current theme
- **Proper Contrast**: Visible on all backgrounds (dark/light)
- **SVG-Based Rendering**: Crisp icons at any size
- **StatusBar Integration**: Update, Explorer, Info, Help icons themed

#### 🏗️ Architecture Refactoring (v1.7.0)
- **Layered Architecture**: Clean separation of concerns
  - **MarkdownViewer.Core**: Business logic library (rendering, file watching, settings)
  - **MarkdownViewer**: WinForms UI layer (thin wrapper)
  - **Benefits**: Better testability, code reuse, maintainability

#### 📢 Update Notifications (v1.8.0)
- **UpdateNotificationBar**: Non-invasive notification above StatusBar
- **3 Action Buttons**: Show Release Notes, Install Update, Ignore
- **Theme-Aware Colors**: Adapts to light/dark theme
- **Fully Localized**: All 8 supported languages

#### 📝 Extended Markdown Features (v1.8.0)
- **Auto Table of Contents**: Generate TOC from `[TOC]` placeholder
- **Emoji Support**: Convert emoji codes (`:smile:`, `:rocket:`) to Unicode
- **Code Diff Highlighting**: Green for added (+), red for removed (-) lines
- **Admonitions/Callouts**: Styled boxes for notes, tips, warnings, danger
  - 5 types: `note`, `info`, `tip`, `warning`, `danger`
  - Colored borders and Unicode icons (ℹ️ 💡 ✅ ⚠️ 🚫)

#### 🔬 Raw Data View (v1.9.0) - Developer Tool
- **Split-View Panel**: Markdown source (left) and generated HTML (right) side-by-side
- **Flicker-Free Row Highlighting**:
  - Mouse-over highlighting (light) for easy line tracking
  - Cursor line highlighting (strong, very visible) for current selection
  - Zero flickering with single paint cycle rendering
- **Integrated Line Numbers**:
  - Professional gutter display (50px width, right-aligned)
  - Perfect scroll synchronization
  - Theme-aware coloring
- **Quick Access**: F12 keyboard shortcut or StatusBar button (file-text icon)
- **Theme-Aware**: Adapts to all 4 themes (Dark, Light, Solarized, Draeger)
- **State Persistence**: Remembers visibility and splitter position
- **Read-Only Mode**: Stays a viewer, not an editor
- **Instant Toggle**: <1ms response time
- **Use Cases**:
  - Debug Markdown rendering issues
  - Learn how Markdown translates to HTML
  - Inspect generated HTML for custom CSS
  - Educational tool for Markdown beginners

### Windows Integration
- ✅ **Double-click** .md files → opens in viewer
- ✅ **Right-click context menu** ("Open with Markdown Viewer")
- ✅ **"Open With" dialog** integration
- ✅ **"Send To" menu** integration
- ✅ **File open dialog** when started without arguments

### Properties
- 🚀 **Fast**: 3.3 MB single-file executable
- 📦 **Portable**: No installation required
- 🔒 **No admin rights**: Registry entries in HKCU only
- 🧹 **Clean uninstall**: `--uninstall` removes everything
- 🌍 **Multi-language**: 8 languages supported
- 🎨 **Themeable**: 4 built-in themes + custom themes
- 🧪 **Well-tested**: 283 passing unit tests (v1.9.0)

## Quick Start

### Download & Run

1. **Download** the latest release: [MarkdownViewer.exe](https://github.com/nobiehl/mini-markdown-viewer/releases/download/v1.9.1/MarkdownViewer.exe) (3.3 MB)
2. **Run** directly - no installation needed!

### Option 1: File Open Dialog
```bash
.\MarkdownViewer.exe
# Opens dialog to select a .md file
```

### Option 2: Open File Directly
```bash
.\MarkdownViewer.exe README.md
```

### Option 3: Windows Explorer Integration (Recommended)
```bash
# Install all integration points
.\MarkdownViewer.exe --install

# Then: Double-click .md files in Explorer
# Or: Right-click → "Open with Markdown Viewer"
# Or: Right-click → "Send to" → "Markdown Viewer"
```

## Installation & Uninstallation

### Install (recommended)
```bash
.\MarkdownViewer.exe --install
```

**What gets installed:**
- Default program for .md files (double-click)
- Context menu entry "Open with Markdown Viewer"
- Entry in "Open With" dialog
- Shortcut in "Send To" menu

**No admin rights required!** All entries in `HKEY_CURRENT_USER`

### Uninstall
```bash
.\MarkdownViewer.exe --uninstall
```

Removes all registry entries and shortcuts. The executable remains and can be deleted manually.

## Mathematical Formulas

Render beautiful mathematical equations using LaTeX syntax powered by KaTeX.

### Inline Math

Use single dollar signs for inline formulas:

```markdown
The famous equation $E = mc^2$ was discovered by Einstein.
Pythagorean theorem: $a^2 + b^2 = c^2$
```

**Renders as:** The famous equation $E = mc^2$ was discovered by Einstein.

### Display Math

Use double dollar signs for centered display equations:

```markdown
$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$
```

**Renders as a centered, formatted equation.**

### Supported Features

- **Fractions:** `$\frac{a}{b}$`
- **Superscripts/Subscripts:** `$x^2$`, `$x_i$`
- **Greek Letters:** `$\alpha$`, `$\beta$`, `$\gamma$`, `$\pi$`, `$\sigma$`
- **Square Roots:** `$\sqrt{x}$`, `$\sqrt[3]{8}$`
- **Summations:** `$\sum_{i=1}^{n} i$`
- **Integrals:** `$\int_a^b f(x) dx$`
- **Matrices:** `$\begin{pmatrix} a & b \\ c & d \end{pmatrix}$`
- **Limits:** `$\lim_{x \to \infty} \frac{1}{x} = 0$`
- **Derivatives:** `$\frac{d}{dx}$`, `$\frac{\partial f}{\partial x}$`

**See `samples/math-examples.md` and `samples/test-math.md` for comprehensive examples!**

## Diagram Support

### Mermaid Diagrams

Supports all Mermaid diagram types:

````markdown
```mermaid
graph TD
    A[Start] --> B{Decision}
    B -->|Yes| C[Success]
    B -->|No| D[Retry]
```
````

**Available types:**
- `graph` / `flowchart` - Flow diagrams
- `sequenceDiagram` - Sequence diagrams
- `classDiagram` - Class diagrams
- `stateDiagram` - State diagrams
- `erDiagram` - Entity-relationship diagrams
- `gantt` - Gantt charts
- `pie` - Pie charts
- `gitGraph` - Git graphs

### PlantUML Diagrams

Uses public PlantUML server for rendering:

````markdown
```plantuml
@startuml
Alice -> Bob: Hello
Bob --> Alice: Hi!
@enduml
```
````

**Available types:**
- Class Diagrams
- Sequence Diagrams
- Use Case Diagrams
- Activity Diagrams
- Component Diagrams
- State Diagrams
- Object Diagrams
- Deployment Diagrams

**See `test-diagrams.md` for examples!**

## Command-Line Options

```bash
# Open file
MarkdownViewer.exe <file.md>

# Open with debug logging
MarkdownViewer.exe <file.md> --log-level Debug

# Check for updates
MarkdownViewer.exe --update

# Install Windows Explorer integration
MarkdownViewer.exe --install

# Uninstall Windows Explorer integration
MarkdownViewer.exe --uninstall

# Show version
MarkdownViewer.exe --version

# Show help
MarkdownViewer.exe --help
```

### Automatic Updates

MarkdownViewer automatically checks for updates once every 7 days on first start (non-blocking background check). You can also manually check for updates:

```bash
# Manual update check
MarkdownViewer.exe --update

# Update check happens automatically on normal start
MarkdownViewer.exe README.md  # Will check for updates if not checked in the last 7 days
```

**Update Behavior:**
- Checks GitHub Releases API for new versions (once every 7 days)
- Silent background check (does not block UI)
- Automatic retry on failure (403 rate limit, network errors)
- Shows dialog if update is available with release notes
- Download to `pending-update.exe` in app directory
- Installation on next start with automatic backup and rollback
- Last check tracked in `logs/last-update-check.txt` (only on success)

**Test Mode (Development):**
```bash
# Test update scenarios without real releases
MarkdownViewer.exe --test-update --test-scenario update-available
MarkdownViewer.exe --test-update --test-scenario no-update

# Run all test scenarios
.\test-update.ps1
```

### Logging

Logs are stored in `./logs/viewer-YYYYMMDD.log` with daily rotation (7 days retention).

**Log Levels:**
- `Debug` - Verbose logging (all operations, useful for troubleshooting)
- `Information` - Normal logging (default, key operations only)
- `Warning` - Only warnings and errors
- `Error` - Only errors

**Example:**
```bash
# Production use (less verbose)
MarkdownViewer.exe document.md

# Development/Debugging (detailed logs)
MarkdownViewer.exe document.md --log-level Debug
```

## Technology Stack

- **Language**: C# 12 (.NET 8 Managed Code)
- **UI Framework**: Windows Forms (WinForms)
- **Rendering**: WebView2 (Edge Chromium)
- **Markdown Parser**: Markdig 0.37.0 with Mathematics extension
- **Math Rendering**: KaTeX 0.16.9 (CDN)
- **Logging**: Serilog 4.0.0 with rolling file sink
- **Syntax Highlighting**: Highlight.js 11.9.0 (CDN)
- **Diagrams & Charts**:
  - Mermaid.js 10 (CDN)
  - PlantUML Server (plantuml.com)
  - Chart.js 4.4.1 (CDN)
- **Build**: Single-file deployment (.NET 8 Runtime required)

## Project Structure

```
mini-markdown-viewer/
├── markdown-viewer/
│   └── MarkdownViewer/
│       ├── Program.cs              # Entry point, CLI handling, registry
│       ├── MainForm.cs             # Main window, WebView2, rendering
│       └── MarkdownViewer.csproj   # Project configuration
├── test-diagrams.md                # Test file with Mermaid & PlantUML
└── README.md                       # This file
```

## Build from Source

### Prerequisites
- .NET 8 SDK
- Windows 10/11
- WebView2 Runtime (preinstalled on Win10/11)

### Build Command
```bash
cd markdown-viewer/MarkdownViewer

# Create single-file executable
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single

# Output: ../../bin-single/MarkdownViewer.exe
```

### Development Build
```bash
dotnet build -c Debug
dotnet run -- test-diagrams.md
```

## Architecture

### Program.cs
- **Entry point**: `Main(string[] args)`
- **CLI argument handling**: `--install`, `--uninstall`, `--help`, `--version`
- **Registry integration**: `InstallFileAssociation()`, `UninstallFileAssociation()`
- **Shortcut creation**: `CreateShortcut()` for "Send To" menu

### MainForm.cs
- **WebView2 initialization**: Custom cache folder (`.cache`)
- **Markdown-to-HTML conversion**: `ConvertMarkdownToHtml()` with Mathematics extension
- **HTML template**: Embedded CSS, KaTeX, Highlight.js, Mermaid.js, PlantUML rendering
- **JavaScript link interceptor**: Captures link clicks via `window.chrome.webview.postMessage()`
- **Live reload**: `FileSystemWatcher` monitors file changes
- **Link navigation**: Navigate between .md files, anchor support, external links in browser
- **Logging**: Serilog with configurable log levels and rolling daily files

### Markdown Rendering Pipeline
```
.md File
  ↓
Markdig (with advanced extensions + Mathematics)
  ↓
HTML String
  ↓
Embedded CSS + Scripts (KaTeX, Highlight.js, Mermaid.js, Chart.js)
  ↓
WebView2.NavigateToString()
  ↓
Client-side processing:
  - KaTeX: Renders mathematical formulas ($...$ and $$...$$)
  - Mermaid: Renders diagrams directly in browser
  - PlantUML: Replaces code block with <img> from server
  - Chart.js: Renders interactive charts from JSON config
  - Highlight.js: Syntax highlighting for code
  - Copy buttons: Dynamically added to code blocks
```

## Registry Entries (after --install)

All entries in `HKEY_CURRENT_USER` (no admin rights required):

```
HKCU\Software\Classes\.md
  (Default) = "MarkdownViewer.Document"

HKCU\Software\Classes\MarkdownViewer.Document
  (Default) = "Markdown Document"

HKCU\Software\Classes\MarkdownViewer.Document\shell\open\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"

HKCU\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer
  (Default) = "Open with Markdown Viewer"
  Icon = "C:\path\to\MarkdownViewer.exe"

HKCU\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"

HKCU\Software\Classes\Applications\MarkdownViewer.exe
  FriendlyAppName = "Markdown Viewer"

HKCU\Software\Classes\Applications\MarkdownViewer.exe\SupportedTypes
  .md = ""
  .markdown = ""

HKCU\Software\Classes\Applications\MarkdownViewer.exe\shell\open\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

Plus shortcut in: `%APPDATA%\Microsoft\Windows\SendTo\Markdown Viewer.lnk`

## FAQ

**Q: Do I need .NET installed?**
A: Yes, .NET 8 Runtime is required. Pre-installed on most modern Windows systems.

**Q: Do diagrams work offline?**
A: Mermaid works offline (after first CDN load). PlantUML requires internet connection to server.

**Q: Can I add custom styles?**
A: Yes, in `MainForm.cs` → `ConvertMarkdownToHtml()` → `<style>` section.

**Q: Why WebView2?**
A: WebView2 uses Edge Chromium engine, is preinstalled on Windows 10/11, and provides perfect HTML/CSS/JS rendering.

**Q: Can I rename the executable?**
A: Yes, but run `--install` again afterwards so registry entries reference the correct path.

## Documentation

### User Documentation
- [User Guide](docs/USER-GUIDE.md) - Complete feature guide with examples
- [Changelog](docs/CHANGELOG.md) - Version history and release notes

### Release Notes
- [v1.5.2](docs/RELEASE-NOTES-v1.5.2.md) - Update check retry fix + 7-day interval
- [v1.5.0](docs/RELEASE-NOTES-v1.5.0.md) - Testing & Documentation
- [v1.4.0](docs/RELEASE-NOTES-v1.4.0.md) - Navigation + Search
- [v1.3.0](docs/RELEASE-NOTES-v1.3.0.md) - Localization + StatusBar
- [v1.2.0](docs/RELEASE-NOTES-v1.2.0.md) - Themes + Architecture

### Developer Documentation
- [Development Guide](docs/DEVELOPMENT.md) - Technical documentation for developers
- [Architecture](docs/ARCHITECTURE.md) - System architecture and design patterns
- [Deployment Guide](docs/DEPLOYMENT-GUIDE.md) - Build and deployment instructions
- [Testing Checklist](docs/TESTING-CHECKLIST.md) - Complete test scenarios

### Reference
- [Glossary](docs/GLOSSARY.md) - Terms, classes, and concepts
- [Roadmap](docs/ROADMAP.md) - Feature roadmap
- [Process Model](docs/PROCESS-MODEL.md) - Development workflow

### Technical Details
- [Update Mechanism](docs/UPDATE-MECHANISMUS-DOKUMENTATION.md) - Complete update system documentation with diagrams
- [Update Interval Fix](docs/UPDATE-INTERVALL-FIX.md) - 7-day interval explanation
- [GitHub Token Support](docs/GITHUB-TOKEN-SUPPORT.md) - Optional token authentication

## License

MIT License - see LICENSE file for details.

## Links

- [Markdig](https://github.com/xoofx/markdig) - Markdown parser
- [Mermaid.js](https://mermaid.js.org/) - Diagram library
- [PlantUML](https://plantuml.com/) - UML diagrams
- [Highlight.js](https://highlightjs.org/) - Syntax highlighting
- [WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) - Edge Chromium rendering

---

**Built with Claude Code** 🤖
