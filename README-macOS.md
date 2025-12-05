# BLLMT for macOS - Setup and Usage Guide

## macOS-Specific Requirements

BLLMT on macOS requires several system permissions to function properly. This is normal for applications that use global hotkeys and keyboard automation.

### Required Permissions

1. **Accessibility Permission**
   - Required for: Global hotkeys, keyboard simulation
   - Location: System Settings > Privacy & Security > Accessibility
   - Add BLLMT to the list and enable it

2. **Screen Recording Permission**
   - Required for: Screenshot capture feature
   - Location: System Settings > Privacy & Security > Screen Recording
   - Add BLLMT to the list and enable it

3. **Input Monitoring** (macOS 10.15+)
   - Required for: Keyboard event interception
   - Location: System Settings > Privacy & Security > Input Monitoring
   - Add BLLMT to the list and enable it

### First Launch

On first launch, macOS will prompt you for these permissions. You must grant them for BLLMT to work properly. If you accidentally deny them:

1. Open System Settings
2. Navigate to Privacy & Security
3. Find and enable BLLMT in each required section
4. Restart BLLMT

## macOS-Specific Features

### Menu Bar Application

Unlike Windows' system tray, BLLMT appears in the macOS menu bar (top-right corner):
- Click the icon to see the menu
- Status updates appear in the menu
- Access Settings from here

### Hotkey Differences

macOS uses different modifier keys than Windows:

| Windows | macOS |
|---------|-------|
| Control | Command (?) |
| Alt | Option (?) |
| Windows Key | Command (?) |

**Example:** Windows `Ctrl+Shift+Q` becomes macOS `?+Shift+Q`

When configuring hotkeys in settings, use these names:
- `Command` or `Cmd` for ?
- `Option` or `Alt` for ?
- `Control` for Control key
- `Shift` for Shift

### Notifications

macOS notifications appear in Notification Center instead of balloon tips. Configure notification settings in System Settings > Notifications > BLLMT.

## Installation

### Building from Source

```bash
# Prerequisites
# - .NET 10 SDK
# - Xcode 15+ (for macOS SDKs)

# Clone repository
git clone https://github.com/yourusername/BLLMT.git
cd BLLMT

# Build for macOS
dotnet build -f net10.0-maccatalyst

# Run
dotnet run -f net10.0-maccatalyst
```

### Publishing

```bash
# Create release build
dotnet publish -f net10.0-maccatalyst -c Release

# The .app bundle will be in:
# bin/Release/net10.0-maccatalyst/publish/
```

## Configuration

Settings are stored in:
```
~/Library/Application Support/BLLMT/settings.json
```

You can manually edit this file if needed, but it's recommended to use the Settings window.

## Troubleshooting

### Hotkeys Not Working

1. **Check Accessibility permissions**:
   - System Settings > Privacy & Security > Accessibility
   - Ensure BLLMT is listed and enabled
   - Restart BLLMT after enabling

2. **Hotkey conflicts**:
   - macOS reserves some key combinations
   - Avoid using hotkeys that match system shortcuts
   - Common conflicts: ?+Space (Spotlight), ?+Tab (App Switcher)

3. **Check Console output**:
   ```bash
   # View logs
   log stream --predicate 'process == "BLLMT"' --level debug
   ```

### Keyboard Simulation Not Working

1. **Verify Input Monitoring permission**
2. **Check if target application accepts synthetic input**:
   - Some secure applications block synthetic keyboard events
   - This is a security feature and cannot be bypassed

3. **Try restarting BLLMT**

### Screenshot Capture Issues

1. **Grant Screen Recording permission**:
   - System Settings > Privacy & Security > Screen Recording
   - Enable BLLMT

2. **Restart BLLMT after granting permission**

3. **Note**: Some applications prevent screenshots (DRM content, secure fields)

### API Connection Issues

1. **Check network connectivity**
2. **Verify API key is correct**
3. **Check firewall settings**:
   - System Settings > Network > Firewall
   - Ensure BLLMT is allowed

### Application Won't Launch

1. **Remove quarantine attribute** (if downloaded from internet):
   ```bash
   xattr -dr com.apple.quarantine /Applications/BLLMT.app
   ```

2. **Check Console for crash logs**:
   ```bash
   log show --predicate 'process == "BLLMT"' --last 1h
   ```

3. **Reset permissions**:
   ```bash
   tccutil reset All com.bllmt.app
   ```
   Then relaunch and grant permissions again

## Known Limitations

1. **Secure Input Mode**: Some applications (like password managers) enable secure input mode, which prevents BLLMT from intercepting keystrokes. This is by design for security.

2. **System-wide Hotkeys**: Some hotkey combinations are reserved by macOS or other applications and cannot be used.

3. **App Sandbox**: BLLMT runs without App Sandbox restrictions to enable global hotkeys. This means it won't be available on the Mac App Store.

4. **Screen Recording**: Required permission allows BLLMT to capture screenshots but also technically allows it to record the entire screen (though it doesn't).

## Privacy & Security

BLLMT requires extensive permissions, which may seem concerning. Here's what it does:

### What BLLMT Accesses:
- **Keyboard input**: Only when emulation is active (after you press output hotkey)
- **Screen captures**: Only when you explicitly trigger screenshot hotkey
- **Clipboard**: Only when you press trigger hotkey
- **Network**: Only to send/receive API requests to your configured LLM provider

### What BLLMT Does NOT Do:
- Record or log keystrokes (except during active emulation)
- Send data anywhere except your configured API endpoint
- Access files outside its application support folder
- Run in background without your knowledge (menu bar icon always visible)

### Open Source
BLLMT is open source. You can review the code to verify its behavior.

## Support

- Issues: https://github.com/yourusername/BLLMT/issues
- Discussions: https://github.com/yourusername/BLLMT/discussions

## Alternative: Using Homebrew

```bash
# Coming soon
brew tap yourusername/bllmt
brew install bllmt
```

## macOS Version Requirements

- **Minimum**: macOS 11.0 (Big Sur)
- **Recommended**: macOS 14.0 (Sonoma) or later
- **Tested on**: macOS 14.x, 15.x

## Code Signing

For distribution, you'll need to sign the application with an Apple Developer ID:

```bash
# Sign the app
codesign --deep --force --verify --verbose --sign "Developer ID Application: Your Name" BLLMT.app

# Verify signature
codesign --verify --deep --strict --verbose=2 BLLMT.app

# Notarize (required for macOS 10.15+)
xcrun notarytool submit BLLMT.app --apple-id your@email.com --team-id TEAMID --password app-specific-password
```

## Uninstallation

```bash
# Remove application
rm -rf /Applications/BLLMT.app

# Remove settings
rm -rf ~/Library/Application\ Support/BLLMT

# Remove from security permissions (optional)
tccutil reset All com.bllmt.app
```
