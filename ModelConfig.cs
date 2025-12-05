using System.Text.Json.Serialization;

namespace BLLMT
{
    /// <summary>
    /// Configuration for a single AI model (text or vision)
    /// </summary>
    public class ModelConfig
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "Unnamed Model";

        [JsonPropertyName("provider")]
        public string Provider { get; set; } = "OpenAI"; // OpenAI, Anthropic, Custom

        [JsonPropertyName("apiKey")]
        public string ApiKey { get; set; } = string.Empty;

        [JsonPropertyName("model")]
        public string Model { get; set; } = "gpt-4o-mini";

        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; } = "https://api.openai.com/v1/chat/completions";

        [JsonPropertyName("supportsVision")]
        public bool SupportsVision { get; set; } = false;

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; } = false;

        // Per-model system prompt
        [JsonPropertyName("systemPrompt")]
        public string SystemPrompt { get; set; } = "You are a helpful assistant. Provide concise and direct responses.";

        // Per-model hotkeys
        [JsonPropertyName("triggerHotkey")]
        public string TriggerHotkey { get; set; } = "Control+Shift+Q";

        [JsonPropertyName("screenshotStartHotkey")]
        public string ScreenshotStartHotkey { get; set; } = string.Empty; // Empty = disabled

        [JsonPropertyName("screenshotEndHotkey")]
        public string ScreenshotEndHotkey { get; set; } = string.Empty; // Empty = disabled

        [JsonPropertyName("analyzeScreenshotHotkey")]
        public string AnalyzeScreenshotHotkey { get; set; } = string.Empty; // New: separate action to analyze

        [JsonPropertyName("appendVisionHotkey")]
        public string AppendVisionHotkey { get; set; } = string.Empty; // Empty = disabled

        [JsonPropertyName("outputHotkey")]
        public string OutputHotkey { get; set; } = "Control+Shift+W";

        [JsonPropertyName("abortHotkey")]
        public string AbortHotkey { get; set; } = "Control+Shift+E";

        // Model chaining configuration
        [JsonPropertyName("chainToModelId")]
        public string ChainToModelId { get; set; } = string.Empty; // ID of model to chain output to

        [JsonPropertyName("chainMode")]
        public string ChainMode { get; set; } = "none"; // none, append, prepend, replace

        // Per-model options
        [JsonPropertyName("autoDetectClipboardImages")]
        public bool AutoDetectClipboardImages { get; set; } = false; // Default to false

        [JsonPropertyName("combineScreenshotWithClipboard")]
        public bool CombineScreenshotWithClipboard { get; set; } = true;
    }
}
