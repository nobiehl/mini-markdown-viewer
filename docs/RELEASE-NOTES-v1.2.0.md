# Release Notes - MarkdownViewer v1.2.0

**Release Date:** 2025-11-06
**Type:** Major Release - Architecture Refactoring
**Status:** Production Ready

---

## 🎯 Highlights

### 🎨 **Theme System** (NEW!)
4 professionally designed themes for both markdown and UI:
- **Dark** - VS Code inspired dark theme
- **Solarized Light** - Elegant light theme without blue tones
- **Dräger** - Corporate theme based on www.draeger.de colors
- **Standard (Enhanced)** - Modern, refined default theme

### 🏗️ **Architecture Refactoring**
Complete architectural overhaul with clean layered design:
- **MainForm.cs**: 735 → 433 lines (41% reduction!)
- **Core Layer**: MarkdownRenderer, FileWatcherManager
- **Services Layer**: SettingsService, ThemeService
- **Models Layer**: AppSettings, Theme, GitHubRelease
- **Dependency Injection**: Clean service architecture

### ⚙️ **Settings System** (NEW!)
- JSON-based settings persistence
- Location: `%APPDATA%/MarkdownViewer/settings.json`
- Theme selection remembered between sessions
- Extensible for future features

---

## 📦 What's New

### Features
- ✅ **4 Complete Themes** with markdown + UI styling
- ✅ **Settings Persistence** - Your preferences are saved
- ✅ **Theme-Aware Rendering** - Markdown adapts to selected theme
- ✅ **Modular Architecture** - Testable, maintainable code

### Technical Improvements
- ✅ **Layered Architecture**: UI → Services → Core → Models
- ✅ **MarkdownRenderer**: Extracted, theme-aware HTML generation
- ✅ **FileWatcherManager**: Event-driven file watching with IDisposable
- ✅ **SettingsService**: JSON persistence with automatic directory creation
- ✅ **ThemeService**: Dynamic theme loading and CSS injection

### Code Quality
- ✅ **41% Code Reduction** in MainForm.cs
- ✅ **Separation of Concerns** - Each class has single responsibility
- ✅ **Dependency Injection** - Services injected via constructor
- ✅ **Clean Event Handling** - Event-driven patterns throughout

---

## 🔧 Technical Details

### Architecture

```
MarkdownViewer/
├── Core/                     # Pure business logic
│   ├── MarkdownRenderer.cs      (336 lines)
│   └── FileWatcherManager.cs    (101 lines)
├── Services/                 # Application services
│   ├── SettingsService.cs       (117 lines)
│   └── ThemeService.cs          (354 lines)
├── Models/                   # Data structures
│   ├── AppSettings.cs           (106 lines)
│   ├── Theme.cs                 (116 lines)
│   └── GitHubRelease.cs         (moved)
├── Configuration/            # Config management
│   └── UpdateConfiguration.cs   (moved)
└── Themes/                   # Theme definitions (JSON)
    ├── dark.json
    ├── solarized.json
    ├── draeger.json
    └── standard.json
```

### Theme Structure

Each theme defines:
- **MarkdownColors** (10 properties): Background, Foreground, CodeBackground, LinkColor, HeadingColor, BlockquoteBorder, TableHeaderBackground, TableBorder, InlineCodeBackground, InlineCodeForeground
- **UiColors** (7 properties): FormBackground, ControlBackground, ControlForeground, StatusBarBackground, StatusBarForeground, MenuBackground, MenuForeground

### Settings Schema

```json
{
  "version": "1.2.0",
  "language": "system",
  "theme": "standard",
  "ui": {
    "statusBar": { "visible": false },
    "navigationBar": { "visible": false },
    "search": { /* ... */ }
  },
  "updates": { /* ... */ },
  "explorer": { /* ... */ },
  "shortcuts": { /* ... */ },
  "navigation": { /* ... */ }
}
```

---

## 📈 Metrics

| Metric | Value |
|--------|-------|
| **MainForm.cs Lines** | 735 → 433 (-302, -41%) |
| **New Core Files** | 2 files, 437 lines |
| **New Service Files** | 2 files, 471 lines |
| **New Model Files** | 2 files, 222 lines |
| **Theme Files** | 4 JSON files |
| **Total Architecture Files** | 12 files |
| **Build Warnings** | ~45 (nullable only) |
| **Build Errors** | 0 ✅ |

---

## 🚀 Installation

### Requirements
- Windows 10/11
- .NET 8.0 Runtime (automatically installed if missing)
- WebView2 Runtime (usually pre-installed on Windows 11)

### Download
Download `MarkdownViewer.exe` from the releases page.

**Important:** The `Themes/` folder must be in the same directory as the executable!

```
your-folder/
├── MarkdownViewer.exe
└── Themes/
    ├── dark.json
    ├── solarized.json
    ├── draeger.json
    └── standard.json
```

### First Launch
1. Run `MarkdownViewer.exe <your-file.md>`
2. Or use Windows Explorer context menu (after `--install`)
3. Default theme: **Standard (Enhanced)**
4. Settings will be created at: `%APPDATA%\MarkdownViewer\settings.json`

---

## 🎨 Theme Previews

### Dark Theme
- Background: `#1e1e1e`
- Foreground: `#d4d4d4`
- Link Color: `#569cd6`
- Perfect for: Late-night coding sessions

### Solarized Light
- Background: `#fdf6e3`
- Foreground: `#657b83`
- Link Color: `#2aa198` (teal, no blue!)
- Perfect for: Easy on the eyes, all-day use

### Dräger Theme
- Background: `#ffffff`
- Foreground: `#003366`
- Link Color: `#0066cc`
- Perfect for: Corporate documentation

### Standard (Enhanced)
- Background: `#ffffff`
- Foreground: `#2c3e50`
- Link Color: `#3498db`
- Perfect for: Clean, modern look

---

## 🔄 Upgrading from v1.1.0

1. **Replace the executable** - Simply overwrite `MarkdownViewer.exe`
2. **Add Themes folder** - Copy the `Themes/` folder next to the exe
3. **Settings are preserved** - Your Windows Explorer integration remains

No breaking changes for users! All v1.1.0 features work exactly as before.

---

## 🐛 Known Issues

- ~45 nullable reference warnings during build (cosmetic, no runtime impact)
- Themes cannot be changed yet via UI (requires manual settings.json edit)
  - Will be addressed in v1.3.0 with StatusBar + Context Menu

---

## 🔮 What's Next (v1.3.0)

- **StatusBar UI** with theme selector
- **8 Languages** localization
- **Context Menu** for quick settings access
- **Live theme switching** without restart

---

## 🙏 Credits

Built with:
- **Markdig 0.37.0** - Markdown parsing
- **WebView2** - HTML rendering
- **Serilog 4.0.0** - Logging
- **Highlight.js 11.9.0** - Syntax highlighting
- **KaTeX 0.16.9** - Math formulas
- **Mermaid.js 10** - Diagrams

---

## 📄 License

See LICENSE file in repository.

---

**🤖 Generated with [Claude Code](https://claude.com/claude-code)**

**Enjoy your new themes! 🎨**
