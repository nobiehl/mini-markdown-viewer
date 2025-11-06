# Update-Check Fix - Einfache Lösung ohne Token

**Problem:** GitHub API Rate Limit (403 Fehler) bei täglichen Update-Checks

**Lösung:** Update-Checks nur noch **alle 7 Tage** statt täglich

---

## Was wurde geändert?

### 1. Update-Intervall: 7 Tage statt 1 Tag

**Datei:** `AppSettings.cs`
```csharp
public int CheckIntervalDays { get; set; } = 7;  // vorher: 1
```

**Effekt:**
- Nur noch 4 API-Calls pro Monat statt 30
- Rate Limit wird nicht mehr erreicht

### 2. Verbesserte Intervall-Logik

**Datei:** `UpdateChecker.cs`
```csharp
// Prüft ob 7 Tage vergangen sind (nicht nur "anderer Tag")
TimeSpan elapsed = now - lastCheck;
bool shouldCheck = elapsed.TotalDays >= 7;
```

**Vorher:** Prüfte nur ob heute ein anderes Datum ist
**Jetzt:** Prüft ob mindestens 7 Tage vergangen sind

### 3. Fehler werden automatisch ignoriert

**Datei:** `Program.cs`
```csharp
if (forcedByUser)  // Nur bei manuellem Check (--update)
{
    MessageBox.Show("Fehler...");
}
// Bei automatischem Check: Keine Fehlermeldung!
```

**Effekt:**
- Bei automatischem Check (beim Start): Keine Fehlermeldungen
- Nur Eintrag im Log
- Benutzer wird nicht belästigt

---

## Für Benutzer

**Keine Aktion erforderlich!**

- Update-Check läuft weiterhin automatisch
- Nur noch alle 7 Tage statt täglich
- Keine Fehlermeldungen bei Problemen
- Funktioniert sofort nach Update

---

## Technische Details

### Rate Limits

| Szenario | Vorher | Jetzt |
|----------|--------|-------|
| API-Calls pro Monat | ~30 | ~4 |
| Rate Limit Problem | Ja (60/Stunde) | Nein |
| Benutzer-Aktion nötig | Ja (Token) | **Nein** |

### Für Power-User

Wer trotzdem öfter checken will:
- Manueller Check: `MarkdownViewer.exe --update`
- Zählt nicht zum automatischen Intervall
- Optional: GitHub Token möglich (aber nicht nötig)

---

## Änderungen

- ✅ AppSettings.cs: CheckIntervalDays = 7
- ✅ UpdateChecker.cs: ShouldCheckForUpdates() mit TotalDays >= 7
- ✅ UpdateChecker.cs: RecordUpdateCheck() speichert DateTime mit Uhrzeit
- ✅ Program.cs: Fehler bei automatischem Check werden ignoriert
- ✅ GitHub Token Support bleibt optional verfügbar (für Notfälle)

---

## Testen

1. **Erste Nutzung:** Update-Check läuft sofort
2. **Zweite Nutzung (nach 5 Tagen):** Kein Check
3. **Dritte Nutzung (nach 8 Tagen):** Check läuft wieder
4. **Bei Rate Limit:** Keine Fehlermeldung, nur Log-Eintrag

---

## Deployment

- ✅ Code geändert
- ✅ Gebaut (Release)
- ✅ Publiziert nach bin-single/
- ⏳ Bereit für v1.5.2 Release

---

**Version:** v1.5.2 (vorgeschlagen)
**Datum:** 2025-11-06
**Status:** Fertig, bereit zum Release
