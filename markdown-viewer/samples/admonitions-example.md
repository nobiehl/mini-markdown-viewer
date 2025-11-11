# Admonitions / Callouts Example

Admonitions (also called callouts) are special boxes that highlight important information.

## Note (Information)

::: note
This is a note admonition. Use it for general information and helpful hints.

It can contain **formatted text**, `code`, and even lists:
- Point 1
- Point 2
:::

## Info (Light Bulb)

::: info
This is an info admonition. Use it for tips and insights.

Perfect for highlighting interesting facts!
:::

## Tip (Success)

::: tip
This is a tip admonition. Use it for best practices and recommendations.

**Pro Tip:** Always test your code before committing!
:::

## Warning (Caution)

::: warning
This is a warning admonition. Use it to warn users about potential issues.

Make sure to backup your data before proceeding!
:::

## Danger (Critical)

::: danger
This is a danger admonition. Use it for critical warnings.

**CRITICAL:** Never run `rm -rf /` on a production server!
:::

## Nested Content

::: tip
You can include complex content:

```csharp
public void Example()
{
    Console.WriteLine("Code blocks work!");
}
```

And even math: $E = mc^2$
:::

## Multiple Admonitions

::: note
First note
:::

::: warning
Then a warning
:::

::: tip
Finally a tip
:::

## Real-World Example

::: info
**Documentation Tip:** Always document your public APIs.
:::

::: warning
**Breaking Change:** The `OldMethod()` will be removed in v2.0.
:::

::: danger
**Security Warning:** Never store passwords in plain text!
:::
