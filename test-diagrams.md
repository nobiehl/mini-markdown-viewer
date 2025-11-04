# Diagram Test für MarkdownViewer

Dieser Markdown-Test zeigt Mermaid und PlantUML Diagramme.

## Mermaid Flowchart

```mermaid
graph TD
    A[Start] --> B{Is it working?}
    B -->|Yes| C[Great!]
    B -->|No| D[Debug]
    D --> A
    C --> E[Done]
```

## Mermaid Sequence Diagram

```mermaid
sequenceDiagram
    participant User
    participant App
    participant WebView2

    User->>App: Open .md file
    App->>WebView2: Load HTML
    WebView2->>WebView2: Render Mermaid
    WebView2->>User: Display diagram
```

## PlantUML Class Diagram

```plantuml
@startuml
class MainForm {
    -WebView2 webView
    -string currentFilePath
    -FileSystemWatcher fileWatcher
    +MainForm(filePath)
    +LoadMarkdownFile(filePath)
    -ConvertMarkdownToHtml(markdown)
}

class Program {
    +Main(args)
    -InstallFileAssociation()
    -UninstallFileAssociation()
}

MainForm --> Program : uses
@enduml
```

## PlantUML Sequence Diagram

```plantuml
@startuml
User -> MarkdownViewer: Open .md file
MarkdownViewer -> Markdig: Convert to HTML
Markdig --> MarkdownViewer: HTML string
MarkdownViewer -> WebView2: NavigateToString()
WebView2 -> Mermaid: Render diagrams
WebView2 -> PlantUML_Server: Fetch PlantUML images
PlantUML_Server --> WebView2: SVG images
WebView2 --> User: Display rendered content
@enduml
```

## Code mit Syntax Highlighting

```csharp
public class Example
{
    public void HelloWorld()
    {
        Console.WriteLine("Hello, World!");
    }
}
```

**Ende des Tests**
