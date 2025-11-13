using System;
using System.IO;
using System.Threading;
using FluentAssertions;
using Moq;
using MarkdownViewer.Core;
using Xunit;

namespace MarkdownViewer.Tests.Services
{
    /// <summary>
    /// Unit tests for FileWatcherManager.
    /// Tests file watching functionality, event triggering, and proper disposal.
    /// </summary>
    public class FileWatcherManagerTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly FileWatcherManager _fileWatcherManager;

        public FileWatcherManagerTests()
        {
            // Create a unique test directory for each test
            _testDirectory = Path.Combine(Path.GetTempPath(), $"FileWatcherTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
            _fileWatcherManager = new FileWatcherManager();
        }

        public void Dispose()
        {
            _fileWatcherManager?.Dispose();

            // Cleanup test directory
            if (Directory.Exists(_testDirectory))
            {
                try
                {
                    Directory.Delete(_testDirectory, recursive: true);
                }
                catch
                {
                    // Ignore cleanup errors in tests
                }
            }
        }

        [Fact]
        public void Watch_ValidPath_StartsWatching()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(testFilePath, "# Test");

            var eventRaised = false;
            _fileWatcherManager.FileChanged += (sender, path) => eventRaised = true;

            // Act
            _fileWatcherManager.Watch(testFilePath);

            // Trigger a change
            Thread.Sleep(200); // Wait for watcher to initialize
            File.WriteAllText(testFilePath, "# Modified");
            Thread.Sleep(300); // Wait for event to fire

            // Assert
            eventRaised.Should().BeTrue("file change event should be raised");
        }

        [Fact]
        public void Watch_NullPath_ThrowsException()
        {
            // Arrange
            string? nullPath = null;

            // Act & Assert
            Action act = () => _fileWatcherManager.Watch(nullPath!);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Watch_InvalidPath_HandlesGracefully()
        {
            // Arrange
            var invalidPath = Path.Combine(_testDirectory, "nonexistent", "file.md");

            // Act - should not throw
            Action act = () => _fileWatcherManager.Watch(invalidPath);

            // Assert - should handle gracefully without throwing
            act.Should().NotThrow();
        }

        [Fact]
        public void StopWatching_StopsCorrectly()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(testFilePath, "# Test");

            var eventRaisedCount = 0;
            _fileWatcherManager.FileChanged += (sender, path) => eventRaisedCount++;

            _fileWatcherManager.Watch(testFilePath);
            Thread.Sleep(200); // Wait for watcher to initialize

            // Act
            _fileWatcherManager.StopWatching();

            // Trigger a change after stopping
            File.WriteAllText(testFilePath, "# Modified after stop");
            Thread.Sleep(300); // Wait to ensure event would have fired if still watching

            // Assert
            eventRaisedCount.Should().Be(0, "no events should be raised after stopping");
        }

        [Fact]
        public void FileChanged_TriggersEvent()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(testFilePath, "# Test");

            string? changedPath = null;
            _fileWatcherManager.FileChanged += (sender, path) => changedPath = path;

            // Act
            _fileWatcherManager.Watch(testFilePath);
            Thread.Sleep(200); // Wait for watcher to initialize

            File.WriteAllText(testFilePath, "# Changed Content");
            Thread.Sleep(300); // Wait for event

            // Assert
            changedPath.Should().NotBeNull();
            changedPath.Should().Be(testFilePath);
        }

        [Fact]
        public void FileChanged_MultipleChanges_TriggersMultipleEvents()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(testFilePath, "# Test");

            var eventCount = 0;
            _fileWatcherManager.FileChanged += (sender, path) => eventCount++;

            // Act
            _fileWatcherManager.Watch(testFilePath);
            Thread.Sleep(200);

            File.WriteAllText(testFilePath, "# Change 1");
            Thread.Sleep(300);

            File.WriteAllText(testFilePath, "# Change 2");
            Thread.Sleep(300);

            // Assert
            eventCount.Should().BeGreaterThanOrEqualTo(2, "multiple file changes should trigger multiple events");
        }

        [Fact]
        public void Dispose_CleansUpCorrectly()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(testFilePath, "# Test");

            var eventRaised = false;
            _fileWatcherManager.FileChanged += (sender, path) => eventRaised = true;

            _fileWatcherManager.Watch(testFilePath);
            Thread.Sleep(200);

            // Act
            _fileWatcherManager.Dispose();

            // Trigger change after dispose
            File.WriteAllText(testFilePath, "# Modified after dispose");
            Thread.Sleep(300);

            // Assert
            eventRaised.Should().BeFalse("no events should be raised after disposal");
        }

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.md");
            File.WriteAllText(testFilePath, "# Test");
            _fileWatcherManager.Watch(testFilePath);

            // Act & Assert - should not throw
            Action act = () =>
            {
                _fileWatcherManager.Dispose();
                _fileWatcherManager.Dispose();
                _fileWatcherManager.Dispose();
            };

            act.Should().NotThrow("Dispose should be idempotent");
        }

        [Fact]
        public void Watch_CalledTwice_DisposesOldWatcher()
        {
            // Arrange
            var testFile1 = Path.Combine(_testDirectory, "test1.md");
            var testFile2 = Path.Combine(_testDirectory, "test2.md");
            File.WriteAllText(testFile1, "# Test 1");
            File.WriteAllText(testFile2, "# Test 2");

            var file1EventCount = 0;
            var file2EventCount = 0;

            _fileWatcherManager.FileChanged += (sender, path) =>
            {
                if (path == testFile1) file1EventCount++;
                if (path == testFile2) file2EventCount++;
            };

            // Act
            _fileWatcherManager.Watch(testFile1);
            Thread.Sleep(200);

            _fileWatcherManager.Watch(testFile2); // Should dispose first watcher
            Thread.Sleep(200);

            // Modify both files
            File.WriteAllText(testFile1, "# Modified 1");
            File.WriteAllText(testFile2, "# Modified 2");
            Thread.Sleep(300);

            // Assert
            file1EventCount.Should().Be(0, "first file should no longer be watched");
            file2EventCount.Should().BeGreaterThanOrEqualTo(1, "second file should be watched");
        }

        [Fact]
        public void Watch_EmptyFileName_HandlesGracefully()
        {
            // Arrange
            var invalidPath = _testDirectory + Path.DirectorySeparatorChar;

            // Act & Assert - should not throw
            Action act = () => _fileWatcherManager.Watch(invalidPath);
            act.Should().NotThrow();
        }

        [Fact]
        public void FileChanged_PassesCorrectPath()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "specific-file.md");
            File.WriteAllText(testFilePath, "# Initial");

            string? receivedPath = null;
            object? receivedSender = null;

            _fileWatcherManager.FileChanged += (sender, path) =>
            {
                receivedSender = sender;
                receivedPath = path;
            };

            // Act
            _fileWatcherManager.Watch(testFilePath);
            Thread.Sleep(200);

            File.WriteAllText(testFilePath, "# Updated");
            Thread.Sleep(300);

            // Assert
            receivedSender.Should().Be(_fileWatcherManager, "sender should be the FileWatcherManager instance");
            receivedPath.Should().Be(testFilePath, "correct file path should be passed in event");
        }

        [Fact]
        public void Watch_WithDifferentFileExtensions_Works()
        {
            // Arrange
            var testFilePath = Path.Combine(_testDirectory, "test.txt");
            File.WriteAllText(testFilePath, "Test content");

            var eventRaised = false;
            _fileWatcherManager.FileChanged += (sender, path) => eventRaised = true;

            // Act
            _fileWatcherManager.Watch(testFilePath);
            Thread.Sleep(200);

            File.WriteAllText(testFilePath, "Modified content");
            Thread.Sleep(300);

            // Assert
            eventRaised.Should().BeTrue("should work with non-markdown files");
        }
    }
}
