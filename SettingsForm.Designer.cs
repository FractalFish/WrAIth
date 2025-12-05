namespace BLLMT
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabModels;
        private System.Windows.Forms.TabPage tabOptions;
        
        // Models tab
        private System.Windows.Forms.ListBox lstModels;
        private System.Windows.Forms.Button btnAddModel;
        private System.Windows.Forms.Button btnRemoveModel;
        private System.Windows.Forms.Button btnSetDefault;
        private System.Windows.Forms.Panel pnlModelDetails;
        private System.Windows.Forms.TextBox txtModelName;
        private System.Windows.Forms.ComboBox cmbProvider;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.TextBox txtEndpoint;
        private System.Windows.Forms.CheckBox chkSupportsVision;
        private System.Windows.Forms.Button btnTestModel;
        private System.Windows.Forms.TextBox txtSystemPrompt; // Per-model system prompt
        
        // Hotkeys
        private HotkeyTextBox txtTriggerHotkey;
        private HotkeyTextBox txtOutputHotkey;
        private HotkeyTextBox txtAbortHotkey;
        private HotkeyTextBox txtScreenshotStartHotkey;
        private HotkeyTextBox txtScreenshotEndHotkey;
        private HotkeyTextBox txtAnalyzeScreenshotHotkey; // New: separate analyze action
        private HotkeyTextBox txtAppendVisionHotkey;
        
        // Model chaining
        private System.Windows.Forms.ComboBox cmbChainToModel;
        private System.Windows.Forms.ComboBox cmbChainMode;
        
        // Options tab
        private System.Windows.Forms.Panel pnlOptions;
        private System.Windows.Forms.NumericUpDown nudTypingDelay;
        private System.Windows.Forms.NumericUpDown nudTypingVariation;
        private System.Windows.Forms.CheckBox chkAutoDetectImages;
        private System.Windows.Forms.CheckBox chkCombineScreenshot;
        
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
            
            // Models tab controls
            this.lstModels = new System.Windows.Forms.ListBox();
            this.btnAddModel = new System.Windows.Forms.Button();
            this.btnRemoveModel = new System.Windows.Forms.Button();
            this.btnSetDefault = new System.Windows.Forms.Button();
            this.pnlModelDetails = new System.Windows.Forms.Panel();
            this.txtModelName = new System.Windows.Forms.TextBox();
            this.cmbProvider = new System.Windows.Forms.ComboBox();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.txtEndpoint = new System.Windows.Forms.TextBox();
            this.chkSupportsVision = new System.Windows.Forms.CheckBox();
            this.btnTestModel = new System.Windows.Forms.Button();
            this.txtSystemPrompt = new System.Windows.Forms.TextBox(); // Per-model system prompt
            
            // Hotkeys
            this.txtTriggerHotkey = new HotkeyTextBox();
            this.txtOutputHotkey = new HotkeyTextBox();
            this.txtAbortHotkey = new HotkeyTextBox();
            this.txtScreenshotStartHotkey = new HotkeyTextBox();
            this.txtScreenshotEndHotkey = new HotkeyTextBox();
            this.txtAnalyzeScreenshotHotkey = new HotkeyTextBox();
            this.txtAppendVisionHotkey = new HotkeyTextBox();
            
            // Model chaining
            this.cmbChainToModel = new System.Windows.Forms.ComboBox();
            this.cmbChainMode = new System.Windows.Forms.ComboBox();
            
            // Options tab
            this.pnlOptions = new System.Windows.Forms.Panel();
            this.nudTypingDelay = new System.Windows.Forms.NumericUpDown();
            this.nudTypingVariation = new System.Windows.Forms.NumericUpDown();
            this.chkAutoDetectImages = new System.Windows.Forms.CheckBox();
            this.chkCombineScreenshot = new System.Windows.Forms.CheckBox();
            
            // Bottom buttons
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            
            ((System.ComponentModel.ISupportInitialize)(this.nudTypingDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTypingVariation)).BeginInit();
            this.SuspendLayout();

            // Form
            this.ClientSize = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(800, 600);
            this.MaximizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "BLLMT Settings";
            this.Font = new Font("Segoe UI", 9F);

            // TabControl
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Location = new Point(0, 0);
            this.tabControl.TabIndex = 0;
            this.tabControl.Controls.Add(this.tabModels);
            this.tabControl.Controls.Add(this.tabOptions);
            this.Controls.Add(this.tabControl);

            // Models Tab
            this.tabModels.Text = "Models";
            this.tabModels.UseVisualStyleBackColor = true;
            this.tabModels.AutoScroll = true;
            
            // Models ListBox
            this.lstModels.Location = new Point(10, 10);
            this.lstModels.Size = new Size(250, 420);
            this.lstModels.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            this.lstModels.SelectedIndexChanged += LstModels_SelectedIndexChanged;
            this.tabModels.Controls.Add(this.lstModels);
            
            // Model buttons
            this.btnAddModel.Location = new Point(10, 440);
            this.btnAddModel.Size = new Size(80, 30);
            this.btnAddModel.Text = "Add";
            this.btnAddModel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnAddModel.Click += BtnAddModel_Click;
            this.tabModels.Controls.Add(this.btnAddModel);
            
            this.btnRemoveModel.Location = new Point(95, 440);
            this.btnRemoveModel.Size = new Size(80, 30);
            this.btnRemoveModel.Text = "Remove";
            this.btnRemoveModel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnRemoveModel.Click += BtnRemoveModel_Click;
            this.tabModels.Controls.Add(this.btnRemoveModel);
            
            this.btnSetDefault.Location = new Point(180, 440);
            this.btnSetDefault.Size = new Size(80, 30);
            this.btnSetDefault.Text = "Set Default";
            this.btnSetDefault.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnSetDefault.Click += BtnSetDefault_Click;
            this.tabModels.Controls.Add(this.btnSetDefault);
            
            // Model Details Panel
            this.pnlModelDetails.Location = new Point(270, 10);
            this.pnlModelDetails.Size = new Size(510, 470);
            this.pnlModelDetails.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlModelDetails.AutoScroll = true;
            this.pnlModelDetails.BorderStyle = BorderStyle.FixedSingle;
            this.tabModels.Controls.Add(this.pnlModelDetails);
            
            int yPos = 10;
            
            AddLabel(this.pnlModelDetails, "Model Name:", 10, yPos);
            this.txtModelName.Location = new Point(120, yPos);
            this.txtModelName.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtModelName);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "Provider:", 10, yPos);
            this.cmbProvider.Location = new Point(120, yPos);
            this.cmbProvider.Size = new Size(370, 23);
            this.cmbProvider.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbProvider.Items.AddRange(new object[] { "OpenAI", "Anthropic", "Groq", "Custom" });
            this.cmbProvider.SelectedIndexChanged += CmbProvider_SelectedIndexChanged;
            this.pnlModelDetails.Controls.Add(this.cmbProvider);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "API Key:", 10, yPos);
            this.txtApiKey.Location = new Point(120, yPos);
            this.txtApiKey.Size = new Size(370, 23);
            this.txtApiKey.UseSystemPasswordChar = true;
            this.pnlModelDetails.Controls.Add(this.txtApiKey);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "Model:", 10, yPos);
            this.txtModel.Location = new Point(120, yPos);
            this.txtModel.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtModel);
            yPos += 35;
            
            AddLabel(this.pnlModelDetails, "Endpoint:", 10, yPos);
            this.txtEndpoint.Location = new Point(120, yPos);
            this.txtEndpoint.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtEndpoint);
            yPos += 35;
            
            this.chkSupportsVision.Location = new Point(120, yPos);
            this.chkSupportsVision.Text = "Supports Vision (Images)";
            this.chkSupportsVision.Size = new Size(200, 23);
            this.pnlModelDetails.Controls.Add(this.chkSupportsVision);
            yPos += 40;
            
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
            
            // Hotkeys section per model
            AddSectionLabel(this.pnlModelDetails, "Hotkeys (leave empty to disable)", 10, yPos, 480);
            yPos += 25;
            
            AddLabel(this.pnlModelDetails, "Trigger:", 10, yPos);
            this.txtTriggerHotkey.Location = new Point(120, yPos);
            this.txtTriggerHotkey.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtTriggerHotkey);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Screenshot Start:", 10, yPos);
            this.txtScreenshotStartHotkey.Location = new Point(120, yPos);
            this.txtScreenshotStartHotkey.Size = new Size(370, 23);
            this.txtScreenshotStartHotkey.PlaceholderText = "Empty = disabled";
            this.pnlModelDetails.Controls.Add(this.txtScreenshotStartHotkey);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Screenshot End:", 10, yPos);
            this.txtScreenshotEndHotkey.Location = new Point(120, yPos);
            this.txtScreenshotEndHotkey.Size = new Size(370, 23);
            this.txtScreenshotEndHotkey.PlaceholderText = "Empty = disabled";
            this.pnlModelDetails.Controls.Add(this.txtScreenshotEndHotkey);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Analyze Screenshot:", 10, yPos);
            this.txtAnalyzeScreenshotHotkey.Location = new Point(120, yPos);
            this.txtAnalyzeScreenshotHotkey.Size = new Size(370, 23);
            this.txtAnalyzeScreenshotHotkey.PlaceholderText = "Empty = disabled";
            this.pnlModelDetails.Controls.Add(this.txtAnalyzeScreenshotHotkey);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Append Vision:", 10, yPos);
            this.txtAppendVisionHotkey.Location = new Point(120, yPos);
            this.txtAppendVisionHotkey.Size = new Size(370, 23);
            this.txtAppendVisionHotkey.PlaceholderText = "Empty = disabled";
            this.pnlModelDetails.Controls.Add(this.txtAppendVisionHotkey);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Output:", 10, yPos);
            this.txtOutputHotkey.Location = new Point(120, yPos);
            this.txtOutputHotkey.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtOutputHotkey);
            yPos += 30;
            
            AddLabel(this.pnlModelDetails, "Abort:", 10, yPos);
            this.txtAbortHotkey.Location = new Point(120, yPos);
            this.txtAbortHotkey.Size = new Size(370, 23);
            this.pnlModelDetails.Controls.Add(this.txtAbortHotkey);
            yPos += 40;
            
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

            // Global Settings Tab (Options)
            this.tabOptions.Text = "Global Settings";
            this.tabOptions.UseVisualStyleBackColor = true;
            this.tabOptions.AutoScroll = true;
            
            this.pnlOptions.Dock = DockStyle.Fill;
            this.pnlOptions.AutoScroll = true;
            this.pnlOptions.Padding = new Padding(20);
            this.tabOptions.Controls.Add(this.pnlOptions);
            
            yPos = 20;
            int labelWidth = 150;
            
            AddLabel(this.pnlOptions, "Typing Delay (ms):", 20, yPos, labelWidth);
            this.nudTypingDelay.Location = new Point(180, yPos);
            this.nudTypingDelay.Size = new Size(100, 23);
            this.nudTypingDelay.Minimum = 10;
            this.nudTypingDelay.Maximum = 500;
            this.pnlOptions.Controls.Add(this.nudTypingDelay);
            yPos += 35;
            
            AddLabel(this.pnlOptions, "Typing Variation (ms):", 20, yPos, labelWidth);
            this.nudTypingVariation.Location = new Point(180, yPos);
            this.nudTypingVariation.Size = new Size(100, 23);
            this.nudTypingVariation.Minimum = 0;
            this.nudTypingVariation.Maximum = 200;
            this.pnlOptions.Controls.Add(this.nudTypingVariation);

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
