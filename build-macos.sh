#!/bin/bash
# Build standalone macOS version of BLLMT
# This creates a self-contained .app bundle that includes .NET runtime
# MUST BE RUN ON A MAC with Xcode installed

set -e  # Exit on error

echo "====================================="
echo "Building BLLMT for macOS (Standalone)"
echo "====================================="
echo ""

# Check if running on macOS
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "? ERROR: This script must be run on macOS"
    echo ""
    echo "To build for macOS:"
    echo "1. Copy this script to a Mac"
    echo "2. Install .NET 10 SDK: https://dot.net"
    echo "3. Install Xcode from the App Store"
    echo "4. Run: bash build-macos.sh"
    exit 1
fi

# Check for .NET SDK
if ! command -v dotnet &> /dev/null; then
    echo "? ERROR: .NET SDK not found"
    echo ""
    echo "Install .NET 10 SDK from: https://dot.net"
    exit 1
fi

# Check for Xcode
if ! command -v xcodebuild &> /dev/null; then
    echo "? ERROR: Xcode not found"
    echo ""
    echo "Install Xcode from the App Store"
    echo "Then run: sudo xcode-select --switch /Applications/Xcode.app/Contents/Developer"
    exit 1
fi

echo "? Prerequisites check passed"
echo ""

# Clean previous builds
echo "?? Cleaning previous builds..."
rm -rf ./publish/macos
rm -rf ./bin
rm -rf ./obj

# Build universal binary (Intel + Apple Silicon)
echo "?? Building universal binary for macOS..."
echo "   This will work on both Intel and Apple Silicon Macs"
echo ""

dotnet publish -f net10.0-maccatalyst \
    -c Release \
    --self-contained true \
    -p:CreatePackage=true \
    -p:EnableCompressionInSingleFile=true \
    -o "./publish/macos"

if [ $? -ne 0 ]; then
    echo ""
    echo "? Build FAILED!"
    echo "Please check the error messages above."
    exit 1
fi

echo ""
echo "====================================="
echo "? Build Successful!"
echo "====================================="
echo ""

# Find the .app bundle
APP_PATH=$(find ./publish/macos -name "BLLMT.app" -type d | head -n 1)

if [ -z "$APP_PATH" ]; then
    echo "??  Warning: Could not find BLLMT.app bundle"
    echo "Output is in: ./publish/macos/"
else
    echo "?? Output location: $APP_PATH"
    
    # Get bundle size
    BUNDLE_SIZE=$(du -sh "$APP_PATH" | cut -f1)
    echo "?? Bundle size: $BUNDLE_SIZE"
    echo ""
    
    # Check if it's a universal binary
    BINARY_PATH="$APP_PATH/Contents/MacOS/BLLMT"
    if [ -f "$BINARY_PATH" ]; then
        echo "?? Checking architecture support..."
        lipo -info "$BINARY_PATH"
        echo ""
    fi
fi

# Create a README
README_PATH="./publish/macos/README.txt"
cat > "$README_PATH" << 'EOF'
# BLLMT - macOS Standalone

This is a self-contained build of BLLMT for macOS.

## What's Included
- BLLMT.app (Universal Binary - Intel & Apple Silicon)
- .NET 9 runtime included
- No additional dependencies needed!

## Installation
1. Copy BLLMT.app to your Applications folder
2. Right-click BLLMT.app and select "Open" (first time only)
3. Click "Open" in the security dialog
4. The app will start in the menu bar

## First Run - Permissions
BLLMT will automatically guide you through granting required permissions:

1. Accessibility Permission (required for hotkeys)
   - System Settings > Privacy & Security > Accessibility
   - Enable BLLMT
   - Restart the app

2. Screen Recording Permission (optional, for screenshots)
   - System Settings > Privacy & Security > Screen Recording
   - Enable BLLMT
   - Restart the app

## Configuration
1. Click the BLLMT icon in the menu bar
2. Select 'Settings'
3. Configure your LLM API key and settings
4. Save and start using!

## System Requirements
- macOS 11.0 (Big Sur) or later
- Works on both Intel and Apple Silicon Macs

## Notes
- Settings are stored in: ~/Library/Application Support/BLLMT/settings.json
- The app runs in the menu bar (top-right corner)
- Hotkeys use Command (?) instead of Control

## Troubleshooting

### App won't open
Run this in Terminal:
xattr -dr com.apple.quarantine /Applications/BLLMT.app

### Permissions not working
Run this in Terminal:
tccutil reset All com.bllmt.app

Then restart BLLMT and grant permissions again.

### Check if app is running
Open Activity Monitor and search for "BLLMT"

For more information, visit: https://github.com/yourusername/bllmt
EOF

echo "?? README.txt created"
echo ""

# Offer to create DMG
echo "?? Create DMG for distribution? (y/n)"
read -r CREATE_DMG

if [[ "$CREATE_DMG" =~ ^[Yy]$ ]]; then
    echo ""
    echo "?? Creating DMG..."
    
    VERSION="1.0.0"  # Change this to your version
    DMG_PATH="./publish/BLLMT-macOS-v$VERSION.dmg"
    
    # Remove old DMG if exists
    rm -f "$DMG_PATH"
    
    # Create DMG
    if [ -n "$APP_PATH" ]; then
        hdiutil create "$DMG_PATH" \
            -volname "BLLMT" \
            -srcfolder "$APP_PATH" \
            -ov \
            -format UDZO
        
        if [ $? -eq 0 ]; then
            DMG_SIZE=$(du -sh "$DMG_PATH" | cut -f1)
            echo ""
            echo "? DMG created successfully!"
            echo "?? Location: $DMG_PATH"
            echo "?? Size: $DMG_SIZE"
            echo ""
            echo "This DMG is ready to upload to GitHub Releases!"
        fi
    else
        echo "? Could not create DMG - .app bundle not found"
    fi
fi

echo ""
echo "====================================="
echo "Next Steps:"
echo "====================================="
echo ""
echo "1. Test the app:"
echo "   open \"$APP_PATH\""
echo ""
echo "2. Sign the app (optional, requires Apple Developer account):"
echo "   codesign --deep --force --verify --verbose \\"
echo "     --sign \"Developer ID Application: Your Name\" \\"
echo "     \"$APP_PATH\""
echo ""
echo "3. Notarize the app (optional, requires Apple Developer account):"
echo "   See: https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution"
echo ""
echo "4. Upload to GitHub Releases"
echo ""
echo "Done! ??"
