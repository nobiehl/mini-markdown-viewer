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

