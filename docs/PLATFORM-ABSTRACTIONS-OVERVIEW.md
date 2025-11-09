# Platform Abstractions Overview

**Erstellt:** 2025-11-08
**Für:** MarkdownViewer Cross-Platform Migration
**Status:** Design Phase abgeschlossen

---

## Übersicht der erstellten Dokumentation

Diese Dokumentation definiert vollständige Platform Abstractions für die Migration von MarkdownViewer von WinForms zu einer Cross-Platform Architektur (Windows/Linux/macOS) mit Avalonia UI.

### Erstellte Dateien

| Datei | Größe | Beschreibung |
|-------|-------|--------------|
| `PLATFORM-ABSTRACTIONS-DESIGN.md` | 59 KB | Vollständiges Design-Dokument mit allen Interfaces, plattformspezifischen Implementierungen und Migrationsanleitungen |
| `PLATFORM-ABSTRACTIONS-INTERFACES.cs` | 24 KB | Vollständige C# Interface-Definitionen zum direkten Kopieren in dein Projekt |
| `MIGRATION-CHECKLIST.md` | 21 KB | Schritt-für-Schritt Checkliste für die komplette Migration (6-8 Wochen) |
| `PLATFORM-ABSTRACTIONS-OVERVIEW.md` | Dieses Dokument | Übersicht und Schnelleinstieg |

---

## Schnelleinstieg

### 1. Design-Dokument lesen
**Datei:** `PLATFORM-ABSTRACTIONS-DESIGN.md`

**Inhalt:**
- Ausführliche Beschreibung aller 5 Platform Abstractions
- Plattformspezifische Implementierungsdetails (Windows/Linux/macOS)
- Migration vom bestehenden Code
- Edge Cases und Lösungsansätze
- Performance Considerations
- Testing Strategy

**Wann lesen?** Bevor du mit der Implementierung beginnst, um das Gesamtkonzept zu verstehen.

### 2. Interfaces übernehmen
**Datei:** `PLATFORM-ABSTRACTIONS-INTERFACES.cs`

**Inhalt:**
- Vollständige C# Definitionen aller Interfaces
- Alle benötigten Klassen und Enums
- Copy-Paste-ready Code

**Wie verwenden?**
```bash
# 1. Neues Projekt erstellen
dotnet new classlib -n MarkdownViewer.Platform.Abstractions -f net8.0

# 2. Interfaces kopieren
# Kopiere den Inhalt von PLATFORM-ABSTRACTIONS-INTERFACES.cs
# in dein neues Projekt

# 3. Referenzen hinzufügen
cd MarkdownViewer
dotnet add reference ../MarkdownViewer.Platform.Abstractions
```

### 3. Migration durchführen
**Datei:** `MIGRATION-CHECKLIST.md`

**Inhalt:**
- 5 Phasen mit detaillierten Checkboxen
- Schritt-für-Schritt Anleitung
- Verifizierungspunkte nach jeder Phase
- Code-Beispiele für Migration

**Wie verwenden?**
1. Starte mit Phase 1 (Platform Abstractions Windows-Only)
2. Arbeite die Checkboxen sequenziell ab
3. Teste nach jeder Phase
4. Erst zur nächsten Phase, wenn alle Checkboxen abgehakt sind

---

## Die 5 Platform Abstractions im Überblick

### 1. IFileAssociationService
**Zweck:** Windows Explorer / Linux .desktop / macOS Launch Services Integration

**Kernmethoden:**
- `InstallAsync()` - Registriert App als Standard-Handler für .md Dateien
- `UninstallAsync()` - Entfernt Registrierung
- `IsInstalledAsync()` - Prüft Registrierungsstatus
- `GetStatusAsync()` - Detaillierter Status mit allen Integrationspunkten

**Migration:**
```csharp
// ALT: Program.cs - InstallFileAssociation()
public static void InstallFileAssociation() { /* Registry Code */ }

// NEU: WindowsFileAssociationService
var service = serviceProvider.GetService<IFileAssociationService>();
await service.InstallAsync(Application.ExecutablePath);
```

### 2. IPlatformService
**Zweck:** OS-spezifische Operationen (Pfade, Prozesse, System-Info)

**Kernmethoden:**
- `GetExecutablePath()` - Ersetzt `Application.ExecutablePath`
- `GetApplicationDataFolder()` - User-spezifischer App-Ordner
- `GetLogsFolder()` - Log-Dateien Ordner
- `GetCacheFolder()` - Cache-Ordner (z.B. für WebView2)
- `RestartApplicationAsync()` - App neu starten
- `OpenUrlInBrowserAsync()` - URL im Browser öffnen
- `ShowFileInExplorerAsync()` - Datei im Explorer zeigen

**Migration:**
```csharp
// ALT: Direkte Aufrufe
string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
string logsFolder = Path.Combine(exeFolder, "logs");

// NEU: Über Service
string logsFolder = _platformService.GetLogsFolder();
```

### 3. IWebViewAdapter
**Zweck:** Plattformunabhängiger WebView (WebView2/WebKitGTK/WKWebView)

**Neue Features:**
- `InjectCssAsync()` - CSS ohne HTML-Neurendering injizieren (Theme-Wechsel)
- `RegisterJavaScriptInterface()` - C# Methoden aus JavaScript aufrufen
- `InitializeAsync()` - Plattformspezifische Initialisierung
- `ZoomLevel`, `CurrentUrl`, `PageTitle` Properties

**Migration:**
```csharp
// ALT: Theme-Wechsel mit komplettem Reload
LoadMarkdownFile(_currentFilePath); // 500ms

// NEU: CSS-Injection ohne Reload
await _webViewAdapter.InjectCssAsync(GenerateThemeCSS(newTheme), "theme-style"); // 50ms
```

### 4. IDialogService
**Zweck:** Plattformunabhängige Dialogs (MessageBox/ContentDialog)

**Neue Features:**
- Alle Methoden haben async-Varianten
- `ShowOpenFileDialogAsync()` - Datei öffnen
- `ShowSaveFileDialogAsync()` - Datei speichern
- `ShowInputDialogAsync()` - Text-Eingabe
- `ShowProgressDialogAsync()` - Progress-Dialog (nicht blockierend)

**Migration:**
```csharp
// ALT: Synchroner MessageBox
MessageBox.Show("Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

// NEU: Async Dialog (Avalonia-kompatibel)
await _dialogService.ShowErrorAsync("Error", "Error");
```

### 5. IThemeService
**Zweck:** Plattformunabhängiges Theme-System

**Kernmethoden:**
- `LoadTheme()` - Theme aus JSON laden
- `ApplyThemeAsync()` - Theme auf UI anwenden (WinForms oder Avalonia)
- `GenerateMarkdownCss()` - CSS für WebView generieren
- `GeneratePlatformStyles()` - Plattformspezifische Styles (ResourceDictionary oder WinForms Properties)

**Migration:**
```csharp
// ALT: Direkte WinForms-Anwendung
form.BackColor = ColorTranslator.FromHtml(theme.UI.FormBackground);

// NEU: Plattformunabhängig
await _themeService.ApplyThemeAsync(theme, form, _webView);
// Works mit WinForms Form oder Avalonia Window!
```

---

## Architektur-Übersicht

```
MarkdownViewer Solution
│
├── MarkdownViewer.Core                    # Shared Business Logic (plattformunabhängig)
│   ├── Models/                            # Theme, AppSettings, etc.
│   ├── Services/                          # ThemeService, SettingsService, LocalizationService
│   ├── Rendering/                         # MarkdownRenderer
│   ├── Navigation/                        # NavigationManager
│   ├── Search/                            # SearchManager
│   └── Presenters/                        # MVP Presenters (MainPresenter, etc.)
│
├── MarkdownViewer.Platform.Abstractions   # Interfaces (plattformunabhängig)
│   ├── IFileAssociationService.cs
│   ├── IPlatformService.cs
│   ├── IWebViewAdapter.cs
│   ├── IDialogService.cs
│   └── IThemeService.cs
│
├── MarkdownViewer.Platform.Windows        # Windows-Implementierungen
│   ├── WindowsFileAssociationService.cs
│   ├── WindowsPlatformService.cs
│   ├── WebView2Adapter.cs (erweitert)
│   └── WinFormsDialogService.cs (erweitert)
│
├── MarkdownViewer.Platform.Linux          # Linux-Implementierungen
│   ├── LinuxFileAssociationService.cs
│   ├── LinuxPlatformService.cs
│   └── (WebViewAdapter kommt von Avalonia.WebView)
│
├── MarkdownViewer.Platform.MacOS          # macOS-Implementierungen
│   ├── MacOSFileAssociationService.cs
│   ├── MacOSPlatformService.cs
│   └── (WebViewAdapter kommt von Avalonia.WebView)
│
├── MarkdownViewer (WinForms)              # Bestehende WinForms UI
│   ├── MainForm.cs
│   ├── StatusBarControl.cs
│   └── Program.cs
│
└── MarkdownViewer.Avalonia                # Neue Cross-Platform UI
    ├── Views/
    │   ├── MainWindow.axaml
    │   ├── StatusBar.axaml
    │   └── NavigationBar.axaml
    ├── AvaloniaWebViewAdapter.cs
    ├── AvaloniaDialogService.cs
    └── Program.cs
```

### Dependency Flow
```
┌─────────────────────────────────────┐
│  MarkdownViewer (WinForms)          │
│  oder                               │
│  MarkdownViewer.Avalonia            │
└───────────────┬─────────────────────┘
                │
                ├─→ MarkdownViewer.Core
                │
                ├─→ MarkdownViewer.Platform.Abstractions
                │
                └─→ MarkdownViewer.Platform.{Windows|Linux|MacOS}
                    (je nach OS via DI registriert)
```

---

## Migrations-Timeline

### Woche 1-2: Phase 1 - Platform Abstractions (Windows)
- [x] Interfaces definieren
- [ ] Windows-Implementierungen erstellen
- [ ] Unit Tests schreiben
- [ ] MainForm.cs migrieren

**Deliverable:** Funktionierende WinForms App mit Platform Abstractions

### Woche 3: Phase 2 - Shared Library
- [ ] MarkdownViewer.Core Projekt erstellen
- [ ] Business Logic verschieben
- [ ] Abhängigkeiten von WinForms entfernen

**Deliverable:** Plattformunabhängige Core Library

### Woche 4-5: Phase 3 - Linux/macOS Services
- [ ] Linux Platform Services implementieren
- [ ] macOS Platform Services implementieren
- [ ] Unit Tests schreiben

**Deliverable:** Platform Services für alle Plattformen

### Woche 6-9: Phase 4 - Avalonia UI
- [ ] Avalonia Projekt erstellen
- [ ] Views implementieren (MainWindow, StatusBar, etc.)
- [ ] AvaloniaWebViewAdapter implementieren
- [ ] Theme-System für Avalonia

**Deliverable:** Funktionsfähige Avalonia App (Windows/Linux/macOS)

### Woche 10-11: Phase 5 - Testing & Release
- [ ] Integration Tests
- [ ] Build Pipeline (CI/CD)
- [ ] Deployment-Pakete erstellen
- [ ] Release v2.0

**Deliverable:** Release mit WinForms (Windows) + Avalonia (Cross-Platform)

---

## Wichtige Entscheidungen

### Theme-Format: JSON beibehalten
**Entscheidung:** JSON-Format beibehalten, Runtime-Konvertierung zu Avalonia Styles

**Begründung:**
- Bestehende Themes kompatibel
- Theme-Wechsel zur Laufzeit möglich
- Plattformunabhängig (WinForms + Avalonia)

### WebView: Avalonia.WebView nutzen
**Entscheidung:** `Avalonia.WebView` Package für Cross-Platform WebView

**Begründung:**
- Unterstützt WebView2 (Windows), WebKitGTK (Linux), WKWebView (macOS)
- Einheitliche API über `IWebViewAdapter`
- Community-Support

### Linux Distribution: Flatpak empfohlen
**Entscheidung:** Flatpak als primäre Linux Distribution

**Begründung:**
- Automatische Updates über Flathub
- Sandboxing + Portal API
- Keine Dependency-Probleme (WebKitGTK bundled)
- Eine Paket-Format für alle Distros

### DI Framework: Microsoft.Extensions.DependencyInjection
**Entscheidung:** Standard .NET DI Container nutzen

**Begründung:**
- Bereits in WinForms-Version genutzt
- Avalonia-kompatibel
- Keine zusätzlichen Dependencies

---

## Nächste Schritte

### Sofort starten (heute):
1. **Interfaces erstellen:**
   ```bash
   dotnet new classlib -n MarkdownViewer.Platform.Abstractions -f net8.0
   cd MarkdownViewer.Platform.Abstractions
   # Kopiere PLATFORM-ABSTRACTIONS-INTERFACES.cs Inhalt
   ```

2. **Windows-Implementierung beginnen:**
   ```bash
   dotnet new classlib -n MarkdownViewer.Platform.Windows -f net8.0-windows
   cd MarkdownViewer.Platform.Windows
   dotnet add reference ../MarkdownViewer.Platform.Abstractions
   ```

3. **Erste Migration:** `IFileAssociationService`
   - Migriere `Program.cs` Zeilen 199-347 zu `WindowsFileAssociationService`

### Diese Woche:
- [ ] Phase 1.1-1.3 abschließen (IFileAssociationService + IPlatformService + IWebViewAdapter)
- [ ] Erste Unit Tests schreiben
- [ ] MainForm.cs auf IPlatformService migrieren

### Nächste Woche:
- [ ] Phase 1 abschließen (IDialogService + DI Setup)
- [ ] Phase 2 starten (Shared Library extrahieren)

---

## Fragen & Antworten

### F: Kann ich WinForms und Avalonia parallel releasen?
**A:** Ja! Das ist explizit im Design berücksichtigt. `MarkdownViewer.Core` ist shared, beide UIs nutzen die gleichen Platform Abstractions. Du kannst beide parallel weiterentwickeln und releasen.

### F: Muss ich die gesamte Migration auf einmal machen?
**A:** Nein! Die Migration ist in 5 Phasen aufgeteilt. Nach Phase 1 hast du eine funktionierende WinForms App mit Platform Abstractions. Du kannst dort stoppen und später weitermachen.

### F: Was passiert mit bestehenden Nutzern?
**A:** Bestehende WinForms Version (v1.x) läuft weiter. Nach Migration:
- Windows-User können wählen: WinForms (v2.0) oder Avalonia (v2.1)
- Linux/macOS-User nutzen Avalonia (v2.1)
- Settings werden automatisch migriert

### F: Brauche ich einen Mac für macOS-Development?
**A:** Für vollständige macOS-Unterstützung (Code Signing, App Store) ja. Aber: Du kannst macOS Services in einem Cross-Compile Setup entwickeln und von der Community testen lassen.

### F: Wie lange dauert die komplette Migration?
**A:** Geschätzt 6-8 Wochen Vollzeit-Entwicklung. Mit 2-3 Stunden pro Tag: 4-6 Monate.

---

## Kontakt & Support

Bei Fragen zur Platform Abstractions-Dokumentation oder Migrations-Problemen:

1. **GitHub Issues:** Erstelle ein Issue mit Tag `platform-abstractions`
2. **Design Review:** Lass die Interfaces von anderen Developern reviewen
3. **Community:** Avalonia Discord/Forum für Avalonia-spezifische Fragen

---

## Changelog dieser Dokumentation

**v1.0 (2025-11-08):**
- Initiales Release
- Alle 5 Platform Abstractions definiert
- Migrations-Checkliste erstellt
- Vollständige C# Interfaces bereitgestellt

---

**Ende des Overview-Dokuments**
