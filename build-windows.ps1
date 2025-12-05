# Build standalone Windows version of BLLMT
# This creates a self-contained executable that includes .NET runtime

Write-Host "=====================================" -ForegroundColor Cyan
Write-Host "Building BLLMT for Windows (Standalone)" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
Remove-Item -Path ".\publish\windows" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".\bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".\obj" -Recurse -Force -ErrorAction SilentlyContinue

# Build self-contained Windows executable
Write-Host "Building self-contained Windows executable..." -ForegroundColor Yellow
dotnet publish -f net10.0-windows `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:EnableCompressionInSingleFile=true `
    -o ".\publish\windows"

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Build FAILED!" -ForegroundColor Red
    Write-Host "Please check the error messages above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=====================================" -ForegroundColor Green
Write-Host "Build Successful!" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""
Write-Host "Output location: .\publish\windows\" -ForegroundColor Cyan
Write-Host "Executable: BLLMT.exe" -ForegroundColor Cyan
Write-Host ""

# Get file size
$exePath = ".\publish\windows\BLLMT.exe"
if (Test-Path $exePath) {
    $fileSize = (Get-Item $exePath).Length / 1MB
    Write-Host "File size: $($fileSize.ToString('N2')) MB" -ForegroundColor Cyan
    Write-Host ""
}

# Create a README in the publish folder
$readmeContent = @"
# BLLMT - Windows Standalone

This is a self-contained build of BLLMT for Windows.

## What's Included
- BLLMT.exe (includes .NET 10 runtime)
- No additional dependencies needed!

## Installation
1. Copy BLLMT.exe to any folder on your computer
2. Run BLLMT.exe
3. The app will start in the system tray

## First Run
1. Right-click the system tray icon
2. Select 'Settings'
3. Configure your LLM API key and settings
4. Save and start using!

## System Requirements
- Windows 10 version 1809 or later
- Windows 11 (any version)

## Notes
- Settings are stored in: %APPDATA%\BLLMT\settings.json
- No installation required - just run the .exe
- You can move the .exe file anywhere

## Troubleshooting
If Windows Defender SmartScreen blocks the app:
1. Click "More info"
2. Click "Run anyway"

This is normal for unsigned applications.

For more information, visit: https://github.com/yourusername/bllmt
"@

Set-Content -Path ".\publish\windows\README.txt" -Value $readmeContent

Write-Host "README.txt created in output folder" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Test the executable: .\publish\windows\BLLMT.exe" -ForegroundColor White
Write-Host "2. Create a ZIP file for distribution" -ForegroundColor White
Write-Host "3. Upload to GitHub Releases" -ForegroundColor White
Write-Host ""

# Offer to create a ZIP file
$createZip = Read-Host "Create a ZIP file for distribution? (Y/N)"
if ($createZip -eq "Y" -or $createZip -eq "y") {
    Write-Host ""
    Write-Host "Creating ZIP file..." -ForegroundColor Yellow
    
    $version = "1.0.0" # You can change this
    $zipPath = ".\publish\BLLMT-Windows-v$version.zip"
    
    # Remove old ZIP if exists
    Remove-Item -Path $zipPath -Force -ErrorAction SilentlyContinue
    
    # Create ZIP
    Compress-Archive -Path ".\publish\windows\*" -DestinationPath $zipPath -CompressionLevel Optimal
    
    if (Test-Path $zipPath) {
        $zipSize = (Get-Item $zipPath).Length / 1MB
        Write-Host ""
        Write-Host "ZIP file created successfully!" -ForegroundColor Green
        Write-Host "Location: $zipPath" -ForegroundColor Cyan
        Write-Host "Size: $($zipSize.ToString('N2')) MB" -ForegroundColor Cyan
        Write-Host ""
        Write-Host "This ZIP is ready to upload to GitHub Releases!" -ForegroundColor Green
    }
}

Write-Host ""
Write-Host "Done! ??" -ForegroundColor Green
