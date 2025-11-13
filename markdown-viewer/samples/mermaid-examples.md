# Mermaid Diagram Examples

This document demonstrates various Mermaid diagram types supported by MarkdownViewer.

[← Back to Index](index.md)

## Flowchart

```mermaid
graph TD
    A[Start] --> B{Decision}
    B -->|Yes| C[Process A]
    B -->|No| D[Process B]
    C --> E[End]
    D --> E
```

## Sequence Diagram

```mermaid
sequenceDiagram
    participant User
    participant App
    participant Server

    User->>App: Open file
    App->>Server: Request data
    Server-->>App: Return data
    App-->>User: Display content
```

## Class Diagram

```mermaid
classDiagram
    class Animal {
        +String name
        +int age
        +makeSound()
    }
    class Dog {
        +String breed
        +bark()
    }
    class Cat {
        +String color
        +meow()
    }
    Animal <|-- Dog
    Animal <|-- Cat
```

## State Diagram

```mermaid
stateDiagram-v2
    [*] --> Idle
    Idle --> Loading: Open File
    Loading --> Rendering: File Loaded
    Rendering --> Displayed: Render Complete
    Displayed --> Idle: Close File
    Displayed --> [*]
```

## Gantt Chart

```mermaid
gantt
    title Project Timeline
    dateFormat  YYYY-MM-DD
    section Phase 1
    Planning           :a1, 2024-01-01, 30d
    Development        :a2, after a1, 60d
    section Phase 2
    Testing            :a3, after a2, 20d
    Deployment         :a4, after a3, 10d
```

## Pie Chart

```mermaid
pie title Programming Languages Used
    "C#" : 40
    "JavaScript" : 25
    "Python" : 20
    "TypeScript" : 15
```

## Git Graph

```mermaid
gitGraph
    commit
    commit
    branch develop
    checkout develop
    commit
    commit
    checkout main
    merge develop
    commit
```

---

**Navigation:**
- [← Back to Index](index.md)
- [Next: PlantUML Examples →](plantuml-examples.md)
