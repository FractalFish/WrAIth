# Step-by-Step Application Instructions

## Issue: Files are locked, manual application needed

Since the Designer file is locked, here are the **exact changes** to make manually:

---

## Part 1: Update SettingsForm.cs

### In `LoadModelToForm()` method, DELETE these lines:

Find this section (around line 120-127):
```csharp
// DELETE THESE LINES:
// Load per-model hotkeys
txtTriggerHotkey.SetHotkeyString(model.TriggerHotkey ?? string.Empty);
txtScreenshotStartHotkey.SetHotkeyString(model.ScreenshotStartHotkey ?? string.Empty);
txtScreenshotEndHotkey.SetHotkeyString(model.ScreenshotEndHotkey ?? string.Empty);
txtAppendVisionHotkey.SetHotkeyString(model.AppendVisionHotkey ?? string.Empty);
txtOutputHotkey.SetHotkeyString(model.OutputHotkey ?? string.Empty);
txtAbortHotkey.SetHotkeyString(model.AbortHotkey ?? string.Empty);
```

### In `SaveCurrentModelWithoutRefresh()` method, DELETE these lines:

Find this section (around line 185-192):
```csharp
// DELETE THESE LINES:
// Save per-model hotkeys
_selectedModel.TriggerHotkey = txtTriggerHotkey.GetHotkeyString();
_selectedModel.ScreenshotStartHotkey = txtScreenshotStartHotkey.GetHotkeyString();
_selectedModel.ScreenshotEndHotkey = txtScreenshotEndHotkey.GetHotkeyString();
_selectedModel.AppendVisionHotkey = txtAppendVisionHotkey.GetHotkeyString();
_selectedModel.OutputHotkey = txtOutputHotkey.GetHotkeyString();
_selectedModel.AbortHotkey = txtAbortHotkey.GetHotkeyString();
```

**Result:** Models tab logic no longer touches per-model hotkeys

---

## Part 2: Update SettingsForm.Designer.cs

This is more complex. I'll create the complete fixed InitializeComponent method in a separate file.

---

## Verification

After making changes:
1. Build should succeed
2. Models tab should NOT show hotkey fields
3. Hotkeys tab should show global list with Add/Edit/Remove buttons
4. Clicking Add/Edit should show a centered modal panel

Would you like me to create the complete replacement InitializeComponent method?
