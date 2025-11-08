# Update Backup Tests

## Problem Description

The update mechanism has an issue where deleting the backup file immediately after applying an update can fail and show an error message to the user.

### Current Flow (Problematic)

```
1. Create backup: exe → exe.backup
2. Apply update: update → exe
3. Delete backup: DELETE exe.backup  ⚠️ CAN FAIL!
4. Restart app
5. Exit
```

### Why It Fails

- **File Locking**: Windows may still have the backup file locked
- **Permissions**: User may not have write permissions
- **Antivirus**: AV software may block file deletion temporarily
- **Timing**: Deletion happens too quickly after file operations

### Solution

**Don't delete the backup immediately!** Instead:

```
1. Create backup: exe → exe.backup
2. Apply update: update → exe
3. Restart app (backup remains as safety net)
4. On NEXT startup: Check for old backup and delete it
```

## Test Coverage

The tests verify:

1. ✅ Backup creation works correctly
2. ✅ Old backups are replaced before creating new ones
3. ✅ Update process creates backup and applies update
4. ✅ Rollback works when update fails
5. ✅ Backup cleanup succeeds on next startup
6. ✅ Locked backup files don't cause errors

## Running the Tests

```bash
cd markdown-viewer/MarkdownViewer.Tests
dotnet test --filter "FullyQualifiedName~UpdateBackupTests"
```

## Implementation Fix

### Before (UpdateChecker.cs)

```csharp
File.Move(exePath, backupPath);      // Create backup
File.Move(updatePath, exePath);      // Apply update
File.Delete(backupPath);             // ⚠️ Delete immediately (can fail!)
Process.Start(...);                  // Restart
Environment.Exit(0);
```

### After (Recommended)

```csharp
// In ApplyPendingUpdate():
File.Move(exePath, backupPath);      // Create backup
File.Move(updatePath, exePath);      // Apply update
// DON'T delete backup here!
Process.Start(...);                  // Restart
Environment.Exit(0);

// In Program.Main() or MainForm constructor:
CleanupOldBackup();                  // Delete old backup on startup
```

### New Method to Add

```csharp
private static void CleanupOldBackup()
{
    try
    {
        string exePath = Application.ExecutablePath;
        string backupPath = exePath + ".backup";

        if (File.Exists(backupPath) && File.Exists(exePath))
        {
            // Both files exist - update was successful
            // Safe to delete backup
            Log.Debug("Cleaning up old backup: {BackupPath}", backupPath);
            File.Delete(backupPath);
            Log.Information("Old backup deleted successfully");
        }
    }
    catch (Exception ex)
    {
        // Silently ignore - backup cleanup is not critical
        Log.Debug(ex, "Could not delete old backup (will retry next time)");
    }
}
```

## Benefits of This Approach

1. **No User Error**: Deletion failures don't show error messages
2. **Safety Net**: Backup remains if something goes wrong
3. **Automatic Cleanup**: Old backups are eventually removed
4. **Graceful Degradation**: If cleanup fails, it retries next time
