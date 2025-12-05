# Quick Build Guide for BLLMT

## ?? Building Standalone Versions

### Windows (Run on Windows PC)

```powershell
# Open PowerShell in the project directory
.\build-windows.ps1
```

**What it does:**
- ? Creates self-contained .exe (includes .NET runtime)
- ? Single file executable (~75 MB)
- ? No dependencies needed
- ? Creates ZIP for distribution
- ? Adds README.txt

**Output:** `.\publish\windows\BLLMT.exe`

---

### macOS (Run on Mac)

```bash
# Open Terminal in the project directory
bash build-macos.sh
```

**What it does:**
- ? Creates universal .app bundle (Intel + Apple Silicon)
- ? Includes .NET runtime
- ? No dependencies needed
- ? Optionally creates DMG
- ? Adds README.txt

**Output:** `./publish/macos/BLLMT.app`

**Prerequisites:**
- macOS 11.0+
- .NET 9 SDK: https://dot.net
- Xcode from App Store
- Run: `sudo xcode-select --switch /Applications/Xcode.app/Contents/Developer`

---

## ?? What Gets Built

### Windows Build
```
publish/windows/
??? BLLMT.exe              (75 MB - single file with .NET runtime)
??? README.txt             (installation instructions)
??? (optional) BLLMT-Windows-v1.0.0.zip
```

### macOS Build
```
publish/macos/
??? BLLMT.app/             (Universal Binary)
?   ??? Contents/
?   ?   ??? MacOS/BLLMT    (binary)
?   ?   ??? Resources/
?   ?   ??? Info.plist
??? README.txt
??? (optional) BLLMT-macOS-v1.0.0.dmg
```

---

## ?? Quick Start (Step by Step)

### For Windows:

1. **Open PowerShell** (right-click project folder ? "Open in Terminal")

2. **Run build script:**
   ```powershell
   .\build-windows.ps1
   ```

3. **Wait for build** (~2-5 minutes first time)

4. **Test the app:**
   ```powershell
   .\publish\windows\BLLMT.exe
   ```

5. **Create distribution ZIP:**
   - Script will ask if you want to create a ZIP
   - Type `Y` and press Enter

6. **Done!** Upload `BLLMT-Windows-v1.0.0.zip` to GitHub Releases

---

### For macOS:

1. **Transfer files to Mac** (via Git, USB, cloud, etc.)

2. **Install prerequisites:**
   ```bash
   # Install .NET 9 SDK
   # Download from: https://dot.net
   
   # Install Xcode from App Store
   
   # Configure Xcode
   sudo xcode-select --switch /Applications/Xcode.app/Contents/Developer
   xcode-select --install
   ```

3. **Open Terminal** in project directory

4. **Make script executable:**
   ```bash
   chmod +x build-macos.sh
   ```

5. **Run build script:**
   ```bash
   ./build-macos.sh
   ```

6. **Wait for build** (~5-10 minutes first time)

7. **Test the app:**
   ```bash
   open ./publish/macos/BLLMT.app
   ```

8. **Create DMG:**
   - Script will ask if you want to create a DMG
   - Type `y` and press Enter

9. **Done!** Upload `BLLMT-macOS-v1.0.0.dmg` to GitHub Releases

---

## ?? Troubleshooting

### Windows Build Issues

**Error: "dotnet not found"**
```powershell
# Install .NET 9 SDK from: https://dot.net
# Restart PowerShell after installation
```

**Error: "Access denied"**
```powershell
# Run as administrator or disable antivirus temporarily
```

**Build succeeds but exe won't run:**
```powershell
# Windows Defender may block it
# Right-click exe ? Properties ? Unblock ? OK
```

---

### macOS Build Issues

**Error: "dotnet not found"**
```bash
# Install .NET 9 SDK from: https://dot.net
# Restart Terminal after installation
```

**Error: "Xcode not found"**
```bash
# Install Xcode from App Store
# Then run:
sudo xcode-select --switch /Applications/Xcode.app/Contents/Developer
xcode-select --install
```

**Error: "workload not installed"**
```bash
# Install MAUI workload:
dotnet workload install maui-maccatalyst
```

**App won't open after build:**
```bash
# Remove quarantine attribute:
xattr -dr com.apple.quarantine ./publish/macos/BLLMT.app
```

---

## ?? Build Sizes

| Platform | Self-Contained | Framework-Dependent |
|----------|----------------|---------------------|
| Windows  | ~75 MB         | ~5 MB               |
| macOS    | ~60 MB         | ~15 MB              |

**Recommendation:** Use self-contained builds for distribution (easier for users)

---

## ?? Customizing Builds

### Change Version Number

**Windows:** Edit `build-windows.ps1`, line 63:
```powershell
$version = "1.0.0"  # Change this
```

**macOS:** Edit `build-macos.sh`, line 123:
```bash
VERSION="1.0.0"  # Change this
```

### Change Output Location

**Both scripts:** Modify the `-o` parameter in the `dotnet publish` command

### Reduce File Size

Add these to the `dotnet publish` command:
```
-p:PublishTrimmed=true
-p:TrimMode=link
```

**Warning:** This may break reflection-based features. Test thoroughly!

---

## ?? Advanced: Code Signing

### Windows (Optional)

Requires code signing certificate (~$100-300/year):
```powershell
signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com .\publish\windows\BLLMT.exe
```

### macOS (Optional)

Requires Apple Developer Program ($99/year):
```bash
# Sign
codesign --deep --force --verify --verbose \
  --sign "Developer ID Application: Your Name (TEAM_ID)" \
  --options runtime \
  ./publish/macos/BLLMT.app

# Notarize
xcrun notarytool submit BLLMT-macOS-v1.0.0.dmg \
  --apple-id your@email.com \
  --team-id TEAM_ID \
  --password app-specific-password

# Staple (after approval)
xcrun stapler staple ./publish/macos/BLLMT.app
```

---

## ?? Checklist Before Release

### Both Platforms
- [ ] Build completes without errors
- [ ] App launches successfully
- [ ] Settings can be opened and saved
- [ ] Hotkeys work
- [ ] API calls succeed
- [ ] Typing emulation works
- [ ] Screenshots work (if permissions granted)
- [ ] App exits cleanly

### Windows Specific
- [ ] System tray icon appears
- [ ] Notifications show correctly
- [ ] No antivirus false positives

### macOS Specific
- [ ] Menu bar icon appears
- [ ] Permission dialogs appear
- [ ] Universal binary works on both Intel and Apple Silicon
- [ ] DMG mounts correctly

---

## ?? Distribution

### GitHub Releases (Recommended)

1. Go to GitHub repository
2. Click "Releases" ? "Create a new release"
3. Tag version (e.g., `v1.0.0`)
4. Upload:
   - `BLLMT-Windows-v1.0.0.zip`
   - `BLLMT-macOS-v1.0.0.dmg`
5. Write release notes
6. Publish!

### Homebrew (macOS)

See `DISTRIBUTION-STRATEGY.md` for creating a Homebrew tap

---

## ?? Tips

1. **First build takes longer** - subsequent builds are faster
2. **Test on clean machines** - VM or friend's computer
3. **Update version numbers** - before each release
4. **Keep build logs** - for troubleshooting
5. **Sign your apps** - when you're ready for wide distribution

---

## ?? Need Help?

- **Build errors:** Check error messages and troubleshooting section
- **Runtime errors:** Check logs (see QUICK-REFERENCE.md)
- **Questions:** Open an issue on GitHub

---

**Ready to build?**

Windows: `.\build-windows.ps1`  
macOS: `bash build-macos.sh`

**Let's go!** ??
