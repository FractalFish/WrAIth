using BLLMT.Interfaces;
using System.Windows.Forms;

namespace BLLMT.Platforms.Windows
{
    /// <summary>
    /// Windows implementation wrapper for existing HotkeyManager
    /// Wraps the existing Windows-specific code to implement the interface
    /// </summary>
    public class WindowsHotkeyManager : IHotkeyManager
    {
        private readonly HotkeyManager _hotkeyManager;

        public WindowsHotkeyManager(Form messageWindow)
        {
            _hotkeyManager = new HotkeyManager(messageWindow);
        }

        public int RegisterHotkey(string hotkeyString, Action action)
        {
            return _hotkeyManager.RegisterHotkey(hotkeyString, action);
        }

        public void UnregisterHotkey(int hotkeyId)
        {
            _hotkeyManager.UnregisterHotkey(hotkeyId);
        }

        public void ProcessHotkey(int hotkeyId)
        {
            _hotkeyManager.ProcessHotkey(hotkeyId);
        }

        public void Dispose()
        {
            _hotkeyManager.Dispose();
        }
    }
}
