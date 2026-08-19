using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Wraith
{
    public class KeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        // Flag to detect injected keystrokes
        private const uint LLKHF_INJECTED = 0x00000010;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public uint vkCode;
            public uint scanCode;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        
        public event EventHandler<KeyEventArgs>? KeyPressed;
        public bool IsHookEnabled { get; private set; }

        public KeyboardHook()
        {
            _proc = HookCallback;
        }

        public void Start()
        {
            if (_hookID == IntPtr.Zero)
            {
                using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
                using (var curModule = curProcess.MainModule)
                {
                    if (curModule != null)
                    {
                        _hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, 
                            GetModuleHandle(curModule.ModuleName), 0);
                        IsHookEnabled = true;
                        Log("Keyboard hook started successfully");
                    }
                }
            }
        }

        public void Stop()
        {
            if (_hookID != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_hookID);
                _hookID = IntPtr.Zero;
                IsHookEnabled = false;
                Log("Keyboard hook stopped");
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                KBDLLHOOKSTRUCT hookStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                
                // CRITICAL: Ignore injected/simulated keystrokes (from our own TypeCharacter method)
                if ((hookStruct.flags & LLKHF_INJECTED) != 0)
                {
                    // This is a simulated keystroke from our app, let it through
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }
                
                Keys key = (Keys)hookStruct.vkCode;
                
                // Ignore modifier-only keys
                if (key == Keys.LControlKey || key == Keys.RControlKey ||
                    key == Keys.LShiftKey || key == Keys.RShiftKey ||
                    key == Keys.LMenu || key == Keys.RMenu ||
                    key == Keys.LWin || key == Keys.RWin ||
                    key == Keys.Control || key == Keys.Shift || key == Keys.Alt)
                {
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }
                
                var args = new KeyEventArgs(key);
                KeyPressed?.Invoke(this, args);
                
                if (args.SuppressKeyPress)
                {
                    // Block the keystroke from reaching the application
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [KeyboardHook] {message}");
            Console.WriteLine($"[{timestamp}] [KeyboardHook] {message}");
        }

        public void Dispose()
        {
            Stop();
        }

        public class KeyEventArgs : EventArgs
        {
            public Keys Key { get; }
            public bool SuppressKeyPress { get; set; }

            public KeyEventArgs(Keys key)
            {
                Key = key;
                SuppressKeyPress = false;
            }
        }
    }
}
