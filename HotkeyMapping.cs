using System.Text.Json.Serialization;
using Wraith.Constants;

namespace Wraith
{
    /// <summary>
    /// Represents a global hotkey mapping that links a keyboard combination to a model and action.
    /// This is the core data model for Phase 2: Global Hotkeys System.
    /// </summary>
    public class HotkeyMapping
    {
        /// <summary>
        /// Unique identifier for this mapping
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Hotkey combination (e.g., "Control+Shift+Q")
        /// </summary>
        [JsonPropertyName("hotkey")]
        public string Hotkey { get; set; } = string.Empty;

        /// <summary>
        /// Model ID to use for this action, or special values:
        /// - "(Current)" = Use the model that generated the current response
        /// - "(Global)" = Global action, no specific model
        /// - "(None)" = No model/disabled
        /// - Specific Model ID = Use that model
        /// </summary>
        [JsonPropertyName("modelId")]
        public string ModelId { get; set; } = HotkeyActions.ModelGlobal;

        /// <summary>
        /// Action to perform when hotkey is pressed.
        /// See HotkeyActions constants for valid values.
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; } = HotkeyActions.ProcessText;

        /// <summary>
        /// User-friendly description/notes for this mapping
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Whether this mapping is currently enabled
        /// </summary>
        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Display order in the UI (lower numbers appear first)
        /// </summary>
        [JsonPropertyName("displayOrder")]
        public int DisplayOrder { get; set; } = 0;

        /// <summary>
        /// Get a user-friendly display string for this mapping
        /// </summary>
        public string GetDisplayString(AppSettings settings)
        {
            string modelName = ModelId switch
            {
                HotkeyActions.ModelCurrent => "(Current Response)",
                HotkeyActions.ModelGlobal => "(Global)",
                HotkeyActions.ModelNone => "(None)",
                _ => settings.GetModelById(ModelId)?.Name ?? "[Unknown Model]"
            };

            string actionName = HotkeyActions.GetDisplayName(Action);
            
            return $"{Hotkey} ? {modelName} ? {actionName}";
        }

        /// <summary>
        /// Validate this mapping
        /// </summary>
        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Hotkey))
                return false;

            if (string.IsNullOrWhiteSpace(Action))
                return false;

            // Global actions don't need a model
            if (Action == HotkeyActions.Output || 
                Action == HotkeyActions.Abort || 
                Action == HotkeyActions.PauseResume)
            {
                return true;
            }

            // Other actions need a model
            if (string.IsNullOrWhiteSpace(ModelId) || ModelId == HotkeyActions.ModelNone)
                return false;

            return true;
        }

        /// <summary>
        /// Check if this is a global action (doesn't require a specific model)
        /// </summary>
        public bool IsGlobalAction()
        {
            return Action == HotkeyActions.Output ||
                   Action == HotkeyActions.Abort ||
                   Action == HotkeyActions.PauseResume;
        }
    }
}
