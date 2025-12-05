# BLLMT Production Build Script (.NET 10)
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "BLLMT Production Build (.NET 10)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kill any stuck processes
Write-Host "Cleaning up processes and caches..." -ForegroundColor Yellow
Get-Process | Where-Object { $_.Name -match "dotnet|NuGet|BLLMT" } | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Clean up NuGet lock files and temp directories
$lockPath = "C:\Windows\Temp\NuGetScratch\lock"
$scratchPath = "C:\Windows\Temp\NuGetScratch"
if (Test-Path $lockPath) { Remove-Item $lockPath -Force -ErrorAction SilentlyContinue }
if (Test-Path $scratchPath) { Remove-Item $scratchPath -Recurse -Force -ErrorAction SilentlyContinue }

# Clear build artifacts
Write-Host "Cleaning build artifacts..." -ForegroundColor Yellow
dotnet clean -c Release
Remove-Item -Path "bin", "obj" -Recurse -Force -ErrorAction SilentlyContinue

# Clear NuGet caches
Write-Host "Clearing NuGet caches..." -ForegroundColor Yellow
dotnet nuget locals all --clear

Write-Host ""
Write-Host "Building BLLMT for production..." -ForegroundColor Cyan
Write-Host "Target: .NET 10 (net10.0-windows)" -ForegroundColor Yellow
Write-Host "Configuration: Single-file, Self-contained, Compressed" -ForegroundColor Yellow
Write-Host ""

# Restore packages first
Write-Host "Restoring packages..." -ForegroundColor Cyan
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Host "Package restore failed!" -ForegroundColor Red
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

# Build first to catch errors
Write-Host "Building project..." -ForegroundColor Cyan
dotnet build -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    Write-Host "Press any key to exit..."
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    exit 1
}

# Publish with optimizations
Write-Host "Publishing optimized build..." -ForegroundColor Cyan
dotnet publish -c Release -r win-x64 --self-contained true `
    /p:PublishSingleFile=true `
    /p:DebugType=None `
    /p:DebugSymbols=false `
    /p:EnableCompressionInSingleFile=true `
    /p:IncludeNativeLibrariesForSelfExtract=true `
    /p:PublishReadyToRun=true `
    /p:TieredCompilation=true `
    /p:TieredCompilationQuickJit=true

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Build successful!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    
    $exePath = "bin\Release\net10.0-windows\win-x64\publish\BLLMT.exe"
    if (Test-Path $exePath) {
        $fileSize = (Get-Item $exePath).Length / 1MB
        Write-Host "Executable: $exePath" -ForegroundColor White
        Write-Host "File size: $([math]::Round($fileSize, 2)) MB" -ForegroundColor Green
        Write-Host ""
        Write-Host "Features:" -ForegroundColor Cyan
        Write-Host "  ? Self-contained (.NET 10 embedded)" -ForegroundColor Green
        Write-Host "  ? Single executable file" -ForegroundColor Green
        Write-Host "  ? Compressed and optimized" -ForegroundColor Green
        Write-Host "  ? Ready to run on any Windows 10+ machine" -ForegroundColor Green
        Write-Host ""
        Write-Host "The .exe is standalone - no installation required!" -ForegroundColor Yellow
    } else {
        Write-Host "Warning: Executable not found at expected location" -ForegroundColor Yellow
    }
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "Publish failed!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host "  1. Make sure .NET 10 SDK is installed" -ForegroundColor White
    Write-Host "  2. Close any running instances of BLLMT.exe" -ForegroundColor White
    Write-Host "  3. Run as Administrator if permissions are needed" -ForegroundColor White
}

Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
