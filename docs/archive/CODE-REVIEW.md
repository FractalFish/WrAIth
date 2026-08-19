# Code Review: Hardcoding Audit & Refactoring Plan

## Goal
Identify and eliminate hardcoded values, improve code reusability, and create a more dynamic, maintainable codebase.

---

## Hardcoding Issues Found

### 1. Provider Names (Strings)
**Location**: LLMService.cs, SettingsForm.cs, AppSettings.cs

**Current**:
```csharp
if (model.Provider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
if (model.Provider.Equals("Anthropic", StringComparison.OrdinalIgnoreCase))
if (provider == "Custom")
```

**Problem**: Magic strings scattered throughout code

**Solution**: Create constants class
```csharp
public static class ProviderTypes
{
    public const string OpenAI = "OpenAI";
    public const string Anthropic = "Anthropic";
    public const string Groq = "Groq";
    public const string Custom = "Custom";
}
```

**Benefit**: Single source of truth, no typos, easier refactoring

---

### 2. Default API Endpoints
**Location**: Multiple files

**Current**:
```csharp
txtEndpoint.Text = "https://api.openai.com/v1/chat/completions";
txtEndpoint.Text = "https://api.anthropic.com/v1/messages";
txtEndpoint.Text = "https://api.groq.com/openai/v1/chat/completions";
```

**Problem**: Hardcoded URLs, no central configuration

**Solution**: Configuration class
```csharp
public static class DefaultEndpoints
{
    public const string OpenAI = "https://api.openai.com/v1/chat/completions";
    public const string Anthropic = "https://api.anthropic.com/v1/messages";
    public const string Groq = "https://api.groq.com/openai/v1/chat/completions";
    
    public static string GetFor(string provider) => provider switch
    {
        ProviderTypes.OpenAI => OpenAI,
        ProviderTypes.Anthropic => Anthropic,
        ProviderTypes.Groq => Groq,
        _ => OpenAI // Default fallback
    };
}
```

---

### 3. Default Model IDs
**Location**: AppSettings.cs, SettingsForm.cs

**Current**:
```csharp
Model = "gpt-4o-mini"
Model = "gpt-4o"
Model = "claude-3-5-sonnet-20241022"
```

**Problem**: Model IDs change over time

**Solution**: Configuration with versioning
```csharp
public static class DefaultModels
{
    public const string OpenAI_Fast = "gpt-4o-mini";
    public const string OpenAI_Capable = "gpt-4o";
    public const string Anthropic_Fast = "claude-3-5-haiku-20241022";
    public const string Anthropic_Capable = "claude-3-5-sonnet-20241022";
    public const string Groq_Fast = "llama-3.1-8b-instant";
    public const string Groq_Capable = "llama-3.1-70b-versatile";
}
```

---

### 4. Hotkey Default Values
**Location**: ModelConfig.cs, AppSettings.cs

**Current**:
```csharp
TriggerHotkey = "Control+Shift+Q"
OutputHotkey = "Control+Shift+W"
AbortHotkey = "Control+Shift+E"
```

**Problem**: Not user-configurable defaults

**Solution**: Hotkey defaults configuration
```csharp
public static class DefaultHotkeys
{
    public const string SendQuery = "Control+Shift+Q";
    public const string Output = "Control+Shift+W";
    public const string Abort = "Control+Shift+E";
    public const string ScreenshotStart = "Control+Shift+S";
    public const string ScreenshotEnd = "Control+Shift+D";
    public const string VisionReasoning = "Control+Shift+A";
}
```

---

### 5. Timing Configuration
**Location**: KeyboardSimulator.cs, AppSettings.cs

**Current**:
```csharp
public int TypingDelayMs { get; set; } = 50;
public int TypingVariationMs { get; set; } = 20;
_httpClient.Timeout = TimeSpan.FromMinutes(2);
```

**Problem**: Some in settings, some hardcoded

**Solution**: Centralize all timing config
```csharp
public static class DefaultTimings
{
    public const int TypingDelayMs = 50;
    public const int TypingVariationMs = 20;
    public const int TypingDelayMin = 10;
    public const int TypingDelayMax = 500;
    public const int ApiTimeoutMinutes = 2;
}
```

---

### 6. UI Strings
**Location**: SettingsForm.cs, Form1.cs

**Current**:
```csharp
MessageBox.Show("Cannot remove the last model.", "Error", ...);
lblStatus.Text = "Settings saved! Restart application for changes to take effect.";
notifyIcon.ShowBalloonTip(2000, "BLLMT", "Response ready!", ...);
```

**Problem**: Strings scattered, hard to localize

**Solution**: Resource strings (future i18n support)
```csharp
public static class UIStrings
{
    public const string CannotRemoveLastModel = "Cannot remove the last model.";
    public const string SettingsSaved = "Settings saved! Restart application for changes to take effect.";
    public const string ResponseReady = "Response ready! Press output hotkey to start emulation.";
    // ... etc
}
```

---

### 7. JSON Property Names
**Location**: All model classes

**Current**:
```csharp
[JsonPropertyName("triggerHotkey")]
[JsonPropertyName("screenshotStartHotkey")]
```

**Problem**: String literals, prone to typos

**Solution**: These are fine - JSON serialization needs literal strings.
**No change needed** - attribute approach is correct.

---

### 8. Action Names (Future: Global Hotkeys)
**Location**: Will be in HotkeyMapping

**Current**: Not yet implemented

**Planned**:
```csharp
public static class HotkeyActions
{
    public const string ProcessText = "ProcessText";
    public const string ProcessImage = "ProcessImage";
    public const string ScreenshotStart = "ScreenshotStart";
    public const string ScreenshotEnd = "ScreenshotEnd";
    public const string VisionReasoning = "VisionReasoning";
    public const string Output = "Output";
    public const string Abort = "Abort";
}
```

---

## Code Duplication Issues

### 1. API Request Building
**Location**: LLMService.cs

**Problem**: Similar code for OpenAI, Anthropic, Custom

**Solution**: Extract common logic
```csharp
private HttpRequestMessage BuildRequest(ModelConfig model, string endpoint, string body)
{
    var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
    AddProviderHeaders(request, model);
    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
    return request;
}

private void AddProviderHeaders(HttpRequestMessage request, ModelConfig model)
{
    switch (model.Provider)
    {
        case ProviderTypes.OpenAI:
        case ProviderTypes.Groq:
            request.Headers.Add("Authorization", $"Bearer {model.ApiKey}");
            break;
        case ProviderTypes.Anthropic:
            request.Headers.Add("x-api-key", model.ApiKey);
            request.Headers.Add("anthropic-version", "2023-06-01");
            break;
    }
}
```

---

### 2. Model Validation
**Location**: SettingsForm.cs, AppSettings.cs

**Problem**: Validation logic scattered

**Solution**: Centralized validation
```csharp
public static class ModelValidator
{
    public static List<string> Validate(ModelConfig model)
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(model.Name))
            errors.Add("Model name is required");
            
        if (string.IsNullOrWhiteSpace(model.ApiKey))
            errors.Add("API key is required");
            
        if (string.IsNullOrWhiteSpace(model.Model))
            errors.Add("Model ID is required");
            
        if (string.IsNullOrWhiteSpace(model.Endpoint))
            errors.Add("Endpoint URL is required");
            
        if (!Uri.IsWellFormedUriString(model.Endpoint, UriKind.Absolute))
            errors.Add("Endpoint URL is not valid");
            
        return errors;
    }
}
```

---

### 3. Hotkey Parsing
**Location**: HotkeyManager.cs, Form1.cs

**Problem**: Parsing logic duplicated

**Solution**: Utility class
```csharp
public static class HotkeyParser
{
    public static (Keys Modifiers, Keys Key) Parse(string hotkeyString)
    {
        var parts = hotkeyString.Split('+', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        Keys modifiers = Keys.None;
        Keys key = Keys.None;
        
        foreach (var part in parts)
        {
            switch (part.ToUpperInvariant())
            {
                case "CONTROL":
                case "CTRL":
                    modifiers |= Keys.Control;
                    break;
                case "SHIFT":
                    modifiers |= Keys.Shift;
                    break;
                case "ALT":
                    modifiers |= Keys.Alt;
                    break;
                default:
                    if (Enum.TryParse<Keys>(part, true, out Keys parsedKey))
                        key = parsedKey;
                    break;
            }
        }
        
        return (modifiers, key);
    }
    
    public static string Format(Keys modifiers, Keys key)
    {
        var parts = new List<string>();
        if (modifiers.HasFlag(Keys.Control)) parts.Add("Control");
        if (modifiers.HasFlag(Keys.Shift)) parts.Add("Shift");
        if (modifiers.HasFlag(Keys.Alt)) parts.Add("Alt");
        parts.Add(key.ToString());
        return string.Join("+", parts);
    }
}
```

---

## Refactoring Plan

### Phase 1: Create Constants (2-3 hours)
1. Create `Constants.cs` with all constant classes
2. Replace magic strings throughout codebase
3. Test that functionality unchanged

**Files to create**:
- `Constants/ProviderTypes.cs`
- `Constants/DefaultEndpoints.cs`
- `Constants/DefaultModels.cs`
- `Constants/DefaultHotkeys.cs`
- `Constants/DefaultTimings.cs`
- `Constants/UIStrings.cs`
- `Constants/HotkeyActions.cs` (for global hotkeys)

### Phase 2: Extract Utilities (3-4 hours)
1. Create `Utils/ModelValidator.cs`
2. Create `Utils/HotkeyParser.cs`
3. Refactor LLMService request building
4. Remove duplicated code

### Phase 3: Service Layer (4-5 hours)
1. Create `Services/ILLMService.cs` interface
2. Extract business logic from Form1.cs
3. Create `Services/ClipboardService.cs`
4. Create `Services/HotkeyService.cs`

### Phase 4: Configuration Management (3-4 hours)
1. Create `Configuration/ConfigManager.cs`
2. Centralize all default values
3. Support config file overrides
4. Environment-specific configs

---

## Architecture Improvements

### Current Architecture
```
Form1.cs (God Object)
??? HotkeyManager
??? LLMService
??? KeyboardSimulator
??? KeyboardHook
??? ScreenshotService
??? SettingsForm
```

**Problem**: Form1.cs does too much

### Proposed Architecture
```
Application
??? Services/
?   ??? ILLMService (interface)
?   ??? LLMService (implementation)
?   ??? IClipboardService
?   ??? ClipboardService
?   ??? IHotkeyService
?   ??? HotkeyService
?   ??? IKeyboardService
?   ??? KeyboardService
??? Controllers/
?   ??? ApplicationController (replaces Form1 logic)
??? UI/
?   ??? SystemTrayForm
?   ??? SettingsForm
??? Utils/
    ??? ModelValidator
    ??? HotkeyParser
    ??? ApiHelper
```

**Benefits**:
- Clear separation of concerns
- Testable components
- Easier to extend
- Dependency injection ready

---

## Testing Strategy

### Unit Tests (Future)
```csharp
[TestClass]
public class ModelValidatorTests
{
    [TestMethod]
    public void Validate_EmptyName_ReturnsError()
    {
        var model = new ModelConfig { Name = "" };
        var errors = ModelValidator.Validate(model);
        Assert.IsTrue(errors.Any(e => e.Contains("name")));
    }
}
```

### Integration Tests (Future)
```csharp
[TestClass]
public class LLMServiceTests
{
    [TestMethod]
    public async Task GetResponse_ValidRequest_ReturnsResponse()
    {
        var service = new LLMService(testSettings);
        var response = await service.GetResponseAsync("Test");
        Assert.IsNotNull(response);
    }
}
```

---

## Priority Order

### Immediate (Before v1.0):
1. -> Create Constants classes
2. -> Replace magic strings
3. -> Extract HotkeyParser utility
4. -> Extract ModelValidator

### Short Term:
5. Refactor LLMService request building
6. Create service interfaces
7. Extract business logic from Form1

### Medium Term:
8. Implement dependency injection
9. Add unit tests
10. Improve error handling

### Long Term:
11. Full architectural refactor
12. Plugin system foundation
13. Comprehensive test suite

---

## Metrics

### Current State:
- **Magic Strings**: ~40 instances
- **Code Duplication**: ~15% (estimated)
- **Lines per Method**: Avg 30 (some >100)
- **Cyclomatic Complexity**: High in Form1.cs

### Target State:
- **Magic Strings**: 0 (all constants)
- **Code Duplication**: <5%
- **Lines per Method**: Avg 20 (max 50)
- **Cyclomatic Complexity**: Medium

---

## Next Steps

1. Review this document
2. Create Constants.cs
3. Begin Phase 1 refactoring
4. Test after each change
5. Commit incrementally

See **ROADMAP.md** for timeline integration.
