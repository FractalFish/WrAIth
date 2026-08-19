namespace Wraith.Interfaces
{
    /// <summary>
    /// Platform-specific hotkey manager interface
    /// </summary>
    public interface IHotkeyManager : IDisposable
    {
        /// <summary>
        /// Register a global hotkey with a callback action
        /// </summary>
        /// <param name="hotkeyString">Hotkey string (e.g., "Control+Shift+Q")</param>
        /// <param name="action">Action to invoke when hotkey is pressed</param>
        /// <returns>Hotkey ID or -1 if registration failed</returns>
        int RegisterHotkey(string hotkeyString, Action action);
        
        /// <summary>
        /// Unregister a specific hotkey
        /// </summary>
        /// <param name="hotkeyId">ID of the hotkey to unregister</param>
        void UnregisterHotkey(int hotkeyId);
        
        /// <summary>
        /// Process a hotkey event (for Windows WndProc)
        /// </summary>
        /// <param name="hotkeyId">ID of the triggered hotkey</param>
        void ProcessHotkey(int hotkeyId);
    }
}
