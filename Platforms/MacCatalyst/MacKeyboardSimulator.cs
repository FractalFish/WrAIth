using System.Runtime.InteropServices;
using BLLMT.Interfaces;
using CoreGraphics;

namespace BLLMT.Platforms.MacCatalyst
{
    /// <summary>
    /// macOS implementation of IKeyboardSimulator using CGEventPost
    /// </summary>
    public class MacKeyboardSimulator : IKeyboardSimulator
    {
        private readonly Random _random = new Random();
        private readonly int _baseDelay;
        private readonly int _variation;

        public MacKeyboardSimulator(int baseDelayMs = 50, int variationMs = 20)
        {
            _baseDelay = baseDelayMs;
            _variation = variationMs;
        }

        public void TypeCharacter(char c)
        {
            // Convert character to Unicode string for CGEvent
            string charString = c.ToString();
            
            // Create keyboard event with the character
            using (var keyDownEvent = CGEvent.CreateKeyboardEvent(null, 0, true))
            {
                keyDownEvent.SetUnicodeString(charString);
                keyDownEvent.SetIntegerValueField(CGEventField.EventSourceUserData, 1); // Mark as synthetic
                keyDownEvent.Post(CGEventTapLocation.Session);
            }

            Thread.Sleep(10); // Brief pause

            using (var keyUpEvent = CGEvent.CreateKeyboardEvent(null, 0, false))
            {
                keyUpEvent.SetUnicodeString(charString);
                keyUpEvent.SetIntegerValueField(CGEventField.EventSourceUserData, 1); // Mark as synthetic
                keyUpEvent.Post(CGEventTapLocation.Session);
            }
        }

        public void SimulateBackspace()
        {
            const int kVK_Delete = 0x33; // macOS key code for delete/backspace

            using (var keyDownEvent = CGEvent.CreateKeyboardEvent(null, (ushort)kVK_Delete, true))
            {
                keyDownEvent.SetIntegerValueField(CGEventField.EventSourceUserData, 1);
                keyDownEvent.Post(CGEventTapLocation.Session);
            }

            Thread.Sleep(10);

            using (var keyUpEvent = CGEvent.CreateKeyboardEvent(null, (ushort)kVK_Delete, false))
            {
                keyUpEvent.SetIntegerValueField(CGEventField.EventSourceUserData, 1);
                keyUpEvent.Post(CGEventTapLocation.Session);
            }
        }

        public int GetRandomDelay()
        {
            int delay = _baseDelay + _random.Next(-_variation, _variation);
            if (delay < 10) delay = 10;
            return delay;
        }
    }
}
