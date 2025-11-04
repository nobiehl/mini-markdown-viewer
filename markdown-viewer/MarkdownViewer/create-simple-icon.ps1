# Simple icon creator - creates a basic Markdown icon
Add-Type -AssemblyName System.Drawing

$size = 256
$bitmap = New-Object System.Drawing.Bitmap($size, $size)
$graphics = [System.Drawing.Graphics]::FromImage($bitmap)
$graphics.SmoothingMode = 'AntiAlias'
$graphics.TextRenderingHint = 'AntiAlias'

# Blue background
$bgBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(66, 139, 202))
$graphics.FillRectangle($bgBrush, 0, 0, $size, $size)

# White "M" with down arrow
$font = New-Object System.Drawing.Font('Segoe UI', 110, [System.Drawing.FontStyle]::Bold)
$whiteBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
$text = 'Md'
$format = New-Object System.Drawing.StringFormat
$format.Alignment = 'Center'
$format.LineAlignment = 'Center'
$rect = New-Object System.Drawing.RectangleF(0, 0, $size, $size)
$graphics.DrawString($text, $font, $whiteBrush, $rect, $format)

# Save
$pngPath = Join-Path $PSScriptRoot 'Resources\icon.png'
$icoPath = Join-Path $PSScriptRoot 'Resources\icon.ico'
New-Item -Path (Split-Path $pngPath) -ItemType Directory -Force | Out-Null
$bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)

# Convert to ICO
$icon = [System.Drawing.Icon]::FromHandle($bitmap.GetHicon())
$fileStream = [System.IO.File]::Create($icoPath)
$icon.Save($fileStream)
$fileStream.Close()

Write-Host "Icon created successfully!"
Write-Host "PNG: $pngPath"
Write-Host "ICO: $icoPath"

# Cleanup
$graphics.Dispose()
$bitmap.Dispose()
$bgBrush.Dispose()
$whiteBrush.Dispose()
$font.Dispose()
