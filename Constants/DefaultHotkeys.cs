namespace Wraith.Constants
{
    /// <summary>
    /// Default hotkey combinations for application actions
    /// </summary>
    public static class DefaultHotkeys
    {
        /// <summary>
        /// Send clipboard content as query to LLM
        /// </summary>
        public const string SendQuery = "Control+Shift+Q";

        /// <summary>
        /// Output/type the queued AI response
        /// </summary>
        public const string Output = "Control+Shift+W";

        /// <summary>
        /// Abort current operation or clear queue
        /// </summary>
        public const string Abort = "Control+Shift+E";

        /// <summary>
        /// Start screenshot region selection
        /// </summary>
        public const string ScreenshotStart = "Control+Shift+S";

        /// <summary>
        /// End screenshot selection and analyze
        /// </summary>
        public const string ScreenshotEnd = "Control+Shift+D";

        /// <summary>
        /// Combine vision analysis with reasoning
        /// </summary>
        public const string VisionReasoning = "Control+Shift+A";

        /// <summary>
        /// Empty/disabled hotkey
        /// </summary>
        public const string None = "";

        /// <summary>
        /// Check if a hotkey string is empty/disabled
        /// </summary>
        public static bool IsEmpty(string hotkey) => string.IsNullOrWhiteSpace(hotkey);
    }
}
