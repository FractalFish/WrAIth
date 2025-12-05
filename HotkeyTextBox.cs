namespace BLLMT
{
    public class HotkeyTextBox : TextBox
    {
        private bool _isCapturing = false;
        private Keys _modifiers = Keys.None;
        private Keys _key = Keys.None;

        public HotkeyTextBox()
        {
            this.ReadOnly = true;
            this.BackColor = SystemColors.Window;
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);
            StartCapture();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!_isCapturing)
            {
                base.OnKeyDown(e);
                return;
            }

            // Suppress the key event
            e.SuppressKeyPress = true;
            e.Handled = true;

            // Capture current modifiers from the event
            _modifiers = Keys.None;
            if (e.Control) _modifiers |= Keys.Control;
            if (e.Shift) _modifiers |= Keys.Shift;
            if (e.Alt) _modifiers |= Keys.Alt;

            // Get the key code
            Keys keyCode = e.KeyCode;

            // Check if this is ONLY a modifier key (not a combo with modifiers)
            if (IsModifierKey(keyCode))
            {
                // Just a modifier being pressed, show live preview
                _key = Keys.None;
                UpdateDisplay(GetLivePreview() + " + ...");
                return;
            }

            // This is a regular key - capture it as the final key
            _key = keyCode;
            UpdateDisplay(GetHotkeyString());
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (!_isCapturing)
            {
                base.OnKeyUp(e);
                return;
            }

            e.SuppressKeyPress = true;
            e.Handled = true;

            // Update live preview as modifiers are released
            if (_key == Keys.None)
            {
                Keys currentModifiers = Keys.None;
                if ((Control.ModifierKeys & Keys.Control) != Keys.None) currentModifiers |= Keys.Control;
                if ((Control.ModifierKeys & Keys.Shift) != Keys.None) currentModifiers |= Keys.Shift;
                if ((Control.ModifierKeys & Keys.Alt) != Keys.None) currentModifiers |= Keys.Alt;

                if (currentModifiers != Keys.None)
                {
                    _modifiers = currentModifiers;
                    UpdateDisplay(GetLivePreview() + " + ...");
                }
                else
                {
                    UpdateDisplay("Press hotkey...");
                }
            }
            else
            {
                // Finish capture when all keys are released
                if (!IsAnyKeyPressed())
                {
                    StopCapture();
                }
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (_isCapturing)
            {
                StopCapture();
            }
        }

        private void StartCapture()
        {
            _isCapturing = true;
            _modifiers = Keys.None;
            _key = Keys.None;
            this.BackColor = System.Drawing.Color.LightYellow;
            this.Text = "Press hotkey...";
            this.Focus();
        }

        private void StopCapture()
        {
            _isCapturing = false;
            this.BackColor = SystemColors.Window;
            
            // If no valid key was captured, restore previous value or clear
            if (_key == Keys.None)
            {
                // Restore to empty or previous value
                this.Text = string.Empty;
            }
        }

        private void UpdateDisplay(string text)
        {
            this.Text = text;
        }

        private string GetLivePreview()
        {
            List<string> parts = new List<string>();

            if ((_modifiers & Keys.Control) != Keys.None)
                parts.Add("Ctrl");
            if ((_modifiers & Keys.Shift) != Keys.None)
                parts.Add("Shift");
            if ((_modifiers & Keys.Alt) != Keys.None)
                parts.Add("Alt");

            return parts.Count > 0 ? string.Join(" + ", parts) : "";
        }

        private bool IsModifierKey(Keys key)
        {
            // Only these keys are pure modifiers
            return key == Keys.ControlKey || key == Keys.LControlKey || key == Keys.RControlKey ||
                   key == Keys.ShiftKey || key == Keys.LShiftKey || key == Keys.RShiftKey ||
                   key == Keys.Menu || key == Keys.LMenu || key == Keys.RMenu ||
                   key == Keys.LWin || key == Keys.RWin;
        }

        private bool IsAnyKeyPressed()
        {
            // Check if any modifier is still pressed
            return (Control.ModifierKeys & (Keys.Control | Keys.Shift | Keys.Alt)) != Keys.None;
        }

        private bool IsSpecialKey(Keys key)
        {
            // Function keys, navigation keys, etc. can be used without modifiers
            return key >= Keys.F1 && key <= Keys.F24 ||
                   key == Keys.Insert || key == Keys.Delete || key == Keys.Home || key == Keys.End ||
                   key == Keys.PageUp || key == Keys.PageDown ||
                   key == Keys.Escape || key == Keys.PrintScreen || key == Keys.Pause;
        }

        public string GetHotkeyString()
        {
            if (_key == Keys.None)
                return string.Empty;

            List<string> parts = new List<string>();

            if ((_modifiers & Keys.Control) != Keys.None)
                parts.Add("Control");
            if ((_modifiers & Keys.Shift) != Keys.None)
                parts.Add("Shift");
            if ((_modifiers & Keys.Alt) != Keys.None)
                parts.Add("Alt");

            parts.Add(_key.ToString());

            return string.Join("+", parts);
        }

        public void SetHotkeyString(string hotkeyString)
        {
            _modifiers = Keys.None;
            _key = Keys.None;

            if (string.IsNullOrWhiteSpace(hotkeyString))
            {
                this.Text = string.Empty;
                return;
            }

            var parts = hotkeyString.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (var part in parts)
            {
                string upperPart = part.ToUpperInvariant();

                if (upperPart == "CONTROL" || upperPart == "CTRL")
                {
                    _modifiers |= Keys.Control;
                }
                else if (upperPart == "SHIFT")
                {
                    _modifiers |= Keys.Shift;
                }
                else if (upperPart == "ALT")
                {
                    _modifiers |= Keys.Alt;
                }
                else
                {
                    if (Enum.TryParse<Keys>(part, true, out Keys key))
                    {
                        _key = key;
                    }
                }
            }

            this.Text = GetHotkeyString();
        }

        public void ClearHotkey()
        {
            _modifiers = Keys.None;
            _key = Keys.None;
            this.Text = string.Empty;
        }
    }
}
