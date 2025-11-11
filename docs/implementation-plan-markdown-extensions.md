# Implementierungsplan: 4 Markdown Extensions

**Datum:** 2025-01-10
**Status:** In Planning
**Geschätzter Gesamt-Aufwand:** ~90 Minuten

## Übersicht

Implementierung von 4 neuen Markdown-Extensions für MarkdownViewer:
1. Auto Table of Contents (TOC)
2. Emoji Support
3. Admonitions/Callouts
4. Code Diff Highlighting

## Parallelisierungsanalyse

**✅ Parallelisierung sinnvoll weil:**
- 4 unabhängige Komponenten
- Minimale Code-Überschneidungen (nur MarkdownRenderer.cs wird von allen berührt)
- Jede Komponente > 15 Minuten Arbeit
- Klare Schnittstellen (CSS, JavaScript, Markdig Pipeline)
- Gesamt-Aufwand ~90 Minuten → Mit Parallelisierung ~30 Minuten

**Strategie:**
- Agent 1: TOC (komplexeste Aufgabe, JavaScript-basiert)
- Agent 2: Emoji + Diff (einfach, Markdig + CSS)
- Agent 3: Admonitions (mittel, CSS + Testing)

## Agent 1: Auto Table of Contents (TOC)

**Ziel:** Automatisches Inhaltsverzeichnis aus Headings generieren

**Dateien:**
- `MarkdownViewer.Core/Core/MarkdownRenderer.cs` (JavaScript hinzufügen)
- `markdown-viewer/samples/toc-example.md` (Beispiel-Datei erstellen)

**Implementierung:**
1. JavaScript-Funktion hinzufügen die:
   - Alle Headings (h1-h6) mit IDs sammelt
   - `[TOC]` Platzhalter findet und ersetzt
   - Verschachtelte Liste generiert basierend auf Heading-Levels
   - Smooth-Scroll zu Anchors ermöglicht
2. CSS für TOC-Styling hinzufügen
3. Beispiel-Datei mit [TOC] erstellen

**Tests:**
- Unit Test: `MarkdownRendererTests.cs` - TOC wird korrekt generiert
- Integration Test: Manuell mit toc-example.md testen
- Edge Cases: Leeres Dokument, nur ein Heading, keine Headings

**Keine Abhängigkeiten zu:** Agent 2, Agent 3

**Geschätzte Dauer:** 45 Minuten

---

## Agent 2: Emoji Support + Code Diff Highlighting

**Ziel:** Emoji-Codes konvertieren und Diff-Syntax highlighten

**Dateien:**
- `MarkdownViewer.Core/Core/MarkdownRenderer.cs` (Pipeline + CSS)
- `markdown-viewer/samples/emoji-example.md` (Beispiel-Datei erstellen)
- `markdown-viewer/samples/diff-example.md` (Beispiel-Datei erstellen)

**Implementierung:**

### Emoji Support:
1. Markdig Pipeline erweitern: `.UseEmojiAndSmiley()`
2. Test mit `:smile:`, `:heart:`, `:rocket:`, `:warning:`
3. Beispiel-Datei erstellen

### Code Diff Highlighting:
1. CSS für diff-Highlighting hinzufügen:
   - `.hljs-deletion` → rote Hintergrundfarbe
   - `.hljs-addition` → grüne Hintergrundfarbe
2. Highlight.js unterstützt bereits `diff` Language
3. Beispiel-Datei mit diff-Code erstellen

**Tests:**
- Unit Test: `MarkdownRendererTests.cs` - Emoji werden konvertiert
- Unit Test: Diff-Code wird mit richtigen CSS-Klassen gerendert
- Integration Test: Manuell emoji-example.md und diff-example.md testen

**Keine Abhängigkeiten zu:** Agent 1, Agent 3

**Geschätzte Dauer:** 20 Minuten

---

## Agent 3: Admonitions/Callouts

**Ziel:** Farbige Info-/Warn-/Tipp-Boxen implementieren

**Dateien:**
- `MarkdownViewer.Core/Core/MarkdownRenderer.cs` (CSS hinzufügen)
- `markdown-viewer/samples/admonitions-example.md` (Beispiel-Datei erstellen)

**Implementierung:**
1. CSS für Custom Containers hinzufügen:
   - `div.note` → blaue Border, heller Hintergrund, Info-Icon
   - `div.warning` → orange Border, gelber Hintergrund, Warn-Icon
   - `div.tip` → grüne Border, grüner Hintergrund, Tipp-Icon
   - `div.danger` → rote Border, roter Hintergrund, Danger-Icon
   - `div.info` → cyan Border, cyan Hintergrund, Info-Icon
2. Icons mit Unicode-Symbolen oder CSS ::before content
3. Beispiel-Datei mit allen Admonition-Typen erstellen
4. Theme-Support: Farben anpassen für Dark/Light Themes

**Markdown-Syntax (Custom Container Extension ist bereits aktiv):**
```markdown
::: note
Dies ist ein Hinweis
:::

::: warning
Dies ist eine Warnung
:::

::: tip
Dies ist ein Tipp
:::

::: danger
Gefährlich!
:::
```

**Tests:**
- Unit Test: `MarkdownRendererTests.cs` - Custom Container wird gerendert
- Integration Test: Manuell admonitions-example.md testen
- Theme Test: Alle Admonitions in Dark/Light Theme prüfen

**Keine Abhängigkeiten zu:** Agent 1, Agent 2

**Geschätzte Dauer:** 30 Minuten

---

## Integration (Sequentiell nach Agenten)

**Nach Agent-Completion:**

1. **Merge durchführen**
   - Alle drei Agent-Ergebnisse reviewen
   - MarkdownRenderer.cs zusammenführen (Haupt-Konfliktpunkt)
   - Sicherstellen dass alle CSS/JavaScript-Blöcke enthalten sind

2. **Integration Tests**
   - Build: `dotnet build -c Release`
   - Unit Tests: `dotnet test`
   - Manuelle Tests mit allen Sample-Dateien:
     - toc-example.md
     - emoji-example.md
     - diff-example.md
     - admonitions-example.md

3. **Dokumentation**
   - README.md updaten mit neuen Features
   - CHANGELOG.md eintragen
   - impl_progress.md aktualisieren

4. **Quality Gates**
   - [ ] Code kompiliert ohne Fehler
   - [ ] 0 Compiler-Warnungen
   - [ ] Alle Unit Tests grün
   - [ ] Manuelles Testing abgeschlossen
   - [ ] Dokumentation aktualisiert

---

## Risiken & Mitigation

**Risiko 1: Merge-Konflikte in MarkdownRenderer.cs**
- **Mitigation:** Alle Agenten fügen Code an verschiedenen Stellen ein:
  - Agent 1: JavaScript-Block (Zeile ~335)
  - Agent 2: Pipeline (Zeile ~26) + CSS (Zeile ~180)
  - Agent 3: CSS (Zeile ~180)
- **Plan:** CSS-Blöcke von Agent 2+3 manuell zusammenführen

**Risiko 2: Theme-Kompatibilität bei Admonitions**
- **Mitigation:** Agent 3 verwendet Theme-Variables aus bestehendem Code
- **Fallback:** Lightness-adjusted Colors für Dark Theme

**Risiko 3: TOC funktioniert nicht bei komplexen Dokumenten**
- **Mitigation:** Edge Cases in Tests abdecken
- **Fallback:** Graceful Degradation - [TOC] bleibt stehen wenn keine Headings

---

## Erfolgs-Kriterien

✅ **Phase 1 - Implementierung:**
- [ ] Agent 1: TOC funktioniert mit [TOC] Platzhalter
- [ ] Agent 2: Emojis werden korrekt konvertiert
- [ ] Agent 2: Diff-Code wird farbig dargestellt
- [ ] Agent 3: Alle 5 Admonition-Typen sind gestyled

✅ **Phase 2 - Integration:**
- [ ] Merge erfolgreich ohne Konflikte
- [ ] Build erfolgreich (0 Errors, 0 Warnings)
- [ ] Alle Unit Tests grün
- [ ] Alle Sample-Dateien funktionieren

✅ **Phase 3 - Dokumentation:**
- [ ] README.md aktualisiert
- [ ] CHANGELOG.md eintragen
- [ ] impl_progress.md aktualisiert
- [ ] Sample-Dateien sind dokumentiert

---

**Nächster Schritt:** Agenten parallel starten mit Task Tool (1 Message, 3 Task-Calls)
