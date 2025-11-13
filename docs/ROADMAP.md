# MarkdownViewer Roadmap v1.2.0 - v1.5.0

Detaillierter Entwicklungsplan für die Weiterentwicklung von MarkdownViewer mit Fokus auf Architektur, Themes, und Benutzerfreundlichkeit.

**Erstellt:** 2025-11-05
**Status:** Planning Phase

---

## Überblick

### Hauptziele
1. **Architektur-Refactoring:** God Classes auflösen, testbare Services erstellen
2. **Theme-System:** 4 Themes für Markdown + UI (Dark, Solarized, Dräger, Standard)
3. **Lokalisierung:** 8 Sprachen mit .resx
4. **Benutzerfreundlichkeit:** StatusBar, Navigation, Search
5. **Testabdeckung:** >= 80% Unit Test Coverage

### Versionsplan
- **v1.2.0:** Architecture + Settings + Theme Foundation
- **v1.3.0:** StatusBar + Localization + Theme Implementation
- **v1.4.0:** Navigation + Search
- **v1.5.0:** Polish + Testing + Documentation

---

## v1.2.0: Architecture Refactoring + Theme Foundation

**Ziel:** Solide Basis für alle weiteren Features schaffen

**Aufwand:** ~3 Sessions

### Feature 1.2.1: Ordnerstruktur & Projektorganisation

**Was:** Neue Ordnerstruktur anlegen

**Neue Struktur:**
```
MarkdownViewer/
├── Core/                     # Business Logic
│   ├── MarkdownRenderer.cs
│   ├── NavigationManager.cs
│   ├── SearchManager.cs
│   └── FileWatcherManager.cs
│
├── Services/                 # Application Services
│   ├── SettingsService.cs
│   ├── ThemeService.cs       # ← NEU für Themes!
│   ├── UpdateService.cs
│   ├── RegistryService.cs
│   └── LocalizationService.cs
│
├── UI/                       # User Interface
│   ├── MainWindow.cs
│   ├── StatusBarManager.cs
│   ├── NavigationBar.cs
│   ├── SearchBar.cs
│   └── ContextMenuBuilder.cs
│
├── Models/                   # Data Models
│   ├── AppSettings.cs
│   ├── Theme.cs              # ← NEU!
│   ├── NavigationState.cs
│   ├── GitHubRelease.cs
│   └── UpdateInfo.cs
│
├── Configuration/            # Configuration
│   ├── UpdateConfiguration.cs
│   └── AppPaths.cs
│
├── Resources/                # Localization
│   ├── Strings.resx
│   ├── Strings.de.resx
│   └── ... (8 languages)
│
├── Themes/                   # ← NEU: Theme Definitions
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

**Tasks:**
- [ ] Ordner erstellen
- [ ] Namespaces anpassen
- [ ] .csproj Struktur anpassen
- [ ] Glossar-Einträge für alle neuen Komponenten

**Test Strategy:** N/A (Struktur-Änderung)

---

### Feature 1.2.2: Settings System

**Was:** JSON-basiertes Settings-System mit Theme-Support

**AppSettings.cs Model:**
```csharp
public class AppSettings
{
    public string Version { get; set; } = "1.2.0";
    public string Language { get; set; } = "system";
    public string Theme { get; set; } = "standard";  // ← NEU!

    public UiSettings UI { get; set; } = new();
    public UpdateSettings Updates { get; set; } = new();
    public ExplorerSettings Explorer { get; set; } = new();
    public ShortcutSettings Shortcuts { get; set; } = new();
    public NavigationSettings Navigation { get; set; } = new();
}

public class UiSettings
{
    public StatusBarSettings StatusBar { get; set; } = new();
    public NavigationBarSettings NavigationBar { get; set; } = new();
    public SearchSettings Search { get; set; } = new();
}

public class StatusBarSettings
{
    public bool Visible { get; set; } = false;
    public bool ShowUpdateStatus { get; set; } = false;
    public bool ShowExplorerStatus { get; set; } = false;
    public bool ShowLanguage { get; set; } = false;
    public bool ShowInfo { get; set; } = false;
    public bool ShowHelp { get; set; } = false;
}

// ... weitere Settings
```

**SettingsService.cs:**
```csharp
public interface ISettingsService
{
    AppSettings Load();
    void Save(AppSettings settings);
    string GetSettingsPath();
}

public class SettingsService : ISettingsService
{
    private readonly string _settingsPath;

    public SettingsService()
    {
        // %APPDATA%/MarkdownViewer/settings.json
        _settingsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MarkdownViewer",
            "settings.json"
        );
    }

    public AppSettings Load()
    {
        if (!File.Exists(_settingsPath))
            return new AppSettings(); // Defaults

        var json = File.ReadAllText(_settingsPath);
        return JsonSerializer.Deserialize<AppSettings>(json)
            ?? new AppSettings();
    }

    public void Save(AppSettings settings)
    {
        var directory = Path.GetDirectoryName(_settingsPath);
        Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(_settingsPath, json);
    }
}
```

**Tasks:**
- [ ] AppSettings.cs mit allen Models
- [ ] SettingsService.cs implementieren
- [ ] Unit Tests für SettingsService
  - [ ] Load when file not exists
  - [ ] Load existing file
  - [ ] Save creates directory
  - [ ] Save creates valid JSON
  - [ ] Round-trip test
- [ ] Integration in MainWindow

**Test Strategy:**
- Testable durch Interface
- Mock file system für Tests
- >= 90% Coverage Ziel

---

### Feature 1.2.3: Theme Foundation

**Was:** Theme-System mit 4 vorkonfigurierten Themes

#### Theme.cs Model

```csharp
public class Theme
{
    public string Name { get; set; } = "";
    public string DisplayName { get; set; } = "";

    // Markdown Rendering Colors
    public MarkdownColors Markdown { get; set; } = new();

    // WinForms UI Colors
    public UiColors UI { get; set; } = new();
}

public class MarkdownColors
{
    public string Background { get; set; } = "#ffffff";
    public string Foreground { get; set; } = "#333333";
    public string CodeBackground { get; set; } = "#f5f5f5";
    public string LinkColor { get; set; } = "#4A90E2";
    public string HeadingColor { get; set; } = "#000000";
    public string BlockquoteBorder { get; set; } = "#4A90E2";
    public string TableHeaderBackground { get; set; } = "#f5f5f5";
    public string TableBorder { get; set; } = "#dddddd";
    public string InlineCodeBackground { get; set; } = "#f5f5f5";
    public string InlineCodeForeground { get; set; } = "#c7254e";
}

public class UiColors
{
    public string FormBackground { get; set; } = "#f0f0f0";
    public string ControlBackground { get; set; } = "#ffffff";
    public string ControlForeground { get; set; } = "#000000";
    public string StatusBarBackground { get; set; } = "#e0e0e0";
    public string StatusBarForeground { get; set; } = "#000000";
    public string MenuBackground { get; set; } = "#ffffff";
    public string MenuForeground { get; set; } = "#000000";
}
```

#### Theme Definitions (JSON)

**Themes/dark.json:**
```json
{
  "name": "dark",
  "displayName": "Dark",
  "markdown": {
    "background": "#1e1e1e",
    "foreground": "#d4d4d4",
    "codeBackground": "#2d2d30",
    "linkColor": "#569cd6",
    "headingColor": "#4ec9b0",
    "blockquoteBorder": "#007acc",
    "tableHeaderBackground": "#2d2d30",
    "tableBorder": "#3e3e42",
    "inlineCodeBackground": "#2d2d30",
    "inlineCodeForeground": "#ce9178"
  },
  "ui": {
    "formBackground": "#1e1e1e",
    "controlBackground": "#252526",
    "controlForeground": "#cccccc",
    "statusBarBackground": "#007acc",
    "statusBarForeground": "#ffffff",
    "menuBackground": "#252526",
    "menuForeground": "#cccccc"
  }
}
```

**Themes/solarized.json:**
```json
{
  "name": "solarized",
  "displayName": "Solarized Light (No Blue)",
  "markdown": {
    "background": "#fdf6e3",
    "foreground": "#657b83",
    "codeBackground": "#eee8d5",
    "linkColor": "#2aa198",
    "headingColor": "#586e75",
    "blockquoteBorder": "#93a1a1",
    "tableHeaderBackground": "#eee8d5",
    "tableBorder": "#93a1a1",
    "inlineCodeBackground": "#eee8d5",
    "inlineCodeForeground": "#dc322f"
  },
  "ui": {
    "formBackground": "#fdf6e3",
    "controlBackground": "#eee8d5",
    "controlForeground": "#657b83",
    "statusBarBackground": "#93a1a1",
    "statusBarForeground": "#073642",
    "menuBackground": "#eee8d5",
    "menuForeground": "#657b83"
  }
}
```

**Themes/draeger.json:**
```json
{
  "name": "draeger",
  "displayName": "Dräger",
  "markdown": {
    "background": "#ffffff",
    "foreground": "#003366",
    "codeBackground": "#f0f4f8",
    "linkColor": "#0066cc",
    "headingColor": "#003366",
    "blockquoteBorder": "#0066cc",
    "tableHeaderBackground": "#e6f2ff",
    "tableBorder": "#b3d9ff",
    "inlineCodeBackground": "#f0f4f8",
    "inlineCodeForeground": "#cc0000"
  },
  "ui": {
    "formBackground": "#f5f5f5",
    "controlBackground": "#ffffff",
    "controlForeground": "#003366",
    "statusBarBackground": "#003366",
    "statusBarForeground": "#ffffff",
    "menuBackground": "#ffffff",
    "menuForeground": "#003366"
  }
}
```

**Themes/standard.json:**
```json
{
  "name": "standard",
  "displayName": "Standard (Enhanced)",
  "markdown": {
    "background": "#ffffff",
    "foreground": "#2c3e50",
    "codeBackground": "#f8f9fa",
    "linkColor": "#3498db",
    "headingColor": "#1a252f",
    "blockquoteBorder": "#3498db",
    "tableHeaderBackground": "#ecf0f1",
    "tableBorder": "#bdc3c7",
    "inlineCodeBackground": "#f8f9fa",
    "inlineCodeForeground": "#e74c3c"
  },
  "ui": {
    "formBackground": "#ecf0f1",
    "controlBackground": "#ffffff",
    "controlForeground": "#2c3e50",
    "statusBarBackground": "#3498db",
    "statusBarForeground": "#ffffff",
    "menuBackground": "#ffffff",
    "menuForeground": "#2c3e50"
  }
}
```

#### ThemeService.cs

```csharp
public interface IThemeService
{
    Theme GetCurrentTheme();
    Theme LoadTheme(string themeName);
    List<string> GetAvailableThemes();
    void ApplyTheme(Theme theme, Form form, WebView2 webView);
}

public class ThemeService : IThemeService
{
    private Theme _currentTheme;
    private readonly string _themesPath;

    public ThemeService()
    {
        // Themes/ folder next to executable
        _themesPath = Path.Combine(
            Path.GetDirectoryName(Application.ExecutablePath),
            "Themes"
        );
    }

    public Theme LoadTheme(string themeName)
    {
        var themeFile = Path.Combine(_themesPath, $"{themeName}.json");

        if (!File.Exists(themeFile))
        {
            Log.Warning("Theme not found: {Theme}, using standard", themeName);
            return LoadTheme("standard");
        }

        var json = File.ReadAllText(themeFile);
        var theme = JsonSerializer.Deserialize<Theme>(json);

        _currentTheme = theme;
        return theme;
    }

    public void ApplyTheme(Theme theme, Form form, WebView2 webView)
    {
        // Apply to WinForms UI
        form.BackColor = ColorTranslator.FromHtml(theme.UI.FormBackground);

        // Apply to StatusBar, Toolbar, etc.
        foreach (Control control in form.Controls)
        {
            ApplyThemeToControl(control, theme);
        }

        // Apply to Markdown rendering (inject CSS)
        InjectThemeCSS(webView, theme);
    }

    private void ApplyThemeToControl(Control control, Theme theme)
    {
        if (control is StatusStrip statusBar)
        {
            statusBar.BackColor = ColorTranslator.FromHtml(theme.UI.StatusBarBackground);
            statusBar.ForeColor = ColorTranslator.FromHtml(theme.UI.StatusBarForeground);
        }
        // ... weitere Controls
    }

    private void InjectThemeCSS(WebView2 webView, Theme theme)
    {
        var css = GenerateThemeCSS(theme.Markdown);

        var script = $@"
            var style = document.createElement('style');
            style.innerHTML = `{css}`;
            document.head.appendChild(style);
        ";

        webView.ExecuteScriptAsync(script);
    }

    private string GenerateThemeCSS(MarkdownColors colors)
    {
        return $@"
            body {{
                background: {colors.Background} !important;
                color: {colors.Foreground} !important;
            }}
            code {{
                background: {colors.InlineCodeBackground} !important;
                color: {colors.InlineCodeForeground} !important;
            }}
            pre {{
                background: {colors.CodeBackground} !important;
            }}
            a {{
                color: {colors.LinkColor} !important;
            }}
            h1, h2, h3, h4, h5, h6 {{
                color: {colors.HeadingColor} !important;
            }}
            blockquote {{
                border-left-color: {colors.BlockquoteBorder} !important;
            }}
            table th {{
                background: {colors.TableHeaderBackground} !important;
            }}
            table th, table td {{
                border-color: {colors.TableBorder} !important;
            }}
        ";
    }
}
```

**Tasks:**
- [ ] Theme.cs Model erstellen
- [ ] 4 Theme JSON files erstellen (dark, solarized, draeger, standard)
- [ ] ThemeService.cs implementieren
- [ ] Unit Tests für ThemeService
  - [ ] Load existing theme
  - [ ] Load non-existing theme (fallback to standard)
  - [ ] Get available themes
  - [ ] Generate CSS from theme
- [ ] Integration in MainWindow
- [ ] Theme Switcher Prototyp (Menü)

**Test Strategy:**
- Mock file system
- Test CSS generation
- Test theme fallback
- >= 85% Coverage

**Note:** Dräger Website (www.draeger.de) analysieren für exakte Farben!

---

### Feature 1.2.4: Refactoring MainForm.cs

**Was:** MainForm.cs von 735 → ~200 Zeilen reduzieren

**Extraktionen:**

#### MarkdownRenderer.cs
```csharp
public class MarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline;

    public MarkdownRenderer()
    {
        _pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseMathematics()
            .Build();
    }

    public string RenderToHtml(string markdown, Theme theme)
    {
        var content = Markdown.ToHtml(markdown, _pipeline);
        return WrapInHtmlTemplate(content, theme);
    }

    private string WrapInHtmlTemplate(string content, Theme theme)
    {
        // Generate full HTML with theme colors
        // Include KaTeX, Highlight.js, Mermaid, PlantUML
    }
}
```

#### FileWatcherManager.cs
```csharp
public class FileWatcherManager : IDisposable
{
    private FileSystemWatcher _watcher;

    public event EventHandler<string> FileChanged;

    public void Watch(string filePath)
    {
        // Setup watcher
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}
```

**New MainWindow.cs (slim):**
```csharp
public class MainWindow : Form
{
    private readonly SettingsService _settingsService;
    private readonly ThemeService _themeService;
    private readonly MarkdownRenderer _renderer;
    private readonly FileWatcherManager _fileWatcher;
    private readonly StatusBarManager _statusBar;

    private WebView2 _webView;
    private string _currentFilePath;

    public MainWindow(string filePath)
    {
        // Dependency Injection
        _settingsService = new SettingsService();
        _themeService = new ThemeService();
        _renderer = new MarkdownRenderer();
        _fileWatcher = new FileWatcherManager();

        InitializeComponents();
        LoadSettings();
        OpenFile(filePath);
    }

    private void InitializeComponents()
    {
        // Minimal UI setup
        // Delegates to managers
    }

    private void LoadSettings()
    {
        var settings = _settingsService.Load();
        var theme = _themeService.LoadTheme(settings.Theme);
        _themeService.ApplyTheme(theme, this, _webView);
    }

    private void OpenFile(string filePath)
    {
        var markdown = File.ReadAllText(filePath);
        var theme = _themeService.GetCurrentTheme();
        var html = _renderer.RenderToHtml(markdown, theme);

        _webView.NavigateToString(html);
        _fileWatcher.Watch(filePath);
    }
}
```

**Tasks:**
- [ ] MarkdownRenderer.cs extrahieren
- [ ] FileWatcherManager.cs extrahieren
- [ ] MainWindow.cs refactoren
- [ ] Unit Tests für MarkdownRenderer
- [ ] Unit Tests für FileWatcherManager
- [ ] Integration Tests

**Test Strategy:**
- MarkdownRenderer: Test HTML generation
- FileWatcherManager: Mock FileSystemWatcher
- MainWindow: Integration test

---

### Feature 1.2.5: Refactoring Program.cs

**Was:** Program.cs von 540 → ~150 Zeilen reduzieren

#### RegistryService.cs
```csharp
public interface IRegistryService
{
    bool IsRegistered();
    void Install();
    void Uninstall();
}

public class RegistryService : IRegistryService
{
    // Extract all registry logic from Program.cs
}
```

**New Program.cs:**
```csharp
static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        UpdateChecker.ApplyPendingUpdate();

        var logLevel = ParseLogLevel(args);
        InitializeLogging(logLevel);

        if (HandleSpecialCommands(args))
            return;

        ApplicationConfiguration.Initialize();

        var filePath = GetFilePathFromArgs(args);
        var mainWindow = new MainWindow(filePath);

        StartUpdateCheckIfNeeded(mainWindow);

        Application.Run(mainWindow);
    }

    private static bool HandleSpecialCommands(string[] args)
    {
        var registry = new RegistryService();

        if (args.Contains("--install"))
        {
            registry.Install();
            return true;
        }

        if (args.Contains("--uninstall"))
        {
            registry.Uninstall();
            return true;
        }

        // ... weitere Commands

        return false;
    }
}
```

**Tasks:**
- [ ] RegistryService.cs erstellen
- [ ] Program.cs refactoren
- [ ] Unit Tests für RegistryService (mock Registry)
- [ ] Manual test for install/uninstall

---

### v1.2.0 Deliverables

**Code:**
- ✅ Neue Ordnerstruktur
- ✅ SettingsService mit Tests
- ✅ ThemeService mit Tests
- ✅ 4 Theme JSON files
- ✅ MarkdownRenderer extrahiert
- ✅ FileWatcherManager extrahiert
- ✅ RegistryService extrahiert
- ✅ MainWindow.cs < 250 Zeilen
- ✅ Program.cs < 200 Zeilen

**Tests:**
- ✅ Unit Tests für alle Services
- ✅ >= 80% Test Coverage
- ✅ All tests passing

**Documentation:**
- ✅ impl_progress.md aktualisiert
- ✅ GLOSSARY.md mit allen neuen Begriffen
- ✅ ARCHITECTURE.md erstellt
- ✅ DEVELOPMENT.md aktualisiert

**Release:**
- ✅ Version 1.2.0
- ✅ Binary getestet
- ✅ Themes funktionieren
- ✅ Settings werden persistiert

---

## v1.3.0: StatusBar + Localization + Theme UI

**Ziel:** Erste sichtbare Benutzer-Features

**Aufwand:** ~2 Sessions

### Feature 1.3.1: Localization Infrastructure

**Was:** .resx-basierte Lokalisierung für 8 Sprachen

**Sprachen:**
1. 🇬🇧 English (en-US) - Standard
2. 🇩🇪 Deutsch (de-DE)
3. 🇲🇳 Mongolisch (mn-MN)
4. 🇫🇷 Französisch (fr-FR)
5. 🇪🇸 Spanisch (es-ES)
6. 🇯🇵 Japanisch (ja-JP)
7. 🇨🇳 Chinesisch (zh-CN)
8. 🇷🇺 Russisch (ru-RU)

**String-Kategorien:**
- UI Labels (StatusBar, Menus, Dialogs)
- Messages (Update, Error, Success)
- Tooltips
- Shortcuts (descriptions)
- Help texts

**LocalizationService.cs:**
```csharp
public interface ILocalizationService
{
    string GetString(string key);
    string GetString(string key, params object[] args);
    void SetLanguage(string languageCode);
    string GetCurrentLanguage();
    List<Language> GetAvailableLanguages();
}

public class Language
{
    public string Code { get; set; }
    public string DisplayName { get; set; }
    public string Flag { get; set; } // Unicode emoji
}
```

**Tasks:**
- [ ] Resources/Strings.resx erstellen (English)
- [ ] Alle UI-Strings identifizieren und extrahieren
- [ ] 7 weitere .resx files erstellen (initial mit Google Translate, später community)
- [ ] LocalizationService.cs implementieren
- [ ] Unit Tests
- [ ] Integration in alle UI-Komponenten

---

### Feature 1.3.2: StatusBar Implementation

**Was:** Statusleiste mit 5 Icons (standardmäßig VERSTECKT)

**StatusBarManager.cs:**
```csharp
public class StatusBarManager
{
    private StatusStrip _statusBar;
    private ToolStripStatusLabel _updateIcon;
    private ToolStripStatusLabel _explorerIcon;
    private ToolStripStatusLabel _languageSelector;
    private ToolStripStatusLabel _infoIcon;
    private ToolStripStatusLabel _helpIcon;

    private readonly IUpdateService _updateService;
    private readonly IRegistryService _registryService;
    private readonly ILocalizationService _localization;

    public StatusBarManager(/* dependencies */)
    {
        InitializeStatusBar();
        UpdateIcons();
    }

    public void Show() { _statusBar.Visible = true; }
    public void Hide() { _statusBar.Visible = false; }
    public void Toggle() { _statusBar.Visible = !_statusBar.Visible; }

    private void InitializeStatusBar()
    {
        // Update Status
        _updateIcon = new ToolStripStatusLabel
        {
            Text = "🔄",
            ToolTipText = _localization.GetString("statusbar.update.checking"),
            Enabled = false
        };
        _updateIcon.Click += UpdateIcon_Click;

        // Explorer Registration
        _explorerIcon = new ToolStripStatusLabel
        {
            Text = "⚙️",
            ToolTipText = _localization.GetString("statusbar.explorer.notRegistered")
        };
        _explorerIcon.Click += ExplorerIcon_Click;

        // Language Selector
        _languageSelector = new ToolStripDropDownButton
        {
            Text = "🌐 EN",
            ToolTipText = _localization.GetString("statusbar.language")
        };
        PopulateLanguageMenu();

        // Info Icon
        _infoIcon = new ToolStripStatusLabel
        {
            Text = "ℹ️",
            ToolTipText = _localization.GetString("statusbar.info")
        };
        _infoIcon.Click += InfoIcon_Click;

        // Help Icon
        _helpIcon = new ToolStripStatusLabel
        {
            Text = "❓",
            ToolTipText = _localization.GetString("statusbar.help")
        };
        _helpIcon.Click += HelpIcon_Click;

        _statusBar.Items.AddRange(new ToolStripItem[]
        {
            _updateIcon,
            new ToolStripSeparator(),
            _explorerIcon,
            new ToolStripStatusLabel { Spring = true }, // Spacer
            _languageSelector,
            _infoIcon,
            _helpIcon
        });
    }

    private async void UpdateIcon_Click(object sender, EventArgs e)
    {
        var updateInfo = await _updateService.CheckForUpdatesAsync();

        if (updateInfo.UpdateAvailable)
        {
            var result = MessageBox.Show(
                _localization.GetString("update.install.prompt", updateInfo.LatestVersion),
                _localization.GetString("update.title"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
                await _updateService.DownloadAndInstallAsync(updateInfo);
        }
    }

    private void ExplorerIcon_Click(object sender, EventArgs e)
    {
        if (_registryService.IsRegistered())
        {
            var result = MessageBox.Show(
                _localization.GetString("explorer.uninstall.prompt"),
                _localization.GetString("explorer.title"),
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
                _registryService.Uninstall();
        }
        else
        {
            var result = MessageBox.Show(
                _localization.GetString("explorer.install.prompt"),
                _localization.GetString("explorer.title"),
                MessageBoxButtons.YesNo
            );

            if (result == DialogResult.Yes)
                _registryService.Install();
        }

        UpdateIcons();
    }

    private void InfoIcon_Click(object sender, EventArgs e)
    {
        // Open GitHub README in browser
        Process.Start(new ProcessStartInfo
        {
            FileName = "https://github.com/nobiehl/mini-markdown-viewer#readme",
            UseShellExecute = true
        });
    }

    private void HelpIcon_Click(object sender, EventArgs e)
    {
        // Open help (F1 handler)
        ShowHelp();
    }

    public void UpdateIcons()
    {
        // Update icon states based on current status
        UpdateUpdateIcon();
        UpdateExplorerIcon();
    }

    private void UpdateUpdateIcon()
    {
        if (_updateService.IsUpdateAvailable)
        {
            _updateIcon.Text = "⚠️";
            _updateIcon.ForeColor = Color.Orange;
            _updateIcon.ToolTipText = _localization.GetString("statusbar.update.available");
            _updateIcon.Enabled = true;
        }
        else if (_updateService.IsUpToDate)
        {
            _updateIcon.Text = "✅";
            _updateIcon.ForeColor = Color.Green;
            _updateIcon.ToolTipText = _localization.GetString("statusbar.update.uptodate");
            _updateIcon.Enabled = false;
        }
    }

    private void UpdateExplorerIcon()
    {
        if (_registryService.IsRegistered())
        {
            _explorerIcon.Text = "✅";
            _explorerIcon.ForeColor = Color.Green;
            _explorerIcon.ToolTipText = _localization.GetString("statusbar.explorer.registered");
        }
        else
        {
            _explorerIcon.Text = "❌";
            _explorerIcon.ForeColor = Color.Gray;
            _explorerIcon.ToolTipText = _localization.GetString("statusbar.explorer.notRegistered");
        }
    }
}
```

**Context Menu:**
```csharp
private void BuildContextMenu()
{
    var menu = new ContextMenuStrip();

    var toggleItem = new ToolStripMenuItem
    {
        Text = _localization.GetString("menu.statusbar.toggle"),
        ShortcutKeys = Keys.Control | Keys.B,
        Checked = _statusBar.Visible
    };
    toggleItem.Click += (s, e) => Toggle();

    menu.Items.Add(toggleItem);
    menu.Items.Add(new ToolStripSeparator());

    // Individual icon toggles
    // ...

    _statusBar.ContextMenuStrip = menu;
}
```

**Tasks:**
- [ ] StatusBarManager.cs implementieren
- [ ] Context Menu mit Shortcuts
- [ ] Icon States (Update, Explorer)
- [ ] Click-Handler für alle Icons
- [ ] Unit Tests (mock dependencies)
- [ ] Integration in MainWindow
- [ ] Settings-Persistierung

---

### Feature 1.3.3: Theme Switcher UI

**Was:** Theme-Auswahl über Context Menu

**UI:**
```
Right-click → Theme →
    ◉ Standard (Enhanced)
    ○ Dark
    ○ Solarized Light
    ○ Dräger
```

**Implementation in ContextMenuBuilder:**
```csharp
private void BuildThemeMenu()
{
    var themeMenu = new ToolStripMenuItem
    {
        Text = _localization.GetString("menu.theme")
    };

    var themes = _themeService.GetAvailableThemes();
    var currentTheme = _settings.Theme;

    foreach (var themeName in themes)
    {
        var themeItem = new ToolStripMenuItem
        {
            Text = GetThemeDisplayName(themeName),
            Checked = themeName == currentTheme
        };

        themeItem.Click += (s, e) => SwitchTheme(themeName);
        themeMenu.DropDownItems.Add(themeItem);
    }

    return themeMenu;
}

private async void SwitchTheme(string themeName)
{
    var theme = _themeService.LoadTheme(themeName);
    _themeService.ApplyTheme(theme, _mainWindow, _webView);

    _settings.Theme = themeName;
    _settingsService.Save(_settings);

    // Reload current markdown with new theme
    await ReloadCurrentFile();
}
```

**Tasks:**
- [ ] Theme menu in Context
- [ ] Theme switching logic
- [ ] Live reload mit neuem Theme
- [ ] Settings persistence
- [ ] Test all 4 themes

---

### v1.3.0 Deliverables

**Code:**
- ✅ LocalizationService
- ✅ 8 .resx files (initial)
- ✅ StatusBarManager
- ✅ Theme Switcher UI
- ✅ All strings localized

**Tests:**
- ✅ Unit tests for all new services
- ✅ Test coverage >= 80%

**Documentation:**
- ✅ impl_progress.md updated
- ✅ GLOSSARY.md updated
- ✅ Screenshots of StatusBar

**Release:**
- ✅ Version 1.3.0
- ✅ All UI elements work
- ✅ Themes switchable
- ✅ Languages switchable

---

## v1.4.0: Navigation + Search

**Ziel:** Navigation und Suche implementieren

**Aufwand:** ~2-3 Sessions

### Feature 1.4.1: Navigation Implementation

**Was:** Back/Forward Navigation (WebView2-basiert)

**NavigationManager.cs:**
```csharp
public class NavigationManager
{
    private readonly WebView2 _webView;

    public bool CanGoBack => _webView.CoreWebView2?.CanGoBack ?? false;
    public bool CanGoForward => _webView.CoreWebView2?.CanGoForward ?? false;

    public event EventHandler NavigationChanged;

    public NavigationManager(WebView2 webView)
    {
        _webView = webView;

        _webView.CoreWebView2InitializationCompleted += (s, e) =>
        {
            _webView.CoreWebView2.HistoryChanged += (s2, e2) =>
                NavigationChanged?.Invoke(this, EventArgs.Empty);
        };
    }

    public void GoBack()
    {
        if (CanGoBack)
            _webView.CoreWebView2.GoBack();
    }

    public void GoForward()
    {
        if (CanGoForward)
            _webView.CoreWebView2.GoForward();
    }
}
```

**NavigationBar.cs (UI):**
```csharp
public class NavigationBar
{
    private ToolStrip _toolbar;
    private ToolStripButton _backButton;
    private ToolStripButton _forwardButton;

    private readonly NavigationManager _navManager;

    public NavigationBar(NavigationManager navManager)
    {
        _navManager = navManager;
        InitializeToolbar();

        _navManager.NavigationChanged += (s, e) => UpdateButtons();
    }

    private void InitializeToolbar()
    {
        _backButton = new ToolStripButton
        {
            Text = "←",
            ToolTipText = "Back (Alt+Left)",
            Enabled = false
        };
        _backButton.Click += (s, e) => _navManager.GoBack();

        _forwardButton = new ToolStripButton
        {
            Text = "→",
            ToolTipText = "Forward (Alt+Right)",
            Enabled = false
        };
        _forwardButton.Click += (s, e) => _navManager.GoForward();

        _toolbar = new ToolStrip();
        _toolbar.Items.AddRange(new[] { _backButton, _forwardButton });
        _toolbar.Visible = false; // Default hidden
    }

    private void UpdateButtons()
    {
        _backButton.Enabled = _navManager.CanGoBack;
        _forwardButton.Enabled = _navManager.CanGoForward;
    }

    public void Show() => _toolbar.Visible = true;
    public void Hide() => _toolbar.Visible = false;
    public void Toggle() => _toolbar.Visible = !_toolbar.Visible;
}
```

**Shortcuts:**
```csharp
protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    if (keyData == (Keys.Alt | Keys.Left))
    {
        _navigationManager.GoBack();
        return true;
    }

    if (keyData == (Keys.Alt | Keys.Right))
    {
        _navigationManager.GoForward();
        return true;
    }

    return base.ProcessCmdKey(ref msg, keyData);
}
```

**Tasks:**
- [ ] NavigationManager.cs
- [ ] NavigationBar.cs
- [ ] Shortcuts (Alt+Left/Right)
- [ ] Context menu toggle
- [ ] Settings persistence
- [ ] Unit tests
- [ ] Integration test

---

### Feature 1.4.2: Search Implementation

**Was:** In-Page-Suche mit Highlight (mark.js)

**SearchManager.cs:**
```csharp
public class SearchManager
{
    private readonly WebView2 _webView;
    private string _currentSearchTerm;
    private int _currentMatchIndex;
    private int _totalMatches;

    public event EventHandler<SearchResultsEventArgs> SearchResultsChanged;

    public async Task SearchAsync(string searchTerm)
    {
        _currentSearchTerm = searchTerm;
        _currentMatchIndex = 0;

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            await ClearSearchAsync();
            return;
        }

        // Inject mark.js if not already loaded
        await EnsureMarkJsLoadedAsync();

        // Perform search
        var script = $@"
            var instance = new Mark(document.body);
            instance.unmark();
            instance.mark('{EscapeJs(searchTerm)}', {{
                'className': 'search-highlight',
                'done': function(totalMatches) {{
                    window.chrome.webview.postMessage({{
                        type: 'search-results',
                        total: totalMatches
                    }});
                }}
            }});
        ";

        await _webView.ExecuteScriptAsync(script);
    }

    public async Task NextMatchAsync()
    {
        if (_totalMatches == 0) return;

        _currentMatchIndex = (_currentMatchIndex + 1) % _totalMatches;
        await ScrollToMatchAsync(_currentMatchIndex);
    }

    public async Task PreviousMatchAsync()
    {
        if (_totalMatches == 0) return;

        _currentMatchIndex = (_currentMatchIndex - 1 + _totalMatches) % _totalMatches;
        await ScrollToMatchAsync(_currentMatchIndex);
    }

    private async Task ScrollToMatchAsync(int index)
    {
        var script = $@"
            var marks = document.querySelectorAll('.search-highlight');
            if (marks[{index}]) {{
                marks[{index}].scrollIntoView({{ behavior: 'smooth', block: 'center' }});
                marks.forEach(m => m.classList.remove('current'));
                marks[{index}].classList.add('current');
            }}
        ";

        await _webView.ExecuteScriptAsync(script);
    }
}
```

**SearchBar.cs (UI):**
```csharp
public class SearchBar
{
    private Panel _searchPanel;
    private TextBox _searchBox;
    private Button _previousButton;
    private Button _nextButton;
    private Label _matchLabel;
    private Button _closeButton;

    private readonly SearchManager _searchManager;

    public SearchBar(SearchManager searchManager)
    {
        _searchManager = searchManager;
        InitializeSearchBar();

        _searchManager.SearchResultsChanged += (s, e) => UpdateMatchLabel(e);
    }

    private void InitializeSearchBar()
    {
        _searchPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            Visible = false,
            BackColor = Color.FromArgb(240, 240, 240)
        };

        _searchBox = new TextBox
        {
            Location = new Point(10, 10),
            Width = 200
        };
        _searchBox.TextChanged += async (s, e) => await _searchManager.SearchAsync(_searchBox.Text);
        _searchBox.KeyDown += SearchBox_KeyDown;

        _previousButton = new Button
        {
            Text = "↑",
            Location = new Point(220, 8),
            Width = 30
        };
        _previousButton.Click += async (s, e) => await _searchManager.PreviousMatchAsync();

        _nextButton = new Button
        {
            Text = "↓",
            Location = new Point(255, 8),
            Width = 30
        };
        _nextButton.Click += async (s, e) => await _searchManager.NextMatchAsync();

        _matchLabel = new Label
        {
            Location = new Point(295, 12),
            AutoSize = true,
            Text = "0 of 0"
        };

        _closeButton = new Button
        {
            Text = "✕",
            Location = new Point(400, 8),
            Width = 30
        };
        _closeButton.Click += (s, e) => Hide();

        _searchPanel.Controls.AddRange(new Control[]
        {
            _searchBox,
            _previousButton,
            _nextButton,
            _matchLabel,
            _closeButton
        });
    }

    private void SearchBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            if (e.Shift)
                _searchManager.PreviousMatchAsync();
            else
                _searchManager.NextMatchAsync();

            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Escape)
        {
            Hide();
            e.Handled = true;
        }
    }

    public void Show()
    {
        _searchPanel.Visible = true;
        _searchBox.Focus();
    }

    public void Hide()
    {
        _searchPanel.Visible = false;
        _searchManager.ClearSearchAsync();
    }
}
```

**Shortcuts:**
```csharp
protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
{
    if (keyData == (Keys.Control | Keys.F))
    {
        _searchBar.Show();
        return true;
    }

    if (keyData == Keys.F3)
    {
        _searchManager.NextMatchAsync();
        return true;
    }

    if (keyData == (Keys.Shift | Keys.F3))
    {
        _searchManager.PreviousMatchAsync();
        return true;
    }

    if (keyData == Keys.Escape && _searchBar.IsVisible)
    {
        _searchBar.Hide();
        return true;
    }

    return base.ProcessCmdKey(ref msg, keyData);
}
```

**CSS für Highlights:**
```css
.search-highlight {
    background-color: #ffff00;
    padding: 2px;
}

.search-highlight.current {
    background-color: #ff9900;
    font-weight: bold;
}
```

**Tasks:**
- [ ] SearchManager.cs
- [ ] SearchBar.cs (UI)
- [ ] mark.js integration
- [ ] Shortcuts (Ctrl+F, F3, Shift+F3, Esc)
- [ ] Match counter
- [ ] CSS styling
- [ ] Unit tests
- [ ] Integration test

---

### Feature 1.4.3: Mouse Gestures (Nice-to-have)

**Was:** Optional swipe gestures für Navigation

**Status:** POSTPONED to v1.6.0

**Reason:** Navigation buttons + shortcuts sind ausreichend. Mouse gestures als future enhancement.

---

### v1.4.0 Deliverables

**Code:**
- ✅ NavigationManager + NavigationBar
- ✅ SearchManager + SearchBar
- ✅ mark.js integration
- ✅ All shortcuts

**Tests:**
- ✅ Unit tests
- ✅ Test coverage >= 80%

**Documentation:**
- ✅ impl_progress.md
- ✅ GLOSSARY.md
- ✅ User guide for search

**Release:**
- ✅ Version 1.4.0
- ✅ Navigation works
- ✅ Search works

---

## v1.5.0: Polish + Documentation + Release

**Ziel:** Production-ready machen

**Aufwand:** ~2 Sessions

### Feature 1.5.1: Integration Testing

**Was:** End-to-End Tests für alle Workflows

**Test Scenarios:**
1. First Launch
   - [ ] Default settings loaded
   - [ ] Standard theme applied
   - [ ] All features hidden (clean start)

2. Settings Persistence
   - [ ] Change theme → restart → theme persisted
   - [ ] Change language → restart → language persisted
   - [ ] Toggle StatusBar → restart → state persisted

3. Update Flow
   - [ ] Check for updates
   - [ ] Download update
   - [ ] Install update
   - [ ] Verify update applied

4. Explorer Registration
   - [ ] Install → registry entries created
   - [ ] Uninstall → registry entries removed
   - [ ] Double-click .md file → opens in viewer

5. Theme Switching
   - [ ] All 4 themes render correctly
   - [ ] Theme applied to UI and Markdown
   - [ ] No visual glitches

6. Localization
   - [ ] All 8 languages load
   - [ ] UI strings translated
   - [ ] No missing translations

7. Navigation
   - [ ] Back/Forward work
   - [ ] History preserved
   - [ ] Shortcuts work

8. Search
   - [ ] Find matches
   - [ ] Navigate matches
   - [ ] Clear search
   - [ ] Shortcuts work

**Tasks:**
- [ ] Write integration tests
- [ ] Manual testing checklist
- [ ] Performance testing
- [ ] Memory leak testing

---

### Feature 1.5.2: Performance Optimization

**Areas:**
- [ ] Startup time (target < 2 seconds)
- [ ] Theme switching (target < 500ms)
- [ ] Large file handling (> 10MB)
- [ ] Memory usage (target < 100MB)
- [ ] Search performance (> 1000 matches)

---

### Feature 1.5.3: Documentation Completion

**Update all docs:**
- [ ] README.md (user-facing features)
- [ ] DEVELOPMENT.md (new architecture)
- [ ] DEPLOYMENT-GUIDE.md (new build process)
- [ ] ARCHITECTURE.md (full system overview)
- [ ] GLOSSARY.md (consolidated)
- [ ] impl_progress.md (final summary)

**Create new docs:**
- [ ] USER-GUIDE.md (comprehensive guide)
- [ ] LOCALIZATION-GUIDE.md (for translators)
- [ ] THEME-GUIDE.md (for custom themes)

---

### Feature 1.5.4: Release Preparation

**Tasks:**
- [ ] Version bump to 1.5.0
- [ ] Update all version strings
- [ ] Create CHANGELOG.md
- [ ] Build release binary
- [ ] Test on clean Windows installation
- [ ] Create GitHub release
- [ ] Update README badges
- [ ] Announce release

---

### v1.5.0 Deliverables

**Code:**
- ✅ All features complete
- ✅ All tests passing
- ✅ Performance optimized
- ✅ No known bugs

**Tests:**
- ✅ Integration tests complete
- ✅ Test coverage >= 80%
- ✅ Manual testing passed

**Documentation:**
- ✅ All docs updated
- ✅ New docs created
- ✅ GLOSSARY consolidated
- ✅ Screenshots updated

**Release:**
- ✅ Version 1.5.0 released
- ✅ Binary tested
- ✅ GitHub release created
- ✅ Community announcement

---

## v1.9.0: Raw Data View (Developer Tools)

**Ziel:** Toggle-View für Entwickler/Power-User um Markdown-Quelle und generiertes HTML parallel anzusehen

**Aufwand:** ~7 Stunden (1 Session)

**Priority:** Medium (Quality-of-Life Feature)

### Feature 1.9.1: Raw Data View Component

**Was:** Split-View Panel mit Syntax-Highlighting für Markdown und HTML

**Technische Details:**
- **Component:** `UI/RawDataViewPanel.cs` (neue Datei)
- **Library:** ScintillaNET v5.3.2 für Syntax-Highlighting
- **Binary Size Impact:** +2.2 MB (3.3 MB → 5.5 MB)
- **Architecture:** Panel mit SplitContainer (horizontal)

**Features:**
- ✅ Read-Only Mode (bleibt ein Viewer!)
- ✅ Syntax-Highlighting für Markdown (links) und HTML (rechts)
- ✅ Resizable Splitter (Position wird persistiert)
- ✅ Theme-Aware (Dark/Light/Solarized/Draeger Support)
- ✅ Keyboard-Shortcut: F12 (wie "View Source" in Browsern)
- ✅ Kontextmenü: "Weitere Tools" → "Rohdaten anzeigen (F12)"
- ✅ State-Persistenz: Splitter-Position in `settings.json`
- ✅ Vollständig lokalisiert in allen 8 Sprachen

**Tasks:**
- [x] Library evaluieren (ScintillaNET vs RichTextBox)
- [ ] RawDataViewPanel.cs erstellen (~250 lines)
- [ ] MainForm Integration (Toggle-Logik)
- [ ] F12 Shortcut implementieren
- [ ] Kontextmenü erweitern
- [ ] AppSettings erweitern (RawDataViewVisible, SplitterDistance)
- [ ] Lokalisierung (5 strings × 8 languages = 40 translations)
- [ ] Unit Tests schreiben
- [ ] Integration Tests durchführen
- [ ] Build & Test

**Test Strategy:**
- Unit Tests: RawDataViewPanelTests.cs (ShowRawData, Hide, SplitterDistance)
- Integration Tests: F12 Toggle, Kontextmenü, Settings-Persistenz, Theme-Wechsel
- Manual Tests: Performance (große Dateien), Theme-Compatibility

**Success Criteria:**
- ✅ F12 togglet zwischen Normal und Raw Data Mode
- ✅ Syntax-Highlighting funktioniert in beiden Panels
- ✅ Splitter-Position wird gespeichert
- ✅ Theme-Wechsel funktioniert im Raw Data Mode
- ✅ Alle Tests bestehen (>= 80% Coverage)
- ✅ Build ohne Fehler und Warnungen
- ✅ Binary Size ~5.5 MB (akzeptabel)

---

### Feature 1.9.2: Settings Extension

**Was:** AppSettings erweitern für Raw Data View State

**Changes in `AppSettings.cs`:**
```csharp
public class UISettings
{
    // ... existing properties ...

    public bool RawDataViewVisible { get; set; } = false;
    public int RawDataSplitterDistance { get; set; } = 500; // 50% default
}
```

---

### Feature 1.9.3: Localization

**Was:** 5 neue Resource-Strings in allen 8 Sprachen

**New Strings:**
- `RawDataViewShow` = "Show Raw Data" / "Rohdaten anzeigen"
- `RawDataViewHide` = "Hide Raw Data" / "Rohdaten ausblenden"
- `ContextMenuMoreTools` = "More Tools" / "Weitere Tools"
- `RawDataViewMarkdownLabel` = "Markdown Source" / "Markdown-Quelle"
- `RawDataViewHtmlLabel` = "Generated HTML" / "Generiertes HTML"

**Implementation:**
- Parallele Übersetzung mit 7 Agenten (wie in v1.8.0)
- Alle Strings mit `<comment>` Tags für Kontext

---

### v1.9.0 Deliverables

**Code:**
- ✅ RawDataViewPanel.cs (~250 lines)
- ✅ MainForm Integration (~50 lines modified)
- ✅ AppSettings extension (2 properties)
- ✅ ScintillaNET dependency added

**Tests:**
- ✅ 3 unit tests (RawDataViewPanelTests.cs)
- ✅ 6 integration test scenarios
- ✅ Manual testing passed (all themes, large files)
- ✅ Test coverage >= 80%

**Localization:**
- ✅ 5 strings × 8 languages = 40 translations
- ✅ All strings with context comments
- ✅ Tested in all 8 languages

**Documentation:**
- ✅ CHANGELOG.md updated
- ✅ USER-GUIDE.md: New "Raw Data View" section
- ✅ ARCHITECTURE.md: RawDataViewPanel documented
- ✅ GLOSSARY.md: New terms added
- ✅ impl_progress.md: Session entry

**Release:**
- ✅ Version 1.9.0 released
- ✅ Binary tested (~5.5 MB)
- ✅ GitHub release created with MarkdownViewer.exe (NO version in filename!)
- ✅ CHANGELOG.md is single source of truth

---

## Future Roadmap (v1.6.0+)

### Nice-to-have Features (nicht committet)
- Mouse Gestures for Navigation
- Custom Keyboard Shortcuts
- Plugin System for Extensions
- Export to PDF/HTML
- Recent Files List
- Bookmarks
- Print Support
- Custom CSS injection
- Multi-tab support?

---

## Metrics & KPIs

### Code Quality
- **Test Coverage:** >= 80%
- **Lines of Code:** MainWindow < 250, Program < 200
- **Cyclomatic Complexity:** < 10 per method
- **Tech Debt:** < 1 hour

### Performance
- **Startup Time:** < 2 seconds
- **Theme Switch:** < 500ms
- **Search (1000 matches):** < 1 second
- **Memory Usage:** < 100MB

### User Experience
- **Time to First Feature Use:** < 30 seconds
- **Theme Switch Satisfaction:** >= 90%
- **Search Accuracy:** 100%

---

## Risk Management

### Risks

1. **WebView2 Compatibility Issues**
   - *Mitigation:* Test on multiple Windows versions
   - *Fallback:* Graceful degradation

2. **Theme CSS Conflicts**
   - *Mitigation:* Use !important, test all themes
   - *Fallback:* Fallback to standard theme

3. **Localization Quality**
   - *Mitigation:* Community review for translations
   - *Fallback:* English fallback always available

4. **Performance with Large Files**
   - *Mitigation:* Lazy loading, pagination
   - *Fallback:* Warning for files > 10MB

5. **Settings Corruption**
   - *Mitigation:* JSON validation, backups
   - *Fallback:* Reset to defaults

---

## Dependencies

### External Libraries
- Markdig 0.37.0 (Markdown)
- WebView2 (Rendering)
- Serilog 4.0.0 (Logging)
- xUnit (Testing)
- Moq (Mocking)
- FluentAssertions (Test assertions)
- mark.js (Search highlighting)

### New Dependencies for v1.2+
- System.Text.Json (Settings)
- None! (Keep it minimal)

---

## Glossary Quick-Ref

See [GLOSSARY.md](GLOSSARY.md) for detailed definitions.

**Key Terms:**
- **Theme:** Visual appearance definition (colors, fonts)
- **StatusBar:** Bottom bar with status icons
- **Navigation:** Back/Forward through markdown links
- **Search:** Find text in current document
- **Localization:** Multi-language support
- **Settings:** User preferences (JSON)

---

**Last Updated:** 2025-11-05
**Status:** Planning Complete → Ready for Implementation
**Next:** Initialize impl_progress.md and start v1.2.0 implementation
