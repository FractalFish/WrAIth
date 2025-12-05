using System.Text.Json;

namespace BLLMT
{
    public class AppSettings
    {
        // Model Configurations (primary system)
        public List<ModelConfig> Models { get; set; } = new List<ModelConfig>();
        
        // Global UI settings only
        public int TypingDelayMs { get; set; } = 50;
        public int TypingVariationMs { get; set; } = 20;
        
        // Legacy settings for backward compatibility (will be migrated)
        public string SystemPrompt { get; set; } = "You are a helpful assistant. Provide concise and direct responses.";
        public string ApiKey { get; set; } = string.Empty;
        public string LlmProvider { get; set; } = "OpenAI";
        public string Model { get; set; } = "gpt-4o-mini";
        public string ApiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
        public string VisionApiKey { get; set; } = string.Empty;
        public string VisionProvider { get; set; } = "OpenAI";
        public string VisionModel { get; set; } = "gpt-4o";
        public string VisionApiEndpoint { get; set; } = "https://api.openai.com/v1/chat/completions";
        public string TriggerHotkey { get; set; } = "Control+Shift+Q";
        public string OutputHotkey { get; set; } = "Control+Shift+W";
        public string AbortHotkey { get; set; } = "Control+Shift+E";
        public string ScreenshotStartHotkey { get; set; } = "Control+Shift+S";
        public string ScreenshotEndHotkey { get; set; } = "Control+Shift+D";
        public string AppendVisionHotkey { get; set; } = "Control+Shift+A";
        public bool AutoDetectClipboardImages { get; set; } = true;
        public bool CombineScreenshotWithClipboard { get; set; } = true;

        public static AppSettings Load()
        {
            string filePath = GetSettingsPath();
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    var settings = JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
                    
                    // Migration: Convert legacy settings to new model system
                    if (settings.Models.Count == 0)
                    {
                        settings.MigrateLegacySettings();
                    }
                    
                    return settings;
                }
                catch
                {
                    return new AppSettings();
                }
            }
            return new AppSettings();
        }

        private void MigrateLegacySettings()
        {
            // Create text model from legacy settings
            if (!string.IsNullOrEmpty(ApiKey))
            {
                Models.Add(new ModelConfig
                {
                    Name = "Text Model (Legacy)",
                    Provider = LlmProvider,
                    ApiKey = ApiKey,
                    Model = Model,
                    Endpoint = ApiEndpoint,
                    SystemPrompt = SystemPrompt,
                    SupportsVision = false,
                    IsDefault = true,
                    TriggerHotkey = TriggerHotkey,
                    OutputHotkey = OutputHotkey,
                    AbortHotkey = AbortHotkey,
                    ScreenshotStartHotkey = string.Empty, // Don't auto-enable
                    ScreenshotEndHotkey = string.Empty,
                    AnalyzeScreenshotHotkey = string.Empty,
                    AppendVisionHotkey = string.Empty,
                    AutoDetectClipboardImages = false, // Default to false
                    CombineScreenshotWithClipboard = CombineScreenshotWithClipboard
                });
            }

            // Create vision model from legacy settings
            if (!string.IsNullOrEmpty(VisionApiKey) || !string.IsNullOrEmpty(VisionModel))
            {
                Models.Add(new ModelConfig
                {
                    Name = "Vision Model (Legacy)",
                    Provider = VisionProvider,
                    ApiKey = string.IsNullOrEmpty(VisionApiKey) ? ApiKey : VisionApiKey,
                    Model = VisionModel,
                    Endpoint = string.IsNullOrEmpty(VisionApiEndpoint) ? ApiEndpoint : VisionApiEndpoint,
                    SystemPrompt = "Analyze this image in detail. Describe what you see.",
                    SupportsVision = true,
                    IsDefault = false,
                    TriggerHotkey = string.Empty, // Different hotkey
                    OutputHotkey = OutputHotkey,
                    AbortHotkey = AbortHotkey,
                    ScreenshotStartHotkey = ScreenshotStartHotkey,
                    ScreenshotEndHotkey = ScreenshotEndHotkey,
                    AnalyzeScreenshotHotkey = AppendVisionHotkey, // Map old append to analyze
                    AppendVisionHotkey = string.Empty,
                    AutoDetectClipboardImages = AutoDetectClipboardImages,
                    CombineScreenshotWithClipboard = CombineScreenshotWithClipboard
                });
            }

            // If no models exist, create defaults with no screenshot hotkeys
            if (Models.Count == 0)
            {
                Models.Add(new ModelConfig
                {
                    Name = "Quick Text",
                    Provider = "OpenAI",
                    Model = "gpt-4o-mini",
                    Endpoint = "https://api.openai.com/v1/chat/completions",
                    SystemPrompt = "You are a helpful assistant. Provide concise and direct responses.",
                    SupportsVision = false,
                    IsDefault = true,
                    TriggerHotkey = "Control+Shift+Q",
                    OutputHotkey = "Control+Shift+W",
                    AbortHotkey = "Control+Shift+E",
                    ScreenshotStartHotkey = string.Empty, // Not enabled by default
                    ScreenshotEndHotkey = string.Empty,
                    AnalyzeScreenshotHotkey = string.Empty,
                    AppendVisionHotkey = string.Empty
                });
            }
        }

        public ModelConfig? GetDefaultTextModel()
        {
            return Models.FirstOrDefault(m => m.IsDefault && !m.SupportsVision) 
                   ?? Models.FirstOrDefault(m => !m.SupportsVision)
                   ?? Models.FirstOrDefault();
        }

        public ModelConfig? GetDefaultVisionModel()
        {
            return Models.FirstOrDefault(m => m.SupportsVision) 
                   ?? Models.FirstOrDefault(m => m.IsDefault);
        }

        public ModelConfig? GetModelById(string id)
        {
            return Models.FirstOrDefault(m => m.Id == id);
        }

        public void Save()
        {
            string filePath = GetSettingsPath();
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filePath, json);
        }

        private static string GetSettingsPath()
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "BLLMT");
            Directory.CreateDirectory(appDataPath);
            return Path.Combine(appDataPath, "settings.json");
        }
    }
}
