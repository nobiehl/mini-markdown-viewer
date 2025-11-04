# PowerShell script to create a simple Markdown icon
# Creates a 256x256 PNG that we'll convert to ICO

Add-Type -AssemblyName System.Drawing

# Create bitmap
$bitmap = New-Object System.Drawing.Bitmap(256, 256)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

# Background - gradient blue
$brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
    [System.Drawing.Point]::new(0, 0),
    [System.Drawing.Point]::new(0, 256),
    [System.Drawing.Color]::FromArgb(66, 139, 202),
    [System.Drawing.Color]::FromArgb(41, 98, 142)
)
$graphics.FillRectangle($brush, 0, 0, 256, 256)

# Add rounded corners effect
$graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias

# Draw "M" with down arrow (markdown symbol)
$font = New-Object System.Drawing.Font("Segoe UI", 120, [System.Drawing.FontStyle]::Bold)
$whiteBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)

# Measure text for centering
$text = "M↓"
$size = $graphics.MeasureString($text, $font)
$x = (256 - $size.Width) / 2
$y = (256 - $size.Height) / 2

$graphics.DrawString($text, $font, $whiteBrush, $x, $y)

# Save as PNG
$bitmap.Save("$PSScriptRoot\Resources\icon.png", [System.Drawing.Imaging.ImageFormat]::Png)

Write-Host "Icon created: Resources\icon.png"

# Cleanup
$graphics.Dispose()
$bitmap.Dispose()
$brush.Dispose()
$whiteBrush.Dispose()
$font.Dispose()

Write-Host "Now converting to ICO format..."

# Convert PNG to ICO (simple approach - just save as different format)
$icon = [System.Drawing.Icon]::FromHandle((New-Object System.Drawing.Bitmap("$PSScriptRoot\Resources\icon.png")).GetHicon())
$stream = New-Object System.IO.FileStream("$PSScriptRoot\Resources\icon.ico", [System.IO.FileMode]::Create)
$icon.Save($stream)
$stream.Close()

Write-Host "Icon saved: Resources\icon.ico"
