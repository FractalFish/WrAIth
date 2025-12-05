using System.Runtime.InteropServices;
using AppKit;
using Foundation;

namespace BLLMT.Platforms.MacCatalyst
{
    /// <summary>
    /// Helper for checking and requesting macOS permissions
    /// Makes setup easier for users by guiding them through permission grants
    /// </summary>
    public static class PermissionHelper
    {
        [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
        private static extern bool AXIsProcessTrusted();

        /// <summary>
        /// Check all required permissions and guide user if missing
        /// </summary>
        /// <returns>True if all permissions granted</returns>
        public static async Task<bool> CheckAndRequestPermissionsAsync()
        {
            bool allGranted = true;

            // Check Accessibility Permission
            if (!HasAccessibilityPermission())
            {
                Log("Accessibility permission not granted");
                ShowAccessibilityPermissionAlert();
                allGranted = false;
            }
            else
            {
                Log("Accessibility permission: OK");
            }

            // Check Screen Recording Permission
            // Note: This is harder to check programmatically, so we guide user proactively
            if (!await HasScreenRecordingPermissionAsync())
            {
                Log("Screen recording permission may not be granted");
                ShowScreenRecordingPermissionAlert();
                allGranted = false;
            }
            else
            {
                Log("Screen recording permission: OK");
            }

            return allGranted;
        }

        /// <summary>
        /// Check if Accessibility permission is granted
        /// </summary>
        public static bool HasAccessibilityPermission()
        {
            return AXIsProcessTrusted();
        }

        /// <summary>
        /// Check if Screen Recording permission is granted
        /// This is a heuristic check - actual permission check requires trying to capture
        /// </summary>
        public static async Task<bool> HasScreenRecordingPermissionAsync()
        {
            try
            {
                // Try to take a small screenshot as a test
                var testCapture = CGWindowListCreateImage(
                    new CoreGraphics.CGRect(0, 0, 1, 1),
                    CGWindowListOption.OnScreenOnly,
                    0,
                    CGWindowImageOption.Default);

                if (testCapture != IntPtr.Zero)
                {
                    CFRelease(testCapture);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static void ShowAccessibilityPermissionAlert()
        {
            NSApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alert = new NSAlert
                {
                    MessageText = "Accessibility Permission Required",
                    InformativeText = "BLLMT needs Accessibility permission to:\n" +
                                    "• Register global hotkeys\n" +
                                    "• Simulate keyboard input\n" +
                                    "• Intercept keystrokes during typing\n\n" +
                                    "Click 'Open Settings' to grant permission, then restart BLLMT.",
                    AlertStyle = NSAlertStyle.Informational
                };
                alert.AddButton("Open Settings");
                alert.AddButton("Remind Me Later");

                var response = alert.RunModal();
                if (response == 1000) // First button (Open Settings)
                {
                    OpenAccessibilitySettings();
                }
            });
        }

        private static void ShowScreenRecordingPermissionAlert()
        {
            NSApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alert = new NSAlert
                {
                    MessageText = "Screen Recording Permission Recommended",
                    InformativeText = "BLLMT needs Screen Recording permission for:\n" +
                                    "• Screenshot capture feature\n" +
                                    "• Visual region selection\n\n" +
                                    "You can skip this if you don't plan to use screenshots.\n\n" +
                                    "Click 'Open Settings' to grant permission, then restart BLLMT.",
                    AlertStyle = NSAlertStyle.Informational
                };
                alert.AddButton("Open Settings");
                alert.AddButton("Skip for Now");

                var response = alert.RunModal();
                if (response == 1000) // First button
                {
                    OpenScreenRecordingSettings();
                }
            });
        }

        /// <summary>
        /// Open System Settings to Accessibility privacy settings
        /// </summary>
        public static void OpenAccessibilitySettings()
        {
            var url = new NSUrl("x-apple.systempreferences:com.apple.preference.security?Privacy_Accessibility");
            NSWorkspace.SharedWorkspace.OpenUrl(url);
            Log("Opened Accessibility settings");
        }

        /// <summary>
        /// Open System Settings to Screen Recording privacy settings
        /// </summary>
        public static void OpenScreenRecordingSettings()
        {
            var url = new NSUrl("x-apple.systempreferences:com.apple.preference.security?Privacy_ScreenCapture");
            NSWorkspace.SharedWorkspace.OpenUrl(url);
            Log("Opened Screen Recording settings");
        }

        /// <summary>
        /// Show a friendly welcome dialog with setup instructions
        /// </summary>
        public static void ShowWelcomeDialog()
        {
            NSApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alert = new NSAlert
                {
                    MessageText = "Welcome to BLLMT!",
                    InformativeText = "BLLMT is a background assistant that helps you type LLM responses.\n\n" +
                                    "To get started, BLLMT needs a few permissions:\n" +
                                    "• Accessibility (required for hotkeys)\n" +
                                    "• Screen Recording (optional for screenshots)\n\n" +
                                    "We'll guide you through granting these permissions.",
                    AlertStyle = NSAlertStyle.Informational
                };
                alert.AddButton("Continue");

                alert.RunModal();
            });
        }

        /// <summary>
        /// Show dialog when permissions are successfully granted
        /// </summary>
        public static void ShowSuccessDialog()
        {
            NSApplication.SharedApplication.InvokeOnMainThread(() =>
            {
                var alert = new NSAlert
                {
                    MessageText = "Setup Complete! ??",
                    InformativeText = "All permissions have been granted.\n\n" +
                                    "BLLMT is now ready to use!\n\n" +
                                    "Find BLLMT in your menu bar and click 'Settings' to configure your LLM API and hotkeys.",
                    AlertStyle = NSAlertStyle.Informational
                };
                alert.AddButton("OK");

                alert.RunModal();
            });
        }

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static extern IntPtr CGWindowListCreateImage(
            CoreGraphics.CGRect screenBounds,
            CGWindowListOption windowOption,
            uint windowID,
            CGWindowImageOption imageOption);

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        private static extern void CFRelease(IntPtr cf);

        private static void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [PermissionHelper] {message}");
            Console.WriteLine($"[{timestamp}] [PermissionHelper] {message}");
        }
    }

    // Enums for CGWindowListCreateImage
    public enum CGWindowListOption : uint
    {
        All = 0,
        OnScreenOnly = 1,
        OnScreenAboveWindow = 2,
        OnScreenBelowWindow = 4,
        IncludingWindow = 8,
        ExcludeDesktopElements = 16
    }

    public enum CGWindowImageOption : uint
    {
        Default = 0,
        BoundsIgnoreFraming = 1,
        ShouldBeOpaque = 2,
        OnlyShadows = 4,
        BestResolution = 8,
        NominalResolution = 16
    }
}
