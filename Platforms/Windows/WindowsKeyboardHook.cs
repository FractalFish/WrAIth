using BLLMT.Interfaces;
using System.Windows.Forms;

namespace BLLMT.Platforms.Windows
{
    /// <summary>
    /// Windows implementation wrapper for existing KeyboardHook
    /// Adapts Windows Forms Keys to platform-agnostic KeyPressedEventArgs
    /// </summary>
    public class WindowsKeyboardHook : IKeyboardHook
    {
        private readonly KeyboardHook _keyboardHook;

        public event EventHandler<KeyPressedEventArgs>? KeyPressed;
        
        public bool IsHookEnabled => _keyboardHook.IsHookEnabled;

        public WindowsKeyboardHook()
        {
            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyPressed += OnKeyPressed;
        }

        private void OnKeyPressed(object? sender, KeyboardHook.KeyEventArgs e)
        {
            // Convert Windows Forms Keys to int keycode
            var args = new KeyPressedEventArgs((int)e.Key);
            KeyPressed?.Invoke(this, args);
            
            // Propagate the suppress flag back
            e.SuppressKeyPress = args.SuppressKeyPress;
        }

        public void Start()
        {
            _keyboardHook.Start();
        }

        public void Stop()
        {
            _keyboardHook.Stop();
        }

        public void Dispose()
        {
            _keyboardHook.Dispose();
        }
    }
}
