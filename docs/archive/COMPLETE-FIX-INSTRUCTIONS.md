# Complete Fix - Manual Application Required

## ?? Files Still Locked

Visual Studio still has the files locked. Here's what to do:

---

## ?? Quick Fix Steps

### Step 1: Close SettingsForm.Designer.cs and SettingsForm.cs in VS

1. In Visual Studio, **close these tabs**:
   - SettingsForm.Designer.cs
   - SettingsForm.cs

2. **Save all** (Ctrl+Shift+S)

### Step 2: Apply Changes to SettingsForm.cs

Open `SettingsForm.cs` and make these TWO deletions:

#### Delete #1: In `LoadModelToForm()` method (around line 119-126)

**FIND and DELETE these lines:**
```csharp
// Load per-model hotkeys
txtTriggerHotkey.SetHotkeyString(model.TriggerHotkey ?? string.Empty);
txtScreenshotStartHotkey.SetHotkeyString(model.ScreenshotStartHotkey ?? string.Empty);
txtScreenshotEndHotkey.SetHotkeyString(model.ScreenshotEndHotkey ?? string.Empty);
txtAppendVisionHotkey.SetHotkeyString(model.AppendVisionHotkey ?? string.Empty);
txtOutputHotkey.SetHotkeyString(model.OutputHotkey ?? string.Empty);
txtAbortHotkey.SetHotkeyString(model.AbortHotkey ?? string.Empty);
```

#### Delete #2: In `SaveCurrentModelWithoutRefresh()` method (around line 185-192)

**FIND and DELETE these lines:**
```csharp
// Save per-model hotkeys
_selectedModel.TriggerHotkey = txtTriggerHotkey.GetHotkeyString();
_selectedModel.ScreenshotStartHotkey = txtScreenshotStartHotkey.GetHotkeyString();
_selectedModel.ScreenshotEndHotkey = txtScreenshotEndHotkey.GetHotkeyString();
_selectedModel.AppendVisionHotkey = txtAppendVisionHotkey.GetHotkeyString();
_selectedModel.OutputHotkey = txtOutputHotkey.GetHotkeyString();
_selectedModel.AbortHotkey = txtAbortHotkey.GetHotkeyString();
```

**Save the file** ?

---

### Step 3: Apply Changes to SettingsForm.Designer.cs

This is more involved. You have **two options**:

#### Option A: Use Find & Replace (Faster)

1. Open `SettingsForm.Designer.cs`

2. **Delete old hotkey field declarations** (around lines 26-41):

**FIND this block:**
```csharp
// Hotkeys
private HotkeyTextBox txtTriggerHotkey;
private HotkeyTextBox txtOutputHotkey;
private HotkeyTextBox txtAbortHotkey;
private HotkeyTextBox txtScreenshotStartHotkey;
private HotkeyTextBox txtScreenshotEndHotkey;
private HotkeyTextBox txtAppendVisionHotkey;

// Clear buttons for hotkeys
private System.Windows.Forms.Button btnClearTrigger;
private System.Windows.Forms.Button btnClearOutput;
private System.Windows.Forms.Button btnClearAbort;
private System.Windows.Forms.Button btnClearScreenshotStart;
private System.Windows.Forms.Button btnClearScreenshotEnd;
private System.Windows.Forms.Button btnClearAppendVision;
```

**DELETE IT** ?

3. **Delete hotkey initialization in InitializeComponent** (around lines 115-135):

**FIND and DELETE:**
```csharp
// Hotkeys
this.txtTriggerHotkey = new HotkeyTextBox();
this.txtOutputHotkey = new HotkeyTextBox();
this.txtAbortHotkey = new HotkeyTextBox();
this.txtScreenshotStartHotkey = new HotkeyTextBox();
this.txtScreenshotEndHotkey = new HotkeyTextBox();
this.txtAppendVisionHotkey = new HotkeyTextBox();

// Clear buttons
this.btnClearTrigger = new System.Windows.Forms.Button();
this.btnClearOutput = new System.Windows.Forms.Button();
this.btnClearAbort = new System.Windows.Forms.Button();
this.btnClearScreenshotStart = new System.Windows.Forms.Button();
this.btnClearScreenshotEnd = new System.Windows.Forms.Button();
this.btnClearAppendVision = new System.Windows.Forms.Button();
```

4. **Delete entire hotkey section from Models tab** (around lines 260-350):

**FIND the section starting with:**
```csharp
// Hotkeys section per model
AddSectionLabel(this.pnlModelDetails, "Hotkeys (leave empty to disable)", 10, yPos, 480);
```

**DELETE everything down to (but NOT including):**
```csharp
// Model Chaining section
```

This includes all the txtTriggerHotkey, txtScreenshotStartHotkey, etc. controls and their clear buttons.

5. **Delete old Hotkeys tab content** (around lines 430-540):

**FIND the section starting with:**
```csharp
// Hotkeys Tab
this.tabHotkeys.Text = "Hotkeys";
```

**DELETE everything down to (but NOT including):**
```csharp
// Options Tab
```

6. **Add NEW Hotkeys tab layout** - Insert this code where you just deleted:

```csharp
// Hotkeys Tab - GLOBAL VIEW
this.tabHotkeys.Text = "Hotkeys";
this.tabHotkeys.UseVisualStyleBackColor = true;
this.tabHotkeys.AutoScroll = false;

// Title
System.Windows.Forms.Label lblHotkeysTitle = new System.Windows.Forms.Label();
lblHotkeysTitle.Text = "Global Hotkey Mappings";
lblHotkeysTitle.Location = new Point(20, 15);
lblHotkeysTitle.Size = new Size(300, 25);
lblHotkeysTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
this.tabHotkeys.Controls.Add(lblHotkeysTitle);

System.Windows.Forms.Label lblHotkeysDesc = new System.Windows.Forms.Label();
lblHotkeysDesc.Text = "Manage all hotkeys across all models. Select any hotkey to edit.";
lblHotkeysDesc.Location = new Point(20, 45);
lblHotkeysDesc.Size = new Size(740, 30);
lblHotkeysDesc.ForeColor = Color.Gray;
this.tabHotkeys.Controls.Add(lblHotkeysDesc);

// Hotkey Mappings List - LARGE AND PROMINENT
this.lstHotkeyMappings.Location = new Point(20, 85);
this.lstHotkeyMappings.Size = new Size(740, 305);
this.lstHotkeyMappings.DrawMode = DrawMode.OwnerDrawFixed;
this.lstHotkeyMappings.ItemHeight = 24;
this.lstHotkeyMappings.DrawItem += LstHotkeyMappings_DrawItem;
this.lstHotkeyMappings.SelectedIndexChanged += LstHotkeyMappings_SelectedIndexChanged;
this.tabHotkeys.Controls.Add(this.lstHotkeyMappings);

// Buttons - HORIZONTAL
int btnY = 400;
this.btnAddHotkey.Location = new Point(20, btnY);
this.btnAddHotkey.Size = new Size(110, 35);
this.btnAddHotkey.Text = "Add Mapping";
this.btnAddHotkey.Click += BtnAddHotkey_Click;
this.tabHotkeys.Controls.Add(this.btnAddHotkey);

this.btnEditHotkey.Location = new Point(140, btnY);
this.btnEditHotkey.Size = new Size(110, 35);
this.btnEditHotkey.Text = "Edit";
this.btnEditHotkey.Enabled = false;
this.btnEditHotkey.Click += BtnEditHotkey_Click;
this.tabHotkeys.Controls.Add(this.btnEditHotkey);

this.btnRemoveHotkey.Location = new Point(260, btnY);
this.btnRemoveHotkey.Size = new Size(110, 35);
this.btnRemoveHotkey.Text = "Remove";
this.btnRemoveHotkey.Enabled = false;
this.btnRemoveHotkey.Click += BtnRemoveHotkey_Click;
this.tabHotkeys.Controls.Add(this.btnRemoveHotkey);

this.btnMoveUp.Location = new Point(380, btnY);
this.btnMoveUp.Size = new Size(110, 35);
this.btnMoveUp.Text = "Move Up";
this.btnMoveUp.Enabled = false;
this.btnMoveUp.Click += BtnMoveUp_Click;
this.tabHotkeys.Controls.Add(this.btnMoveUp);

this.btnMoveDown.Location = new Point(500, btnY);
this.btnMoveDown.Size = new Size(110, 35);
this.btnMoveDown.Text = "Move Down";
this.btnMoveDown.Enabled = false;
this.btnMoveDown.Click += BtnMoveDown_Click;
this.tabHotkeys.Controls.Add(this.btnMoveDown);

// Edit Panel - MODAL STYLE
this.pnlHotkeyEdit.Location = new Point(200, 120);
this.pnlHotkeyEdit.Size = new Size(400, 280);
this.pnlHotkeyEdit.BorderStyle = BorderStyle.FixedSingle;
this.pnlHotkeyEdit.BackColor = Color.WhiteSmoke;
this.pnlHotkeyEdit.Visible = false;
this.tabHotkeys.Controls.Add(this.pnlHotkeyEdit);
this.pnlHotkeyEdit.BringToFront();

int editY = 15;

System.Windows.Forms.Label lblEditTitle = new System.Windows.Forms.Label();
lblEditTitle.Text = "Hotkey Mapping";
lblEditTitle.Location = new Point(10, editY);
lblEditTitle.Size = new Size(380, 25);
lblEditTitle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
this.pnlHotkeyEdit.Controls.Add(lblEditTitle);
editY += 35;

AddLabel(this.pnlHotkeyEdit, "Hotkey:", 10, editY, 80);
this.txtHotkeyEdit.Location = new Point(100, editY);
this.txtHotkeyEdit.Size = new Size(280, 23);
this.pnlHotkeyEdit.Controls.Add(this.txtHotkeyEdit);
editY += 35;

AddLabel(this.pnlHotkeyEdit, "Model:", 10, editY, 80);
this.cmbHotkeyModel.Location = new Point(100, editY);
this.cmbHotkeyModel.Size = new Size(280, 23);
this.cmbHotkeyModel.DropDownStyle = ComboBoxStyle.DropDownList;
this.pnlHotkeyEdit.Controls.Add(this.cmbHotkeyModel);
editY += 35;

AddLabel(this.pnlHotkeyEdit, "Action:", 10, editY, 80);
this.cmbHotkeyAction.Location = new Point(100, editY);
this.cmbHotkeyAction.Size = new Size(280, 23);
this.cmbHotkeyAction.DropDownStyle = ComboBoxStyle.DropDownList;
this.pnlHotkeyEdit.Controls.Add(this.cmbHotkeyAction);
editY += 35;

AddLabel(this.pnlHotkeyEdit, "Description:", 10, editY, 80);
this.txtHotkeyDescription.Location = new Point(100, editY);
this.txtHotkeyDescription.Size = new Size(280, 50);
this.txtHotkeyDescription.Multiline = true;
this.txtHotkeyDescription.ScrollBars = ScrollBars.Vertical;
this.pnlHotkeyEdit.Controls.Add(this.txtHotkeyDescription);
editY += 60;

this.chkHotkeyEnabled.Location = new Point(100, editY);
this.chkHotkeyEnabled.Text = "Enabled";
this.chkHotkeyEnabled.Size = new Size(100, 23);
this.chkHotkeyEnabled.Checked = true;
this.pnlHotkeyEdit.Controls.Add(this.chkHotkeyEnabled);
editY += 35;

this.btnSaveHotkey.Location = new Point(200, editY);
this.btnSaveHotkey.Size = new Size(90, 30);
this.btnSaveHotkey.Text = "Save";
this.btnSaveHotkey.Click += BtnSaveHotkey_Click;
this.pnlHotkeyEdit.Controls.Add(this.btnSaveHotkey);

this.btnCancelHotkey.Location = new Point(295, editY);
this.btnCancelHotkey.Size = new Size(90, 30);
this.btnCancelHotkey.Text = "Cancel";
this.btnCancelHotkey.Click += BtnCancelHotkey_Click;
this.pnlHotkeyEdit.Controls.Add(this.btnCancelHotkey);
```

**Save the file** ?

---

### Step 4: Build and Test

1. **Build** the solution (Ctrl+Shift+B)
2. **Run** the application (F5)
3. **Open Settings** ? **Hotkeys tab**
4. **Verify**:
   - Models tab has NO hotkey fields
   - Hotkeys tab shows global list
   - Add/Edit buttons work

---

## ? Expected Result

**Models Tab:** Only shows model configuration (no hotkeys)
**Hotkeys Tab:** Shows global list of ALL hotkeys with intuitive Add/Edit/Remove interface

---

**Let me know if you need help with any step!** ??
