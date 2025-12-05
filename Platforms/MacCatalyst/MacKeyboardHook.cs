using System.Runtime.InteropServices;
using BLLMT.Interfaces;
using CoreGraphics;
using Foundation;

namespace BLLMT.Platforms.MacCatalyst
{
    /// <summary>
    /// macOS implementation of IKeyboardHook using CGEventTap
    /// Requires Accessibility permissions
    /// </summary>
    public class MacKeyboardHook : IKeyboardHook
    {
        private CFRunLoopSource? _eventSource;
        private CFMachPort? _eventTap;
        private bool _isHookEnabled = false;

        public event EventHandler<KeyPressedEventArgs>? KeyPressed;
        public bool IsHookEnabled => _isHookEnabled;

        public void Start()
        {
            if (_eventTap != null)
            {
                Log("Keyboard hook already started");
                return;
            }

            // Create event tap for key down events
            _eventTap = CGEvent.CreateTap(
                CGEventTapLocation.Session,
                CGEventTapPlacement.HeadInsert,
                CGEventTapOptions.Default,
                CGEventMask.KeyDown,
                HandleKeyEvent,
                IntPtr.Zero);

            if (_eventTap != null)
            {
                _eventSource = _eventTap.CreateRunLoopSource();
                CFRunLoop.Current.AddSource(_eventSource, CFRunLoop.ModeCommon);
                _eventTap.Enable();
                _isHookEnabled = true;
                Log("Keyboard hook started successfully");
            }
            else
            {
                Log("ERROR: Failed to create event tap. Check accessibility permissions.");
            }
        }

        public void Stop()
        {
            if (_eventTap != null)
            {
                _eventTap.Disable();
                _eventTap.Dispose();
                _eventTap = null;
            }

            if (_eventSource != null)
            {
                _eventSource.Dispose();
                _eventSource = null;
            }

            _isHookEnabled = false;
            Log("Keyboard hook stopped");
        }

        private IntPtr HandleKeyEvent(IntPtr proxy, CGEventType type, IntPtr eventRef, IntPtr userInfo)
        {
            try
            {
                using (var evt = new CGEvent(eventRef))
                {
                    // Check if this is a synthetic event (from our own KeyboardSimulator)
                    var userData = evt.GetIntegerValueField(CGEventField.EventSourceUserData);
                    if (userData == 1)
                    {
                        // This is from our simulator, let it through
                        return eventRef;
                    }

                    if (type == CGEventType.KeyDown)
                    {
                        var keyCode = (int)evt.GetIntegerValueField(CGEventField.KeyboardEventKeycode);
                        
                        // Ignore modifier-only keys
                        if (IsModifierKey(keyCode))
                        {
                            return eventRef;
                        }

                        var args = new KeyPressedEventArgs(keyCode);
                        KeyPressed?.Invoke(this, args);

                        if (args.SuppressKeyPress)
                        {
                            // Return null to suppress the event
                            return IntPtr.Zero;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR in HandleKeyEvent: {ex.Message}");
            }

            return eventRef;
        }

        private bool IsModifierKey(int keyCode)
        {
            // macOS modifier key codes
            return keyCode == 0x37 || // Command
                   keyCode == 0x38 || // Shift
                   keyCode == 0x3A || // Option/Alt
                   keyCode == 0x3B || // Control
                   keyCode == 0x3C || // Right Shift
                   keyCode == 0x3D || // Right Option
                   keyCode == 0x3E || // Right Control
                   keyCode == 0x3F;   // Function
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [MacKeyboardHook] {message}");
            Console.WriteLine($"[{timestamp}] [MacKeyboardHook] {message}");
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
