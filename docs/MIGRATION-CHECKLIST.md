# MarkdownViewer Cross-Platform Migration Checkliste

**Version:** 1.0
**Datum:** 2025-11-08
**Ziel:** Schrittweise Migration von WinForms zu Cross-Platform (Avalonia) mit Platform Abstractions

---

## Phase 1: Platform Abstractions (Windows-Only)
**Ziel:** Abstrakte Interfaces erstellen und Windows-Implementierungen aus bestehendem Code extrahieren
**Geschätzte Dauer:** 1-2 Wochen

### 1.1 Projekt-Struktur erstellen
- [ ] Neues Projekt `MarkdownViewer.Platform.Abstractions` erstellen (.NET 8 Class Library)
- [ ] Neues Projekt `MarkdownViewer.Platform.Windows` erstellen (.NET 8 Class Library)
- [ ] NuGet Dependencies hinzufügen:
  - [ ] `Microsoft.Extensions.DependencyInjection` (für DI)
  - [ ] `Serilog` (für Logging)

### 1.2 IFileAssociationService implementieren
- [ ] Interface `IFileAssociationService` in Abstractions Projekt erstellen
  - [ ] Kopiere Definition aus `PLATFORM-ABSTRACTIONS-INTERFACES.cs`
- [ ] Klassen `FileAssociationOptions` und `FileAssociationStatus` erstellen
- [ ] `WindowsFileAssociationService` implementieren:
  - [ ] Migriere Code aus `Program.cs` Zeile 199-279 (`InstallFileAssociation`)
  - [ ] Migriere Code aus `Program.cs` Zeile 314-347 (`UninstallFileAssociation`)
  - [ ] Implementiere `IsInstalledAsync()` (prüfe Registry Keys)
  - [ ] Implementiere `GetStatusAsync()` (detaillierter Status)
  - [ ] Migriere `CreateShortcut()` für Send To Menu
- [ ] Unit Tests schreiben:
  - [ ] Test `InstallAsync_CreatesRegistryKeys`
  - [ ] Test `UninstallAsync_RemovesRegistryKeys`
  - [ ] Test `IsInstalledAsync_ReturnsTrueWhenInstalled`
  - [ ] Test `GetStatusAsync_ReturnsCorrectStatus`

**Migration-Hinweise:**
```csharp
// ALT: Program.cs - Zeile 207-222
using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\.md"))
{
    key?.SetValue("", "MarkdownViewer.Document");
}

// NEU: WindowsFileAssociationService.cs
public async Task<bool> InstallAsync(string applicationPath, FileAssociationOptions? options = null)
{
    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Classes\.md"))
    {
        key?.SetValue("", "MarkdownViewer.Document");
    }
    // ... rest der Logic
    return true;
}
```

### 1.3 IPlatformService implementieren
- [ ] Interface `IPlatformService` in Abstractions Projekt erstellen
- [ ] Klassen `PlatformInfo` und `PlatformType` enum erstellen
- [ ] `WindowsPlatformService` implementieren:
  - [ ] `GetExecutablePath()` → ersetzt `Application.ExecutablePath`
  - [ ] `GetExecutableDirectory()` → ersetzt `Path.GetDirectoryName(Application.ExecutablePath)`
  - [ ] `GetApplicationDataFolder()` → `%APPDATA%\MarkdownViewer`
  - [ ] `GetLogsFolder()` → `exe folder\logs`
  - [ ] `GetCacheFolder()` → `%LOCALAPPDATA%\MarkdownViewer\Cache`
  - [ ] `GetTempFolder()` → `%TEMP%\MarkdownViewer`
  - [ ] `RestartApplicationAsync()` → Process.Start + Environment.Exit
  - [ ] `OpenUrlInBrowserAsync()` → Process.Start mit URL
  - [ ] `OpenFileWithDefaultHandlerAsync()` → Process.Start mit Datei
  - [ ] `ShowFileInExplorerAsync()` → `explorer.exe /select,"{path}"`
  - [ ] `GetPlatformInfo()` → System-Informationen sammeln
  - [ ] `IsRunningAsAdmin()` → WindowsIdentity prüfen
- [ ] Unit Tests schreiben:
  - [ ] Test `GetApplicationDataFolder_CreatesDirectory`
  - [ ] Test `GetLogsFolder_CreatesDirectory`
  - [ ] Test `GetPlatformInfo_ReturnsCorrectInfo`

**Migration-Hinweise:**
```csharp
// ALT: MainForm.cs - Zeile 255-256
string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
string logsFolder = Path.Combine(exeFolder, "logs");

// NEU: Dependency Injection
public MainForm(..., IPlatformService platformService)
{
    _platformService = platformService;
    string logsFolder = _platformService.GetLogsFolder();
}
```

### 1.4 IWebViewAdapter erweitern
- [ ] Erweitere `IWebViewAdapter` Interface:
  - [ ] Neue Properties hinzufügen: `CurrentUrl`, `PageTitle`, `ZoomLevel`, `AreDevToolsEnabled`, `UserAgent`
  - [ ] Neue Events hinzufügen: `NavigationCompleted`, `ErrorOccurred`, `PageTitleChanged`
  - [ ] Neue Methoden hinzufügen: `NavigateToUrlAsync`, `Stop`, `InjectCssAsync`, `RemoveCssAsync`, `RegisterJavaScriptInterface`, `InitializeAsync`, `DisposeAsync`
- [ ] Event Args Klassen erstellen: `NavigationCompletedEventArgs`, `WebViewErrorEventArgs`
- [ ] `WebViewInitializationOptions` Klasse erstellen
- [ ] Erweitere `WebView2Adapter`:
  - [ ] Implementiere neue Properties
  - [ ] Implementiere neue Events
  - [ ] Implementiere `InjectCssAsync()` für Theme-Wechsel ohne Reload
  - [ ] Implementiere `InitializeAsync()` mit Options
- [ ] Unit Tests schreiben:
  - [ ] Test `InjectCssAsync_UpdatesStyles`
  - [ ] Test `InitializeAsync_ConfiguresSettings`

**Migration-Hinweise:**
```csharp
// ALT: Theme-Wechsel erfordert komplettes Reload
LoadMarkdownFile(_currentFilePath);

// NEU: CSS-Injection ohne Reload
await _webViewAdapter.InjectCssAsync(GenerateThemeCSS(newTheme), "theme-style");
```

### 1.5 IDialogService erweitern
- [ ] Erweitere `IDialogService` Interface:
  - [ ] Async-Methoden hinzufügen: `ShowErrorAsync`, `ShowInfoAsync`, `ShowWarningAsync`, `ShowConfirmationAsync`, `ShowYesNoAsync`
  - [ ] File-Dialog-Methoden hinzufügen: `ShowOpenFileDialogAsync`, `ShowSaveFileDialogAsync`, `ShowFolderPickerDialogAsync`
  - [ ] Input-Dialog-Methode hinzufügen: `ShowInputDialogAsync`
  - [ ] Progress-Dialog-Methode hinzufügen: `ShowProgressDialogAsync`
- [ ] `IProgressDialog` Interface erstellen
- [ ] Erweitere `WinFormsDialogService`:
  - [ ] Implementiere async-Methoden als Wrapper um synchrone Methoden
  - [ ] Implementiere `ShowOpenFileDialogAsync()` mit `OpenFileDialog`
  - [ ] Implementiere `ShowSaveFileDialogAsync()` mit `SaveFileDialog`
  - [ ] Implementiere `ShowFolderPickerDialogAsync()` mit `FolderBrowserDialog`
- [ ] Unit Tests schreiben (mit Mocks):
  - [ ] Test `ShowYesNoAsync_ReturnsYes`
  - [ ] Test `ShowOpenFileDialogAsync_ReturnsPath`

### 1.6 Dependency Injection Setup
- [ ] Erstelle `PlatformServiceExtensions` Klasse:
  - [ ] `AddPlatformServices()` Extension Method
  - [ ] `AddWinFormsServices()` Extension Method
  - [ ] Platform-Detection via `OperatingSystem.IsWindows()`
- [ ] Migriere `Program.cs BuildServiceProvider()`:
  - [ ] ALT: Manuelle Service-Registrierung → NEU: Extension Methods
  - [ ] Registriere Platform Services automatisch basierend auf OS
- [ ] Unit Tests schreiben:
  - [ ] Test `AddPlatformServices_Windows_RegistersWindowsServices`

**Migration-Hinweise:**
```csharp
// ALT: Program.cs - Zeile 565-605
private static ServiceProvider BuildServiceProvider(string filePath, LogEventLevel logLevel)
{
    var services = new ServiceCollection();
    services.AddSingleton<ISettingsService, SettingsService>();
    services.AddSingleton<IThemeService, ThemeService>();
    // ...
}

// NEU: Platform-aware DI
private static ServiceProvider BuildServiceProvider(string filePath, LogEventLevel logLevel)
{
    var services = new ServiceCollection();

    // Platform Services (auto-detect)
    services.AddPlatformServices();

    // UI Framework Services
    services.AddWinFormsServices();

    // Core Services
    services.AddSingleton<ISettingsService, SettingsService>();
    services.AddSingleton<IThemeService, ThemeService>();

    return services.BuildServiceProvider();
}
```

### 1.7 MainForm.cs migrieren (Windows)
- [ ] Dependency Injection in `MainForm` Constructor:
  - [ ] `IPlatformService` injizieren
  - [ ] `IFileAssociationService` injizieren (optional, für Status-Check)
- [ ] Ersetze direkte Platform-Calls:
  - [ ] `Application.ExecutablePath` → `_platformService.GetExecutablePath()`
  - [ ] `Path.GetDirectoryName(Application.ExecutablePath)` → `_platformService.GetExecutableDirectory()`
  - [ ] Logs Folder → `_platformService.GetLogsFolder()`
  - [ ] Cache Folder → `_platformService.GetCacheFolder()`
- [ ] Teste, dass Applikation weiterhin funktioniert

**Verifizierung:**
- [ ] Applikation startet erfolgreich
- [ ] File Association Installation funktioniert
- [ ] Logs werden im richtigen Ordner erstellt
- [ ] WebView2 Cache wird im richtigen Ordner erstellt
- [ ] Alle bestehenden Unit Tests laufen durch

---

## Phase 2: Shared Library extrahieren
**Ziel:** Business Logic von UI trennen
**Geschätzte Dauer:** 1 Woche

### 2.1 Projekt-Struktur
- [ ] Neues Projekt `MarkdownViewer.Core` erstellen (.NET 8 Class Library)
- [ ] TargetFramework: `net8.0` (keine Windows-Abhängigkeit!)
- [ ] NuGet Dependencies hinzufügen:
  - [ ] `Markdig` (Markdown Parser)
  - [ ] `Serilog` (Logging)
  - [ ] `System.Text.Json` (Settings Serialization)

### 2.2 Models verschieben
- [ ] `Theme.cs` → `MarkdownViewer.Core/Models/`
- [ ] `AppSettings.cs` → `MarkdownViewer.Core/Models/`
- [ ] `GitHubRelease.cs` → `MarkdownViewer.Core/Models/`
- [ ] Namespace anpassen: `MarkdownViewer.Core.Models`

### 2.3 Services verschieben
- [ ] `ThemeService.cs` → `MarkdownViewer.Core/Services/`
- [ ] `SettingsService.cs` → `MarkdownViewer.Core/Services/`
- [ ] `LocalizationService.cs` → `MarkdownViewer.Core/Services/`
- [ ] **WICHTIG:** Service Interfaces (`IThemeService`, `ISettingsService`, `ILocalizationService`) auch verschieben
- [ ] Namespace anpassen: `MarkdownViewer.Core.Services`
- [ ] Prüfe Dependencies auf Windows-spezifische APIs:
  - [ ] `System.Windows.Forms` → Entfernen (nur in Platform-Implementierungen erlaubt)
  - [ ] `Microsoft.Web.WebView2` → Entfernen (nutze `IWebViewAdapter`)

### 2.4 Core Logic verschieben
- [ ] `MarkdownRenderer.cs` → `MarkdownViewer.Core/Rendering/`
- [ ] `NavigationManager.cs` → `MarkdownViewer.Core/Navigation/`
- [ ] `SearchManager.cs` → `MarkdownViewer.Core/Search/`
- [ ] `FileWatcherManager.cs` → `MarkdownViewer.Core/IO/`
- [ ] `UpdateChecker.cs` → `MarkdownViewer.Core/Updates/`
- [ ] Namespace anpassen

### 2.5 Presenters verschieben
- [ ] `MainPresenter.cs` → `MarkdownViewer.Core/Presenters/`
- [ ] `StatusBarPresenter.cs` → `MarkdownViewer.Core/Presenters/`
- [ ] `SearchBarPresenter.cs` → `MarkdownViewer.Core/Presenters/`
- [ ] `NavigationPresenter.cs` → `MarkdownViewer.Core/Presenters/`
- [ ] Namespace anpassen

### 2.6 Platform Abstractions referenzieren
- [ ] `MarkdownViewer.Core` referenziert `MarkdownViewer.Platform.Abstractions`
- [ ] Services nutzen Interfaces statt konkrete Implementierungen:
  - [ ] `ThemeService` nutzt `IWebViewAdapter` (nicht `WebView2`)
  - [ ] `UpdateChecker` nutzt `IPlatformService.RestartApplicationAsync()`

### 2.7 WinForms-Projekt anpassen
- [ ] `MarkdownViewer` (WinForms) referenziert:
  - [ ] `MarkdownViewer.Core`
  - [ ] `MarkdownViewer.Platform.Abstractions`
  - [ ] `MarkdownViewer.Platform.Windows`
- [ ] Entferne verschobene Klassen aus WinForms-Projekt
- [ ] Aktualisiere Namespaces in bestehenden Dateien
- [ ] Teste Kompilierung

**Verifizierung:**
- [ ] Lösung kompiliert ohne Fehler
- [ ] Keine `System.Windows.Forms` References in `MarkdownViewer.Core`
- [ ] Keine `Microsoft.Web.WebView2` References in `MarkdownViewer.Core`
- [ ] WinForms-App startet und funktioniert wie zuvor

---

## Phase 3: Linux/macOS Platform Services
**Ziel:** Platform Abstractions für Linux und macOS implementieren
**Geschätzte Dauer:** 1-2 Wochen

### 3.1 Linux Platform Services
- [ ] Neues Projekt `MarkdownViewer.Platform.Linux` erstellen (.NET 8 Class Library)
- [ ] `LinuxPlatformService` implementieren:
  - [ ] `GetApplicationDataFolder()` → `~/.config/MarkdownViewer` (XDG_CONFIG_HOME)
  - [ ] `GetCacheFolder()` → `~/.cache/MarkdownViewer` (XDG_CACHE_HOME)
  - [ ] `GetLogsFolder()` → `~/.local/share/MarkdownViewer/logs`
  - [ ] `ShowFileInExplorerAsync()` → `nautilus --select`, `dolphin --select`, etc.
  - [ ] `IsRunningAsAdmin()` → prüfe `EUID == 0`
- [ ] `LinuxFileAssociationService` implementieren:
  - [ ] `.desktop` File generieren und schreiben
  - [ ] `xdg-mime default` aufrufen
  - [ ] `update-desktop-database` aufrufen
  - [ ] `IsInstalledAsync()` → prüfe `.desktop` File Existenz
- [ ] Unit Tests schreiben (Linux-spezifisch):
  - [ ] Test `GetApplicationDataFolder_RespectsXDGConfigHome`
  - [ ] Test `InstallAsync_CreatesDesktopFile`

### 3.2 macOS Platform Services
- [ ] Neues Projekt `MarkdownViewer.Platform.MacOS` erstellen (.NET 8 Class Library)
- [ ] `MacOSPlatformService` implementieren:
  - [ ] `GetApplicationDataFolder()` → `~/Library/Application Support/MarkdownViewer`
  - [ ] `GetCacheFolder()` → `~/Library/Caches/MarkdownViewer`
  - [ ] `GetLogsFolder()` → `~/Library/Logs/MarkdownViewer`
  - [ ] `ShowFileInExplorerAsync()` → `open -R "{path}"`
  - [ ] `IsRunningAsAdmin()` → prüfe `USER == root`
- [ ] `MacOSFileAssociationService` implementieren:
  - [ ] `lsregister` aufrufen
  - [ ] `duti` für UTI-Registrierung nutzen
  - [ ] `IsInstalledAsync()` → prüfe Launch Services
- [ ] Unit Tests schreiben (macOS-spezifisch):
  - [ ] Test `GetApplicationDataFolder_UsesLibraryFolder`
  - [ ] Test `InstallAsync_RegistersInLaunchServices`

### 3.3 DI Extension Methods erweitern
- [ ] `AddPlatformServices()` erweitern:
  - [ ] `OperatingSystem.IsLinux()` → registriere Linux Services
  - [ ] `OperatingSystem.IsMacOS()` → registriere macOS Services

**Verifizierung:**
- [ ] Linux Services kompilieren auf Linux
- [ ] macOS Services kompilieren auf macOS
- [ ] DI registriert korrekte Implementierung basierend auf OS

---

## Phase 4: Avalonia UI implementieren
**Ziel:** Cross-Platform UI mit Avalonia
**Geschätzte Dauer:** 3-4 Wochen

### 4.1 Avalonia Projekt erstellen
- [ ] Neues Projekt `MarkdownViewer.Avalonia` erstellen
  - [ ] Avalonia 11.x Template
  - [ ] TargetFrameworks: `net8.0-windows;net8.0-linux;net8.0-osx`
- [ ] NuGet Dependencies hinzufügen:
  - [ ] `Avalonia` (11.x)
  - [ ] `Avalonia.Desktop`
  - [ ] `Avalonia.Themes.Fluent`
  - [ ] `Avalonia.WebView` (für Cross-Platform WebView)
  - [ ] `MarkdownViewer.Core`
  - [ ] `MarkdownViewer.Platform.Abstractions`
- [ ] Platform-spezifische Referenzen:
  - [ ] Windows: `MarkdownViewer.Platform.Windows`
  - [ ] Linux: `MarkdownViewer.Platform.Linux`
  - [ ] macOS: `MarkdownViewer.Platform.MacOS`

### 4.2 AvaloniaWebViewAdapter implementieren
- [ ] `AvaloniaWebViewAdapter.cs` erstellen (implementiert `IWebViewAdapter`)
- [ ] Platform-spezifische Backends unterstützen:
  - [ ] Windows: WebView2
  - [ ] Linux: WebKitGTK
  - [ ] macOS: WKWebView
- [ ] `InitializeAsync()` implementieren:
  - [ ] Windows: `EnsureCoreWebView2Async()` mit UserDataFolder
  - [ ] Linux: WebKitGTK Settings
  - [ ] macOS: WKWebView Configuration
- [ ] `InjectCssAsync()` implementieren (für Theme-Wechsel)
- [ ] `RegisterJavaScriptInterface()` implementieren
- [ ] Unit Tests schreiben:
  - [ ] Test `InitializeAsync_Windows_UsesWebView2`
  - [ ] Test `InjectCssAsync_UpdatesStyles`

### 4.3 AvaloniaDialogService implementieren
- [ ] `AvaloniaDialogService.cs` erstellen (implementiert `IDialogService`)
- [ ] `ContentDialog` für Meldungen nutzen
- [ ] `StorageProvider.OpenFilePickerAsync()` für Datei-Dialoge
- [ ] `AvaloniaProgressDialog` implementieren (Custom Window mit ProgressBar)
- [ ] Unit Tests schreiben:
  - [ ] Test `ShowErrorAsync_DisplaysDialog`
  - [ ] Test `ShowOpenFileDialogAsync_ReturnsPath`

### 4.4 MainWindow erstellen (Avalonia)
- [ ] `MainWindow.axaml` erstellen (entspricht `MainForm.cs`)
- [ ] Layout:
  - [ ] DockPanel als Root
  - [ ] NavigationBar (Top)
  - [ ] SearchBar (Top, unterhalb NavigationBar)
  - [ ] WebView (Center)
  - [ ] StatusBar (Bottom)
- [ ] `MainWindow.axaml.cs` Code-Behind:
  - [ ] Dependency Injection im Constructor
  - [ ] Event Handlers für UI-Interaktionen
  - [ ] Theme-Wechsel via `IThemeService`
- [ ] `IMainView` Interface implementieren (MVP Pattern)

### 4.5 StatusBar implementieren (Avalonia)
- [ ] `StatusBar.axaml` erstellen
- [ ] UI-Elemente:
  - [ ] Language Dropdown (ComboBox)
  - [ ] Theme Dropdown (ComboBox)
  - [ ] Update Status Icon (Button mit Icon)
  - [ ] Explorer Status Icon (Button mit Icon)
  - [ ] Info Icon (Button mit Icon)
  - [ ] Help Icon (Button mit Icon)
- [ ] `StatusBar.axaml.cs` Code-Behind
- [ ] `IStatusBarView` Interface implementieren

### 4.6 NavigationBar implementieren (Avalonia)
- [ ] `NavigationBar.axaml` erstellen
- [ ] UI-Elemente:
  - [ ] Back Button
  - [ ] Forward Button
- [ ] `INavigationBarView` Interface implementieren

### 4.7 SearchBar implementieren (Avalonia)
- [ ] `SearchBar.axaml` erstellen
- [ ] UI-Elemente:
  - [ ] Search TextBox
  - [ ] Previous Button
  - [ ] Next Button
  - [ ] Results Label
  - [ ] Close Button
- [ ] `ISearchBarView` Interface implementieren

### 4.8 Theme-System für Avalonia
- [ ] `AvaloniaThemeRenderer` erstellen
- [ ] `GenerateAvaloniaStyles()` Methode:
  - [ ] Konvertiere JSON Theme zu `ResourceDictionary`
  - [ ] Erstelle `SolidColorBrush` Resources
  - [ ] Erstelle Styles für Controls (Window, TextBox, Button, etc.)
- [ ] `ApplyThemeAsync()` implementieren:
  - [ ] `Application.Current.Resources.MergedDictionaries` aktualisieren
  - [ ] CSS in WebView injizieren
- [ ] Teste Theme-Wechsel zur Laufzeit

### 4.9 App.axaml & Program.cs
- [ ] `App.axaml` erstellen (Avalonia Application)
  - [ ] FluentTheme als Base Theme
  - [ ] Custom Styles
- [ ] `Program.cs` erstellen:
  - [ ] `BuildAvaloniaApp()` mit `UsePlatformDetect()`
  - [ ] DI Setup mit `AddPlatformServices()` und `AddAvaloniaServices()`
  - [ ] `StartWithClassicDesktopLifetime()`

**Verifizierung:**
- [ ] Avalonia App startet auf Windows
- [ ] Avalonia App startet auf Linux (VM oder WSL)
- [ ] Avalonia App startet auf macOS (falls verfügbar)
- [ ] Theme-Wechsel funktioniert
- [ ] File Association funktioniert

---

## Phase 5: Testing & Deployment
**Ziel:** Umfassende Tests und Release
**Geschätzte Dauer:** 1-2 Wochen

### 5.1 Integration Tests
- [ ] End-to-End Tests für WinForms:
  - [ ] Test Markdown-Rendering
  - [ ] Test Theme-Wechsel
  - [ ] Test File Association Installation
  - [ ] Test Navigation (Back/Forward)
  - [ ] Test Search
- [ ] End-to-End Tests für Avalonia:
  - [ ] Gleiche Tests wie WinForms
  - [ ] Platform-spezifische Tests (Windows/Linux/macOS)

### 5.2 Performance Tests
- [ ] Benchmark: Theme-Wechsel (CSS-Injection vs. Reload)
- [ ] Benchmark: Markdown-Rendering (große Dateien)
- [ ] Benchmark: Startup-Zeit

### 5.3 Build Pipeline (CI/CD)
- [ ] GitHub Actions Workflow erstellen:
  - [ ] Windows Build (WinForms + Avalonia)
  - [ ] Linux Build (Avalonia)
  - [ ] macOS Build (Avalonia)
- [ ] Unit Tests in CI ausführen
- [ ] Integration Tests in CI ausführen
- [ ] Artifacts erstellen:
  - [ ] Windows: `MarkdownViewer-win-x64.exe` (Single File)
  - [ ] Linux: AppImage / Flatpak / .deb
  - [ ] macOS: `.app` Bundle / `.dmg`

### 5.4 Deployment-Pakete
- [ ] Windows:
  - [ ] Portable EXE (keine Installation)
  - [ ] Optional: Installer (NSIS/WiX)
- [ ] Linux:
  - [ ] AppImage (empfohlen)
  - [ ] Flatpak (Flathub Submission)
  - [ ] .deb für Ubuntu/Debian
  - [ ] .rpm für Fedora/RHEL
- [ ] macOS:
  - [ ] .app Bundle (Code-signed)
  - [ ] .dmg Installer
  - [ ] Notarization via Apple Notary Service

### 5.5 Dokumentation
- [ ] README.md aktualisieren:
  - [ ] Cross-Platform Support erwähnen
  - [ ] Installation Instructions für alle Plattformen
  - [ ] Screenshots für Windows/Linux/macOS
- [ ] CHANGELOG.md erstellen:
  - [ ] v2.0: Cross-Platform Support
  - [ ] Breaking Changes dokumentieren (falls vorhanden)
- [ ] Migration Guide für Nutzer:
  - [ ] Settings-Migration von v1.x zu v2.x
  - [ ] Bekannte Probleme

### 5.6 Release
- [ ] GitHub Release erstellen:
  - [ ] Tag: `v2.0.0`
  - [ ] Release Notes
  - [ ] Artifacts hochladen
- [ ] Flathub Submission (falls Flatpak gewählt)
- [ ] macOS App Store Submission (optional)

**Finale Verifizierung:**
- [ ] Windows: Alle Features funktionieren
- [ ] Linux: Alle Features funktionieren
- [ ] macOS: Alle Features funktionieren
- [ ] File Association funktioniert auf allen Plattformen
- [ ] Theme-Wechsel funktioniert auf allen Plattformen
- [ ] Update-Mechanismus funktioniert (Windows: Self-Update, Linux: Package Manager)

---

## Zusammenfassung

**Geschätzte Gesamtzeit:** 6-8 Wochen

**Reihenfolge:**
1. Phase 1: Platform Abstractions (1-2 Wochen)
2. Phase 2: Shared Library (1 Woche)
3. Phase 3: Linux/macOS Services (1-2 Wochen)
4. Phase 4: Avalonia UI (3-4 Wochen)
5. Phase 5: Testing & Deployment (1-2 Wochen)

**Kritische Pfade:**
- `IWebViewAdapter` Erweiterung (benötigt für Theme-System)
- Avalonia.WebView Integration (Cross-Platform WebView)
- File Association Services (plattformspezifisch)

**Risiken:**
- macOS Code Signing (erfordert Developer Account $99/Jahr)
- Linux File Manager Varianz (Nautilus, Dolphin, Thunar, etc.)
- Avalonia.WebView Stabilität auf Linux (WebKitGTK Dependencies)

**Ende der Checkliste**
