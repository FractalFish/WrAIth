using System.Runtime.InteropServices;

namespace BLLMT
{
    public class KeyboardSimulator
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern short VkKeyScan(char ch);

        private const int KEYEVENTF_KEYDOWN = 0x0000;
        private const int KEYEVENTF_KEYUP = 0x0002;

        private readonly Random _random = new Random();
        private readonly int _baseDelay;
        private readonly int _variation;

        public KeyboardSimulator(int baseDelayMs = 50, int variationMs = 20)
        {
            _baseDelay = baseDelayMs;
            _variation = variationMs;
        }

        public void TypeCharacter(char c)
        {
            short vkCode = VkKeyScan(c);
            byte virtualKey = (byte)(vkCode & 0xFF);
            byte shiftState = (byte)((vkCode >> 8) & 0xFF);

            // Press Shift if needed
            bool needsShift = (shiftState & 1) != 0;
            if (needsShift)
            {
                keybd_event(0x10, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero); // VK_SHIFT
            }

            // Press and release the key
            keybd_event(virtualKey, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            Thread.Sleep(10); // Brief pause between press and release
            keybd_event(virtualKey, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

            // Release Shift if it was pressed
            if (needsShift)
            {
                keybd_event(0x10, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            }
        }

        public void SimulateBackspace()
        {
            const byte VK_BACK = 0x08;
            keybd_event(VK_BACK, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
            Thread.Sleep(10);
            keybd_event(VK_BACK, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        public int GetRandomDelay()
        {
            int delay = _baseDelay + _random.Next(-_variation, _variation);
            if (delay < 10) delay = 10;
            return delay;
        }
    }
}
