using BLLMT.Interfaces;
using AppKit;
using Foundation;

namespace BLLMT.Platforms.MacCatalyst
{
    /// <summary>
    /// macOS implementation of IMenuBarManager using NSStatusBar
    /// </summary>
    public class MacMenuBarManager : IMenuBarManager
    {
        private NSStatusItem? _statusItem;
        private NSMenu? _menu;
        private NSMenuItem? _statusMenuItem;
        private Action? _settingsCallback;
        private Action? _exitCallback;

        public void Initialize()
        {
            // Create status bar item
            _statusItem = NSStatusBar.SystemStatusBar.CreateStatusItem(NSStatusItemLength.Variable);
            
            if (_statusItem != null)
            {
                // Set icon (you'll need to provide an icon asset)
                _statusItem.Button.Image = NSImage.ImageNamed("StatusIcon");
                _statusItem.Button.Image.Size = new CoreGraphics.CGSize(18, 18);
                _statusItem.Button.Image.Template = true; // Makes it adapt to dark/light mode
                
                // If no icon, use text
                if (_statusItem.Button.Image == null)
                {
                    _statusItem.Button.Title = "BLLMT";
                }

                // Create menu
                _menu = new NSMenu();
                
                // Status item (non-clickable)
                _statusMenuItem = new NSMenuItem("Ready", null, "");
                _statusMenuItem.Enabled = false;
                _menu.AddItem(_statusMenuItem);
                _menu.AddItem(NSMenuItem.SeparatorItem);

                // Settings menu item
                var settingsItem = new NSMenuItem("Settings...", (sender, e) =>
                {
                    _settingsCallback?.Invoke();
                }, "");
                _menu.AddItem(settingsItem);

                // Exit menu item
                var exitItem = new NSMenuItem("Quit BLLMT", (sender, e) =>
                {
                    _exitCallback?.Invoke();
                }, "q");
                _menu.AddItem(exitItem);

                _statusItem.Menu = _menu;
                
                Log("Menu bar initialized");
            }
        }

        public void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            var notification = new NSUserNotification
            {
                Title = title,
                InformativeText = message,
                DeliveryDate = NSDate.Now
            };

            // Set sound based on type
            if (type == NotificationType.Error)
            {
                notification.SoundName = NSUserNotification.NSUserNotificationDefaultSoundName;
            }

            NSUserNotificationCenter.DefaultUserNotificationCenter.DeliverNotification(notification);
            Log($"Notification shown: {title} - {message}");
        }

        public void UpdateStatus(string status)
        {
            if (_statusMenuItem != null)
            {
                NSApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    _statusMenuItem.Title = status;
                });
            }
        }

        public void SetSettingsCallback(Action callback)
        {
            _settingsCallback = callback;
        }

        public void SetExitCallback(Action callback)
        {
            _exitCallback = callback;
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [MacMenuBarManager] {message}");
            Console.WriteLine($"[{timestamp}] [MacMenuBarManager] {message}");
        }

        public void Dispose()
        {
            if (_statusItem != null)
            {
                NSStatusBar.SystemStatusBar.RemoveStatusItem(_statusItem);
                _statusItem = null;
            }

            Log("Menu bar disposed");
        }
    }
}
