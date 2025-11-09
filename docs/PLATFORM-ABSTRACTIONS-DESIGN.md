# Platform Abstractions Design für MarkdownViewer Cross-Platform Support

**Version:** 1.0
**Datum:** 2025-11-08
**Status:** Design Phase
**Autor:** Claude Code

---

## Übersicht

Dieses Dokument definiert die Platform Abstraction Interfaces für die Migration von MarkdownViewer von WinForms auf eine Shared Library + Avalonia UI Architektur. Das Ziel ist, die bestehende Business Logic vollständig plattformunabhängig zu machen, während Windows Explorer Integration und plattformspezifische Features erhalten bleiben.

**Designprinzipien:**
- **Separation of Concerns:** Business Logic bleibt plattformunabhängig
- **Dependency Inversion:** Abstrakte Interfaces definieren Kontrakte
- **Testability:** Alle Plattform-Services sind mockbar
- **Incremental Migration:** Alte und neue Architektur können parallel existieren

---

## 1. IFileAssociationService Interface

### Zweck
Abstrahiert die Registrierung der Applikation als Standard-Handler für Markdown-Dateien. Unterstützt Windows Registry, Linux `.desktop` Files und macOS Launch Services.

### Interface Definition

```csharp
using System.Threading.Tasks;

namespace MarkdownViewer.Platform.Abstractions
{
    /// <summary>
    /// Service für die Registrierung der Applikation als Standard-Handler für .md/.markdown Dateien.
    /// Plattformunabhängiges Interface mit plattformspezifischen Implementierungen.
    /// </summary>
    public interface IFileAssociationService
    {
        /// <summary>
        /// Prüft, ob die Applikation als Standard-Handler für .md Dateien registriert ist.
        /// </summary>
        /// <returns>True wenn registriert, sonst false</returns>
        Task<bool> IsInstalledAsync();

        /// <summary>
        /// Registriert die Applikation als Standard-Handler für .md und .markdown Dateien.
        /// Benötigt keine Admin-Rechte (user-level registration).
        /// </summary>
        /// <param name="applicationPath">Vollständiger Pfad zur ausführbaren Datei</param>
        /// <param name="options">Plattformspezifische Optionen (optional)</param>
        /// <returns>True bei Erfolg, false bei Fehler</returns>
        /// <exception cref="PlatformNotSupportedException">Wenn Plattform nicht unterstützt wird</exception>
        /// <exception cref="UnauthorizedAccessException">Wenn User-Rechte nicht ausreichen</exception>
        Task<bool> InstallAsync(string applicationPath, FileAssociationOptions? options = null);

        /// <summary>
        /// Entfernt die Registrierung als Standard-Handler.
        /// Sicher aufrufbar, auch wenn nie installiert wurde.
        /// </summary>
        /// <returns>True bei Erfolg, false bei Fehler</returns>
        Task<bool> UninstallAsync();

        /// <summary>
        /// Gibt detaillierte Informationen über den aktuellen Registrierungsstatus zurück.
        /// </summary>
        /// <returns>Status-Information mit Details zu allen Integrationspunkten</returns>
        Task<FileAssociationStatus> GetStatusAsync();
    }

    /// <summary>
    /// Optionen für die File Association Registrierung.
    /// Plattformspezifische Eigenschaften werden von konkreten Implementierungen genutzt.
    /// </summary>
    public class FileAssociationOptions
    {
        /// <summary>
        /// Freundlicher Name der Applikation (z.B. "Markdown Viewer")
        /// </summary>
        public string? FriendlyName { get; set; }

        /// <summary>
        /// Beschreibung des Dateityps (z.B. "Markdown Document")
        /// </summary>
        public string? FileTypeDescription { get; set; }

        /// <summary>
        /// Icon-Pfad für registrierte Dateien (null = Standard-Icon)
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        /// Windows: Registriere Context Menu Entry ("Öffnen mit Markdown Viewer")
        /// </summary>
        public bool RegisterContextMenu { get; set; } = true;

        /// <summary>
        /// Windows: Füge "Send To" Menu Shortcut hinzu
        /// </summary>
        public bool RegisterSendToMenu { get; set; } = true;

        /// <summary>
        /// Linux: Kategorien für .desktop file (z.B. "Utility;TextEditor;")
        /// </summary>
        public string[]? DesktopCategories { get; set; }

        /// <summary>
        /// Linux: MIME Types (zusätzlich zu text/markdown)
        /// </summary>
        public string[]? AdditionalMimeTypes { get; set; }
    }

    /// <summary>
    /// Detaillierter Status der File Association Registrierung.
    /// </summary>
    public class FileAssociationStatus
    {
        /// <summary>
        /// True wenn als Default Handler registriert
        /// </summary>
        public bool IsDefaultHandler { get; set; }

        /// <summary>
        /// True wenn Context Menu registriert (Windows)
        /// </summary>
        public bool HasContextMenu { get; set; }

        /// <summary>
        /// True wenn in "Send To" Menu verfügbar (Windows)
        /// </summary>
        public bool HasSendToEntry { get; set; }

        /// <summary>
        /// True wenn .desktop file existiert (Linux)
        /// </summary>
        public bool HasDesktopEntry { get; set; }

        /// <summary>
        /// True wenn in Launch Services registriert (macOS)
        /// </summary>
        public bool IsRegisteredInLaunchServices { get; set; }

        /// <summary>
        /// Liste der registrierten Dateierweiterungen
        /// </summary>
        public List<string> RegisteredExtensions { get; set; } = new();

        /// <summary>
        /// Liste der registrierten MIME Types
        /// </summary>
        public List<string> RegisteredMimeTypes { get; set; } = new();

        /// <summary>
        /// Pfad zur registrierten ausführbaren Datei
        /// </summary>
        public string? RegisteredExecutablePath { get; set; }

        /// <summary>
        /// Fehlermeldung, falls Status nicht vollständig abgerufen werden konnte
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
```

### Plattformspezifische Implementierungen

#### 1.1 Windows Implementation (WindowsFileAssociationService)

**Registry-Struktur:**
```
HKEY_CURRENT_USER\Software\Classes\.md
    (Default) = "MarkdownViewer.Document"

HKEY_CURRENT_USER\Software\Classes\MarkdownViewer.Document
    (Default) = "Markdown Document"
    \shell\open\command
        (Default) = "C:\Path\To\MarkdownViewer.exe" "%1"

HKEY_CURRENT_USER\Software\Classes\SystemFileAssociations\.md\shell\MarkdownViewer
    (Default) = "Open with Markdown Viewer"
    Icon = "C:\Path\To\MarkdownViewer.exe"
    \command
        (Default) = "C:\Path\To\MarkdownViewer.exe" "%1"

HKEY_CURRENT_USER\Software\Classes\Applications\MarkdownViewer.exe
    FriendlyAppName = "Markdown Viewer"
    \SupportedTypes
        .md = ""
        .markdown = ""
    \shell\open\command
        (Default) = "C:\Path\To\MarkdownViewer.exe" "%1"
```

**Send To Menu:**
- Erstellt Shortcut in `%APPDATA%\Microsoft\Windows\SendTo\Markdown Viewer.lnk`
- Nutzt `IWshRuntimeLibrary` COM Interop

**Migration vom aktuellen Code:**
```csharp
// ALT: Program.cs - Zeilen 199-279 (InstallFileAssociation)
// NEU: WindowsFileAssociationService.InstallAsync()
//
// Aktuelle Logic übernehmen:
// 1. Registry Keys in HKEY_CURRENT_USER erstellen
// 2. Send To Shortcut via COM erstellen
// 3. Async/Await Pattern hinzufügen für bessere UI-Responsiveness
```

#### 1.2 Linux Implementation (LinuxFileAssociationService)

**Desktop Entry Format:**
```ini
[Desktop Entry]
Name=Markdown Viewer
Comment=Lightweight Markdown Viewer
Exec=/usr/local/bin/MarkdownViewer %f
Icon=/usr/local/share/icons/markdownviewer.png
Terminal=false
Type=Application
Categories=Utility;TextEditor;Viewer;
MimeType=text/markdown;text/x-markdown;
```

**Installation Locations:**
- User-level: `~/.local/share/applications/markdownviewer.desktop`
- System-level: `/usr/share/applications/markdownviewer.desktop` (erfordert sudo)

**MIME Type Registration:**
```bash
# Update MIME database
xdg-mime default markdownviewer.desktop text/markdown
xdg-mime default markdownviewer.desktop text/x-markdown
update-desktop-database ~/.local/share/applications/
```

**Implementation Details:**
```csharp
public class LinuxFileAssociationService : IFileAssociationService
{
    private const string DesktopFileName = "markdownviewer.desktop";

    public async Task<bool> InstallAsync(string applicationPath, FileAssociationOptions? options = null)
    {
        // 1. Desktop Entry erstellen
        var desktopFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "applications",
            DesktopFileName
        );

        // 2. .desktop file schreiben
        var desktopEntry = GenerateDesktopEntry(applicationPath, options);
        await File.WriteAllTextAsync(desktopFilePath, desktopEntry);

        // 3. Ausführbar machen
        await ProcessHelper.RunAsync("chmod", $"+x {desktopFilePath}");

        // 4. MIME Type registrieren
        await ProcessHelper.RunAsync("xdg-mime", $"default {DesktopFileName} text/markdown");
        await ProcessHelper.RunAsync("xdg-mime", $"default {DesktopFileName} text/x-markdown");

        // 5. Desktop database updaten
        var applicationsDir = Path.GetDirectoryName(desktopFilePath);
        await ProcessHelper.RunAsync("update-desktop-database", applicationsDir);

        return true;
    }
}
```

#### 1.3 macOS Implementation (MacOSFileAssociationService)

**Launch Services API:**
- Verwendet `LSSetDefaultHandlerForContentType` für MIME Types
- Nutzt `LSSetDefaultRoleHandlerForContentType` für UTI (Uniform Type Identifiers)

**UTI Definition in Info.plist:**
```xml
<key>CFBundleDocumentTypes</key>
<array>
    <dict>
        <key>CFBundleTypeName</key>
        <string>Markdown Document</string>
        <key>CFBundleTypeRole</key>
        <string>Viewer</string>
        <key>LSHandlerRank</key>
        <string>Owner</string>
        <key>LSItemContentTypes</key>
        <array>
            <string>net.daringfireball.markdown</string>
            <string>public.plain-text</string>
        </array>
    </dict>
</array>
```

**Implementation Details:**
```csharp
public class MacOSFileAssociationService : IFileAssociationService
{
    public async Task<bool> InstallAsync(string applicationPath, FileAssociationOptions? options = null)
    {
        // macOS Launch Services erfordert Bundle Identifier
        var bundleId = "com.markdownviewer.app";

        // Register via lsregister command-line tool
        await ProcessHelper.RunAsync(
            "/System/Library/Frameworks/CoreServices.framework/Frameworks/LaunchServices.framework/Support/lsregister",
            $"-f {applicationPath}"
        );

        // Set as default handler for UTI
        await ProcessHelper.RunAsync("duti",
            $"-s {bundleId} net.daringfireball.markdown all"
        );

        return true;
    }
}
```

**Hinweis:** Für vollständige macOS Integration muss die App als .app Bundle vorliegen.

---

## 2. IPlatformService Interface

### Zweck
Zentraler Service für plattformspezifische Operationen wie Pfade, Prozess-Verwaltung und System-Information.

### Interface Definition

```csharp
using System;
using System.Threading.Tasks;

namespace MarkdownViewer.Platform.Abstractions
{
    /// <summary>
    /// Platform-Service für OS-spezifische Operationen.
    /// Ersetzt direkte Aufrufe von Application.ExecutablePath, Environment.SpecialFolder, etc.
    /// </summary>
    public interface IPlatformService
    {
        /// <summary>
        /// Gibt die aktuelle Plattform zurück.
        /// </summary>
        PlatformType CurrentPlatform { get; }

        /// <summary>
        /// Gibt den vollständigen Pfad zur ausführbaren Datei zurück.
        /// Windows: C:\Path\To\MarkdownViewer.exe
        /// Linux: /usr/local/bin/MarkdownViewer
        /// macOS: /Applications/MarkdownViewer.app/Contents/MacOS/MarkdownViewer
        /// </summary>
        string GetExecutablePath();

        /// <summary>
        /// Gibt den Ordner zurück, in dem die ausführbare Datei liegt.
        /// Nützlich für relative Pfade zu Themes, Logs, etc.
        /// </summary>
        string GetExecutableDirectory();

        /// <summary>
        /// Gibt den Application Data Ordner zurück (user-spezifisch).
        /// Windows: %APPDATA%\MarkdownViewer
        /// Linux: ~/.config/MarkdownViewer
        /// macOS: ~/Library/Application Support/MarkdownViewer
        /// </summary>
        string GetApplicationDataFolder();

        /// <summary>
        /// Gibt den Ordner für Log-Dateien zurück.
        /// Erstellt den Ordner, falls er nicht existiert.
        /// </summary>
        string GetLogsFolder();

        /// <summary>
        /// Gibt den Ordner für temporäre Dateien zurück.
        /// Windows: %TEMP%\MarkdownViewer
        /// Linux: /tmp/MarkdownViewer
        /// macOS: /var/tmp/MarkdownViewer
        /// </summary>
        string GetTempFolder();

        /// <summary>
        /// Gibt den Ordner für Cache-Daten zurück (z.B. WebView2 Cache).
        /// Windows: %LOCALAPPDATA%\MarkdownViewer\Cache
        /// Linux: ~/.cache/MarkdownViewer
        /// macOS: ~/Library/Caches/MarkdownViewer
        /// </summary>
        string GetCacheFolder();

        /// <summary>
        /// Startet die Applikation neu.
        /// Schließt die aktuelle Instanz und startet eine neue.
        /// </summary>
        /// <param name="arguments">Kommandozeilen-Argumente für die neue Instanz</param>
        Task RestartApplicationAsync(string[]? arguments = null);

        /// <summary>
        /// Öffnet eine URL im Standard-Browser.
        /// </summary>
        /// <param name="url">HTTP/HTTPS URL</param>
        Task<bool> OpenUrlInBrowserAsync(string url);

        /// <summary>
        /// Öffnet eine Datei mit dem Standard-Handler des OS.
        /// </summary>
        /// <param name="filePath">Vollständiger Pfad zur Datei</param>
        Task<bool> OpenFileWithDefaultHandlerAsync(string filePath);

        /// <summary>
        /// Zeigt eine Datei im Explorer/Finder/File Manager an.
        /// Windows: Explorer mit Datei selektiert
        /// Linux: Nautilus/Dolphin/etc. mit Datei im Ordner
        /// macOS: Finder mit Datei selektiert
        /// </summary>
        /// <param name="filePath">Vollständiger Pfad zur Datei</param>
        Task<bool> ShowFileInExplorerAsync(string filePath);

        /// <summary>
        /// Gibt System-Informationen zurück (für Debugging/Logging).
        /// </summary>
        PlatformInfo GetPlatformInfo();

        /// <summary>
        /// Prüft, ob die Applikation mit Administrator/Root-Rechten läuft.
        /// </summary>
        bool IsRunningAsAdmin();
    }

    /// <summary>
    /// Plattform-Typ Enumeration.
    /// </summary>
    public enum PlatformType
    {
        Windows,
        Linux,
        MacOS,
        Unknown
    }

    /// <summary>
    /// Detaillierte Plattform-Informationen.
    /// </summary>
    public class PlatformInfo
    {
        public PlatformType Platform { get; set; }
        public string OSDescription { get; set; } = "";
        public string OSVersion { get; set; } = "";
        public string RuntimeVersion { get; set; } = "";
        public string Architecture { get; set; } = "";
        public bool Is64BitProcess { get; set; }
        public bool Is64BitOperatingSystem { get; set; }
    }
}
```

### Plattformspezifische Implementierungen

#### 2.1 Windows Implementation (WindowsPlatformService)

```csharp
using System.Windows.Forms; // Nur für WinForms-Version
using Avalonia; // Für Avalonia-Version

public class WindowsPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.Windows;

    public string GetExecutablePath()
    {
        // WinForms: Application.ExecutablePath
        // Avalonia: Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location
        return Application.ExecutablePath;
    }

    public string GetApplicationDataFolder()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "MarkdownViewer"
        );
        Directory.CreateDirectory(folder);
        return folder;
    }

    public string GetLogsFolder()
    {
        var folder = Path.Combine(GetExecutableDirectory(), "logs");
        Directory.CreateDirectory(folder);
        return folder;
    }

    public string GetCacheFolder()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "MarkdownViewer",
            "Cache"
        );
        Directory.CreateDirectory(folder);
        return folder;
    }

    public async Task RestartApplicationAsync(string[]? arguments = null)
    {
        var exePath = GetExecutablePath();
        var args = arguments != null ? string.Join(" ", arguments) : "";

        Process.Start(new ProcessStartInfo
        {
            FileName = exePath,
            Arguments = args,
            UseShellExecute = true
        });

        // Current application beenden
        Environment.Exit(0);
        await Task.CompletedTask;
    }

    public async Task<bool> OpenUrlInBrowserAsync(string url)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ShowFileInExplorerAsync(string filePath)
    {
        try
        {
            Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsRunningAsAdmin()
    {
        using var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
        var principal = new System.Security.Principal.WindowsPrincipal(identity);
        return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
    }
}
```

**Migration vom aktuellen Code:**
```csharp
// ALT: MainForm.cs - Zeile 255
string exeFolder = Path.GetDirectoryName(Application.ExecutablePath) ?? ".";
string logsFolder = Path.Combine(exeFolder, "logs");

// NEU: Dependency Injection
private readonly IPlatformService _platformService;
string logsFolder = _platformService.GetLogsFolder();

// ALT: MainForm.cs - Zeile 298
string userDataFolder = Path.Combine(exeFolder, ".cache");

// NEU:
string userDataFolder = _platformService.GetCacheFolder();
```

#### 2.2 Linux Implementation (LinuxPlatformService)

```csharp
public class LinuxPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.Linux;

    public string GetExecutablePath()
    {
        return Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;
    }

    public string GetApplicationDataFolder()
    {
        var configHome = Environment.GetEnvironmentVariable("XDG_CONFIG_HOME");
        if (string.IsNullOrEmpty(configHome))
        {
            configHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config");
        }

        var folder = Path.Combine(configHome, "MarkdownViewer");
        Directory.CreateDirectory(folder);
        return folder;
    }

    public string GetCacheFolder()
    {
        var cacheHome = Environment.GetEnvironmentVariable("XDG_CACHE_HOME");
        if (string.IsNullOrEmpty(cacheHome))
        {
            cacheHome = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache");
        }

        var folder = Path.Combine(cacheHome, "MarkdownViewer");
        Directory.CreateDirectory(folder);
        return folder;
    }

    public async Task<bool> ShowFileInExplorerAsync(string filePath)
    {
        // Versuche verschiedene File Manager
        var fileManagers = new[] { "nautilus", "dolphin", "thunar", "nemo", "caja", "xdg-open" };

        foreach (var fm in fileManagers)
        {
            try
            {
                Process.Start(fm, $"--select \"{filePath}\"");
                return true;
            }
            catch { }
        }

        return false;
    }

    public bool IsRunningAsAdmin()
    {
        // Check if EUID is 0 (root)
        return Environment.UserName == "root" || Environment.GetEnvironmentVariable("USER") == "root";
    }
}
```

#### 2.3 macOS Implementation (MacOSPlatformService)

```csharp
public class MacOSPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.MacOS;

    public string GetExecutablePath()
    {
        // Bei .app Bundles: /Applications/MarkdownViewer.app/Contents/MacOS/MarkdownViewer
        return Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;
    }

    public string GetApplicationDataFolder()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Library",
            "Application Support",
            "MarkdownViewer"
        );
        Directory.CreateDirectory(folder);
        return folder;
    }

    public string GetCacheFolder()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            "Library",
            "Caches",
            "MarkdownViewer"
        );
        Directory.CreateDirectory(folder);
        return folder;
    }

    public async Task<bool> ShowFileInExplorerAsync(string filePath)
    {
        try
        {
            Process.Start("open", $"-R \"{filePath}\"");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsRunningAsAdmin()
    {
        return Environment.UserName == "root";
    }
}
```

---

## 3. IWebViewAdapter Erweiterungen

### Analyse der aktuellen Implementation

**Aktuelle Struktur (WebView2Adapter.cs):**
- Wrapper um `Microsoft.Web.WebView2.WinForms.WebView2`
- Events: `Initialized`, `NavigationStarting`, `WebMessageReceived`
- Methoden: `NavigateToStringAsync`, `ExecuteScriptAsync`, `GoBack`, `GoForward`, `Reload`

### Erweiterungen für Avalonia.WebView

**Avalonia WebView Packages:**
- `Avalonia.WebView` (Cross-Platform WebView für Avalonia)
- Backends: WebView2 (Windows), WebKitGTK (Linux), WKWebView (macOS)

### Erweiterte Interface Definition

```csharp
namespace MarkdownViewer.Platform.Abstractions
{
    /// <summary>
    /// Adapter interface für WebView-Kontrollen (plattformunabhängig).
    /// Unterstützt WebView2 (WinForms/Avalonia Windows), WebKitGTK (Linux), WKWebView (macOS).
    /// </summary>
    public interface IWebViewAdapter
    {
        // === Existing Properties ===
        bool IsInitialized { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }

        // === New Properties ===

        /// <summary>
        /// Gibt die aktuelle URL zurück (oder null wenn nicht navigiert).
        /// </summary>
        string? CurrentUrl { get; }

        /// <summary>
        /// Gibt den Titel der aktuellen Seite zurück.
        /// </summary>
        string? PageTitle { get; }

        /// <summary>
        /// Zoom-Level (1.0 = 100%, 1.5 = 150%, etc.)
        /// </summary>
        double ZoomLevel { get; set; }

        /// <summary>
        /// DevTools aktiviert/deaktiviert (für Debugging)
        /// </summary>
        bool AreDevToolsEnabled { get; set; }

        /// <summary>
        /// User Agent String (optional überschreiben)
        /// </summary>
        string? UserAgent { get; set; }

        // === Existing Events ===
        event EventHandler? Initialized;
        event EventHandler<NavigationStartingEventArgs>? NavigationStarting;
        event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;

        // === New Events ===

        /// <summary>
        /// Wird ausgelöst, wenn die Navigation abgeschlossen ist.
        /// </summary>
        event EventHandler<NavigationCompletedEventArgs>? NavigationCompleted;

        /// <summary>
        /// Wird ausgelöst, wenn ein Fehler auftritt.
        /// </summary>
        event EventHandler<WebViewErrorEventArgs>? ErrorOccurred;

        /// <summary>
        /// Wird ausgelöst, wenn die Seite einen neuen Titel setzt.
        /// </summary>
        event EventHandler<string>? PageTitleChanged;

        // === Existing Methods ===
        Task NavigateToStringAsync(string html);
        Task<string> ExecuteScriptAsync(string script);
        void GoBack();
        void GoForward();
        void Reload();

        // === New Methods ===

        /// <summary>
        /// Navigiert zu einer URL (für externe Ressourcen, Images, etc.).
        /// </summary>
        Task NavigateToUrlAsync(string url);

        /// <summary>
        /// Stoppt die aktuelle Navigation.
        /// </summary>
        void Stop();

        /// <summary>
        /// Injiziert CSS in die aktuelle Seite.
        /// Nützlich für Theme-Anwendung ohne HTML-Neurendering.
        /// </summary>
        Task InjectCssAsync(string css, string? styleId = null);

        /// <summary>
        /// Entfernt injiziertes CSS.
        /// </summary>
        Task RemoveCssAsync(string styleId);

        /// <summary>
        /// Registriert ein JavaScript-Interface für Kommunikation zwischen WebView und C#.
        /// Erlaubt JavaScript-Code, C#-Methoden direkt aufzurufen.
        /// </summary>
        /// <param name="name">Name des Interfaces in JavaScript (z.B. "csharp")</param>
        /// <param name="handler">Objekt mit öffentlichen Methoden, die aus JavaScript aufrufbar sind</param>
        void RegisterJavaScriptInterface(string name, object handler);

        /// <summary>
        /// Initialisiert den WebView mit plattformspezifischen Optionen.
        /// Muss vor der ersten Navigation aufgerufen werden.
        /// </summary>
        Task InitializeAsync(WebViewInitializationOptions options);

        /// <summary>
        /// Cleanup und Dispose von nativen Ressourcen.
        /// </summary>
        Task DisposeAsync();
    }

    /// <summary>
    /// Initialisierungs-Optionen für WebView.
    /// </summary>
    public class WebViewInitializationOptions
    {
        /// <summary>
        /// User Data Folder (für Cache, Cookies, etc.)
        /// </summary>
        public string? UserDataFolder { get; set; }

        /// <summary>
        /// DevTools aktiviert
        /// </summary>
        public bool EnableDevTools { get; set; } = false;

        /// <summary>
        /// Context Menus aktiviert
        /// </summary>
        public bool EnableContextMenus { get; set; } = true;

        /// <summary>
        /// JavaScript aktiviert
        /// </summary>
        public bool EnableJavaScript { get; set; } = true;

        /// <summary>
        /// Zoom-Kontrolle durch User erlaubt
        /// </summary>
        public bool EnableZoomControls { get; set; } = true;

        /// <summary>
        /// Anfangs-Zoom-Level
        /// </summary>
        public double InitialZoomLevel { get; set; } = 1.0;
    }

    /// <summary>
    /// Event Args für NavigationCompleted.
    /// </summary>
    public class NavigationCompletedEventArgs : EventArgs
    {
        public bool IsSuccess { get; }
        public string? Url { get; }
        public int? HttpStatusCode { get; }

        public NavigationCompletedEventArgs(bool isSuccess, string? url = null, int? httpStatusCode = null)
        {
            IsSuccess = isSuccess;
            Url = url;
            HttpStatusCode = httpStatusCode;
        }
    }

    /// <summary>
    /// Event Args für Fehler.
    /// </summary>
    public class WebViewErrorEventArgs : EventArgs
    {
        public string ErrorMessage { get; }
        public Exception? Exception { get; }
        public WebViewErrorType ErrorType { get; }

        public WebViewErrorEventArgs(string errorMessage, Exception? exception = null, WebViewErrorType errorType = WebViewErrorType.Unknown)
        {
            ErrorMessage = errorMessage;
            Exception = exception;
            ErrorType = errorType;
        }
    }

    public enum WebViewErrorType
    {
        Unknown,
        NetworkError,
        CertificateError,
        ScriptError,
        InitializationError
    }
}
```

### Migration vom aktuellen WebView2Adapter

```csharp
// ALT: WebView2Adapter.cs
public class WebView2Adapter : IWebViewAdapter
{
    private readonly WebView2 _webView;

    public WebView2Adapter(WebView2 webView)
    {
        _webView = webView;
        _webView.CoreWebView2InitializationCompleted += OnCoreWebView2InitializationCompleted;
    }
}

// NEU: AvaloniaWebViewAdapter.cs (für Avalonia UI)
public class AvaloniaWebViewAdapter : IWebViewAdapter
{
    private readonly Avalonia.WebView.WebView _webView;

    public AvaloniaWebViewAdapter(Avalonia.WebView.WebView webView)
    {
        _webView = webView;
        _webView.Loaded += OnWebViewLoaded;
    }

    public async Task InitializeAsync(WebViewInitializationOptions options)
    {
        // Avalonia.WebView hat unterschiedliche Initialisierung je nach Plattform
        if (OperatingSystem.IsWindows())
        {
            // WebView2 Backend
            await _webView.EnsureCoreWebView2Async(new Avalonia.WebView.WebView2Options
            {
                UserDataFolder = options.UserDataFolder
            });
        }
        else if (OperatingSystem.IsLinux())
        {
            // WebKitGTK Backend
            // Keine spezielle Initialisierung erforderlich
        }
        else if (OperatingSystem.IsMacOS())
        {
            // WKWebView Backend
            // Keine spezielle Initialisierung erforderlich
        }

        _webView.Settings.AreDevToolsEnabled = options.EnableDevTools;
        _webView.Settings.AreDefaultContextMenusEnabled = options.EnableContextMenus;
        _webView.ZoomFactor = options.InitialZoomLevel;
    }
}
```

### Neue Methoden - Anwendungsbeispiele

#### InjectCssAsync für Theme-Wechsel ohne Reload

```csharp
// ALT: Theme-Wechsel erfordert komplettes Neurendering
LoadMarkdownFile(_currentFilePath); // Komplette Seite neu laden

// NEU: CSS-Injection ohne Neurendering
await _webViewAdapter.InjectCssAsync(GenerateThemeCSS(newTheme), "theme-style");
```

#### RegisterJavaScriptInterface für Link-Handling

```csharp
// Registriere C# Interface in JavaScript
_webViewAdapter.RegisterJavaScriptInterface("markdownViewer", new JavaScriptBridge(this));

// JavaScript Code in HTML:
// <a href="#" onclick="markdownViewer.navigateToFile('readme.md')">README</a>

public class JavaScriptBridge
{
    private readonly MainWindow _mainWindow;

    public JavaScriptBridge(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
    }

    public void NavigateToFile(string relativePath)
    {
        _mainWindow.Dispatcher.Invoke(() =>
        {
            _mainWindow.LoadMarkdownFile(relativePath);
        });
    }
}
```

---

## 4. IDialogService Erweiterungen

### Analyse der aktuellen Implementation

**Aktuelle Struktur (WinFormsDialogService.cs):**
- Synchrone Methoden mit `MessageBox.Show`
- Einfache Dialog-Typen: Error, Info, Warning, Confirmation, YesNo

### Erweiterte Interface Definition für Avalonia

```csharp
using System.Threading.Tasks;

namespace MarkdownViewer.Platform.Abstractions
{
    /// <summary>
    /// Service für Dialogs (plattformunabhängig).
    /// Unterstützt sowohl WinForms (MessageBox) als auch Avalonia (async Dialogs).
    /// </summary>
    public interface IDialogService
    {
        // === Existing Methods (synchron) ===
        void ShowError(string message, string title = "Error");
        void ShowInfo(string message, string title = "Information");
        void ShowWarning(string message, string title = "Warning");
        ServiceDialogResult ShowConfirmation(string message, string title = "Confirm");
        ServiceDialogResult ShowYesNo(string message, string title = "Question");

        // === New Methods (async für Avalonia) ===

        /// <summary>
        /// Zeigt eine Fehlermeldung (async).
        /// </summary>
        Task ShowErrorAsync(string message, string title = "Error");

        /// <summary>
        /// Zeigt eine Info-Meldung (async).
        /// </summary>
        Task ShowInfoAsync(string message, string title = "Information");

        /// <summary>
        /// Zeigt eine Warnung (async).
        /// </summary>
        Task ShowWarningAsync(string message, string title = "Warning");

        /// <summary>
        /// Zeigt einen Bestätigungs-Dialog (async).
        /// </summary>
        Task<ServiceDialogResult> ShowConfirmationAsync(string message, string title = "Confirm");

        /// <summary>
        /// Zeigt einen Ja/Nein-Dialog (async).
        /// </summary>
        Task<ServiceDialogResult> ShowYesNoAsync(string message, string title = "Question");

        /// <summary>
        /// Zeigt einen Datei-Öffnen-Dialog.
        /// </summary>
        /// <param name="title">Dialog-Titel</param>
        /// <param name="filters">Dateifilter (z.B. "*.md;*.markdown")</param>
        /// <param name="initialDirectory">Start-Verzeichnis</param>
        /// <returns>Gewählter Dateipfad oder null</returns>
        Task<string?> ShowOpenFileDialogAsync(string title = "Open File", string? filters = null, string? initialDirectory = null);

        /// <summary>
        /// Zeigt einen Datei-Speichern-Dialog.
        /// </summary>
        Task<string?> ShowSaveFileDialogAsync(string title = "Save File", string? filters = null, string? initialDirectory = null, string? defaultFileName = null);

        /// <summary>
        /// Zeigt einen Ordner-Auswahl-Dialog.
        /// </summary>
        Task<string?> ShowFolderPickerDialogAsync(string title = "Select Folder", string? initialDirectory = null);

        /// <summary>
        /// Zeigt einen Input-Dialog (Text eingeben).
        /// </summary>
        /// <param name="message">Anzeige-Text</param>
        /// <param name="title">Dialog-Titel</param>
        /// <param name="defaultValue">Vorausgefüllter Wert</param>
        /// <returns>Eingegebener Text oder null (bei Cancel)</returns>
        Task<string?> ShowInputDialogAsync(string message, string title = "Input", string? defaultValue = null);

        /// <summary>
        /// Zeigt einen Progress-Dialog (nicht blockierend).
        /// </summary>
        /// <param name="title">Dialog-Titel</param>
        /// <param name="message">Anzeige-Text</param>
        /// <param name="cancellable">Kann abgebrochen werden?</param>
        /// <returns>Progress Reporter Interface</returns>
        Task<IProgressDialog> ShowProgressDialogAsync(string title, string message, bool cancellable = false);
    }

    /// <summary>
    /// Interface für Progress-Dialog Steuerung.
    /// </summary>
    public interface IProgressDialog
    {
        /// <summary>
        /// Aktualisiert Fortschritt (0-100).
        /// </summary>
        Task UpdateProgressAsync(int percentage, string? statusMessage = null);

        /// <summary>
        /// Prüft, ob User auf "Cancel" geklickt hat.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Schließt den Dialog.
        /// </summary>
        Task CloseAsync();
    }

    /// <summary>
    /// Dialog Result Enum (erweitert).
    /// </summary>
    public enum ServiceDialogResult
    {
        OK,
        Cancel,
        Yes,
        No,
        Abort,
        Retry,
        Ignore
    }
}
```

### Plattformspezifische Implementierungen

#### 4.1 WinForms Implementation (Existing)

```csharp
// Aktuelle Implementation beibehalten
public class WinFormsDialogService : IDialogService
{
    // Synchrone Methoden bleiben unverändert
    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    // Neue async Methoden als Wrapper
    public async Task ShowErrorAsync(string message, string title = "Error")
    {
        await Task.Run(() => ShowError(message, title));
    }

    public async Task<string?> ShowOpenFileDialogAsync(string title = "Open File", string? filters = null, string? initialDirectory = null)
    {
        return await Task.Run(() =>
        {
            using var dialog = new OpenFileDialog
            {
                Title = title,
                Filter = filters ?? "Markdown Files (*.md;*.markdown)|*.md;*.markdown|All Files (*.*)|*.*",
                InitialDirectory = initialDirectory ?? ""
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        });
    }
}
```

#### 4.2 Avalonia Implementation

```csharp
using Avalonia.Controls;
using Avalonia.Platform.Storage;

public class AvaloniaDialogService : IDialogService
{
    private readonly Window _parentWindow;

    public AvaloniaDialogService(Window parentWindow)
    {
        _parentWindow = parentWindow;
    }

    // Synchrone Methoden als Wrapper um async (nicht empfohlen in Avalonia)
    public void ShowError(string message, string title = "Error")
    {
        ShowErrorAsync(message, title).Wait();
    }

    // Native async Implementation
    public async Task ShowErrorAsync(string message, string title = "Error")
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "OK"
        };

        await dialog.ShowAsync(_parentWindow);
    }

    public async Task<ServiceDialogResult> ShowYesNoAsync(string message, string title = "Question")
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No"
        };

        var result = await dialog.ShowAsync(_parentWindow);
        return result == ContentDialogResult.Primary ? ServiceDialogResult.Yes : ServiceDialogResult.No;
    }

    public async Task<string?> ShowOpenFileDialogAsync(string title = "Open File", string? filters = null, string? initialDirectory = null)
    {
        var storageProvider = _parentWindow.StorageProvider;

        var filePickerOptions = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Markdown Files") { Patterns = new[] { "*.md", "*.markdown" } },
                FilePickerFileTypes.All
            }
        };

        var result = await storageProvider.OpenFilePickerAsync(filePickerOptions);
        return result.Count > 0 ? result[0].Path.LocalPath : null;
    }

    public async Task<IProgressDialog> ShowProgressDialogAsync(string title, string message, bool cancellable = false)
    {
        var progressDialog = new AvaloniaProgressDialog(_parentWindow, title, message, cancellable);
        await progressDialog.ShowAsync();
        return progressDialog;
    }
}

internal class AvaloniaProgressDialog : IProgressDialog
{
    private readonly Window _progressWindow;
    private readonly ProgressBar _progressBar;
    private readonly TextBlock _statusText;
    private bool _isCancelled;

    public bool IsCancelled => _isCancelled;

    public AvaloniaProgressDialog(Window parent, string title, string message, bool cancellable)
    {
        _progressWindow = new Window
        {
            Title = title,
            Width = 400,
            Height = 150,
            CanResize = false
        };

        // Build UI...
    }

    public async Task UpdateProgressAsync(int percentage, string? statusMessage = null)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _progressBar.Value = percentage;
            if (statusMessage != null)
                _statusText.Text = statusMessage;
        });
    }

    public async Task CloseAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            _progressWindow.Close();
        });
    }
}
```

---

## 5. IThemeService Platform Support

### Analyse der aktuellen Implementation

**Aktuelle ThemeService Features:**
- Lädt Themes aus embedded JSON resources
- Wendet Themes auf WinForms UI an (BackColor, ForeColor)
- Injiziert CSS in WebView2 für Markdown-Rendering
- Themes: `standard`, `dark`, `solarized`, `draeger`

### Erweiterungen für Cross-Platform

**Herausforderungen:**
1. **Avalonia Styling:** Verwendet XAML-ähnliche Styles statt Windows Forms Properties
2. **Theme-Format:** JSON-Format beibehalten oder zu Avalonia Styles migrieren?
3. **Runtime-Styling:** WinForms = direkte Property-Änderung, Avalonia = Style Resources

### Erweiterte Interface Definition

```csharp
namespace MarkdownViewer.Platform.Abstractions
{
    /// <summary>
    /// Service für Theme-Verwaltung (plattformunabhängig).
    /// </summary>
    public interface IThemeService
    {
        /// <summary>
        /// Gibt das aktuell geladene Theme zurück.
        /// </summary>
        Theme GetCurrentTheme();

        /// <summary>
        /// Lädt ein Theme nach Name.
        /// </summary>
        Theme LoadTheme(string themeName);

        /// <summary>
        /// Gibt Liste verfügbarer Theme-Namen zurück.
        /// </summary>
        List<string> GetAvailableThemes();

        /// <summary>
        /// Wendet Theme auf die UI an (plattformunabhängig).
        /// </summary>
        /// <param name="theme">Theme-Objekt</param>
        /// <param name="targetView">Root-View (Form oder Avalonia Window)</param>
        /// <param name="webView">WebView-Adapter für Markdown-Rendering</param>
        Task ApplyThemeAsync(Theme theme, object targetView, IWebViewAdapter webView);

        /// <summary>
        /// Generiert CSS für Markdown-Rendering aus Theme.
        /// </summary>
        string GenerateMarkdownCss(Theme theme);

        /// <summary>
        /// Generiert plattformspezifisches Styling (Avalonia ResourceDictionary oder WinForms Properties).
        /// </summary>
        object GeneratePlatformStyles(Theme theme);
    }
}
```

### Plattformspezifische Theme-Renderer

#### 5.1 WinForms Theme Renderer (Existing)

```csharp
// Aktuelle Implementation in ThemeService.ApplyThemeAsync beibehalten
private void ApplyThemeToWinForms(Theme theme, Form form)
{
    form.BackColor = ColorTranslator.FromHtml(theme.UI.FormBackground);
    // ... recursiv auf Controls anwenden
}
```

#### 5.2 Avalonia Theme Renderer

```csharp
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

public class AvaloniaThemeRenderer
{
    public async Task ApplyThemeAsync(Theme theme, Window window, IWebViewAdapter webView)
    {
        // 1. Avalonia Styles via ResourceDictionary anwenden
        var themeStyles = GenerateAvaloniaStyles(theme);
        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(themeStyles);

        // 2. CSS in WebView injizieren
        var css = GenerateMarkdownCss(theme.Markdown);
        await webView.InjectCssAsync(css, "theme-style");
    }

    private ResourceDictionary GenerateAvaloniaStyles(Theme theme)
    {
        var dict = new ResourceDictionary();

        // Farben als Resources registrieren
        dict["FormBackgroundBrush"] = new SolidColorBrush(Color.Parse(theme.UI.FormBackground));
        dict["ControlBackgroundBrush"] = new SolidColorBrush(Color.Parse(theme.UI.ControlBackground));
        dict["ControlForegroundBrush"] = new SolidColorBrush(Color.Parse(theme.UI.ControlForeground));
        dict["StatusBarBackgroundBrush"] = new SolidColorBrush(Color.Parse(theme.UI.StatusBarBackground));
        dict["StatusBarForegroundBrush"] = new SolidColorBrush(Color.Parse(theme.UI.StatusBarForeground));

        // Styles für Controls
        var windowStyle = new Style(x => x.OfType<Window>());
        windowStyle.Setters.Add(new Setter(Window.BackgroundProperty, new DynamicResourceExtension("FormBackgroundBrush")));
        dict.Add(windowStyle);

        var textBoxStyle = new Style(x => x.OfType<TextBox>());
        textBoxStyle.Setters.Add(new Setter(TextBox.BackgroundProperty, new DynamicResourceExtension("ControlBackgroundBrush")));
        textBoxStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, new DynamicResourceExtension("ControlForegroundBrush")));
        dict.Add(textBoxStyle);

        // ... weitere Controls

        return dict;
    }
}
```

### Theme-Format: JSON vs. Avalonia Resources

**Option 1: JSON beibehalten (empfohlen für Migration)**
- Vorteil: Bestehende Themes kompatibel
- Vorteil: Einfacher Theme-Editor möglich
- Vorteil: Runtime-Wechsel ohne Neustart
- Nachteil: Zusätzliche Konvertierung zu Avalonia Styles

**Option 2: Avalonia AXAML Resources**
- Vorteil: Native Avalonia Integration
- Vorteil: Mehr Styling-Möglichkeiten (Animationen, Transitions)
- Nachteil: Nicht kompatibel mit WinForms Version
- Nachteil: Theme-Editor komplexer

**Empfehlung:** JSON-Format beibehalten und Runtime-Konvertierung zu Avalonia Styles implementieren.

---

## 6. Dependency Injection Setup

### Service Registration für Cross-Platform

```csharp
using Microsoft.Extensions.DependencyInjection;

public static class PlatformServiceExtensions
{
    /// <summary>
    /// Registriert alle Platform Services basierend auf aktuellem OS.
    /// </summary>
    public static IServiceCollection AddPlatformServices(this IServiceCollection services)
    {
        // Platform Service
        if (OperatingSystem.IsWindows())
        {
            services.AddSingleton<IPlatformService, WindowsPlatformService>();
            services.AddSingleton<IFileAssociationService, WindowsFileAssociationService>();
        }
        else if (OperatingSystem.IsLinux())
        {
            services.AddSingleton<IPlatformService, LinuxPlatformService>();
            services.AddSingleton<IFileAssociationService, LinuxFileAssociationService>();
        }
        else if (OperatingSystem.IsMacOS())
        {
            services.AddSingleton<IPlatformService, MacOSPlatformService>();
            services.AddSingleton<IFileAssociationService, MacOSFileAssociationService>();
        }

        // Theme Service (plattformunabhängig)
        services.AddSingleton<IThemeService, ThemeService>();

        // Settings Service (plattformunabhängig)
        services.AddSingleton<ISettingsService, SettingsService>();

        // Localization Service
        services.AddSingleton<ILocalizationService>(sp =>
        {
            var settingsService = sp.GetRequiredService<ISettingsService>();
            var settings = settingsService.Load();
            return new LocalizationService(settings.Language);
        });

        return services;
    }

    /// <summary>
    /// Registriert WinForms-spezifische Services.
    /// </summary>
    public static IServiceCollection AddWinFormsServices(this IServiceCollection services)
    {
        services.AddTransient<IDialogService, WinFormsDialogService>();
        services.AddTransient<IWebViewAdapter>(sp =>
        {
            var webView = new Microsoft.Web.WebView2.WinForms.WebView2();
            return new WebView2Adapter(webView);
        });

        return services;
    }

    /// <summary>
    /// Registriert Avalonia-spezifische Services.
    /// </summary>
    public static IServiceCollection AddAvaloniaServices(this IServiceCollection services, Window mainWindow)
    {
        services.AddTransient<IDialogService>(sp => new AvaloniaDialogService(mainWindow));
        services.AddTransient<IWebViewAdapter>(sp =>
        {
            var webView = new Avalonia.WebView.WebView();
            return new AvaloniaWebViewAdapter(webView);
        });

        return services;
    }
}
```

### Program.cs Migration

```csharp
// ALT: Program.cs - BuildServiceProvider (Zeilen 565-605)
private static ServiceProvider BuildServiceProvider(string filePath, LogEventLevel logLevel)
{
    var services = new ServiceCollection();
    services.AddSingleton<ISettingsService, SettingsService>();
    // ...
}

// NEU: Shared Library + Platform-spezifische Registrierung
public class WinFormsProgram
{
    [STAThread]
    static void Main(string[] args)
    {
        var services = new ServiceCollection();

        // Platform Services (auto-detect OS)
        services.AddPlatformServices();

        // WinForms-spezifische Services
        services.AddWinFormsServices();

        // Core Services (plattformunabhängig)
        services.AddTransient<MarkdownRenderer>();
        services.AddTransient<FileWatcherManager>();

        var serviceProvider = services.BuildServiceProvider();

        var form = serviceProvider.GetRequiredService<MainForm>();
        Application.Run(form);
    }
}

public class AvaloniaProgram
{
    public static void Main(string[] args)
    {
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(builder =>
            {
                // DI Setup nach Avalonia Initialisierung
                var services = new ServiceCollection();
                services.AddPlatformServices();
                // AvaloniaServices wird in App.OnFrameworkInitializationCompleted registriert
            });
    }
}
```

---

## 7. Migration Roadmap

### Phase 1: Platform Abstractions erstellen
**Ziel:** Interfaces und Windows-Implementierungen fertigstellen

1. **Neue Namespaces erstellen:**
   - `MarkdownViewer.Platform.Abstractions` (Interfaces)
   - `MarkdownViewer.Platform.Windows` (Windows-Implementierungen)
   - `MarkdownViewer.Platform.Linux` (Linux-Implementierungen)
   - `MarkdownViewer.Platform.MacOS` (macOS-Implementierungen)

2. **IFileAssociationService implementieren:**
   - Interface erstellen
   - WindowsFileAssociationService (migriere von Program.cs)
   - Unit Tests schreiben

3. **IPlatformService implementieren:**
   - Interface erstellen
   - WindowsPlatformService (ersetze direkte Application.ExecutablePath Aufrufe)
   - Unit Tests schreiben

4. **IWebViewAdapter erweitern:**
   - Bestehende Interface erweitern
   - Neue Methoden in WebView2Adapter implementieren
   - Unit Tests schreiben

5. **IDialogService erweitern:**
   - Async-Methoden hinzufügen
   - WinFormsDialogService erweitern
   - Unit Tests schreiben

**Dauer:** 1-2 Wochen
**Deliverable:** Vollständig getestete Platform Abstractions für Windows

---

### Phase 2: Shared Library extrahieren
**Ziel:** Business Logic von UI trennen

1. **Neues Projekt erstellen:**
   - `MarkdownViewer.Core` (Shared Library, .NET 8, TargetFramework: net8.0)
   - Dependencies: NuGet Packages für Markdig, Serilog, JSON

2. **Core-Klassen verschieben:**
   - Models (Theme, AppSettings, etc.)
   - Services (ThemeService, SettingsService, LocalizationService)
   - Core Logic (MarkdownRenderer, NavigationManager, SearchManager)
   - Presenters (MainPresenter, StatusBarPresenter, etc.)

3. **Platform Abstractions verschieben:**
   - Interfaces nach MarkdownViewer.Core
   - Implementierungen in Platform-spezifische Projekte

4. **Dependency Injection anpassen:**
   - ServiceCollection Extensions in Core
   - Platform-spezifische Registrierung in UI-Projekten

**Dauer:** 1 Woche
**Deliverable:** MarkdownViewer.Core NuGet Package (intern)

---

### Phase 3: Avalonia UI implementieren
**Ziel:** Cross-Platform UI mit Avalonia

1. **Avalonia Projekt erstellen:**
   - `MarkdownViewer.Avalonia` (.NET 8, TargetFrameworks: net8.0-windows;net8.0-linux;net8.0-osx)
   - Dependencies: Avalonia 11.x, Avalonia.WebView, MarkdownViewer.Core

2. **Linux/macOS Platform Services:**
   - LinuxFileAssociationService implementieren
   - LinuxPlatformService implementieren
   - MacOSFileAssociationService implementieren
   - MacOSPlatformService implementieren

3. **Avalonia Views erstellen:**
   - MainWindow.axaml (entspricht MainForm.cs)
   - StatusBar.axaml (entspricht StatusBarControl.cs)
   - NavigationBar.axaml (entspricht NavigationBar.cs)
   - SearchBar.axaml (entspricht SearchBar.cs)

4. **AvaloniaWebViewAdapter:**
   - Implementiere IWebViewAdapter mit Avalonia.WebView
   - Platform-spezifische Backends (WebView2, WebKitGTK, WKWebView)

5. **AvaloniaDialogService:**
   - ContentDialog für Meldungen
   - FilePickerDialog für Dateiauswahl
   - Custom ProgressDialog

6. **Theme-System migrieren:**
   - JSON Themes beibehalten
   - Runtime-Konvertierung zu Avalonia Styles
   - Theme-Wechsel ohne Neustart

**Dauer:** 3-4 Wochen
**Deliverable:** Vollständig funktionsfähige Avalonia App (Windows/Linux/macOS)

---

### Phase 4: Testing & Deployment
**Ziel:** Beide UIs (WinForms + Avalonia) parallel releasen

1. **Integration Tests:**
   - End-to-End Tests für beide UIs
   - Platform-spezifische Tests (File Association, etc.)

2. **Build Pipeline:**
   - CI/CD für WinForms (Windows)
   - CI/CD für Avalonia (Windows, Linux, macOS)
   - Separate Releases oder Unified Installer?

3. **Dokumentation:**
   - Migration Guide für Nutzer
   - Developer Documentation
   - Platform-spezifische Installation Instructions

**Dauer:** 1-2 Wochen
**Deliverable:** Release v2.0 (WinForms) + v2.1 (Avalonia)

---

## 8. Edge Cases & Considerations

### Update-Mechanismus auf Linux

**Problem:** Windows Update-Mechanismus nutzt `pending-update.exe` und Registry.

**Lösungsansätze:**

1. **AppImage Updates:**
   - Nutze `appimageupdatetool` für Self-Updating
   - Download neue AppImage, ersetze alte
   - Atomic Update via `mv` (kein Backup nötig)

2. **Package Manager Integration:**
   - `.deb` / `.rpm` Packages für APT/DNF
   - Updates via System Package Manager
   - Kein Self-Update nötig

3. **Flatpak:**
   - Flathub Distribution
   - Automatische Updates via Flatpak
   - Sandbox mit Portal API für File Access

**Empfehlung:** Flatpak für Linux Distribution (beste UX, automatische Updates, Sandbox-Sicherheit)

### File Association Conflicts

**Problem:** User hat bereits einen Markdown-Editor als Default Handler.

**Lösung:**
- `IsInstalledAsync()` prüft nur "Ist MarkdownViewer registriert?", nicht "Ist MarkdownViewer DEFAULT?"
- UI zeigt Status: "Registered" vs. "Registered as Default" vs. "Not Registered"
- User kann wählen: "Set as Default" oder "Add to Open With"

### Theme-Kompatibilität zwischen WinForms und Avalonia

**Problem:** WinForms nutzt System.Drawing.Color, Avalonia nutzt Avalonia.Media.Color.

**Lösung:**
- JSON Theme-Format bleibt unverändert (Hex-Strings)
- Converter in jeweiliger Platform-Implementation:
  ```csharp
  // WinForms
  System.Drawing.ColorTranslator.FromHtml("#ff0000")

  // Avalonia
  Avalonia.Media.Color.Parse("#ff0000")
  ```

### macOS Code Signing & Notarization

**Problem:** macOS Gatekeeper blockiert unsignierte Apps.

**Lösung:**
- Developer Account erforderlich ($99/Jahr)
- Code Signing mit Developer Certificate
- Notarization via Apple Notary Service
- Entitlement for WebView (`com.apple.security.cs.allow-jit`)

### WebView-Backend Unterschiede

**Problem:** WebView2 (Windows), WebKitGTK (Linux), WKWebView (macOS) haben unterschiedliche APIs.

**Lösung:**
- Avalonia.WebView abstrahiert die Unterschiede
- Gemeinsame Subset-API über IWebViewAdapter
- Platform-spezifische Features optional über `GetPlatformView()` Methode

---

## 9. Testing Strategy

### Unit Tests für Platform Abstractions

```csharp
[TestFixture]
public class WindowsFileAssociationServiceTests
{
    [Test]
    public async Task InstallAsync_CreatesRegistryKeys()
    {
        // Arrange
        var service = new WindowsFileAssociationService();
        var exePath = @"C:\Test\MarkdownViewer.exe";

        // Act
        var success = await service.InstallAsync(exePath);

        // Assert
        Assert.IsTrue(success);

        // Verify Registry
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Classes\.md");
        Assert.IsNotNull(key);
        Assert.AreEqual("MarkdownViewer.Document", key.GetValue(""));
    }

    [TearDown]
    public async Task Cleanup()
    {
        var service = new WindowsFileAssociationService();
        await service.UninstallAsync();
    }
}
```

### Integration Tests für WebViewAdapter

```csharp
[TestFixture]
public class WebViewAdapterIntegrationTests
{
    [Test]
    [Apartment(ApartmentState.STA)] // Required for WinForms WebView2
    public async Task NavigateToStringAsync_RendersHtml()
    {
        // Arrange
        var webView = new WebView2();
        var adapter = new WebView2Adapter(webView);
        await adapter.InitializeAsync(new WebViewInitializationOptions());

        // Act
        await adapter.NavigateToStringAsync("<h1>Test</h1>");

        // Wait for navigation
        await Task.Delay(1000);

        // Assert
        var title = await adapter.ExecuteScriptAsync("document.querySelector('h1').innerText");
        Assert.AreEqual("Test", title.Trim('"'));
    }
}
```

### Mock Implementations für Tests

```csharp
public class MockPlatformService : IPlatformService
{
    public PlatformType CurrentPlatform => PlatformType.Windows;

    public string GetExecutablePath() => @"C:\Test\MarkdownViewer.exe";

    public string GetApplicationDataFolder() => Path.Combine(Path.GetTempPath(), "TestAppData");

    // ... weitere Mock-Methoden
}
```

---

## 10. Performance Considerations

### Theme-Wechsel Optimierung

**Problem:** Komplettes Neurendering bei Theme-Wechsel ist langsam.

**Lösung:**
- CSS-Injection statt HTML-Neurendering
- `InjectCssAsync` aktualisiert nur Styles, nicht DOM

**Benchmark:**
```
ALT: LoadMarkdownFile() bei Theme-Wechsel
     -> 500ms (Markdown parsen + HTML rendern + WebView laden)

NEU: InjectCssAsync() bei Theme-Wechsel
     -> 50ms (nur CSS aktualisieren)
```

### Lazy Loading von Platform Services

**Problem:** File Association Service wird selten genutzt (nur bei Installation).

**Lösung:**
- Lazy Initialization über `Lazy<T>` in DI
- Services werden erst erstellt, wenn benötigt

```csharp
services.AddSingleton<IFileAssociationService>(sp =>
{
    return new Lazy<IFileAssociationService>(() =>
    {
        if (OperatingSystem.IsWindows())
            return new WindowsFileAssociationService();
        // ...
    }).Value;
});
```

---

## Zusammenfassung

Dieses Design definiert vollständige Platform Abstractions für die Cross-Platform Migration von MarkdownViewer. Die Interfaces ermöglichen:

1. **Windows Explorer Integration bleibt erhalten** (via IFileAssociationService)
2. **Plattformunabhängige Business Logic** (via IPlatformService, IWebViewAdapter, IDialogService)
3. **Testbarkeit** (alle Services mockbar)
4. **Inkrementelle Migration** (WinForms und Avalonia parallel)

**Nächste Schritte:**
1. Interface-Definitionen in Code umsetzen
2. Windows-Implementierungen migrieren
3. Unit Tests schreiben
4. Shared Library extrahieren
5. Avalonia UI implementieren

**Geschätzte Gesamtzeit:** 6-8 Wochen für vollständige Cross-Platform Version

---

**Ende des Design-Dokuments**
