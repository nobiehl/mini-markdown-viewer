@echo off
echo ========================================
echo   Building Single-File EXE...
echo ========================================
echo.

cd MarkdownViewer
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ..\..\bin-single

if errorlevel 1 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ========================================
echo   Build successful!
echo ========================================
echo   Output: ..\bin-single\MarkdownViewer.exe (Single File)
echo.
echo Usage:
echo   ..\bin-single\MarkdownViewer.exe
echo   ..\bin-single\MarkdownViewer.exe test.md
echo   ..\bin-single\MarkdownViewer.exe --install
echo.

pause
