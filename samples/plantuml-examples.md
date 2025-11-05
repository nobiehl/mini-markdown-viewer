# PlantUML Diagram Examples

This document demonstrates various PlantUML diagram types supported by MarkdownViewer.

[← Back to Index](index.md)

## Class Diagram

```plantuml
@startuml
class Vehicle {
    -String brand
    -String model
    -int year
    +start()
    +stop()
}

class Car {
    -int doors
    +openTrunk()
}

class Motorcycle {
    -boolean hasSidecar
    +wheelie()
}

Vehicle <|-- Car
Vehicle <|-- Motorcycle
@enduml
```

## Sequence Diagram

```plantuml
@startuml
actor User
participant "Web Browser" as Browser
participant "Web Server" as Server
database "Database" as DB

User -> Browser: Enter URL
Browser -> Server: HTTP Request
Server -> DB: Query Data
DB --> Server: Return Data
Server --> Browser: HTTP Response
Browser --> User: Display Page
@enduml
```

## Use Case Diagram

```plantuml
@startuml
left to right direction
actor User
actor Admin

rectangle "MarkdownViewer System" {
    usecase "Open File" as UC1
    usecase "View Markdown" as UC2
    usecase "Render Diagrams" as UC3
    usecase "Install Integration" as UC4

    User --> UC1
    User --> UC2
    User --> UC3
    Admin --> UC4
}
@enduml
```

## Activity Diagram

```plantuml
@startuml
start
:Open Markdown File;
if (File Exists?) then (yes)
  :Read File Content;
  :Convert to HTML;
  if (Contains Diagrams?) then (yes)
    :Render Diagrams;
  else (no)
  endif
  :Display in WebView2;
else (no)
  :Show Error;
endif
stop
@enduml
```

## Component Diagram

```plantuml
@startuml
package "MarkdownViewer" {
    [Program.cs] --> [MainForm.cs]
    [MainForm.cs] --> [WebView2]
    [MainForm.cs] --> [Markdig]
    [MainForm.cs] --> [FileSystemWatcher]
}

cloud "External Services" {
    [Mermaid CDN]
    [PlantUML Server]
}

[WebView2] --> [Mermaid CDN]
[WebView2] --> [PlantUML Server]
@enduml
```

## State Diagram

```plantuml
@startuml
[*] --> Idle

Idle --> Opening: Open File
Opening --> Loading: File Selected
Loading --> Rendering: Content Loaded
Rendering --> Displayed: Render Complete

Displayed --> Reloading: File Changed
Reloading --> Rendering: Reload Complete

Displayed --> Closing: Close File
Closing --> Idle: Cleanup Complete

Displayed --> [*]
@enduml
```

## Deployment Diagram

```plantuml
@startuml
node "User's Computer" {
    component "MarkdownViewer.exe" as App
    component "WebView2 Runtime" as WebView
    component ".NET 8 Runtime" as DotNet

    App --> WebView
    App --> DotNet
}

cloud "Internet" {
    component "PlantUML Server" as PlantUML
    component "CDN (Mermaid, Highlight.js)" as CDN
}

WebView --> PlantUML
WebView --> CDN
@enduml
```

## Object Diagram

```plantuml
@startuml
object "MarkdownViewer : MainForm" as viewer {
    currentFilePath = "index.md"
    webView = WebView2
}

object "webView : WebView2" as wv {
    width = 1024
    height = 768
}

object "fileWatcher : FileSystemWatcher" as fw {
    path = "C:/samples/"
    filter = "*.md"
}

viewer --> wv
viewer --> fw
@enduml
```

---

**Navigation:**
- [← Previous: Mermaid Examples](mermaid-examples.md)
- [← Back to Index](index.md)
- [Next: Code Examples →](code-examples.md)
