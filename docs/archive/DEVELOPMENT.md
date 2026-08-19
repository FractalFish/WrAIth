# BLLMT Development Guide

## Architecture Overview

BLLMT (Background LLM Multi-Tool) is a Windows/macOS background service that provides keyboard-driven AI assistance by intercepting keystrokes and replacing them with LLM responses.

### Core Components

#### 1. **Form1.cs** - Main Application Controller
- Runs as hidden background form
- Manages system tray icon
- Coordinates all services
- Handles hotkey events

#### 2. **HotkeyManager.cs** - Global Hotkey Registration
- Registers Windows global hotkeys
- Routes hotkey events to handlers
- Prevents conflicts with other applications

#### 3. **KeyboardHook.cs** - Low-Level Keyboard Interception
- Intercepts individual keystrokes during emulation
- Replaces user input with AI response
- Character-by-character replacement

#### 4. **KeyboardSimulator.cs** - Keystroke Emulation
- Simulates typing with realistic delays
- Uses SendInput API for natural typing
- Configurable speed and variation

#### 5. **LLMService.cs** - AI API Client
- Handles OpenAI, Anthropic, and custom API formats
- Supports text and vision models
- Provider-agnostic request/response handling

#### 6. **ScreenshotService.cs** - Screen Capture
- Overlay window for region selection
- Captures selected area
- Converts to base64 for vision APIs

#### 7. **SettingsForm.cs** - Configuration UI
- Per-model configuration
- Hotkey assignment
- System settings

---

## Data Models

### ModelConfig
Represents a single AI model configuration:
```csharp
public class ModelConfig
{
    public string Id { get; set; }                    // Unique identifier
    public string Name { get; set; }                  // User-friendly name
    public string Provider { get; set; }              // "OpenAI", "Anthropic", "Custom"
    public string ApiKey { get; set; }                // API authentication
    public string Model { get; set; }                 // Model ID (e.g., "gpt-4o-mini")
    public string Endpoint { get; set; }              // API endpoint URL
    public string SystemPrompt { get; set; }          // Per-model system prompt
    public bool IsDefault { get; set; }               // Default model flag
    public bool IsEnabled { get; set; }               // Can be disabled without deleting
    
    // Hotkeys (per-model)
    public string TriggerHotkey { get; set; }         // Send query
    public string ScreenshotStartHotkey { get; set; } // Begin screenshot
    public string ScreenshotEndHotkey { get; set; }   // End & analyze
    public string AppendVisionHotkey { get; set; }    // Vision + reasoning
    public string OutputHotkey { get; set; }          // Type response
    public string AbortHotkey { get; set; }           // Cancel operation
    
    // Custom API format (for non-standard APIs)
    public CustomApiFormat? CustomApiFormat { get; set; }
}
```

### CustomApiFormat
Allows users to define custom API request structures:
```csharp
public class CustomApiFormat
{
    public string Method { get; set; } = "POST";
    public List<string> Headers { get; set; }
    public string RequestBodyTemplate { get; set; }   // With {PLACEHOLDERS}
    public string VisionRequestBodyTemplate { get; set; }
    public string ResponseTextPath { get; set; }      // JSON path to extract response
}
```

**Placeholders**:
- `{API_KEY}` - Replaced with model's API key
- `{MODEL_ID}` - Replaced with model ID
- `{SYSTEM_PROMPT}` - Replaced with system prompt
- `{USER_MESSAGE}` - Replaced with user's query
- `{IMAGE_BASE64}` - Replaced with base64 image (vision only)

---

## Workflow: Text Processing

1. User copies text to clipboard
2. Presses **Send Query** hotkey -> `OnTriggerHotkey()`
3. Clipboard content read
4. Sent to LLMService with selected model
5. Response received and queued
6. User presses **Output** hotkey -> `OnOutputHotkey()`
7. Emulation starts: `KeyboardHook` intercepts keystrokes
8. Each keystroke replaced with next character from response
9. User types naturally, AI response appears instead

---

## Workflow: Vision + Reasoning (Two-Stage)

### Stage 1: Vision Analysis
1. User presses **Screenshot Start** -> Overlay appears
2. User drags to select region
3. User presses **Screenshot End** -> Capture & analyze
4. Vision model analyzes image
5. Result stored in `_visionResult`
6. Also queued in `_queuedResponse` for direct output

### Stage 2: Reasoning (Optional)
7. User copies question to clipboard
8. User presses **Vision + Reasoning** hotkey
9. Combines vision result with question:
   ```
   Context: [vision analysis]
   Question: [user's question]
   ```
10. Sent to reasoning model (text model)
11. Response queued
12. User presses **Output** to type

---

## Design Decisions

### Why Per-Model Hotkeys? (Current System)

**Current Design**: Each model has its own set of hotkeys

**Reasoning**:
- Different models for different tasks
- Quick switching between models without UI
- Can assign hotkeys only to frequently-used models

**Problem**:
- Hotkey conflicts possible
- Confusing which model responds to which key
- Hard to manage many models

**Solution** (Planned): Global hotkeys tab (see ROADMAP.md)

---

### Why Custom API Format?

**Problem**: Hard-coded support only for OpenAI and Anthropic formats

**Solution**: User-defined JSON templates with placeholders

**Benefits**:
- Support for any API (Groq, Together, Perplexity, local models)
- Future-proof as new providers emerge
- Users can adapt to API changes without code updates

**Example**:
```json
{
  "messages": [{"role": "user", "content": "{USER_MESSAGE}"}],
  "model": "{MODEL_ID}",
  "temperature": 0.7,
  "max_tokens": 2000,
  "stream": false
}
```

---

### Why Remove "Supports Vision" Checkbox?

**Old Design**: Manual checkbox to mark vision models

**Problem**:
- Redundant (if model has screenshot hotkeys -> it's vision-capable)
- Doesn't actually control anything
- Extra configuration step

**New Design**: Auto-detect vision capability:
```csharp
// Model is vision-capable if:
// 1. Has screenshot hotkeys assigned, OR
// 2. Model ID contains "vision" or "4o"
```

---

## API Provider Abstraction

### Current Implementation

LLMService routes requests based on `Provider` field:

```csharp
if (model.Provider == "OpenAI" || model.Provider == "Groq")
    return await GetOpenAIResponseAsync(...);
else if (model.Provider == "Anthropic")
    return await GetAnthropicResponseAsync(...);
else if (model.Provider == "Custom")
    return await GetCustomApiResponseAsync(...);
```

### OpenAI Format
```http
POST /v1/chat/completions
Authorization: Bearer {API_KEY}

{
  "model": "gpt-4o-mini",
  "messages": [
    {"role": "system", "content": "..."},
    {"role": "user", "content": "..."}
  ]
}
```

### Anthropic Format
```http
POST /v1/messages
x-api-key: {API_KEY}
anthropic-version: 2023-06-01

{
  "model": "claude-3-5-sonnet-20241022",
  "max_tokens": 2000,
  "system": "...",
  "messages": [{"role": "user", "content": "..."}]
}
```

### Custom Format
User-defined via CustomApiFormat (see above)

---

## Security Considerations

### API Key Storage
- Stored in plaintext in `AppData/BLLMT/settings.json`
- Masked in UI after first load (shows `********`)
- Original key preserved in `_originalApiKeys` dictionary

**TODO**: Consider encryption for production use

### Clipboard Access
- Application reads clipboard without user confirmation
- Required for core functionality
- User should be aware via documentation

---

## Known Limitations

### Hotkey Conflicts
**Current**: Per-model hotkeys can conflict between models

**Planned**: Global hotkeys tab (see ROADMAP.md)

### Windows-Only Keyboard Emulation
**Current**: Uses Windows SendInput API

**macOS**: Requires different approach (CGEvent)

**Planned**: Platform-specific implementations

### No Undo
**Current**: Once emulation starts, can't easily undo typed response

**Possible**:
- Ctrl+Z detection during emulation
- Character-by-character backspace simulation

---

## Testing Strategy

### Manual Testing
Currently tested manually through:
1. Settings form functionality
2. Hotkey registration
3. Keyboard interception
4. API calls to various providers
5. Screenshot capture

### Test Model Feature
Settings form includes "Test Model" button:
- Sends simple query: "Say 'Hello!' if you can read this."
- Verifies API connection and authentication
- Displays response preview

---

## Future Architectural Changes

See **ROADMAP.md** for detailed plans:

1. **Global Hotkeys System** - Hotkey -> Model + Action mapping
2. **Model Presets** - User-defined configuration templates
3. **Plugin System** - Extensible action framework
4. **Conversation History** - Track queries and responses
5. **Response Editing** - Preview/modify before output

---

## Code Organization

```
BLLMT/
??? Core/
?   ??? Form1.cs              - Main controller
?   ??? HotkeyManager.cs      - Hotkey registration
?   ??? KeyboardHook.cs       - Keyboard interception
?   ??? KeyboardSimulator.cs  - Keystroke emulation
??? Services/
?   ??? LLMService.cs         - AI API client
?   ??? ScreenshotService.cs  - Screen capture
??? Models/
?   ??? AppSettings.cs        - Configuration model
?   ??? ModelConfig.cs        - Model configuration
?   ??? CustomApiFormat.cs    - Custom API format
??? UI/
?   ??? SettingsForm.cs       - Settings UI
?   ??? SettingsForm.Designer.cs
?   ??? CustomFormatEditorForm.cs - Custom format editor
??? Utils/
    ??? WinFormsClipboard.cs  - Clipboard utilities
    ??? ...
```

---

## Contributing

See **CONTRIBUTING.md** for guidelines on:
- Code style
- Pull request process
- Issue reporting
- Feature requests
