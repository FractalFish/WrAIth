namespace BLLMT
{
    // Extension methods for Settings Form
    public static class SettingsFormExtensions
    {
        public static List<string> DetectHotkeyConflicts(this AppSettings settings)
        {
            var conflicts = new List<string>();
            var hotkeyMap = new Dictionary<string, List<(string ModelName, string HotkeyType)>>();

            foreach (var model in settings.Models)
            {
                // Collect all hotkeys for this model
                var hotkeys = new Dictionary<string, string>
                {
                    { "Trigger", model.TriggerHotkey },
                    { "Output", model.OutputHotkey },
                    { "Abort", model.AbortHotkey },
                    { "Screenshot Start", model.ScreenshotStartHotkey },
                    { "Screenshot End", model.ScreenshotEndHotkey },
                    { "Append Vision", model.AppendVisionHotkey }
                };

                foreach (var kvp in hotkeys)
                {
                    if (string.IsNullOrWhiteSpace(kvp.Value))
                        continue; // Skip empty hotkeys

                    string normalizedHotkey = NormalizeHotkey(kvp.Value);
                    
                    if (!hotkeyMap.ContainsKey(normalizedHotkey))
                        hotkeyMap[normalizedHotkey] = new List<(string, string)>();
                    
                    hotkeyMap[normalizedHotkey].Add((model.Name, kvp.Key));
                }
            }

            // Find conflicts
            foreach (var kvp in hotkeyMap)
            {
                if (kvp.Value.Count > 1)
                {
                    var usages = kvp.Value.Select(v => $"'{v.ModelName}' ({v.HotkeyType})");
                    conflicts.Add($"'{kvp.Key}' is used by: {string.Join(", ", usages)}");
                }
            }

            return conflicts;
        }

        private static string NormalizeHotkey(string hotkey)
        {
            // Normalize hotkey string for comparison
            // Convert "Ctrl" to "Control", remove spaces, make uppercase
            return hotkey
                .Replace("Ctrl", "Control")
                .Replace(" ", "")
                .Replace("+", "")
                .ToUpperInvariant();
        }
    }
}
