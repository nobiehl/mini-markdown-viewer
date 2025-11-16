# User Guide - MarkdownViewer v1.12.0

Complete guide for using MarkdownViewer with all features.

---

## Table of Contents
1. [Installation](#installation)
2. [Opening Files](#opening-files)
3. [Remote Markdown Files](#remote-markdown-files)
4. [Themes](#themes)
5. [Languages](#languages)
6. [Navigation](#navigation)
7. [Search](#search)
8. [Keyboard Shortcuts](#keyboard-shortcuts)
9. [Release Notes & Help](#release-notes--help)
10. [Settings](#settings)

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

## Remote Markdown Files

Open and view Markdown files directly from URLs - perfect for viewing documentation from GitHub, GitLab, Bitbucket, and other Git hosting platforms.

### How to Use

#### Method 1: Click on .md Links
- Click on any `.md` link in your browser or other applications
- The file opens directly in MarkdownViewer
- Example: Click on README.md links in GitHub repositories

#### Method 2: Paste URLs
- Copy any URL to a Markdown file
- Paste into the file open dialog
- Works with direct raw URLs or platform-specific URLs

### Supported Git Platforms

MarkdownViewer automatically converts platform-specific URLs to raw content URLs:

| Platform | Example URL Format | Converted To |
|----------|-------------------|--------------|
| **GitHub** | `github.com/.../blob/main/README.md` | `raw.githubusercontent.com/.../main/README.md` |
| **GitLab** | `gitlab.com/.../-/blob/main/README.md` | `gitlab.com/.../-/raw/main/README.md` |
| **Bitbucket** | `bitbucket.org/.../src/main/README.md` | `bitbucket.org/.../raw/main/README.md` |
| **Gitea/Forgejo** | `.../src/branch/main/README.md` | `.../raw/branch/main/README.md` |

### Features

- **Automatic URL Conversion** - No need to manually find the raw URL
- **Navigation History** - Back/Forward buttons work with remote files
- **Remote Indicator** - Title shows "(Remote)" for clarity
- **Temporary Storage** - Files cached in `%TEMP%/MarkdownViewer/` for fast re-access
- **Link Following** - Click on relative links within remote files to navigate

### Example Use Cases

1. **Documentation Review**
   - Open project README.md from GitHub
   - Review documentation without cloning repository
   - Navigate through multiple documentation files

2. **Code Review**
   - View CHANGELOG.md from releases
   - Check contribution guidelines (CONTRIBUTING.md)
   - Review architectural documentation

3. **Learning**
   - Read tutorials from repositories
   - Browse documentation projects
   - Explore open-source project docs

### Limitations

- Requires internet connection
- 30-second timeout for downloads
- Large files (>10MB) may take longer to load
- No authentication support (public files only)

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
1. Look at the **StatusBar** at the bottom of the window
2. **Select Language** from the language dropdown
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
| **F12** | Toggle Raw Data View (developer tool) |

---

## Release Notes & Help

### Info Button (New in v1.12.0)

The Info button in the StatusBar provides quick access to important information:

- **Click the Info button** (ℹ️ icon) in the StatusBar
- Shows release notes for the current version
- Content loaded directly from CHANGELOG.md
- Stay informed about new features and improvements

### Help Button

- **Click the Help button** (? icon) in the StatusBar
- Opens online documentation
- Provides context-sensitive help
- Links to GitHub repository

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

**Version:** v1.12.0
**Last Updated:** 2025-11-16
