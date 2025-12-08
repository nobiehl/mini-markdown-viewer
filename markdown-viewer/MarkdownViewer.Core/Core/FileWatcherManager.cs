using System;
using System.IO;
using Serilog;

namespace MarkdownViewer.Core
{
    /// <summary>
    /// Manages file watching for live-reload functionality.
    /// Watches a single file and triggers an event when it changes.
    /// Includes a small delay to avoid multiple triggers from editors.
    /// </summary>
    public class FileWatcherManager : IDisposable
    {
        private FileSystemWatcher? _watcher;
        private bool _disposed = false;

        /// <summary>
        /// Event raised when the watched file changes.
        /// Provides the full path to the changed file.
        /// </summary>
        public event EventHandler<string>? FileChanged;

        /// <summary>
        /// Event raised when the watched file is deleted.
        /// Provides the full path to the deleted file.
        /// </summary>
        public event EventHandler<string>? FileDeleted;

        /// <summary>
        /// Event raised when the watched file is renamed.
        /// Provides (OldPath, NewPath) tuple.
        /// </summary>
        public event EventHandler<(string OldPath, string NewPath)>? FileRenamed;

        /// <summary>
        /// Event raised when the watched file is created (after being deleted).
        /// Provides the full path to the created file.
        /// </summary>
        public event EventHandler<string>? FileCreated;

        /// <summary>
        /// Event raised when an error occurs in the file watcher.
        /// Provides error message.
        /// </summary>
        public event EventHandler<string>? WatcherError;

        /// <summary>
        /// Starts watching a file for changes.
        /// Only one file can be watched at a time - calling this again disposes the previous watcher.
        /// </summary>
        /// <param name="filePath">Full path to the file to watch</param>
        public void Watch(string filePath)
        {
            ArgumentNullException.ThrowIfNull(filePath);

            Log.Debug("FileWatcherManager: Starting to watch {FilePath}", filePath);

            try
            {
                string? directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                if (string.IsNullOrEmpty(directory))
                {
                    Log.Warning("Could not determine directory for file: {FilePath}", filePath);
                    return;
                }

                if (string.IsNullOrEmpty(fileName))
                {
                    Log.Warning("Could not determine file name for file: {FilePath}", filePath);
                    return;
                }

                // Dispose old watcher if exists
                _watcher?.Dispose();

                _watcher = new FileSystemWatcher(directory)
                {
                    Filter = fileName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName
                };

                _watcher.Changed += OnFileChanged;
                _watcher.Deleted += OnFileDeleted;
                _watcher.Renamed += OnFileRenamed;
                _watcher.Created += OnFileCreated;
                _watcher.Error += OnWatcherError;

                _watcher.EnableRaisingEvents = true;
                _currentWatchedFilePath = filePath;
                Log.Debug("FileWatcher enabled for: {Directory}/{FileName}", directory, fileName);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to setup file watcher for: {FilePath}", filePath);
            }
        }

        /// <summary>
        /// Stops watching the current file.
        /// </summary>
        public void StopWatching()
        {
            if (_watcher != null)
            {
                _watcher.EnableRaisingEvents = false;
                Log.Debug("FileWatcher disabled");
            }
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            Log.Information("File changed detected: {FilePath}", e.FullPath);

            // Small delay to avoid multiple triggers from editors
            // Some editors save multiple times in quick succession
            System.Threading.Thread.Sleep(100);

            // Raise event to subscribers
            FileChanged?.Invoke(this, e.FullPath);
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            Log.Warning("File deleted detected: {FilePath}", e.FullPath);
            FileDeleted?.Invoke(this, e.FullPath);
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            Log.Information("File renamed detected: {OldPath} -> {NewPath}", e.OldFullPath, e.FullPath);
            FileRenamed?.Invoke(this, (e.OldFullPath, e.FullPath));
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Log.Information("File created detected: {FilePath}", e.FullPath);
            FileCreated?.Invoke(this, e.FullPath);
        }

        private void OnWatcherError(object sender, ErrorEventArgs e)
        {
            Exception? ex = e.GetException();
            string errorMessage = ex?.Message ?? "Unknown error";
            Log.Error(ex, "FileWatcher error: {ErrorMessage}", errorMessage);
            WatcherError?.Invoke(this, errorMessage);

            // Try to recreate the watcher after an error (e.g., buffer overflow)
            TryRecreateWatcher();
        }

        /// <summary>
        /// Gets the currently watched file path.
        /// </summary>
        public string? CurrentFilePath => _currentWatchedFilePath;
        private string? _currentWatchedFilePath;

        /// <summary>
        /// Attempts to recreate the watcher after an error.
        /// This helps recover from buffer overflow or other transient errors.
        /// </summary>
        private void TryRecreateWatcher()
        {
            if (_currentWatchedFilePath != null)
            {
                Log.Information("Attempting to recreate FileWatcher after error for: {FilePath}", _currentWatchedFilePath);
                try
                {
                    // Small delay before recreating
                    System.Threading.Thread.Sleep(500);
                    Watch(_currentWatchedFilePath);
                    Log.Information("FileWatcher recreated successfully");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to recreate FileWatcher");
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Log.Debug("FileWatcherManager: Disposing");
                    _watcher?.Dispose();
                    _watcher = null;
                }
                _disposed = true;
            }
        }
    }
}
