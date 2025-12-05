namespace BLLMT.Interfaces
{
    /// <summary>
    /// Platform-specific keyboard hook interface for intercepting keystrokes
    /// </summary>
    public interface IKeyboardHook : IDisposable
    {
        /// <summary>
        /// Event fired when a key is pressed
        /// </summary>
        event EventHandler<KeyPressedEventArgs>? KeyPressed;
        
        /// <summary>
        /// Start the keyboard hook
        /// </summary>
        void Start();
        
        /// <summary>
        /// Stop the keyboard hook
        /// </summary>
        void Stop();
        
        /// <summary>
        /// Indicates if the hook is currently active
        /// </summary>
        bool IsHookEnabled { get; }
    }
    
    /// <summary>
    /// Event args for keyboard hook events
    /// </summary>
    public class KeyPressedEventArgs : EventArgs
    {
        public int KeyCode { get; }
        public bool SuppressKeyPress { get; set; }

        public KeyPressedEventArgs(int keyCode)
        {
            KeyCode = keyCode;
            SuppressKeyPress = false;
        }
    }
}
