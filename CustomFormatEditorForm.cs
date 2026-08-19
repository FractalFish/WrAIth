using System.Windows.Forms;

namespace Wraith
{
    public partial class CustomFormatEditorForm : Form
    {
        public CustomApiFormat Format { get; private set; }

        private TextBox txtHeaders;
        private TextBox txtRequestBody;
        private TextBox txtVisionBody;
        private TextBox txtResponsePath;
        private Button btnSave;
        private Button btnCancel;
        private Label lblStatus;

        public CustomFormatEditorForm(CustomApiFormat? existingFormat)
        {
            Format = existingFormat ?? new CustomApiFormat();
            InitializeComponent();
            LoadFormat();
        }

        private void InitializeComponent()
        {
            this.Text = "Custom API Format Editor";
            this.Size = new Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Consolas", 9F); // Monospace for JSON

            int yPos = 10;

            // Headers
            AddLabel("Headers (one per line, format: HeaderName: HeaderValue)", 10, yPos);
            yPos += 20;
            AddLabel("Use {API_KEY} placeholder for the API key", 10, yPos);
            yPos += 25;
            
            txtHeaders = new TextBox();
            txtHeaders.Location = new Point(10, yPos);
            txtHeaders.Size = new Size(760, 80);
            txtHeaders.Multiline = true;
            txtHeaders.ScrollBars = ScrollBars.Vertical;
            txtHeaders.Font = new Font("Consolas", 9F);
            this.Controls.Add(txtHeaders);
            yPos += 90;

            // Request Body Template
            AddLabel("Request Body Template (JSON)", 10, yPos);
            yPos += 20;
            AddLabel("Placeholders: {API_KEY}, {MODEL_ID}, {SYSTEM_PROMPT}, {USER_MESSAGE}", 10, yPos);
            yPos += 25;
            
            txtRequestBody = new TextBox();
            txtRequestBody.Location = new Point(10, yPos);
            txtRequestBody.Size = new Size(760, 150);
            txtRequestBody.Multiline = true;
            txtRequestBody.ScrollBars = ScrollBars.Both;
            txtRequestBody.Font = new Font("Consolas", 9F);
            txtRequestBody.WordWrap = false;
            this.Controls.Add(txtRequestBody);
            yPos += 160;

            // Vision Request Body Template
            AddLabel("Vision Request Body Template (JSON) - adds {IMAGE_BASE64}", 10, yPos);
            yPos += 25;
            
            txtVisionBody = new TextBox();
            txtVisionBody.Location = new Point(10, yPos);
            txtVisionBody.Size = new Size(760, 150);
            txtVisionBody.Multiline = true;
            txtVisionBody.ScrollBars = ScrollBars.Both;
            txtVisionBody.Font = new Font("Consolas", 9F);
            txtVisionBody.WordWrap = false;
            this.Controls.Add(txtVisionBody);
            yPos += 160;

            // Response Text Path
            AddLabel("Response Text Path (JSON path, e.g., 'choices[0].message.content')", 10, yPos);
            yPos += 25;
            
            txtResponsePath = new TextBox();
            txtResponsePath.Location = new Point(10, yPos);
            txtResponsePath.Size = new Size(760, 23);
            txtResponsePath.Font = new Font("Consolas", 9F);
            this.Controls.Add(txtResponsePath);
            yPos += 40;

            // Buttons
            btnSave = new Button();
            btnSave.Location = new Point(580, yPos);
            btnSave.Size = new Size(90, 30);
            btnSave.Text = "Save";
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button();
            btnCancel.Location = new Point(680, yPos);
            btnCancel.Size = new Size(90, 30);
            btnCancel.Text = "Cancel";
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            // Status label
            lblStatus = new Label();
            lblStatus.Location = new Point(10, yPos);
            lblStatus.Size = new Size(560, 30);
            lblStatus.AutoSize = false;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblStatus);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label();
            label.Text = text;
            label.Location = new Point(x, y);
            label.Size = new Size(760, 20);
            label.Font = new Font("Segoe UI", 9F);
            label.AutoSize = true;
            this.Controls.Add(label);
        }

        private void LoadFormat()
        {
            txtHeaders.Text = string.Join(Environment.NewLine, Format.Headers);
            txtRequestBody.Text = Format.RequestBodyTemplate;
            txtVisionBody.Text = Format.VisionRequestBodyTemplate;
            txtResponsePath.Text = Format.ResponseTextPath;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Validate JSON
                var testBody = txtRequestBody.Text
                    .Replace("{API_KEY}", "test")
                    .Replace("{MODEL_ID}", "test")
                    .Replace("{SYSTEM_PROMPT}", "test")
                    .Replace("{USER_MESSAGE}", "test");
                
                System.Text.Json.JsonDocument.Parse(testBody); // Will throw if invalid

                // Save
                Format.Headers = txtHeaders.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
                Format.RequestBodyTemplate = txtRequestBody.Text;
                Format.VisionRequestBodyTemplate = txtVisionBody.Text;
                Format.ResponseTextPath = txtResponsePath.Text;

                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Format saved!";

                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Invalid JSON: {ex.Message}";
            }
        }
    }
}
