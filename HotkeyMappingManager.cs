using Wraith.Constants;

namespace Wraith
{
    /// <summary>
    /// Utility class for managing and validating hotkey mappings
    /// </summary>
    public static class HotkeyMappingManager
    {
        /// <summary>
        /// Find mapping by hotkey combination
        /// </summary>
        public static HotkeyMapping? FindByHotkey(List<HotkeyMapping> mappings, string hotkey)
        {
            return mappings.FirstOrDefault(m => 
                m.IsEnabled && 
                m.Hotkey.Equals(hotkey, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Find all mappings for a specific model
        /// </summary>
        public static List<HotkeyMapping> FindByModel(List<HotkeyMapping> mappings, string modelId)
        {
            return mappings.Where(m => 
                m.IsEnabled && 
                m.ModelId.Equals(modelId, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.DisplayOrder)
                .ToList();
        }

        /// <summary>
        /// Find all mappings for a specific action
        /// </summary>
        public static List<HotkeyMapping> FindByAction(List<HotkeyMapping> mappings, string action)
        {
            return mappings.Where(m => 
                m.IsEnabled && 
                m.Action.Equals(action, StringComparison.OrdinalIgnoreCase))
                .OrderBy(m => m.DisplayOrder)
                .ToList();
        }

        /// <summary>
        /// Check if a hotkey is already in use
        /// </summary>
        public static bool IsHotkeyInUse(List<HotkeyMapping> mappings, string hotkey, string? excludeMappingId = null)
        {
            return mappings.Any(m => 
                m.IsEnabled &&
                m.Hotkey.Equals(hotkey, StringComparison.OrdinalIgnoreCase) &&
                (excludeMappingId == null || m.Id != excludeMappingId));
        }

        /// <summary>
        /// Validate a mapping and return error message if invalid
        /// </summary>
        public static string? ValidateMapping(HotkeyMapping mapping, AppSettings settings, string? excludeMappingId = null)
        {
            if (string.IsNullOrWhiteSpace(mapping.Hotkey))
                return "Hotkey cannot be empty";

            if (string.IsNullOrWhiteSpace(mapping.Action))
                return "Action must be selected";

            // Check for duplicate hotkeys
            if (IsHotkeyInUse(settings.HotkeyMappings, mapping.Hotkey, excludeMappingId))
                return $"Hotkey '{mapping.Hotkey}' is already in use";

            // Global actions don't need a model
            if (mapping.IsGlobalAction())
                return null;

            // Other actions need a valid model
            if (string.IsNullOrWhiteSpace(mapping.ModelId) || 
                mapping.ModelId == HotkeyActions.ModelNone)
                return "A model must be selected for this action";

            // Verify model exists (except special values)
            if (mapping.ModelId != HotkeyActions.ModelCurrent &&
                mapping.ModelId != HotkeyActions.ModelGlobal)
            {
                var model = settings.GetModelById(mapping.ModelId);
                if (model == null)
                    return $"Model not found: {mapping.ModelId}";
            }

            return null; // Valid
        }

        /// <summary>
        /// Get all enabled mappings sorted by display order
        /// </summary>
        public static List<HotkeyMapping> GetEnabledMappings(List<HotkeyMapping> mappings)
        {
            return mappings
                .Where(m => m.IsEnabled)
                .OrderBy(m => m.DisplayOrder)
                .ToList();
        }

        /// <summary>
        /// Create a default mapping for a model
        /// </summary>
        public static HotkeyMapping CreateDefaultMapping(ModelConfig model, string action, string hotkey)
        {
            return new HotkeyMapping
            {
                Hotkey = hotkey,
                ModelId = model.Id,
                Action = action,
                Description = $"{HotkeyActions.GetDisplayName(action)} with {model.Name}",
                IsEnabled = true
            };
        }

        /// <summary>
        /// Reorder mappings after one is moved
        /// </summary>
        public static void ReorderMappings(List<HotkeyMapping> mappings)
        {
            var ordered = mappings.OrderBy(m => m.DisplayOrder).ToList();
            for (int i = 0; i < ordered.Count; i++)
            {
                ordered[i].DisplayOrder = i;
            }
        }

        /// <summary>
        /// Move a mapping up in the display order
        /// </summary>
        public static bool MoveUp(List<HotkeyMapping> mappings, string mappingId)
        {
            var mapping = mappings.FirstOrDefault(m => m.Id == mappingId);
            if (mapping == null || mapping.DisplayOrder == 0)
                return false;

            var previous = mappings
                .Where(m => m.DisplayOrder < mapping.DisplayOrder)
                .OrderByDescending(m => m.DisplayOrder)
                .FirstOrDefault();

            if (previous != null)
            {
                int temp = mapping.DisplayOrder;
                mapping.DisplayOrder = previous.DisplayOrder;
                previous.DisplayOrder = temp;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Move a mapping down in the display order
        /// </summary>
        public static bool MoveDown(List<HotkeyMapping> mappings, string mappingId)
        {
            var mapping = mappings.FirstOrDefault(m => m.Id == mappingId);
            if (mapping == null)
                return false;

            var next = mappings
                .Where(m => m.DisplayOrder > mapping.DisplayOrder)
                .OrderBy(m => m.DisplayOrder)
                .FirstOrDefault();

            if (next != null)
            {
                int temp = mapping.DisplayOrder;
                mapping.DisplayOrder = next.DisplayOrder;
                next.DisplayOrder = temp;
                return true;
            }

            return false;
        }
    }
}
