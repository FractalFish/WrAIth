using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Wraith
{
    public class HotkeyManager : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int WM_HOTKEY = 0x0312;

        // Modifiers
        public const uint MOD_ALT = 0x0001;
        public const uint MOD_CONTROL = 0x0002;
        public const uint MOD_SHIFT = 0x0004;
        public const uint MOD_WIN = 0x0008;

        private readonly Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();
        private readonly Form _messageWindow;
        private int _nextHotkeyId = 1;

        public HotkeyManager(Form messageWindow)
        {
            _messageWindow = messageWindow;
            Log("HotkeyManager initialized");
        }

        public int RegisterHotkey(string hotkeyString, Action action)
        {
            if (string.IsNullOrWhiteSpace(hotkeyString))
            {
                Log($"Cannot register empty hotkey string");
                return -1;
            }

            ParseHotkey(hotkeyString, out uint modifiers, out uint keyCode);
            
            Log($"Attempting to register hotkey: '{hotkeyString}' -> Modifiers=0x{modifiers:X}, KeyCode=0x{keyCode:X} ({keyCode})");

            int hotkeyId = _nextHotkeyId++;
            if (RegisterHotKey(_messageWindow.Handle, hotkeyId, modifiers, keyCode))
            {
                _hotkeyActions[hotkeyId] = action;
                Log($"Successfully registered hotkey ID {hotkeyId} for '{hotkeyString}'");
                return hotkeyId;
            }

            int error = Marshal.GetLastWin32Error();
            Log($"FAILED to register hotkey '{hotkeyString}'. Win32 Error: {error}");
            return -1;
        }

        public void UnregisterHotkey(int hotkeyId)
        {
            if (_hotkeyActions.ContainsKey(hotkeyId))
            {
                UnregisterHotKey(_messageWindow.Handle, hotkeyId);
                _hotkeyActions.Remove(hotkeyId);
                Log($"Unregistered hotkey ID {hotkeyId}");
            }
        }

        public void ProcessHotkey(int hotkeyId)
        {
            if (_hotkeyActions.TryGetValue(hotkeyId, out Action? action))
            {
                Log($"Processing hotkey ID {hotkeyId}");
                action?.Invoke();
            }
            else
            {
                Log($"WARNING: Received hotkey ID {hotkeyId} but no action registered");
            }
        }

        private void ParseHotkey(string hotkeyString, out uint modifiers, out uint keyCode)
        {
            modifiers = 0;
            keyCode = 0;

            var parts = hotkeyString.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            Log($"Parsing hotkey string: '{hotkeyString}' into {parts.Length} parts");
            
            foreach (var part in parts)
            {
                string upperPart = part.ToUpperInvariant();
                
                if (upperPart == "CONTROL" || upperPart == "CTRL")
                {
                    modifiers |= MOD_CONTROL;
                    Log($"  - Added CONTROL modifier");
                }
                else if (upperPart == "SHIFT")
                {
                    modifiers |= MOD_SHIFT;
                    Log($"  - Added SHIFT modifier");
                }
                else if (upperPart == "ALT")
                {
                    modifiers |= MOD_ALT;
                    Log($"  - Added ALT modifier");
                }
                else if (upperPart == "WIN" || upperPart == "WINDOWS")
                {
                    modifiers |= MOD_WIN;
                    Log($"  - Added WIN modifier");
                }
                else
                {
                    // Try to parse as a key
                    if (Enum.TryParse<Keys>(part, true, out Keys key))
                    {
                        keyCode = (uint)key;
                        Log($"  - Set key code: {key} (0x{keyCode:X})");
                    }
                    else
                    {
                        Log($"  - WARNING: Could not parse key '{part}'");
                    }
                }
            }
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [HotkeyManager] {message}");
            Console.WriteLine($"[{timestamp}] [HotkeyManager] {message}");
        }

        public void Dispose()
        {
            Log("Disposing HotkeyManager...");
            foreach (var hotkeyId in _hotkeyActions.Keys.ToList())
            {
                UnregisterHotkey(hotkeyId);
            }
            Log("HotkeyManager disposed");
        }
    }
}
