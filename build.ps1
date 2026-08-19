# Wraith Build Script (PowerShell)
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Wraith Build Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kill any stuck processes
Write-Host "Killing any stuck NuGet/dotnet processes..." -ForegroundColor Yellow
Get-Process | Where-Object { $_.Name -match "dotnet|NuGet" } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Clean up NuGet lock files
Write-Host "Cleaning up NuGet lock files..." -ForegroundColor Yellow
$lockPath = "C:\Windows\Temp\NuGetScratch\lock"
if (Test-Path $lockPath) {
    Remove-Item $lockPath -Force -ErrorAction SilentlyContinue
}
$scratchPath = "C:\Windows\Temp\NuGetScratch"
if (Test-Path $scratchPath) {
    Remove-Item $scratchPath -Recurse -Force -ErrorAction SilentlyContinue
}

# Clean project
Write-Host ""
Write-Host "Cleaning project..." -ForegroundColor Yellow
dotnet clean --nologo

# Clear NuGet caches
Write-Host ""
Write-Host "Clearing NuGet caches..." -ForegroundColor Yellow
dotnet nuget locals all --clear

# Restore packages
Write-Host ""
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
$restoreResult = dotnet restore --force-evaluate --no-cache 2>&1

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "Restore failed! Trying alternative method..." -ForegroundColor Red
    Write-Host ""
    
    # Try restoring from a different temp location
    $env:NUGET_PACKAGES = "$env:USERPROFILE\.nuget\packages"
    $env:TEMP = "$env:USERPROFILE\AppData\Local\Temp"
    dotnet restore --force-evaluate
}

# Build
Write-Host ""
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build --no-restore

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Build complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
