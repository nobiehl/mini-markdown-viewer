# MarkdownViewer Features

This document demonstrates all features of MarkdownViewer v2.0.0.

## Navigation

Test the navigation features:
- [Back to test-links.md](test-links.md)
- [Go to test.md](test.md)
- [View README](README.md)
- [Visit subfolder (if exists)](markdown-viewer/README.md)

## Themes

MarkdownViewer supports 4 themes:
1. **Dark** - VS Code inspired dark theme
2. **Standard** - Modern clean light theme
3. **Solarized** - Eye-friendly Solarized Light
4. **Dräger** - Corporate theme

Switch themes via:
- StatusBar dropdown (bottom right)
- Right-click context menu

## Languages

8 languages supported:
- 🇬🇧 English
- 🇩🇪 Deutsch (German)
- 🇲🇳 Монгол (Mongolian)
- 🇫🇷 Français (French)
- 🇪🇸 Español (Spanish)
- 🇯🇵 日本語 (Japanese)
- 🇨🇳 简体中文 (Chinese)
- 🇷🇺 Русский (Russian)

## Syntax Highlighting

```javascript
function greet(name) {
    console.log(`Hello, ${name}!`);
}

greet("MarkdownViewer");
```

```python
def fibonacci(n):
    if n <= 1:
        return n
    return fibonacci(n-1) + fibonacci(n-2)

print(fibonacci(10))
```

```csharp
public class MainPresenter
{
    private readonly IMainView _view;

    public MainPresenter(IMainView view)
    {
        _view = view;
    }
}
```

## Math Formulas (KaTeX)

Inline: $E = mc^2$

Block:
$$
\int_{-\infty}^{\infty} e^{-x^2} dx = \sqrt{\pi}
$$

Quadratic formula:
$$
x = \frac{-b \pm \sqrt{b^2 - 4ac}}{2a}
$$

## Tables

| Feature | Version | Status |
|---------|---------|--------|
| Markdown Rendering | 1.0.0 | ✅ |
| Themes | 1.2.0 | ✅ |
| Localization | 1.3.0 | ✅ |
| Navigation & Search | 1.4.0 | ✅ |
| MVP Architecture | 2.0.0 | ✅ |

## Blockquotes

> "The best way to predict the future is to invent it."
>
> — Alan Kay

> **Note:** This is an important note!

## Links

External links open in browser:
- [Google](https://www.google.com)
- [GitHub](https://github.com)
- [Microsoft](https://www.microsoft.com)

## Keyboard Shortcuts

| Shortcut | Action |
|----------|--------|
| `F5` | Reload file |
| `Ctrl+F` | Open search |
| `F3` | Next match |
| `Shift+F3` | Previous match |
| `Alt+Left` | Navigate back |
| `Alt+Right` | Navigate forward |
| `Ctrl+Mouse Wheel` | Zoom in/out |

## Search Test

Search for these keywords to test the search functionality:
- **MVP** - Model-View-Presenter pattern
- **Theme** - Theme switching
- **Navigation** - Back/forward navigation
- **Testability** - Unit testing support

## Anchor Navigation

Jump to sections:
- [Go to Languages](#languages)
- [Go to Syntax Highlighting](#syntax-highlighting)
- [Go to Math Formulas](#math-formulas-katex)
- [Go to Keyboard Shortcuts](#keyboard-shortcuts)
