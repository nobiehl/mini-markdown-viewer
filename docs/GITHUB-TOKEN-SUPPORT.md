# GitHub Token Support - v1.5.1 Enhancement

**Date:** 2025-11-06
**Issue:** GitHub API rate limit errors (403) when checking for updates
**Solution:** Optional GitHub token authentication support

---

## Problem

User reported the following error in logs:
```
2025-11-06 11:16:36.700 [WRN] Update check failed (network error): Response status code does not indicate success: 403 (rate limit exceeded).
```

**Root Cause:**
GitHub API limits unauthenticated requests to **60 requests/hour per IP address**.
For users who frequently start MarkdownViewer or have multiple tools checking GitHub, this limit is quickly exhausted.

---

## Solution

Added optional GitHub token authentication via environment variable `GITHUB_TOKEN`.

### Code Changes

**File:** `markdown-viewer/MarkdownViewer/UpdateChecker.cs`
**Lines:** 83-96

```csharp
// Optional GitHub token for higher rate limits
// Unauthenticated: 60 requests/hour
// Authenticated: 5000 requests/hour
string? githubToken = Environment.GetEnvironmentVariable("GITHUB_TOKEN");
if (!string.IsNullOrEmpty(githubToken))
{
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {githubToken}");
    Log.Debug("Using GitHub token for authentication");
}
else
{
    Log.Debug("No GitHub token provided (using unauthenticated API, 60 requests/hour limit)");
}
```

---

## Benefits

| Mode | Rate Limit | Use Case |
|------|------------|----------|
| **Unauthenticated** (default) | 60 requests/hour | Normal users, occasional checks |
| **Authenticated** (with token) | 5000 requests/hour | Power users, CI/CD, frequent checks |

---

## Usage

### For Normal Users (No Action Required)
The app continues to work as before with no changes needed.

### For Power Users (Optional Token)

1. **Create GitHub Personal Access Token:**
   - Go to GitHub → Settings → Developer settings → Personal access tokens → Tokens (classic)
   - Click "Generate new token (classic)"
   - Name: "MarkdownViewer Updates"
   - Scopes: **No scopes needed** (public API only)
   - Generate token and copy it

2. **Set Environment Variable:**

   **Windows (User):**
   ```cmd
   setx GITHUB_TOKEN "your_token_here"
   ```

   **Windows (Session):**
   ```cmd
   set GITHUB_TOKEN=your_token_here
   ```

   **PowerShell:**
   ```powershell
   [Environment]::SetEnvironmentVariable("GITHUB_TOKEN", "your_token_here", "User")
   ```

3. **Restart MarkdownViewer** - Token will be used automatically

---

## Verification

### Check Logs
After starting MarkdownViewer with update check, look for:

**Without token:**
```
[DBG] No GitHub token provided (using unauthenticated API, 60 requests/hour limit)
```

**With token:**
```
[DBG] Using GitHub token for authentication
```

**Log location:** `<EXE_FOLDER>/logs/viewer-YYYYMMDD.log`

---

## Technical Details

- **Backward Compatible:** Works without token (default behavior)
- **Secure:** Token read from environment variable (not stored in settings.json)
- **Graceful Fallback:** If token is invalid, GitHub API returns error but app continues
- **Debug Logging:** Token usage logged for troubleshooting

---

## Testing

### Test 1: Without Token (Default)
```bash
# No environment variable set
MarkdownViewer.exe test.md --update
# Should work with 60 requests/hour limit
```

### Test 2: With Token
```bash
# Set token
set GITHUB_TOKEN=ghp_xxxxxxxxxxxxxxxxxxxx
MarkdownViewer.exe test.md --update
# Should work with 5000 requests/hour limit
```

### Test 3: Verify in Logs
```bash
# Check logs
type logs\viewer-20251106.log | findstr "GitHub token"
```

---

## Security Considerations

1. **Token Permissions:** No scopes needed for public API access
2. **Token Storage:** Stored in OS environment variable (not in app settings)
3. **Token Exposure:** Never logged (only usage confirmation logged)
4. **Token Validation:** GitHub validates token, app doesn't store or validate it

---

## Deployment Status

- ✅ Code implemented in UpdateChecker.cs
- ✅ Built successfully (Release mode)
- ✅ Published to bin-single/MarkdownViewer.exe
- ⏳ Not yet released to GitHub (v1.5.1 already released)
- ℹ️ Can be included in future release (v1.5.2 or v1.6.0)

---

## Future Enhancements (Optional)

1. **Settings UI:** Add token field in settings.json (alternative to env var)
2. **Token Validation:** Test token validity on save
3. **Rate Limit Display:** Show remaining API calls in StatusBar
4. **Token Generation:** Direct link to GitHub token creation page

---

## References

- GitHub API Documentation: https://docs.github.com/en/rest/overview/resources-in-the-rest-api#rate-limiting
- GitHub Token Creation: https://github.com/settings/tokens
- UpdateChecker.cs: `markdown-viewer/MarkdownViewer/UpdateChecker.cs:83-96`
