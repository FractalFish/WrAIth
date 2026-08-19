namespace Wraith
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabModels;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.TabPage tabHotkeys;
        
        // Models tab
        private System.Windows.Forms.ListBox lstModels;
        private System.Windows.Forms.Button btnAddModel;
        private System.Windows.Forms.Button btnRemoveModel;
        private System.Windows.Forms.Button btnSetDefault;
        private System.Windows.Forms.Panel pnlModelDetails;
        private System.Windows.Forms.TextBox txtModelName;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.Button btnEditCustomFormat;
        private System.Windows.Forms.ComboBox txtModel;
        private System.Windows.Forms.Button btnFetchModels;
        private System.Windows.Forms.TextBox txtEndpoint;
        private System.Windows.Forms.CheckBox chkIsEnabled;
        private System.Windows.Forms.Button btnTestModel;
        private System.Windows.Forms.TextBox txtSystemPrompt;
        
        // Model chaining
        private System.Windows.Forms.ComboBox cmbChainToModel;
        private System.Windows.Forms.ComboBox cmbChainMode;
        
        // Options tab controls
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.NumericUpDown nudTypingDelay;
        private System.Windows.Forms.NumericUpDown nudTypingVariation;
        private System.Windows.Forms.CheckBox chkAutoDetectImages;
        private System.Windows.Forms.CheckBox chkCombineScreenshot;
        
        // Hotkeys tab controls
        private System.Windows.Forms.ListBox lstHotkeyMappings;
        private System.Windows.Forms.Button btnAddHotkey;
        private System.Windows.Forms.Button btnEditHotkey;
        private System.Windows.Forms.Button btnRemoveHotkey;
        private System.Windows.Forms.Button btnMoveUp;
        private System.Windows.Forms.Button btnMoveDown;
        private System.Windows.Forms.Panel pnlHotkeyEdit;
        private HotkeyTextBox txtHotkeyEdit;
        private System.Windows.Forms.ComboBox cmbHotkeyModel;
        private System.Windows.Forms.ComboBox cmbHotkeyAction;
        private System.Windows.Forms.TextBox txtHotkeyDescription;
        private System.Windows.Forms.CheckBox chkHotkeyEnabled;
        private System.Windows.Forms.Button btnSaveHotkey;
        private System.Windows.Forms.Button btnCancelHotkey;
        
        // Bottom buttons
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabModels = new System.Windows.Forms.TabPage();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.tabHotkeys = new System.Windows.Forms.TabPage();
            
            // Models tab controls
            this.lstModels = new System.Windows.Forms.ListBox();
            this.btnAddModel = new System.Windows.Forms.Button();
            this.btnRemoveModel = new System.Windows.Forms.Button();
            this.btnSetDefault = new System.Windows.Forms.Button();
            this.pnlModelDetails = new System.Windows.Forms.Panel();
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.btnEditCustomFormat = new System.Windows.Forms.Button();
            this.txtModel = new System.Windows.Forms.ComboBox();
            this.btnFetchModels = new System.Windows.Forms.Button();
            this.txtEndpoint = new System.Windows.Forms.TextBox();
            this.chkIsEnabled = new System.Windows.Forms.CheckBox();
            this.btnTestModel = new System.Windows.Forms.Button();
            this.txtSystemPrompt = new System.Windows.Forms.TextBox();
            
            // Model chaining
            this.cmbChainToModel = new System.Windows.Forms.ComboBox();
            this.cmbChainMode = new System.Windows.Forms.ComboBox();
            
            // Options tab
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.nudTypingDelay = new System.Windows.Forms.NumericUpDown();
            this.nudTypingVariation = new System.Windows.Forms.NumericUpDown();
            this.chkAutoDetectImages = new System.Windows.Forms.CheckBox();
            this.chkCombineScreenshot = new System.Windows.Forms.CheckBox();
            
            // Hotkeys tab
            this.lstHotkeyMappings = new System.Windows.Forms.ListBox();
            this.btnAddHotkey = new System.Windows.Forms.Button();
            this.btnEditHotkey = new System.Windows.Forms.Button();
            this.btnRemoveHotkey = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.pnlHotkeyEdit = new System.Windows.Forms.Panel();
            this.txtHotkeyEdit = new HotkeyTextBox();
            this.cmbHotkeyModel = new System.Windows.Forms.ComboBox();
            this.cmbHotkeyAction = new System.Windows.Forms.ComboBox();
            this.txtHotkeyDescription = new System.Windows.Forms.TextBox();
            this.chkHotkeyEnabled = new System.Windows.Forms.CheckBox();
            this.btnSaveHotkey = new System.Windows.Forms.Button();
            this.btnCancelHotkey = new System.Windows.Forms.Button();
            
            // Bottom buttons
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            
            ((System.ComponentModel.ISupportInitialize)(this.nudTypingDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTypingVariation)).BeginInit();
            this.SuspendLayout();

            // Form
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimumSize = new Size(800, 600);
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Wraith Settings";
            this.Font = new Font("Segoe UI", 9F);

            // TabControl
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 0);
            this.tabControl.TabIndex = 0;
            this.tabControl.Controls.Add(this.tabModels);
            this.tabControl.Controls.Add(this.tabOptions);
            this.tabControl.Controls.Add(this.tabHotkeys);
            this.Controls.Add(this.tabControl);

            // Models Tab
            this.tabModels.Text = "Models";
            this.tabModels.UseVisualStyleBackColor = true;
            this.tabModels.AutoScroll = true;
            
            // Models ListBox
            this.lstModels.Location = new Point(10, 10);
            this.lstModels.Size = new Size(250, 420);
            // DrawMode = Normal for now, custom drawing can be added later
            this.lstModels.SelectedIndexChanged += LstModels_SelectedIndexChanged;
            this.tabModels.Controls.Add(this.lstModels);
            
            // Model buttons
            this.btnAddModel.Location = new Point(10, 440);
            this.btnAddModel.Size = new Size(80, 30);
            this.btnAddModel.Text = "Add";
            this.btnAddModel.Click += BtnAddModel_Click;
            this.tabModels.Controls.Add(this.btnAddModel);
            
            this.btnRemoveModel.Location = new Point(95, 440);
            this.btnRemoveModel.Size = new Size(80, 30);
            this.btnRemoveModel.Text = "Remove";
            this.btnRemoveModel.Click += BtnRemoveModel_Click;
            this.tabModels.Controls.Add(this.btnRemoveModel);
            
            this.btnSetDefault.Location = new Point(180, 440);
            this.btnSetDefault.Size = new Size(80, 30);
            this.btnSetDefault.Text = "Set Default";
            this.btnSetDefault.Click += BtnSetDefault_Click;
            this.tabModels.Controls.Add(this.btnSetDefault);
            
            // Model Details Panel
            this.pnlModelDetails.Location = new Point(270, 10);
            this.pnlModelDetails.Size = new Size(510, 470);
            this.pnlModelDetails.AutoScroll = true;
            this.pnlModelDetails.BorderStyle = BorderStyle.FixedSingle;
            this.tabModels.Controls.Add(this.pnlModelDetails);
            
            int yPos = 10;
            
            AddLabel(this.pnlModelDetails, "Model Name:", 10, yPos);
            this.txtModelName.Location = new Point(120, yPos);
            this.txtModelName.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtModelName);
            yPos += 35;
            
            // Add Enabled checkbox at the top
            this.chkIsEnabled = new System.Windows.Forms.CheckBox();
            this.chkIsEnabled.Location = new Point(10, yPos);
            this.chkIsEnabled.Text = "Enabled (Model is active)";
            this.chkIsEnabled.Size = new Size(200, 23);
            this.chkIsEnabled.Checked = true;
            this.pnlModelDetails.Controls.Add(this.chkIsEnabled);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "Provider:", 10, yPos);
            this.cmbProvider.Location = new Point(120, yPos);
            this.cmbProvider.Size = new Size(280, 23);
            this.cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbProvider.Items.AddRange(new object[] { "OpenAI", "Anthropic", "Groq", "Custom" });
            this.cmbProvider.SelectedIndexChanged += CmbProvider_SelectedIndexChanged;
            this.pnlModelDetails.Controls.Add(this.cmbProvider);
            
            this.btnEditCustomFormat = new System.Windows.Forms.Button();
            this.btnEditCustomFormat.Location = new Point(405, yPos);
            this.btnEditCustomFormat.Size = new Size(85, 23);
            this.btnEditCustomFormat.Text = "Edit Format";
            this.btnEditCustomFormat.Visible = false; // Only show when Custom is selected
            this.btnEditCustomFormat.Click += BtnEditCustomFormat_Click;
            this.pnlModelDetails.Controls.Add(this.btnEditCustomFormat);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "API Key:", 10, yPos);
            this.txtApiKey.Location = new Point(120, yPos);
            this.txtApiKey.Size = new Size(370, 23);
            this.txtApiKey.UseSystemPasswordChar = true;
            this.pnlModelDetails.Controls.Add(this.txtApiKey);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "Model:", 10, yPos);
            this.txtModel.Location = new Point(120, yPos);
            this.txtModel.Size = new Size(280, 23);
            this.txtModel.DropDownStyle = ComboBoxStyle.DropDown; // editable: pick from the list or type your own
            this.pnlModelDetails.Controls.Add(this.txtModel);

            this.btnFetchModels.Location = new Point(405, yPos);
            this.btnFetchModels.Size = new Size(85, 23);
            this.btnFetchModels.Text = "Fetch List";
            this.btnFetchModels.Click += BtnFetchModels_Click;
            this.pnlModelDetails.Controls.Add(this.btnFetchModels);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "Endpoint:", 10, yPos);
            this.txtEndpoint.Location = new Point(120, yPos);
            this.txtEndpoint.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtEndpoint);
            yPos += 35;
            
            // System Prompt per model
            AddLabel(this.pnlModelDetails, "System Prompt:", 10, yPos);
            yPos += 25;
            this.txtSystemPrompt.Location = new Point(10, yPos);
            this.txtSystemPrompt.Size = new Size(480, 80);
            this.txtSystemPrompt.Multiline = true;
            this.txtSystemPrompt.ScrollBars = ScrollBars.Vertical;
            this.txtSystemPrompt.BorderStyle = BorderStyle.FixedSingle;
            this.pnlModelDetails.Controls.Add(this.txtSystemPrompt);
            yPos += 90;
            
            // Model Chaining section
            AddSectionLabel(this.pnlModelDetails, "Model Chaining", 10, yPos, 480);
            yPos += 25;
            
            AddLabel(this.pnlModelDetails, "Chain output to:", 10, yPos);
            this.cmbChainToModel.Location = new Point(120, yPos);
            this.cmbChainToModel.Size = new Size(370, 23);
            this.cmbChainToModel.DropDownStyle = ComboBoxStyle.DropDownList;
            this.pnlModelDetails.Controls.Add(this.cmbChainToModel);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Chain mode:", 10, yPos);
            this.cmbChainMode.Location = new Point(120, yPos);
            this.cmbChainMode.Size = new Size(370, 23);
            this.cmbChainMode.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbChainMode.Items.AddRange(new object[] { "None", "Append", "Prepend", "Replace" });
            this.pnlModelDetails.Controls.Add(this.cmbChainMode);
            yPos += 40;
            
            // Options section per model
            AddSectionLabel(this.pnlModelDetails, "Options", 10, yPos, 480);
            yPos += 25;
            
            this.chkAutoDetectImages.Location = new Point(120, yPos);
            this.chkAutoDetectImages.Text = "Auto-detect clipboard images";
            this.chkAutoDetectImages.Size = new Size(300, 23);
            this.pnlModelDetails.Controls.Add(this.chkAutoDetectImages);
            yPos += 30;
            
            this.chkCombineScreenshot.Location = new Point(120, yPos);
            this.chkCombineScreenshot.Text = "Use clipboard as vision prompt";
            this.chkCombineScreenshot.Size = new Size(300, 23);
            this.pnlModelDetails.Controls.Add(this.chkCombineScreenshot);
            yPos += 40;
            
            this.btnTestModel.Location = new Point(120, yPos);
            this.btnTestModel.Size = new Size(120, 32);
            this.btnTestModel.Text = "Test Model";
            this.btnTestModel.Click += BtnTestModel_Click;
            this.pnlModelDetails.Controls.Add(this.btnTestModel);

            // Options Tab
            this.tabOptions.Text = "Options";
            this.tabOptions.UseVisualStyleBackColor = true;
            this.tabOptions.AutoScroll = true;
            
            int optYPos = 20;
            
            AddSectionLabel(this.tabOptions, "Global Typing Settings", 20, optYPos, 500);
            optYPos += 35;
            
            AddLabel(this.tabOptions, "Typing Delay (ms):", 20, optYPos, 150);
            this.nudTypingDelay.Location = new Point(180, optYPos);
            this.nudTypingDelay.Size = new Size(100, 23);
            this.nudTypingDelay.Minimum = 0;
            this.nudTypingDelay.Maximum = 500;
            this.nudTypingDelay.Value = 50;
            this.tabOptions.Controls.Add(this.nudTypingDelay);
            
            System.Windows.Forms.Label lblDelayHelp = new System.Windows.Forms.Label();
            lblDelayHelp.Text = "Base delay between characters";
            lblDelayHelp.Location = new Point(290, optYPos + 3);
            lblDelayHelp.Size = new Size(300, 20);
            lblDelayHelp.ForeColor = Color.Gray;
            this.tabOptions.Controls.Add(lblDelayHelp);
            optYPos += 35;
            
            AddLabel(this.tabOptions, "Typing Variation (ms):", 20, optYPos, 150);
            this.nudTypingVariation.Location = new Point(180, optYPos);
            this.nudTypingVariation.Size = new Size(100, 23);
            this.nudTypingVariation.Minimum = 0;
            this.nudTypingVariation.Maximum = 200;
            this.nudTypingVariation.Value = 20;
            this.tabOptions.Controls.Add(this.nudTypingVariation);
            
            System.Windows.Forms.Label lblVariationHelp = new System.Windows.Forms.Label();
            lblVariationHelp.Text = "Random variation for natural typing";
            lblVariationHelp.Location = new Point(290, optYPos + 3);
            lblVariationHelp.Size = new Size(300, 20);
            lblVariationHelp.ForeColor = Color.Gray;
            this.tabOptions.Controls.Add(lblVariationHelp);
            optYPos += 50;
            
            System.Windows.Forms.Label lblExample = new System.Windows.Forms.Label();
            lblExample.Text = "Example: Delay=50, Variation=20 means 30-70ms between characters";
            lblExample.Location = new Point(180, optYPos);
            lblExample.Size = new Size(450, 40);
            lblExample.ForeColor = Color.DarkBlue;
            lblExample.Font = new Font(lblExample.Font, FontStyle.Italic);
            this.tabOptions.Controls.Add(lblExample);
            
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
            
            // Bottom Panel
            System.Windows.Forms.Panel bottomPanel = new System.Windows.Forms.Panel();
            bottomPanel.Dock = DockStyle.Bottom;
            bottomPanel.Height = 80;
            this.Controls.Add(bottomPanel);
            
            this.btnSave.Location = new Point(520, 15);
            this.btnSave.Size = new Size(120, 35);
            this.btnSave.Text = "Save";
            this.btnSave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.btnSave.Click += BtnSave_Click;
            bottomPanel.Controls.Add(this.btnSave);
            
            this.btnCancel.Location = new Point(650, 15);
            this.btnCancel.Size = new Size(120, 35);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += BtnCancel_Click;
            bottomPanel.Controls.Add(this.btnCancel);
            
            this.lblStatus.Location = new Point(10, 15);
            this.lblStatus.Size = new Size(500, 50);
            this.lblStatus.AutoSize = false;
            bottomPanel.Controls.Add(this.lblStatus);

            ((System.ComponentModel.ISupportInitialize)(this.nudTypingDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTypingVariation)).EndInit();
            this.ResumeLayout(false);
        }

        private void AddLabel(Control parent, string text, int x, int y, int width = 100)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Text = text;
            label.Location = new Point(x, y + 3);
            label.Size = new Size(width, 20);
            label.TextAlign = ContentAlignment.MiddleRight;
            label.AutoSize = false;
            parent.Controls.Add(label);
        }

        private void AddSectionLabel(Control parent, string text, int x, int y, int width)
        {
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(width, 25);
            label.Font = new Font(label.Font, FontStyle.Bold);
            label.AutoSize = false;
            parent.Controls.Add(label);
        }
    }
}
