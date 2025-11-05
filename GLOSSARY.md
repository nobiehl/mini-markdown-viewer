# Glossary - MarkdownViewer

Definitions of all terms, classes, services, and concepts used in MarkdownViewer.

**Alphabetical Order**
**Last Updated:** 2025-11-05

---

### AppSettings
Data model for application settings stored in JSON format at %APPDATA%/MarkdownViewer/settings.json.

**File:** Models/AppSettings.cs
**Used by:** SettingsService
**Format:** JSON

---

### GitHubRelease
Model class representing a GitHub release from the API. Contains tag_name, name, body (release notes), assets, etc.

**File:** Models/GitHubRelease.cs
**Used by:** UpdateChecker, UpdateService
**API:** GitHub REST API v3

---

### StatusBar
UI component at the bottom of the window showing status icons (update, explorer registration, language, info, help). Hidden by default.

**Component:** WinForms StatusStrip
**Manager:** UI/StatusBarManager.cs
**Toggle:** Ctrl+B, Context Menu

---

### Theme
Visual appearance definition containing colors for both Markdown rendering and WinForms UI. Stored as JSON files in Themes/ folder.

**File:** Models/Theme.cs
**Definitions:** Themes/*.json (dark, solarized, draeger, standard)
**Service:** Services/ThemeService.cs

---

### UpdateChecker
Service that checks GitHub for new releases, compares versions, downloads updates, and applies them safely with backup/rollback.

**File:** UpdateChecker.cs → Services/UpdateService.cs (refactored in v1.2.0)
**Methods:** CheckForUpdatesAsync, DownloadUpdateAsync, ApplyPendingUpdate
**Test Mode:** Supports local JSON mock data

---


### SettingsService
Service for loading and saving application settings from/to JSON file. Provides ISettingsService interface for dependency injection and testing.

**File:** Services/SettingsService.cs
**Storage:** %APPDATA%/MarkdownViewer/settings.json
**Used by:** MainWindow (future implementation)
**Methods:** Load(), Save(), GetSettingsPath()

---

### ThemeService
Service for managing visual themes. Loads theme definitions from JSON files and applies colors to both WinForms UI controls and WebView2 markdown rendering via CSS injection.

**File:** Services/ThemeService.cs
**Themes folder:** Themes/*.json
**Used by:** MainWindow (future implementation)
**Methods:** LoadTheme(), ApplyThemeAsync(), GetAvailableThemes(), GetCurrentTheme()

---

### MarkdownColors
Data model containing color definitions for markdown content rendering. Part of Theme model. Includes 10 color properties for background, foreground, code blocks, links, headings, blockquotes, tables, and inline code.

**File:** Models/Theme.cs
**Properties:** Background, Foreground, CodeBackground, LinkColor, HeadingColor, BlockquoteBorder, TableHeaderBackground, TableBorder, InlineCodeBackground, InlineCodeForeground
**Applied by:** ThemeService via CSS injection to WebView2

---

### UiColors
Data model containing color definitions for WinForms UI elements. Part of Theme model. Includes 7 color properties for form, controls, status bar, and menus.

**File:** Models/Theme.cs
**Properties:** FormBackground, ControlBackground, ControlForeground, StatusBarBackground, StatusBarForeground, MenuBackground, MenuForeground
**Applied by:** ThemeService via ColorTranslator.FromHtml()

---

### UpdateInfo
Model class representing the result of an update check operation. Contains information about update availability, version numbers, release notes, and download URL.

**File:** Models/GitHubRelease.cs
**Used by:** UpdateChecker, UpdateService (future)
**Properties:** UpdateAvailable, LatestVersion, CurrentVersion, ReleaseNotes, DownloadUrl, FileSize, Error, IsPrerelease

---
