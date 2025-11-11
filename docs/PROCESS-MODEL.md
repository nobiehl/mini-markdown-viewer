# Process Model - MarkdownViewer Development

Dieses Dokument beschreibt das Vorgehensmodell für die strukturierte Weiterentwicklung von MarkdownViewer ab v1.2.0.

## Zielsetzung

Saubere, testbare, wartbare Architektur mit klarer Dokumentation und nachvollziehbarem Fortschritt.

### Neu: Parallele Implementierung mit Agenten

Ab v1.7.0 nutzen wir **parallele Agenten** für schnellere Feature-Entwicklung:
- ✅ **3x schnellere Implementierung** bei unabhängigen Tasks
- ✅ **Keine Merge-Konflikte** durch klare Aufgabentrennung
- ✅ **Bessere Code-Qualität** durch fokussierte Agent-Aufgaben

**Kernprinzip:** Features werden in unabhängige Teilaufgaben zerlegt, die parallel von mehreren Agenten implementiert werden. Jeder Agent arbeitet an eigenen Dateien mit eigenen Tests.

**Voraussetzung:** Saubere Planung und Dokumentation BEVOR Agenten gestartet werden (siehe Phase 1.4).

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

#### 1.4 Parallelisierungsanalyse & Agent-Planung
**REGEL:** Bevor du implementierst, überlege ob parallele Agenten die Arbeit beschleunigen können!

##### 1.4.1 Feature-Analyse
1. **Feature in Teilaufgaben zerlegen**
   - Welche logischen Komponenten gibt es?
   - Welche Dateien/Klassen müssen angelegt/geändert werden?
   - Welche Tests sind nötig?

2. **Abhängigkeiten identifizieren**
   - Welche Tasks sind **unabhängig** voneinander?
   - Welche Tasks **bauen aufeinander auf**?
   - Welche Tasks teilen **gemeinsame Ressourcen**?

3. **Parallelisierbarkeit bewerten**
   - Können Tasks gleichzeitig bearbeitet werden?
   - Gibt es Merge-Konflikte?
   - Ist die Koordination aufwändiger als sequentielle Arbeit?

##### 1.4.2 Agent-Aufgaben definieren
Wenn Parallelisierung sinnvoll ist, definiere für jeden Agent:

```markdown
## Agent 1: [Kurze Beschreibung]
**Ziel:** [Was soll erreicht werden?]
**Dateien:** [Welche Dateien werden erstellt/geändert?]
**Tests:** [Welche Tests sollen geschrieben werden?]
**Keine Abhängigkeiten zu:** Agent 2, Agent 3
**Geschätzte Dauer:** [X Minuten]

## Agent 2: [Kurze Beschreibung]
...
```

##### 1.4.3 Kriterien für Parallelisierung

**✅ Parallelisierung ist sinnvoll wenn:**
- 3+ unabhängige Teilaufgaben existieren
- Jede Teilaufgabe > 5 Minuten Arbeit benötigt
- Keine oder minimale Code-Überschneidungen
- Klare Schnittstellen zwischen Komponenten
- Tasks sind gut dokumentiert und verständlich

**❌ Parallelisierung vermeiden wenn:**
- Tasks stark voneinander abhängen
- Gemeinsame Dateien intensiv bearbeitet werden
- Aufgabe zu klein (< 15 Minuten gesamt)
- Koordinationsaufwand > Zeitersparnis
- Unklare Anforderungen oder Design

##### 1.4.4 Beispiele aus der Praxis

**Gutes Beispiel: MarkdownDialog Feature (v1.7.1)**
```
Agent 1: MarkdownDialog erstellen
- UI/MarkdownDialog.cs implementieren
- WebView2 mit Scrollbar
- Tests: MarkdownDialogTests.cs

Agent 2: StatusBar Info Handler
- MainForm.cs: OnInfoClicked implementieren
- BuildApplicationInfoMarkdown() erstellen
- Tests: MainFormTests.cs (Info-Button)

Agent 3: UI Text Cleanup
- StatusBarControl.cs: Tooltips bereinigen
- Language/Theme ComboBox cleanup
- AutomationId properties hinzufügen
- Tests: StatusBarControlTests.cs
```
**Ergebnis:** 3x schnellere Implementierung, keine Merge-Konflikte

**Gutes Beispiel: Chart.js Integration (v1.7.3)**
```
Agent 1: Chart.js in MarkdownRenderer
- MarkdownRenderer.cs: CDN + Rendering-Logik
- Tests: MarkdownRendererTests.cs

Agent 2: Sample Files erstellen
- samples/charts-overview.md
- samples/charts-business.md
- samples/charts-data-science.md
- samples/charts-realtime.md

Agent 3: Dokumentation & Tests
- README.md updaten
- UI-Tests für Chart-Rendering
```
**Ergebnis:** Parallele Entwicklung möglich, Agent 2 + 3 komplett unabhängig von Agent 1

**Schlechtes Beispiel: Theme Refactoring (hypothetisch)**
```
❌ NICHT parallelisieren:
Agent 1: ThemeService erstellen
Agent 2: MainForm auf ThemeService umstellen
Agent 3: StatusBar auf ThemeService umstellen
```
**Problem:** Agent 2 + 3 benötigen die Interfaces von Agent 1 → Sequentiell arbeiten!

### Phase 2: Implementierung
**REGEL:** Nach jedem Abschnitt dokumentieren!

#### 2.0 Implementierungsmodus wählen

**Sequentielle Implementierung:** Ein Task nach dem anderen
- Verwende für kleine Features (< 15 Min)
- Verwende bei starken Abhängigkeiten
- Verwende bei unkomplexen Änderungen

**Parallele Implementierung:** Mehrere Agenten gleichzeitig
- Verwende für große Features (> 30 Min)
- Verwende bei unabhängigen Teilaufgaben
- **WICHTIG:** Erstelle ZUERST einen Implementierungsplan (siehe Phase 1.4)

#### 2.1 Vor der Implementierung

**Sequentielle Implementierung:**
- Aktuellen Abschnitt aus ROADMAP.md lesen
- Verstehen was zu tun ist
- Testfälle überlegen

**Parallele Implementierung:**
1. **Implementierungsplan schreiben:**
   ```markdown
   # Implementierungsplan: [Feature Name]

   ## Agent 1: [Name]
   - [ ] Task 1
   - [ ] Task 2
   - [ ] Tests schreiben

   ## Agent 2: [Name]
   - [ ] Task 1
   - [ ] Task 2
   - [ ] Tests schreiben

   ## Agent 3: [Name]
   - [ ] Task 1
   - [ ] Task 2
   - [ ] Tests schreiben

   ## Integration (sequentiell nach Agenten)
   - [ ] Merge durchführen
   - [ ] Integration Tests
   - [ ] Build & Test
   ```

2. **Agenten starten mit Task Tool:**
   ```
   - Verwende EINEN Message-Block mit MEHREREN Task-Tool-Aufrufen
   - Jeder Agent bekommt seinen spezifischen Auftrag aus dem Plan
   - Agenten arbeiten parallel und unabhängig
   ```

3. **Nach Agent-Completion:**
   - Alle Agent-Ergebnisse reviewen
   - Merge-Konflikte auflösen (falls vorhanden)
   - Integration Tests durchführen
   - Build & Test

#### 2.2 Während der Implementierung
- Code schreiben
- Unit Tests schreiben (parallel!)
- Refactoring durchführen
- Build erfolgreich durchführen
- **Bei parallelen Agenten:** Regelmäßig Fortschritt prüfen

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
- Tests laufen erfolgreich: `dotnet test`

##### 3.1.1 Link Navigation Tests (LinkNavigationHelperTests.cs - 30 Tests)

**ResolveRelativePath Tests (8 Tests):**
- [ ] ResolveRelativePath_WithAbsolutePath_ReturnsNormalizedPath
- [ ] ResolveRelativePath_WithRelativePath_ResolvesRelativeToCurrentFileDirectory
- [ ] ResolveRelativePath_WithRelativePathGoingUp_ResolvesCorrectly
- [ ] ResolveRelativePath_WithDeepRelativePath_ResolvesCorrectly
- [ ] ResolveRelativePath_WithNullLinkPath_ThrowsArgumentException
- [ ] ResolveRelativePath_WithEmptyLinkPath_ThrowsArgumentException
- [ ] ResolveRelativePath_WithNullCurrentFile_ThrowsArgumentException
- [ ] Integration_ResolveAndValidateLocalMarkdownLink_Success

**GetLinkType Tests (8 Tests):**
- [ ] GetLinkType_WithHttpUrl_ReturnsExternalHttp
- [ ] GetLinkType_WithHttpsUrl_ReturnsExternalHttp
- [ ] GetLinkType_WithAnchorLink_ReturnsAnchor
- [ ] GetLinkType_WithMarkdownFile_ReturnsLocalMarkdown
- [ ] GetLinkType_WithMarkdownExtension_ReturnsLocalMarkdown
- [ ] GetLinkType_WithUnknownLink_ReturnsUnknown
- [ ] GetLinkType_WithNullLink_ReturnsUnknown
- [ ] GetLinkType_WithEmptyLink_ReturnsUnknown

**ValidateFileExists Tests (4 Tests):**
- [ ] ValidateFileExists_WithExistingFile_ReturnsTrue
- [ ] ValidateFileExists_WithNonExistentFile_ReturnsFalse
- [ ] ValidateFileExists_WithNullPath_ReturnsFalse
- [ ] ValidateFileExists_WithEmptyPath_ReturnsFalse

**IsInlineResource Tests (8 Tests):**
- [ ] IsInlineResource_WithPlantUmlUrl_ReturnsTrue
- [ ] IsInlineResource_WithCdnUrl_ReturnsTrue
- [ ] IsInlineResource_WithImageUrl_ReturnsTrue
- [ ] IsInlineResource_WithJpgUrl_ReturnsTrue
- [ ] IsInlineResource_WithSvgUrl_ReturnsTrue
- [ ] IsInlineResource_WithRegularWebPage_ReturnsFalse
- [ ] IsInlineResource_WithNullUrl_ReturnsFalse
- [ ] IsInlineResource_WithEmptyUrl_ReturnsFalse

**Integration Tests (2 Tests):**
- [ ] Integration_ResolveAndValidateLocalMarkdownLink_Success
- [ ] Integration_ResolveAndValidateMissingFile_Fails

**Kommando zum Ausführen:**
```bash
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --verbosity normal
```

#### 3.2 Integration Tests

##### 3.2.1 Link Navigation Integration Tests
**Manual Testing durchführen mit test-links.md:**

**External Links:**
- [ ] HTTP Link (http://www.google.com) öffnet im Browser
- [ ] HTTPS Link (https://github.com) öffnet im Browser
- [ ] PlantUML inline resources werden NICHT im Browser geöffnet
- [ ] CDN resources (jsdelivr, cloudflare) werden NICHT im Browser geöffnet

**Local File Links:**
- [ ] Relative Link zu existierender Datei (test.md) funktioniert
- [ ] Relative Link mit Verzeichnis (docs/file.md) funktioniert
- [ ] Relative Link nach oben (../README.md) funktioniert
- [ ] Absolute Pfad Link funktioniert
- [ ] Link zu nicht-existierender Datei wird geloggt (KEIN MessageBox!)
- [ ] FileWatcher crasht NICHT bei relativen Pfaden

**Anchor Links:**
- [ ] Anchor Link (#external-links) scrollt zur Section
- [ ] Anchor Link (#local-links) scrollt zur Section
- [ ] Anchor Link zu nicht-existierender Section scrollt nicht (aber crasht nicht)

**Logging:**
- [ ] Alle Link-Klicks werden geloggt (Link-Typ, Quelle, Ziel)
- [ ] Pfad-Auflösung wird geloggt (relativ → absolut)
- [ ] File-Existenz-Checks werden geloggt
- [ ] Fehlgeschlagene Navigationen werden geloggt
- [ ] KEINE MessageBox-Dialoge bei fehlenden Dateien!

**Test-Kommando:**
```bash
# Starte App mit test-links.md
cd markdown-viewer/MarkdownViewer/bin/Release/net8.0-windows
./MarkdownViewer.exe "C:\develop\workspace\misc\test-links.md"

# Prüfe Logs
cat logs/viewer-YYYYMMDD.log | grep "Link type:"
cat logs/viewer-YYYYMMDD.log | grep "Path resolution:"
cat logs/viewer-YYYYMMDD.log | grep "File not found:"
```

#### 3.3 Theme Tests
- [ ] Theme wird beim Start korrekt angewendet (kein "Mischmasch")
- [ ] Statusbar hat korrekte Theme-Farben beim Start
- [ ] Icons sind sichtbar auf allen Themes (dark, light, solarized, draeger)
- [ ] Theme-Wechsel zur Laufzeit funktioniert
- [ ] Icons werden bei Theme-Wechsel regeneriert
- [ ] Settings werden gespeichert

#### 3.4 File Watching Tests
- [ ] File Watcher funktioniert mit absoluten Pfaden
- [ ] File Watcher funktioniert mit relativen Pfaden (nach Auflösung!)
- [ ] File Watcher crasht NICHT bei fehlenden Verzeichnissen
- [ ] Änderungen an Datei triggern Reload
- [ ] File Watcher wird korrekt disposed

#### 3.5 Dokumentation prüfen
- [ ] Alle neuen Features dokumentiert?
- [ ] Code-Kommentare vorhanden?
- [ ] GLOSSARY.md vollständig?
- [ ] impl_progress.md aktualisiert?

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

**KRITISCH: Alle Tests MÜSSEN vor Release ausgeführt werden!**

**1. Unit Tests ausführen:**
```bash
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --verbosity normal
```
- Alle Tests müssen grün sein (0 failed)
- Test Coverage >= 80%
- Bei Fehlern: Release NICHT erstellen!

**2. UI Automation Tests ausführen:**
```bash
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --filter "FullyQualifiedName~UIAutomation" --verbosity normal
```
- **WICHTIG**: UI Tests verifizieren dass die Anwendung tatsächlich startet
- Würde Fehler wie "138 KB broken binary" sofort erkennen
- Bei Fehlern: Release NICHT erstellen!

**3. Publish Build erstellen und testen:**
```bash
cd markdown-viewer/MarkdownViewer
dotnet publish -c Release -r win-x64 --self-contained false \
  -p:PublishSingleFile=true \
  -p:IncludeNativeLibrariesForSelfExtract=true -o publish

# WICHTIG: Verifiziere Dateigröße (sollte ~3.3 MB sein, NICHT 138 KB!)
ls -lh publish/MarkdownViewer.exe
```
- **Dateigröße prüfen**: ~3.3 MB ist korrekt, 138 KB ist FALSCH (Managed DLL statt EXE)
- **Binary manuell testen**: Doppelklick, öffnet Datei, keine Crashes, UI funktioniert

**4. KRITISCH: Dokumentation aktualisieren (VOR Release!):**

**4.1 Version Bump:**
- `Program.cs`: `private const string Version = "X.Y.Z";`
- `MainForm.cs`: `private const string Version = "X.Y.Z";`
- `README.md`: Version Badge aktualisieren

**4.2 CHANGELOG.md aktualisieren (PFLICHT!):**
```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- Feature 1 mit Beschreibung
- Feature 2 mit Beschreibung

### Fixed
- Bug 1 mit Beschreibung

### Changed
- Änderung 1 mit Beschreibung

### Technical
- Build info, Tests, etc.
```
**WICHTIG:**
- CHANGELOG.md ist die **einzige Quelle der Wahrheit** für Release-Informationen
- **KEINE separaten RELEASE-NOTES-vX.Y.Z.md Dateien mehr erstellen!**
- Alle Release-Informationen gehen in CHANGELOG.md
- GitHub Release Notes können aus CHANGELOG.md kopiert werden

**4.3 README.md aktualisieren:**
- Version Badge (nur Nummer, keine Features!)
- Binary Size Badge falls geändert
- Test Count Badge
- Quick Start Section: Download-Link auf neue Version
- Installation Commands: `MarkdownViewer-vX.Y.Z.exe` statt generisch

**4.4 impl_progress.md aktualisieren:**
```bash
printf "\n## [$(date +%Y-%m-%d)] Session - Feature Name\n\n**Status:** ✅ Completed\n\n..." >> docs/impl_progress.md
```

**5. Erst nach erfolgreichen Tests UND Dokumentation: Release erstellen**

## Dokumentationsstruktur

```
mini-markdown-viewer/
├── README.md                 # User Docs (Features, Quick Start, Installation)
├── docs/
│   ├── CHANGELOG.md          # ⭐ SINGLE SOURCE OF TRUTH für alle Releases!
│   ├── PROCESS-MODEL.md      # Dieses Dokument - Entwicklungsprozess
│   ├── ROADMAP.md            # Detaillierter Fahrplan
│   ├── ARCHITECTURE.md       # Architektur-Übersicht
│   ├── impl_progress.md      # Implementierungs-Fortschritt (chronologisch)
│   ├── GLOSSARY.md           # Begriffe & Definitionen
│   ├── DEVELOPMENT.md        # Developer Docs
│   ├── DEPLOYMENT-GUIDE.md   # Deployment Process
│   └── USER-GUIDE.md         # Ausführliche Benutzer-Dokumentation
```

**WICHTIG:**
- **CHANGELOG.md** ist die einzige Quelle für Release-Informationen
- **KEINE RELEASE-NOTES-vX.Y.Z.md Dateien** mehr erstellen!
- GitHub Release Notes werden aus CHANGELOG.md kopiert
- README.md enthält nur Quickstart und Feature-Übersicht, keine detaillierten Release Notes

## Workflow pro Feature

### Sequentielle Implementierung

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

### Parallele Implementierung mit Agenten

```mermaid
graph TD
    A[Feature identifizieren] --> B[In ROADMAP.md eintragen]
    B --> C[Architektur planen]
    C --> D[In ARCHITECTURE.md dokumentieren]
    D --> E[Parallelisierungsanalyse]
    E --> F{Parallelisierung sinnvoll?}
    F -->|Nein| G[Sequentiell arbeiten]
    F -->|Ja| H[Implementierungsplan erstellen]
    H --> I[Aufgaben definieren Agent 1-N]
    I --> J[Agenten parallel starten]
    J --> K1[Agent 1: Code + Tests]
    J --> K2[Agent 2: Code + Tests]
    J --> K3[Agent 3: Code + Tests]
    K1 --> L[Agent-Ergebnisse reviewen]
    K2 --> L
    K3 --> L
    L --> M[Merge & Integration]
    M --> N[Integration Tests]
    N --> O[Build & Test]
    O --> P{Tests OK?}
    P -->|Nein| Q[Fehler beheben]
    Q --> O
    P -->|Ja| R[Progress in impl_progress.md]
    R --> S[Glossar-Einträge hinzufügen]
    S --> T[Doku aktualisieren]
    T --> U[Git Commit]
    U --> V{Mehr Features?}
    V -->|Ja| A
    V -->|Nein| W[Glossar konsolidieren]
    W --> X[Doku synchronisieren]
    X --> Y[Release]
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

### Parallel Implementation
- **Plane BEVOR du parallelisierst:** Erstelle Implementierungsplan mit klaren Agent-Aufgaben
- **Unabhängigkeit ist King:** Agenten sollten KEINE gemeinsamen Dateien bearbeiten
- **Ein Message = Alle Agenten:** Starte alle Agenten gleichzeitig in einem einzigen Message-Block
- **Review nach Completion:** Prüfe alle Agent-Ergebnisse bevor du mergst
- **Integration Tests:** Nach Merge immer Integration Tests durchführen
- **Wenn unklar → Sequentiell:** Bei Zweifeln lieber sequentiell arbeiten

## Quality Gates

### Vor jedem Commit:
- [ ] Code kompiliert ohne Fehler
- [ ] **KEINE Compiler-Warnungen** (0 warnings erforderlich!)
- [ ] Alle Tests laufen durch
- [ ] impl_progress.md aktualisiert
- [ ] Neue Begriffe im Glossar
- [ ] Relevante Doku angepasst

### Vor jedem Release:
- [ ] **Alle Unit Tests ausgeführt und bestanden** (dotnet test) - KRITISCH!
- [ ] **Alle UI Automation Tests ausgeführt und bestanden** - KRITISCH!
  - UI Tests verifizieren dass die App tatsächlich startet
  - Würde broken binaries (138 KB statt 3.3 MB) sofort erkennen
- [ ] **Publish Build erstellt mit dotnet publish** (NICHT dotnet build!) - KRITISCH!
  - Befehl: `dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true`
  - Output aus `publish/` Ordner verwenden, NICHT aus `bin/Release/`
- [ ] **Binary Größe verifiziert** (~3.3 MB korrekt, 138 KB = FEHLER!) - KRITISCH!
- [ ] **Published Binary manuell getestet** (startet, öffnet Dateien, keine Crashes, UI funktioniert) - KRITISCH!
- [ ] **Code kompiliert mit 0 Errors und 0 Warnings** (KRITISCH!)
- [ ] Test Coverage >= 80%
- [ ] Alle Features dokumentiert
- [ ] Glossar konsolidiert
- [ ] Doku synchronisiert
- [ ] Manual Testing durchgeführt
- [ ] CHANGELOG.md vollständig

### Code Quality Standards:
- **NIEMALS** mit Warnungen releasen
- Nullable Reference Warnings müssen behoben werden
- Unused Code muss entfernt werden
- Alle Warnungen ernst nehmen und beheben

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
- ✅ **Parallele Agenten für unabhängige Tasks** (3x schneller!)
- ✅ **Implementierungsplan vor Agenten-Start** (verhindert Chaos)
- ✅ **Ein Message mit mehreren Task-Aufrufen** (echte Parallelität)
- ✅ **Klare Agent-Aufgaben mit spezifischen Dateien** (keine Überschneidungen)

### Was vermeiden:
- ❌ "Ich dokumentiere später" (wird vergessen)
- ❌ Große Refactorings ohne Tests
- ❌ Code schreiben ohne Plan
- ❌ Glossar am Ende zusammenstellen (zu aufwändig)
- ❌ **Mit Compiler-Warnungen releasen** (Code Quality!)
- ❌ Nullable Reference Warnings ignorieren
- ❌ "Die Warnungen sind nicht schlimm" Mentalität
- ❌ **Agenten ohne Plan starten** (führt zu Merge-Konflikten)
- ❌ **Abhängige Tasks parallel ausführen** (Agent 2 braucht Ergebnis von Agent 1)
- ❌ **Agenten sequentiell starten** (verliert Geschwindigkeitsvorteil)
- ❌ **Gemeinsame Dateien von mehreren Agenten bearbeiten** (Merge-Hölle)
- ❌ **Release ohne UI Automation Tests erstellen** (würde broken binaries erkennen!)
- ❌ **dotnet build statt dotnet publish verwenden** (erzeugt 138 KB DLL statt 3.3 MB EXE)
- ❌ **Binary-Größe nicht prüfen vor Upload** (138 KB ist offensichtlich falsch, sollte ~3.3 MB sein)
- ❌ **Binary aus bin/Release/ statt publish/ verwenden** (Managed DLL statt Self-Contained EXE)

## Quick Reference: Soll ich parallelisieren?

Nutze diese Checkliste um schnell zu entscheiden ob parallele Agenten sinnvoll sind:

### ✅ JA - Parallelisiere wenn:
```
[ ] Feature hat 3+ logisch getrennte Komponenten
[ ] Jede Komponente benötigt > 5 Minuten Arbeit
[ ] Komponenten arbeiten in verschiedenen Dateien
[ ] Keine zirkulären Abhängigkeiten zwischen Komponenten
[ ] Schnittstellen zwischen Komponenten sind klar definiert
[ ] Gesamt-Aufwand > 30 Minuten
```

### ❌ NEIN - Arbeite sequentiell wenn:
```
[ ] Feature ist klein (< 15 Minuten total)
[ ] Komponenten teilen viele gemeinsame Dateien
[ ] Starke Abhängigkeiten: Component B braucht Output von A
[ ] Unklare Anforderungen oder Design
[ ] Unsicher über Aufgabentrennung
```

### 📋 Workflow bei Parallelisierung:

1. **Plan erstellen** (5 Min)
   ```markdown
   Agent 1: [Komponente] - Dateien: [X, Y] - Tests: [Z]
   Agent 2: [Komponente] - Dateien: [A, B] - Tests: [C]
   Agent 3: [Komponente] - Dateien: [D, E] - Tests: [F]
   ```

2. **Agenten starten** (1 Message mit 3 Task-Calls)
   - Alle gleichzeitig in EINEM Message-Block!

3. **Nach Completion**
   - Review aller Ergebnisse
   - Merge durchführen
   - Integration Tests
   - Build & Test

### 💡 Beispiel aus v1.7.1:
```
Feature: MarkdownDialog mit Info-Button

✅ PARALLELISIERT:
- Agent 1: MarkdownDialog.cs erstellen (neue Datei)
- Agent 2: Info-Button Handler (MainForm.cs)
- Agent 3: UI Cleanup (StatusBarControl.cs)

Ergebnis: 3x schneller, 0 Konflikte
```

## Nächste Schritte

Nach diesem Dokument:
1. ✅ ROADMAP.md erstellen (detailliert!)
2. ✅ ARCHITECTURE.md erstellen
3. ✅ impl_progress.md initialisieren
4. ✅ GLOSSARY.md initialisieren
5. ⏳ Mit Implementation beginnen

---

**Version:** 2.1 (Mit verpflichtenden Test-Anforderungen vor Releases)
**Erstellt:** 2025-11-05
**Aktualisiert:** 2025-11-09
**Status:** Active

**Änderungen in v2.1:**
- Phase 4.3: Detaillierte Test-Schritte vor Release (Unit Tests, UI Automation, Publish Build)
- Quality Gates: Erweiterte Checkliste mit verpflichtenden Tests
- Lessons Learned: Dokumentation des "138 KB broken binary" Fehlers
- Prävention: UI Automation Tests würden solche Fehler sofort erkennen
