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
UI component at the bottom of the window showing status icons (update, explorer registration, language, info, help). Always visible by default.

**Component:** WinForms StatusStrip
**Manager:** UI/StatusBarControl.cs
**Visibility:** Always visible (Visible = true by default)

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

### Embedded Resources (.NET)
Resources (files) embedded into the compiled assembly at build time, rather than copied as external files. Accessed at runtime using Assembly.GetManifestResourceStream().

**Configuration:** `<EmbeddedResource Include="path/to/file" />` in .csproj
**Benefits:** Single-file deployment, resources always available, no external dependencies
**Used for:** Theme JSON files (Themes/*.json), Icons (Icons/*.svg)
**Naming pattern:** `"MarkdownViewer.Themes.dark.json"` → `"dark"`
**Version:** Implemented in v1.5.4

---

### Assembly.GetManifestResourceStream()
.NET method to load embedded resources from a compiled assembly. Returns a Stream to read the resource data.

**Usage:** `_assembly.GetManifestResourceStream("MarkdownViewer.Themes.dark.json")`
**Returns:** Stream or null if resource not found
**Pattern:** `using (Stream stream = ...) { using (StreamReader reader = ...) { } }`
**Location:** ThemeService.LoadTheme()
**Version:** Implemented in v1.5.4

---

### Theme Selector (StatusBar)
UI component in the StatusBar that allows instant theme switching via dropdown menu. Positioned on right side, left of language selector.

**Component:** WinForms ToolStripDropDownButton
**Location:** StatusBarControl.cs (_themeSelector field)
**Event:** ThemeChanged (raised when user selects new theme)
**Property:** CurrentTheme (string) - currently selected theme name
**Display names:** Dark, Standard, Solarized, Dräger
**Version:** Implemented in v1.5.4

---

### Resource Naming Convention
Pattern for naming embedded resources in .NET assemblies. Format: `{Namespace}.{Folder}.{Filename}.{Extension}`

**Example:** `"MarkdownViewer.Themes.dark.json"`
**Parsing:** Split by '.', extract second-to-last part ("dark")
**Discovery:** `Assembly.GetManifestResourceNames()` returns all embedded resource names
**Used by:** ThemeService.GetAvailableThemes()
**Version:** Implemented in v1.5.4

---

### GetAvailableThemes()
Method in ThemeService that discovers all available themes by scanning embedded resources.

**Location:** ThemeService.cs
**Returns:** List<string> of theme names (e.g., "dark", "standard")
**Logic:** Scans Assembly.GetManifestResourceNames() for "MarkdownViewer.Themes.*.json" pattern
**Parsing:** Extracts theme name from resource name
**Used by:** StatusBarControl.PopulateThemeDropdown(), MainForm.InitializeThemeContextMenu()
**Version:** Refactored in v1.5.4 from file system scanning to embedded resource scanning

---

### PopulateThemeDropdown()
Method in StatusBarControl that fills the theme selector dropdown with available themes.

**Location:** StatusBarControl.cs
**Parameters:** List<string> availableThemes, string currentTheme
**Actions:** Clears dropdown, adds ToolStripMenuItem for each theme, sets checkmark on current theme
**Called by:** MainForm.InitializeStatusBar()
**Version:** Implemented in v1.5.4

---

### OnThemeChanged()
Async event handler in MainForm that responds to theme selection changes from StatusBar.

**Location:** MainForm.cs
**Signature:** `private async void OnThemeChanged(object? sender, EventArgs e)`
**Actions:**
1. Reads new theme name from _statusBar.CurrentTheme
2. Loads theme from embedded resources via ThemeService
3. Saves theme to settings.json
4. Applies theme to UI and WebView2
5. Shows error dialog on failure

**Version:** Implemented in v1.5.4

---

### MVP (Model-View-Presenter) Pattern
Architectural pattern that separates business logic from UI for testability.
- **Model:** Data models (AppSettings, Theme, etc.)
- **View:** UI components implementing view interfaces (MainForm implements IMainView)
- **Presenter:** Business logic coordinators (MainPresenter, StatusBarPresenter, etc.)

**Benefits:** Full UI testability without requiring actual UI components.

**Files:**
- Views/: Interface definitions (IMainView.cs, IWebViewAdapter.cs, etc.)
- Presenters/: Business logic (MainPresenter.cs, etc.)
- MainForm.cs: Implements IMainView interface

---

### IMainView
View interface for MainForm. Abstracts all UI interactions for testability.

**Purpose:** Allows MainPresenter to control UI without direct dependency on WinForms.

**Properties:** CurrentFilePath, WindowTitle, IsNavigationBarVisible, IsSearchBarVisible

**Events:** ViewLoaded, ThemeChangeRequested, LanguageChangeRequested, RefreshRequested, etc.

**Methods:** DisplayMarkdown(), ShowError(), UpdateTheme(), SetNavigationState(), etc.

**File:** Views/IMainView.cs
**Implemented by:** MainForm.cs
**Used by:** MainPresenter.cs

---

### MainPresenter
Core presenter managing main window business logic.

**Responsibilities:**
- Theme and language switching
- Settings persistence
- File loading and watching
- Coordinating view updates

**Dependencies:** IMainView, IWebViewAdapter, ISettingsService, IThemeService, ILocalizationService, IDialogService

**File:** Presenters/MainPresenter.cs (314 lines)
**Tests:** Presenters/MainPresenterTests.cs (10 tests, all passing)

---

### IWebViewAdapter
Adapter interface for WebView2 control to enable testing without actual WebView2.

**Purpose:** Wraps WebView2 functionality behind testable interface.

**Properties:** IsInitialized, CanGoBack, CanGoForward

**Methods:** NavigateToStringAsync(), ExecuteScriptAsync(), GoBack(), GoForward(), Reload()

**File:** Views/IWebViewAdapter.cs
**Implementation:** Views/WebView2Adapter.cs (wraps actual WebView2)
**Mock:** Mocks/MockWebViewAdapter.cs (for testing)

---

### IDialogService
Service interface for showing dialogs (MessageBox abstraction).

**Purpose:** Makes dialog calls testable by abstracting MessageBox.

**Methods:** ShowError(), ShowInfo(), ShowWarning(), ShowConfirmation(), ShowYesNo()

**File:** Services/IDialogService.cs
**Implementation:** Services/WinFormsDialogService.cs (uses MessageBox)
**Mock:** Mocks/MockDialogService.cs (for testing)

---

### DI Container (Dependency Injection)
Microsoft.Extensions.DependencyInjection container configured in Program.cs.

**Registered Services:**
- Singleton: ISettingsService, IThemeService, ILocalizationService, IDialogService
- Transient: MarkdownRenderer, FileWatcherManager, All Presenters, MainForm

**Configuration:** Program.cs::BuildServiceProvider()

**Benefits:** Loose coupling, easy testing, clear dependencies

---

