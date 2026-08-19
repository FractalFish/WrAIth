using Wraith.Interfaces;
using System.Windows.Forms;

namespace Wraith.Platforms.Windows
{
    /// <summary>
    /// Windows implementation wrapper for NotifyIcon (system tray)
    /// Implements IMenuBarManager using Windows Forms NotifyIcon
    /// </summary>
    public class WindowsMenuBarManager : IMenuBarManager
    {
        private readonly NotifyIcon _notifyIcon;
        private ToolStripMenuItem? _statusMenuItem;
        private Action? _settingsCallback;
        private Action? _exitCallback;

        public WindowsMenuBarManager(NotifyIcon notifyIcon)
        {
            _notifyIcon = notifyIcon;
        }

        public void Initialize()
        {
            // Context menu should already be set up in Form1.Designer.cs
            // This just ensures callbacks are wired up
            if (_notifyIcon.ContextMenuStrip != null)
            {
                // Find status menu item
                foreach (ToolStripItem item in _notifyIcon.ContextMenuStrip.Items)
                {
                    if (item is ToolStripMenuItem menuItem)
                    {
                        if (!string.IsNullOrEmpty(menuItem.Text) && 
                            (menuItem.Text.StartsWith("Status:") || !menuItem.Enabled))
                        {
                            _statusMenuItem = menuItem;
                        }
                        else if (!string.IsNullOrEmpty(menuItem.Text) && menuItem.Text.Contains("Settings"))
                        {
                            menuItem.Click += (s, e) => _settingsCallback?.Invoke();
                        }
                        else if (!string.IsNullOrEmpty(menuItem.Text) && 
                                (menuItem.Text.Contains("Exit") || menuItem.Text.Contains("Quit")))
                        {
                            menuItem.Click += (s, e) => _exitCallback?.Invoke();
                        }
                    }
                }
            }
        }

        public void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            ToolTipIcon icon = type switch
            {
                NotificationType.Warning => ToolTipIcon.Warning,
                NotificationType.Error => ToolTipIcon.Error,
                _ => ToolTipIcon.Info
            };

            _notifyIcon.ShowBalloonTip(3000, title, message, icon);
        }

        public void UpdateStatus(string status)
        {
            if (_statusMenuItem != null)
            {
                if (_notifyIcon.ContextMenuStrip?.InvokeRequired == true)
                {
                    _notifyIcon.ContextMenuStrip.Invoke(() => _statusMenuItem.Text = status);
                }
                else
                {
                    _statusMenuItem.Text = status;
                }
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

        public void Dispose()
        {
            // NotifyIcon is disposed by Form1
        }
    }
}
