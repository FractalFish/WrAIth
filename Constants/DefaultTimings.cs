namespace BLLMT.Constants
{
    /// <summary>
    /// Default timing configurations for application behavior
    /// </summary>
    public static class DefaultTimings
    {
        #region Keyboard Emulation
        /// <summary>
        /// Base delay between keystrokes in milliseconds
        /// </summary>
        public const int TypingDelayMs = 50;

        /// <summary>
        /// Random variation added to typing delay for natural feel
        /// </summary>
        public const int TypingVariationMs = 20;

        /// <summary>
        /// Minimum allowed typing delay
        /// </summary>
        public const int TypingDelayMin = 10;

        /// <summary>
        /// Maximum allowed typing delay
        /// </summary>
        public const int TypingDelayMax = 500;

        /// <summary>
        /// Minimum allowed typing variation
        /// </summary>
        public const int TypingVariationMin = 0;

        /// <summary>
        /// Maximum allowed typing variation
        /// </summary>
        public const int TypingVariationMax = 200;
        #endregion

        #region API Requests
        /// <summary>
        /// HTTP client timeout for API requests in minutes
        /// </summary>
        public const int ApiTimeoutMinutes = 2;

        /// <summary>
        /// Retry delay for failed API requests in milliseconds
        /// </summary>
        public const int ApiRetryDelayMs = 1000;

        /// <summary>
        /// Maximum retry attempts for failed API requests
        /// </summary>
        public const int ApiMaxRetries = 3;
        #endregion

        #region UI Notifications
        /// <summary>
        /// Duration for balloon tip notifications in milliseconds
        /// </summary>
        public const int BalloonTipDuration = 2000;

        /// <summary>
        /// Duration for longer balloon tips in milliseconds
        /// </summary>
        public const int BalloonTipDurationLong = 3000;

        /// <summary>
        /// Delay before auto-closing settings form after save
        /// </summary>
        public const int SettingsAutoCloseDelayMs = 2000;
        #endregion

        #region Screenshot
        /// <summary>
        /// Delay after screenshot end before starting analysis
        /// </summary>
        public const int ScreenshotProcessDelayMs = 100;
        #endregion
    }
}
