# User Guide - MarkdownViewer v1.5.2

Complete guide for using MarkdownViewer with all features.

---

## Table of Contents
1. [Installation](#installation)
2. [Opening Files](#opening-files)
3. [Themes](#themes)
4. [Languages](#languages)
5. [Navigation](#navigation)
6. [Search](#search)
7. [Keyboard Shortcuts](#keyboard-shortcuts)
8. [Settings](#settings)

---

## Installation

### Quick Install
```bash
MarkdownViewer.exe --install
```

**What this does:**
- Registers .md file association (double-click opens in viewer)
- Adds "Open with Markdown Viewer" to right-click menu
- Adds entry to "Open With" dialog
- Adds shortcut to "Send To" menu

**No admin rights required!** All registry entries in `HKEY_CURRENT_USER`.

### Uninstall
```bash
MarkdownViewer.exe --uninstall
```

Removes all registry entries and shortcuts.

---

## Opening Files

### Method 1: Double-Click (After `--install`)
- Double-click any .md file in Windows Explorer
- Opens directly in MarkdownViewer

### Method 2: Drag & Drop
- Drag .md file onto MarkdownViewer.exe

### Method 3: Command Line
```bash
MarkdownViewer.exe myfile.md
```

### Method 4: File Dialog
```bash
MarkdownViewer.exe
```
Opens file selection dialog.

---

## Themes

**4 Built-in Themes:**
- **Dark** - VS Code-inspired dark theme
- **Solarized Light** - Eye-friendly light theme
- **Dräger** - Corporate theme (blue/green)
- **Standard** - Enhanced modern look

### Switch Theme
1. **Right-click** anywhere in the window
2. Select theme from context menu
3. Theme applies instantly (no restart)

### Custom Themes
1. Create `Themes/mytheme.json` next to exe
2. Copy structure from existing theme file
3. Customize colors
4. Restart viewer → theme appears in menu

---

## Languages

**8 Supported Languages:**
- English
- Deutsch (German)
- Монгол (Mongolian)
- Français (French)
- Español (Spanish)
- 日本語 (Japanese)
- 简体中文 (Chinese Simplified)
- Русский (Russian)

### Switch Language
1. **Enable StatusBar** (if hidden):
   - Edit `%APPDATA%\MarkdownViewer\settings.json`
   - Set `"UI": {"StatusBar": {"Visible": true}}`
2. **Select Language** from StatusBar dropdown
3. Language changes instantly

---

## Navigation

Navigate between markdown files with back/forward history.

### Enable Navigation Bar
Edit `%APPDATA%\MarkdownViewer\settings.json`:
```json
{
  "UI": {
    "NavigationBar": {
      "Visible": true
    }
  }
}
```

### Use Navigation
- **Alt+Left** - Go back
- **Alt+Right** - Go forward
- **Click Back Button** (if NavigationBar visible)
- **Click Forward Button** (if NavigationBar visible)

**Works with:**
- Internal .md file links
- Anchor navigation within same file

---

## Search

Find text within current document with real-time highlighting.

### Open Search
- Press **Ctrl+F**
- Search bar appears at top
- Type search term → highlights appear instantly

### Navigate Matches
- **F3** - Next match
- **Shift+F3** - Previous match
- **Enter** (in search box) - Next match
- **Shift+Enter** (in search box) - Previous match

### Close Search
- Press **Esc** (in search box)
- Click **✕** button
- Highlights clear automatically

### Visual Feedback
- **Yellow** - All matches
- **Orange** - Current match
- **Counter** - "X of Y" or "No results"

---

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| **Ctrl+F** | Open search |
| **F3** | Next search result |
| **Shift+F3** | Previous search result |
| **Alt+Left** | Navigate back |
| **Alt+Right** | Navigate forward |
| **Esc** | Close search |
| **Ctrl+Mouse Wheel** | Zoom in/out (WebView2) |
| **F5** | Reload file |

---

## Settings

Settings stored in `%APPDATA%\MarkdownViewer\settings.json`

### Default Settings
```json
{
  "Version": "1.5.0",
  "Language": "system",
  "Theme": "standard",
  "UI": {
    "StatusBar": {
      "Visible": false
    },
    "NavigationBar": {
      "Visible": false
    },
    "Search": {
      "CaseSensitive": false,
      "WholeWords": false
    }
  },
  "Updates": {
    "CheckOnStartup": true,
    "CheckIntervalDays": 1,
    "AutoDownload": false
  }
}
```

### Common Customizations

#### Enable StatusBar
```json
{
  "UI": {
    "StatusBar": {
      "Visible": true
    }
  }
}
```

#### Enable NavigationBar
```json
{
  "UI": {
    "NavigationBar": {
      "Visible": true
    }
  }
}
```

#### Change Theme
```json
{
  "Theme": "dark"
}
```
Options: `"standard"`, `"dark"`, `"solarized"`, `"draeger"`

#### Change Language
```json
{
  "Language": "de"
}
```
Options: `"en"`, `"de"`, `"mn"`, `"fr"`, `"es"`, `"ja"`, `"zh"`, `"ru"`, `"system"`

#### Disable Auto-Updates
```json
{
  "Updates": {
    "CheckOnStartup": false
  }
}
```

---

## Troubleshooting

### File doesn't open
- Check file exists and has .md extension
- Check logs in `logs/viewer-YYYY-MM-DD.log`

### WebView2 Error
- Install Microsoft Edge WebView2 Runtime
- Download from: https://developer.microsoft.com/microsoft-edge/webview2/

### Search not working
- First search requires internet (loads mark.js from CDN)
- After first load, works offline
- Check network connection for first use

### Theme not applying
- Check `Themes/` folder exists next to exe
- Verify theme JSON file is valid
- Check logs for errors

### Language not changing
- Enable StatusBar first
- Restart if StatusBar not visible
- Check settings.json for correct language code

---

## Tips & Tricks

### Portable Installation
- Copy MarkdownViewer.exe to USB drive
- Copy `Themes/` folder alongside
- Settings save to `%APPDATA%` on each PC

### Multiple Themes
- Create multiple theme JSON files in `Themes/`
- All appear in right-click menu
- Name determines menu order (alphabetical)

### Quick File Switching
- Open file dialog (no args)
- Navigate with Alt+Left/Right between files
- Search across files with Ctrl+F in each

### Performance
- Large files (>10MB) may render slower
- Disable live reload for very large files (edit `FileWatcher` settings)
- Close unused instances to save memory

---

## Getting Help

- **Issues**: https://github.com/nobiehl/mini-markdown-viewer/issues
- **Docs**: https://github.com/nobiehl/mini-markdown-viewer
- **Logs**: `logs/viewer-YYYY-MM-DD.log` next to exe

---

**Version:** v1.5.0
**Last Updated:** 2025-11-06
