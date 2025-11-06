# Changelog

All notable changes to MarkdownViewer are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

---

## [1.5.0] - 2025-11-06

### Added
- **Testing Infrastructure**: TESTING-CHECKLIST.md with 85 integration tests
- **Documentation**: USER-GUIDE.md with comprehensive feature documentation
- **Documentation**: Updated README.md with all v1.2.0-v1.5.0 features
- **Version Updates**: All version strings bumped to 1.5.0

### Changed
- **Production Ready**: All features tested and documented
- **Polish**: Code quality, documentation completeness

---

## [1.4.0] - 2025-11-06

### Added
- **Navigation**: Back/forward navigation (Alt+Left/Right)
  - NavigationManager.cs (107 lines) - WebView2 history management
  - NavigationBar.cs (161 lines) - Optional toolbar
  - Keyboard shortcuts for navigation
- **In-Page Search**: Real-time highlighting with mark.js
  - SearchManager.cs (339 lines) - mark.js integration
  - SearchBar.cs (247 lines) - Search toolbar
  - Ctrl+F, F3, Shift+F3, Enter, Esc shortcuts
  - Yellow highlighting for matches, orange for current match
  - Results counter ("X of Y")
- **Resource Strings**: 8 new localization strings for navigation and search

### Changed
- **MainForm.cs**: Added keyboard shortcut handling (ProcessCmdKey override)
- **Binary Size**: Increased to 2.0 MB (from 1.6 MB) due to new features

### Technical
- mark.js v8.11.1 loaded dynamically from CDN
- WebMessage-based communication for search results
- Event-driven architecture for navigation state
- ~1,010 lines of code added

---

## [1.3.0] - 2025-11-06

### Added
- **Localization**: Support for 8 languages
  - English, Deutsch, Монгол, Français, Español, 日本語, 简体中文, Русский
  - 8 complete .resx resource files (~3,200 lines)
  - LocalizationService.cs (220 lines)
  - Instant language switching
  - Fallback to English for missing translations
- **StatusBar**: 5-section status bar (hidden by default)
  - Update status icon (✅/🔄/❌/❓)
  - Explorer registration status (✅📁/❌📁)
  - Language selector dropdown
  - Info button (shows version, theme, settings location)
  - Help button (keyboard shortcuts, features)
  - StatusBarControl.cs (334 lines)
- **Theme Switcher**: Right-click context menu
  - Select from 4 themes instantly
  - Localized theme names
  - Visual feedback (checkmarks)
  - No restart required

### Changed
- **MainForm.cs**: Version 1.3.0, integrated localization and StatusBar
- **Settings**: Added Language setting (default: "system")

### Technical
- ~3,800 lines of code added
- ResourceManager-based localization
- Event-driven language switching

---

## [1.2.0] - 2025-11-06

### Added
- **Theme System**: 4 built-in themes
  - Dark (VS Code-inspired)
  - Solarized Light (eye-friendly)
  - Dräger (corporate)
  - Standard (enhanced)
  - Theme files in Themes/ folder (JSON format)
- **Settings System**: JSON-based configuration
  - AppSettings.cs with nested classes
  - SettingsService.cs for load/save
  - Location: %APPDATA%\MarkdownViewer\settings.json
- **Theme Service**: ThemeService.cs for theme management
  - Load themes from JSON
  - Apply to UI and markdown content
  - Dynamic CSS injection

### Changed
- **Architecture**: Refactored to layered architecture
  - Core/ - Business logic
  - Services/ - Application services
  - UI/ - User interface components
  - Models/ - Data models
  - Configuration/ - Configuration classes
  - Resources/ - Localization resources
  - Themes/ - Theme JSON files
- **MainForm.cs**: Reduced from 735 to 433 lines (41% reduction)
  - Extracted MarkdownRenderer.cs (336 lines)
  - Extracted FileWatcherManager.cs (101 lines)

### Technical
- ~950 lines of code added
- 7 new folders created
- Clean separation of concerns

---

## [1.1.0] - 2025-11-05

### Added
- **Automatic Updates**: Daily update checks
  - Background check (non-blocking)
  - UpdateChecker.cs with retry logic
  - Release notes display
  - Manual check with `--update`
  - Test mode for development (`--test-update`)
- **Update Configuration**: UpdateConfiguration.cs
  - Rate limiting (60 requests/hour)
  - Retry with exponential backoff
  - Network error handling

### Changed
- **Settings**: Added UpdateSettings class

---

## [1.0.3] - 2025-11-05

### Fixed
- **PlantUML Rendering**: Fixed HEX encoding for complex diagrams
- **Version Display**: Added version in window title

---

## [1.0.2] - 2025-11-05

### Fixed
- **PlantUML Diagrams**: Fixed rendering issue with diagram encoding

---

## [1.0.1] - 2025-11-05

### Added
- **Documentation**: English documentation (README, guides)
- **Testing**: Comprehensive test suite

### Fixed
- **PlantUML**: Various diagram rendering fixes

---

## [1.0.0] - 2025-11-05

### Initial Release

#### Core Features
- Markdown rendering (CommonMark)
- Syntax highlighting (Highlight.js)
- Mathematical formulas (KaTeX)
- Mermaid diagrams
- PlantUML diagrams
- Live file reload
- Copy buttons for code blocks
- Link navigation
- Anchor links
- Professional logging (Serilog)

#### Windows Integration
- Double-click .md files
- Right-click context menu
- "Open With" dialog integration
- "Send To" menu integration
- File open dialog
- Command-line arguments

#### Properties
- Single-file executable (1.6 MB)
- Portable (no installation required)
- No admin rights required
- Clean uninstall

---

## Version History Summary

| Version | Date | Type | Key Features |
|---------|------|------|--------------|
| 1.5.0 | 2025-11-06 | Polish | Testing, Documentation |
| 1.4.0 | 2025-11-06 | Feature | Navigation + Search |
| 1.3.0 | 2025-11-06 | Feature | Localization + StatusBar + Theme Switcher |
| 1.2.0 | 2025-11-06 | Major | Themes + Settings + Architecture Refactoring |
| 1.1.0 | 2025-11-05 | Feature | Automatic Updates |
| 1.0.3 | 2025-11-05 | Hotfix | PlantUML HEX encoding fix |
| 1.0.2 | 2025-11-05 | Hotfix | PlantUML rendering fix |
| 1.0.1 | 2025-11-05 | Patch | Documentation + Tests |
| 1.0.0 | 2025-11-05 | Initial | Core features + Windows integration |

---

**Development Model:**
- Following semantic versioning (MAJOR.MINOR.PATCH)
- Developed incrementally with comprehensive documentation
- Each release fully tested before publication
- Commitment to backward compatibility

**Repository:** https://github.com/nobiehl/mini-markdown-viewer
