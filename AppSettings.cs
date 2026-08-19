using System.Text.Json;
using Wraith.Constants;

namespace Wraith
{
    public class AppSettings
    {
        // Model Configurations (primary system)
        public List<ModelConfig> Models { get; set; } = new List<ModelConfig>();
        
        // Global Hotkey Mappings (Phase 2: Global Hotkeys System)
        public List<HotkeyMapping> HotkeyMappings { get; set; } = new List<HotkeyMapping>();
        
        // Global UI settings only
        public int TypingDelayMs { get; set; } = DefaultTimings.TypingDelayMs;
        public int TypingVariationMs { get; set; } = DefaultTimings.TypingVariationMs;
        
        // Legacy settings for backward compatibility (will be migrated)
        public string SystemPrompt { get; set; } = "You are a helpful assistant. Provide concise and direct responses.";
        public string ApiKey { get; set; } = string.Empty;
        public string LlmProvider { get; set; } = ProviderTypes.OpenAI;
        public string Model { get; set; } = DefaultModels.OpenAI_GPT4oMini;
        public string ApiEndpoint { get; set; } = DefaultEndpoints.OpenAI;
        public string VisionApiKey { get; set; } = string.Empty;
        public string VisionProvider { get; set; } = ProviderTypes.OpenAI;
        public string VisionModel { get; set; } = DefaultModels.OpenAI_GPT4o;
        public string VisionApiEndpoint { get; set; } = DefaultEndpoints.OpenAI;
        public string TriggerHotkey { get; set; } = DefaultHotkeys.SendQuery;
        public string OutputHotkey { get; set; } = DefaultHotkeys.Output;
        public string AbortHotkey { get; set; } = DefaultHotkeys.Abort;
        public string ScreenshotStartHotkey { get; set; } = DefaultHotkeys.ScreenshotStart;
        public string ScreenshotEndHotkey { get; set; } = DefaultHotkeys.ScreenshotEnd;
        public string AppendVisionHotkey { get; set; } = DefaultHotkeys.VisionReasoning;
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
                    
                    // Migration: Convert per-model hotkeys to global hotkey mappings
                    if (settings.HotkeyMappings.Count == 0 && settings.Models.Count > 0)
                    {
                        settings.MigrateToGlobalHotkeys();
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
                    IsDefault = true
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
                    IsDefault = false
                });
            }

            // If no models exist, create defaults
            if (Models.Count == 0)
            {
                Models.Add(new ModelConfig
                {
                    Name = "Quick Text",
                    Provider = ProviderTypes.OpenAI,
                    Model = DefaultModels.OpenAI_GPT4oMini,
                    Endpoint = DefaultEndpoints.OpenAI,
                    SystemPrompt = "You are a helpful assistant. Provide concise and direct responses.",
                    IsDefault = true
                });
            }
        }

        public ModelConfig? GetDefaultTextModel()
        {
            // Text model = default or first available
            return Models.FirstOrDefault(m => m.IsDefault) 
                   ?? Models.FirstOrDefault();
        }

        public ModelConfig? GetDefaultVisionModel()
        {
            // Vision model = model ID suggests vision capability
            return Models.FirstOrDefault(m => 
                       m.Model.Contains("vision", StringComparison.OrdinalIgnoreCase) ||
                       m.Model.Contains("4o", StringComparison.OrdinalIgnoreCase)) 
                   ?? Models.FirstOrDefault(m => m.IsDefault);
        }

        public ModelConfig? GetModelById(string id)
        {
            return Models.FirstOrDefault(m => m.Id == id);
        }

        private void MigrateToGlobalHotkeys()
        {
            // Migrate per-model hotkeys to global hotkey mappings
            int displayOrder = 0;

            // First, try to migrate from per-model hotkey properties if they still exist
            foreach (var model in Models)
            {
                // Try to get hotkeys from model properties (if they exist in JSON)
                // If not, we'll use legacy AppSettings hotkeys below
                var modelType = model.GetType();
                
                var triggerProp = modelType.GetProperty("TriggerHotkey");
                if (triggerProp != null)
                {
                    string? triggerValue = triggerProp.GetValue(model) as string;
                    if (!string.IsNullOrEmpty(triggerValue))
                    {
                        HotkeyMappings.Add(new HotkeyMapping
                        {
                            Hotkey = triggerValue,
                            ModelId = model.Id,
                            Action = HotkeyActions.ProcessText,
                            Description = $"Send query to {model.Name}",
                            DisplayOrder = displayOrder++
                        });
                    }
                }
            }

            // If no mappings created from models, use legacy AppSettings hotkeys
            if (HotkeyMappings.Count == 0)
            {
                var defaultModel = Models.FirstOrDefault(m => m.IsDefault) ?? Models.FirstOrDefault();
                if (defaultModel != null)
                {
                    // Use legacy hotkeys from AppSettings
                    if (!string.IsNullOrEmpty(TriggerHotkey))
                    {
                        HotkeyMappings.Add(new HotkeyMapping
                        {
                            Hotkey = TriggerHotkey,
                            ModelId = defaultModel.Id,
                            Action = HotkeyActions.ProcessText,
                            Description = "Send query to default model",
                            DisplayOrder = displayOrder++
                        });
                    }

                    if (!string.IsNullOrEmpty(ScreenshotStartHotkey))
                    {
                        HotkeyMappings.Add(new HotkeyMapping
                        {
                            Hotkey = ScreenshotStartHotkey,
                            ModelId = defaultModel.Id,
                            Action = HotkeyActions.ScreenshotStart,
                            Description = "Start screenshot",
                            DisplayOrder = displayOrder++
                        });
                    }

                    if (!string.IsNullOrEmpty(ScreenshotEndHotkey))
                    {
                        HotkeyMappings.Add(new HotkeyMapping
                        {
                            Hotkey = ScreenshotEndHotkey,
                            ModelId = defaultModel.Id,
                            Action = HotkeyActions.ScreenshotEnd,
                            Description = "End screenshot and analyze",
                            DisplayOrder = displayOrder++
                        });
                    }

                    if (!string.IsNullOrEmpty(AppendVisionHotkey))
                    {
                        HotkeyMappings.Add(new HotkeyMapping
                        {
                            Hotkey = AppendVisionHotkey,
                            ModelId = defaultModel.Id,
                            Action = HotkeyActions.VisionReasoning,
                            Description = "Vision + Reasoning",
                            DisplayOrder = displayOrder++
                        });
                    }
                }
            }

            // Always add global actions (Output, Abort) if not present
            if (!HotkeyMappings.Any(m => m.Action == HotkeyActions.Output))
            {
                HotkeyMappings.Add(new HotkeyMapping
                {
                    Hotkey = OutputHotkey,
                    ModelId = HotkeyActions.ModelGlobal,
                    Action = HotkeyActions.Output,
                    Description = "Output/type response",
                    DisplayOrder = displayOrder++
                });
            }

            if (!HotkeyMappings.Any(m => m.Action == HotkeyActions.Abort))
            {
                HotkeyMappings.Add(new HotkeyMapping
                {
                    Hotkey = AbortHotkey,
                    ModelId = HotkeyActions.ModelGlobal,
                    Action = HotkeyActions.Abort,
                    Description = "Abort/cancel operation",
                    DisplayOrder = displayOrder++
                });
            }

            // If still no mappings, create sensible defaults
            if (HotkeyMappings.Count == 0)
            {
                var defaultModel = Models.FirstOrDefault(m => m.IsDefault) ?? Models.FirstOrDefault();
                if (defaultModel != null)
                {
                    HotkeyMappings.AddRange(new[]
                    {
                        new HotkeyMapping
                        {
                            Hotkey = DefaultHotkeys.SendQuery,
                            ModelId = defaultModel.Id,
                            Action = HotkeyActions.ProcessText,
                            Description = "Send query to default model",
                            DisplayOrder = 0
                        },
                        new HotkeyMapping
                        {
                            Hotkey = DefaultHotkeys.Output,
                            ModelId = HotkeyActions.ModelGlobal,
                            Action = HotkeyActions.Output,
                            Description = "Output/type response",
                            DisplayOrder = 1
                        },
                        new HotkeyMapping
                        {
                            Hotkey = DefaultHotkeys.Abort,
                            ModelId = HotkeyActions.ModelGlobal,
                            Action = HotkeyActions.Abort,
                            Description = "Abort/cancel operation",
                            DisplayOrder = 2
                        }
                    });
                }
            }
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
                "Wraith");
            Directory.CreateDirectory(appDataPath);
            return Path.Combine(appDataPath, "settings.json");
        }
    }
}
