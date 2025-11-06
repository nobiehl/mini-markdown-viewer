# Release Notes - MarkdownViewer v1.3.0

**Release Date:** 2025-11-06
**Type:** Feature Release
**Previous Version:** v1.2.0

---

## Overview

Version 1.3.0 introduces **multi-language support** with 8 languages, an optional **status bar** for application monitoring, and an intuitive **theme switcher** via right-click menu. This release focuses on internationalization and user experience enhancements.

---

## New Features

### 🌍 Feature 1.3.1: Multi-Language Support

Complete localization infrastructure supporting **8 languages**:

- **English** (en) - Base language
- **Deutsch** (de) - German
- **Монгол** (mn) - Mongolian
- **Français** (fr) - French
- **Español** (es) - Spanish
- **日本語** (ja) - Japanese
- **简体中文** (zh) - Chinese Simplified
- **Русский** (ru) - Russian

**Technical Implementation:**
- 60+ localized UI strings per language
- .NET ResourceManager-based architecture
- Automatic fallback to English for missing translations
- Culture-based language switching
- ISO 639-1 language codes

**Files Added:**
- `Resources/Strings.resx` (English - base)
- `Resources/Strings.de.resx` (German)
- `Resources/Strings.mn.resx` (Mongolian)
- `Resources/Strings.fr.resx` (French)
- `Resources/Strings.es.resx` (Spanish)
- `Resources/Strings.ja.resx` (Japanese)
- `Resources/Strings.zh.resx` (Chinese Simplified)
- `Resources/Strings.ru.resx` (Russian)
- `Services/LocalizationService.cs` (220 lines)

---

### 📊 Feature 1.3.2: Status Bar

Optional status bar with **5 sections** providing real-time application information:

1. **Update Status Icon** (✅/🔄/❌/❓)
   - Visual indicator for update availability
   - Shows: Unknown, Checking, Up-to-date, Update Available, Error

2. **Explorer Registration Icon** (✅📁/❌📁)
   - Shows Windows Explorer integration status
   - Checks HKCU registry for .md file association

3. **Language Selector Dropdown**
   - Instant language switching
   - Shows all 8 supported languages with native names
   - Changes take effect immediately

4. **Info Button** (clickable label)
   - Shows current version, language, and theme
   - Displays settings file location

5. **Help Button** (clickable label)
   - Shows keyboard shortcuts
   - Lists all features
   - GitHub repository link

**Configuration:**
- StatusBar is **hidden by default** (can be enabled in settings)
- Setting: `UI.StatusBar.Visible = false` in `settings.json`
- Location: `%APPDATA%\MarkdownViewer\settings.json`

**Technical Implementation:**
- Custom `StatusBarControl` class (334 lines)
- Event-driven architecture (LanguageChanged, InfoClicked, HelpClicked)
- Full localization support
- Registry integration for Explorer status
- Color-coded status indicators

**Files Added:**
- `UI/StatusBarControl.cs` (334 lines)

---

### 🎨 Feature 1.3.3: Theme Switcher

Intuitive theme switching via **right-click context menu**:

**How to Use:**
1. Right-click anywhere in the window
2. Select a theme from the menu
3. Theme applies instantly (no restart required)

**Available Themes:**
- **Dark** - VS Code-inspired dark theme
- **Solarized Light** - Eye-friendly light theme (no blue component)
- **Dräger** - Corporate theme (based on www.draeger.de)
- **Standard** - Enhanced modern clean look

**Features:**
- Localized theme names in all 8 languages
- Visual feedback (checkmark shows active theme)
- Instant application to both UI and markdown content
- Settings persistence (saved to `settings.json`)
- Live markdown re-rendering with new theme

**Technical Implementation:**
- WinForms ContextMenuStrip
- Dynamic menu generation from available themes
- Event-driven theme switching
- Immediate UI and WebView2 re-rendering

---

## Improvements

### Architecture Enhancements
- Added `LocalizationService` as core service
- Integrated StatusBar into MainForm architecture
- Event-driven design for language and theme changes
- Clean separation of concerns with service layer

### User Experience
- Instant language switching without restart
- Instant theme switching without restart
- Visual feedback for all status indicators
- Link-style clickable labels in status bar
- Tooltips on all status bar items

### Settings Management
- Language preference persisted to `settings.json`
- Theme preference persisted to `settings.json`
- Settings automatically saved on change
- Graceful handling of missing or corrupt settings

---

## Technical Details

### Project Structure Changes
```
MarkdownViewer/
├── Resources/           (NEW - 8 .resx files, ~3200 lines)
│   ├── Strings.resx
│   ├── Strings.de.resx
│   ├── Strings.mn.resx
│   ├── Strings.fr.resx
│   ├── Strings.es.resx
│   ├── Strings.ja.resx
│   ├── Strings.zh.resx
│   └── Strings.ru.resx
├── Services/
│   └── LocalizationService.cs  (NEW - 220 lines)
└── UI/
    └── StatusBarControl.cs     (NEW - 334 lines)
```

### Modified Files
- `MainForm.cs` (+200 lines)
  - Added LocalizationService integration
  - Added StatusBar initialization
  - Added theme context menu
  - Event handlers for language and theme changes
  - Version updated to 1.3.0

- `Program.cs` (version updated)
  - Version constant updated to 1.3.0

### Build Metrics
- **Total lines added:** ~3,800 lines
- **New classes:** 2 (LocalizationService, StatusBarControl)
- **Resource files:** 8 complete .resx files
- **Languages supported:** 8
- **Build status:** ✅ Success (0 errors, ~54 nullable warnings)

### Binary Size
- `MarkdownViewer.exe`: 2.0 MB (single-file deployment)
- `Themes/` folder: 4 JSON files (~2.7 KB total)

---

## Upgrade Instructions

### From v1.2.0 to v1.3.0

1. **Replace executable:**
   ```
   - Copy new MarkdownViewer.exe to your installation folder
   - Existing settings.json will be preserved
   ```

2. **Settings changes:**
   - New setting: `Language` (default: "system")
   - New setting: `UI.StatusBar.Visible` (default: false)
   - Theme setting remains: `Theme` (default: "standard")

3. **Enable Status Bar (optional):**
   - Edit `%APPDATA%\MarkdownViewer\settings.json`
   - Set `"UI": { "StatusBar": { "Visible": true } }`
   - Or let StatusBar remain hidden (default)

4. **No breaking changes:**
   - All v1.2.0 settings remain compatible
   - Existing theme files still work
   - No manual migration required

### First-Time Installation

1. Download `MarkdownViewer.exe` from releases
2. Extract to desired folder (e.g., `C:\Program Files\MarkdownViewer`)
3. Run `MarkdownViewer.exe --install` to register .md file association
4. Settings will be created automatically on first run

---

## Known Limitations

1. **StatusBar Hidden by Default**
   - StatusBar requires manual activation in settings
   - Will be addressed in future UI preferences dialog

2. **Partial Localization**
   - Only StatusBar is fully localized
   - MainForm and Program.cs dialogs still use hardcoded strings (mostly German)
   - Full localization planned for v1.4.0

3. **Update Status Integration**
   - Update status icon shows "Unknown" by default
   - Integration with UpdateChecker pending
   - Planned for v1.4.0

4. **No Keyboard Shortcut for StatusBar Toggle**
   - StatusBar can only be toggled via settings file
   - UI preferences dialog planned for v1.4.0

---

## Compatibility

- **OS:** Windows 10/11 (x64)
- **Runtime:** .NET 8.0 (included in single-file deployment)
- **WebView2:** Microsoft Edge WebView2 Runtime (required)
- **Settings:** Compatible with v1.2.0 settings.json

---

## File Hashes (SHA256)

```
MarkdownViewer.exe: [To be generated on release]
```

---

## What's Next?

### Planned for v1.4.0
- Full UI localization (all dialogs and messages)
- Settings UI dialog for preferences management
- Update status integration with StatusBar
- Navigation bar with breadcrumbs
- Search functionality (Ctrl+F)

### Roadmap
- v1.4.0: Navigation & Search
- v1.5.0: Explorer Panel & Performance

---

## Credits

**Development:** Nicolas Biehl
**GitHub:** https://github.com/nobiehl/mini-markdown-viewer
**License:** MIT

---

## Feedback & Issues

Found a bug or have a feature request?
- GitHub Issues: https://github.com/nobiehl/mini-markdown-viewer/issues
- Email: [Contact via GitHub profile]

---

**Thank you for using MarkdownViewer!** 🎉
