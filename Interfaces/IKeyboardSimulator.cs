namespace BLLMT.Interfaces
{
    /// <summary>
    /// Platform-specific keyboard simulator interface
    /// </summary>
    public interface IKeyboardSimulator
    {
        /// <summary>
        /// Type a single character
        /// </summary>
        /// <param name="c">Character to type</param>
        void TypeCharacter(char c);
        
        /// <summary>
        /// Simulate a backspace key press
        /// </summary>
        void SimulateBackspace();
        
        /// <summary>
        /// Get a random typing delay based on configured settings
        /// </summary>
        /// <returns>Delay in milliseconds</returns>
        int GetRandomDelay();
    }
}
