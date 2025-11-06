# Testing Checklist - MarkdownViewer v1.5.0

Manual integration testing checklist for production release validation.

**Tested by:** _____________
**Date:** _____________
**Build:** v1.5.0
**Platform:** Windows 10/11 (x64)

---

## 1. First Launch ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Default settings loaded | settings.json created in %APPDATA%\MarkdownViewer\ | ☐ Pass ☐ Fail | |
| Standard theme applied | Markdown renders with standard theme colors | ☐ Pass ☐ Fail | |
| All features hidden | StatusBar, NavigationBar hidden by default | ☐ Pass ☐ Fail | |
| Window opens centered | 1024x768, centered on screen | ☐ Pass ☐ Fail | |
| No errors in logs | logs/viewer-YYYY-MM-DD.log has no ERROR entries | ☐ Pass ☐ Fail | |

---

## 2. Settings Persistence ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Change theme → restart | Theme persisted (e.g., Dark theme still applied) | ☐ Pass ☐ Fail | |
| Change language → restart | Language persisted (e.g., Deutsch still active) | ☐ Pass ☐ Fail | |
| Toggle StatusBar → restart | StatusBar visibility state persisted | ☐ Pass ☐ Fail | |
| Toggle NavigationBar → restart | NavigationBar visibility state persisted | ☐ Pass ☐ Fail | |

---

## 3. Explorer Registration ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Run `--install` | Registry entries created in HKCU\Software\Classes\.md | ☐ Pass ☐ Fail | |
| Double-click .md file | Opens in MarkdownViewer | ☐ Pass ☐ Fail | |
| Right-click context menu | "Open with Markdown Viewer" appears | ☐ Pass ☐ Fail | |
| "Send To" menu | "Markdown Viewer" shortcut present | ☐ Pass ☐ Fail | |
| Run `--uninstall` | Registry entries removed, shortcut deleted | ☐ Pass ☐ Fail | |
| StatusBar icon | Shows ✅📁 when registered, ❌📁 when not | ☐ Pass ☐ Fail | |

---

## 4. Theme Switching ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Dark theme | Dark background, light text, syntax highlighting works | ☐ Pass ☐ Fail | |
| Solarized Light | Solarized colors, no blue component | ☐ Pass ☐ Fail | |
| Dräger theme | Corporate colors (blue/green) | ☐ Pass ☐ Fail | |
| Standard theme | Enhanced standard look | ☐ Pass ☐ Fail | |
| Theme applied instantly | No restart required, markdown re-renders | ☐ Pass ☐ Fail | |
| Checkmark indicator | Current theme shows checkmark in context menu | ☐ Pass ☐ Fail | |

---

## 5. Localization ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| English (en) | All strings in English | ☐ Pass ☐ Fail | |
| Deutsch (de) | All StatusBar strings in German | ☐ Pass ☐ Fail | |
| Монгол (mn) | All StatusBar strings in Mongolian Cyrillic | ☐ Pass ☐ Fail | |
| Français (fr) | All StatusBar strings in French | ☐ Pass ☐ Fail | |
| Español (es) | All StatusBar strings in Spanish | ☐ Pass ☐ Fail | |
| 日本語 (ja) | All StatusBar strings in Japanese | ☐ Pass ☐ Fail | |
| 简体中文 (zh) | All StatusBar strings in Chinese Simplified | ☐ Pass ☐ Fail | |
| Русский (ru) | All StatusBar strings in Russian | ☐ Pass ☐ Fail | |
| Language switch instant | StatusBar updates immediately | ☐ Pass ☐ Fail | |
| No missing translations | No [KEY] placeholders visible | ☐ Pass ☐ Fail | |

---

## 6. Navigation ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Alt+Left (back) | Navigates to previous document | ☐ Pass ☐ Fail | |
| Alt+Right (forward) | Navigates to next document | ☐ Pass ☐ Fail | |
| Back button enabled | Only when history available | ☐ Pass ☐ Fail | |
| Forward button enabled | Only when forward history available | ☐ Pass ☐ Fail | |
| History preserved | Can navigate back through multiple documents | ☐ Pass ☐ Fail | |
| Anchor navigation | Clicking internal link scrolls to anchor | ☐ Pass ☐ Fail | |

---

## 7. Search ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Ctrl+F opens search | Search bar appears at top | ☐ Pass ☐ Fail | |
| Real-time highlighting | Yellow highlights appear as you type | ☐ Pass ☐ Fail | |
| Results counter | Shows "X of Y" or "No results" | ☐ Pass ☐ Fail | |
| F3 (next match) | Scrolls to next match, highlights orange | ☐ Pass ☐ Fail | |
| Shift+F3 (previous) | Scrolls to previous match | ☐ Pass ☐ Fail | |
| Enter in textbox | Goes to next match | ☐ Pass ☐ Fail | |
| Shift+Enter in textbox | Goes to previous match | ☐ Pass ☐ Fail | |
| Esc closes search | Search bar hides, highlights cleared | ☐ Pass ☐ Fail | |
| Smooth scrolling | Current match centered in view | ☐ Pass ☐ Fail | |
| mark.js loads | Works even on first search (CDN) | ☐ Pass ☐ Fail | |

---

## 8. Markdown Rendering ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Basic syntax | Headers, bold, italic, lists render correctly | ☐ Pass ☐ Fail | |
| Code blocks | Syntax highlighting with Highlight.js | ☐ Pass ☐ Fail | |
| Math formulas | KaTeX renders LaTeX (inline and block) | ☐ Pass ☐ Fail | |
| Mermaid diagrams | Flowcharts, sequence diagrams render | ☐ Pass ☐ Fail | |
| PlantUML diagrams | UML diagrams render via plantuml.com | ☐ Pass ☐ Fail | |
| Images | Local and remote images load | ☐ Pass ☐ Fail | |
| Links (internal) | .md file links navigate correctly | ☐ Pass ☐ Fail | |
| Links (external) | http/https open in browser | ☐ Pass ☐ Fail | |
| Tables | Markdown tables render correctly | ☐ Pass ☐ Fail | |

---

## 9. File Watching ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| Edit file externally | Auto-reloads in viewer (live reload) | ☐ Pass ☐ Fail | |
| Save changes | Content updates within 1 second | ☐ Pass ☐ Fail | |
| Multiple saves | Each save triggers reload | ☐ Pass ☐ Fail | |
| No flicker | Reload is smooth, scroll position preserved where possible | ☐ Pass ☐ Fail | |

---

## 10. Performance ✅

| Test | Expected | Target | Result | Notes |
|------|----------|--------|--------|-------|
| Startup time | From click to window visible | < 2 seconds | ☐ Pass ☐ Fail | |
| Theme switching | Time to apply new theme | < 500ms | ☐ Pass ☐ Fail | |
| Large file (10MB) | Opens and scrolls smoothly | No lag | ☐ Pass ☐ Fail | |
| Memory usage | After 10 minutes of use | < 100MB | ☐ Pass ☐ Fail | |
| Search (1000+ matches) | Highlighting completes quickly | < 2 seconds | ☐ Pass ☐ Fail | |

---

## 11. Error Handling ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| File not found | Shows error dialog, logs error | ☐ Pass ☐ Fail | |
| Invalid .md file | Displays error gracefully | ☐ Pass ☐ Fail | |
| WebView2 not installed | Shows helpful error message | ☐ Pass ☐ Fail | |
| Corrupted settings.json | Falls back to defaults, logs warning | ☐ Pass ☐ Fail | |
| Missing theme file | Falls back to standard theme | ☐ Pass ☐ Fail | |
| Network error (mark.js) | Search still works after retry | ☐ Pass ☐ Fail | |

---

## 12. Command-Line Arguments ✅

| Test | Expected | Result | Notes |
|------|----------|--------|-------|
| No args | Opens file dialog | ☐ Pass ☐ Fail | |
| `file.md` | Opens specified file | ☐ Pass ☐ Fail | |
| `--version` | Shows version dialog | ☐ Pass ☐ Fail | |
| `--help` | Shows help dialog | ☐ Pass ☐ Fail | |
| `--install` | Registers Explorer integration | ☐ Pass ☐ Fail | |
| `--uninstall` | Unregisters Explorer integration | ☐ Pass ☐ Fail | |
| `--log-level Debug` | Enables debug logging | ☐ Pass ☐ Fail | |

---

## Summary

**Total Tests:** 85
**Passed:** ___
**Failed:** ___
**Pass Rate:** ___%

**Critical Issues:**
_______________________
_______________________
_______________________

**Minor Issues:**
_______________________
_______________________
_______________________

**Notes:**
_______________________
_______________________
_______________________

---

**Recommendation:**
☐ **Approve for Release** - All critical tests passed
☐ **Reject** - Critical issues found
☐ **Conditional Approval** - Minor issues acceptable for release

**Signature:** _____________
**Date:** _____________
