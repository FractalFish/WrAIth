namespace Wraith.Constants
{
    /// <summary>
    /// Action identifiers for global hotkey mappings (Phase 2: Global Hotkeys System)
    /// </summary>
    public static class HotkeyActions
    {
        #region Text Processing
        /// <summary>
        /// Process text from clipboard and send to model
        /// </summary>
        public const string ProcessText = "ProcessText";

        /// <summary>
        /// Process image from clipboard and send to vision model
        /// </summary>
        public const string ProcessImage = "ProcessImage";
        #endregion

        #region Screenshot
        /// <summary>
        /// Start screenshot region selection
        /// </summary>
        public const string ScreenshotStart = "ScreenshotStart";

        /// <summary>
        /// End screenshot selection and analyze with vision model
        /// </summary>
        public const string ScreenshotEnd = "ScreenshotEnd";

        /// <summary>
        /// Cancel screenshot in progress
        /// </summary>
        public const string ScreenshotCancel = "ScreenshotCancel";
        #endregion

        #region Vision + Reasoning
        /// <summary>
        /// Combine stored vision result with clipboard text for reasoning
        /// </summary>
        public const string VisionReasoning = "VisionReasoning";

        /// <summary>
        /// Analyze stored screenshot with vision model
        /// </summary>
        public const string AnalyzeScreenshot = "AnalyzeScreenshot";
        #endregion

        #region Output Control
        /// <summary>
        /// Output/type the queued response
        /// </summary>
        public const string Output = "Output";

        /// <summary>
        /// Pause/resume output emulation
        /// </summary>
        public const string PauseResume = "PauseResume";

        /// <summary>
        /// Abort current operation or clear queue
        /// </summary>
        public const string Abort = "Abort";
        #endregion

        #region Special Model IDs
        /// <summary>
        /// Use the model that generated the current response
        /// </summary>
        public const string ModelCurrent = "(Current)";

        /// <summary>
        /// Global action (no specific model)
        /// </summary>
        public const string ModelGlobal = "(Global)";

        /// <summary>
        /// No model selected / disabled
        /// </summary>
        public const string ModelNone = "(None)";
        #endregion

        /// <summary>
        /// Get all available action types
        /// </summary>
        public static readonly string[] All = 
        {
            ProcessText,
            ProcessImage,
            ScreenshotStart,
            ScreenshotEnd,
            ScreenshotCancel,
            VisionReasoning,
            AnalyzeScreenshot,
            Output,
            PauseResume,
            Abort
        };

        /// <summary>
        /// Get user-friendly display name for an action
        /// </summary>
        public static string GetDisplayName(string action) => action switch
        {
            ProcessText => "Send Query (Text)",
            ProcessImage => "Send Query (Image)",
            ScreenshotStart => "Screenshot - Start Selection",
            ScreenshotEnd => "Screenshot - End & Analyze",
            ScreenshotCancel => "Screenshot - Cancel",
            VisionReasoning => "Vision + Reasoning",
            AnalyzeScreenshot => "Analyze Screenshot",
            Output => "Output Response",
            PauseResume => "Pause/Resume Output",
            Abort => "Abort Operation",
            _ => action
        };
    }
}
