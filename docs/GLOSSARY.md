# Glossary - MarkdownViewer

Definitions of all terms, classes, services, and concepts used in MarkdownViewer.

**Alphabetical Order**
**Last Updated:** 2025-11-06

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
Service that checks GitHub for new releases once every 7 days, compares versions, downloads updates, and applies them safely with backup/rollback. Implements automatic retry logic for failed checks.

**File:** UpdateChecker.cs
**Methods:** CheckForUpdatesAsync, DownloadUpdateAsync, ApplyPendingUpdate, ShouldCheckForUpdates, RecordUpdateCheck
**Test Mode:** Supports local JSON mock data
**Retry Logic:** Only records timestamp on success, retries on failure (v1.5.2)
**Interval:** 7 days (changed from daily in v1.5.2)

---

### RecordUpdateCheck
Method that saves the current timestamp to logs/last-update-check.txt. Only called on successful update check to enable automatic retry on failure.

**Location:** UpdateChecker.cs
**Called by:** Program.cs:CheckForUpdatesAsync() (after success)
**File:** logs/last-update-check.txt
**Format:** yyyy-MM-dd HH:mm:ss
**Critical:** Only called on successful API response (v1.5.2 fix)

---

### ShouldCheckForUpdates
Method that determines if an update check should be performed. Returns true if no previous check exists or if 7 days have elapsed since last successful check.

**Location:** UpdateChecker.cs
**Returns:** bool
**Logic:** elapsed.TotalDays >= 7
**File Read:** logs/last-update-check.txt
**Changed in v1.5.2:** From daily check to 7-day interval

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

### MarkdownRenderer
Core component responsible for converting Markdown text to themed HTML. Extracted from MainForm.cs in v1.2.0. Uses Markdig pipeline with advanced extensions and mathematics support.

**File:** Core/MarkdownRenderer.cs
**Features:** Syntax highlighting (Highlight.js), Math formulas (KaTeX), Mermaid diagrams, PlantUML diagrams (HEX encoding), Copy buttons
**Theme-aware:** Applies Theme.MarkdownColors to generated HTML/CSS
**Dependencies:** Markdig, Theme model

---

### FileWatcherManager
Core component for live-reload functionality. Watches a single file for changes and raises FileChanged event. Extracted from MainForm.cs in v1.2.0.

**File:** Core/FileWatcherManager.cs
**Pattern:** IDisposable, Event-driven
**Event:** FileChanged (EventHandler<string>)
**Methods:** Watch(filePath), StopWatching(), Dispose()
**Anti-flicker:** 100ms delay to avoid multiple editor save triggers

---
