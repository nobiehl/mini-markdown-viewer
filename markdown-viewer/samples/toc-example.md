# TOC Example

[TOC]

## Introduction

This document demonstrates the auto-generated Table of Contents feature.

The `[TOC]` placeholder above will be automatically replaced with a navigable table of contents that includes all headings in this document.

## Features

### Basic Features

The TOC generator collects all headings (h1-h6) with IDs and creates a hierarchical list.

Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.

### Advanced Features

#### Feature A

Feature A provides enhanced functionality for complex documents.

#### Feature B

Feature B offers additional options for customization.

#### Feature C

Feature C integrates seamlessly with other markdown extensions.

### Performance

The TOC is generated client-side using JavaScript, ensuring fast performance even with large documents.

## Installation

### Prerequisites

Before using this feature, ensure your markdown viewer supports:
- Auto-identifier extension (for heading IDs)
- JavaScript execution
- DOM manipulation

### Steps

1. Add `[TOC]` placeholder where you want the table of contents
2. Write your document with proper headings
3. The TOC will be generated automatically on page load

## Usage Examples

### Simple Document

For simple documents, just add `[TOC]` at the top:

```markdown
# My Document

[TOC]

## Section 1
Content here...

## Section 2
Content here...
```

### Complex Document

For complex documents with multiple heading levels:

```markdown
# Main Title

[TOC]

## Chapter 1
### Section 1.1
#### Subsection 1.1.1
#### Subsection 1.1.2
### Section 1.2

## Chapter 2
### Section 2.1
```

## Styling

The TOC uses the following CSS classes:
- `.table-of-contents` - Main container
- `.toc-title` - "Table of Contents" title
- `.toc-list` - List container
- `.toc-link` - Individual links

These classes inherit colors from the current theme.

## Edge Cases

### Empty Document

If there are no headings, the `[TOC]` placeholder is simply removed.

### Single Heading

Even with just one heading, a TOC will be generated.

### Non-Sequential Levels

The generator handles non-sequential heading levels gracefully (e.g., H1 → H3 → H2).

## Conclusion

The auto-generated Table of Contents makes navigating large documents much easier. It automatically adapts to your document structure and supports all heading levels.

### Benefits

- Automatic generation
- Theme-aware styling
- Smooth navigation
- No manual maintenance required

### Future Enhancements

Potential improvements could include:
- Collapsible sections
- Active section highlighting
- Custom title support
- Exclude certain headings

## References

For more information about markdown extensions, see:
- Markdown specification
- MarkdownViewer documentation
- Markdig library documentation
