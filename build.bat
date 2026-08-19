@echo off
echo ========================================
echo Wraith Build Script
echo ========================================
echo.

REM Kill any stuck NuGet processes
echo Checking for stuck NuGet processes...
taskkill /F /IM NuGet.exe 2>nul
taskkill /F /IM dotnet.exe 2>nul
timeout /t 2 /nobreak >nul

REM Clean up NuGet lock files and caches
echo Cleaning up NuGet lock files...
if exist "C:\Windows\Temp\NuGetScratch\lock" (
    del /F /Q "C:\Windows\Temp\NuGetScratch\lock" 2>nul
)
if exist "C:\Windows\Temp\NuGetScratch\" (
    rmdir /S /Q "C:\Windows\Temp\NuGetScratch\" 2>nul
)

REM Clean the project
echo.
echo Cleaning project...
dotnet clean --nologo

REM Clear NuGet HTTP cache
echo.
echo Clearing NuGet caches...
dotnet nuget locals http-cache --clear
dotnet nuget locals temp --clear

REM Restore with verbose logging to see what's happening
echo.
echo Restoring NuGet packages...
dotnet nuget locals all --clear && dotnet restore && dotnet build

echo.
echo ========================================
echo Build complete!
echo ========================================
pause
