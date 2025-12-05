namespace BLLMT
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private NotifyIcon notifyIcon;
        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem settingsMenuItem;
        private ToolStripMenuItem exitMenuItem;
        private ToolStripSeparator separator;
        private ToolStripMenuItem statusMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _hotkeyManager?.Dispose();
                _keyboardHook?.Dispose();
                notifyIcon?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            // NotifyIcon
            this.notifyIcon = new NotifyIcon(this.components);
            this.notifyIcon.Text = "BLLMT - Background LLM Assistant";
            this.notifyIcon.Visible = true;
            this.notifyIcon.DoubleClick += (s, e) => NotifyIcon_DoubleClick(s, e);
            
            // Use a simple icon (you can replace with a custom .ico file)
            this.notifyIcon.Icon = SystemIcons.Application;

            // Context Menu
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            
            this.statusMenuItem = new ToolStripMenuItem();
            this.statusMenuItem.Text = "Ready";
            this.statusMenuItem.Enabled = false;
            
            this.separator = new ToolStripSeparator();
            
            this.settingsMenuItem = new ToolStripMenuItem();
            this.settingsMenuItem.Text = "Settings";
            this.settingsMenuItem.Click += (s, e) => SettingsMenuItem_Click(s, e);
            
            this.exitMenuItem = new ToolStripMenuItem();
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += (s, e) => ExitMenuItem_Click(s, e);

            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] {
                this.statusMenuItem,
                this.separator,
                this.settingsMenuItem,
                this.exitMenuItem
            });

            this.notifyIcon.ContextMenuStrip = this.contextMenuStrip;

            // Form
            this.ClientSize = new Size(0, 0);
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Opacity = 0;
            this.Text = "BLLMT";
        }
    }
}
