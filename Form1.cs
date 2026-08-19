using Wraith.Constants;

namespace Wraith
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
        private string _visionResult = string.Empty;
        private string _pendingScreenshotModelId = string.Empty;
        private int _responsePosition = 0;
        private bool _isEmulationActive = false;
        private bool _isEmulationPaused = false;
        private bool _isProcessingRequest = false;

        private Keys _abortModifiers = Keys.None;
        private Keys _abortKey = Keys.None;

        public Form1()
        {
            InitializeComponent();
            Log("Application starting...");
            _settings = AppSettings.Load();
            Log($"Settings loaded from: {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Wraith", "settings.json")}");
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

            Log("Registering global hotkeys from HotkeyMappings...");

            int registeredCount = 0;
            foreach (var mapping in _settings.HotkeyMappings)
            {
                if (!mapping.IsEnabled)
                {
                    Log($"Skipping disabled mapping: {mapping.GetDisplayString(_settings)}");
                    continue;
                }

                if (string.IsNullOrEmpty(mapping.Hotkey))
                {
                    Log($"Skipping mapping with empty hotkey: {mapping.Description}");
                    continue;
                }

                int id = _hotkeyManager.RegisterHotkey(mapping.Hotkey, () =>
                {
                    OnHotkeyTriggered(mapping);
                });

                if (id >= 0)
                {
                    Log($"? Registered: {mapping.Hotkey} ? {HotkeyActions.GetDisplayName(mapping.Action)} (ID: {id})");
                    registeredCount++;
                }
                else
                {
                    Log($"? FAILED to register: {mapping.Hotkey} ? {HotkeyActions.GetDisplayName(mapping.Action)}");
                }
            }

            UpdateStatus($"Ready - {registeredCount} hotkeys registered");
            Log($"=== Hotkey registration complete: {registeredCount} active ===");
        }

        private void OnHotkeyTriggered(HotkeyMapping mapping)
        {
            Log($"=== HOTKEY TRIGGERED: {mapping.Action} ===");
            Log($"Mapping: {mapping.GetDisplayString(_settings)}");

            try
            {
                switch (mapping.Action)
                {
                    case HotkeyActions.ProcessText:
                        OnProcessText(mapping.ModelId);
                        break;

                    case HotkeyActions.ProcessImage:
                        OnProcessImage(mapping.ModelId);
                        break;

                    case HotkeyActions.ScreenshotStart:
                        OnScreenshotStart(mapping.ModelId);
                        break;

                    case HotkeyActions.ScreenshotEnd:
                        OnScreenshotEnd(mapping.ModelId);
                        break;

                    case HotkeyActions.VisionReasoning:
                        OnVisionReasoning(mapping.ModelId);
                        break;

                    case HotkeyActions.Output:
                        OnOutputHotkey();
                        break;

                    case HotkeyActions.Abort:
                        OnAbortHotkey();
                        break;

                    default:
                        Log($"WARNING: Unknown action type: {mapping.Action}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"ERROR handling hotkey action: {ex.Message}");
                Log($"Stack trace: {ex.StackTrace}");
            }
        }

        private ModelConfig? GetModelForAction(string modelId)
        {
            if (modelId == HotkeyActions.ModelCurrent)
            {
                Log("(Current) model requested - using default");
                return _settings.GetDefaultTextModel();
            }

            if (modelId == HotkeyActions.ModelGlobal)
            {
                Log("Global action - no specific model");
                return null;
            }

            var model = _settings.GetModelById(modelId);
            if (model == null)
            {
                Log($"Model ID {modelId} not found, falling back to default");
                return _settings.GetDefaultTextModel();
            }

            return model;
        }

        private LLMService CreateLLMServiceForModel(ModelConfig model)
        {
            var tempSettings = new AppSettings
            {
                Models = new List<ModelConfig> { model },
                TypingDelayMs = _settings.TypingDelayMs,
                TypingVariationMs = _settings.TypingVariationMs
            };
            tempSettings.Models[0].IsDefault = true;

            return new LLMService(tempSettings);
        }

        private void ParseAbortHotkey()
        {
            // Try to get abort hotkey from first mapping with Abort action
            var abortMapping = _settings.HotkeyMappings.FirstOrDefault(m =>
                m.Action == HotkeyActions.Abort && m.IsEnabled);

            string abortHotkey = abortMapping?.Hotkey ?? _settings.AbortHotkey;

            var parts = abortHotkey.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            _abortModifiers = Keys.None;
            _abortKey = Keys.None;

            foreach (var part in parts)
            {
                string upperPart = part.ToUpperInvariant();

                if (upperPart == "CONTROL" || upperPart == "CTRL")
                    _abortModifiers |= Keys.Control;
                else if (upperPart == "SHIFT")
                    _abortModifiers |= Keys.Shift;
                else if (upperPart == "ALT")
                    _abortModifiers |= Keys.Alt;
                else if (Enum.TryParse<Keys>(part, true, out Keys key))
                    _abortKey = key;
            }

            Log($"Abort hotkey parsed: Modifiers={_abortModifiers}, Key={_abortKey}");
        }

        private void OnScreenshotStart(string modelId)
        {
            Log("=== SCREENSHOT START ACTION ===");
            Log($"Model: {modelId}");

            if (_screenshotService != null)
            {
                _screenshotService.StartCapture();
                UpdateStatus("Screenshot: Move mouse and press end hotkey to capture");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.NotificationScreenshotStarted, ToolTipIcon.Info);
            }
        }

        private void OnScreenshotEnd(string modelId)
        {
            Log("=== SCREENSHOT END ACTION ===");
            Log($"Model: {modelId}");

            _pendingScreenshotModelId = modelId;

            if (_screenshotService != null && _screenshotService.IsCapturing)
            {
                _screenshotService.EndCapture();
                UpdateStatus("Analyzing screenshot...");
            }
            else
            {
                Log("Screenshot end called but no capture in progress");
            }
        }

        private void OnScreenshotStartHotkey() => OnScreenshotStart(HotkeyActions.ModelGlobal);
        private void OnScreenshotEndHotkey() => OnScreenshotEnd(HotkeyActions.ModelGlobal);

        private async void OnScreenshotCaptured(object? sender, ScreenshotService.ScreenshotCapturedEventArgs e)
        {
            Log($"Screenshot captured: {e.CaptureArea.Width}x{e.CaptureArea.Height}");

            if (_isProcessingRequest)
            {
                Log("Already processing a request, queuing screenshot");
                UpdateStatus("Already processing, screenshot queued");
                return;
            }

            var model = GetModelForAction(_pendingScreenshotModelId);
            if (model == null)
            {
                Log("No model for screenshot, using default vision model");
                model = _settings.GetDefaultVisionModel();
            }

            if (model != null)
            {
                await ProcessVisionRequest(e.Base64Image, "screenshot", model);
            }
            _pendingScreenshotModelId = string.Empty;
        }

        private async void OnVisionReasoning(string modelId)
        {
            Log("=== VISION + REASONING ACTION ===");

            if (_isProcessingRequest)
            {
                Log("Already processing a request, ignoring");
                UpdateStatus("Already processing a request...");
                return;
            }

            if (string.IsNullOrEmpty(_visionResult))
            {
                Log("No vision result available");
                UpdateStatus(UIStrings.ErrorNoVisionResult);
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, "No vision result available", ToolTipIcon.Warning);
                return;
            }

            string clipboardText = string.Empty;
            if (WinFormsClipboard.ContainsText())
            {
                clipboardText = WinFormsClipboard.GetText();
            }

            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                Log("No clipboard text for reasoning");
                UpdateStatus("Clipboard is empty. Copy your question first.");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, "Clipboard is empty", ToolTipIcon.Warning);
                return;
            }

            var model = GetModelForAction(modelId);
            if (model == null)
            {
                Log("No model for reasoning");
                model = _settings.GetDefaultTextModel();
            }

            if (model == null) return;

            try
            {
                _isProcessingRequest = true;
                UpdateStatus("Combining vision with reasoning...");
                Log("=== STAGE 2: REASONING WITH VISION CONTEXT ===");

                string combinedPrompt = string.Format(UIStrings.VisionPromptCombined, _visionResult, clipboardText);

                Log($"Combined prompt length: {combinedPrompt.Length} characters");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, "Reasoning with vision context...", ToolTipIcon.Info);

                var llmService = CreateLLMServiceForModel(model);
                _queuedResponse = await llmService.GetResponseAsync(combinedPrompt);
                _responsePosition = 0;

                Log($"Reasoning response received: {_queuedResponse.Length} characters");
                UpdateStatus(string.Format(UIStrings.ResponseReady, _queuedResponse.Length));
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, UIStrings.AppName, UIStrings.NotificationReasoningComplete, ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                UpdateStatus($"Error: {ex.Message}");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, "Wraith Error", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                _isProcessingRequest = false;
                Log("=== STAGE 2 COMPLETE ===");
            }
        }

        private void OnAppendVisionHotkey() => OnVisionReasoning(HotkeyActions.ModelGlobal);

        private async Task ProcessVisionRequest(string base64Image, string source, ModelConfig model)
        {
            try
            {
                _isProcessingRequest = true;
                UpdateStatus($"Analyzing {source}...");
                Log($"=== STAGE 1: VISION ANALYSIS ({source}) with {model.Name} ===");

                string visionPrompt = UIStrings.VisionPromptDefault;

                if (model.CombineScreenshotWithClipboard && WinFormsClipboard.ContainsText())
                {
                    string clipboardText = WinFormsClipboard.GetText();
                    if (!string.IsNullOrWhiteSpace(clipboardText))
                    {
                        visionPrompt = clipboardText;
                        Log($"Using clipboard text as vision prompt: {clipboardText.Length} characters");
                    }
                }

                Log($"Vision prompt: {visionPrompt.Substring(0, Math.Min(100, visionPrompt.Length))}...");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, $"Analyzing {source}...", ToolTipIcon.Info);

                var llmService = CreateLLMServiceForModel(model);
                _visionResult = await llmService.GetResponseAsync(visionPrompt, base64Image);

                Log($"Vision analysis received: {_visionResult.Length} characters");

                _queuedResponse = _visionResult;
                _responsePosition = 0;

                UpdateStatus($"Vision analysis complete! ({_visionResult.Length} chars)");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, UIStrings.AppName, UIStrings.NotificationVisionAnalysisComplete, ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                Log($"ERROR processing vision: {ex.Message}");
                UpdateStatus($"Error: {ex.Message}");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, "Wraith Error", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                _isProcessingRequest = false;
                Log("=== STAGE 1 COMPLETE ===");
            }
        }

        private async void OnProcessText(string modelId)
        {
            Log("=== PROCESS TEXT ACTION ===");

            if (_isProcessingRequest)
            {
                Log("Already processing a request, ignoring");
                UpdateStatus("Already processing a request...");
                return;
            }

            var model = GetModelForAction(modelId);
            if (model == null)
            {
                Log($"Model not found: {modelId}");
                UpdateStatus("Error: Model not found");
                return;
            }

            Log($"Using model: {model.Name}");

            try
            {
                _isProcessingRequest = true;

                if (model.AutoDetectClipboardImages && WinFormsClipboard.ContainsImage())
                {
                    Log("Image detected in clipboard, processing with vision");
                    UpdateStatus("Processing clipboard image...");

                    var image = WinFormsClipboard.GetImage();
                    if (image != null)
                    {
                        string base64Image = ImageToBase64(image);
                        await ProcessVisionRequest(base64Image, "clipboard image", model);
                        return;
                    }
                }

                UpdateStatus("Processing clipboard text...");
                Log("Processing clipboard text content...");

                string clipboardText = string.Empty;
                if (WinFormsClipboard.ContainsText())
                {
                    clipboardText = WinFormsClipboard.GetText();
                    Log($"Clipboard content read: {clipboardText.Length} characters");
                }

                if (string.IsNullOrWhiteSpace(clipboardText))
                {
                    Log("Clipboard is empty!");
                    UpdateStatus("Clipboard is empty");
                    notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.ErrorClipboardEmpty, ToolTipIcon.Warning);
                    _isProcessingRequest = false;
                    return;
                }

                UpdateStatus("Sending to LLM...");
                Log($"Sending to model: {model.Name}");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.NotificationProcessing, ToolTipIcon.Info);

                var llmService = CreateLLMServiceForModel(model);

                _queuedResponse = await llmService.GetResponseAsync(clipboardText);
                _responsePosition = 0;
                Log($"LLM response received: {_queuedResponse.Length} characters");
                UpdateStatus(string.Format(UIStrings.ResponseReady, _queuedResponse.Length));
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, UIStrings.AppName, UIStrings.NotificationResponseReady, ToolTipIcon.Info);
            }
            catch (Exception ex)
            {
                Log($"ERROR: {ex.Message}");
                UpdateStatus($"Error: {ex.Message}");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, "Wraith Error", ex.Message, ToolTipIcon.Error);
            }
            finally
            {
                _isProcessingRequest = false;
                Log("=== PROCESS TEXT COMPLETE ===");
            }
        }

        private void OnProcessImage(string modelId) => OnProcessText(modelId);
        private void OnTriggerHotkey() => OnProcessText(HotkeyActions.ModelGlobal);

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
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.ErrorNoResponseQueued, ToolTipIcon.Warning);
                return;
            }

            if (!_isEmulationActive)
            {
                _isEmulationActive = true;
                _isEmulationPaused = false;
                _responsePosition = 0;
                _keyboardHook?.Start();

                int remaining = _queuedResponse.Length - _responsePosition;
                Log($"Emulation STARTED - {remaining} characters remaining");
                UpdateStatus($"Emulation active ({remaining} chars remaining)");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, string.Format(UIStrings.NotificationEmulationStarted, remaining), ToolTipIcon.Info);
            }
            else if (_isEmulationPaused)
            {
                _isEmulationPaused = false;

                int remaining = _queuedResponse.Length - _responsePosition;
                Log($"Emulation RESUMED - {remaining} characters remaining");
                UpdateStatus($"Emulation active ({remaining} chars remaining)");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, string.Format(UIStrings.NotificationEmulationResumed, remaining), ToolTipIcon.Info);
            }
            else
            {
                _isEmulationPaused = true;

                int remaining = _queuedResponse.Length - _responsePosition;
                Log($"Emulation PAUSED - {remaining} characters remaining");
                UpdateStatus($"Emulation paused ({remaining} chars remaining)");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, string.Format(UIStrings.NotificationEmulationPaused, remaining), ToolTipIcon.Info);
            }
        }

        private void OnAbortHotkey()
        {
            Log("=== ABORT HOTKEY PRESSED ===");

            if (_screenshotService != null && _screenshotService.IsCapturing)
            {
                _screenshotService.CancelCapture();
                UpdateStatus("Screenshot cancelled");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.NotificationScreenshotCancelled, ToolTipIcon.Info);
                return;
            }

            if (_isEmulationActive)
            {
                Log("Aborting emulation");
                StopEmulation();
                UpdateStatus("Emulation aborted");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.NotificationEmulationAborted, ToolTipIcon.Warning);
            }
            else if (!string.IsNullOrEmpty(_queuedResponse))
            {
                Log($"Clearing queued response ({_queuedResponse.Length} characters) and vision result");
                _queuedResponse = string.Empty;
                _visionResult = string.Empty;
                _responsePosition = 0;
                UpdateStatus("Queue cleared");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.NotificationQueueCleared, ToolTipIcon.Info);
            }
        }

        private void OnKeyPressed(object? sender, KeyboardHook.KeyEventArgs e)
        {
            if (!_isEmulationActive || _isEmulationPaused)
                return;

            Keys currentModifiers = Control.ModifierKeys;
            if (currentModifiers == _abortModifiers && e.Key == _abortKey)
            {
                Log($"Abort hotkey detected in keyboard hook: {e.Key} with modifiers {currentModifiers}");
                e.SuppressKeyPress = false;
                StopEmulation();
                UpdateStatus("Emulation aborted");
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.NotificationEmulationAborted, ToolTipIcon.Warning);
                return;
            }

            if (_responsePosition >= _queuedResponse.Length)
            {
                Log("Response queue exhausted - stopping emulation");
                StopEmulation();
                return;
            }

            if (e.Key == Keys.LControlKey || e.Key == Keys.RControlKey ||
                e.Key == Keys.LShiftKey || e.Key == Keys.RShiftKey ||
                e.Key == Keys.LMenu || e.Key == Keys.RMenu ||
                e.Key == Keys.LWin || e.Key == Keys.RWin)
            {
                return;
            }

            Log($"Key intercepted: {e.Key} - Replacing with response char #{_responsePosition}");

            e.SuppressKeyPress = true;

            char nextChar = _queuedResponse[_responsePosition];
            _responsePosition++;

            Log($"Typing character: '{nextChar}' (position {_responsePosition}/{_queuedResponse.Length})");

            _keyboardSimulator?.TypeCharacter(nextChar);

            int remaining = _queuedResponse.Length - _responsePosition;
            UpdateStatus($"Emulation active ({remaining} chars remaining)");

            if (_responsePosition >= _queuedResponse.Length)
            {
                Log("Response fully typed - stopping emulation");
                StopEmulation();
                notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDuration, UIStrings.AppName, UIStrings.ResponseCompleted, ToolTipIcon.Info);
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

            notifyIcon.ShowBalloonTip(DefaultTimings.BalloonTipDurationLong, UIStrings.AppName,
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