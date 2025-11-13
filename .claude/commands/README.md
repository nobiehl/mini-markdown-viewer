# Custom Slash Commands - MarkdownViewer

This directory contains custom slash commands for the MarkdownViewer project.

## Available Commands

### `/roadmap` - Add Feature Idea to ROADMAP

Quickly add a feature idea to `docs/ROADMAP.md` using a background agent.

**Usage:**
```
/roadmap <your feature idea>
```

**Examples:**
```
/roadmap Add support for PDF export
/roadmap Implement dark mode for mobile view
/roadmap Add syntax highlighting for more languages
/roadmap Refactor theme service for better performance
```

**What it does:**
1. Launches a background agent
2. Agent reads current ROADMAP.md
3. Agent finds the best section for your idea
4. Agent adds the idea with proper formatting
5. Agent reports back where it added the idea

**Benefits:**
- ✅ No context pollution in your main conversation
- ✅ Runs in parallel while you continue working
- ✅ Follows ROADMAP.md formatting automatically
- ✅ No need to manually edit the file

**Note:** The command only edits the file, it doesn't commit. You can review and commit later.

---

## How to Create New Commands

1. Create a new `.md` file in `.claude/commands/`
2. Add a description in YAML frontmatter:
   ```markdown
   ---
   description: What this command does
   ---

   Your command prompt here with {parameter} placeholders
   ```
3. Use the command with `/command-name`

See [Claude Code Docs](https://docs.claude.com/claude-code) for more details.
