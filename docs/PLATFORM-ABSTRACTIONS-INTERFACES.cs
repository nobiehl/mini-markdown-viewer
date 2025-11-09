// ================================================================================
// Platform Abstractions Interfaces für MarkdownViewer Cross-Platform Support
// Version: 1.0
// Datum: 2025-11-08
//
// Diese Datei enthält alle Interface-Definitionen für Platform Abstractions.
// Kopiere diese Definitionen in dein MarkdownViewer.Platform.Abstractions Projekt.
// ================================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MarkdownViewer.Platform.Abstractions
{
    // ============================================================================
    // 1. IFileAssociationService
    // ============================================================================

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

    // ============================================================================
    // 2. IPlatformService
    // ============================================================================

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

    // ============================================================================
    // 3. IWebViewAdapter (Erweiterte Version)
    // ============================================================================

    /// <summary>
    /// Adapter interface für WebView-Kontrollen (plattformunabhängig).
    /// Unterstützt WebView2 (WinForms/Avalonia Windows), WebKitGTK (Linux), WKWebView (macOS).
    /// </summary>
    public interface IWebViewAdapter
    {
        // === Properties ===
        bool IsInitialized { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }

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

        // === Events ===
        event EventHandler? Initialized;
        event EventHandler<NavigationStartingEventArgs>? NavigationStarting;
        event EventHandler<WebMessageReceivedEventArgs>? WebMessageReceived;

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

        // === Methods ===
        Task NavigateToStringAsync(string html);
        Task<string> ExecuteScriptAsync(string script);
        void GoBack();
        void GoForward();
        void Reload();

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
    /// Event Args für Navigation Starting.
    /// </summary>
    public class NavigationStartingEventArgs : EventArgs
    {
        public string Uri { get; }
        public bool Cancel { get; set; }
        public NavigationStartingEventArgs(string uri) => Uri = uri;
    }

    /// <summary>
    /// Event Args für Web Message Received.
    /// </summary>
    public class WebMessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }
        public WebMessageReceivedEventArgs(string message) => Message = message;
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

    // ============================================================================
    // 4. IDialogService (Erweiterte Version)
    // ============================================================================

    /// <summary>
    /// Service für Dialogs (plattformunabhängig).
    /// Unterstützt sowohl WinForms (MessageBox) als auch Avalonia (async Dialogs).
    /// </summary>
    public interface IDialogService
    {
        // === Synchrone Methoden (für WinForms-Kompatibilität) ===
        void ShowError(string message, string title = "Error");
        void ShowInfo(string message, string title = "Information");
        void ShowWarning(string message, string title = "Warning");
        ServiceDialogResult ShowConfirmation(string message, string title = "Confirm");
        ServiceDialogResult ShowYesNo(string message, string title = "Question");

        // === Async Methoden (empfohlen für Avalonia) ===

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
    /// Dialog Result Enum.
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

    // ============================================================================
    // 5. IThemeService (Erweiterte Version)
    // ============================================================================

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

    // Theme-Modell (aus bestehender Theme.cs übernommen)
    public class Theme
    {
        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public MarkdownColors Markdown { get; set; } = new();
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
}

// ================================================================================
// END OF FILE
// ================================================================================
