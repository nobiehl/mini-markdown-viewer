# Release Checklist - MarkdownViewer

**⚠️ KRITISCH: Lies ALLES bevor du ein Release erstellst!**

Diese Checkliste ist **PFLICHT** vor jedem Release. Mehrere kritische Fehler sind bereits passiert, weil diese Checkliste nicht befolgt wurde.

---

## Phase 1: Tests & Build

### 1.1 Unit Tests
```bash
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --configuration Release --verbosity normal
```

**Anforderungen:**
- [ ] **Alle Tests bestanden** (0 failed)
- [ ] Test Coverage >= 80%
- [ ] Keine Test-Warnungen

**Bei Fehlern:** Release NICHT erstellen! Tests erst beheben.

---

### 1.2 UI Automation Tests
```bash
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --filter "FullyQualifiedName~UIAutomation" --verbosity normal
```

**Anforderungen:**
- [ ] **Alle UI Tests bestanden**
- [ ] Anwendung startet korrekt
- [ ] Keine UI-Freezes oder Crashes

**Wichtig:** UI Tests erkennen broken binaries (138 KB statt 3.3 MB)!

---

### 1.3 Build Quality
```bash
cd markdown-viewer/MarkdownViewer
dotnet build --configuration Release
```

**Anforderungen:**
- [ ] **0 Errors** (KRITISCH!)
- [ ] **0 Warnings** (KRITISCH! - Nicht mit Warnungen releasen!)
- [ ] Keine Nullable Reference Warnings
- [ ] Keine Unused Code Warnings

---

## Phase 2: Binary Erstellung

### 2.1 Publish Build erstellen
```bash
cd markdown-viewer/MarkdownViewer
dotnet publish --configuration Release --runtime win-x64 --output ../../bin-single
```

**NICHT verwenden:**
- ❌ `dotnet build` (erzeugt 138 KB Managed DLL)
- ❌ Output aus `bin/Release/` (nicht deployment-ready)

**Nur verwenden:**
- ✅ `dotnet publish`
- ✅ Output aus `bin-single/` oder `publish/`

---

### 2.2 Binary Größe verifizieren

```bash
cd bin-single
ls -lh MarkdownViewer.exe
```

**⚠️ KRITISCHE Prüfung:**
- [ ] Dateigröße ist **~3.3 MB** ✅ KORREKT
- [ ] Dateigröße ist NICHT 138 KB ❌ FALSCH (Managed DLL statt EXE)
- [ ] Dateigröße ist NICHT 158 MB ❌ FALSCH (Self-contained statt Framework-dependent)

**Bei falscher Größe:** Binary neu erstellen mit korrektem Befehl!

---

### 2.3 Binary manuell testen

```bash
# Binary starten mit Test-Datei
./MarkdownViewer.exe test-file.md
```

**Pflicht-Tests:**
- [ ] Anwendung startet ohne Fehler
- [ ] Markdown-Datei wird korrekt gerendert
- [ ] Keine Crashes oder Freezes
- [ ] UI ist responsive
- [ ] Theme-Wechsel funktioniert
- [ ] Sprach-Wechsel funktioniert

**Bei Problemen:** Binary neu erstellen und erneut testen!

---

## Phase 3: Binary Upload Vorbereitung

### 3.1 ⚠️⚠️⚠️ KRITISCH: Binary Name prüfen ⚠️⚠️⚠️

```bash
# Binary MUSS umbenannt werden für GitHub Release
cd ../../
cp bin-single/MarkdownViewer.exe ./MarkdownViewer.exe
```

**⚠️ ABSOLUT KRITISCH - MEHRFACH SCHIEFGEGANGEN:**

- [ ] Binary heißt **EXAKT** `MarkdownViewer.exe` ✅
- [ ] Binary heißt **NICHT** `MarkdownViewer-v1.9.0.exe` ❌
- [ ] Binary heißt **NICHT** `MarkdownViewer-vX.Y.Z.exe` ❌
- [ ] Keine Versionsnummer im Dateinamen! ❌

**Warum so wichtig?**
- UpdateChecker.cs sucht nach Asset `"MarkdownViewer.exe"`
- Falsche Namen brechen Auto-Update für **ALLE Nutzer**!
- Dies ist bereits **MEHRFACH** schief gegangen!
- v1.9.0 Release wurde dadurch kaputt gemacht!

**Upload-Befehl prüfen:**
```bash
# ✅ RICHTIG:
gh release create v1.9.0 ... "MarkdownViewer.exe"

# ❌ FALSCH:
gh release create v1.9.0 ... "MarkdownViewer-v1.9.0.exe"
```

---

## Phase 4: Dokumentation

### 4.1 Version Bump

**Dateien prüfen:**
- [ ] `Program.cs`: `private const string Version = "X.Y.Z";`
- [ ] `MainForm.cs`: `private const string Version = "X.Y.Z";`
- [ ] `README.md`: Version Badge aktualisiert

---

### 4.2 CHANGELOG.md (SINGLE SOURCE OF TRUTH!)

```markdown
## [X.Y.Z] - YYYY-MM-DD

### Added
- Feature 1 mit detaillierter Beschreibung
- Feature 2 mit detaillierter Beschreibung

### Fixed
- Bug 1 mit Beschreibung und Konsequenz

### Changed
- Änderung 1 mit Auswirkung

### Technical
- Build info (0 errors, 0 warnings)
- Tests: X/Y passing (Z% success rate)
- Binary size: X.X MB
```

**Pflicht-Checks:**
- [ ] Release-Eintrag vollständig
- [ ] Alle Features dokumentiert
- [ ] Alle Bugfixes dokumentiert
- [ ] Technical Metrics enthalten
- [ ] Keine separaten `RELEASE-NOTES-vX.Y.Z.md` Dateien erstellt

---

### 4.3 README.md

**Zu aktualisieren:**
- [ ] Version Badge (nur Nummer, keine Features!)
- [ ] Binary Size Badge (falls geändert)
- [ ] Test Count Badge
- [ ] Quick Start Download-Link

---

### 4.4 impl_progress.md

- [ ] Session-Eintrag mit vollständigen Metriken
- [ ] Lines added/removed
- [ ] Files changed
- [ ] Tests added
- [ ] Status: ✅ Completed

---

## Phase 5: Git & GitHub

### 5.1 Git Commit

```bash
git add <files>
git commit -m "feat: <Feature> (vX.Y.Z)

<Detailed description>

🤖 Generated with Claude Code

Co-Authored-By: Claude <noreply@anthropic.com>"
```

**Commit Message Requirements:**
- [ ] Conventional Commits Format
- [ ] Aussagekräftige Beschreibung
- [ ] Claude Code Attribution

---

### 5.2 Git Tag

```bash
git tag -a vX.Y.Z -m "Release vX.Y.Z - <Feature>"
```

**Tag Requirements:**
- [ ] Semantic Versioning (vX.Y.Z)
- [ ] Aussagekräftige Message

---

### 5.3 Push zu GitHub

```bash
git push origin master
git push origin vX.Y.Z
```

**Nach Push prüfen:**
- [ ] Commit auf GitHub sichtbar
- [ ] Tag auf GitHub sichtbar

---

## Phase 6: GitHub Release

### 6.1 ⚠️ FINALE BINARY-NAME-PRÜFUNG ⚠️

**VOR dem Release-Befehl nochmal prüfen:**

```bash
ls -la MarkdownViewer.exe
```

**KRITISCHE Fragen:**
- [ ] Heißt die Datei **EXAKT** `MarkdownViewer.exe`? ✅
- [ ] Ist die Größe **~3.3 MB**? ✅
- [ ] Liegt sie im Root-Verzeichnis? ✅

**Bei NEIN:** STOPP! Korrigiere zuerst!

---

### 6.2 GitHub Release erstellen

```bash
gh release create vX.Y.Z \
  --title "vX.Y.Z - <Feature>" \
  --notes "$(cat <<'EOF'
<Release Notes aus CHANGELOG.md kopieren>
EOF
)" \
  "MarkdownViewer.exe"
```

**⚠️ LETZTE Prüfung vor Enter:**
- [ ] Binary heißt `MarkdownViewer.exe` (KEINE Versionsnummer!)
- [ ] Release Notes vollständig
- [ ] Title format korrekt

---

### 6.3 GitHub Release verifizieren

**Nach Upload auf GitHub prüfen:**
- [ ] Release ist sichtbar: https://github.com/nobiehl/mini-markdown-viewer/releases/tag/vX.Y.Z
- [ ] Binary heißt `MarkdownViewer.exe` (nicht v1.9.0.exe!)
- [ ] Binary Größe ist ~3.3 MB
- [ ] Release Notes korrekt formatiert
- [ ] Download-Link funktioniert

**Bei Fehler:** Release sofort löschen und neu erstellen!

---

## Phase 7: Post-Release Validation

### 7.1 Binary Download testen

```bash
# Download von GitHub Release
curl -L https://github.com/nobiehl/mini-markdown-viewer/releases/download/vX.Y.Z/MarkdownViewer.exe -o MarkdownViewer-downloaded.exe

# Größe prüfen
ls -lh MarkdownViewer-downloaded.exe

# Funktionalität testen
./MarkdownViewer-downloaded.exe test-file.md
```

**Validierung:**
- [ ] Download erfolgreich
- [ ] Größe ist ~3.3 MB
- [ ] Binary startet
- [ ] Markdown wird korrekt gerendert

---

### 7.2 Update-Mechanismus testen (optional)

```bash
# Alte Version installieren
# Update-Check manuell auslösen
# Verifizieren dass neue Version gefunden wird
```

**Wichtig:** Nur möglich wenn alte Version vorhanden ist.

---

## Fehlerhafte Releases korrigieren

### Wenn Binary falschen Namen hat:

```bash
# 1. Release auf GitHub öffnen
# 2. Bearbeiten klicken
# 3. Falsches Binary löschen
# 4. Korrektes Binary (MarkdownViewer.exe) hochladen
# 5. Speichern
```

### Wenn Binary falsche Größe hat:

```bash
# 1. Binary neu erstellen mit dotnet publish
# 2. GitHub Release bearbeiten
# 3. Altes Binary löschen
# 4. Neues Binary hochladen
```

---

## Lessons Learned

**Was MEHRFACH schief ging:**

1. ❌ Binary mit Versionsnummer hochgeladen (`MarkdownViewer-v1.9.0.exe`)
   - **Konsequenz:** Auto-Update kaputt für alle Nutzer
   - **Lösung:** IMMER `MarkdownViewer.exe` ohne Version

2. ❌ `dotnet build` statt `dotnet publish` verwendet
   - **Konsequenz:** 138 KB Managed DLL statt 3.3 MB EXE
   - **Lösung:** IMMER `dotnet publish` verwenden

3. ❌ Binary aus `bin/Release/` statt `publish/` verwendet
   - **Konsequenz:** Nicht deployment-ready
   - **Lösung:** IMMER Output aus `publish/` oder `bin-single/`

4. ❌ Mit Compiler-Warnungen released
   - **Konsequenz:** Code Quality Probleme
   - **Lösung:** IMMER 0 Warnings erzwingen

5. ❌ UI Tests nicht ausgeführt vor Release
   - **Konsequenz:** Broken Binaries nicht erkannt
   - **Lösung:** IMMER UI Automation Tests ausführen

---

## Quick Reference Card

**Vor jedem Release diese 5 KRITISCHEN Punkte prüfen:**

```
⚠️ 1. BINARY NAME: MarkdownViewer.exe (KEINE VERSION!)
⚠️ 2. BINARY SIZE: ~3.3 MB (NICHT 138 KB!)
⚠️ 3. ALL TESTS: Passed (Unit + UI Automation)
⚠️ 4. BUILD: 0 Errors, 0 Warnings
⚠️ 5. DOCS: CHANGELOG.md vollständig
```

**Bei auch nur EINEM Fehler:** Release NICHT erstellen!

---

**Version:** 1.0
**Created:** 2025-11-13
**Last Updated:** 2025-11-13
**Maintained By:** Development Team
