# Release Notes v1.5.2

**Release Date:** 2025-11-06
**Type:** Bugfix Release

---

## Critical Bug Fix

### Update-Check Retry Logic Fixed

**Problem:**
Update-Check-Timestamp wurde VOR dem API-Call gespeichert. Bei Fehlern (403 Rate Limit, Network Error, etc.) wurde kein Retry durchgeführt für 7 Tage!

**Impact:**
- User erhielten nie Update-Benachrichtigungen wenn der erste Check fehlschlug
- Bei Rate Limit Errors: 7 Tage kein erneuter Versuch
- Update-Mechanismus funktionierte nicht zuverlässig

**Fix:**
`RecordUpdateCheck()` wird jetzt nur bei **erfolgreichem** API-Call ausgeführt.

**Neues Verhalten:**
- API-Call fehlschlägt → **KEIN** Datum gespeichert
- Nächster Start → **Retry** des Update-Checks
- Log-Message: "Update check failed with error: ... - Will retry on next start"
- Irgendwann klappt es! ✅

---

## Changes

### Code Changes

**File:** `Program.cs:CheckForUpdatesAsync()`

**Before:**
```csharp
// Line 404: RecordUpdateCheck() called BEFORE API call
checker.RecordUpdateCheck();

// Line 407: THEN API call
var updateInfo = await checker.CheckForUpdatesAsync(Version);

// On error: Timestamp already saved! ❌
if (!string.IsNullOrEmpty(updateInfo.Error))
{
    return; // No retry for 7 days!
}
```

**After:**
```csharp
// Line 404: API call FIRST
var updateInfo = await checker.CheckForUpdatesAsync(Version);

// On error: NO RecordUpdateCheck() ✅
if (!string.IsNullOrEmpty(updateInfo.Error))
{
    Log.Warning("... Will retry on next start");
    return; // Timestamp NOT saved → Retry next start!
}

// Line 427: Record ONLY on success ✅
checker.RecordUpdateCheck();
Log.Debug("Update check successful, recorded timestamp");
```

### Logging Improvements

**Enhanced Error Logging:**
```
[WRN] Update check failed with error: Response status code does not indicate success: 403 (rate limit exceeded). - Will retry on next start
```

**Success Logging:**
```
[DBG] Update check successful, recorded timestamp
```

### User-Facing Changes

**Error MessageBox (manual check with --update):**
```
Update-Prüfung fehlgeschlagen:
Response status code does not indicate success: 403 (rate limit exceeded).

Wird beim nächsten Start erneut versucht.
```

---

## Test Scenarios

### Scenario 1: Rate Limit Error (403)

**Steps:**
1. User starts app
2. API Call → 403 Rate Limit
3. Check Program.cs:409 log: "Will retry on next start"
4. Check last-update-check.txt → **NICHT aktualisiert**
5. User startet app erneut (sofort)
6. API Call wird **erneut** versucht

**Expected:** Retry funktioniert ✅

### Scenario 2: Network Error

**Steps:**
1. User starts app (offline)
2. API Call → Network Error
3. Check log: "Will retry on next start"
4. Check last-update-check.txt → **NICHT aktualisiert**
5. User geht online
6. User startet app erneut
7. API Call → 200 OK → Success!
8. Check last-update-check.txt → **JETZT aktualisiert**

**Expected:** Update-Check funktioniert beim zweiten Versuch ✅

### Scenario 3: Successful Check

**Steps:**
1. User starts app
2. API Call → 200 OK
3. Check log: "Update check successful, recorded timestamp"
4. Check last-update-check.txt → **Aktualisiert mit aktuellem Timestamp**
5. Nächste 7 Tage: Kein Update-Check

**Expected:** Normal Verhalten ✅

---

## Compatibility

- **Breaking Changes:** None
- **Backward Compatible:** Yes
- **Migration Required:** No

---

## Files Changed

| File | Lines Changed | Type |
|------|---------------|------|
| `Program.cs` | 404-428 | Bug Fix |
| `AppSettings.cs` | 70 | Version bump |

---

## Version History

| Version | Date | Type | Description |
|---------|------|------|-------------|
| v1.5.2 | 2025-11-06 | Bugfix | Fix: RecordUpdateCheck only on success |
| v1.5.1 | 2025-11-06 | Release | StatusBar always visible |
| v1.5.0 | 2025-11-05 | Feature | Navigation, Search, Localization |

---

## Deployment

**Build:**
```bash
cd markdown-viewer/MarkdownViewer
dotnet build --configuration Release
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single
```

**Output:**
```
bin-single/MarkdownViewer.exe
```

**Status:** ✅ Built and ready for release

---

## Credits

- **Bug Report:** User observation during testing
- **Fix:** Immediate implementation
- **Tested:** Locally verified with log analysis

---

**Recommendation:** Deploy immediately - this fixes a critical bug in the update mechanism!
