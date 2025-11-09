# Platform Abstractions - Architektur-Diagramme

**Version:** 1.0
**Datum:** 2025-11-08

Dieses Dokument enthält visuelle Diagramme zur Platform Abstractions Architektur für MarkdownViewer.

---

## 1. Solution-Struktur Übersicht

```mermaid
graph TB
    subgraph "UI Layer"
        WF[MarkdownViewer<br/>WinForms]
        AV[MarkdownViewer<br/>Avalonia]
    end

    subgraph "Core Layer"
        CORE[MarkdownViewer.Core<br/>Business Logic]
    end

    subgraph "Platform Abstraction Layer"
        ABS[MarkdownViewer.Platform<br/>Abstractions<br/>Interfaces]
        WIN[MarkdownViewer.Platform<br/>Windows]
        LIN[MarkdownViewer.Platform<br/>Linux]
        MAC[MarkdownViewer.Platform<br/>MacOS]
    end

    WF --> CORE
    WF --> ABS
    WF --> WIN

    AV --> CORE
    AV --> ABS
    AV --> WIN
    AV --> LIN
    AV --> MAC

    CORE --> ABS

    WIN --> ABS
    LIN --> ABS
    MAC --> ABS

    style CORE fill:#4A90E2,stroke:#2E5C8A,color:#fff
    style ABS fill:#50C878,stroke:#2D7A4A,color:#fff
    style WF fill:#FFB347,stroke:#CC8A39,color:#000
    style AV fill:#FFB347,stroke:#CC8A39,color:#000
```

---

## 2. Platform Abstractions Interfaces

```mermaid
classDiagram
    class IFileAssociationService {
        +InstallAsync(path, options)
        +UninstallAsync()
        +IsInstalledAsync()
        +GetStatusAsync()
    }

    class IPlatformService {
        +GetExecutablePath()
        +GetApplicationDataFolder()
        +GetLogsFolder()
        +GetCacheFolder()
        +RestartApplicationAsync()
        +OpenUrlInBrowserAsync()
        +ShowFileInExplorerAsync()
        +GetPlatformInfo()
        +IsRunningAsAdmin()
    }

    class IWebViewAdapter {
        +NavigateToStringAsync()
        +ExecuteScriptAsync()
        +InjectCssAsync()
        +RegisterJavaScriptInterface()
        +GoBack()
        +GoForward()
        +Reload()
    }

    class IDialogService {
        +ShowErrorAsync()
        +ShowInfoAsync()
        +ShowWarningAsync()
        +ShowYesNoAsync()
        +ShowOpenFileDialogAsync()
        +ShowSaveFileDialogAsync()
        +ShowProgressDialogAsync()
    }

    class IThemeService {
        +LoadTheme()
        +ApplyThemeAsync()
        +GenerateMarkdownCss()
        +GeneratePlatformStyles()
        +GetAvailableThemes()
    }

    class MainPresenter {
        -IFileAssociationService
        -IPlatformService
        -IWebViewAdapter
        -IDialogService
        -IThemeService
    }

    MainPresenter --> IFileAssociationService
    MainPresenter --> IPlatformService
    MainPresenter --> IWebViewAdapter
    MainPresenter --> IDialogService
    MainPresenter --> IThemeService
```

---

## 3. Platform-spezifische Implementierungen

```mermaid
graph LR
    subgraph "Interfaces"
        IFA[IFileAssociationService]
        IPS[IPlatformService]
        IWV[IWebViewAdapter]
        IDS[IDialogService]
    end

    subgraph "Windows"
        WFA[WindowsFileAssociationService]
        WPS[WindowsPlatformService]
        WV2[WebView2Adapter]
        WDS[WinFormsDialogService]
    end

    subgraph "Linux"
        LFA[LinuxFileAssociationService]
        LPS[LinuxPlatformService]
        LWV[LinuxWebViewAdapter]
        LDS[AvaloniaDialogService]
    end

    subgraph "macOS"
        MFA[MacOSFileAssociationService]
        MPS[MacOSPlatformService]
        MWV[MacOSWebViewAdapter]
        MDS[AvaloniaDialogService]
    end

    IFA -.implements.-> WFA
    IFA -.implements.-> LFA
    IFA -.implements.-> MFA

    IPS -.implements.-> WPS
    IPS -.implements.-> LPS
    IPS -.implements.-> MPS

    IWV -.implements.-> WV2
    IWV -.implements.-> LWV
    IWV -.implements.-> MWV

    IDS -.implements.-> WDS
    IDS -.implements.-> LDS
    IDS -.implements.-> MDS

    style IFA fill:#50C878,color:#fff
    style IPS fill:#50C878,color:#fff
    style IWV fill:#50C878,color:#fff
    style IDS fill:#50C878,color:#fff
```

---

## 4. Dependency Injection Flow

```mermaid
sequenceDiagram
    participant App as Application
    participant DI as ServiceCollection
    participant Detector as Platform Detector
    participant Win as Windows Services
    participant Lin as Linux Services
    participant Mac as macOS Services

    App->>DI: AddPlatformServices()
    DI->>Detector: OperatingSystem.IsWindows()?
    alt Windows
        Detector->>Win: Register Windows Services
        Win->>DI: AddSingleton<IFileAssociationService, WindowsFileAssociationService>
        Win->>DI: AddSingleton<IPlatformService, WindowsPlatformService>
    else Linux
        Detector->>Lin: Register Linux Services
        Lin->>DI: AddSingleton<IFileAssociationService, LinuxFileAssociationService>
        Lin->>DI: AddSingleton<IPlatformService, LinuxPlatformService>
    else macOS
        Detector->>Mac: Register macOS Services
        Mac->>DI: AddSingleton<IFileAssociationService, MacOSFileAssociationService>
        Mac->>DI: AddSingleton<IPlatformService, MacOSPlatformService>
    end

    DI-->>App: ServiceProvider
```

---

## 5. Theme-System Architektur

```mermaid
graph TB
    JSON[Theme JSON Files<br/>standard.json<br/>dark.json<br/>solarized.json]

    TS[ThemeService]

    JSON --> TS

    subgraph "WinForms"
        WFR[WinFormsThemeRenderer]
        WFC[Form.BackColor<br/>Control.ForeColor]
    end

    subgraph "Avalonia"
        AVR[AvaloniaThemeRenderer]
        AVS[ResourceDictionary<br/>SolidColorBrush<br/>Styles]
    end

    subgraph "WebView"
        CSS[CSS Injection<br/>InjectCssAsync]
        MD[Markdown Rendering]
    end

    TS --> WFR
    TS --> AVR
    TS --> CSS

    WFR --> WFC
    AVR --> AVS
    CSS --> MD

    style JSON fill:#FFD700,stroke:#B8860B,color:#000
    style TS fill:#4A90E2,stroke:#2E5C8A,color:#fff
    style CSS fill:#50C878,stroke:#2D7A4A,color:#fff
```

---

## 6. WebView Adapter Hierarchie

```mermaid
classDiagram
    class IWebViewAdapter {
        <<interface>>
        +NavigateToStringAsync()
        +ExecuteScriptAsync()
        +InjectCssAsync()
        +RegisterJavaScriptInterface()
    }

    class WebView2Adapter {
        -WebView2 _webView
        +Platform: Windows
        +Backend: WebView2
    }

    class AvaloniaWebViewAdapter {
        -Avalonia.WebView.WebView _webView
        +Platform: Cross-Platform
    }

    class WindowsWebViewBackend {
        +Backend: WebView2
    }

    class LinuxWebViewBackend {
        +Backend: WebKitGTK
    }

    class MacOSWebViewBackend {
        +Backend: WKWebView
    }

    IWebViewAdapter <|.. WebView2Adapter
    IWebViewAdapter <|.. AvaloniaWebViewAdapter

    AvaloniaWebViewAdapter --> WindowsWebViewBackend
    AvaloniaWebViewAdapter --> LinuxWebViewBackend
    AvaloniaWebViewAdapter --> MacOSWebViewBackend

    style IWebViewAdapter fill:#50C878,color:#fff
```

---

## 7. File Association Workflow

```mermaid
stateDiagram-v2
    [*] --> NotInstalled

    NotInstalled --> Installing : User clicks Install
    Installing --> CheckingPlatform : Detect OS

    CheckingPlatform --> WindowsInstall : Windows
    CheckingPlatform --> LinuxInstall : Linux
    CheckingPlatform --> MacOSInstall : macOS

    WindowsInstall --> CreatingRegistryKeys
    CreatingRegistryKeys --> CreatingContextMenu
    CreatingContextMenu --> CreatingSendToShortcut
    CreatingSendToShortcut --> Installed

    LinuxInstall --> CreatingDesktopFile
    CreatingDesktopFile --> RegisteringMimeTypes
    RegisteringMimeTypes --> UpdatingDatabase
    UpdatingDatabase --> Installed

    MacOSInstall --> RegisteringLaunchServices
    RegisteringLaunchServices --> RegisteringUTI
    RegisteringUTI --> Installed

    Installed --> [*]

    Installed --> Uninstalling : User clicks Uninstall
    Uninstalling --> NotInstalled

    note right of WindowsInstall
        Registry Keys:
        - .md Extension
        - Context Menu
        - Send To Menu
    end note

    note right of LinuxInstall
        .desktop File:
        - ~/.local/share/applications/
        - xdg-mime registration
    end note

    note right of MacOSInstall
        Launch Services:
        - lsregister
        - duti for UTI
    end note
```

---

## 8. Migration Flow (Phasen)

```mermaid
gantt
    title MarkdownViewer Cross-Platform Migration Timeline
    dateFormat YYYY-MM-DD
    section Phase 1: Platform Abstractions
    Interfaces erstellen           :p1a, 2025-01-01, 3d
    Windows Implementierung         :p1b, after p1a, 5d
    Unit Tests                      :p1c, after p1b, 2d
    MainForm migrieren              :p1d, after p1c, 3d

    section Phase 2: Shared Library
    Core Projekt erstellen          :p2a, after p1d, 1d
    Business Logic verschieben      :p2b, after p2a, 3d
    Abhängigkeiten bereinigen       :p2c, after p2b, 2d

    section Phase 3: Linux/macOS
    Linux Services                  :p3a, after p2c, 5d
    macOS Services                  :p3b, after p3a, 5d

    section Phase 4: Avalonia UI
    Avalonia Projekt                :p4a, after p3b, 2d
    Views implementieren            :p4b, after p4a, 10d
    WebView Adapter                 :p4c, after p4b, 5d
    Theme System                    :p4d, after p4c, 3d

    section Phase 5: Release
    Integration Tests               :p5a, after p4d, 5d
    Build Pipeline                  :p5b, after p5a, 3d
    Deployment                      :p5c, after p5b, 2d
```

---

## 9. Data Flow: Theme-Wechsel

```mermaid
sequenceDiagram
    participant User
    participant StatusBar
    participant MainPresenter
    participant ThemeService
    participant SettingsService
    participant WebView
    participant Form

    User->>StatusBar: Wählt Theme "dark"
    StatusBar->>MainPresenter: ThemeChangeRequested("dark")
    MainPresenter->>ThemeService: LoadTheme("dark")
    ThemeService-->>MainPresenter: Theme Object

    MainPresenter->>SettingsService: Save(settings)
    SettingsService-->>MainPresenter: Success

    par Apply to WebView
        MainPresenter->>WebView: InjectCssAsync(darkCss)
        WebView-->>MainPresenter: Done (50ms)
    and Apply to UI
        MainPresenter->>Form: ApplyTheme(theme)
        Form-->>MainPresenter: Done (20ms)
    end

    MainPresenter-->>StatusBar: Theme applied
    StatusBar-->>User: Visual Update
```

---

## 10. Platform Detection & Service Registration

```mermaid
flowchart TD
    Start[Application Start] --> Detect{Detect OS}

    Detect -->|Windows| WinServices[Register Windows Services]
    Detect -->|Linux| LinServices[Register Linux Services]
    Detect -->|macOS| MacServices[Register macOS Services]

    WinServices --> WinImpl[WindowsFileAssociationService<br/>WindowsPlatformService<br/>WebView2Adapter<br/>WinFormsDialogService]

    LinServices --> LinImpl[LinuxFileAssociationService<br/>LinuxPlatformService<br/>AvaloniaWebViewAdapter<br/>AvaloniaDialogService]

    MacServices --> MacImpl[MacOSFileAssociationService<br/>MacOSPlatformService<br/>AvaloniaWebViewAdapter<br/>AvaloniaDialogService]

    WinImpl --> DI[Dependency Injection Container]
    LinImpl --> DI
    MacImpl --> DI

    DI --> Core[MarkdownViewer.Core Services]
    Core --> UI{UI Framework?}

    UI -->|WinForms| WinUI[WinForms MainForm]
    UI -->|Avalonia| AvUI[Avalonia MainWindow]

    WinUI --> Run[Application Run]
    AvUI --> Run

    style Start fill:#4A90E2,color:#fff
    style Detect fill:#FFB347,color:#000
    style DI fill:#50C878,color:#fff
    style Run fill:#4A90E2,color:#fff
```

---

## 11. Component Dependencies (Layer View)

```mermaid
graph TB
    subgraph "Layer 1: UI"
        WF[WinForms UI<br/>MainForm.cs]
        AV[Avalonia UI<br/>MainWindow.axaml]
    end

    subgraph "Layer 2: Presenters (MVP)"
        MP[MainPresenter]
        SP[StatusBarPresenter]
        NP[NavigationPresenter]
        SRP[SearchBarPresenter]
    end

    subgraph "Layer 3: Core Services"
        TS[ThemeService]
        SS[SettingsService]
        LS[LocalizationService]
        MR[MarkdownRenderer]
        NM[NavigationManager]
        SM[SearchManager]
    end

    subgraph "Layer 4: Platform Abstractions"
        IFA[IFileAssociationService]
        IPS[IPlatformService]
        IWV[IWebViewAdapter]
        IDS[IDialogService]
        ITS[IThemeService]
    end

    subgraph "Layer 5: Platform Implementations"
        WINSRV[Windows Services]
        LINSRV[Linux Services]
        MACSRV[macOS Services]
    end

    WF --> MP
    AV --> MP

    MP --> TS
    MP --> SS
    SP --> LS
    NP --> NM
    SRP --> SM

    TS --> ITS
    MR --> IWV
    NM --> IWV

    TS --> IPS
    SS --> IPS

    IFA --> WINSRV
    IFA --> LINSRV
    IFA --> MACSRV

    IPS --> WINSRV
    IPS --> LINSRV
    IPS --> MACSRV

    style WF fill:#FFB347,color:#000
    style AV fill:#FFB347,color:#000
    style MP fill:#A78BFA,color:#fff
    style TS fill:#4A90E2,color:#fff
    style IFA fill:#50C878,color:#fff
```

---

## 12. CSS Injection Performance Comparison

```mermaid
graph LR
    subgraph "ALT: Complete Reload"
        OldTheme[Theme Change] --> OldParse[Parse Markdown]
        OldParse --> OldRender[Render HTML]
        OldRender --> OldNav[Navigate WebView]
        OldNav --> OldResult[Result: 500ms]
    end

    subgraph "NEU: CSS Injection"
        NewTheme[Theme Change] --> NewCSS[Generate CSS]
        NewCSS --> NewInject[InjectCssAsync]
        NewInject --> NewResult[Result: 50ms]
    end

    style OldResult fill:#FF6B6B,color:#fff
    style NewResult fill:#50C878,color:#fff
```

---

## 13. Cross-Platform File Paths

```mermaid
graph TB
    subgraph "IPlatformService.GetApplicationDataFolder()"
        Question{OS?}

        Question -->|Windows| WinPath["C:\Users\Username\AppData\Roaming\MarkdownViewer"]
        Question -->|Linux| LinPath["~/.config/MarkdownViewer<br/>(XDG_CONFIG_HOME)"]
        Question -->|macOS| MacPath["~/Library/Application Support/MarkdownViewer"]
    end

    subgraph "IPlatformService.GetCacheFolder()"
        CacheQuestion{OS?}

        CacheQuestion -->|Windows| WinCache["C:\Users\Username\AppData\Local\MarkdownViewer\Cache"]
        CacheQuestion -->|Linux| LinCache["~/.cache/MarkdownViewer<br/>(XDG_CACHE_HOME)"]
        CacheQuestion -->|macOS| MacCache["~/Library/Caches/MarkdownViewer"]
    end

    style WinPath fill:#4A90E2,color:#fff
    style LinPath fill:#FFA500,color:#000
    style MacPath fill:#FF6B6B,color:#fff
    style WinCache fill:#4A90E2,color:#fff
    style LinCache fill:#FFA500,color:#000
    style MacCache fill:#FF6B6B,color:#fff
```

---

## 14. Testing Strategy

```mermaid
graph TB
    subgraph "Unit Tests"
        UT1[Platform Services Tests]
        UT2[WebView Adapter Tests]
        UT3[Dialog Service Tests]
        UT4[Theme Service Tests]
    end

    subgraph "Integration Tests"
        IT1[WinForms End-to-End]
        IT2[Avalonia End-to-End]
        IT3[File Association Tests]
        IT4[Theme Switch Tests]
    end

    subgraph "Platform Tests"
        PT1[Windows-specific Tests]
        PT2[Linux-specific Tests]
        PT3[macOS-specific Tests]
    end

    subgraph "Performance Tests"
        PF1[Theme Switch Benchmark]
        PF2[Markdown Render Benchmark]
        PF3[Startup Time Benchmark]
    end

    UT1 --> IT1
    UT2 --> IT1
    UT3 --> IT1
    UT4 --> IT1

    IT1 --> PT1
    IT2 --> PT2
    IT2 --> PT3

    PT1 --> PF1
    PT2 --> PF1
    PT3 --> PF1

    style UT1 fill:#4A90E2,color:#fff
    style IT1 fill:#50C878,color:#fff
    style PT1 fill:#FFB347,color:#000
    style PF1 fill:#A78BFA,color:#fff
```

---

## Legende

### Farbcodes
- **Blau** (#4A90E2): Core Services / Business Logic
- **Grün** (#50C878): Interfaces / Abstractions
- **Orange** (#FFB347): UI Layer / Platform-specific
- **Violett** (#A78BFA): Presenters / MVP Pattern
- **Rot** (#FF6B6B): Performance-kritische Bereiche
- **Gelb** (#FFD700): Configuration / Data

### Diagramm-Typen
- **Graph TB/LR**: Architektur-Übersichten
- **Sequence Diagram**: Ablauf-Diagramme
- **Class Diagram**: Interface-Definitionen
- **State Diagram**: Zustandsautomaten
- **Gantt Chart**: Timeline / Planung
- **Flowchart**: Entscheidungslogik

---

**Ende der Architektur-Diagramme**
