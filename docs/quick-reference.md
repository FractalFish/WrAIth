# Quick Reference: Building & Running BLLMT

## ?? Quick Commands

### Windows
```powershell
# Development
dotnet run -f net10.0-windows

# Release Build
dotnet publish -f net10.0-windows -c Release

# Self-Contained (includes .NET)
dotnet publish -f net10.0-windows -c Release --self-contained -r win-x64
```

### macOS
```bash
# Development
dotnet run -f net10.0-maccatalyst

# Release Build
dotnet publish -f net10.0-maccatalyst -c Release

# Create DMG
hdiutil create -volname "BLLMT" -srcfolder "bin/Release/net10.0-maccatalyst/publish/BLLMT.app" -ov -format UDZO BLLMT.dmg
```

---

## ?? Common Tasks

### Add a New Platform Service

1. Define interface in `Interfaces/`
2. Implement in `Platforms/Windows/`
3. Implement in `Platforms/MacCatalyst/`
4. Add factory method in `Services/PlatformServiceFactory.cs`

### Test Permissions on Mac

```bash
# Check accessibility permission
sqlite3 ~/Library/Application\ Support/com.apple.TCC/TCC.db "SELECT * FROM access WHERE client='com.bllmt.app' AND service='kTCCServiceAccessibility';"

# Check screen recording permission  
sqlite3 ~/Library/Application\ Support/com.apple.TCC/TCC.db "SELECT * FROM access WHERE client='com.bllmt.app' AND service='kTCCServiceScreenCapture';"

# Or use the setup script
bash Scripts/setup-macos.sh
```

### Reset macOS Permissions

```bash
# Reset all permissions
tccutil reset All com.bllmt.app

# Reset specific permission
tccutil reset Accessibility com.bllmt.app
tccutil reset ScreenCapture com.bllmt.app
```

### View Logs

**Windows:**
```powershell
# Visual Studio Output window
# Or use Debug Viewer
```

**macOS:**
```bash
# Real-time logs
log stream --predicate 'process == "BLLMT"' --level debug

# Last 5 minutes
log show --predicate 'process == "BLLMT"' --last 5m

# Search for errors
log show --predicate 'process == "BLLMT" AND eventMessage CONTAINS "ERROR"' --last 1h
```

---

## ?? Debug Tricks

### Windows Debugging
- Run in Visual Studio with F5
- Breakpoints work normally
- Output window shows logs

### macOS Debugging
- Run in Visual Studio for Mac or VS Code
- Use `Console.WriteLine()` liberally
- Check macOS Console app for system logs
- Use `lldb` if needed

### Platform-Specific Code
```csharp
#if WINDOWS
    // Windows-only code
#elif MACCATALYST
    // macOS-only code
#else
    // Fallback
#endif
```

### Check Platform at Runtime
```csharp
using BLLMT.Services;

if (PlatformServiceFactory.IsWindows())
{
    // Windows-specific logic
}
else if (PlatformServiceFactory.IsMacOS())
{
    // macOS-specific logic
}
```

---

## ?? Distribution Checklist

### Windows
- [ ] Build release: `dotnet publish -f net10.0-windows -c Release`
- [ ] Test on clean Windows machine
- [ ] Verify hotkeys work
- [ ] Check API calls succeed
- [ ] Test typing emulation
- [ ] Zip the publish folder
- [ ] Create GitHub release
- [ ] Document system requirements

### macOS
- [ ] Build release: `dotnet publish -f net10.0-maccatalyst -c Release`
- [ ] Test on Intel Mac
- [ ] Test on Apple Silicon Mac
- [ ] Verify permissions request
- [ ] Check hotkeys work (Command, not Control!)
- [ ] Test all features
- [ ] Create DMG: `hdiutil create...`
- [ ] (Optional) Sign with Developer ID
- [ ] (Optional) Notarize
- [ ] Create GitHub release
- [ ] Document permission requirements

---

## ?? UI Customization

### Change Menu Bar Icon (macOS)

1. Add icon to `Resources/` folder
2. Update `MacMenuBarManager.cs`:
```csharp
_statusItem.Button.Image = NSImage.ImageNamed("YourIconName");
```

### Change System Tray Icon (Windows)

1. Add icon to project
2. Update `Form1.Designer.cs`:
```csharp
this.notifyIcon.Icon = new System.Drawing.Icon("path/to/icon.ico");
```

---

## ?? Signing & Notarization (macOS)

### Get Apple Developer Certificate

1. Join Apple Developer Program ($99/year)
2. Generate Developer ID certificate in Xcode
3. Download and install in Keychain

### Sign App

```bash
# Find your certificate
security find-identity -v -p codesigning

# Sign
codesign --deep --force --verify --verbose \
  --sign "Developer ID Application: Your Name (TEAM_ID)" \
  --options runtime \
  BLLMT.app

# Verify
codesign --verify --deep --strict --verbose=2 BLLMT.app
spctl --assess --verbose=4 BLLMT.app
```

### Notarize App

```bash
# Create app-specific password in Apple ID account
# https://appleid.apple.com/account/manage

# Create ZIP
ditto -c -k --keepParent BLLMT.app BLLMT.zip

# Submit for notarization
xcrun notarytool submit BLLMT.zip \
  --apple-id your@email.com \
  --team-id TEAM_ID \
  --password xxxx-xxxx-xxxx-xxxx

# Check status
xcrun notarytool info SUBMISSION_ID \
  --apple-id your@email.com \
  --team-id TEAM_ID \
  --password xxxx-xxxx-xxxx-xxxx

# Staple ticket (once approved)
xcrun stapler staple BLLMT.app
```

---

## ?? File Sizes

### Windows
- Framework-dependent: ~5-10 MB
- Self-contained: ~70-80 MB
- Single-file: ~75-85 MB

### macOS
- Universal binary: ~15-25 MB
- DMG (compressed): ~10-15 MB

---

## ?? Useful Links

- [.NET MAUI Docs](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Mac Catalyst Docs](https://developer.apple.com/mac-catalyst/)
- [macOS Permissions](https://developer.apple.com/documentation/security/requesting_access_to_protected_resources)
- [App Notarization](https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution)
- [Homebrew Tap Guide](https://docs.brew.sh/How-to-Create-and-Maintain-a-Tap)

---

## ?? Emergency Fixes

### Build Errors on Mac

```bash
# Clean and rebuild
dotnet clean
rm -rf bin obj
dotnet build -f net10.0-maccatalyst

# Update workload
dotnet workload update
dotnet workload install maui-maccatalyst
```

### Permissions Lost (Mac)

```bash
# Quit app
# Reset permissions
tccutil reset All com.bllmt.app
# Restart app - permissions will be re-requested
```

### Hotkeys Stop Working (Windows)

```powershell
# Restart app
# Try different hotkey combination
# Run as administrator (if needed)
```

---

## ?? Pro Tips

### Development
- Use `#if DEBUG` for debug-only code
- Log platform in startup: `Log($"Platform: {PlatformServiceFactory.GetPlatformName()}")`
- Test on both platforms regularly

### Performance
- macOS: Keep UI updates on main thread (`NSApplication.SharedApplication.InvokeOnMainThread`)
- Windows: Use `Invoke` for UI thread access
- Both: Async/await for API calls

### User Experience
- Show progress for long operations
- Clear error messages with solutions
- Save settings immediately
- Restore state on restart

---

## ?? Getting Help

1. Check logs (see "View Logs" above)
2. Search [Issues](https://github.com/yourusername/bllmt/issues)
3. Review documentation
4. Ask in [Discussions](https://github.com/yourusername/bllmt/discussions)
5. Create new issue with:
   - Platform (Windows/macOS)
   - .NET version
   - macOS version (if Mac)
   - Steps to reproduce
   - Error logs

---

**Happy Coding! ??**
