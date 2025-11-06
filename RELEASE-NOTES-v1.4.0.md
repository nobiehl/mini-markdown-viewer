# Release Notes - MarkdownViewer v1.4.0

**Release Date:** 2025-11-06
**Type:** Feature Release
**Previous Version:** v1.3.0

---

## Overview

Version 1.4.0 adds **Navigation** with back/forward support and **In-Page Search** with highlighting. These features enhance document exploration and content discovery within markdown files.

---

## New Features

### 🧭 Feature 1.4.1: Navigation

**WebView2-based history navigation:**
- **Back/Forward Buttons** - Navigate through document history
- **Keyboard Shortcuts** - Alt+Left (back), Alt+Right (forward)
- **Auto-enable/disable** - Buttons reflect navigation state
- **Navigation Bar** - Optional toolbar (hidden by default)

**Technical Implementation:**
- NavigationManager.cs (107 lines) - Manages WebView2 history
- NavigationBar.cs (161 lines) - ToolStrip with back/forward buttons
- Event-driven state updates
- Localized tooltips

**Enable Navigation Bar:**
```json
// %APPDATA%\MarkdownViewer\settings.json
{
  "UI": {
    "NavigationBar": {
      "Visible": true
    }
  }
}
```

---

### 🔍 Feature 1.4.2: In-Page Search

**mark.js-powered search with highlighting:**
- **Search Bar** - Ctrl+F opens search toolbar
- **Real-time Highlighting** - Yellow for all matches, orange for current
- **Match Navigation** - F3 (next), Shift+F3 (previous), Enter, Shift+Enter
- **Results Counter** - "X of Y" or "No results"
- **Smooth Scrolling** - Current match centered in view

**Keyboard Shortcuts:**
- **Ctrl+F** - Open search bar
- **F3** - Next match
- **Shift+F3** - Previous match
- **Enter** (in search box) - Next match
- **Shift+Enter** (in search box) - Previous match
- **Esc** (in search box) - Close search

**Technical Implementation:**
- SearchManager.cs (339 lines) - mark.js integration with CDN loading
- SearchBar.cs (247 lines) - Search toolbar with debounced input
- JavaScript injection for search operations
- WebMessage-based communication for results
- Custom CSS for highlighting

---

## Improvements

### Architecture
- Added NavigationManager and SearchManager to Core layer
- Added NavigationBar and SearchBar to UI layer
- ProcessCmdKey override for keyboard shortcuts
- Event-driven search results updates

### User Experience
- Non-blocking CDN loading for mark.js library
- 300ms debounce on search input to reduce lag
- Visual feedback for current match vs other matches
- Keyboard-first workflow for both features

### Localization
- 8 new resource strings (NavigationBack, NavigationForward, Search*)
- Full localization support for all UI elements
- Fallback to English for missing translations

---

## Technical Details

### New Files
```
Core/
├── NavigationManager.cs (107 lines)
└── SearchManager.cs (339 lines)

UI/
├── NavigationBar.cs (161 lines)
└── SearchBar.cs (247 lines)
```

### Modified Files
- MainForm.cs (+120 lines, now 763 lines total)
- Program.cs (version updated to 1.4.0)
- AppSettings.cs (version updated to 1.4.0)
- Resources/Strings.resx (+8 resource strings)

### Dependencies
- mark.js v8.11.1 (loaded from CDN on demand)
- No additional NuGet packages required

### Build Metrics
- **Total lines added:** ~1,010 lines
- **New classes:** 4 (NavigationManager, NavigationBar, SearchManager, SearchBar)
- **Git commits:** 2 (8a69963, d9d31ea)
- **Build status:** ✅ Success (0 errors, 54 nullable warnings)

### Binary Size
- `MarkdownViewer.exe`: ~2.0 MB (no size increase)
- `Themes/` folder: 4 JSON files (~2.7 KB total)

---

## Upgrade Instructions

### From v1.3.0 to v1.4.0

1. **Replace executable:**
   ```
   - Copy new MarkdownViewer.exe to your installation folder
   - Existing settings.json will be preserved
   ```

2. **Settings changes:**
   - New setting: `UI.NavigationBar.Visible` (default: false)
   - New setting: `UI.Search.*` (case-sensitive, whole-words)
   - All v1.3.0 settings remain compatible

3. **No breaking changes:**
   - All v1.3.0 settings still work
   - Existing theme files compatible
   - No manual migration required

---

## Known Limitations

1. **Navigation Bar Hidden by Default**
   - Requires manual activation in settings.json
   - Keyboard shortcuts work regardless of visibility

2. **Search Bar Appearance**
   - Simple ToolStrip UI (no fancy animations)
   - Search history not persisted

3. **mark.js CDN Dependency**
   - Requires internet connection for first search
   - Library cached after first load
   - Offline fallback: search still works after initial load

4. **Resource Strings**
   - Navigation and Search strings only in English base
   - Other languages fall back to English
   - Full translation pending

---

## Compatibility

- **OS:** Windows 10/11 (x64)
- **Runtime:** .NET 8.0 (included in single-file deployment)
- **WebView2:** Microsoft Edge WebView2 Runtime (required)
- **Settings:** Compatible with v1.3.0 settings.json
- **Themes:** Compatible with all v1.2.0+ theme files

---

## What's Next?

### Planned for v1.5.0
- Integration testing for all workflows
- Performance optimization (startup time, large files)
- Documentation completion (USER-GUIDE.md, THEME-GUIDE.md)
- Production-ready polish
- Final release preparation

### Roadmap
- v1.5.0: Polish + Testing + Documentation
- v1.6.0+: Additional features (Explorer panel, custom shortcuts, etc.)

---

## Credits

**Development:** Nicolas Biehl
**GitHub:** https://github.com/nobiehl/mini-markdown-viewer
**License:** MIT

---

## Feedback & Issues

Found a bug or have a feature request?
- GitHub Issues: https://github.com/nobiehl/mini-markdown-viewer/issues

---

**Thank you for using MarkdownViewer!** 🎉
