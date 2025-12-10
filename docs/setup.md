# BLLMT - Cross-Platform Setup Guide

## ?? Quick Start

### Windows
```powershell
# Build and run
dotnet run -f net10.0-windows

# Or publish
dotnet publish -f net10.0-windows -c Release
```

### macOS
```bash
# Build and run
dotnet run -f net10.0-maccatalyst

# Or publish
dotnet publish -f net10.0-maccatalyst -c Release
```

---

## ??? Architecture Overview

BLLMT now uses a **cross-platform architecture** with platform-specific implementations:

```
???????????????????????????????????????
?         Your Application            ?
?         (Form1, Services)           ?
???????????????????????????????????????
               ?
???????????????????????????????????????
?     Platform Interfaces             ?
?  (IHotkeyManager, IKeyboard, etc)   ?
???????????????????????????????????????
               ?
       ?????????????????
       ?               ?
??????????????  ??????????????
?  Windows   ?  ?   macOS    ?
?    Impl    ?  ?    Impl    ?
??????????????  ??????????????
```

### Key Components

1. **Interfaces/** - Platform-agnostic service contracts
2. **Services/** - Shared business logic & platform factory
3. **Platforms/Windows/** - Windows-specific implementations
4. **Platforms/MacCatalyst/** - macOS-specific implementations

---

## ?? Dependencies

### Windows (.NET 10.0-windows)
- Windows Forms
- System.Drawing
- Native Win32 APIs (user32.dll, kernel32.dll)

### macOS (.NET 10.0-maccatalyst)
- Microsoft.Maui
- AppKit (NSStatusBar, NSAlert)
- CoreGraphics (CGEvent, CGWindowListCreateImage)
- Foundation (NSUserNotification)

---

## ?? Development Setup

### Prerequisites

**Windows:**
- .NET 10 SDK
- Visual Studio 2022 (17.8+) or VS Code
- Windows 10/11

**macOS:**
- .NET 10 SDK
- Xcode 15+
- macOS 11.0+ (Big Sur or later)

### Clone and Build

```bash
git clone https://github.com/yourusername/bllmt.git
cd bllmt

# Windows
dotnet build -f net10.0-windows

# macOS
dotnet build -f net10.0-maccatalyst
```

---

## ?? First Run Experience

### Windows

1. Launch BLLMT.exe
2. Icon appears in system tray
3. Configure settings (API key, hotkeys)
4. Start using immediately!

**No permissions needed** - Windows allows hotkey registration by default.

### macOS

1. Launch BLLMT.app
2. **Automatic permission check** runs
3. Friendly dialogs guide you:
   - ? Accessibility permission (for hotkeys)
   - ? Screen Recording (for screenshots)
4. Click "Open Settings" ? macOS settings opens
5. Enable BLLMT
6. Restart app
7. **Setup complete!** ??

**The app automatically:**
- Detects missing permissions
- Shows user-friendly dialogs
- Opens correct System Settings page
- Validates permissions on restart

---

## ?? Platform Differences

### Hotkey Notation

| Feature | Windows | macOS |
|---------|---------|-------|
| Primary Modifier | `Control` | `Command` (?) |
| Alt Key | `Alt` | `Option` (?) |
| Example | `Control+Shift+Q` | `Command+Shift+Q` |

**Note:** In code, use `"Control+Shift+Q"` - it automatically maps to Command on Mac!

### UI Elements

| Feature | Windows | macOS |
|---------|---------|-------|
| System Tray | NotifyIcon | NSStatusBar (menu bar) |
| Notifications | Balloon Tips | Notification Center |
| Dialogs | MessageBox | NSAlert |
| Settings | Windows Forms | MAUI (coming soon) |

---

## ?? Permissions Explained

### macOS Permissions (Required)

#### 1. Accessibility
**Why:** Global hotkey registration and keyboard event interception
**Without it:** Hotkeys won't work
**Grant:** System Settings > Privacy & Security > Accessibility

#### 2. Screen Recording
**Why:** Screenshot capture feature
**Without it:** Screenshots won't work (text features still work)
**Grant:** System Settings > Privacy & Security > Screen Recording

### Security Notes

- ? **Open source** - Verify what the app does
- ? **Local processing** - Only sends to your configured API
- ? **No logging** - Doesn't record keystrokes outside emulation
- ? **Transparent** - Menu bar icon always visible

---

## ?? Building for Distribution

### Windows Distribution

```powershell
# Self-contained (includes .NET runtime)
dotnet publish -f net10.0-windows -c Release -r win-x64 --self-contained

# Framework-dependent (smaller, requires .NET 10)
dotnet publish -f net10.0-windows -c Release -r win-x64 --no-self-contained

# Output: bin/Release/net10.0-windows/win-x64/publish/
```

**Result:** Single executable, ~5MB (framework-dependent) or ~70MB (self-contained)

### macOS Distribution

```bash
# Universal binary (Intel + Apple Silicon)
dotnet publish -f net10.0-maccatalyst -c Release

# Output: bin/Release/net10.0-maccatalyst/publish/BLLMT.app
```

**Result:** .app bundle, ready to distribute

#### Package as DMG (Recommended)

```bash
# Create DMG
hdiutil create -volname "BLLMT" \
    -srcfolder "bin/Release/net10.0-maccatalyst/publish/BLLMT.app" \
    -ov -format UDZO BLLMT.dmg
```

#### Sign and Notarize (For distribution)

```bash
# Requires Apple Developer Program ($99/year)
codesign --deep --force --verify --verbose \
    --sign "Developer ID Application: Your Name" \
    BLLMT.app

xcrun notarytool submit BLLMT.dmg \
    --apple-id your@email.com \
    --team-id TEAMID \
    --password app-specific-password
```

---

## ?? Testing

### Windows Testing
```powershell
dotnet test -f net10.0-windows
```

### macOS Testing
```bash
dotnet test -f net10.0-maccatalyst
```

### Manual Testing Checklist

**Both Platforms:**
- [ ] App launches successfully
- [ ] Settings can be opened and saved
- [ ] API key is stored securely
- [ ] Hotkeys are registered
- [ ] Clipboard processing works
- [ ] LLM API calls succeed
- [ ] Typing emulation works
- [ ] Screenshot capture works (if permissions granted)
- [ ] Notifications appear
- [ ] App exits cleanly

**macOS Specific:**
- [ ] Permission dialogs appear on first run
- [ ] System Settings opens to correct page
- [ ] Menu bar icon appears
- [ ] Universal binary works on Intel and Apple Silicon

---

## ?? Configuration

Settings location:
- **Windows:** `%APPDATA%\BLLMT\settings.json`
- **macOS:** `~/Library/Application Support/BLLMT/settings.json`

### Example settings.json

```json
{
  "Models": [
    {
      "Id": "...",
      "Name": "GPT-4o Mini",
      "Provider": "OpenAI",
      "ApiKey": "sk-...",
      "Model": "gpt-4o-mini",
      "Endpoint": "https://api.openai.com/v1/chat/completions",
      "SystemPrompt": "You are a helpful assistant.",
      "SupportsVision": false,
      "IsDefault": true,
      "TriggerHotkey": "Control+Shift+Q",
      "OutputHotkey": "Control+Shift+W",
      "AbortHotkey": "Control+Shift+E"
    }
  ],
  "TypingDelayMs": 50,
  "TypingVariationMs": 20
}
```

---

## ?? Troubleshooting

### Windows Issues

**Hotkeys not working:**
- Check if another app is using the same hotkey
- Try running as administrator
- Restart the application

**Typing not working:**
- Ensure target app accepts input
- Check keyboard simulator settings
- Verify output hotkey is correct

### macOS Issues

**Hotkeys not working:**
1. Check Accessibility permission:
   ```bash
   # Run from Terminal
   sqlite3 ~/Library/Application\ Support/com.apple.TCC/TCC.db \
   "SELECT * FROM access WHERE client='com.bllmt.app';"
   ```
2. Grant permission in System Settings
3. Restart BLLMT

**Screenshots not working:**
1. Grant Screen Recording permission
2. Restart BLLMT
3. Test with screenshot hotkey

**App won't launch:**
```bash
# Remove quarantine (if downloaded from internet)
xattr -dr com.apple.quarantine /Applications/BLLMT.app

# Check logs
log show --predicate 'process == "BLLMT"' --last 5m
```

---

## ?? Additional Resources

- [README-macOS.md](README-macOS.md) - macOS-specific details
- [DISTRIBUTION-STRATEGY.md](DISTRIBUTION-STRATEGY.md) - Distribution options
- [Scripts/setup-macos.sh](Scripts/setup-macos.sh) - Permission check script

---

## ?? Contributing

1. Fork the repository
2. Create your feature branch
3. Test on both Windows and macOS
4. Submit a pull request

### Code Style
- Use interfaces for platform-specific code
- Put platform code in `Platforms/Windows/` or `Platforms/MacCatalyst/`
- Shared logic goes in `Services/`
- Follow existing naming conventions

---

## ?? License

[Your License Here]

---

## ? What's Next?

- [ ] MAUI-based settings UI (works on both platforms)
- [ ] Linux support
- [ ] Cloud sync for settings
- [ ] Model chaining improvements
- [ ] Plugin system

---

**Built with .NET 10, runs everywhere!** ??
