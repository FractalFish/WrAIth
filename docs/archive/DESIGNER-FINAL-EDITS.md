# Final Designer Fixes - Line-by-Line Guide

You have SettingsForm.Designer.cs open. The file is too large for me to edit automatically.

Here are the EXACT line numbers to delete:

---

## Delete Section 1: Hotkeys from Models Tab (Lines ~260-350)

**FIND this line (around line 260):**
```csharp
            // Hotkeys section per model
```

**DELETE everything from that line down to (but NOT including) this line:**
```csharp
            // Model Chaining section
```

This removes all the txt Trigger/Screenshot/Output/Abort hotkey controls from the Models tab.

---

## Delete Section 2: Old Hotkeys Tab Content (Lines ~430-540)

**FIND this section (around line 430):**
```csharp
            // Hotkeys Tab
            this.tabHotkeys.Text = "Hotkeys";
            this.tabHotkeys.UseVisualStyleBackColor = true;
            this.tabHotkeys.AutoScroll = true;
            
            // Hotkeys settings (global)
            int hotkeyYPos = 20;
```

**DELETE everything from `// Hotkeys Tab` down to (but NOT including):**
```csharp
            // Options Tab
```

---

## Add Section 3: NEW Hotkeys Tab (Insert where you just deleted)

**INSERT this code where you just deleted the old Hotkeys tab:**

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

---

## Summary:

1. Delete hotkeys from Models tab (~lines 260-350)
2. Delete old Hotkeys tab content (~lines 430-540)
3. Insert new Hotkeys tab code where you deleted #2

**Save, build, and test!**
