# PowerShell Test Script for MarkdownViewer Update Mechanism
# Tests all update scenarios without creating real GitHub releases

param(
    [string]$Scenario = "all",
    [string]$ExePath = ".\bin-single\MarkdownViewer.exe"
)

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MarkdownViewer Update Test Suite" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if executable exists
if (-not (Test-Path $ExePath)) {
    Write-Host "Error: Executable not found: $ExePath" -ForegroundColor Red
    Write-Host "Please build the project first:" -ForegroundColor Yellow
    Write-Host "  cd markdown-viewer\MarkdownViewer" -ForegroundColor Yellow
    Write-Host "  dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ..\..\bin-single" -ForegroundColor Yellow
    exit 1
}

# Set test environment variables
Write-Host "Setting up test environment..." -ForegroundColor Yellow
$env:MDVIEWER_TEST_MODE = "1"
$env:MDVIEWER_API_URL = "http://localhost:8080"
$env:MDVIEWER_TEST_DATA = ".\test-data"

# Define test scenarios
$scenarios = @(
    @{
        Name = "update-available"
        Description = "New version available (v1.2.0)"
        ExpectedResult = "Update detected"
        ShouldPass = $true
    },
    @{
        Name = "no-update"
        Description = "Already on latest version (v1.0.5 < current v1.1.0)"
        ExpectedResult = "No update"
        ShouldPass = $true
    },
    @{
        Name = "malformed"
        Description = "Invalid JSON response"
        ExpectedResult = "Error handled gracefully"
        ShouldPass = $true
    },
    @{
        Name = "prerelease"
        Description = "Prerelease version available (v1.2.0-beta.1)"
        ExpectedResult = "Update detected (prerelease)"
        ShouldPass = $true
    }
)

# Filter scenarios if specific one requested
if ($Scenario -ne "all") {
    $scenarios = $scenarios | Where-Object { $_.Name -eq $Scenario }
    if ($scenarios.Count -eq 0) {
        Write-Host "Error: Unknown scenario '$Scenario'" -ForegroundColor Red
        Write-Host "Available scenarios: update-available, no-update, malformed, prerelease" -ForegroundColor Yellow
        exit 1
    }
}

$passed = 0
$failed = 0
$results = @()

Write-Host "Running $($scenarios.Count) test scenario(s)..." -ForegroundColor Cyan
Write-Host ""

foreach ($s in $scenarios) {
    Write-Host "+-----------------------------------------------------+" -ForegroundColor Gray
    Write-Host "| Test: $($s.Name.PadRight(46)) |" -ForegroundColor White
    Write-Host "+-----------------------------------------------------+" -ForegroundColor Gray
    Write-Host "Description: $($s.Description)" -ForegroundColor Gray
    Write-Host "Expected: $($s.ExpectedResult)" -ForegroundColor Gray
    Write-Host ""

    # Set scenario
    $env:MDVIEWER_TEST_SCENARIO = $s.Name

    Write-Host "Executing: $ExePath --test-update --test-scenario $($s.Name)" -ForegroundColor DarkGray

    try {
        # Run test (capture output and exit code)
        # Use Start-Process for more reliable exit code capture
        $psi = New-Object System.Diagnostics.ProcessStartInfo
        $psi.FileName = $ExePath
        $psi.Arguments = "--test-update --test-scenario $($s.Name)"
        $psi.RedirectStandardOutput = $true
        $psi.RedirectStandardError = $true
        $psi.UseShellExecute = $false
        $psi.CreateNoWindow = $true

        $process = New-Object System.Diagnostics.Process
        $process.StartInfo = $psi
        $process.Start() | Out-Null

        $output = $process.StandardOutput.ReadToEnd()
        $errorOutput = $process.StandardError.ReadToEnd()
        $process.WaitForExit()
        $exitCode = $process.ExitCode

        Write-Host ""
        Write-Host "Output:" -ForegroundColor DarkGray
        if ($output) { Write-Host $output -ForegroundColor DarkGray }
        if ($errorOutput) { Write-Host $errorOutput -ForegroundColor DarkGray }
        Write-Host ""

        # Determine pass/fail based on exit code
        # Exit 0 = update available (update-available, prerelease)
        # Exit 1 = no update (no-update, malformed)
        $updateScenarios = @("update-available", "prerelease")
        $testPassed = ($exitCode -eq 0 -and $updateScenarios -contains $s.Name) -or
                      ($exitCode -eq 1 -and $updateScenarios -notcontains $s.Name)

        if ($testPassed) {
            Write-Host "[PASS]" -ForegroundColor Green
            $passed++
            $results += @{ Name = $s.Name; Status = "PASS"; ExitCode = $exitCode }
        } else {
            Write-Host "[FAIL] (Exit code: $exitCode)" -ForegroundColor Red
            $failed++
            $results += @{ Name = $s.Name; Status = "FAIL"; ExitCode = $exitCode }
        }
    }
    catch {
        Write-Host "[FAIL] (Exception: $($_.Exception.Message))" -ForegroundColor Red
        $failed++
        $results += @{ Name = $s.Name; Status = "FAIL"; ExitCode = -1; Error = $_.Exception.Message }
    }

    Write-Host ""
}

# Summary
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Test Summary" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

foreach ($result in $results) {
    $statusColor = if ($result.Status -eq "PASS") { "Green" } else { "Red" }
    $statusIcon = if ($result.Status -eq "PASS") { "[OK]" } else { "[FAIL]" }
    Write-Host "$statusIcon $($result.Name.PadRight(25)) $($result.Status)" -ForegroundColor $statusColor
}

Write-Host ""
Write-Host "Total: $($scenarios.Count)" -ForegroundColor White
Write-Host "Passed: $passed" -ForegroundColor Green
Write-Host "Failed: $failed" -ForegroundColor $(if ($failed -gt 0) { "Red" } else { "Green" })
Write-Host ""

# Cleanup
Write-Host "Cleaning up test environment..." -ForegroundColor Yellow
Remove-Item env:MDVIEWER_TEST_MODE -ErrorAction SilentlyContinue
Remove-Item env:MDVIEWER_API_URL -ErrorAction SilentlyContinue
Remove-Item env:MDVIEWER_TEST_DATA -ErrorAction SilentlyContinue
Remove-Item env:MDVIEWER_TEST_SCENARIO -ErrorAction SilentlyContinue

Write-Host ""
if ($failed -eq 0) {
    Write-Host "All tests passed!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "Some tests failed!" -ForegroundColor Red
    exit 1
}
