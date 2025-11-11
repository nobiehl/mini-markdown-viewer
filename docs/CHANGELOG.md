# Changelog

All notable changes to MarkdownViewer are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.8.0] - 2025-01-10

### Added
- **Auto Table of Contents**: Generate hierarchical TOC from `[TOC]` placeholder
  - JavaScript-based navigation with smooth scrolling
  - Supports all heading levels (h1-h6)
  - Theme-aware styling
  - Sample: `samples/toc-example.md`
- **Emoji Support**: Convert emoji codes to Unicode emojis
  - Markdig extension: `UseEmojiAndSmiley()`
  - Supports `:smile:`, `:heart:`, `:rocket:`, `:warning:`, `:fire:`, etc.
  - Works in headings, lists, and text
  - Sample: `samples/emoji-example.md`
- **Code Diff Highlighting**: Syntax highlighting for diff code blocks
  - Green background for added lines (+)
  - Red background for removed lines (-)
  - Git diff compatible
  - Sample: `samples/diff-example.md`
- **Admonitions/Callouts**: Styled information boxes
  - 5 types: `note`, `info`, `tip`, `warning`, `danger`
  - Colored borders, backgrounds, and Unicode icons (ℹ️ 💡 ✅ ⚠️ 🚫)
  - Dark theme support with automatic color adjustments
  - Sample: `samples/admonitions-example.md`

### Technical
- Parallel implementation with 3 specialized agents
  - Agent 1: TOC (JavaScript + CSS + Tests)
  - Agent 2: Emoji + Diff (Pipeline + CSS + Tests)
  - Agent 3: Admonitions (CSS + Tests)
  - Result: 0 merge conflicts, 66% time savings (30 min vs 90 min)
- 18 new unit tests (all passing)
- Build: 0 Errors, 0 Warnings
- Binary size: 3.3 MB (unchanged)

### Files Changed
- `MarkdownViewer.Core/Core/MarkdownRenderer.cs`: Extended pipeline, added CSS and JavaScript
- `samples/toc-example.md`, `emoji-example.md`, `diff-example.md`, `admonitions-example.md`: New examples
- `MarkdownViewer.Tests/Tests/Core/MarkdownRendererTests.cs`: 18 new tests

---

## [1.7.4] - 2025-11-09

### Fixed
- **WebView2 Resource Error**: Resolved "resource already in use" error when opening files
  - Fixed race condition in WebView2 initialization
  - Improved error handling during file loading
  - More robust navigation logic

### Changed
- Enhanced logging for WebView2 initialization
- Better error messages for file loading issues

---

## [1.5.2] - 2025-11-06

### Fixed
- **Critical Bug**: Update check retry logic
  - `RecordUpdateCheck()` now only called on successful API response
  - Failed checks (403 rate limit, network errors) no longer block retries
  - Automatic retry on next start until success
  - Enhanced error logging: "Will retry on next start"
  - **Problem**: Timestamp was saved BEFORE API call, preventing retries for 7 days
  - **Impact**: Users never received updates if first check failed

### Changed
- **Update Interval**: Changed from daily to once every 7 days
  - Reduces API calls from ~30/month to ~4/month
  - Prevents GitHub API rate limit issues (60 requests/hour)
  - Optional GitHub token support for power users (5000 requests/hour)

### Documentation
- `UPDATE-MECHANISMUS-DOKUMENTATION.md`: Complete update mechanism with 3 Mermaid diagrams
- `RELEASE-NOTES-v1.5.2.md`: Detailed bug analysis and fix documentation
- `UPDATE-INTERVALL-FIX.md`: 7-day interval explanation
- `GITHUB-TOKEN-SUPPORT.md`: Optional token authentication guide

---

## [1.5.0] - 2025-11-06

### Added
- **Testing Infrastructure**: `TESTING-CHECKLIST.md` with 85 integration tests
- **Documentation**: `USER-GUIDE.md` with comprehensive feature documentation
- **Documentation**: Updated `README.md` with all v1.2.0-v1.5.0 features
- **Version Updates**: All version strings bumped to 1.5.0

### Changed
- **Production Ready**: All features tested and documented
- **Polish**: Code quality improvements, documentation completeness

### Test Categories (85 tests total)
1. First Launch
2. Settings Persistence
3. Explorer Registration
4. Theme Switching
5. Localization (8 languages)
6. Navigation
7. Search
8. Markdown Rendering
9. File Watching
10. Performance
11. Error Handling
12. Command-Line Arguments

---

## [1.4.0] - 2025-11-06

### Added
- **🧭 Navigation**: WebView2-based history navigation
  - Back/Forward buttons with keyboard shortcuts (Alt+Left/Right)
  - Auto-enable/disable based on navigation state
  - Optional NavigationBar (hidden by default)
  - Localized tooltips
  - `NavigationManager.cs` (107 lines)
  - `NavigationBar.cs` (161 lines)

- **🔍 In-Page Search**: mark.js-powered search with highlighting
  - Search bar with Ctrl+F
  - Real-time highlighting (yellow for matches, orange for current)
  - Match navigation: F3 (next), Shift+F3 (previous)
  - Results counter ("X of Y" or "No results")
  - Smooth scrolling to matches
  - `SearchManager.cs` (339 lines)
  - `SearchBar.cs` (247 lines)

### Improved
- Architecture: Added NavigationManager and SearchManager to Core layer
- User Experience: Non-blocking CDN loading, 300ms debounce on search
- Localization: 8 new resource strings for Navigation and Search

### Technical
- ~1,010 lines of code added
- 4 new classes
- mark.js v8.11.1 (loaded from CDN on demand)
- Build: 0 errors, 54 nullable warnings
- Binary size: ~2.0 MB (unchanged)

---

## [1.3.0] - 2025-11-05

### Added
- **🌍 Localization**: Multi-language support
  - 8 languages: English, Deutsch, Монгол, Français, Español, 日本語, 简体中文, Русский
  - Instant language switching via StatusBar dropdown
  - All UI elements fully translated
  - Fallback to English for missing translations
  - `LocalizationService.cs` with .resx resource files

- **📊 StatusBar**: Always-visible status bar at bottom
  - 5 sections: Update status, Explorer registration, Language selector, Info, Help
  - Real-time status updates
  - Integrated theme and language switching
  - Themed icons and styling
  - `StatusBarControl.cs` (390 lines)

### Improved
- Theme switcher: Moved to StatusBar context menu
- Settings: Language preference persisted in `settings.json`
- Architecture: Added localization layer with service pattern

### Technical
- 8 `.resx` resource files (~50 strings each)
- `LocalizationService` with culture management
- StatusBar with ComboBox and ToolStripButtons
- Build: 0 errors, 54 nullable warnings
- Binary size: ~2.0 MB

---

## [1.2.0] - 2025-11-04

### Added
- **🎨 Theme System**: Complete theming support
  - 4 built-in themes: Dark, Solarized Light, Dräger, Standard
  - Theme switcher via right-click context menu
  - Instant theme application (no restart required)
  - JSON-based theme files in `Themes/` folder
  - Settings persistence in `%APPDATA%\MarkdownViewer\settings.json`
  - Custom theme support (add your own JSON files)

### Architecture
- **Layered Architecture**: Organized into logical layers
  - `Core/`: Business logic (MarkdownRenderer, FileWatcher, LinkNavigation)
  - `Services/`: Reusable services (SettingsService, ThemeService)
  - `UI/`: UI components (MainForm)
  - `Models/`: Data models (AppSettings, Theme, GitHubRelease)
  - `Configuration/`: Configuration classes (UpdateConfiguration)

### Improved
- Settings system with JSON persistence
- Theme service with dynamic loading
- Markdown rendering with theme-aware colors
- WebView2 integration with themed backgrounds
- Context menu with theme selection

### Technical
- `ThemeService.cs` for theme management
- `SettingsService.cs` for settings persistence
- 4 theme JSON files (~100 lines each)
- Build: 0 errors, 0 warnings
- Binary size: ~2.0 MB

---

## [1.1.0] - 2025-11-03

### Added
- **Chart.js Integration**: Data visualization support
  - Line, bar, pie, doughnut, radar, polar area charts
  - JSON-based chart configuration in code blocks
  - CDN-loaded library (Chart.js v4.4.1)
  - Sample files demonstrating various chart types

### Improved
- JavaScript error handling for chart rendering
- Sample files with comprehensive chart examples

---

## [1.0.0] - 2025-11-01

### Initial Release

#### Core Features
- **Markdown Rendering**: Full CommonMark support via Markdig
  - Tables, lists, blockquotes
  - Task lists
  - Footnotes
  - Definition lists
  - Abbreviations
  - Custom containers

- **Syntax Highlighting**: Code blocks with Highlight.js
  - 190+ languages supported
  - GitHub theme styling
  - Copy buttons for code blocks

- **Mathematical Formulas**: LaTeX support via KaTeX
  - Inline math: `$E = mc^2$`
  - Display math: `$$\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}$$`
  - Full LaTeX support (matrices, Greek letters, fractions, etc.)

- **Diagrams**: Multiple diagram types
  - **Mermaid**: Flowcharts, sequence, class, state, gantt, ER diagrams
  - **PlantUML**: Class, sequence, use case, activity, component diagrams
  - Online rendering via plantuml.com

- **Live Reload**: FileSystemWatcher for automatic updates
- **Link Navigation**: Navigate between .md files, open external links
- **Anchor Links**: Jump to headings with # anchors
- **Professional Logging**: Serilog with rolling daily logs

#### Windows Integration
- Double-click `.md` files to open in viewer
- Right-click context menu ("Open with Markdown Viewer")
- "Open With" dialog integration
- "Send To" menu integration
- File open dialog when started without arguments

#### Properties
- Single-file executable (2.0 MB)
- Portable (no installation required)
- No admin rights required (HKCU registry only)
- Clean uninstall with `--uninstall`

#### Automatic Updates
- Check for updates once every 7 days
- Manual check with `--update`
- Background check (non-blocking)
- Release notes display
- Safe installation with backup and rollback

---

## Version History Summary

| Version | Date | Type | Key Features |
|---------|------|------|--------------|
| 1.8.0 | 2025-01-10 | Feature | TOC, Emoji, Diff, Admonitions |
| 1.7.4 | 2025-11-09 | Bugfix | WebView2 resource error fix |
| 1.5.2 | 2025-11-06 | Bugfix | Update retry logic fix |
| 1.5.0 | 2025-11-06 | Polish | Testing + Documentation |
| 1.4.0 | 2025-11-06 | Feature | Navigation + Search |
| 1.3.0 | 2025-11-05 | Feature | Localization + StatusBar |
| 1.2.0 | 2025-11-04 | Feature | Theme System + Architecture |
| 1.1.0 | 2025-11-03 | Feature | Chart.js Integration |
| 1.0.0 | 2025-11-01 | Initial | Core Features + Windows Integration |

---

## Semantic Versioning

This project follows [Semantic Versioning](https://semver.org/):

- **MAJOR** version (X.0.0): Incompatible API changes
- **MINOR** version (1.X.0): New features (backward compatible)
- **PATCH** version (1.0.X): Bug fixes (backward compatible)

---

## Upgrade Guide

### From v1.7.x to v1.8.0
- ✅ **Drop-in replacement**: Just replace the `.exe`
- ✅ **No breaking changes**: All settings preserved
- ✅ **New features work immediately**: No configuration needed

### From v1.5.x to v1.8.0
- ✅ **Fully compatible**: Replace `.exe` and you're done
- ✅ **Settings migration**: Automatic on first launch
- ✅ **Theme files**: All compatible

### From v1.0.x to v1.8.0
- ⚠️ **Settings format changed**: Backup old settings if needed
- ✅ **Manual migration**: Old settings will be auto-upgraded
- ✅ **Themes**: Must use new JSON format (old themes incompatible)

---

## Links

- **Repository**: https://github.com/nobiehl/mini-markdown-viewer
- **Issues**: https://github.com/nobiehl/mini-markdown-viewer/issues
- **Releases**: https://github.com/nobiehl/mini-markdown-viewer/releases

---

**Changelog maintained since v1.0.0**
**Last updated: 2025-01-10**
