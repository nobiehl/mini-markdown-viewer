# Process Model - MarkdownViewer Development

Dieses Dokument beschreibt das Vorgehensmodell für die strukturierte Weiterentwicklung von MarkdownViewer ab v1.2.0.

## Zielsetzung

Saubere, testbare, wartbare Architektur mit klarer Dokumentation und nachvollziehbarem Fortschritt.

## Phasen

### Phase 1: Planung & Dokumentation
**REGEL:** Erst planen, dann implementieren!

#### 1.1 Anforderungsanalyse
- Features sammeln und priorisieren
- Technische Herausforderungen identifizieren
- Architekturentscheidungen treffen

#### 1.2 Roadmap erstellen
- Detaillierte Beschreibung aller Features
- Aufwandsschätzung pro Feature
- Abhängigkeiten dokumentieren
- In `ROADMAP.md` festhalten

#### 1.3 Architektur dokumentieren
- Neue Klassen/Services definieren
- Ordnerstruktur planen
- Interfaces definieren
- Dependencies klären
- In `ARCHITECTURE.md` festhalten

### Phase 2: Implementierung
**REGEL:** Nach jedem Abschnitt dokumentieren!

#### 2.1 Vor der Implementierung
- Aktuellen Abschnitt aus ROADMAP.md lesen
- Verstehen was zu tun ist
- Testfälle überlegen

#### 2.2 Während der Implementierung
- Code schreiben
- Unit Tests schreiben (parallel!)
- Refactoring durchführen
- Build erfolgreich durchführen

#### 2.3 Nach der Implementierung
- **SOFORT:** Progress in `impl_progress.md` festhalten (via printf)
  ```bash
  printf "\n## [$(date +%Y-%m-%d)] Session X - Feature Y\n\n**Status:** ✅ Completed\n\n**Changes:**\n- File1.cs: Added class X\n- Test1.cs: Added tests for X\n\n**Metrics:**\n- Lines added: XXX\n- Tests added: XX\n- Test coverage: XX%%\n\n**Next:**\n- [ ] Feature Z\n\n---\n" >> impl_progress.md
  ```

- Glossar-Einträge für neue Begriffe hinzufügen (via printf)
  ```bash
  printf "\n### ThemeManager\nManages application themes (Dark, Light, Solarized, Dräger). Applies theme to both Markdown rendering and WinForms UI.\n\n**File:** Services/ThemeManager.cs\n**Used by:** MainWindow, StatusBarManager\n\n" >> GLOSSARY.md
  ```

- Relevante Dokumentation aktualisieren (ARCHITECTURE.md, DEVELOPMENT.md)
- Git Commit mit aussagekräftiger Message

### Phase 3: Testing & Validation

#### 3.1 Unit Tests
- Alle neuen Services haben Tests
- Test Coverage >= 80%
- Tests laufen erfolgreich

#### 3.2 Integration Tests
- End-to-End Szenarien testen
- Manual Testing durchführen
- Edge Cases prüfen

#### 3.3 Dokumentation prüfen
- Alle neuen Features dokumentiert?
- Code-Kommentare vorhanden?
- GLOSSARY.md vollständig?

### Phase 4: Konsolidierung

#### 4.1 Glossar konsolidieren
- Duplikate entfernen
- Alphabetisch sortieren
- Cross-References hinzufügen
- Mit Dokumentation abgleichen

#### 4.2 Dokumentation synchronisieren
- ARCHITECTURE.md mit aktuellem Code abgleichen
- DEVELOPMENT.md aktualisieren
- README.md bei Bedarf anpassen
- DEPLOYMENT-GUIDE.md prüfen

#### 4.3 Release vorbereiten
- Version bump
- CHANGELOG.md erstellen
- Release Notes schreiben
- Build testen

## Dokumentationsstruktur

```
mini-markdown-viewer/
├── PROCESS-MODEL.md          # Dieses Dokument
├── ROADMAP.md                # Detaillierter Fahrplan
├── ARCHITECTURE.md           # Architektur-Übersicht
├── impl_progress.md          # Implementierungs-Fortschritt (chronologisch)
├── GLOSSARY.md               # Begriffe & Definitionen
├── DEVELOPMENT.md            # Developer Docs
├── DEPLOYMENT-GUIDE.md       # Deployment Process
└── README.md                 # User Docs
```

## Workflow pro Feature

```mermaid
graph TD
    A[Feature identifizieren] --> B[In ROADMAP.md eintragen]
    B --> C[Architektur planen]
    C --> D[In ARCHITECTURE.md dokumentieren]
    D --> E[Implementation starten]
    E --> F[Tests schreiben]
    F --> G[Build & Test]
    G --> H{Tests OK?}
    H -->|Nein| E
    H -->|Ja| I[Progress in impl_progress.md]
    I --> J[Glossar-Einträge hinzufügen]
    J --> K[Doku aktualisieren]
    K --> L[Git Commit]
    L --> M{Mehr Features?}
    M -->|Ja| A
    M -->|Nein| N[Glossar konsolidieren]
    N --> O[Doku synchronisieren]
    O --> P[Release]
```

## Best Practices

### Documentation First
- **NIEMALS** Code schreiben ohne vorher zu dokumentieren was gemacht wird
- Roadmap und Architektur MÜSSEN vor Implementation stehen
- Bei Unklarheiten: Erst diskutieren, dann dokumentieren, dann implementieren

### Incremental Progress
- Kleine, abgeschlossene Schritte
- Nach jedem Schritt: Commit
- Jeder Commit ist lauffähig
- Keine "WIP" Commits ohne Tests

### Test-Driven Documentation
- Für jeden neuen Service: Interface dokumentieren
- Für jede neue Klasse: Zweck dokumentieren
- Für jedes neue Feature: User-Perspektive dokumentieren

### Glossary Discipline
- Neuer Begriff → Sofort ins Glossar
- Akronym → Sofort ins Glossar
- Service-Name → Sofort ins Glossar
- Nicht sammeln, sondern sofort eintragen (via printf)

## Quality Gates

### Vor jedem Commit:
- [ ] Code kompiliert ohne Fehler
- [ ] Alle Tests laufen durch
- [ ] impl_progress.md aktualisiert
- [ ] Neue Begriffe im Glossar
- [ ] Relevante Doku angepasst

### Vor jedem Release:
- [ ] Test Coverage >= 80%
- [ ] Alle Features dokumentiert
- [ ] Glossar konsolidiert
- [ ] Doku synchronisiert
- [ ] Manual Testing durchgeführt
- [ ] CHANGELOG.md vollständig

## Tools & Commands

### Progress festhalten:
```bash
printf "\n## [$(date +%Y-%m-%d)] Session X - Feature Y\n\n**Status:** ✅ Completed\n\n" >> impl_progress.md
```

### Glossar-Eintrag hinzufügen:
```bash
printf "\n### TermName\nDefinition here.\n\n**File:** path/to/file.cs\n\n" >> GLOSSARY.md
```

### Test Coverage prüfen:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=lcov
```

## Lessons Learned

### Was funktioniert:
- ✅ Kleine, dokumentierte Schritte
- ✅ Tests parallel zum Code
- ✅ Sofortiges Dokumentieren (nicht aufschieben)
- ✅ Glossar via printf (schnell und einfach)

### Was vermeiden:
- ❌ "Ich dokumentiere später" (wird vergessen)
- ❌ Große Refactorings ohne Tests
- ❌ Code schreiben ohne Plan
- ❌ Glossar am Ende zusammenstellen (zu aufwändig)

## Nächste Schritte

Nach diesem Dokument:
1. ✅ ROADMAP.md erstellen (detailliert!)
2. ✅ ARCHITECTURE.md erstellen
3. ✅ impl_progress.md initialisieren
4. ✅ GLOSSARY.md initialisieren
5. ⏳ Mit Implementation beginnen

---

**Version:** 1.0
**Erstellt:** 2025-11-05
**Status:** Active
