# Architecture - MarkdownViewer

System architecture documentation for MarkdownViewer v1.2.0+

**Version:** 1.2.0+
**Created:** 2025-11-05
**Status:** Under Development

---

## Overview

MarkdownViewer follows a **layered architecture** with clear separation of concerns:

```
┌─────────────────────────────────────────┐
│            UI Layer                     │
│  (MainWindow, Managers, Controls)       │
├─────────────────────────────────────────┤
│          Services Layer                 │
│  (Business Logic, External APIs)        │
├─────────────────────────────────────────┤
│           Core Layer                    │
│  (Domain Logic, Pure Functions)         │
├─────────────────────────────────────────┤
│          Models Layer                   │
│  (Data Structures, Settings)            │
└─────────────────────────────────────────┘
```

---

## Project Structure

```
MarkdownViewer/
├── Core/                     # Domain Logic (Pure, Testable)
│   ├── MarkdownRenderer.cs       → Markdown to HTML conversion
│   ├── NavigationManager.cs      → History tracking
│   ├── SearchManager.cs          → Search logic
│   └── FileWatcherManager.cs     → Live reload
│
├── Services/                 # Application Services
│   ├── SettingsService.cs        → JSON settings management
│   ├── ThemeService.cs           → Theme loading & application
│   ├── UpdateService.cs          → Update checking & installation
│   ├── RegistryService.cs        → Windows Explorer integration
│   └── LocalizationService.cs    → Multi-language support
│
├── UI/                       # User Interface
│   ├── MainWindow.cs             → Main form (orchestrator)
│   ├── StatusBarManager.cs       → Status bar logic
│   ├── NavigationBar.cs          → Navigation buttons
│   ├── SearchBar.cs              → Search UI
│   └── ContextMenuBuilder.cs     → Context menus
│
├── Models/                   # Data Models
│   ├── AppSettings.cs            → Settings schema
│   ├── Theme.cs                  → Theme definition
│   ├── NavigationState.cs        → Navigation state
│   ├── GitHubRelease.cs          → GitHub API models
│   └── UpdateInfo.cs             → Update information
│
├── Configuration/            # Configuration
│   ├── UpdateConfiguration.cs    → Update system config
│   └── AppPaths.cs               → Path management
│
├── Resources/                # Localization
│   ├── Strings.resx              → English (default)
│   ├── Strings.de.resx           → German
│   └── ... (8 languages total)
│
├── Themes/                   # Theme Definitions (JSON)
│   ├── dark.json
│   ├── solarized.json
│   ├── draeger.json
│   └── standard.json
│
└── Tests/                    # Unit Tests
    ├── Core.Tests/
    ├── Services.Tests/
    └── Models.Tests/
```

---

## Layer Responsibilities

### Core Layer

**Purpose:** Pure business logic, no dependencies on UI or external services

**Characteristics:**
- No WinForms dependencies
- No I/O operations (file, network)
- Fully unit-testable
- Dependency injection friendly

**Classes:**

#### MarkdownRenderer
```csharp
public class MarkdownRenderer
{
    public string RenderToHtml(string markdown, Theme theme)
    {
        // Markdig pipeline
        // Template generation
        // Theme-based CSS injection
    }
}
```

**Dependencies:** None (pure)
**Tests:** Input markdown → Output HTML assertions

#### NavigationManager
```csharp
public class NavigationManager
{
    public bool CanGoBack { get; }
    public bool CanGoForward { get; }

    public void GoBack()
    public void GoForward()
}
```

**Dependencies:** WebView2 (interface)
**Tests:** Mock WebView2, test history logic

#### SearchManager
```csharp
public class SearchManager
{
    public Task SearchAsync(string term)
    public Task NextMatchAsync()
    public Task PreviousMatchAsync()
}
```

**Dependencies:** WebView2 (for script execution)
**Tests:** Mock WebView2, test search state

#### FileWatcherManager
```csharp
public class FileWatcherManager : IDisposable
{
    public event EventHandler<string> FileChanged;

    public void Watch(string filePath)
    public void Dispose()
}
```

**Dependencies:** FileSystemWatcher
**Tests:** Mock file system events

---

### Services Layer

**Purpose:** Application services with external dependencies (file system, network, registry)

**Characteristics:**
- Can depend on Core layer
- Should be interface-based (ISettingsService, etc.)
- Mockable for testing
- Single responsibility

**Classes:**

#### SettingsService
```csharp
public interface ISettingsService
{
    AppSettings Load();
    void Save(AppSettings settings);
}

public class SettingsService : ISettingsService
{
    // JSON file: %APPDATA%/MarkdownViewer/settings.json
}
```

**Dependencies:** File system
**Tests:** Mock file system, test serialization

#### ThemeService
```csharp
public interface IThemeService
{
    Theme LoadTheme(string name);
    Task ApplyThemeAsync(Theme theme, Form form, WebView2 webView);
    List<string> GetAvailableThemes();
}

public class ThemeService : IThemeService
{
    private readonly Assembly _assembly;

    // v1.5.4: Loads from embedded resources (no file system!)
    // Uses Assembly.GetManifestResourceStream()
    // Resource pattern: "MarkdownViewer.Themes.{name}.json"
    // Applies colors to UI + injects CSS to WebView2
}
```

**Dependencies:** Assembly (reflection), WinForms, WebView2 (no file system!)
**Tests:** Mock assembly resources, test CSS generation
**Version:** v1.5.4 - Refactored from file system to embedded resources

#### UpdateService
```csharp
public interface IUpdateService
{
    Task<UpdateInfo> CheckForUpdatesAsync(string currentVersion);
    Task<bool> DownloadUpdateAsync(string url);
    bool ShouldCheckForUpdates();
    void RecordUpdateCheck();
}

public class UpdateService : IUpdateService
{
    // Refactored from UpdateChecker.cs
    // GitHub API or test JSON files
}
```

**Dependencies:** HTTP client, file system
**Tests:** Mock HTTP, test version comparison

#### RegistryService
```csharp
public interface IRegistryService
{
    bool IsRegistered();
    void Install();
    void Uninstall();
}

public class RegistryService : IRegistryService
{
    // Refactored from Program.cs
    // HKEY_CURRENT_USER registry operations
}
```

**Dependencies:** Windows Registry
**Tests:** Mock registry operations

#### LocalizationService
```csharp
public interface ILocalizationService
{
    string GetString(string key);
    void SetLanguage(string languageCode);
    List<Language> GetAvailableLanguages();
}

public class LocalizationService : ILocalizationService
{
    // .resx-based localization
    // 8 languages supported
}
```

**Dependencies:** .NET ResourceManager
**Tests:** Test string lookup, language switching

---

### UI Layer

**Purpose:** User interface components, event handling, orchestration

**Characteristics:**
- Thin layer (delegates to services)
- WinForms-based
- No business logic (only presentation)
- Event-driven

**Classes:**

#### MainWindow (Orchestrator)
```csharp
public class MainWindow : Form
{
    private readonly ISettingsService _settings;
    private readonly IThemeService _themes;
    private readonly MarkdownRenderer _renderer;
    private readonly FileWatcherManager _fileWatcher;
    private readonly StatusBarManager _statusBar;
    private readonly NavigationBar _navigationBar;
    private readonly SearchBar _searchBar;

    public MainWindow(string filePath, /* dependencies */)
    {
        // Initialize UI
        // Wire up events
        // Load file
    }
}
```

**Responsibilities:**
- Dependency injection (creates/injects services)
- Event orchestration
- Keyboard shortcuts
- Menu building

**Target Size:** < 250 lines (down from 735!)

#### StatusBarManager
```csharp
public class StatusBarManager
{
    public void Show();
    public void Hide();
    public void Toggle();
    public void UpdateIcons();

    private void UpdateIcon_Click(/* ... */);
    private void ExplorerIcon_Click(/* ... */);
    private void LanguageSelector_Click(/* ... */);
    private void InfoIcon_Click(/* ... */);
    private void HelpIcon_Click(/* ... */);
}
```

**Responsibilities:**
- StatusStrip management
- Icon state updates
- Click event handlers
- Context menu

#### NavigationBar
```csharp
public class NavigationBar
{
    public void Show();
    public void Hide();
    public void Toggle();

    private void BackButton_Click();
    private void ForwardButton_Click();
}
```

**Responsibilities:**
- ToolStrip with navigation buttons
- Enable/disable based on NavigationManager state

#### SearchBar
```csharp
public class SearchBar
{
    public void Show();
    public void Hide();

    private void SearchBox_TextChanged();
    private void NextButton_Click();
    private void PreviousButton_Click();
}
```

**Responsibilities:**
- Search UI panel
- Match counter
- Keyboard shortcuts (Enter, F3, Esc)

#### ContextMenuBuilder
```csharp
public class ContextMenuBuilder
{
    public ContextMenuStrip BuildMainContextMenu();
    public ContextMenuStrip BuildStatusBarContextMenu();

    private ToolStripMenuItem BuildThemeMenu();
    private ToolStripMenuItem BuildLanguageMenu();
}
```

**Responsibilities:**
- Centralized context menu creation
- Theme switcher menu
- Language switcher menu

---

### Models Layer

**Purpose:** Data structures, DTOs, settings schemas

**Characteristics:**
- POCOs (Plain Old CLR Objects)
- No logic (only properties)
- Serializable (JSON)
- Immutable when possible

**Classes:**

#### AppSettings
```csharp
public class AppSettings
{
    public string Version { get; set; }
    public string Language { get; set; }
    public string Theme { get; set; }

    public UiSettings UI { get; set; }
    public UpdateSettings Updates { get; set; }
    public ExplorerSettings Explorer { get; set; }
    public ShortcutSettings Shortcuts { get; set; }
}
```

**Serialization:** System.Text.Json
**Storage:** %APPDATA%/MarkdownViewer/settings.json

#### Theme
```csharp
public class Theme
{
    public string Name { get; set; }
    public string DisplayName { get; set; }

    public MarkdownColors Markdown { get; set; }
    public UiColors UI { get; set; }
}

public class MarkdownColors
{
    public string Background { get; set; }
    public string Foreground { get; set; }
    // ... 10 color properties
}

public class UiColors
{
    public string FormBackground { get; set; }
    public string StatusBarBackground { get; set; }
    // ... 7 color properties
}
```

**Serialization:** System.Text.Json
**Storage:** Themes/*.json

#### NavigationState
```csharp
public class NavigationState
{
    public Stack<string> BackStack { get; }
    public Stack<string> ForwardStack { get; }
    public string CurrentPath { get; set; }
}
```

**Purpose:** Track navigation history

#### GitHubRelease & UpdateInfo
```csharp
public class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; }

    // ... GitHub API mapping
}

public class UpdateInfo
{
    public bool UpdateAvailable { get; set; }
    public string LatestVersion { get; set; }
    public string DownloadUrl { get; set; }
    // ...
}
```

**Purpose:** GitHub API communication

---

## Dependency Flow

```
MainWindow (UI)
    ↓
    ├─→ SettingsService → AppSettings (Model)
    ├─→ ThemeService → Theme (Model)
    ├─→ MarkdownRenderer (Core)
    ├─→ FileWatcherManager (Core)
    ├─→ UpdateService → UpdateInfo (Model)
    └─→ RegistryService

StatusBarManager (UI)
    ↓
    ├─→ UpdateService
    ├─→ RegistryService
    └─→ LocalizationService

NavigationBar (UI)
    ↓
    └─→ NavigationManager (Core)

SearchBar (UI)
    ↓
    └─→ SearchManager (Core)
```

**Rule:** Dependencies always flow DOWNWARD
- UI → Services → Core → Models
- NEVER: Core → Services or Core → UI

---

## Data Flow

### Settings Loading
```
Application Start
    ↓
SettingsService.Load()
    ↓
%APPDATA%/MarkdownViewer/settings.json
    ↓
AppSettings (Model)
    ↓
ThemeService.LoadTheme(settings.Theme)
    ↓
Assembly.GetManifestResourceStream("MarkdownViewer.Themes.{theme}.json")
    ↓
Embedded Resource (compiled into .exe)
    ↓
Theme (Model)
    ↓
ThemeService.ApplyThemeAsync()
    ↓
UI Updated + WebView2 CSS Injected
```

**Note (v1.5.4):** Themes are now loaded from embedded resources, not external files.

### Markdown Rendering
```
User Opens File
    ↓
MainWindow.OpenFile(path)
    ↓
File.ReadAllText(path)
    ↓
MarkdownRenderer.RenderToHtml(markdown, theme)
    ↓
Markdig Pipeline
    ↓
HTML + Theme CSS
    ↓
WebView2.NavigateToString(html)
    ↓
Displayed to User
```

### Update Check
```
Application Start (once every 7 days)
    ↓
UpdateService.ShouldCheckForUpdates()
    ↓
Check logs/last-update-check.txt (elapsed.TotalDays >= 7?)
    ↓
Task.Run(CheckForUpdatesAsync)  // Background
    ↓
GitHub API (or test JSON)
    ↓
On Success: RecordUpdateCheck() saves timestamp
On Failure: No timestamp saved → Automatic retry next start
    ↓
UpdateInfo (Model)
    ↓
StatusBarManager.UpdateIcon (if update available)
    ↓
User Click → Download → Install → Restart
```

---

## Testing Strategy

### Unit Tests (>= 80% coverage)

**Core Layer:** 100% coverage goal
- MarkdownRenderer: Input/output tests
- NavigationManager: State machine tests
- SearchManager: Match logic tests
- FileWatcherManager: Event tests

**Services Layer:** 90% coverage goal
- SettingsService: Serialization, file I/O (mocked)
- ThemeService: Theme loading, CSS generation
- UpdateService: Version comparison, download logic
- RegistryService: Registry operations (mocked)
- LocalizationService: String lookup, language switching

**Models Layer:** 100% coverage goal
- Serialization round-trip tests
- Validation tests

**UI Layer:** Integration tests only
- Not unit-testable (WinForms complexity)
- Covered by manual + integration tests

### Integration Tests

**Scenarios:**
1. First launch → Default settings
2. Theme switching → UI updates
3. Language switching → Strings updated
4. Search → Matches found
5. Navigation → History works
6. Update flow → Download & install

### Manual Tests

**Checklist:** (see DEPLOYMENT-GUIDE.md)
- All features work
- No visual glitches
- Performance acceptable
- No crashes

---

## Design Patterns

### Singleton
- `UpdateConfiguration`: Global config access
- `LocalizationService`: Single resource manager

### Dependency Injection (Constructor)
```csharp
public MainWindow(
    ISettingsService settings,
    IThemeService themes,
    MarkdownRenderer renderer)
{
    _settings = settings;
    _themes = themes;
    _renderer = renderer;
}
```

### Observer (Event-Driven)
```csharp
_fileWatcher.FileChanged += OnFileChanged;
_navigationManager.NavigationChanged += OnNavigationChanged;
_searchManager.SearchResultsChanged += OnSearchResultsChanged;
```

### Factory
```csharp
public class ThemeFactory
{
    public static Theme CreateFromJson(string json);
    public static Theme CreateDefault();
}
```

### Strategy (Theme Application)
```csharp
public interface IThemeApplicator
{
    void Apply(Theme theme);
}

public class MarkdownThemeApplicator : IThemeApplicator { }
public class UiThemeApplicator : IThemeApplicator { }
```

---

## Performance Considerations

### Lazy Loading
- Themes loaded on demand
- Language resources loaded on first use

### Caching
- Parsed theme objects cached
- Localized strings cached

### Async Operations
- Update check: Background task
- File loading: Async I/O
- Search: Async WebView2 execution

### Memory Management
- FileWatcherManager: IDisposable
- WebView2: Proper cleanup
- Event handler unsubscription

---

## Security Considerations

### Registry Access
- HKEY_CURRENT_USER only (no admin)
- Validation of paths
- Safe key deletion (throwOnMissingSubKey: false)

### File System
- Settings in %APPDATA% (user-writable)
- No temp file vulnerabilities
- Proper file locking

### Update Mechanism
- HTTPS only (GitHub API)
- Signature verification (future)
- Rollback on failure

### WebView2
- Content Security Policy
- No arbitrary script injection
- Sanitized markdown input

---

## Extension Points

### Custom Themes
Users can add `Themes/custom.json` files

### Custom Translations
Community can contribute .resx files

### Future: Plugin System
```csharp
public interface IMarkdownViewerPlugin
{
    string Name { get; }
    void Initialize(IServiceProvider services);
    void OnDocumentLoaded(string path);
}
```

---

## Deployment Architecture

### Single-File Executable
```
MarkdownViewer.exe (~2.0 MB)
    ↓
Contains:
- .NET 8 Runtime libraries
- All application code
- Embedded resources (.resx for localization)
- Embedded themes (Themes/*.json) [v1.5.4]
- Embedded icons (Icons/*.svg)
- Application icon

External files:
- Settings: %APPDATA%/MarkdownViewer/settings.json
- Logs: ./logs/
```

**Note (v1.5.4):** Themes are now embedded at compile time. No external Themes/ folder needed.

### Update Process
```
1. pending-update.exe downloaded
2. On restart: ApplyPendingUpdate()
3. Backup current: MarkdownViewer.exe.backup
4. Replace: pending-update.exe → MarkdownViewer.exe
5. Delete backup
6. Restart with same arguments
```

---

## Technology Stack

### Core Technologies
- **.NET 8.0** - Target framework
- **Windows Forms** - UI framework
- **WebView2** - HTML rendering (Edge Chromium)

### Libraries
- **Markdig 0.37.0** - Markdown parsing
- **Serilog 4.0.0** - Logging
- **System.Text.Json** - JSON serialization

### Testing
- **xUnit 2.6.2** - Test framework
- **Moq 4.20.70** - Mocking
- **FluentAssertions 6.12.0** - Test assertions

### External Resources (CDN)
- **Highlight.js 11.9.0** - Syntax highlighting
- **KaTeX 0.16.9** - Math rendering
- **Mermaid.js 10** - Diagrams
- **mark.js** - Search highlighting

---

## Metrics & Monitoring

### Code Metrics
- Lines of Code: Track per class
- Cyclomatic Complexity: Target < 10
- Test Coverage: Target >= 80%
- Tech Debt: Track hours

### Performance Metrics
- Startup Time: Target < 2s
- Theme Switch: Target < 500ms
- Search Performance: Target < 1s for 1000 matches
- Memory Usage: Target < 100MB

### User Metrics (Future)
- Feature usage (opt-in telemetry)
- Error rates (crash reporting)
- Update adoption rates

---

## Maintenance & Evolution

### Backward Compatibility
- Settings schema versioning
- Migration logic for old settings
- Fallback to defaults

### Breaking Changes
- Major version bumps only (2.0.0)
- Migration guides
- Deprecation warnings

### Refactoring Guidelines
- Extract before extend
- Tests before refactor
- Small, incremental changes
- Document in impl_progress.md

---

## References

- [ROADMAP.md](ROADMAP.md) - Feature roadmap
- [PROCESS-MODEL.md](PROCESS-MODEL.md) - Development process
- [GLOSSARY.md](GLOSSARY.md) - Terms & definitions
- [DEVELOPMENT.md](DEVELOPMENT.md) - Developer docs
- [DEPLOYMENT-GUIDE.md](DEPLOYMENT-GUIDE.md) - Deployment process

---

**Next Steps:**
1. Review this architecture with team/user
2. Start implementing v1.2.0 (see ROADMAP.md)
3. Update this document as implementation evolves
4. Track progress in impl_progress.md

**Status:** ✅ Architecture Defined → Ready for Implementation
