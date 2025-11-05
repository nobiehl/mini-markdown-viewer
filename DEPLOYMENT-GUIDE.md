# Complete Development & Deployment Guide - MarkdownViewer

Comprehensive guide covering the entire development, build, test, and deployment process to GitHub.

## Table of Contents
- [Prerequisites](#prerequisites)
- [Development Environment Setup](#development-environment-setup)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Building the Application](#building-the-application)
- [Testing](#testing)
- [Git and GitHub Setup](#git-and-github-setup)
- [Deployment to GitHub](#deployment-to-github)
- [Release Process](#release-process)
- [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Software

1. **.NET 8 SDK**
   - Download: https://dotnet.microsoft.com/download/dotnet/8.0
   - Verify installation:
     ```bash
     dotnet --version
     # Should output: 8.0.x
     ```

2. **Git for Windows**
   - Download: https://git-scm.com/download/win
   - Or install via winget:
     ```bash
     winget install Git.Git
     ```
   - Verify installation:
     ```bash
     git --version
     ```

3. **GitHub CLI (gh)**
   - Install via winget:
     ```bash
     winget install GitHub.cli
     ```
   - Verify installation:
     ```bash
     "C:\Program Files\GitHub CLI\gh.exe" --version
     ```

4. **Text Editor / IDE** (optional but recommended)
   - Visual Studio 2022
   - Visual Studio Code
   - JetBrains Rider

5. **WebView2 Runtime**
   - Pre-installed on Windows 10/11
   - If missing: https://developer.microsoft.com/en-us/microsoft-edge/webview2/

### GitHub Account Requirements
- Active GitHub account
- Personal Access Token (PAT) with `repo` permissions

---

## Development Environment Setup

### 1. Git Configuration

```bash
# Set global user information
git config --global user.name "Your Name"
git config --global user.email "your.email@example.com"

# Verify configuration
git config --list
```

### 2. GitHub CLI Authentication

#### Step 1: Create Personal Access Token
1. Go to: https://github.com/settings/tokens
2. Click "Generate new token" → "Generate new token (classic)"
3. Select scopes:
   - ✅ `repo` (Full control of private repositories)
   - ✅ `workflow` (Update GitHub Action workflows)
4. Generate token and copy it (you won't see it again!)

#### Step 2: Configure GitHub CLI
```bash
# Add to ~/.bashrc for persistent authentication
echo 'export GITHUB_TOKEN=your_token_here' >> ~/.bashrc

# Or set for current session
export GITHUB_TOKEN=your_token_here

# Authenticate with GitHub CLI
"C:\Program Files\GitHub CLI\gh.exe" auth login --with-token <<< $GITHUB_TOKEN

# Verify authentication
"C:\Program Files\GitHub CLI\gh.exe" auth status
```

### 3. Project Directory Setup

```bash
# Create project directory
mkdir -p C:\develop\workspace\misc
cd C:\develop\workspace\misc

# Initialize git repository (if not already done)
git init
```

---

## Project Structure

```
mini-markdown-viewer/
├── markdown-viewer/
│   └── MarkdownViewer/
│       ├── MarkdownViewer.csproj    # Project file
│       ├── Program.cs               # Entry point, CLI, registry, update testing
│       ├── MainForm.cs              # Main window, rendering
│       ├── UpdateChecker.cs         # Update checking & installation logic
│       ├── UpdateConfiguration.cs   # Update configuration (prod/test mode)
│       ├── GitHubRelease.cs         # GitHub API models
│       ├── bin/                     # Build output (ignored)
│       └── obj/                     # Build temp files (ignored)
├── bin-single/                      # Single-file output (ignored)
│   └── MarkdownViewer.exe          # Portable executable
├── test-data/                       # Test data for update mechanism
│   ├── README.md                   # Test data documentation
│   └── api-responses/              # Mock GitHub API responses
│       ├── update-available.json   # New version scenario
│       ├── no-update.json          # Already on latest
│       ├── malformed.json          # Invalid JSON
│       ├── rate-limit.json         # API rate limit
│       └── prerelease.json         # Beta version
├── .cache/                          # WebView2 cache (ignored)
├── logs/                            # Application logs (ignored)
│   ├── viewer-YYYYMMDD.log         # Daily log files
│   └── last-update-check.txt       # Last update check timestamp
├── test-diagrams.md                 # Diagram test file
├── test-features.md                 # Feature test file
├── test-plantuml-encoding.html      # PlantUML encoding tests
├── test-update.ps1                  # Automated update test suite
├── .gitignore                       # Git exclusions
├── LICENSE                          # MIT License
├── README.md                        # User documentation
├── DEVELOPMENT.md                   # Technical documentation
└── DEPLOYMENT-GUIDE.md              # This file
```

### Key Files Explained

#### `MarkdownViewer.csproj`
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Markdig" Version="0.37.0" />
    <PackageReference Include="Microsoft.Web.WebView2" Version="1.0.2420.47" />
    <PackageReference Include="Serilog" Version="4.0.0" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.0" />
  </ItemGroup>
</Project>
```

#### Update Mechanism Files

**`UpdateChecker.cs`** (437 lines)
- Core update checking logic
- Async methods for GitHub API communication
- Download and installation management
- Safe update with backup/rollback
- Test mode support (reads local JSON files)

**`UpdateConfiguration.cs`** (177 lines)
- Singleton pattern for global configuration
- Production mode: GitHub API
- Test mode: Local JSON mock data
- Environment variable support

**`GitHubRelease.cs`** (120 lines)
- JSON models for GitHub Releases API
- `GitHubRelease` class
- `GitHubReleaseAsset` class
- `UpdateInfo` class

**`test-update.ps1`** (155 lines)
- Automated PowerShell test suite
- 4 test scenarios
- Exit code validation
- Detailed test reporting

#### `.gitignore`
```
# Build artifacts
bin/
obj/
bin-single/

# WebView2 cache
.cache/

# IDE files
.vs/
.vscode/
*.user
*.suo
```

---

## Development Workflow

### 1. Making Code Changes

#### Program.cs
- Entry point and CLI argument handling
- Windows Registry integration (--install, --uninstall)
- File association management

Key areas:
- `Main()`: CLI argument parsing
- `InstallFileAssociation()`: Registry setup
- `UninstallFileAssociation()`: Registry cleanup
- `Version` constant: Update for releases

#### MainForm.cs
- Main application window
- Markdown to HTML conversion
- WebView2 initialization and rendering
- File watching for live-reload

Key areas:
- `InitializeComponents()`: Window setup, title with version
- `LoadMarkdownFile()`: File loading and title update
- `ConvertMarkdownToHtml()`: Markdown processing, diagram rendering
- PlantUML encoding logic (lines 267-302)
- `Version` constant: Keep in sync with Program.cs

### 2. Version Management

**CRITICAL:** Version must be updated in 3 places for every release:

1. **Program.cs**:
   ```csharp
   private const string Version = "1.0.3";
   ```

2. **MainForm.cs**:
   ```csharp
   private const string Version = "1.0.3";
   ```

3. **README.md**:
   ```markdown
   ![Version](https://img.shields.io/badge/version-1.0.3-blue)
   ```

### 3. Code Style Guidelines

- Use XML documentation comments for public methods
- Follow C# naming conventions (PascalCase for methods, camelCase for private fields)
- Keep methods focused and under 50 lines when possible
- Use descriptive variable names
- Add comments for complex logic (e.g., PlantUML encoding)

---

## Building the Application

### Debug Build (Development)

```bash
cd markdown-viewer/MarkdownViewer

# Build
dotnet build -c Debug

# Run
dotnet run -- test-diagrams.md
```

### Release Build (Production)

```bash
cd markdown-viewer/MarkdownViewer

# Single-file executable (recommended)
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single

# Output: ../../bin-single/MarkdownViewer.exe (1.6 MB)
```

### Build Configuration Explained

- `-c Release`: Optimized release build
- `-r win-x64`: Target Windows 64-bit
- `--self-contained false`: Requires .NET 8 Runtime (smaller executable)
- `-p:PublishSingleFile=true`: Single portable .exe
- `-o ../../bin-single`: Output to bin-single/ directory

### Alternative: Self-Contained Build

```bash
# Includes .NET Runtime (larger, ~70 MB)
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o ../../bin-single
```

---

## Testing

### 1. Local Testing Checklist

**IMPORTANT:** Test ALL features before committing or releasing!

#### Basic Features
```bash
# Test file opening dialog
bin-single/MarkdownViewer.exe

# Test direct file opening
bin-single/MarkdownViewer.exe test-diagrams.md
bin-single/MarkdownViewer.exe test-features.md

# Test --install (requires admin prompt)
bin-single/MarkdownViewer.exe --install

# Test double-click (after --install)
# Double-click any .md file in Windows Explorer

# Test --uninstall
bin-single/MarkdownViewer.exe --uninstall
```

#### Feature Testing

**✅ Mermaid Diagrams:**
- Open `test-diagrams.md`
- Verify flowchart renders
- Verify sequence diagram renders

**✅ PlantUML Diagrams:**
- Open `test-diagrams.md`
- Verify class diagram renders (NOT just placeholder!)
- Verify sequence diagram renders (NOT just placeholder!)
- **Check internet connection** (PlantUML requires online server)

**✅ Base64 Images:**
- Open `test-features.md`
- Verify embedded images display (red pixel, blue pixel, SVG circle)

**✅ Code Syntax Highlighting:**
- Open `test-features.md`
- Verify C#, Python, JavaScript code has syntax highlighting
- Verify copy buttons appear on hover

**✅ Live Reload:**
- Open any .md file
- Edit and save the file
- Viewer should automatically reload

**✅ Version Display:**
- Check window title shows: "filename.md - Markdown Viewer v1.0.x"

### 2. PlantUML Encoding Test

For troubleshooting PlantUML issues, use the test HTML:

```bash
# Open test HTML in browser
start test-plantuml-encoding.html
```

This tests 4 encoding methods:
1. ✅ HEX Encoding (~h) - Currently used
2. ✅ plantuml-encoder library
3. ❌ Plain Base64 - Known to fail
4. ✅ pako + custom encode64

All methods except #3 should show green ✅.

### 3. Update Mechanism Testing

**Automated Test Suite:**
```bash
# Run all update test scenarios
.\test-update.ps1

# Run specific scenario
.\test-update.ps1 -Scenario update-available
.\test-update.ps1 -Scenario no-update
.\test-update.ps1 -Scenario malformed
.\test-update.ps1 -Scenario prerelease
```

**Expected Results:**
- ✅ All 4 tests should pass
- ✅ Exit codes: 0 for updates available, 1 for no update

**Manual Update Testing:**
```bash
# Test manual update check
bin-single/MarkdownViewer.exe --update

# Test update check with file opening
bin-single/MarkdownViewer.exe README.md --update

# Test individual scenarios
bin-single/MarkdownViewer.exe --test-update --test-scenario update-available
bin-single/MarkdownViewer.exe --test-update --test-scenario no-update
```

**Update Installation Testing:**
1. Create test update file: `echo test > bin-single/pending-update.exe`
2. Start application
3. Verify backup/rollback behavior

**Test Data:**
- Located in `test-data/api-responses/`
- 5 scenarios: update-available, no-update, malformed, rate-limit, prerelease
- Modify JSON files to test different versions

### 4. Regression Testing

Before each release, test:
- [ ] All Mermaid diagram types (flowchart, sequence, class)
- [ ] All PlantUML diagram types (class, sequence)
- [ ] Base64 images (PNG, SVG)
- [ ] External images (requires internet)
- [ ] Syntax highlighting (C#, Python, JavaScript, Bash)
- [ ] Tables (simple, aligned)
- [ ] Lists (ordered, unordered, task lists)
- [ ] Links (external, internal anchors)
- [ ] Live reload
- [ ] Windows Explorer integration (if --install was run)
- [ ] **Update mechanism (automated test suite)**
- [ ] **Update check (manual --update parameter)**

---

## Git and GitHub Setup

### 1. Initialize Repository

```bash
cd C:\develop\workspace\misc

# Initialize git
git init

# Create .gitignore
cat > .gitignore << 'EOF'
# Build artifacts
bin/
obj/
bin-single/

# WebView2 cache
.cache/

# IDE files
.vs/
.vscode/
*.user
*.suo
EOF

# Add all files
git add .

# Initial commit
git commit -m "Initial commit: MarkdownViewer v1.0.0"
```

### 2. Create GitHub Repository

```bash
# Create repository (replace 'nobiehl' with your username)
"C:\Program Files\GitHub CLI\gh.exe" repo create mini-markdown-viewer --public --source=. --remote=origin

# Verify remote
git remote -v
# Should show:
# origin  https://github.com/yourusername/mini-markdown-viewer.git (fetch)
# origin  https://github.com/yourusername/mini-markdown-viewer.git (push)

# Push initial commit
git push -u origin master
```

### 3. Update Repository Description

```bash
"C:\Program Files\GitHub CLI\gh.exe" repo edit --description "Lightweight Windows Desktop Markdown viewer with Mermaid & PlantUML support"
```

---

## Deployment to GitHub

### Standard Git Workflow

```bash
# 1. Check status
git status

# 2. Stage changes
git add <files>
# Or stage all:
git add .

# 3. Commit with descriptive message
git commit -m "Description of changes"

# 4. Push to GitHub
git push
```

### Commit Message Guidelines

**Good commit messages:**
```
✅ "Fix PlantUML rendering with HEX encoding"
✅ "Add version display in window title"
✅ "Update README.md with installation instructions"
✅ "Refactor HTML generation for better maintainability"
```

**Bad commit messages:**
```
❌ "fix bug"
❌ "update"
❌ "changes"
❌ "wip"
```

### Commit Message Template

```bash
git commit -m "$(cat <<'EOF'
<Short summary line (50 chars max)>

<Detailed description of what changed and why>

Changes:
- <Change 1>
- <Change 2>
- <Change 3>

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
EOF
)"
```

---

## Release Process

### Pre-Release Checklist

**DO NOT SKIP THESE STEPS!**

- [ ] All features tested locally and verified working
- [ ] PlantUML diagrams render correctly (not just placeholders)
- [ ] Mermaid diagrams render correctly
- [ ] **Update test suite passes (`.\test-update.ps1` → 4/4 tests pass)**
- [ ] **Manual update check works (`--update` parameter)**
- [ ] Version updated in 3 places (Program.cs, MainForm.cs, README.md)
- [ ] **Update test data versions updated (test-data/api-responses/*.json)**
- [ ] Build successful with no errors
- [ ] README.md updated with new features/fixes
- [ ] DEPLOYMENT-GUIDE.md updated (if process changed)
- [ ] Git working directory clean (`git status` shows no uncommitted changes)
- [ ] Previous release v1.0.x downloaded and compared

### Step-by-Step Release Process

#### Step 1: Update Version Numbers

```bash
# Edit these 3 files:
# 1. markdown-viewer/MarkdownViewer/Program.cs (line 15)
# 2. markdown-viewer/MarkdownViewer/MainForm.cs (line 17)
# 3. README.md (line 5)

# Change version from X.Y.Z to X.Y.Z+1

# Verify consistency
grep "1.0.4" markdown-viewer/MarkdownViewer/Program.cs
grep "1.0.4" markdown-viewer/MarkdownViewer/MainForm.cs
grep "1.0.4" README.md
```

#### Step 2: Build Release Executable

```bash
cd markdown-viewer/MarkdownViewer

# Clean previous builds
dotnet clean

# Build release
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single

# Return to root
cd ../..

# Verify executable exists
ls -lh bin-single/MarkdownViewer.exe
```

#### Step 3: Test Release Build Thoroughly

```bash
# Test 1: Basic functionality
bin-single/MarkdownViewer.exe test-diagrams.md

# Verify in opened window:
# ✅ Title shows: "test-diagrams.md - Markdown Viewer v1.0.4"
# ✅ Mermaid flowchart renders
# ✅ Mermaid sequence diagram renders
# ✅ PlantUML class diagram renders (NOT placeholder!)
# ✅ PlantUML sequence diagram renders (NOT placeholder!)

# Close window

# Test 2: Feature tests
bin-single/MarkdownViewer.exe test-features.md

# Verify:
# ✅ Base64 images display (red, blue pixels, SVG circle)
# ✅ Syntax highlighting works (C#, Python, JavaScript)
# ✅ Copy buttons appear on code blocks
# ✅ Tables render correctly
# ✅ Links are clickable

# Close window

# Test 3: Command-line options
bin-single/MarkdownViewer.exe --version
# Should show: "Markdown Viewer v1.0.4"

bin-single/MarkdownViewer.exe --help
# Should show help text

# Test 4: Update mechanism (automated)
.\test-update.ps1

# Expected output:
# ✅ All tests passed!
# Total: 4
# Passed: 4
# Failed: 0

# Test 5: Manual update check
bin-single/MarkdownViewer.exe --update
# Should show: "Sie verwenden bereits die neueste Version" or update available dialog
```

#### Step 4: Commit Changes

```bash
# Stage version changes only
git add README.md markdown-viewer/MarkdownViewer/Program.cs markdown-viewer/MarkdownViewer/MainForm.cs

# Check what will be committed
git diff --staged

# Commit with detailed message
git commit -m "$(cat <<'EOF'
Release v1.0.4: <One-line summary>

<Detailed description of what changed>

Changes:
- <Change 1 with details>
- <Change 2 with details>
- <Change 3 with details>

Testing:
- Verified PlantUML rendering with test-diagrams.md
- Verified Mermaid rendering with test-diagrams.md
- Verified all features with test-features.md
- Verified version display in window title

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
EOF
)"

# Verify commit
git log -1 --stat
```

#### Step 5: Push to GitHub

```bash
# Push to remote
git push

# Verify push succeeded
git status
# Should show: "Your branch is up to date with 'origin/master'"
```

#### Step 6: Create GitHub Release

```bash
# Create release with executable attachment
"C:\Program Files\GitHub CLI\gh.exe" release create v1.0.4 \
  bin-single/MarkdownViewer.exe \
  --title "v1.0.4 - <Short Title>" \
  --notes "$(cat <<'EOF'
## v1.0.4 - <Descriptive Title>

<Introduction paragraph explaining the release>

### What's New
- <New feature 1>
- <New feature 2>

### What's Fixed
- <Bug fix 1>
- <Bug fix 2>

### What's Changed
- <Change 1>
- <Change 2>

### Installation
Download `MarkdownViewer.exe` and run:
```bash
.\MarkdownViewer.exe --install
```

Then double-click any .md file to open it in the viewer.

### Testing
Test PlantUML and Mermaid diagrams:
```bash
.\MarkdownViewer.exe test-diagrams.md
```

Test all features:
```bash
.\MarkdownViewer.exe test-features.md
```

### Requirements
- Windows 10/11
- .NET 8 Runtime (usually pre-installed)
- Internet connection (for PlantUML diagrams)

---
🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"
```

#### Step 7: Verify Release

```bash
# View release in terminal
"C:\Program Files\GitHub CLI\gh.exe" release view v1.0.4

# Open release page in browser
"C:\Program Files\GitHub CLI\gh.exe" release view v1.0.4 --web

# Verify checklist:
# ✅ Release title is correct
# ✅ Release notes are formatted correctly
# ✅ Version number matches (v1.0.4)
# ✅ MarkdownViewer.exe is attached
# ✅ File size is ~1.6 MB
# ✅ Release is marked as "Latest"
```

#### Step 8: Download and Test Release Build

```bash
# Download the released executable to temp directory
cd /tmp
"C:\Program Files\GitHub CLI\gh.exe" release download v1.0.4 -p "MarkdownViewer.exe"

# Test downloaded executable
./MarkdownViewer.exe --version
# Should show: "Markdown Viewer v1.0.4"

# Clean up
rm MarkdownViewer.exe
cd -
```

### Version Numbering Scheme

Follow **Semantic Versioning (SemVer)**: `MAJOR.MINOR.PATCH`

- **MAJOR** (X.0.0): Breaking changes, incompatible API changes
- **MINOR** (1.X.0): New features, backwards compatible
- **PATCH** (1.0.X): Bug fixes, backwards compatible

**Examples:**
- `1.0.0` → Initial release
- `1.0.1` → Bug fix (PlantUML attempt #1)
- `1.0.2` → Bug fix (PlantUML attempt #2)
- `1.0.3` → Bug fix + minor feature (PlantUML HEX + version in title)
- `1.1.0` → New feature (e.g., dark mode theme)
- `1.2.0` → Another new feature (e.g., export to PDF)
- `2.0.0` → Breaking change (e.g., require .NET 9, change CLI arguments)

**When to increment:**
- **PATCH**: Fix PlantUML encoding, fix crash, fix UI bug
- **MINOR**: Add dark mode, add new diagram type, add settings
- **MAJOR**: Require .NET 9, change from WinForms to WPF, remove --install flag

---

## Troubleshooting

### Build Issues

#### Problem: "SDK not found"
```
error MSB4236: The SDK 'Microsoft.NET.Sdk' specified could not be found.
```
**Solution:**
```bash
# Install .NET 8 SDK
winget install Microsoft.DotNet.SDK.8

# Verify installation
dotnet --version
# Should output: 8.0.x
```

#### Problem: "WebView2 package not found"
```
error NU1101: Unable to find package Microsoft.Web.WebView2
```
**Solution:**
```bash
# Restore NuGet packages
cd markdown-viewer/MarkdownViewer
dotnet restore

# If still fails, clear NuGet cache
dotnet nuget locals all --clear
dotnet restore
```

#### Problem: Build warnings about nullable types
```
warning CS8618: Non-Nullable-Feld "webView" muss beim Beenden des Konstruktors einen Wert ungleich NULL enthalten
```
**Solution:**
These are warnings, not errors. The code is safe because the fields are initialized in `InitializeComponents()`. To suppress:
```csharp
// Add to MainForm.cs
#pragma warning disable CS8618
private WebView2 webView;
private FileSystemWatcher fileWatcher;
#pragma warning restore CS8618
```

#### Problem: "Executable is 70 MB instead of 1.6 MB"
**Solution:**
You built with `--self-contained true`. Use:
```bash
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single
```

### Git Issues

#### Problem: "fatal: not a git repository"
**Solution:**
```bash
# Initialize git in project root
cd C:\develop\workspace\misc
git init
```

#### Problem: "Permission denied (publickey)"
**Solution:**
```bash
# Use HTTPS with GitHub CLI token instead of SSH
"C:\Program Files\GitHub CLI\gh.exe" auth login

# Or add token to ~/.bashrc
echo 'export GITHUB_TOKEN=your_token_here' >> ~/.bashrc
source ~/.bashrc
```

#### Problem: "remote: Repository not found"
**Solution:**
```bash
# Verify GitHub username
"C:\Program Files\GitHub CLI\gh.exe" auth status

# Check remote URL
git remote -v

# Update remote URL (replace 'nobiehl' with your username)
git remote set-url origin https://github.com/nobiehl/mini-markdown-viewer.git
```

#### Problem: "Your branch is behind 'origin/master'"
**Solution:**
```bash
# Pull latest changes
git pull

# If conflicts, resolve manually then:
git add <resolved-files>
git commit
git push
```

### GitHub CLI Issues

#### Problem: "gh: command not found"
**Solution:**
```bash
# Install GitHub CLI
winget install GitHub.cli

# Restart terminal, then use full path
"C:\Program Files\GitHub CLI\gh.exe" --version
```

#### Problem: "gh: authentication failed"
**Solution:**
```bash
# Check authentication status
"C:\Program Files\GitHub CLI\gh.exe" auth status

# Re-authenticate
export GITHUB_TOKEN=your_new_token_here
"C:\Program Files\GitHub CLI\gh.exe" auth login --with-token <<< $GITHUB_TOKEN

# Verify
"C:\Program Files\GitHub CLI\gh.exe" auth status
```

#### Problem: "API rate limit exceeded"
**Solution:**
- **Cause:** GitHub limits unauthenticated requests to 60/hour
- **Solution:** Use authenticated requests (see authentication section)
- **Temporary workaround:** Wait 1 hour

#### Problem: "release create failed: validation failed"
**Solution:**
```bash
# Check if release already exists
"C:\Program Files\GitHub CLI\gh.exe" release list

# Delete existing release (if needed)
"C:\Program Files\GitHub CLI\gh.exe" release delete v1.0.4 -y

# Try again
"C:\Program Files\GitHub CLI\gh.exe" release create v1.0.4 ...
```

### Runtime Issues

#### Problem: "Application failed to start - .NET Runtime not installed"
**Solution:**
- **Cause:** Built with `--self-contained false` but .NET 8 not installed
- **Solution 1:** Install .NET 8 Runtime: https://dotnet.microsoft.com/download/dotnet/8.0
- **Solution 2:** Rebuild with `--self-contained true` (creates larger exe)

#### Problem: "WebView2 Runtime not installed"
**Solution:**
- **Cause:** WebView2 not available (rare on Win10/11)
- **Solution 1:** Update Windows (WebView2 pre-installed)
- **Solution 2:** Download WebView2: https://developer.microsoft.com/en-us/microsoft-edge/webview2/

#### Problem: PlantUML diagrams show placeholder/broken image
**Causes and Solutions:**

1. **No internet connection**
   ```bash
   # PlantUML requires plantuml.com server
   # Test connection:
   curl https://www.plantuml.com/plantuml/png/~h40
   ```

2. **Wrong encoding method**
   ```bash
   # Verify encoding in MainForm.cs uses HEX (~h prefix)
   # Line 281 should have:
   img.src = `https://www.plantuml.com/plantuml/png/~h${hex}`;
   ```

3. **Test encoding methods**
   ```bash
   # Use test HTML to verify which encoding works
   start test-plantuml-encoding.html
   # HEX encoding (Test 1) should show green ✅
   ```

#### Problem: Mermaid diagrams not rendering
**Causes and Solutions:**

1. **CDN blocked**
   ```javascript
   // Check if Mermaid CDN is accessible
   // Line 260 in MainForm.cs:
   import mermaid from 'https://cdn.jsdelivr.net/npm/mermaid@10/dist/mermaid.esm.min.mjs';
   ```

2. **Syntax error in diagram**
   ```markdown
   # Correct:
   ```mermaid
   graph TD
       A --> B
   ```

   # Incorrect:
   ```mermaid
   graph TD
   A -> B  # Wrong arrow syntax
   ```
   ```

3. **Check browser console**
   - Press F12 in viewer
   - Check Console tab for errors

### Registry Issues

#### Problem: "--install failed: Access denied"
**Solutions:**
1. **Use HKEY_CURRENT_USER** (already implemented, no admin needed)
2. **If still fails:** Run as administrator
   - Right-click MarkdownViewer.exe → Run as administrator
   - Then run: `.\MarkdownViewer.exe --install`

#### Problem: Double-click doesn't open in MarkdownViewer
**Solutions:**

1. **Re-run install:**
   ```bash
   bin-single/MarkdownViewer.exe --install
   ```

2. **Manually set default program:**
   - Right-click .md file
   - "Open with" → "Choose another app"
   - "More apps" → "Look for another app on this PC"
   - Browse to MarkdownViewer.exe
   - Check "Always use this app"

3. **Check registry entries:**
   ```bash
   # Open Registry Editor
   regedit

   # Navigate to:
   # HKEY_CURRENT_USER\Software\Classes\.md
   # Should have: (Default) = "MarkdownViewer.Document"
   ```

#### Problem: "Send To" shortcut missing
**Solution:**
```bash
# Re-run install
bin-single/MarkdownViewer.exe --install

# Verify shortcut exists:
# Open: %APPDATA%\Microsoft\Windows\SendTo\
# Should see: "Markdown Viewer.lnk"

# Manual creation (if needed):
# Create shortcut manually and copy to SendTo folder
```

### Testing Issues

#### Problem: "test-diagrams.md not found"
**Solution:**
```bash
# Verify file exists
ls test-diagrams.md

# If missing, download from repository:
"C:\Program Files\GitHub CLI\gh.exe" repo clone nobiehl/mini-markdown-viewer
cd mini-markdown-viewer
```

#### Problem: Can't verify PlantUML works because no internet
**Solution:**
- PlantUML **requires** internet connection (uses plantuml.com server)
- Test on machine with internet
- Or implement local PlantUML server (advanced, not documented here)

---

## Appendix A: Complete Release Example (v1.0.4)

Complete workflow from code change to published release:

```bash
# ========================================
# STEP 1: Update version numbers
# ========================================

# Edit 3 files:
# 1. markdown-viewer/MarkdownViewer/Program.cs: Line 15
#    Change: private const string Version = "1.0.4";
#
# 2. markdown-viewer/MarkdownViewer/MainForm.cs: Line 17
#    Change: private const string Version = "1.0.4";
#
# 3. README.md: Line 5
#    Change: ![Version](https://img.shields.io/badge/version-1.0.4-blue)

# Verify consistency
grep "1.0.4" markdown-viewer/MarkdownViewer/Program.cs
grep "1.0.4" markdown-viewer/MarkdownViewer/MainForm.cs
grep "1.0.4" README.md

# ========================================
# STEP 2: Build release
# ========================================

cd markdown-viewer/MarkdownViewer
dotnet clean
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single
cd ../..

# ========================================
# STEP 3: Test locally (CRITICAL!)
# ========================================

# Test 1: Diagrams
bin-single/MarkdownViewer.exe test-diagrams.md
# ✅ Verify: Title shows "test-diagrams.md - Markdown Viewer v1.0.4"
# ✅ Verify: Mermaid diagrams render
# ✅ Verify: PlantUML diagrams render (NOT placeholders!)

# Test 2: Features
bin-single/MarkdownViewer.exe test-features.md
# ✅ Verify: Base64 images display
# ✅ Verify: Syntax highlighting works
# ✅ Verify: Tables render correctly

# Test 3: CLI
bin-single/MarkdownViewer.exe --version
# ✅ Verify: Shows "Markdown Viewer v1.0.4"

# ========================================
# STEP 4: Commit changes
# ========================================

git add README.md markdown-viewer/MarkdownViewer/Program.cs markdown-viewer/MarkdownViewer/MainForm.cs

git commit -m "$(cat <<'EOF'
Release v1.0.4: Add dark mode theme

Added dark mode theme that automatically switches based on Windows theme.

Changes:
- Added dark mode CSS in MainForm.cs (line 250-280)
- Added theme detection using Windows Registry
- Updated version to 1.0.4 in Program.cs, MainForm.cs, README.md

Testing:
- Verified dark mode works with Windows dark theme
- Verified light mode works with Windows light theme
- Verified PlantUML rendering still works
- Verified Mermaid rendering still works
- Verified all features with test-features.md

🤖 Generated with [Claude Code](https://claude.com/claude-code)

Co-Authored-By: Claude <noreply@anthropic.com>
EOF
)"

# ========================================
# STEP 5: Push to GitHub
# ========================================

git push

# ========================================
# STEP 6: Create release
# ========================================

"C:\Program Files\GitHub CLI\gh.exe" release create v1.0.4 \
  bin-single/MarkdownViewer.exe \
  --title "v1.0.4 - Dark Mode Support" \
  --notes "$(cat <<'EOF'
## v1.0.4 - Dark Mode Support

This release adds automatic dark mode that switches based on your Windows theme preference.

### What's New
- **Dark mode support** 🌙
  - Automatically detects Windows theme (light/dark)
  - Switches between light and dark CSS themes
  - Optimized colors for both themes

### Installation
Download `MarkdownViewer.exe` and run:
```bash
.\MarkdownViewer.exe --install
```

Then double-click any .md file to open it.

### Testing
Test dark mode:
1. Set Windows to dark theme (Settings → Personalization → Colors)
2. Open any .md file
3. Viewer should use dark theme automatically

Test all features:
```bash
.\MarkdownViewer.exe test-diagrams.md
.\MarkdownViewer.exe test-features.md
```

### Requirements
- Windows 10/11
- .NET 8 Runtime
- Internet connection (for PlantUML diagrams)

---
🤖 Generated with [Claude Code](https://claude.com/claude-code)
EOF
)"

# ========================================
# STEP 7: Verify release
# ========================================

# View in terminal
"C:\Program Files\GitHub CLI\gh.exe" release view v1.0.4

# Open in browser
"C:\Program Files\GitHub CLI\gh.exe" release view v1.0.4 --web

# Checklist:
# ✅ Release title correct: "v1.0.4 - Dark Mode Support"
# ✅ Release notes formatted properly
# ✅ MarkdownViewer.exe attached (~1.6 MB)
# ✅ Marked as "Latest release"
# ✅ Version badge in README shows 1.0.4

# ========================================
# STEP 8: Test downloaded release
# ========================================

cd /tmp
"C:\Program Files\GitHub CLI\gh.exe" release download v1.0.4 -p "MarkdownViewer.exe"
./MarkdownViewer.exe --version
# Should show: "Markdown Viewer v1.0.4"
rm MarkdownViewer.exe
cd -

echo "✅ Release v1.0.4 completed successfully!"
```

---

## Appendix B: Quick Reference Commands

### Development
```bash
# Debug build
dotnet build -c Debug

# Run in debug mode
dotnet run -- test-diagrams.md

# Release build
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ../../bin-single
```

### Testing
```bash
# Test executable
bin-single/MarkdownViewer.exe test-diagrams.md

# Test CLI options
bin-single/MarkdownViewer.exe --version
bin-single/MarkdownViewer.exe --help
bin-single/MarkdownViewer.exe --install
bin-single/MarkdownViewer.exe --uninstall

# Test PlantUML encoding
start test-plantuml-encoding.html
```

### Git
```bash
# Status
git status

# Stage changes
git add <files>

# Commit
git commit -m "message"

# Push
git push

# View log
git log --oneline -10
```

### GitHub CLI
```bash
# Authentication
"C:\Program Files\GitHub CLI\gh.exe" auth status
"C:\Program Files\GitHub CLI\gh.exe" auth login

# Repository
"C:\Program Files\GitHub CLI\gh.exe" repo view
"C:\Program Files\GitHub CLI\gh.exe" repo edit --description "description"

# Releases
"C:\Program Files\GitHub CLI\gh.exe" release list
"C:\Program Files\GitHub CLI\gh.exe" release view v1.0.4
"C:\Program Files\GitHub CLI\gh.exe" release view v1.0.4 --web
"C:\Program Files\GitHub CLI\gh.exe" release create v1.0.4 bin-single/MarkdownViewer.exe --title "title" --notes "notes"
"C:\Program Files\GitHub CLI\gh.exe" release download v1.0.4 -p "MarkdownViewer.exe"
"C:\Program Files\GitHub CLI\gh.exe" release delete v1.0.4 -y
```

---

## Appendix C: Resources

### Official Documentation
- **.NET:** https://learn.microsoft.com/en-us/dotnet/
- **Windows Forms:** https://learn.microsoft.com/en-us/dotnet/desktop/winforms/
- **WebView2:** https://learn.microsoft.com/en-us/microsoft-edge/webview2/
- **Markdig:** https://github.com/xoofx/markdig
- **Mermaid:** https://mermaid.js.org/
- **PlantUML:** https://plantuml.com/
- **PlantUML Text Encoding:** https://plantuml.com/text-encoding

### Tools
- **Git:** https://git-scm.com/doc
- **GitHub CLI:** https://cli.github.com/manual/
- **NuGet:** https://www.nuget.org/

### Best Practices
- **Semantic Versioning:** https://semver.org/
- **Conventional Commits:** https://www.conventionalcommits.org/
- **Git Flow:** https://nvie.com/posts/a-successful-git-branching-model/

### Community
- **GitHub Repository:** https://github.com/nobiehl/mini-markdown-viewer
- **GitHub Issues:** https://github.com/nobiehl/mini-markdown-viewer/issues
- **GitHub Discussions:** https://github.com/nobiehl/mini-markdown-viewer/discussions

---

**Document Version:** 1.0
**Last Updated:** 2025-11-05
**Project Version:** 1.0.3
**Author:** nobiehl
**Generated with:** Claude Code

---

**Need help?** Open an issue at: https://github.com/nobiehl/mini-markdown-viewer/issues
