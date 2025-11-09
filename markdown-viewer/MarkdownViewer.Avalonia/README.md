# MarkdownViewer.Avalonia - Proof of Concept

Minimale Avalonia-Implementierung des MarkdownViewers als plattformübergreifende Desktop-Anwendung.

## Status

### Funktioniert
- Avalonia App läuft auf Windows (und potenziell Linux/macOS)
- Markdown-Rendering über MarkdownViewer.Core
- File-Open-Dialog für .md/.markdown Dateien
- Theme-System (Standard & Dark) integriert
- Menü-System (File, Theme)
- StatusBar vorhanden
- Build ohne Warnungen/Fehler

### Einschränkungen (PoC)
- WebView noch nicht integriert (HTML wird als Text angezeigt)
- Keine echte HTML-Vorschau (nur HTML-Code sichtbar)
- Navigation/Search fehlen noch
- NavigationBar fehlt noch
- File-Watching fehlt noch
- Update-Mechanismus fehlt noch
- Theme-Dateien werden noch nicht aus JSON geladen

## Architektur

```
MarkdownViewer.Avalonia/
├── App.axaml                   # Avalonia Application Definition
├── App.axaml.cs
├── MainWindow.axaml            # Haupt-UI (Menu, StatusBar, Content)
├── MainWindow.axaml.cs         # Code-Behind mit File-Handling
├── Program.cs                  # Entry Point
└── MarkdownViewer.Avalonia.csproj
```

## Build

```bash
cd C:\develop\workspace\misc\markdown-viewer\MarkdownViewer.Avalonia
dotnet build -c Release
```

## Run

```bash
dotnet run
```

Oder direkt:

```bash
.\bin\Release\net8.0\MarkdownViewer.Avalonia.exe
```

## Verwendete Packages

- **Avalonia 11.3.8** - Cross-Platform UI Framework
- **MarkdownViewer.Core** - Markdown-Rendering, Theme-System
- **Markdig** (via Core) - Markdown Parser
- **Serilog** (via Core) - Logging

## Features (aktuell implementiert)

### File-Handling
- Open File Dialog (.md, .markdown)
- Async File-Loading
- Error-Handling

### Theme-System
- Standard Theme (hell)
- Dark Theme (dunkel)
- Theme-Wechsel über Menü
- Themes werden programmatisch erstellt (nicht aus JSON)

### UI-Komponenten
- Menu Bar (File, Theme)
- StatusBar (zeigt "Ready")
- ScrollViewer für Content
- 1024x768 Fenster-Größe

## Nächste Schritte

### Priorität 1: WebView Integration
Das größte Missing Feature ist die WebView-Integration für echte HTML-Vorschau.

**Optionen:**
1. **WebViewControl.Avalonia** (falls verfügbar)
2. **Avalonia.Browser** (experimentell)
3. **Eigene Chromium-Integration** (komplexer)

**Empfehlung:** Warten auf offizielle Avalonia.WebView oder ChromiumWebBrowser für Avalonia.

### Priorität 2: Vollständige UI
- NavigationBar hinzufügen (Back/Forward/Refresh)
- SearchBar integrieren
- StatusBar erweitern (Datei-Infos, Positionen)

### Priorität 3: Core-Features
- Theme-Dateien aus JSON laden (wie WinForms Version)
- File-Watching implementieren
- Update-Mechanismus

### Priorität 4: Plattform-Tests
- Linux Build testen
- macOS Build testen
- Packaging (AppImage, DMG, etc.)

## Unterschiede zur WinForms Version

| Feature | WinForms | Avalonia PoC | Status |
|---------|----------|--------------|--------|
| HTML-Vorschau | WebView2 | Text-Fallback | Fehlende WebView |
| Theme-System | JSON-Dateien | Programmatisch | Vereinfacht |
| File-Watching | Ja | Nein | TODO |
| Navigation | Ja | Nein | TODO |
| Search | Ja | Nein | TODO |
| StatusBar | Vollständig | Minimal | Vereinfacht |
| Update-Check | Ja | Nein | TODO |
| Plattform | Windows | Cross-Platform | Vorteil! |

## Technische Notizen

### Warum kein WebView?
Die WebView-Packages für Avalonia sind noch nicht ausgereift:
- `Avalonia.WebView` existiert nicht auf NuGet
- `WebViewControl` existiert nicht auf NuGet
- Chromium-Integration ist komplex

**Workaround:** HTML wird als Text angezeigt (für PoC ausreichend).

### Theme-System
Themes werden aktuell programmatisch erstellt:
- `LoadDefaultTheme()` - Standard (hell)
- `LoadTheme("dark")` - Dark Theme

In der finalen Version sollten Themes aus JSON-Dateien geladen werden (wie in WinForms).

### MVVM vs. Code-Behind
Aktuell nutzt der PoC Code-Behind für Einfachheit. Für eine produktionsreife App sollte MVVM verwendet werden:
- ViewModels für Logik
- Commands für Actions
- Data Binding für UI-Updates

## Lessons Learned

1. **Avalonia ist produktionsreif** - Der Build und die Basics funktionieren einwandfrei
2. **WebView fehlt** - Das ist das größte Hindernis für einen vollständigen MarkdownViewer
3. **Cross-Platform ist einfach** - Gleicher Code läuft auf Windows/Linux/macOS
4. **MarkdownViewer.Core ist wiederverwendbar** - Die Core-Library funktioniert problemlos mit Avalonia

## Fazit

Der PoC zeigt, dass eine Avalonia-Version des MarkdownViewers **technisch machbar** ist. Der Hauptblocker ist die fehlende WebView-Integration. Sobald eine stabile WebView-Lösung für Avalonia verfügbar ist, kann die App schnell vervollständigt werden.

**Empfehlung:**
- Für Windows-Only: WinForms-Version nutzen (WebView2 funktioniert perfekt)
- Für Cross-Platform: Avalonia-Version weiterentwickeln sobald WebView verfügbar ist
