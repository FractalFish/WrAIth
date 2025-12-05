#!/bin/bash
# BLLMT First-Run Setup Helper for macOS
# This script helps users set up required permissions

set -e

APP_NAME="BLLMT"
BUNDLE_ID="com.bllmt.app"

echo "=== BLLMT Setup Helper ==="
echo ""

# Check if running on macOS
if [[ "$OSTYPE" != "darwin"* ]]; then
    echo "Error: This script is for macOS only."
    exit 1
fi

# Function to check if app has accessibility permission
check_accessibility() {
    # This will return true if app is trusted
    if command -v sqlite3 &> /dev/null; then
        local result=$(sqlite3 /Library/Application\ Support/com.apple.TCC/TCC.db \
            "SELECT allowed FROM access WHERE client='${BUNDLE_ID}' AND service='kTCCServiceAccessibility';" 2>/dev/null || echo "0")
        
        if [[ "$result" == "1" ]]; then
            echo "? Accessibility: Granted"
            return 0
        fi
    fi
    
    echo "? Accessibility: Not granted"
    return 1
}

# Function to check screen recording permission
check_screen_recording() {
    if command -v sqlite3 &> /dev/null; then
        local result=$(sqlite3 /Library/Application\ Support/com.apple.TCC/TCC.db \
            "SELECT allowed FROM access WHERE client='${BUNDLE_ID}' AND service='kTCCServiceScreenCapture';" 2>/dev/null || echo "0")
        
        if [[ "$result" == "1" ]]; then
            echo "? Screen Recording: Granted"
            return 0
        fi
    fi
    
    echo "? Screen Recording: Not granted"
    return 1
}

echo "Checking required permissions..."
echo ""

accessibility_ok=false
screen_ok=false

check_accessibility && accessibility_ok=true || accessibility_ok=false
check_screen_recording && screen_ok=true || screen_ok=false

echo ""

if $accessibility_ok && $screen_ok; then
    echo "?? All permissions granted! BLLMT is ready to use."
    echo ""
    echo "Launch BLLMT from your Applications folder."
    exit 0
fi

echo "??  BLLMT needs additional permissions to function."
echo ""
echo "Please follow these steps:"
echo ""

if ! $accessibility_ok; then
    echo "1. Accessibility Permission:"
    echo "   • Open System Settings"
    echo "   • Go to Privacy & Security > Accessibility"
    echo "   • Click the lock to make changes"
    echo "   • Find 'BLLMT' and enable it"
    echo ""
fi

if ! $screen_ok; then
    echo "2. Screen Recording Permission:"
    echo "   • Open System Settings"
    echo "   • Go to Privacy & Security > Screen Recording"
    echo "   • Click the lock to make changes"
    echo "   • Find 'BLLMT' and enable it"
    echo ""
fi

echo "After granting permissions, restart BLLMT."
echo ""

# Ask if user wants to open System Settings
read -p "Open System Settings now? (y/n) " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo "Opening System Settings..."
    open "x-apple.systempreferences:com.apple.preference.security?Privacy_Accessibility"
fi

exit 0
