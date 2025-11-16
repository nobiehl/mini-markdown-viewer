# Contributing to MarkdownViewer

Thank you for your interest in contributing to MarkdownViewer! This document provides guidelines and instructions for contributing to the project.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [How Can I Contribute?](#how-can-i-contribute)
- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Coding Guidelines](#coding-guidelines)
- [Pull Request Process](#pull-request-process)
- [Reporting Bugs](#reporting-bugs)
- [Suggesting Features](#suggesting-features)

## Code of Conduct

This project adheres to a code of conduct that all contributors are expected to follow. Please be respectful and constructive in all interactions.

## How Can I Contribute?

### Reporting Bugs

Before creating a bug report:
1. Check the [existing issues](https://github.com/nobiehl/mini-markdown-viewer/issues) to avoid duplicates
2. Test with the latest version
3. Gather relevant information (OS version, .NET version, steps to reproduce)

When creating a bug report, include:
- **Clear title** describing the issue
- **Steps to reproduce** the problem
- **Expected behavior** vs **actual behavior**
- **Screenshots** if applicable
- **Environment details**: Windows version, .NET version, MarkdownViewer version
- **Log files** from `%APPDATA%\MarkdownViewer\logs\` if relevant

### Suggesting Features

Feature requests are welcome! Before suggesting:
1. Check [ROADMAP.md](docs/ROADMAP.md) for planned features
2. Search [existing issues](https://github.com/nobiehl/mini-markdown-viewer/issues) for similar requests

When suggesting a feature:
- **Describe the problem** you're trying to solve
- **Explain your proposed solution**
- **Consider alternatives** you've thought about
- **Add examples** of how the feature would be used

### Contributing Code

1. **Fork the repository**
2. **Create a feature branch**: `git checkout -b feature/my-new-feature`
3. **Make your changes** following the coding guidelines
4. **Add tests** for new functionality
5. **Update documentation** (README, CHANGELOG, GLOSSARY)
6. **Commit your changes** using [Conventional Commits](https://www.conventionalcommits.org/)
7. **Push to your fork**: `git push origin feature/my-new-feature`
8. **Open a Pull Request**

## Development Setup

### Prerequisites

- **Windows 10/11** (64-bit)
- **.NET 8.0 SDK** or later
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** for version control

### Clone and Build

```bash
# Clone the repository
git clone https://github.com/nobiehl/mini-markdown-viewer.git
cd mini-markdown-viewer

# Build the solution
cd markdown-viewer
dotnet build

# Run tests
cd MarkdownViewer.Tests
dotnet test

# Run the application
cd ../MarkdownViewer
dotnet run
```

For detailed development instructions, see [DEVELOPMENT.md](docs/DEVELOPMENT.md).

## Project Structure

```
MarkdownViewer/
├── markdown-viewer/
│   ├── MarkdownViewer/          # WinForms UI layer
│   ├── MarkdownViewer.Core/     # Business logic library
│   └── MarkdownViewer.Tests/    # Unit and integration tests
├── docs/                        # Documentation
│   ├── ARCHITECTURE.md          # System architecture
│   ├── CHANGELOG.md             # Version history
│   ├── GLOSSARY.md              # Terms and concepts
│   └── ...
├── Themes/                      # Theme definitions (JSON)
└── README.md                    # Main project documentation
```

### Key Components

- **MarkdownViewer.Core**: Platform-independent business logic (MVP pattern)
  - `Services/`: ThemeService, SettingsService, LocalizationService
  - `Core/`: MarkdownRenderer, FileWatcherManager, LinkNavigationHelper
  - `Models/`: Theme, AppSettings, GitHubRelease
  - `Presenters/`: MainPresenter, NavigationPresenter, etc.

- **MarkdownViewer**: WinForms UI implementation
  - `UI/`: NavigationBar, SearchBar, StatusBarControl, RawDataViewPanel
  - `MainForm.cs`: Main application window
  - `UpdateChecker.cs`: Auto-update functionality

For detailed architecture, see [ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Coding Guidelines

### C# Style

- Follow [C# Coding Conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use **PascalCase** for public members, **camelCase** for private fields
- Add **XML documentation comments** (`///`) for all public APIs
- Keep methods focused and concise (prefer <50 lines)
- Use **meaningful names** (avoid abbreviations)

### Code Quality

- **Write tests** for new functionality (aim for 80%+ coverage)
- **No warnings**: Build must complete with 0 warnings
- **Handle errors gracefully**: Use try-catch and log exceptions
- **Use logging**: Leverage Serilog for diagnostic output
- **Avoid hardcoded strings**: Use `LocalizationService` for UI text

### Documentation

When adding/modifying features:
1. **Update README.md** if user-facing
2. **Add entry to CHANGELOG.md** under `[Unreleased]`
3. **Add/update GLOSSARY.md** entries for new classes
4. **Update ROADMAP.md** if completing planned features
5. **Add XML comments** to public APIs

### Commit Messages

Use [Conventional Commits](https://www.conventionalcommits.org/) format:

```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style/formatting
- `refactor`: Code refactoring
- `test`: Adding/updating tests
- `chore`: Maintenance tasks

**Examples:**
```
feat(themes): Add Dracula theme

Added new dark theme based on Dracula color scheme.
Includes theme file and integration tests.

Closes #123
```

```
fix(update): Fix version comparison for pre-releases

Version comparison now correctly handles pre-release tags
like "1.0.0-beta" vs "1.0.0".

Fixes #456
```

## Pull Request Process

### Before Submitting

- ✅ All tests pass (`dotnet test`)
- ✅ No build warnings
- ✅ Documentation updated
- ✅ CHANGELOG.md updated (under `[Unreleased]`)
- ✅ Code follows style guidelines
- ✅ Commits follow Conventional Commits format

### PR Description Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix (non-breaking change)
- [ ] New feature (non-breaking change)
- [ ] Breaking change (fix or feature that would cause existing functionality to change)
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code follows style guidelines
- [ ] Self-review completed
- [ ] Documentation updated
- [ ] CHANGELOG.md updated
- [ ] No new warnings introduced
- [ ] Tests pass locally
```

### Review Process

1. **Automated checks** must pass (build, tests)
2. **Code review** by maintainer(s)
3. **Address feedback** if any
4. **Approval** required before merge
5. **Squash and merge** (maintainer will handle)

## Reporting Bugs

### Before Reporting

1. **Search existing issues**: Your bug may already be reported
2. **Try latest version**: Bug might already be fixed
3. **Check logs**: `%APPDATA%\MarkdownViewer\logs\log-YYYYMMDD.txt`

### Bug Report Template

```markdown
**Describe the bug**
Clear description of what happened

**To Reproduce**
Steps to reproduce:
1. Open file '...'
2. Click on '...'
3. See error

**Expected behavior**
What you expected to happen

**Screenshots**
If applicable, add screenshots

**Environment:**
 - OS: [e.g., Windows 11 23H2]
 - .NET Version: [e.g., 8.0.11]
 - MarkdownViewer Version: [e.g., 1.11.0]

**Additional context**
Log file contents or any other relevant information
```

## Suggesting Features

### Feature Request Template

```markdown
**Is your feature request related to a problem?**
Clear description of the problem

**Describe the solution you'd like**
How you envision the feature working

**Describe alternatives you've considered**
Other approaches you've thought about

**Additional context**
Screenshots, mockups, or examples from other apps
```

### Feature Development Process

1. **Discuss in issue** first for major features
2. **Check ROADMAP.md** for planned features
3. **Follow PROCESS-MODEL.md** for implementation phases
4. **Update documentation** thoroughly

## Resources

- [DEVELOPMENT.md](docs/DEVELOPMENT.md) - Developer guide
- [ARCHITECTURE.md](docs/ARCHITECTURE.md) - System architecture
- [PROCESS-MODEL.md](docs/PROCESS-MODEL.md) - Development workflow
- [RELEASE-CHECKLIST.md](docs/RELEASE-CHECKLIST.md) - Release process
- [TESTING-CHECKLIST.md](docs/TESTING-CHECKLIST.md) - Test scenarios

## Questions?

- Open a [Discussion](https://github.com/nobiehl/mini-markdown-viewer/discussions) for general questions
- Open an [Issue](https://github.com/nobiehl/mini-markdown-viewer/issues) for bugs or features
- Check [README.md](README.md) for project overview

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing to MarkdownViewer! 🎉
