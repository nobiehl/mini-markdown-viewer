using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Xunit;
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

namespace MarkdownViewer.Tests.UIAutomation
{
    /// <summary>
    /// Collection Definition für UI Tests die sequentiell laufen müssen.
    /// Tests in dieser Collection laufen nicht parallel und vermeiden UI Automation Konflikte.
    /// </summary>
    [CollectionDefinition("Sequential UI Tests")]
    public class SequentialUITestCollection : ICollectionFixture<object>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

    /// <summary>
    /// DEMO UI Test - Dieser Test läuft wirklich und startet die Anwendung!
    /// WICHTIG: Läuft sequentiell (nicht parallel) um UI Automation Konflikte zu vermeiden.
    /// </summary>
    [Collection("Sequential UI Tests")]
    public class DemoUITest : IDisposable
    {
        private Process? _process;
        private readonly string _testFilePath;

        public DemoUITest()
        {
            // Erstelle Test-Markdown-Datei
            _testFilePath = Path.Combine(Path.GetTempPath(), $"demo_{Guid.NewGuid()}.md");
            File.WriteAllText(_testFilePath, @"# Demo UI Test

Dies ist eine **Demo** für UI Automation mit FlaUI!

## Features getestet:
- App startet
- Fenster erscheint
- Titel ist korrekt

*Test läuft gerade...*
");
        }

        [Fact]
        public void DEMO_ApplicationStarts_WindowAppears_TitleCorrect()
        {
            // === SCHRITT 1: FINDE DIE EXE ===
            Console.WriteLine("🔍 Suche MarkdownViewer.exe...");
            var exePath = FindExecutable();
            Assert.NotNull(exePath);
            Console.WriteLine($"✅ Gefunden: {exePath}");

            // === SCHRITT 2: STARTE DIE ANWENDUNG ===
            Console.WriteLine("\n🚀 Starte MarkdownViewer.exe...");
            Console.WriteLine($"   Mit Datei: {_testFilePath}");

            _process = Process.Start(new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = $"\"{_testFilePath}\"",
                UseShellExecute = true
            });

            Assert.NotNull(_process);
            Console.WriteLine($"✅ Prozess gestartet (PID: {_process.Id})");

            // Warte bis Fenster erscheint
            Thread.Sleep(2000);

            // === SCHRITT 3: FINDE DAS FENSTER ===
            Console.WriteLine("\n🔍 Suche Hauptfenster...");
            using var automation = new UIA3Automation();
            var desktop = automation.GetDesktop();

            Window? mainWindow = null;
            var startTime = DateTime.Now;
            var timeout = TimeSpan.FromSeconds(10);

            while (DateTime.Now - startTime < timeout)
            {
                var windows = desktop.FindAllChildren(cf =>
                    cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));

                mainWindow = windows
                    .Select(w => w.AsWindow())
                    .FirstOrDefault(w => w.Title.Contains("demo_") || w.Title.Contains("Markdown Viewer"));

                if (mainWindow != null) break;
                Thread.Sleep(500);
            }

            Assert.NotNull(mainWindow);
            Console.WriteLine($"✅ Fenster gefunden: \"{mainWindow.Title}\"");

            // === SCHRITT 4: PRÜFE TITEL ===
            Console.WriteLine("\n🔍 Prüfe Fenstertitel...");
            var title = mainWindow.Title;
            Console.WriteLine($"   Titel: {title}");

            // Titel sollte Dateinamen enthalten
            Assert.Contains("demo_", title);
            Console.WriteLine("✅ Titel ist korrekt!");

            // === SCHRITT 5: WARTE KURZ (damit du es siehst!) ===
            Console.WriteLine("\n⏳ Warte 3 Sekunden (damit du das Fenster siehst)...");
            Thread.Sleep(3000);

            // === SCHRITT 6: SCHLIEẞE FENSTER ===
            Console.WriteLine("\n🔴 Schließe Anwendung...");
            mainWindow.Close();
            Console.WriteLine("✅ Fenster geschlossen");

            // Warte bis Prozess beendet
            if (_process != null && !_process.HasExited)
            {
                _process.WaitForExit(2000);
                if (!_process.HasExited)
                {
                    _process.Kill();
                }
            }

            Console.WriteLine("✅ Test abgeschlossen!");
        }

        private string? FindExecutable()
        {
            // Suche nach MarkdownViewer.exe
            var possiblePaths = new[]
            {
                @"..\..\..\..\..\MarkdownViewer\bin\Release\net8.0-windows\MarkdownViewer.exe",
                @"..\..\..\..\..\MarkdownViewer\bin\Debug\net8.0-windows\MarkdownViewer.exe",
                @"..\..\..\..\MarkdownViewer\bin\Release\net8.0-windows\MarkdownViewer.exe",
                @"..\..\..\..\MarkdownViewer\bin\Debug\net8.0-windows\MarkdownViewer.exe",
            };

            foreach (var path in possiblePaths)
            {
                var fullPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path));
                if (File.Exists(fullPath))
                    return fullPath;
            }

            return null;
        }

        public void Dispose()
        {
            try
            {
                // Cleanup
                if (_process != null && !_process.HasExited)
                {
                    _process.Kill();
                    _process.Dispose();
                }

                if (File.Exists(_testFilePath))
                {
                    File.Delete(_testFilePath);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
