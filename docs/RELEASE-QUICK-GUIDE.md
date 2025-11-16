# Release Quick Guide - MarkdownViewer

**⏱️ 20-30 Minuten | Immer DIESE Reihenfolge einhalten!**

---

## Phase 1: DOKUMENTATION (10-15 Min) ⚠️ ZUERST!

**Parallel mit 3 Agents:**

### Agent 1: Core Documentation
- [ ] **CHANGELOG.md**: Release-Eintrag vX.Y.Z
  - Added/Changed/Fixed Sektionen
  - Technical Metrics (Tests, Binary Size, Lines)
- [ ] **impl_progress.md**: Session-Eintrag mit Metriken

### Agent 2: Developer Documentation
- [ ] **ROADMAP.md**: Fertige Features als ✅ markieren
- [ ] **ARCHITECTURE.md**: Neue Komponenten dokumentieren (falls nötig)

### Agent 3: Reference & Cleanup
- [ ] **GLOSSARY.md**: Neue Begriffe/Services alphabetisch
- [ ] **USER-GUIDE.md**: Neue Features dokumentieren
- [ ] **Cleanup**: Temporäre Dateien löschen (.backup, implementation-plan-*.md)

---

## Phase 2: Tests & Build (5-10 Min)

```bash
# 2.1 Unit Tests
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --configuration Release --verbosity normal
```
- [ ] Alle Tests bestanden (0 failed)
- [ ] Test Coverage >= 80%

```bash
# 2.2 Build Quality
cd ../MarkdownViewer
dotnet build --configuration Release
```
- [ ] **0 Errors** ⚠️
- [ ] **0 Warnings** ⚠️ KRITISCH!

---

## Phase 3: Binary (5 Min)

```bash
# 3.1 Publish (NICHT build!)
dotnet publish --configuration Release --runtime win-x64 --output ../../bin-single
```

```bash
# 3.2 Größe prüfen
cd ../../bin-single
ls -lh MarkdownViewer.exe
```
- [ ] Größe: **~3.3 MB** ✅ (NICHT 138 KB!)

```bash
# 3.3 Manuell testen
./MarkdownViewer.exe ../README.md
```
- [ ] Startet ohne Fehler
- [ ] Markdown wird gerendert
- [ ] Theme-Wechsel funktioniert

---

## Phase 4: ⚠️⚠️⚠️ Binary Name (KRITISCH!)

```bash
# Binary für Upload vorbereiten
cd ../
cp bin-single/MarkdownViewer.exe ./MarkdownViewer.exe
ls -lh MarkdownViewer.exe
```

**⚠️ 3x PRÜFEN - MEHRFACH SCHIEFGEGANGEN:**
- [ ] Name: **EXAKT** `MarkdownViewer.exe` ✅
- [ ] Größe: **~3.3 MB** ✅
- [ ] **KEINE Versionsnummer** im Namen!

**Warum kritisch?**
- UpdateChecker sucht nach "MarkdownViewer.exe"
- Falsche Namen brechen Auto-Update für ALLE Nutzer!

---

## Phase 5: Git (2 Min)

```bash
git add .
git commit -m "feat: <Feature> (vX.Y.Z)

<Details>

🤖 Generated with Claude Code
Co-Authored-By: Claude <noreply@anthropic.com>"

git tag -a vX.Y.Z -m "Release vX.Y.Z - <Feature>"
git push origin master
git push origin vX.Y.Z
```

---

## Phase 6: GitHub Release (3 Min)

**⚠️ FINALE PRÜFUNG:**
```bash
ls -la MarkdownViewer.exe
```
- [ ] Name: `MarkdownViewer.exe` ✅
- [ ] Größe: ~3.3 MB ✅

```bash
gh release create vX.Y.Z \
  --title "vX.Y.Z - <Feature>" \
  --notes "<Release Notes aus CHANGELOG.md>" \
  "MarkdownViewer.exe"
```

**Nach Upload auf GitHub verifizieren:**
- [ ] Binary heißt `MarkdownViewer.exe` (NICHT vX.Y.Z.exe!)
- [ ] Binary Größe ~3.3 MB
- [ ] Download-Link funktioniert

---

## Phase 7: Validation (2 Min)

```bash
# Download von GitHub testen
curl -L <release-url> -o test.exe
ls -lh test.exe  # Muss ~3.3 MB sein
./test.exe README.md  # Muss starten
```

---

## ⚠️ Quick Checklist (5 KRITISCHE Punkte)

```
1. BINARY NAME: MarkdownViewer.exe (KEINE VERSION!)
2. BINARY SIZE: ~3.3 MB (NICHT 138 KB!)
3. ALL TESTS: Passed (Unit Tests)
4. BUILD: 0 Errors, 0 Warnings
5. DOCS: CHANGELOG.md vollständig
```

**Bei EINEM Fehler: Release NICHT erstellen!**

---

## Häufige Fehler (Lessons Learned)

1. ❌ Dokumentation NACH Release → **IMMER ZUERST!**
2. ❌ Binary mit Versionsnummer → **Auto-Update kaputt!**
3. ❌ `dotnet build` statt `publish` → **138 KB statt 3.3 MB!**
4. ❌ Mit Warnings releasen → **Code Quality Probleme!**
5. ❌ Binary nicht manuell testen → **Broken Binaries online!**

---

**Version:** 1.0
**Created:** 2025-11-16
**Purpose:** Kompakter Release-Guide ohne Kontext-Overhead
