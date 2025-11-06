# Test Plan - v1.5.1

**Version:** 1.5.1
**Date:** 2025-11-06
**Feature:** F11 StatusBar Toggle + Settings Persistence

---

## Manual Test Checklist

### Test 1: F11 Toggle StatusBar (Fresh Start)

**Prerequisites:**
- Clean installation (no settings.json exists)
- MarkdownViewer.exe v1.5.1

**Steps:**
1. Start MarkdownViewer.exe with any .md file
2. ✅ **Verify:** StatusBar is HIDDEN (default state)
3. Press **F11**
4. ✅ **Verify:** StatusBar appears at bottom with 5 sections:
   - Update status icon
   - Explorer registration icon
   - Language selector dropdown
   - Info link
   - Help link
5. Press **F11** again
6. ✅ **Verify:** StatusBar disappears
7. Press **F11** to show again
8. ✅ **Verify:** StatusBar reappears

**Expected Result:**
- ✅ F11 toggles StatusBar visibility
- ✅ No errors in logs

---

### Test 2: Settings Persistence

**Steps:**
1. Start MarkdownViewer.exe
2. Press **F11** to SHOW StatusBar
3. Close MarkdownViewer
4. ✅ **Verify settings.json:**
   ```json
   {
     "UI": {
       "StatusBar": {
         "Visible": true
       }
     }
   }
   ```
5. Restart MarkdownViewer
6. ✅ **Verify:** StatusBar is VISIBLE on startup
7. Press **F11** to HIDE StatusBar
8. Close MarkdownViewer
9. ✅ **Verify settings.json:**
   ```json
   {
     "UI": {
       "StatusBar": {
         "Visible": false
       }
     }
   }
   ```
10. Restart MarkdownViewer
11. ✅ **Verify:** StatusBar is HIDDEN on startup

**Expected Result:**
- ✅ StatusBar state persists across restarts
- ✅ settings.json updated correctly

---

### Test 3: Help Dialog Updated

**Steps:**
1. Start MarkdownViewer
2. Press **F11** to show StatusBar
3. Click **Help** in StatusBar
4. ✅ **Verify help text includes:**
   ```
   Keyboard Shortcuts:
   • F5 - Reload file
   • F11 - Toggle StatusBar    ← NEW!
   • Ctrl+F - Open search
   • F3 / Shift+F3 - Next/Previous match
   • Alt+Left / Alt+Right - Navigate back/forward
   ...
   ```

**Expected Result:**
- ✅ F11 shortcut documented in Help

---

### Test 4: StatusBar Functionality (All Features Work)

**Steps:**
1. Press **F11** to show StatusBar
2. ✅ **Verify Update Status:** Icon shows (✅/🔄/❌/❓)
3. ✅ **Verify Explorer Status:** Icon shows (✅📁 or ❌📁)
4. ✅ **Verify Language Selector:** Dropdown shows 8 languages
5. Click language → ✅ **Verify:** Language switches instantly
6. Click **Info** → ✅ **Verify:** Info dialog shows version 1.5.1
7. Click **Help** → ✅ **Verify:** Help dialog shows F11 shortcut

**Expected Result:**
- ✅ All StatusBar features functional after F11 toggle

---

### Test 5: Multiple Toggles

**Steps:**
1. Toggle StatusBar 10 times with F11
2. ✅ **Verify:** No lag, no memory leaks
3. Check logs for errors
4. ✅ **Verify:** No ERROR entries

**Expected Result:**
- ✅ Smooth toggles, no performance issues

---

### Test 6: Edge Cases

**Test 6.1: Toggle during file loading**
1. Open large .md file (> 1MB)
2. Immediately press **F11** multiple times
3. ✅ **Verify:** No crashes, StatusBar toggles correctly

**Test 6.2: Toggle with theme switching**
1. Show StatusBar (F11)
2. Right-click → Change theme
3. ✅ **Verify:** StatusBar maintains visibility
4. Hide StatusBar (F11)
5. Change theme again
6. ✅ **Verify:** StatusBar stays hidden

**Expected Result:**
- ✅ No conflicts with other features

---

## Automated Test Considerations

**Note:** UI tests for WinForms are complex. Manual testing is sufficient for this hotfix.

**Future Improvements:**
- Add unit tests for ToggleStatusBar() logic (mock settings)
- Integration test: Verify settings.json persistence
- UI Automation tests with FlaUI (optional)

---

## Test Results

**Tested By:** _____________
**Date:** _____________
**Build:** v1.5.1
**Platform:** Windows 10/11 x64

### Summary

| Test | Result | Notes |
|------|--------|-------|
| Test 1: F11 Toggle | ☐ Pass ☐ Fail | |
| Test 2: Persistence | ☐ Pass ☐ Fail | |
| Test 3: Help Dialog | ☐ Pass ☐ Fail | |
| Test 4: StatusBar Features | ☐ Pass ☐ Fail | |
| Test 5: Multiple Toggles | ☐ Pass ☐ Fail | |
| Test 6: Edge Cases | ☐ Pass ☐ Fail | |

**Overall:** ☐ **PASS** ☐ **FAIL**

**Comments:**
_______________________________________________________
_______________________________________________________
_______________________________________________________

---

## Known Issues

None.

---

## Regression Tests

✅ All existing features from v1.5.0 still work:
- Themes (4 themes)
- Localization (8 languages)
- Navigation (Alt+Left/Right)
- Search (Ctrl+F, F3)
- Markdown rendering
- Live file reload

---

**Version:** v1.5.1
**Change:** Added F11 keyboard shortcut to toggle StatusBar with settings persistence
