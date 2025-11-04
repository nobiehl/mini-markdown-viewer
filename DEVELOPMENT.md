# MarkdownViewer - Developer Documentation

Technische Dokumentation für Entwickler, die am MarkdownViewer arbeiten oder ihn erweitern möchten.

## Codebase-Übersicht

### Dateistruktur

```
markdown-viewer/MarkdownViewer/
├── Program.cs              # Entry Point & CLI
├── MainForm.cs             # Main Window & Rendering
└── MarkdownViewer.csproj   # Projekt-Konfiguration
```

### Program.cs - Entry Point & CLI

**Verantwortlichkeiten:**
- Kommandozeilen-Argument-Parsing
- Windows Explorer Integration (Registry)
- Application Lifetime Management

**Wichtige Methoden:**

```csharp
static void Main(string[] args)
```
- Entry Point der Anwendung
- Parsed CLI-Argumente (`--install`, `--uninstall`, `--help`, `--version`, oder Dateipfad)
- Öffnet FileOpenDialog wenn keine Argumente

```csharp
void InstallFileAssociation()
```
- Erstellt Registry-Einträge in `HKEY_CURRENT_USER`
- Keine Admin-Rechte erforderlich
- Registriert 4 Integrationspunkte:
  1. Standard-Dateiassoziation (`.md` → MarkdownViewer)
  2. Kontextmenü ("Open with Markdown Viewer")
  3. "Open With"-Dialog-Eintrag
  4. "Send To"-Menü-Verknüpfung

```csharp
void UninstallFileAssociation()
```
- Entfernt alle Registry-Einträge
- Löscht "Send To"-Verknüpfung
- Safe to call (throwOnMissingSubKey: false)

```csharp
void CreateShortcut(string targetPath, string shortcutPath)
```
- Erstellt .lnk-Datei via WScript.Shell COM-Objekt
- Für "Send To"-Menü

### MainForm.cs - Main Window & Rendering

**Verantwortlichkeiten:**
- WebView2-Initialisierung und -Management
- Markdown-zu-HTML-Konvertierung
- Live-Reload via FileSystemWatcher
- HTML-Template mit CSS und JavaScript

**Wichtige Methoden:**

```csharp
MainForm(string filePath)
```
- Konstruktor: Initialisiert Form, lädt Datei, startet File Watcher

```csharp
void InitializeComponents()
```
- Erstellt WebView2-Control
- Konfiguriert Custom Cache-Ordner (`.cache` neben EXE)
- Setzt Navigation-Handler (externe Links → Browser)

```csharp
void LoadMarkdownFile(string filePath)
```
- Liest .md-Datei
- Konvertiert zu HTML
- Rendert in WebView2
- Behandelt Async WebView2-Initialisierung

```csharp
string ConvertMarkdownToHtml(string markdown)
```
- **Kern der Rendering-Engine**
- Nutzt Markdig mit `UseAdvancedExtensions()`
- Generiert vollständiges HTML-Dokument mit:
  - Embedded CSS (GitHub-Style)
  - Highlight.js (Syntax-Highlighting)
  - Mermaid.js (Diagramme)
  - PlantUML-Server-Integration
  - Copy-Buttons für Code-Blöcke

```csharp
void SetupFileWatcher(string filePath)
```
- FileSystemWatcher für Live-Reload
- Beobachtet `LastWrite` und `Size` Changes
- 100ms Delay gegen Multiple-Trigger

## Architektur-Entscheidungen

### Warum WebView2?
- **HTML/CSS/JS-Rendering** via Edge Chromium Engine
- Auf Windows 10/11 **vorinstalliert**
- Perfektes Rendering komplexer Markdown-Inhalte
- Ermöglicht JavaScript-basierte Features (Mermaid, Highlight.js)

### Warum Markdig?
- **Schnellster** C# Markdown-Parser
- CommonMark-konform
- Erweiterbar via Pipeline (Advanced Extensions)
- Unterstützt Tabellen, Task Lists, Auto-Links, etc.

### Warum Single-File Deployment?
- **Einfache Verteilung** (1 Datei)
- Portable (kann überall hin kopiert werden)
- Keine Installer-Komplexität
- Trade-off: .NET Runtime muss installiert sein

### Warum HKCU statt HKLM?
- **Keine Admin-Rechte** erforderlich
- Pro-User-Installation
- Einfacher zu deinstallieren
- Windows Best Practice für User-Tools

## Rendering Pipeline

```
┌─────────────────┐
│  .md File       │
└────────┬────────┘
         │
         ↓
┌─────────────────────────────────┐
│  File.ReadAllText()             │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  Markdig.ToHtml()               │
│  (mit Advanced Extensions)      │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  HTML String                    │
│  + Embedded CSS                 │
│  + Highlight.js CDN             │
│  + Mermaid.js CDN               │
│  + PlantUML Script              │
│  + Copy-Button Script           │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  WebView2.NavigateToString()    │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  Client-Side Processing:        │
│  • Highlight.js: Code-Blocks    │
│  • Mermaid: Diagramme           │
│  • PlantUML: Server-Fetch       │
│  • Copy-Buttons: Dynamisch      │
└─────────────────────────────────┘
```

## Diagramm-Integration

### Mermaid (Client-Side)
```javascript
import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
mermaid.initialize({ startOnLoad: true, theme: 'default' });
```

- **Rendert direkt im Browser**
- Code-Blöcke mit `language-mermaid` werden automatisch erkannt
- Keine Server-Kommunikation nötig

### PlantUML (Server-Side)
```javascript
document.querySelectorAll('code.language-plantuml').forEach((block) => {
    const encoded = encodePlantUML(block.textContent);
    const img = document.createElement('img');
    img.src = `https://www.plantuml.com/plantuml/svg/${encoded}`;
    pre.replaceWith(img);
});
```

- **Server-basiert** (plantuml.com)
- Code wird base64-encodiert
- Server generiert SVG
- Ersetzt Code-Block durch `<img>`

## Registry-Struktur

Nach `--install` werden folgende Registry-Keys erstellt:

### 1. Dateiassoziation (.md)
```
HKCU\Software\Classes\.md
  (Default) = "MarkdownViewer.Document"
```

### 2. Dokument-Klasse
```
HKCU\Software\Classes\MarkdownViewer.Document
  (Default) = "Markdown Document"

HKCU\Software\Classes\MarkdownViewer.Document\shell\open\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

### 3. Kontextmenü (funktioniert auch wenn anderes Programm Standard ist)
```
HKCU\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer
  (Default) = "Open with Markdown Viewer"
  Icon = "C:\path\to\MarkdownViewer.exe"

HKCU\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

### 4. "Open With"-Dialog
```
HKCU\Software\Classes\Applications\MarkdownViewer.exe
  FriendlyAppName = "Markdown Viewer"

HKCU\Software\Classes\Applications\MarkdownViewer.exe\SupportedTypes
  .md = ""
  .markdown = ""

HKCU\Software\Classes\Applications\MarkdownViewer.exe\shell\open\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

### 5. "Send To"-Menü
Verknüpfung in: `%APPDATA%\Microsoft\Windows\SendTo\Markdown Viewer.lnk`

## Build-Konfiguration

### MarkdownViewer.csproj

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>               <!-- Windows GUI App (keine Console) -->
  <TargetFramework>net8.0-windows</TargetFramework>
  <Nullable>enable</Nullable>                   <!-- Nullable Reference Types -->
  <UseWindowsForms>true</UseWindowsForms>       <!-- WinForms Support -->
  <ImplicitUsings>enable</ImplicitUsings>       <!-- Implizite Usings -->
  <PublishSingleFile>true</PublishSingleFile>   <!-- Single-File Deployment -->
  <SelfContained>false</SelfContained>          <!-- .NET Runtime erforderlich -->
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Web.WebView2" Version="1.0.2792.45" />
  <PackageReference Include="Markdig" Version="0.37.0" />
</ItemGroup>
```

### Build-Commands

```bash
# Debug Build
dotnet build -c Debug

# Release Build (normal)
dotnet build -c Release

# Single-File Publish (empfohlen)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single

# Development Run
dotnet run -- test-diagrams.md
```

## Erweiterungsmöglichkeiten

### 1. Neue Markdig-Extensions hinzufügen

In `MainForm.cs`:
```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseYourCustomExtension()  // <-- Hier
    .Build();
```

### 2. Custom CSS-Theme hinzufügen

In `MainForm.cs` → `ConvertMarkdownToHtml()` → `<style>` Block anpassen

### 3. Weitere Diagramm-Typen

In `ConvertMarkdownToHtml()` → `<script>` Block:
```javascript
document.querySelectorAll('code.language-yourdiagram').forEach((block) => {
    // Custom rendering logic
});
```

### 4. Weitere CLI-Parameter

In `Program.cs` → `Main()`:
```csharp
if (firstArg == "--your-param")
{
    YourCustomLogic();
    return;
}
```

### 5. Dark Mode

Option A: CSS Media Query (automatisch)
```css
@media (prefers-color-scheme: dark) {
    body { background: #1e1e1e; color: #d4d4d4; }
}
```

Option B: Toggle-Button mit JavaScript

## Testing

### Manuelle Tests
```bash
# Test File Open Dialog
.\bin-single\MarkdownViewer.exe

# Test Datei öffnen
.\bin-single\MarkdownViewer.exe test-diagrams.md

# Test Install/Uninstall
.\bin-single\MarkdownViewer.exe --install
.\bin-single\MarkdownViewer.exe --uninstall

# Test Version/Help
.\bin-single\MarkdownViewer.exe --version
.\bin-single\MarkdownViewer.exe --help
```

### Test-Cases
- [ ] Markdown mit allen Features (Tabellen, Listen, Code, etc.)
- [ ] Mermaid Flowchart, Sequence, Class Diagram
- [ ] PlantUML Class, Sequence Diagram
- [ ] Base64-embedded Images
- [ ] External Links (öffnen im Browser)
- [ ] Live-Reload (Datei ändern während offen)
- [ ] Registry Installation/Deinstallation
- [ ] Kontextmenü im Explorer
- [ ] "Open With" Dialog
- [ ] "Send To" Menü

## Bekannte Einschränkungen

1. **PlantUML benötigt Internet**: Server-basiert (plantuml.com)
   - Lösung: Lokalen PlantUML-Server einbinden (benötigt Java)

2. **Nullable Warnings**: Code nutzt nullable types, einige Warnings bleiben
   - Nicht kritisch, könnte aber bereinigt werden

3. **Keine Multi-Tab-Unterstützung**: Nur eine Datei pro Fenster
   - Feature-Request für zukünftige Version

4. **WebView2 Cache wächst**: `.cache` Ordner kann groß werden
   - Könnte automatisches Cleanup implementieren

5. **Live-Reload-Delay**: 100ms hardcoded
   - Könnte konfigurierbar gemacht werden

## Performance

### Startup-Zeit
- Cold Start: ~500-800ms (WebView2 Init)
- Warm Start: ~200-300ms

### Memory Usage
- Base: ~50-80 MB (WebView2)
- Pro Datei: +2-10 MB (abhängig von Bildern)

### File Size Limits
- Praktisches Limit: ~10 MB Markdown
- Größere Dateien: WebView2 kann langsam werden
- Empfehlung: Große Dateien splitten

## Debugging

### WebView2 DevTools aktivieren

In `MainForm.cs`:
```csharp
webView.CoreWebView2.Settings.AreDevToolsEnabled = true;  // Auf true setzen
```

Dann: `F12` im laufenden Viewer

### Logging hinzufügen

```csharp
Console.WriteLine("Debug Info");  // Erscheint in VS Output
Debug.WriteLine("Debug Info");     // Erscheint in Debug Output
```

### Registry-Einträge prüfen

```powershell
# Registry durchsuchen
reg query HKCU\Software\Classes\.md
reg query HKCU\Software\Classes\MarkdownViewer.Document
```

## Contributing

Wenn du beitragen möchtest:

1. Code-Style: Standard C# Conventions
2. Kommentare: XML-Docs für öffentliche Methoden
3. Testing: Manuell testen vor Commit
4. Dokumentation: README.md und DEVELOPMENT.md aktualisieren

## Lizenz

Open Source - nutze es wie du willst!

---

**Weitere Fragen?** Schau in den Code - er ist gut dokumentiert! 📖
