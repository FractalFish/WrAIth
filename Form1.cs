namespace BLLMT
{
    public partial class Form1 : Form
    {
        private AppSettings _settings;
        private HotkeyManager? _hotkeyManager;
        private LLMService? _llmService;
        private KeyboardSimulator? _keyboardSimulator;
        private KeyboardHook? _keyboardHook;
        private ScreenshotService? _screenshotService;
        
        private string _queuedResponse = string.Empty;
        private string _visionResult = string.Empty; // Store vision analysis result
        private int _responsePosition = 0;
        private bool _isEmulationActive = false;
        private bool _isEmulationPaused = false;
        private bool _isProcessingRequest = false;
        
        // Track abort hotkey for detection during emulation
        private Keys _abortModifiers = Keys.None;
        private Keys _abortKey = Keys.None;

        public Form1()
        {
            InitializeComponent();
            Log("Application starting...");
            _settings = AppSettings.Load();
            Log($"Settings loaded from: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BLLMT", "settings.json")}");
            InitializeServices();
            RegisterHotkeys();
            ParseAbortHotkey();
        }

        private void InitializeServices()
        {
            Log("Initializing services...");
            _hotkeyManager = new HotkeyManager(this);
            _llmService = new LLMService(_settings);
            _keyboardSimulator = new KeyboardSimulator(_settings.TypingDelayMs, _settings.TypingVariationMs);
            _keyboardHook = new KeyboardHook();
            _keyboardHook.KeyPressed += OnKeyPressed;
            
            _screenshotService = new ScreenshotService();
            _screenshotService.ScreenshotCaptured += OnScreenshotCaptured;
            
            Log("Services initialized successfully");
        }

        private void RegisterHotkeys()
        {
            if (_hotkeyManager == null) return;

            Log("Registering hotkeys...");
            
            // Trigger hotkey - Read clipboard and send to LLM
            int triggerId = _hotkeyManager.RegisterHotkey(_settings.TriggerHotkey, OnTriggerHotkey);
            Log($"Trigger hotkey '{_settings.TriggerHotkey}' registered with ID: {triggerId}");
            
            // Output hotkey - Toggle emulation mode
            int outputId = _hotkeyManager.RegisterHotkey(_settings.OutputHotkey, OnOutputHotkey);
            Log($"Output hotkey '{_settings.OutputHotkey}' registered with ID: {outputId}");
            
            // Abort hotkey - Cancel emulation
            int abortId = _hotkeyManager.RegisterHotkey(_settings.AbortHotkey, OnAbortHotkey);
            Log($"Abort hotkey '{_settings.AbortHotkey}' registered with ID: {abortId}");

            // Screenshot Start hotkey
            int screenshotStartId = _hotkeyManager.RegisterHotkey(_settings.ScreenshotStartHotkey, OnScreenshotStartHotkey);
            Log($"Screenshot Start hotkey '{_settings.ScreenshotStartHotkey}' registered with ID: {screenshotStartId}");

            // Screenshot End hotkey
            int screenshotEndId = _hotkeyManager.RegisterHotkey(_settings.ScreenshotEndHotkey, OnScreenshotEndHotkey);
            Log($"Screenshot End hotkey '{_settings.ScreenshotEndHotkey}' registered with ID: {screenshotEndId}");

            // Append Vision Result hotkey
            int appendVisionId = _hotkeyManager.RegisterHotkey(_settings.AppendVisionHotkey, OnAppendVisionHotkey);
            Log($"Append Vision hotkey '{_settings.AppendVisionHotkey}' registered with ID: {appendVisionId}");

            UpdateStatus("Ready - Hotkeys registered");
            Log("All hotkeys registered successfully. Application ready.");
        }

        private void ParseAbortHotkey()
        {
            // Parse abort hotkey for detection in keyboard hook
            var parts = _settings.AbortHotkey.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            
            _abortModifiers = Keys.None;
            _abortKey = Keys.None;

            foreach (var part in parts)
            {
                string upperPart = part.ToUpperInvariant();
                
                if (upperPart == "CONTROL" || upperPart == "CTRL")
                {
                    _abortModifiers |= Keys.Control;
                }
                else if (upperPart == "SHIFT")
                {
                    _abortModifiers |= Keys.Shift;
                }
                else if (upperPart == "ALT")
                {
                    _abortModifiers |= Keys.Alt;
                }
                else
                {
                    if (Enum.TryParse<Keys>(part, true, out Keys key))
                    {
                        _abortKey = key;
                    }
                }
            }

            Log($"Abort hotkey parsed: Modifiers={_abortModifiers}, Key={_abortKey}");
        }

        private void OnScreenshotStartHotkey()
        {
            Log("=== SCREENSHOT START HOTKEY PRESSED ===");
            
            if (_screenshotService != null)
            {
                _screenshotService.StartCapture();
                UpdateStatus("Screenshot: Move mouse and press end hotkey to capture");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Screenshot started. Move mouse and press end hotkey.", ToolTipIcon.Info);
            }
        }

        private void OnScreenshotEndHotkey()
        {
            Log("=== SCREENSHOT END HOTKEY PRESSED ===");
            
            if (_screenshotService != null && _screenshotService.IsCapturing)
            {
                _screenshotService.EndCapture();
                UpdateStatus("Analyzing screenshot with vision model...");
            }
            else
            {
                Log("Screenshot end called but no capture in progress");
            }
        }

        private async void OnScreenshotCaptured(object? sender, ScreenshotService.ScreenshotCapturedEventArgs e)
        {
            Log($"Screenshot captured: {e.CaptureArea.Width}x{e.CaptureArea.Height}");
            
            if (_isProcessingRequest)
            {
                Log("Already processing a request, queuing screenshot for next cycle");
                UpdateStatus("Already processing, screenshot queued");
                return;
            }

            await ProcessVisionRequest(e.Base64Image, "screenshot");
        }

        private async void OnAppendVisionHotkey()
        {
            Log("=== APPEND VISION HOTKEY PRESSED ===");
            
            if (_isProcessingRequest)
            {
                Log("Already processing a request, ignoring append vision");
                UpdateStatus("Already processing a request...");
                return;
            }

            // Check if we have a stored vision result
            if (string.IsNullOrEmpty(_visionResult))
            {
                Log("No vision result available to append");
                UpdateStatus("No vision result to append. Capture screenshot or copy image first.");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "No vision result available", ToolTipIcon.Warning);
                return;
            }

            // Get clipboard text
            string clipboardText = string.Empty;
            if (WinFormsClipboard.ContainsText())
            {
                clipboardText = WinFormsClipboard.GetText();
                Log($"Clipboard text: {clipboardText.Length} characters");
            }

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                Log("No clipboard text to combine with vision result");
                UpdateStatus("Clipboard is empty. Copy your question first.");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Clipboard is empty", ToolTipIcon.Warning);
                return;
            }

            try
            {
                _isProcessingRequest = true;
                UpdateStatus("Combining vision result with your query...");
                Log("=== STAGE 2: REASONING WITH VISION CONTEXT ===");
                
                // Combine vision result with user's question
                string combinedPrompt = $"Context from image analysis:\n{_visionResult}\n\nUser question:\n{clipboardText}";
                
                Log($"Combined prompt length: {combinedPrompt.Length} characters");
                Log($"Combined prompt preview: {combinedPrompt.Substring(0, Math.Min(200, combinedPrompt.Length))}...");
                
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Reasoning with vision context...", ToolTipIcon.Info);

                // Send to reasoning model (text API)
                if (_llmService != null)
                {
                    _queuedResponse = await _llmService.GetResponseAsync(combinedPrompt);
                    _responsePosition = 0;
                    Log($"Reasoning response received: {_queuedResponse.Length} characters");
                    Log($"Response preview: {_queuedResponse.Substring(0, Math.Min(200, _queuedResponse.Length))}...");
                    UpdateStatus($"Response ready ({_queuedResponse.Length} chars). Press output hotkey.");
                    notifyIcon.ShowBalloonTip(3000, "BLLMT", "Reasoning complete! Response ready.", ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.GetType().Name}: {ex.Message}");
                UpdateStatus($"Error: {ex.Message}");
                notifyIcon.ShowBalloonTip(3000, "BLLMT Error", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                _isProcessingRequest = false;
                Log("=== STAGE 2 COMPLETE ===");
            }
        }

        private async Task ProcessVisionRequest(string base64Image, string source)
        {
            try
            {
                _isProcessingRequest = true;
                UpdateStatus($"Analyzing {source} with vision model...");
                Log($"=== STAGE 1: VISION ANALYSIS ({source}) ===");

                // Get clipboard text if combine option is enabled
                string visionPrompt = "Please describe what you see in this image in detail. Focus on all text, UI elements, diagrams, code, or any relevant information.";
                
                if (_settings.CombineScreenshotWithClipboard && WinFormsClipboard.ContainsText())
                {
                    string clipboardText = WinFormsClipboard.GetText();
                    if (!string.IsNullOrWhiteSpace(clipboardText))
                    {
                        visionPrompt = clipboardText;
                        Log($"Using clipboard text as vision prompt: {clipboardText.Length} characters");
                    }
                }

                Log($"Vision prompt: {visionPrompt.Substring(0, Math.Min(100, visionPrompt.Length))}...");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", $"Analyzing {source} with vision model...", ToolTipIcon.Info);

                // Send to vision model
                if (_llmService != null)
                {
                    _visionResult = await _llmService.GetResponseAsync(visionPrompt, base64Image);
                    Log($"Vision analysis received: {_visionResult.Length} characters");
                    Log($"Vision result preview: {_visionResult.Substring(0, Math.Min(300, _visionResult.Length))}...");
                    
                    // Store as queued response for direct output
                    _queuedResponse = _visionResult;
                    _responsePosition = 0;
                    
                    UpdateStatus($"Vision analysis complete! ({_visionResult.Length} chars). Press append hotkey to combine with query.");
                    notifyIcon.ShowBalloonTip(3000, "BLLMT", $"Vision analysis done! Press append hotkey or output to type.", ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR processing vision: {ex.GetType().Name}: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                UpdateStatus($"Error: {ex.Message}");
                notifyIcon.ShowBalloonTip(3000, "BLLMT Error", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                _isProcessingRequest = false;
                Log("=== STAGE 1 COMPLETE ===");
            }
        }

        private async void OnTriggerHotkey()
        {
            Log("=== TRIGGER HOTKEY PRESSED ===");
            
            if (_isProcessingRequest)
            {
                Log("Already processing a request, ignoring trigger");
                UpdateStatus("Already processing a request...");
                return;
            }

            try
            {
                _isProcessingRequest = true;
                
                // Check for image in clipboard first (if auto-detect enabled)
                if (_settings.AutoDetectClipboardImages && WinFormsClipboard.ContainsImage())
                {
                    Log("Image detected in clipboard, processing with vision model");
                    UpdateStatus("Processing clipboard image...");
                    
                    var image = WinFormsClipboard.GetImage();
                    if (image != null)
                    {
                        string base64Image = ImageToBase64(image);
                        await ProcessVisionRequest(base64Image, "clipboard image");
                        return;
                    }
                }

                // Process text from clipboard
                UpdateStatus("Processing clipboard text...");
                Log("Processing clipboard text content...");

                string clipboardText = string.Empty;
                if (WinFormsClipboard.ContainsText())
                {
                    clipboardText = WinFormsClipboard.GetText();
                    Log($"Clipboard content read: {clipboardText.Length} characters");
                    Log($"Clipboard preview: {clipboardText.Substring(0, Math.Min(100, clipboardText.Length))}...");
                }

                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    Log("Clipboard is empty!");
                    UpdateStatus("Clipboard is empty");
                    notifyIcon.ShowBalloonTip(2000, "BLLMT", "Clipboard is empty", ToolTipIcon.Warning);
                    _isProcessingRequest = false;
                    return;
                }

                UpdateStatus("Sending to LLM...");
                Log($"Sending to LLM (Provider: {_settings.LlmProvider}, Model: {_settings.Model})...");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Processing clipboard content...", ToolTipIcon.Info);

                // Send to LLM
                if (_llmService != null)
                {
                    _queuedResponse = await _llmService.GetResponseAsync(clipboardText);
                    _responsePosition = 0;
                    Log($"LLM response received: {_queuedResponse.Length} characters");
                    Log($"Response preview: {_queuedResponse.Substring(0, Math.Min(200, _queuedResponse.Length))}...");
                    UpdateStatus($"Response ready ({_queuedResponse.Length} chars). Press output hotkey to start emulation.");
                    notifyIcon.ShowBalloonTip(3000, "BLLMT", "Response ready! Press output hotkey to start emulation.", ToolTipIcon.Info);
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.GetType().Name}: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
                UpdateStatus($"Error: {ex.Message}");
                notifyIcon.ShowBalloonTip(3000, "BLLMT Error", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                _isProcessingRequest = false;
                Log("=== TRIGGER PROCESSING COMPLETE ===");
            }
        }

        private string ImageToBase64(System.Drawing.Image image)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private void OnOutputHotkey()
        {
            Log("=== OUTPUT HOTKEY PRESSED ===");
            
            if (string.IsNullOrEmpty(_queuedResponse))
            {
                Log("No response queued, nothing to emulate");
                UpdateStatus("No response queued");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "No response available. Copy text and press trigger hotkey first.", ToolTipIcon.Warning);
                return;
            }

            if (!_isEmulationActive)
            {
                // Start emulation
                _isEmulationActive = true;
                _isEmulationPaused = false;
                _responsePosition = 0;
                _keyboardHook?.Start();
                
                int remaining = _queuedResponse.Length - _responsePosition;
                Log($"Emulation STARTED - {remaining} characters remaining");
                UpdateStatus($"Emulation active ({remaining} chars remaining)");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", $"Emulation started! Type to output response ({remaining} chars)", ToolTipIcon.Info);
            }
            else if (_isEmulationPaused)
            {
                // Resume emulation
                _isEmulationPaused = false;
                
                int remaining = _queuedResponse.Length - _responsePosition;
                Log($"Emulation RESUMED - {remaining} characters remaining");
                UpdateStatus($"Emulation active ({remaining} chars remaining)");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", $"Emulation resumed ({remaining} chars)", ToolTipIcon.Info);
            }
            else
            {
                // Pause emulation
                _isEmulationPaused = true;
                
                int remaining = _queuedResponse.Length - _responsePosition;
                Log($"Emulation PAUSED - {remaining} characters remaining");
                UpdateStatus($"Emulation paused ({remaining} chars remaining)");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", $"Emulation paused ({remaining} chars)", ToolTipIcon.Info);
            }
        }

        private void OnAbortHotkey()
        {
            Log("=== ABORT HOTKEY PRESSED ===");
            
            // Cancel screenshot if in progress
            if (_screenshotService != null && _screenshotService.IsCapturing)
            {
                _screenshotService.CancelCapture();
                UpdateStatus("Screenshot cancelled");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Screenshot cancelled", ToolTipIcon.Info);
                return;
            }
            
            if (_isEmulationActive)
            {
                Log("Aborting emulation");
                StopEmulation();
                UpdateStatus("Emulation aborted");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Emulation aborted", ToolTipIcon.Warning);
            }
            else if (!string.IsNullOrEmpty(_queuedResponse))
            {
                // Clear queued response and vision result
                Log($"Clearing queued response ({_queuedResponse.Length} characters) and vision result");
                _queuedResponse = string.Empty;
                _visionResult = string.Empty;
                _responsePosition = 0;
                UpdateStatus("Queue cleared");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Response queue cleared", ToolTipIcon.Info);
            }
        }

        private void OnKeyPressed(object? sender, KeyboardHook.KeyEventArgs e)
        {
            // Only process if emulation is active and not paused
            if (!_isEmulationActive || _isEmulationPaused)
                return;

            // Check if this is the abort hotkey combination
            Keys currentModifiers = Control.ModifierKeys;
            if (currentModifiers == _abortModifiers && e.Key == _abortKey)
            {
                Log($"Abort hotkey detected in keyboard hook: {e.Key} with modifiers {currentModifiers}");
                e.SuppressKeyPress = false;
                StopEmulation();
                UpdateStatus("Emulation aborted");
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Emulation aborted", ToolTipIcon.Warning);
                return;
            }

            // Don't intercept if we've reached the end of the response
            if (_responsePosition >= _queuedResponse.Length)
            {
                Log("Response queue exhausted - stopping emulation");
                StopEmulation();
                return;
            }

            // Ignore modifier keys
            if (e.Key == Keys.LControlKey || e.Key == Keys.RControlKey ||
                e.Key == Keys.LShiftKey || e.Key == Keys.RShiftKey ||
                e.Key == Keys.LMenu || e.Key == Keys.RMenu ||
                e.Key == Keys.LWin || e.Key == Keys.RWin)
            {
                return;
            }

            Log($"Key intercepted: {e.Key} - Replacing with response char #{_responsePosition}");

            // Suppress the original keystroke
            e.SuppressKeyPress = true;

            // Get the next character from the response
            char nextChar = _queuedResponse[_responsePosition];
            _responsePosition++;

            Log($"Typing character: '{nextChar}' (position {_responsePosition}/{_queuedResponse.Length})");

            // Type the character
            _keyboardSimulator?.TypeCharacter(nextChar);

            // Update status
            int remaining = _queuedResponse.Length - _responsePosition;
            UpdateStatus($"Emulation active ({remaining} chars remaining)");

            // Check if we're done
            if (_responsePosition >= _queuedResponse.Length)
            {
                Log("Response fully typed - stopping emulation");
                StopEmulation();
                notifyIcon.ShowBalloonTip(2000, "BLLMT", "Response completed!", ToolTipIcon.Info);
            }
        }

        private void StopEmulation()
        {
            _isEmulationActive = false;
            _isEmulationPaused = false;
            _keyboardHook?.Stop();
            _queuedResponse = string.Empty;
            _responsePosition = 0;
            UpdateStatus("Ready");
            Log("Emulation stopped and queue cleared");
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_HOTKEY = 0x0312;
            
            if (m.Msg == WM_HOTKEY)
            {
                int hotkeyId = m.WParam.ToInt32();
                Log($"Hotkey message received: ID={hotkeyId}");
                _hotkeyManager?.ProcessHotkey(hotkeyId);
            }
            
            base.WndProc(ref m);
        }

        private void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                Invoke(() => UpdateStatus(status));
                return;
            }

            if (statusMenuItem != null)
            {
                statusMenuItem.Text = status;
            }
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string logMessage = $"[{timestamp}] {message}";
            
            System.Diagnostics.Debug.WriteLine(logMessage);
            Console.WriteLine(logMessage);
        }

        private void NotifyIcon_DoubleClick(object? sender, EventArgs e)
        {
            Log("System tray icon double-clicked - opening settings");
            ShowSettings();
        }

        private void SettingsMenuItem_Click(object? sender, EventArgs e)
        {
            Log("Settings menu item clicked");
            ShowSettings();
        }

        private void ShowSettings()
        {
            var settingsForm = new SettingsForm(_settings);
            settingsForm.ShowDialog();
            
            _settings = AppSettings.Load();
            Log("Settings reloaded after settings form closed");
            
            notifyIcon.ShowBalloonTip(3000, "BLLMT", 
                "Settings saved. Restart application for hotkey changes to take effect.", 
                ToolTipIcon.Info);
        }

        private void ExitMenuItem_Click(object? sender, EventArgs e)
        {
            Log("Exit menu item clicked - shutting down application");
            StopEmulation();
            _screenshotService?.CancelCapture();
            _keyboardHook?.Dispose();
            _hotkeyManager?.Dispose();
            notifyIcon.Visible = false;
            WinFormsApp.Exit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Hide();
            this.ShowInTaskbar = false;
            Log("Form loaded and hidden");
        }
    }
}
