# Hotkeys Tab UX Fix - Complete Instructions

## ?? Goals

1. **Remove** per-model hotkey section from Models tab completely
2. **Redesign** Hotkeys tab to show ALL mappings globally in one intuitive view
3. Make it easy to add/edit hotkeys for any model without switching tabs

---

## ?? Changes Needed

### Part 1: Clean Up Models Tab (Remove Hotkey Section)

In `SettingsForm.Designer.cs`, **DELETE** these sections from the Models tab:

**Remove from field declarations (top of file):**
```csharp
// DELETE THESE:
private HotkeyTextBox txtTriggerHotkey;
private HotkeyTextBox txtOutputHotkey;
private HotkeyTextBox txtAbortHotkey;
private HotkeyTextBox txtScreenshotStartHotkey;
private HotkeyTextBox txtScreenshotEndHotkey;
private HotkeyTextBox txtAppendVisionHotkey;
private System.Windows.Forms.Button btnClearTrigger;
private System.Windows.Forms.Button btnClearOutput;
private System.Windows.Forms.Button btnClearAbort;
private System.Windows.Forms.Button btnClearScreenshotStart;
private System.Windows.Forms.Button btnClearScreenshotEnd;
private System.Windows.Forms.Button btnClearAppendVision;
```

**Remove from InitializeComponent (around lines 260-350):**

Delete the entire "Hotkeys section per model" block that starts with:
```csharp
// Hotkeys section per model
AddSectionLabel(this.pnlModelDetails, "Hotkeys (leave empty to disable)", 10, yPos, 480);
```

And ends before:
```csharp
// Model Chaining section
```

This includes all the txtTriggerHotkey, txtScreenshotStartHotkey, etc. controls and their clear buttons.

---

### Part 2: Redesign Hotkeys Tab Layout

**Replace the Hotkeys tab section** in `SettingsForm.Designer.cs` with this improved layout:

```csharp
// Hotkeys Tab - GLOBAL VIEW
this.tabHotkeys.Text = "Hotkeys";
this.tabHotkeys.UseVisualStyleBackColor = true;
this.tabHotkeys.AutoScroll = false;

// Title and description
System.Windows.Forms.Label lblHotkeysTitle = new System.Windows.Forms.Label();
lblHotkeysTitle.Text = "Global Hotkey Mappings";
lblHotkeysTitle.Location = new Point(20, 15);
lblHotkeysTitle.Size = new Size(300, 25);
lblHotkeysTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
this.tabHotkeys.Controls.Add(lblHotkeysTitle);

System.Windows.Forms.Label lblHotkeysDesc = new System.Windows.Forms.Label();
lblHotkeysDesc.Text = "Manage all hotkeys across all models in one place. Each hotkey can be assigned to any model and action.";
lblHotkeysDesc.Location = new Point(20, 45);
lblHotkeysDesc.Size = new Size(740, 35);
lblHotkeysDesc.ForeColor = Color.Gray;
this.tabHotkeys.Controls.Add(lblHotkeysDesc);

// Hotkey Mappings List - LARGER AND MORE PROMINENT
this.lstHotkeyMappings.Location = new Point(20, 90);
this.lstHotkeyMappings.Size = new Size(740, 300);
this.lstHotkeyMappings.DrawMode = DrawMode.OwnerDrawFixed;
this.lstHotkeyMappings.ItemHeight = 24; // Taller items for better readability
this.lstHotkeyMappings.DrawItem += LstHotkeyMappings_DrawItem;
this.lstHotkeyMappings.SelectedIndexChanged += LstHotkeyMappings_SelectedIndexChanged;
this.tabHotkeys.Controls.Add(this.lstHotkeyMappings);

// Action buttons - HORIZONTAL LAYOUT
int btnWidth = 110;
int btnY = 400;
int btnSpacing = 120;
int btnX = 20;

this.btnAddHotkey.Location = new Point(btnX, btnY);
this.btnAddHotkey.Size = new Size(btnWidth, 35);
this.btnAddHotkey.Text = "? Add Mapping";
this.btnAddHotkey.Click += BtnAddHotkey_Click;
this.tabHotkeys.Controls.Add(this.btnAddHotkey);

this.btnEditHotkey.Location = new Point(btnX + btnSpacing, btnY);
this.btnEditHotkey.Size = new Size(btnWidth, 35);
this.btnEditHotkey.Text = "?? Edit";
this.btnEditHotkey.Enabled = false;
this.btnEditHotkey.Click += BtnEditHotkey_Click;
this.tabHotkeys.Controls.Add(this.btnEditHotkey);

this.btnRemoveHotkey.Location = new Point(btnX + btnSpacing * 2, btnY);
this.btnRemoveHotkey.Size = new Size(btnWidth, 35);
this.btnRemoveHotkey.Text = "??? Remove";
this.btnRemoveHotkey.Enabled = false;
this.btnRemoveHotkey.Click += BtnRemoveHotkey_Click;
this.tabHotkeys.Controls.Add(this.btnRemoveHotkey);

this.btnMoveUp.Location = new Point(btnX + btnSpacing * 3, btnY);
this.btnMoveUp.Size = new Size(btnWidth, 35);
this.btnMoveUp.Text = "?? Move Up";
this.btnMoveUp.Enabled = false;
this.btnMoveUp.Click += BtnMoveUp_Click;
this.tabHotkeys.Controls.Add(this.btnMoveUp);

this.btnMoveDown.Location = new Point(btnX + btnSpacing * 4, btnY);
this.btnMoveDown.Size = new Size(btnWidth, 35);
this.btnMoveDown.Text = "?? Move Down";
this.btnMoveDown.Enabled = false;
this.btnMoveDown.Click += BtnMoveDown_Click;
this.tabHotkeys.Controls.Add(this.btnMoveDown);

// Edit Panel - MODAL DIALOG STYLE (centered, larger)
this.pnlHotkeyEdit.Location = new Point(200, 120);
this.pnlHotkeyEdit.Size = new Size(400, 280);
this.pnlHotkeyEdit.BorderStyle = BorderStyle.FixedSingle;
this.pnlHotkeyEdit.BackColor = Color.WhiteSmoke;
this.pnlHotkeyEdit.Visible = false;
this.tabHotkeys.Controls.Add(this.pnlHotkeyEdit);
this.pnlHotkeyEdit.BringToFront(); // Ensure it appears on top

int editY = 15;

// Title in edit panel
System.Windows.Forms.Label lblEditTitle = new System.Windows.Forms.Label();
lblEditTitle.Text = "Add/Edit Hotkey Mapping";
lblEditTitle.Location = new Point(10, editY);
lblEditTitle.Size = new Size(380, 25);
lblEditTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
this.pnlHotkeyEdit.Controls.Add(lblEditTitle);
editY += 35;

// Hotkey input
AddLabel(this.pnlHotkeyEdit, "Hotkey:", 10, editY, 80);
this.txtHotkeyEdit.Location = new Point(100, editY);
this.txtHotkeyEdit.Size = new Size(280, 23);
this.pnlHotkeyEdit.Controls.Add(this.txtHotkeyEdit);
editY += 35;

// Model dropdown
AddLabel(this.pnlHotkeyEdit, "Model:", 10, editY, 80);
this.cmbHotkeyModel.Location = new Point(100, editY);
this.cmbHotkeyModel.Size = new Size(280, 23);
this.cmbHotkeyModel.DropDownStyle = ComboBoxStyle.DropDownList;
this.pnlHotkeyEdit.Controls.Add(this.cmbHotkeyModel);
editY += 35;

// Action dropdown
AddLabel(this.pnlHotkeyEdit, "Action:", 10, editY, 80);
this.cmbHotkeyAction.Location = new Point(100, editY);
this.cmbHotkeyAction.Size = new Size(280, 23);
this.cmbHotkeyAction.DropDownStyle = ComboBoxStyle.DropDownList;
this.pnlHotkeyEdit.Controls.Add(this.cmbHotkeyAction);
editY += 35;

// Description
AddLabel(this.pnlHotkeyEdit, "Description:", 10, editY, 80);
this.txtHotkeyDescription.Location = new Point(100, editY);
this.txtHotkeyDescription.Size = new Size(280, 50);
this.txtHotkeyDescription.Multiline = true;
this.txtHotkeyDescription.ScrollBars = ScrollBars.Vertical;
this.pnlHotkeyEdit.Controls.Add(this.txtHotkeyDescription);
editY += 60;

// Enabled checkbox
this.chkHotkeyEnabled.Location = new Point(100, editY);
this.chkHotkeyEnabled.Text = "Enabled";
this.chkHotkeyEnabled.Size = new Size(100, 23);
this.chkHotkeyEnabled.Checked = true;
this.pnlHotkeyEdit.Controls.Add(this.chkHotkeyEnabled);
editY += 35;

// Save/Cancel buttons
this.btnSaveHotkey.Location = new Point(200, editY);
this.btnSaveHotkey.Size = new Size(90, 30);
this.btnSaveHotkey.Text = "?? Save";
this.btnSaveHotkey.Click += BtnSaveHotkey_Click;
this.pnlHotkeyEdit.Controls.Add(this.btnSaveHotkey);

this.btnCancelHotkey.Location = new Point(295, editY);
this.btnCancelHotkey.Size = new Size(90, 30);
this.btnCancelHotkey.Text = "? Cancel";
this.btnCancelHotkey.Click += BtnCancelHotkey_Click;
this.pnlHotkeyEdit.Controls.Add(this.btnCancelHotkey);
```

---

### Part 3: Update SettingsForm.cs Logic

**Remove per-model hotkey loading/saving** from `LoadModelToForm()` and `SaveCurrentModelWithoutRefresh()`:

In **LoadModelToForm()**, DELETE these lines:
```csharp
// DELETE:
txtTriggerHotkey.SetHotkeyString(model.TriggerHotkey ?? string.Empty);
txtScreenshotStartHotkey.SetHotkeyString(model.ScreenshotStartHotkey ?? string.Empty);
txtScreenshotEndHotkey.SetHotkeyString(model.ScreenshotEndHotkey ?? string.Empty);
txtAppendVisionHotkey.SetHotkeyString(model.AppendVisionHotkey ?? string.Empty);
txtOutputHotkey.SetHotkeyString(model.OutputHotkey ?? string.Empty);
txtAbortHotkey.SetHotkeyString(model.AbortHotkey ?? string.Empty);
```

In **SaveCurrentModelWithoutRefresh()**, DELETE these lines:
```csharp
// DELETE:
_selectedModel.TriggerHotkey = txtTriggerHotkey.GetHotkeyString();
_selectedModel.ScreenshotStartHotkey = txtScreenshotStartHotkey.GetHotkeyString();
_selectedModel.ScreenshotEndHotkey = txtScreenshotEndHotkey.GetHotkeyString();
_selectedModel.AppendVisionHotkey = txtAppendVisionHotkey.GetHotkeyString();
_selectedModel.OutputHotkey = txtOutputHotkey.GetHotkeyString();
_selectedModel.AbortHotkey = txtAbortHotkey.GetHotkeyString();
```

---

## ?? Expected Result

### Models Tab (After Fix):
```
???????????????????????????????????????
? [Model List]  ?  Model Details      ?
?               ?  • Name             ?
? Quick Text    ?  • Provider         ?
? GPT-4 Vision  ?  • API Key          ?
?               ?  • Model            ?
?               ?  • Endpoint         ?
?               ?  • System Prompt    ?
?               ?  • Model Chaining   ?
?               ?  • Options          ?
?               ?  [Test Model]       ?
?               ?                     ?
?               ?  (NO HOTKEYS HERE!) ?
???????????????????????????????????????
```

### Hotkeys Tab (After Fix):
```
???????????????????????????????????????????????????
?  Global Hotkey Mappings                         ?
?  Manage all hotkeys across all models...       ?
?                                                 ?
?  ???????????????????????????????????????????  ?
?  ? Ctrl+Shift+Q ? GPT-4 ? Send Query      ?  ?
?  ? Ctrl+Shift+1 ? Quick Text ? Send Query ?  ?
?  ? Ctrl+Shift+2 ? GPT-4 Vision ? Image    ?  ?
?  ? Ctrl+Shift+W ? (Global) ? Output       ?  ?
?  ? Ctrl+Shift+E ? (Global) ? Abort        ?  ?
?  ???????????????????????????????????????????  ?
?                                                 ?
?  [? Add] [?? Edit] [??? Remove] [?? Up] [?? Down] ?
???????????????????????????????????????????????????
```

When you click **Add** or **Edit**, a centered modal-like panel appears showing:
```
????????????????????????????
? Add/Edit Hotkey Mapping  ?
????????????????????????????
? Hotkey:  [Press keys...] ?
? Model:   [GPT-4 ?]       ?
? Action:  [Send Query ?]  ?
? Descr:   [Optional text] ?
? [?] Enabled              ?
?          [?? Save] [? Cancel] ?
????????????????????????????
```

---

## ? Benefits

1. **No per-model hotkey confusion** - Models tab only deals with model configuration
2. **Global view** - See ALL hotkeys across ALL models at once
3. **Intuitive editing** - Modal-style panel makes it clear you're editing
4. **No tab switching** - Everything happens in one place
5. **Clear visual hierarchy** - Buttons with emojis, larger list, better spacing

---

## ?? Implementation Steps

1. Open `SettingsForm.Designer.cs`
2. Delete per-model hotkey field declarations
3. Delete per-model hotkey section from Models tab InitializeComponent
4. Replace Hotkeys tab section with new layout
5. Open `SettingsForm.cs`
6. Remove hotkey loading/saving from LoadModelToForm and SaveCurrentModelWithoutRefresh
7. Build and test!

---

**This creates a truly global hotkeys system with an intuitive, single-tab interface!** ??
