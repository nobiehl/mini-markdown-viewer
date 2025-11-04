# MarkdownViewer - Developer Documentation

Technical documentation for developers working on or extending MarkdownViewer.

## Codebase Overview

### File Structure

```
markdown-viewer/MarkdownViewer/
├── Program.cs              # Entry point & CLI
├── MainForm.cs             # Main window & rendering
└── MarkdownViewer.csproj   # Project configuration
```

### Program.cs - Entry Point & CLI

**Responsibilities:**
- Command-line argument parsing
- Windows Explorer integration (registry)
- Application lifetime management

**Key Methods:**

```csharp
static void Main(string[] args)
```
- Application entry point
- Parses CLI arguments (`--install`, `--uninstall`, `--help`, `--version`, or file path)
- Opens FileOpenDialog when no arguments provided

```csharp
void InstallFileAssociation()
```
- Creates registry entries in `HKEY_CURRENT_USER`
- No admin rights required
- Registers 4 integration points:
  1. Default file association (`.md` → MarkdownViewer)
  2. Context menu ("Open with Markdown Viewer")
  3. "Open With" dialog entry
  4. "Send To" menu shortcut

```csharp
void UninstallFileAssociation()
```
- Removes all registry entries
- Deletes "Send To" shortcut
- Safe to call (throwOnMissingSubKey: false)

```csharp
void CreateShortcut(string targetPath, string shortcutPath)
```
- Creates .lnk file via WScript.Shell COM object
- Used for "Send To" menu

### MainForm.cs - Main Window & Rendering

**Responsibilities:**
- WebView2 initialization and management
- Markdown-to-HTML conversion
- Live reload via FileSystemWatcher
- HTML template with CSS and JavaScript

**Key Methods:**

```csharp
MainForm(string filePath)
```
- Constructor: initializes form, loads file, starts file watcher

```csharp
void InitializeComponents()
```
- Creates WebView2 control
- Configures custom cache folder (`.cache` next to EXE)
- Sets navigation handler (external links → browser)

```csharp
void LoadMarkdownFile(string filePath)
```
- Reads .md file
- Converts to HTML
- Renders in WebView2
- Handles async WebView2 initialization

```csharp
string ConvertMarkdownToHtml(string markdown)
```
- Core rendering engine
- Uses Markdig with `UseAdvancedExtensions()`
- Generates complete HTML document with:
  - Embedded CSS (GitHub style)
  - Highlight.js (syntax highlighting)
  - Mermaid.js (diagrams)
  - PlantUML server integration
  - Copy buttons for code blocks

```csharp
void SetupFileWatcher(string filePath)
```
- FileSystemWatcher for live reload
- Monitors `LastWrite` and `Size` changes
- 100ms delay to prevent multiple triggers

## Architecture Decisions

### Why WebView2?
- HTML/CSS/JS rendering via Edge Chromium engine
- Preinstalled on Windows 10/11
- Perfect rendering of complex Markdown content
- Enables JavaScript-based features (Mermaid, Highlight.js)

### Why Markdig?
- Fastest C# Markdown parser
- CommonMark compliant
- Extensible via pipeline (advanced extensions)
- Supports tables, task lists, auto-links, etc.

### Why Single-File Deployment?
- Simple distribution (one file)
- Portable (can be copied anywhere)
- No installer complexity
- Trade-off: .NET Runtime must be installed

### Why HKCU instead of HKLM?
- No admin rights required
- Per-user installation
- Easier to uninstall
- Windows best practice for user tools

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
│  (with advanced extensions)     │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  HTML String                    │
│  + Embedded CSS                 │
│  + Highlight.js CDN             │
│  + Mermaid.js CDN               │
│  + PlantUML script              │
│  + Copy button script           │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  WebView2.NavigateToString()    │
└────────┬────────────────────────┘
         │
         ↓
┌─────────────────────────────────┐
│  Client-side processing:        │
│  • Highlight.js: Code blocks    │
│  • Mermaid: Diagrams            │
│  • PlantUML: Server fetch       │
│  • Copy buttons: Dynamic        │
└─────────────────────────────────┘
```

## Diagram Integration

### Mermaid (Client-Side)
```javascript
import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
mermaid.initialize({ startOnLoad: true, theme: 'default' });
```

- Renders directly in browser
- Code blocks with `language-mermaid` are automatically detected
- No server communication needed

### PlantUML (Server-Side)
```javascript
document.querySelectorAll('code.language-plantuml').forEach((block) => {
    const encoded = encodePlantUML(block.textContent);
    const img = document.createElement('img');
    img.src = `https://www.plantuml.com/plantuml/svg/${encoded}`;
    pre.replaceWith(img);
});
```

- Server-based (plantuml.com)
- Code is base64-encoded
- Server generates SVG
- Replaces code block with `<img>`

## Registry Structure

After `--install`, the following registry keys are created:

### 1. File Association (.md)
```
HKCU\Software\Classes\.md
  (Default) = "MarkdownViewer.Document"
```

### 2. Document Class
```
HKCU\Software\Classes\MarkdownViewer.Document
  (Default) = "Markdown Document"

HKCU\Software\Classes\MarkdownViewer.Document\shell\open\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

### 3. Context Menu (works even if another program is default)
```
HKCU\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer
  (Default) = "Open with Markdown Viewer"
  Icon = "C:\path\to\MarkdownViewer.exe"

HKCU\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

### 4. "Open With" Dialog
```
HKCU\Software\Classes\Applications\MarkdownViewer.exe
  FriendlyAppName = "Markdown Viewer"

HKCU\Software\Classes\Applications\MarkdownViewer.exe\SupportedTypes
  .md = ""
  .markdown = ""

HKCU\Software\Classes\Applications\MarkdownViewer.exe\shell\open\command
  (Default) = "C:\path\to\MarkdownViewer.exe" "%1"
```

### 5. "Send To" Menu
Shortcut in: `%APPDATA%\Microsoft\Windows\SendTo\Markdown Viewer.lnk`

## Build Configuration

### MarkdownViewer.csproj

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>               <!-- Windows GUI app (no console) -->
  <TargetFramework>net8.0-windows</TargetFramework>
  <Nullable>enable</Nullable>                   <!-- Nullable reference types -->
  <UseWindowsForms>true</UseWindowsForms>       <!-- WinForms support -->
  <ImplicitUsings>enable</ImplicitUsings>       <!-- Implicit usings -->
  <PublishSingleFile>true</PublishSingleFile>   <!-- Single-file deployment -->
  <SelfContained>false</SelfContained>          <!-- .NET Runtime required -->
  <IncludeNativeLibrariesForSelfExtract>true</IncludeNativeLibrariesForSelfExtract>
</PropertyGroup>

<ItemGroup>
  <PackageReference Include="Microsoft.Web.WebView2" Version="1.0.2792.45" />
  <PackageReference Include="Markdig" Version="0.37.0" />
</ItemGroup>
```

### Build Commands

```bash
# Debug build
dotnet build -c Debug

# Release build (standard)
dotnet build -c Release

# Single-file publish (recommended)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single

# Development run
dotnet run -- test-diagrams.md
```

## Extension Possibilities

### 1. Add New Markdig Extensions

In `MainForm.cs`:
```csharp
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseYourCustomExtension()  // <-- Here
    .Build();
```

### 2. Add Custom CSS Theme

In `MainForm.cs` → `ConvertMarkdownToHtml()` → modify `<style>` block

### 3. Add More Diagram Types

In `ConvertMarkdownToHtml()` → `<script>` block:
```javascript
document.querySelectorAll('code.language-yourdiagram').forEach((block) => {
    // Custom rendering logic
});
```

### 4. Add More CLI Parameters

In `Program.cs` → `Main()`:
```csharp
if (firstArg == "--your-param")
{
    YourCustomLogic();
    return;
}
```

### 5. Dark Mode

Option A: CSS media query (automatic)
```css
@media (prefers-color-scheme: dark) {
    body { background: #1e1e1e; color: #d4d4d4; }
}
```

Option B: Toggle button with JavaScript

## Testing

### Manual Tests
```bash
# Test file open dialog
.\bin-single\MarkdownViewer.exe

# Test file opening
.\bin-single\MarkdownViewer.exe test-diagrams.md

# Test install/uninstall
.\bin-single\MarkdownViewer.exe --install
.\bin-single\MarkdownViewer.exe --uninstall

# Test version/help
.\bin-single\MarkdownViewer.exe --version
.\bin-single\MarkdownViewer.exe --help
```

### Test Cases
- [ ] Markdown with all features (tables, lists, code, etc.)
- [ ] Mermaid flowchart, sequence, class diagram
- [ ] PlantUML class, sequence diagram
- [ ] Base64-embedded images
- [ ] External links (open in browser)
- [ ] Live reload (change file while open)
- [ ] Registry installation/uninstallation
- [ ] Context menu in Explorer
- [ ] "Open With" dialog
- [ ] "Send To" menu

## Known Limitations

1. **PlantUML requires internet**: Server-based (plantuml.com)
   - Solution: Integrate local PlantUML server (requires Java)

2. **Nullable warnings**: Code uses nullable types, some warnings remain
   - Not critical, but could be cleaned up

3. **No multi-tab support**: Only one file per window
   - Feature request for future version

4. **WebView2 cache grows**: `.cache` folder can become large
   - Could implement automatic cleanup

5. **Live reload delay**: 100ms hardcoded
   - Could be made configurable

## Performance

### Startup Time
- Cold start: ~500-800ms (WebView2 init)
- Warm start: ~200-300ms

### Memory Usage
- Base: ~50-80 MB (WebView2)
- Per file: +2-10 MB (depending on images)

### File Size Limits
- Practical limit: ~10 MB Markdown
- Larger files: WebView2 can become slow
- Recommendation: Split large files

## Debugging

### Enable WebView2 DevTools

In `MainForm.cs`:
```csharp
webView.CoreWebView2.Settings.AreDevToolsEnabled = true;  // Set to true
```

Then: Press `F12` in running viewer

### Add Logging

```csharp
Console.WriteLine("Debug info");  // Appears in VS Output
Debug.WriteLine("Debug info");     // Appears in Debug Output
```

### Check Registry Entries

```powershell
# Search registry
reg query HKCU\Software\Classes\.md
reg query HKCU\Software\Classes\MarkdownViewer.Document
```

## Contributing

When contributing:

1. Code style: Standard C# conventions
2. Comments: XML docs for public methods
3. Testing: Manual testing before commit
4. Documentation: Update README.md and DEVELOPMENT.md

## License

MIT License - see LICENSE file for details.

---

**Questions?** Check the code - it's well documented! 📖
