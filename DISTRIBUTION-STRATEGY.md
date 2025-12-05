# Distribution Strategy for Minimal Setup

## Goal: Make BLLMT as easy to install on Mac as it is on Windows

## Windows Current State ?
- Single `.exe` file
- No installer needed
- Permissions requested on first run
- Works immediately

## Mac Target State ??
- Single `.app` bundle
- Drag to Applications
- Permissions requested on first run
- Works immediately

---

## Strategy 1: Notarized DMG (RECOMMENDED)

### What is it?
A disk image (.dmg) that contains your app, similar to how most Mac apps are distributed.

### Steps for Users:
```
1. Download BLLMT.dmg
2. Open DMG
3. Drag BLLMT.app to Applications folder
4. Launch BLLMT
5. Grant permissions when prompted
6. Done!
```

### Advantages:
- ? Standard Mac distribution method
- ? Familiar to Mac users
- ? No Gatekeeper warnings (when notarized)
- ? Professional appearance
- ? Can include README/documentation

### Implementation:
```bash
# Create DMG
hdiutil create -volname "BLLMT" \
    -srcfolder "bin/Release/net10.0-maccatalyst/publish/BLLMT.app" \
    -ov -format UDZO BLLMT.dmg

# Sign and notarize
xcrun notarytool submit BLLMT.dmg \
    --apple-id your@email.com \
    --team-id YOURTEAMID \
    --password app-specific-password
```

### Cost:
- **$99/year** for Apple Developer Program (required for notarization)

---

## Strategy 2: Homebrew Distribution (EASIEST FOR USERS)

### What is it?
Mac package manager - users install with one command.

### Steps for Users:
```bash
# First time setup
brew tap yourusername/bllmt

# Install
brew install bllmt

# Launch
bllmt
```

### Advantages:
- ? **Zero manual steps** for users
- ? Automatic updates via `brew upgrade`
- ? Popular with developers
- ? No notarization needed
- ? FREE!

### Implementation:
```ruby
# Formula file: homebrew-bllmt/Formula/bllmt.rb
class Bllmt < Formula
  desc "Background LLM Typing Assistant"
  homepage "https://github.com/yourusername/bllmt"
  url "https://github.com/yourusername/bllmt/releases/download/v1.0/bllmt-macos.tar.gz"
  sha256 "..."
  version "1.0"

  def install
    prefix.install "BLLMT.app"
    bin.write_exec_script "#{prefix}/BLLMT.app/Contents/MacOS/BLLMT"
  end

  def caveats
    <<~EOS
      BLLMT requires Accessibility and Screen Recording permissions.
      
      Grant permissions in:
      System Settings > Privacy & Security > Accessibility
      System Settings > Privacy & Security > Screen Recording
      
      Then restart BLLMT.
    EOS
  end
end
```

---

## Strategy 3: GitHub Releases (SIMPLEST)

### What is it?
Direct download from GitHub, no fancy distribution.

### Steps for Users:
```
1. Go to GitHub releases
2. Download BLLMT.app.zip
3. Unzip
4. Move to Applications
5. Right-click > Open (first time only)
6. Grant permissions
```

### Advantages:
- ? **Completely free**
- ? No Apple Developer account needed
- ? Simple for you to maintain
- ? Works immediately

### Disadvantages:
- ? Gatekeeper warning on first run (user must right-click > Open)
- ? Less professional
- ? Manual updates

---

## Strategy 4: Auto-Permission Request (IN-APP)

### Make the app smart about permissions:

```csharp
// Add to MacMenuBarManager or startup code
public class PermissionHelper
{
    public static async Task<bool> RequestPermissionsAsync()
    {
        bool allGranted = true;
        
        // Check Accessibility
        if (!AXIsProcessTrusted())
        {
            var alert = new NSAlert
            {
                MessageText = "Accessibility Permission Required",
                InformativeText = "BLLMT needs Accessibility permission to register global hotkeys.\n\nClick 'Open Settings' to grant permission.",
                AlertStyle = NSAlertStyle.Informational
            };
            alert.AddButton("Open Settings");
            alert.AddButton("Cancel");
            
            var response = alert.RunModal();
            if (response == 1000) // First button
            {
                OpenAccessibilitySettings();
            }
            allGranted = false;
        }
        
        // Check Screen Recording
        if (!HasScreenRecordingPermission())
        {
            var alert = new NSAlert
            {
                MessageText = "Screen Recording Permission Required",
                InformativeText = "BLLMT needs Screen Recording permission for screenshot capture.\n\nClick 'Open Settings' to grant permission.",
                AlertStyle = NSAlertStyle.Informational
            };
            alert.AddButton("Open Settings");
            alert.AddButton("Cancel");
            
            var response = alert.RunModal();
            if (response == 1000)
            {
                OpenScreenRecordingSettings();
            }
            allGranted = false;
        }
        
        return allGranted;
    }
    
    private static void OpenAccessibilitySettings()
    {
        NSWorkspace.SharedWorkspace.OpenUrl(
            new NSUrl("x-apple.systempreferences:com.apple.preference.security?Privacy_Accessibility"));
    }
    
    private static void OpenScreenRecordingSettings()
    {
        NSWorkspace.SharedWorkspace.OpenUrl(
            new NSUrl("x-apple.systempreferences:com.apple.preference.security?Privacy_ScreenCapture"));
    }
}
```

---

## RECOMMENDED COMBINATION

### For Maximum User Friendliness:

**Phase 1: GitHub + Smart Permissions (Free, Immediate)**
1. Distribute via GitHub Releases
2. Include in-app permission helper
3. Provide clear setup instructions

**Phase 2: Homebrew (Free, Better UX)**
1. Create Homebrew tap once app is stable
2. Simplifies installation to one command
3. Automatic updates

**Phase 3: Notarized DMG (Professional, $99/year)**
1. Once app is popular enough
2. Get Apple Developer account
3. Remove Gatekeeper warnings
4. Professional distribution

---

## Quick Start: Minimal Setup Checklist

### For You (Developer):
- [ ] Build universal binary (Apple Silicon + Intel)
- [ ] Add permission request dialogs to app
- [ ] Create simple README with permission instructions
- [ ] Zip the .app bundle
- [ ] Upload to GitHub Releases

### For Users:
1. **Download** BLLMT.app.zip from GitHub
2. **Unzip** and move to Applications
3. **Right-click** > Open (first time only, bypasses Gatekeeper)
4. **Grant permissions** when prompted by the app
5. **Configure** settings via menu bar icon
6. **Done!**

### Permission Flow (Auto-guided):
```
1. Launch BLLMT
2. App detects missing permissions
3. Shows friendly dialog: "Need Accessibility permission"
4. User clicks "Open Settings"
5. macOS Settings opens to correct page
6. User enables BLLMT
7. Restart BLLMT
8. App works!
```

This achieves **near-Windows simplicity** without requiring $99/year!
