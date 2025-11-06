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
        private FileSystemWatcher _watcher;
        private bool _disposed = false;

        /// <summary>
        /// Event raised when the watched file changes.
        /// Provides the full path to the changed file.
        /// </summary>
        public event EventHandler<string> FileChanged;

        /// <summary>
        /// Starts watching a file for changes.
        /// Only one file can be watched at a time - calling this again disposes the previous watcher.
        /// </summary>
        /// <param name="filePath">Full path to the file to watch</param>
        public void Watch(string filePath)
        {
            Log.Debug("FileWatcherManager: Starting to watch {FilePath}", filePath);

            try
            {
                string directory = Path.GetDirectoryName(filePath);
                string fileName = Path.GetFileName(filePath);

                // Dispose old watcher if exists
                _watcher?.Dispose();

                _watcher = new FileSystemWatcher(directory)
                {
                    Filter = fileName,
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
                };

                _watcher.Changed += OnFileChanged;

                _watcher.EnableRaisingEvents = true;
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
