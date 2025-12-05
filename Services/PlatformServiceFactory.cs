using BLLMT.Interfaces;

namespace BLLMT.Services
{
    /// <summary>
    /// Factory for creating platform-specific service implementations
    /// Automatically selects Windows or macOS implementations based on compilation target
    /// </summary>
    public static class PlatformServiceFactory
    {
        /// <summary>
        /// Create platform-specific hotkey manager
        /// </summary>
        /// <param name="messageWindow">Windows Form reference (Windows only, can be null on Mac)</param>
        public static IHotkeyManager CreateHotkeyManager(object? messageWindow = null)
        {
#if WINDOWS
            if (messageWindow is System.Windows.Forms.Form form)
            {
                return new Platforms.Windows.WindowsHotkeyManager(form);
            }
            throw new ArgumentException("Windows platform requires a Form instance", nameof(messageWindow));
#elif MACCATALYST
            return new Platforms.MacCatalyst.MacHotkeyManager();
#else
            throw new PlatformNotSupportedException("Current platform is not supported");
#endif
        }

        /// <summary>
        /// Create platform-specific keyboard simulator
        /// </summary>
        public static IKeyboardSimulator CreateKeyboardSimulator(int baseDelayMs = 50, int variationMs = 20)
        {
#if WINDOWS
            return new Platforms.Windows.WindowsKeyboardSimulator(baseDelayMs, variationMs);
#elif MACCATALYST
            return new Platforms.MacCatalyst.MacKeyboardSimulator(baseDelayMs, variationMs);
#else
            throw new PlatformNotSupportedException("Current platform is not supported");
#endif
        }

        /// <summary>
        /// Create platform-specific keyboard hook
        /// </summary>
        public static IKeyboardHook CreateKeyboardHook()
        {
#if WINDOWS
            return new Platforms.Windows.WindowsKeyboardHook();
#elif MACCATALYST
            return new Platforms.MacCatalyst.MacKeyboardHook();
#else
            throw new PlatformNotSupportedException("Current platform is not supported");
#endif
        }

        /// <summary>
        /// Create platform-specific screenshot service
        /// </summary>
        public static IScreenshotService CreateScreenshotService()
        {
#if WINDOWS
            return new Platforms.Windows.WindowsScreenshotService();
#elif MACCATALYST
            return new Platforms.MacCatalyst.MacScreenshotService();
#else
            throw new PlatformNotSupportedException("Current platform is not supported");
#endif
        }

        /// <summary>
        /// Create platform-specific menu bar/system tray manager
        /// </summary>
        /// <param name="notifyIcon">Windows NotifyIcon (Windows only, can be null on Mac)</param>
        public static IMenuBarManager CreateMenuBarManager(object? notifyIcon = null)
        {
#if WINDOWS
            if (notifyIcon is System.Windows.Forms.NotifyIcon icon)
            {
                return new Platforms.Windows.WindowsMenuBarManager(icon);
            }
            throw new ArgumentException("Windows platform requires a NotifyIcon instance", nameof(notifyIcon));
#elif MACCATALYST
            return new Platforms.MacCatalyst.MacMenuBarManager();
#else
            throw new PlatformNotSupportedException("Current platform is not supported");
#endif
        }

        /// <summary>
        /// Get the current platform name
        /// </summary>
        public static string GetPlatformName()
        {
#if WINDOWS
            return "Windows";
#elif MACCATALYST
            return "macOS";
#else
            return "Unknown";
#endif
        }

        /// <summary>
        /// Check if running on macOS
        /// </summary>
        public static bool IsMacOS()
        {
#if MACCATALYST
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Check if running on Windows
        /// </summary>
        public static bool IsWindows()
        {
#if WINDOWS
            return true;
#else
            return false;
#endif
        }
    }
}
