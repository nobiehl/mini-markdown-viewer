# Link Test

This file tests all navigation and link types in MarkdownViewer.

## External Links
- [Google](https://www.google.com) - Should open in browser
- [GitHub](https://github.com) - Should open in browser
- [Microsoft](https://www.microsoft.com) - Should open in browser

## Local Links (Relative)
- [Test File](test.md) - Navigate to test.md (now exists!)
- [README](README.md) - Navigate to project README
- [Features](features.md) - View all features

## Anchor Links (Same File)
- [Jump to External Links](#external-links)
- [Jump to Local Links](#local-links-relative)
- [Jump to Testing Section](#testing-instructions)

## Testing Instructions

1. Click each external link → Should open in browser
2. Click each local link → Should navigate in viewer
3. Click anchor links → Should scroll to section
4. Press Alt+Left → Should go back
5. Press Alt+Right → Should go forward
6. Press Ctrl+F → Should open search
7. Right-click → Should show theme menu
8. StatusBar → Switch themes and languages

Test completed. ✅
