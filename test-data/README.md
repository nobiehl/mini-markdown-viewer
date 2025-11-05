# Test Data for Update Mechanism

This directory contains test data for testing the update mechanism without creating real GitHub releases.

## Directory Structure

```
test-data/
├── api-responses/          # Mock GitHub API responses
│   ├── update-available.json    # New version available (v1.0.6)
│   ├── no-update.json           # Current version is latest (v1.0.5)
│   ├── rate-limit.json          # API rate limit response (429)
│   ├── malformed.json           # Invalid JSON structure
│   └── prerelease.json          # Prerelease version (v1.1.0-beta.1)
└── executables/            # Fake executables for download testing
    └── (created on demand)
```

## Usage

### Method 1: Environment Variables

```powershell
# Set test mode
$env:MDVIEWER_TEST_MODE = "1"
$env:MDVIEWER_API_URL = "http://localhost:8080"
$env:MDVIEWER_TEST_DATA = ".\test-data"
$env:MDVIEWER_TEST_SCENARIO = "update-available"

# Run application
.\bin-single\MarkdownViewer.exe README.md

# Clean up
Remove-Item env:MDVIEWER_TEST_MODE
Remove-Item env:MDVIEWER_API_URL
Remove-Item env:MDVIEWER_TEST_DATA
Remove-Item env:MDVIEWER_TEST_SCENARIO
```

### Method 2: Test Parameter

```bash
# Test specific scenario
.\bin-single\MarkdownViewer.exe --test-update --test-scenario update-available
.\bin-single\MarkdownViewer.exe --test-update --test-scenario no-update
.\bin-single\MarkdownViewer.exe --test-update --test-scenario rate-limit
.\bin-single\MarkdownViewer.exe --test-update --test-scenario malformed
.\bin-single\MarkdownViewer.exe --test-update --test-scenario prerelease
```

### Method 3: Automated Test Script

```powershell
# Run all test scenarios
.\test-update.ps1

# Run specific scenario
.\test-update.ps1 -Scenario update-available
```

## Test Scenarios

### 1. update-available
- **Description:** New version (v1.0.6) is available
- **Expected:** Update dialog shown, download option available
- **File:** api-responses/update-available.json

### 2. no-update
- **Description:** Current version (v1.0.5) is the latest
- **Expected:** No update dialog (silent), or "Already up to date" if --update used
- **File:** api-responses/no-update.json

### 3. rate-limit
- **Description:** GitHub API rate limit exceeded (HTTP 429)
- **Expected:** Silent fail, logged as warning
- **File:** api-responses/rate-limit.json

### 4. malformed
- **Description:** Invalid JSON response from API
- **Expected:** Error handled gracefully, logged
- **File:** api-responses/malformed.json

### 5. prerelease
- **Description:** Prerelease version (v1.1.0-beta.1) available
- **Expected:** Update detected (beta > stable), dialog shown
- **File:** api-responses/prerelease.json

## Adding New Scenarios

1. Create a new JSON file in `api-responses/`
2. Follow GitHub API v3 release format
3. Test with: `--test-scenario your-scenario-name`

## Mock Server (Optional)

For more advanced testing, you can run a local HTTP server:

```powershell
# Simple Python HTTP server
cd test-data
python -m http.server 8080
```

Then set `MDVIEWER_API_URL=http://localhost:8080`

## Notes

- Test mode is completely isolated from production
- No real GitHub API calls are made
- No real downloads occur
- Perfect for CI/CD pipeline testing
