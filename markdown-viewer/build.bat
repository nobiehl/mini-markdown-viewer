@echo off
echo ========================================
echo   Building Markdown Viewer...
echo ========================================
echo.

cd MarkdownViewer
dotnet build -c Release -o ..\..\bin

if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Build successful!
echo ========================================
echo   Output: ..\bin\MarkdownViewer.exe
echo.
echo Usage:
echo   ..\bin\MarkdownViewer.exe test.md
echo   ..\bin\MarkdownViewer.exe --install
echo.

pause
