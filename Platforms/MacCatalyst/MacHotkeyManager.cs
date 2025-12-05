using System.Runtime.InteropServices;
using BLLMT.Interfaces;
using Foundation;
using CoreGraphics;
using AppKit;

namespace BLLMT.Platforms.MacCatalyst
{
    /// <summary>
    /// macOS implementation of IHotkeyManager using CGEventTap
    /// Requires Accessibility permissions
    /// </summary>
    public class MacHotkeyManager : IHotkeyManager
    {
        private readonly Dictionary<int, Action> _hotkeyActions = new Dictionary<int, Action>();
        private readonly Dictionary<int, HotkeyDefinition> _hotkeyDefinitions = new Dictionary<int, HotkeyDefinition>();
        private CFRunLoopSource? _eventSource;
        private CFMachPort? _eventTap;
        private int _nextHotkeyId = 1;

        private class HotkeyDefinition
        {
            public CGEventFlags Modifiers { get; set; }
            public int KeyCode { get; set; }
        }

        public MacHotkeyManager()
        {
            Log("MacHotkeyManager initialized");
            StartEventTap();
        }

        private void StartEventTap()
        {
            // Request accessibility permissions
            var options = new NSDictionary();
            if (!AXIsProcessTrusted())
            {
                Log("WARNING: Accessibility permissions not granted. Hotkeys will not work.");
                Log("Please grant accessibility permissions in System Preferences > Security & Privacy > Privacy > Accessibility");
            }

            // Create event tap for key down events
            _eventTap = CGEvent.CreateTap(
                CGEventTapLocation.Session,
                CGEventTapPlacement.HeadInsert,
                CGEventTapOptions.Default,
                CGEventMask.KeyDown | CGEventMask.FlagsChanged,
                HandleEvent,
                IntPtr.Zero);

            if (_eventTap != null)
            {
                _eventSource = _eventTap.CreateRunLoopSource();
                CFRunLoop.Current.AddSource(_eventSource, CFRunLoop.ModeCommon);
                _eventTap.Enable();
                Log("Event tap created successfully");
            }
            else
            {
                Log("ERROR: Failed to create event tap. Check accessibility permissions.");
            }
        }

        private IntPtr HandleEvent(IntPtr proxy, CGEventType type, IntPtr eventRef, IntPtr userInfo)
        {
            try
            {
                using (var evt = new CGEvent(eventRef))
                {
                    if (type == CGEventType.KeyDown)
                    {
                        var keyCode = (int)evt.GetIntegerValueField(CGEventField.KeyboardEventKeycode);
                        var flags = evt.Flags;

                        // Check if this matches any registered hotkey
                        foreach (var kvp in _hotkeyDefinitions)
                        {
                            if (kvp.Value.KeyCode == keyCode && HasModifiers(flags, kvp.Value.Modifiers))
                            {
                                // Execute hotkey action on main thread
                                NSApplication.SharedApplication.InvokeOnMainThread(() =>
                                {
                                    if (_hotkeyActions.TryGetValue(kvp.Key, out var action))
                                    {
                                        Log($"Executing hotkey ID {kvp.Key}");
                                        action?.Invoke();
                                    }
                                });

                                // Don't suppress the key event - let it through
                                return eventRef;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR in HandleEvent: {ex.Message}");
            }

            return eventRef;
        }

        private bool HasModifiers(CGEventFlags current, CGEventFlags required)
        {
            // Mask for modifier keys only
            var modifierMask = CGEventFlags.MaskCommand | CGEventFlags.MaskControl |
                             CGEventFlags.MaskShift | CGEventFlags.MaskAlternate;

            var currentMods = current & modifierMask;
            return currentMods == required;
        }

        public int RegisterHotkey(string hotkeyString, Action action)
        {
            if (string.IsNullOrWhiteSpace(hotkeyString))
            {
                Log("Cannot register empty hotkey string");
                return -1;
            }

            ParseHotkey(hotkeyString, out CGEventFlags modifiers, out int keyCode);

            int hotkeyId = _nextHotkeyId++;
            _hotkeyActions[hotkeyId] = action;
            _hotkeyDefinitions[hotkeyId] = new HotkeyDefinition
            {
                Modifiers = modifiers,
                KeyCode = keyCode
            };

            Log($"Registered hotkey ID {hotkeyId}: '{hotkeyString}' -> KeyCode={keyCode}, Modifiers={modifiers}");
            return hotkeyId;
        }

        public void UnregisterHotkey(int hotkeyId)
        {
            if (_hotkeyActions.ContainsKey(hotkeyId))
            {
                _hotkeyActions.Remove(hotkeyId);
                _hotkeyDefinitions.Remove(hotkeyId);
                Log($"Unregistered hotkey ID {hotkeyId}");
            }
        }

        public void ProcessHotkey(int hotkeyId)
        {
            // Not used on macOS - events come through HandleEvent
        }

        private void ParseHotkey(string hotkeyString, out CGEventFlags modifiers, out int keyCode)
        {
            modifiers = 0;
            keyCode = 0;

            var parts = hotkeyString.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                string upperPart = part.ToUpperInvariant();

                // Map Windows modifier names to macOS
                if (upperPart == "CONTROL" || upperPart == "CTRL")
                {
                    modifiers |= CGEventFlags.MaskCommand; // Command is the primary modifier on macOS
                }
                else if (upperPart == "SHIFT")
                {
                    modifiers |= CGEventFlags.MaskShift;
                }
                else if (upperPart == "ALT")
                {
                    modifiers |= CGEventFlags.MaskAlternate; // Option/Alt
                }
                else if (upperPart == "WIN" || upperPart == "WINDOWS" || upperPart == "COMMAND" || upperPart == "CMD")
                {
                    modifiers |= CGEventFlags.MaskCommand;
                }
                else
                {
                    // Try to map key name to macOS key code
                    keyCode = MapKeyNameToKeyCode(upperPart);
                    Log($"Mapped key '{part}' to key code {keyCode}");
                }
            }
        }

        private int MapKeyNameToKeyCode(string keyName)
        {
            // macOS key codes (from Events.h)
            return keyName switch
            {
                "A" => 0x00, "S" => 0x01, "D" => 0x02, "F" => 0x03, "H" => 0x04,
                "G" => 0x05, "Z" => 0x06, "X" => 0x07, "C" => 0x08, "V" => 0x09,
                "B" => 0x0B, "Q" => 0x0C, "W" => 0x0D, "E" => 0x0E, "R" => 0x0F,
                "Y" => 0x10, "T" => 0x11, "1" => 0x12, "2" => 0x13, "3" => 0x14,
                "4" => 0x15, "5" => 0x17, "6" => 0x16, "=" => 0x18, "9" => 0x19,
                "7" => 0x1A, "-" => 0x1B, "8" => 0x1C, "0" => 0x1D, "]" => 0x1E,
                "O" => 0x1F, "U" => 0x20, "[" => 0x21, "I" => 0x22, "P" => 0x23,
                "L" => 0x25, "J" => 0x26, "'" => 0x27, "K" => 0x28, ";" => 0x29,
                "\\" => 0x2A, "," => 0x2B, "/" => 0x2C, "N" => 0x2D, "M" => 0x2E,
                "." => 0x2F, "`" => 0x32,
                "RETURN" => 0x24, "ENTER" => 0x24, "TAB" => 0x30, "SPACE" => 0x31,
                "DELETE" => 0x33, "BACKSPACE" => 0x33, "ESCAPE" => 0x35, "ESC" => 0x35,
                "F1" => 0x7A, "F2" => 0x78, "F3" => 0x63, "F4" => 0x76, "F5" => 0x60,
                "F6" => 0x61, "F7" => 0x62, "F8" => 0x64, "F9" => 0x65, "F10" => 0x6D,
                "F11" => 0x67, "F12" => 0x6F,
                _ => 0
            };
        }

        [DllImport("/System/Library/Frameworks/ApplicationServices.framework/ApplicationServices")]
        private static extern bool AXIsProcessTrusted();

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [MacHotkeyManager] {message}");
            Console.WriteLine($"[{timestamp}] [MacHotkeyManager] {message}");
        }

        public void Dispose()
        {
            Log("Disposing MacHotkeyManager...");

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

            _hotkeyActions.Clear();
            _hotkeyDefinitions.Clear();
            Log("MacHotkeyManager disposed");
        }
    }
}
