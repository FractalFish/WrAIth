using System.Windows.Forms;
using Wraith.Constants;

namespace Wraith
{
    public partial class SettingsForm : Form
    {
        private readonly AppSettings _settings;
        private ModelConfig? _selectedModel = null;
        private HotkeyMapping? _selectedMapping = null;
        private bool _isEditingMapping = false;
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
            
            // Load hotkey mappings
            LoadHotkeyMappings();
            
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
                cmbProvider.Text = model.Provider ?? ProviderTypes.OpenAI;
                btnEditCustomFormat.Visible = (model.Provider == ProviderTypes.Custom);
                txtApiKey.Text = model.ApiKey ?? string.Empty;
                txtModel.Text = model.Model ?? string.Empty;
                txtEndpoint.Text = model.Endpoint ?? string.Empty;
                txtSystemPrompt.Text = model.SystemPrompt ?? string.Empty;
                
                
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
                _selectedModel.SystemPrompt = txtSystemPrompt.Text;
                
                
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
                Provider = ProviderTypes.OpenAI,
                Model = DefaultModels.OpenAI_GPT4oMini,
                Endpoint = DefaultEndpoints.OpenAI,
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
                    UIStrings.ConfirmDeleteModel,
                    UIStrings.ConfirmDeleteTitle,
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
                MessageBox.Show(UIStrings.ErrorCannotRemoveLastModel, UIStrings.ErrorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            
            // Show/hide Edit Format button
            btnEditCustomFormat.Visible = (provider == ProviderTypes.Custom);
            
            if (provider == ProviderTypes.OpenAI)
            {
                txtEndpoint.Text = DefaultEndpoints.OpenAI;
            }
            else if (provider == ProviderTypes.Anthropic)
            {
                txtEndpoint.Text = DefaultEndpoints.Anthropic;
            }
            else if (provider == ProviderTypes.Groq)
            {
                txtEndpoint.Text = DefaultEndpoints.Groq;
            }
        }

        private void BtnEditCustomFormat_Click(object? sender, EventArgs e)
        {
            if (_selectedModel == null) return;

            // Initialize custom format if null
            if (_selectedModel.CustomApiFormat == null)
            {
                _selectedModel.CustomApiFormat = new CustomApiFormat();
            }

            var editor = new CustomFormatEditorForm(_selectedModel.CustomApiFormat);
            if (editor.ShowDialog() == DialogResult.OK)
            {
                _selectedModel.CustomApiFormat = editor.Format;
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = UIStrings.CustomFormatSaved;
            }
        }

        private async void BtnTestModel_Click(object? sender, EventArgs e)
        {
            if (_selectedModel == null) return;
            
            // Save current changes first
            SaveCurrentModel();
            
            lblStatus.ForeColor = Color.Blue;
            lblStatus.Text = UIStrings.StatusTestingModel;
            btnTestModel.Enabled = false;

            try
            {
                var testSettings = new AppSettings
                {
                    Models = new List<ModelConfig> { _selectedModel }
                };
                testSettings.Models[0].IsDefault = true;

                var llmService = new LLMService(testSettings);
                
                // Always test with text - simple and universal
                var response = await llmService.GetResponseAsync(UIStrings.TestQueryText);
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = string.Format(UIStrings.TestSuccess, response.Substring(0, Math.Min(80, response.Length)));
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = string.Format(UIStrings.ErrorTestFailed, ex.Message);
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
                lblStatus.Text = UIStrings.SettingsSaved;

                Task.Delay(DefaultTimings.SettingsAutoCloseDelayMs).ContinueWith(_ => 
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
                lblStatus.Text = string.Format(UIStrings.ErrorSavingSettings, ex.Message);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        #region Hotkey Management Methods

        private void LoadHotkeyMappings()
        {
            // Populate model dropdown for hotkey edit
            cmbHotkeyModel.Items.Clear();
            cmbHotkeyModel.Items.Add(HotkeyActions.ModelGlobal);
            cmbHotkeyModel.Items.Add(HotkeyActions.ModelCurrent);
            foreach (var model in _settings.Models)
            {
                cmbHotkeyModel.Items.Add(model.Name);
            }

            // Populate action dropdown
            cmbHotkeyAction.Items.Clear();
            foreach (var action in HotkeyActions.All)
            {
                cmbHotkeyAction.Items.Add(HotkeyActions.GetDisplayName(action));
            }

            RefreshHotkeyMappingsList();
        }

        private void RefreshHotkeyMappingsList()
        {
            int selectedIndex = lstHotkeyMappings.SelectedIndex;
            lstHotkeyMappings.Items.Clear();

            var mappings = _settings.HotkeyMappings.OrderBy(m => m.DisplayOrder).ToList();
            foreach (var mapping in mappings)
            {
                lstHotkeyMappings.Items.Add(mapping);
            }

            // Restore selection
            if (selectedIndex >= 0 && selectedIndex < lstHotkeyMappings.Items.Count)
                lstHotkeyMappings.SelectedIndex = selectedIndex;
        }

        private void LstHotkeyMappings_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (lstHotkeyMappings.SelectedIndex >= 0 && 
                lstHotkeyMappings.SelectedIndex < _settings.HotkeyMappings.Count)
            {
                _selectedMapping = _settings.HotkeyMappings[lstHotkeyMappings.SelectedIndex];
                btnEditHotkey.Enabled = true;
                btnRemoveHotkey.Enabled = true;
                btnMoveUp.Enabled = lstHotkeyMappings.SelectedIndex > 0;
                btnMoveDown.Enabled = lstHotkeyMappings.SelectedIndex < lstHotkeyMappings.Items.Count - 1;
            }
            else
            {
                _selectedMapping = null;
                btnEditHotkey.Enabled = false;
                btnRemoveHotkey.Enabled = false;
                btnMoveUp.Enabled = false;
                btnMoveDown.Enabled = false;
            }
        }

        private void LstHotkeyMappings_DrawItem(object? sender, DrawItemEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _settings.HotkeyMappings.Count) return;

            var mapping = _settings.HotkeyMappings[e.Index];
            e.DrawBackground();

            Color textColor = mapping.IsEnabled ? e.ForeColor : Color.Gray;
            using (Brush brush = new SolidBrush(textColor))
            {
                string text = mapping.GetDisplayString(_settings);
                if (!mapping.IsEnabled)
                    text += " [DISABLED]";
                    
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            e.DrawFocusRectangle();
        }

        private void BtnAddHotkey_Click(object? sender, EventArgs e)
        {
            _isEditingMapping = false;
            _selectedMapping = new HotkeyMapping
            {
                DisplayOrder = _settings.HotkeyMappings.Count
            };

            // Set defaults
            cmbHotkeyModel.SelectedIndex = 0; // (Global)
            cmbHotkeyAction.SelectedIndex = 0; // First action
            txtHotkeyEdit.ClearHotkey();
            txtHotkeyDescription.Text = string.Empty;
            chkHotkeyEnabled.Checked = true;

            pnlHotkeyEdit.Visible = true;
            lblStatus.Text = "Add new hotkey mapping";
        }

        private void BtnEditHotkey_Click(object? sender, EventArgs e)
        {
            if (_selectedMapping == null) return;

            _isEditingMapping = true;
            LoadMappingToEditPanel(_selectedMapping);
            pnlHotkeyEdit.Visible = true;
            lblStatus.Text = $"Editing: {_selectedMapping.GetDisplayString(_settings)}";
        }

        private void LoadMappingToEditPanel(HotkeyMapping mapping)
        {
            // Set hotkey
            txtHotkeyEdit.SetHotkeyString(mapping.Hotkey);

            // Set model
            if (mapping.ModelId == HotkeyActions.ModelGlobal)
            {
                cmbHotkeyModel.SelectedIndex = 0;
            }
            else if (mapping.ModelId == HotkeyActions.ModelCurrent)
            {
                cmbHotkeyModel.SelectedIndex = 1;
            }
            else
            {
                var model = _settings.GetModelById(mapping.ModelId);
                if (model != null)
                {
                    int index = cmbHotkeyModel.Items.IndexOf(model.Name);
                    cmbHotkeyModel.SelectedIndex = index >= 0 ? index : 0;
                }
            }

            // Set action
            string actionDisplay = HotkeyActions.GetDisplayName(mapping.Action);
            int actionIndex = cmbHotkeyAction.Items.IndexOf(actionDisplay);
            cmbHotkeyAction.SelectedIndex = actionIndex >= 0 ? actionIndex : 0;

            // Set other properties
            txtHotkeyDescription.Text = mapping.Description;
            chkHotkeyEnabled.Checked = mapping.IsEnabled;
        }

        private void BtnSaveHotkey_Click(object? sender, EventArgs e)
        {
            if (_selectedMapping == null) return;

            try
            {
                // Get hotkey
                string hotkey = txtHotkeyEdit.GetHotkeyString();
                if (string.IsNullOrWhiteSpace(hotkey))
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "Hotkey cannot be empty";
                    return;
                }

                // Get model ID
                string modelId = HotkeyActions.ModelGlobal;
                if (cmbHotkeyModel.SelectedIndex == 1)
                {
                    modelId = HotkeyActions.ModelCurrent;
                }
                else if (cmbHotkeyModel.SelectedIndex > 1)
                {
                    string modelName = cmbHotkeyModel.SelectedItem?.ToString() ?? string.Empty;
                    var model = _settings.Models.FirstOrDefault(m => m.Name == modelName);
                    if (model != null)
                        modelId = model.Id;
                }

                // Get action
                string actionDisplay = cmbHotkeyAction.SelectedItem?.ToString() ?? string.Empty;
                string action = HotkeyActions.All.FirstOrDefault(a => 
                    HotkeyActions.GetDisplayName(a) == actionDisplay) ?? HotkeyActions.ProcessText;

                // Update mapping
                _selectedMapping.Hotkey = hotkey;
                _selectedMapping.ModelId = modelId;
                _selectedMapping.Action = action;
                _selectedMapping.Description = txtHotkeyDescription.Text;
                _selectedMapping.IsEnabled = chkHotkeyEnabled.Checked;

                // Validate
                string? validationError = HotkeyMappingManager.ValidateMapping(
                    _selectedMapping, 
                    _settings, 
                    _isEditingMapping ? _selectedMapping.Id : null);

                if (validationError != null)
                {
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = validationError;
                    return;
                }

                // Add if new
                if (!_isEditingMapping)
                {
                    _settings.HotkeyMappings.Add(_selectedMapping);
                }

                // Refresh and hide panel
                RefreshHotkeyMappingsList();
                pnlHotkeyEdit.Visible = false;
                _selectedMapping = null;
                _isEditingMapping = false;

                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Hotkey mapping saved";
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.Red;
                lblStatus.Text = $"Error saving hotkey: {ex.Message}";
            }
        }

        private void BtnCancelHotkey_Click(object? sender, EventArgs e)
        {
            pnlHotkeyEdit.Visible = false;
            _selectedMapping = null;
            _isEditingMapping = false;
            lblStatus.Text = string.Empty;
        }

        private void BtnRemoveHotkey_Click(object? sender, EventArgs e)
        {
            if (_selectedMapping == null) return;

            var result = MessageBox.Show(
                $"Remove hotkey mapping:\n{_selectedMapping.GetDisplayString(_settings)}?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _settings.HotkeyMappings.Remove(_selectedMapping);
                _selectedMapping = null;
                RefreshHotkeyMappingsList();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Hotkey mapping removed";
            }
        }

        private void BtnMoveUp_Click(object? sender, EventArgs e)
        {
            if (_selectedMapping == null) return;

            if (HotkeyMappingManager.MoveUp(_settings.HotkeyMappings, _selectedMapping.Id))
            {
                RefreshHotkeyMappingsList();
                // Maintain selection
                int newIndex = lstHotkeyMappings.SelectedIndex - 1;
                if (newIndex >= 0)
                    lstHotkeyMappings.SelectedIndex = newIndex;
            }
        }

        private void BtnMoveDown_Click(object? sender, EventArgs e)
        {
            if (_selectedMapping == null) return;

            if (HotkeyMappingManager.MoveDown(_settings.HotkeyMappings, _selectedMapping.Id))
            {
                RefreshHotkeyMappingsList();
                // Maintain selection
                int newIndex = lstHotkeyMappings.SelectedIndex + 1;
                if (newIndex < lstHotkeyMappings.Items.Count)
                    lstHotkeyMappings.SelectedIndex = newIndex;
            }
        }

        #endregion
    }
}

