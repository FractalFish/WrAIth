namespace BLLMT
{
    public partial class SettingsForm : Form
    {
        private readonly AppSettings _settings;
        private ModelConfig? _selectedModel = null;
        private Dictionary<string, string> _originalApiKeys = new Dictionary<string, string>();
        private bool _isUpdatingList = false; // Prevent recursion

        public SettingsForm(AppSettings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load models into list
            RefreshModelsList();
            
            // Load global options only (no system prompt)
            nudTypingDelay.Value = _settings.TypingDelayMs;
            nudTypingVariation.Value = _settings.TypingVariationMs;
        }

        private void RefreshModelsList()
        {
            _isUpdatingList = true; // Prevent event from triggering save
            try
            {
                int selectedIndex = lstModels.SelectedIndex;
                lstModels.Items.Clear();
                
                // Populate chain target dropdown
                cmbChainToModel.Items.Clear();
                cmbChainToModel.Items.Add("(None)");
                
                foreach (var model in _settings.Models)
                {
                    string displayName = model.Name;
                    if (model.IsDefault)
                        displayName += " [DEFAULT]";
                    if (model.SupportsVision)
                        displayName += " ??";
                    
                    lstModels.Items.Add(displayName);
                    cmbChainToModel.Items.Add(model.Name);
                    
                    // Store original API key for this model
                    if (!_originalApiKeys.ContainsKey(model.Id))
                        _originalApiKeys[model.Id] = model.ApiKey;
                }
                
                // Restore selection
                if (selectedIndex >= 0 && selectedIndex < lstModels.Items.Count)
                    lstModels.SelectedIndex = selectedIndex;
                else if (lstModels.Items.Count > 0)
                    lstModels.SelectedIndex = 0;
            }
            finally
            {
                _isUpdatingList = false;
            }
        }

        private void LstModels_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_isUpdatingList) return; // Skip if we're updating the list
            
            try
            {
                if (lstModels.SelectedIndex >= 0 && lstModels.SelectedIndex < _settings.Models.Count)
                {
                    // Save current model if any (but don't refresh list yet)
                    if (_selectedModel != null)
                    {
                        SaveCurrentModelWithoutRefresh();
                    }
                    
                    // Load selected model
                    _selectedModel = _settings.Models[lstModels.SelectedIndex];
                    LoadModelToForm(_selectedModel);
                    pnlModelDetails.Enabled = true;
                }
                else
                {
                    _selectedModel = null;
                    pnlModelDetails.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Error loading model: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[SettingsForm] Error in LstModels_SelectedIndexChanged: {ex}");
            }
        }

        private void LoadModelToForm(ModelConfig model)
        {
            try
            {
                txtModelName.Text = model.Name ?? string.Empty;
                cmbProvider.Text = model.Provider ?? "OpenAI";
                txtApiKey.Text = model.ApiKey ?? string.Empty;
                txtModel.Text = model.Model ?? string.Empty;
                txtEndpoint.Text = model.Endpoint ?? string.Empty;
                chkSupportsVision.Checked = model.SupportsVision;
                txtSystemPrompt.Text = model.SystemPrompt ?? string.Empty;
                
                // Load per-model hotkeys
                txtTriggerHotkey.SetHotkeyString(model.TriggerHotkey ?? string.Empty);
                txtScreenshotStartHotkey.SetHotkeyString(model.ScreenshotStartHotkey ?? string.Empty);
                txtScreenshotEndHotkey.SetHotkeyString(model.ScreenshotEndHotkey ?? string.Empty);
                txtAnalyzeScreenshotHotkey.SetHotkeyString(model.AnalyzeScreenshotHotkey ?? string.Empty);
                txtAppendVisionHotkey.SetHotkeyString(model.AppendVisionHotkey ?? string.Empty);
                txtOutputHotkey.SetHotkeyString(model.OutputHotkey ?? string.Empty);
                txtAbortHotkey.SetHotkeyString(model.AbortHotkey ?? string.Empty);
                
                // Load model chaining
                if (string.IsNullOrEmpty(model.ChainToModelId))
                {
                    cmbChainToModel.SelectedIndex = 0; // (None)
                }
                else
                {
                    var targetModel = _settings.GetModelById(model.ChainToModelId);
                    if (targetModel != null)
                    {
                        int index = cmbChainToModel.Items.IndexOf(targetModel.Name);
                        cmbChainToModel.SelectedIndex = index >= 0 ? index : 0;
                    }
                    else
                    {
                        cmbChainToModel.SelectedIndex = 0;
                    }
                }
                
                cmbChainMode.Text = model.ChainMode switch
                {
                    "append" => "Append",
                    "prepend" => "Prepend",
                    "replace" => "Replace",
                    _ => "None"
                };
                
                // Load per-model options
                chkAutoDetectImages.Checked = model.AutoDetectClipboardImages;
                chkCombineScreenshot.Checked = model.CombineScreenshotWithClipboard;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Error loading model details: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[SettingsForm] Error in LoadModelToForm: {ex}");
            }
        }

        private void SaveCurrentModelWithoutRefresh()
        {
            try
            {
                if (_selectedModel == null) return;
                
                _selectedModel.Name = txtModelName.Text;
                _selectedModel.Provider = cmbProvider.Text;
                
                // Handle API key - check for masking
                string newApiKey = txtApiKey.Text;
                if (newApiKey.Contains("*") && _originalApiKeys.ContainsKey(_selectedModel.Id))
                {
                    _selectedModel.ApiKey = _originalApiKeys[_selectedModel.Id];
                }
                else
                {
                    _selectedModel.ApiKey = newApiKey;
                    _originalApiKeys[_selectedModel.Id] = newApiKey;
                }
                
                _selectedModel.Model = txtModel.Text;
                _selectedModel.Endpoint = txtEndpoint.Text;
                _selectedModel.SupportsVision = chkSupportsVision.Checked;
                _selectedModel.SystemPrompt = txtSystemPrompt.Text;
                
                // Save per-model hotkeys
                _selectedModel.TriggerHotkey = txtTriggerHotkey.GetHotkeyString();
                _selectedModel.ScreenshotStartHotkey = txtScreenshotStartHotkey.GetHotkeyString();
                _selectedModel.ScreenshotEndHotkey = txtScreenshotEndHotkey.GetHotkeyString();
                _selectedModel.AnalyzeScreenshotHotkey = txtAnalyzeScreenshotHotkey.GetHotkeyString();
                _selectedModel.AppendVisionHotkey = txtAppendVisionHotkey.GetHotkeyString();
                _selectedModel.OutputHotkey = txtOutputHotkey.GetHotkeyString();
                _selectedModel.AbortHotkey = txtAbortHotkey.GetHotkeyString();
                
                // Save model chaining
                if (cmbChainToModel.SelectedIndex > 0) // Not "(None)"
                {
                    string targetName = cmbChainToModel.SelectedItem?.ToString() ?? string.Empty;
                    var targetModel = _settings.Models.FirstOrDefault(m => m.Name == targetName);
                    _selectedModel.ChainToModelId = targetModel?.Id ?? string.Empty;
                }
                else
                {
                    _selectedModel.ChainToModelId = string.Empty;
                }
                
                _selectedModel.ChainMode = cmbChainMode.Text.ToLowerInvariant();
                
                // Save per-model options
                _selectedModel.AutoDetectClipboardImages = chkAutoDetectImages.Checked;
                _selectedModel.CombineScreenshotWithClipboard = chkCombineScreenshot.Checked;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Error saving model: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"[SettingsForm] Error in SaveCurrentModel: {ex}");
            }
        }

        private void SaveCurrentModel()
        {
            SaveCurrentModelWithoutRefresh();
            RefreshModelsList(); // Now refresh the list
        }

        private void BtnAddModel_Click(object? sender, EventArgs e)
        {
            var newModel = new ModelConfig
            {
                Name = $"New Model {_settings.Models.Count + 1}",
                Provider = "OpenAI",
                Model = "gpt-4o-mini",
                Endpoint = "https://api.openai.com/v1/chat/completions",
                SupportsVision = false,
                IsDefault = false
            };
            
            _settings.Models.Add(newModel);
            RefreshModelsList();
            lstModels.SelectedIndex = _settings.Models.Count - 1;
        }

        private void BtnRemoveModel_Click(object? sender, EventArgs e)
        {
            if (lstModels.SelectedIndex >= 0 && _settings.Models.Count > 1)
            {
                var result = MessageBox.Show(
                    "Are you sure you want to remove this model?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                
                if (result == DialogResult.Yes)
                {
                    int indexToRemove = lstModels.SelectedIndex;
                    _settings.Models.RemoveAt(indexToRemove);
                    _selectedModel = null;
                    RefreshModelsList();
                }
            }
            else if (_settings.Models.Count <= 1)
            {
                MessageBox.Show("Cannot remove the last model.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetDefault_Click(object? sender, EventArgs e)
        {
            if (_selectedModel != null)
            {
                // Remove default from all models
                foreach (var model in _settings.Models)
                    model.IsDefault = false;
                
                // Set selected as default
                _selectedModel.IsDefault = true;
                RefreshModelsList();
                
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = $"'{_selectedModel.Name}' set as default model.";
            }
        }

        private void CmbProvider_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string provider = cmbProvider.Text;
            
            if (provider == "OpenAI")
            {
                txtEndpoint.Text = "https://api.openai.com/v1/chat/completions";
            }
            else if (provider == "Anthropic")
            {
                txtEndpoint.Text = "https://api.anthropic.com/v1/messages";
            }
            else if (provider == "Groq")
            {
                txtEndpoint.Text = "https://api.groq.com/openai/v1/chat/completions";
            }
        }

        private async void BtnTestModel_Click(object? sender, EventArgs e)
        {
            if (_selectedModel == null) return;
            
            // Save current changes first
            SaveCurrentModel();
            
            lblStatus.ForeColor = Color.Blue;
            lblStatus.Text = "Testing model connection...";
            btnTestModel.Enabled = false;

            try
            {
                var testSettings = new AppSettings
                {
                    Models = new List<ModelConfig> { _selectedModel }
                };
                testSettings.Models[0].IsDefault = true;

                var llmService = new LLMService(testSettings);
                
                if (_selectedModel.SupportsVision)
                {
                    // Test with a simple image
                    string testImage = "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8DwHwAFBQIAX8jx0gAAAABJRU5ErkJggg==";
                    var response = await llmService.GetResponseAsync("What color is this image?", testImage);
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = $"Vision test success! Response: {response.Substring(0, Math.Min(80, response.Length))}...";
                }
                else
                {
                    var response = await llmService.GetResponseAsync("Say 'Hello!' if you can read this.");
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = $"Test success! Response: {response.Substring(0, Math.Min(80, response.Length))}...";
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Test failed: {ex.Message}";
            }
            finally
            {
                btnTestModel.Enabled = true;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            try
            {
                // Save current model if editing
                if (_selectedModel != null)
                {
                    SaveCurrentModelWithoutRefresh();
                }
                
                // Save global options only (no system prompt)
                _settings.TypingDelayMs = (int)nudTypingDelay.Value;
                _settings.TypingVariationMs = (int)nudTypingVariation.Value;

                // Ensure at least one default model
                if (!_settings.Models.Any(m => m.IsDefault))
                {
                    if (_settings.Models.Count > 0)
                        _settings.Models[0].IsDefault = true;
                }

                _settings.Save();

                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Settings saved! Restart application for changes to take effect.";

                Task.Delay(2000).ContinueWith(_ => 
                {
                    if (!IsDisposed)
                    {
                        this.Invoke(() => this.Close());
                    }
                });
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Error saving: {ex.Message}";
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
        }
    }
}
