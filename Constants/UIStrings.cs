namespace BLLMT.Constants
{
    /// <summary>
    /// User-facing strings for UI messages and notifications
    /// </summary>
    public static class UIStrings
    {
        #region Application
        public const string AppName = "BLLMT";
        public const string AppFullName = "Background LLM Multi-Tool";
        public const string TrayIconText = "BLLMT - Background LLM Assistant";
        #endregion

        #region Status Messages
        public const string StatusReady = "Ready";
        public const string StatusProcessing = "Processing clipboard content...";
        public const string StatusSendingToLLM = "Sending to LLM...";
        public const string StatusTestingModel = "Testing model connection...";
        public const string StatusAnalyzingScreenshot = "Analyzing screenshot with vision model...";
        public const string StatusCombiningVision = "Combining vision result with your query...";
        #endregion

        #region Success Messages
        public const string SettingsSaved = "Settings saved! Restart application for changes to take effect.";
        public const string TestSuccess = "Test success! Response: {0}...";
        public const string VisionTestSuccess = "Vision test success! Response: {0}...";
        public const string ResponseReady = "Response ready ({0} chars). Press output hotkey to start emulation.";
        public const string ResponseCompleted = "Response completed!";
        public const string ModelSetAsDefault = "'{0}' set as default model.";
        public const string CustomFormatSaved = "Custom API format saved. Don't forget to Save settings!";
        #endregion

        #region Error Messages
        public const string ErrorClipboardEmpty = "Clipboard is empty";
        public const string ErrorNoResponseQueued = "No response queued";
        public const string ErrorAlreadyProcessing = "Already processing a request...";
        public const string ErrorNoVisionResult = "No vision result to append. Capture screenshot or copy image first.";
        public const string ErrorCannotRemoveLastModel = "Cannot remove the last model.";
        public const string ErrorLoadingModel = "Error loading model: {0}";
        public const string ErrorSavingModel = "Error saving model: {0}";
        public const string ErrorSavingSettings = "Error saving: {0}";
        public const string ErrorTestFailed = "Test failed: {0}";
        #endregion

        #region Notification Messages
        public const string NotificationProcessing = "Processing clipboard content...";
        public const string NotificationResponseReady = "Response ready! Press output hotkey to start emulation.";
        public const string NotificationEmulationStarted = "Emulation started! Type to output response ({0} chars)";
        public const string NotificationEmulationPaused = "Emulation paused ({0} chars)";
        public const string NotificationEmulationResumed = "Emulation resumed ({0} chars)";
        public const string NotificationEmulationAborted = "Emulation aborted";
        public const string NotificationQueueCleared = "Response queue cleared";
        public const string NotificationScreenshotStarted = "Screenshot started. Move mouse and press end hotkey.";
        public const string NotificationScreenshotCancelled = "Screenshot cancelled";
        public const string NotificationVisionAnalysisComplete = "Vision analysis done! Press append hotkey or output to type.";
        public const string NotificationReasoningComplete = "Reasoning complete! Response ready.";
        #endregion

        #region Prompts and Dialogs
        public const string ConfirmDeleteModel = "Are you sure you want to remove this model?";
        public const string ConfirmDeleteTitle = "Confirm Delete";
        public const string ErrorTitle = "Error";
        public const string WarningTitle = "Warning";
        #endregion

        #region Instructions
        public const string InstructionCopyTextFirst = "Copy text and press trigger hotkey first.";
        public const string InstructionCopyQuestionFirst = "Clipboard is empty. Copy your question first.";
        public const string InstructionHotkeyDisabled = "Empty = disabled";
        public const string InstructionLeaveEmptyToDisable = "Hotkeys (leave empty to disable)";
        #endregion

        #region Model States
        public const string ModelStateDefault = "[DEFAULT]";
        public const string ModelStateDisabled = "[DISABLED]";
        public const string ModelStateEnabled = "Enabled (Model is active)";
        #endregion

        #region Vision
        public const string VisionPromptDefault = "Please describe what you see in this image in detail. Focus on all text, UI elements, diagrams, code, or any relevant information.";
        public const string VisionPromptCombined = "Context from image analysis:\n{0}\n\nUser question:\n{1}";
        #endregion

        #region Testing
        public const string TestQueryText = "Say 'Hello!' if you can read this.";
        public const string TestQueryVision = "What color is this image?";
        #endregion
    }
}
