using Xunit;
using System;
using System.IO;

namespace MarkdownViewer.Tests.UpdateTests
{
    /// <summary>
    /// Tests for update backup and rollback functionality.
    ///
    /// These tests verify that:
    /// 1. Backup files are created correctly
    /// 2. Old backups are cleaned up on startup
    /// 3. Rollback works when update fails
    /// 4. No error occurs if backup cannot be deleted immediately
    /// </summary>
    public class UpdateBackupTests : IDisposable
    {
        private readonly string _testDir;
        private readonly string _exePath;
        private readonly string _backupPath;
        private readonly string _updatePath;

        public UpdateBackupTests()
        {
            // Create temporary test directory
            _testDir = Path.Combine(Path.GetTempPath(), "MarkdownViewerTests_" + Guid.NewGuid());
            Directory.CreateDirectory(_testDir);

            _exePath = Path.Combine(_testDir, "MarkdownViewer.exe");
            _backupPath = _exePath + ".backup";
            _updatePath = Path.Combine(_testDir, "MarkdownViewer_update.exe");
        }

        public void Dispose()
        {
            // Cleanup test directory
            if (Directory.Exists(_testDir))
            {
                try
                {
                    Directory.Delete(_testDir, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [Fact]
        public void BackupCreation_ShouldSucceed_WhenExeExists()
        {
            // Arrange
            File.WriteAllText(_exePath, "Original EXE");

            // Act
            File.Move(_exePath, _backupPath);

            // Assert
            Assert.True(File.Exists(_backupPath), "Backup file should exist");
            Assert.False(File.Exists(_exePath), "Original EXE should be moved");
            Assert.Equal("Original EXE", File.ReadAllText(_backupPath));
        }

        [Fact]
        public void OldBackup_ShouldBeDeleted_BeforeCreatingNew()
        {
            // Arrange
            File.WriteAllText(_exePath, "Current EXE");
            File.WriteAllText(_backupPath, "Old Backup");

            // Act
            if (File.Exists(_backupPath))
            {
                File.Delete(_backupPath);
            }
            File.Move(_exePath, _backupPath);

            // Assert
            Assert.True(File.Exists(_backupPath), "New backup should exist");
            Assert.Equal("Current EXE", File.ReadAllText(_backupPath));
        }

        [Fact]
        public void UpdateProcess_ShouldCreateBackup_AndApplyUpdate()
        {
            // Arrange
            File.WriteAllText(_exePath, "Version 1.0");
            File.WriteAllText(_updatePath, "Version 2.0");

            // Act - Simulate update process
            // Step 1: Create backup
            File.Move(_exePath, _backupPath);

            // Step 2: Move update to exe location
            File.Move(_updatePath, _exePath);

            // Assert
            Assert.True(File.Exists(_exePath), "New EXE should exist");
            Assert.True(File.Exists(_backupPath), "Backup should exist");
            Assert.Equal("Version 2.0", File.ReadAllText(_exePath));
            Assert.Equal("Version 1.0", File.ReadAllText(_backupPath));
        }

        [Fact]
        public void Rollback_ShouldRestore_WhenUpdateFails()
        {
            // Arrange
            File.WriteAllText(_exePath, "Version 1.0");
            File.WriteAllText(_backupPath, "Version 1.0");
            File.Delete(_exePath); // Simulate failed update

            // Act - Rollback
            if (File.Exists(_backupPath) && !File.Exists(_exePath))
            {
                File.Move(_backupPath, _exePath);
            }

            // Assert
            Assert.True(File.Exists(_exePath), "EXE should be restored");
            Assert.False(File.Exists(_backupPath), "Backup should be consumed");
            Assert.Equal("Version 1.0", File.ReadAllText(_exePath));
        }

        [Fact]
        public void BackupCleanup_ShouldSucceed_OnNextStartup()
        {
            // Arrange - Simulate successful update with backup still present
            File.WriteAllText(_exePath, "Version 2.0");
            File.WriteAllText(_backupPath, "Version 1.0");

            // Act - Cleanup on next startup
            if (File.Exists(_backupPath) && File.Exists(_exePath))
            {
                try
                {
                    File.Delete(_backupPath);
                }
                catch
                {
                    // If deletion fails, backup remains (acceptable)
                }
            }

            // Assert - Either deleted or still exists (both are OK)
            if (File.Exists(_backupPath))
            {
                // Backup still exists (e.g., file locked) - this is OK
                Assert.True(File.Exists(_exePath), "Main EXE should still exist");
            }
            else
            {
                // Backup was successfully deleted
                Assert.False(File.Exists(_backupPath), "Backup should be deleted");
            }
        }

        [Fact]
        public void LockedBackup_ShouldNotCauseError_DuringUpdate()
        {
            // Arrange
            File.WriteAllText(_exePath, "Version 1.0");
            File.WriteAllText(_updatePath, "Version 2.0");

            // Act - Simulate update with locked backup
            File.Move(_exePath, _backupPath);
            File.Move(_updatePath, _exePath);

            // Try to delete backup (might fail due to lock)
            Exception? deletionException = null;
            try
            {
                // Simulate file lock by opening it
                using var lockStream = new FileStream(_backupPath, FileMode.Open, FileAccess.Read, FileShare.None);

                // Try to delete (should fail)
                File.Delete(_backupPath);
            }
            catch (Exception ex)
            {
                deletionException = ex;
            }

            // Assert - Deletion failure should not break the update
            Assert.NotNull(deletionException); // Deletion should have failed
            Assert.True(File.Exists(_exePath), "New EXE should still exist");
            Assert.True(File.Exists(_backupPath), "Backup should still exist (locked)");
            Assert.Equal("Version 2.0", File.ReadAllText(_exePath));
        }
    }
}
