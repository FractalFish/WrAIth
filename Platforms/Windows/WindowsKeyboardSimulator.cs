using Wraith.Interfaces;

namespace Wraith.Platforms.Windows
{
    /// <summary>
    /// Windows implementation wrapper for existing KeyboardSimulator
    /// </summary>
    public class WindowsKeyboardSimulator : IKeyboardSimulator
    {
        private readonly KeyboardSimulator _keyboardSimulator;

        public WindowsKeyboardSimulator(int baseDelayMs = 50, int variationMs = 20)
        {
            _keyboardSimulator = new KeyboardSimulator(baseDelayMs, variationMs);
        }

        public void TypeCharacter(char c)
        {
            _keyboardSimulator.TypeCharacter(c);
        }

        public void SimulateBackspace()
        {
            _keyboardSimulator.SimulateBackspace();
        }

        public int GetRandomDelay()
        {
            return _keyboardSimulator.GetRandomDelay();
        }
    }
}
