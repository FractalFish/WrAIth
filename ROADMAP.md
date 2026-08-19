# Wraith Roadmap

## Current Status: v0.9-alpha

### Completed Features

#### Core Functionality
- Background service with system tray icon
- Global hotkey registration
- Low-level keyboard interception
- Keystroke-by-keystroke emulation with realistic timing
- Clipboard integration
- Multi-model support
- Per-model configuration
- Per-model hotkeys
- Per-model system prompts

#### AI Integration
- OpenAI API support
- Anthropic (Claude) API support
- Groq API support
- Custom API format (user-defined JSON templates)
- Text and vision model support
- Two-stage vision + reasoning workflow

#### Screenshot & Vision
- Interactive screenshot selection
- Region capture with overlay
- Base64 encoding for vision APIs
- Screenshot hotkeys (Start/End)
- Auto-detect clipboard images
- Vision + reasoning combination

#### Settings & Configuration
- Settings form with tabs (Models, Global Settings)
- Per-model configuration
- Custom API format editor
- Typing speed configuration
- API key masking in UI
- Model enable/disable toggle
- Test model connection button
- Automatic vision capability detection

#### Recent Improvements
- Removed redundant "Supports Vision" checkbox
- Wired up Custom API Format editor
- Fixed label overlap issues
- Simplified model testing
- Renamed "Trigger" -> "Send Query"
- Renamed "Append Vision" -> "Vision + Reasoning"
- Removed dead "Analyze Screenshot" code
- Removed duplicate "Shutdown" button

---

## In Progress

### Phase 1: Documentation Cleanup
**Status**: In progress
**Goal**: Reduce 31 markdown files -> 10 organized files

**Tasks**:
- Create consolidation plan
- Create DEVELOPMENT.md (this file)
- Create ROADMAP.md (this file)
- Create CODE-REVIEW.md
- Delete 18 obsolete instruction files
- Update README.md

---

## Planned Features

### Phase 2: Global Hotkeys System -> **HIGH PRIORITY**
**Problem**: Current per-model hotkeys can conflict

**Solution**: Invert relationship - Hotkey -> Model + Action

**New Data Model**:
```csharp
public class HotkeyMapping
{
    public string Id { get; set; }
    public string Hotkey { get; set; }      // "Control+Shift+Q"
    public string ModelId { get; set; }     // Model ID or "(Current)" or "(Global)"
    public string Action { get; set; }      // "ProcessText", "Output", "Abort", etc.
}
```

**Benefits**:
- No conflicts possible (one hotkey = one action)
- See all mappings at a glance
- Flexible (multiple hotkeys -> same model)
- Intuitive workflow

**Implementation**:
1. Create HotkeyMapping class
2. Add `List<HotkeyMapping>` to AppSettings
3. Create new "Hotkeys" tab in Settings
4. Remove hotkey fields from Models tab
5. Update Form1.cs RegisterHotkeys() to use global mappings
6. Migration from per-model to global hotkeys

**Estimated Effort**: 15-20 hours

**Files to Modify**:
- AppSettings.cs
- ModelConfig.cs (remove hotkey properties)
- SettingsForm.Designer.cs (new tab)
- SettingsForm.cs (hotkey management)
- Form1.cs (RegisterHotkeys refactor)

---

### Phase 3: Model Presets System -> **HIGH PRIORITY**
**Problem**: Users must manually configure each model

**Solution**: Pre-defined and user-defined model templates

**New Data Model**:
```csharp
public class ModelPreset
{
    public string Name { get; set; }           // "OpenAI GPT-4o"
    public string Provider { get; set; }       // "OpenAI"
    public string Model { get; set; }          // "gpt-4o"
    public string Endpoint { get; set; }       // Default endpoint
    public string SystemPrompt { get; set; }   // Default prompt
    public bool IsUserDefined { get; set; }    // vs. built-in
}
```

**Built-in Presets**:
- OpenAI GPT-4o
- OpenAI GPT-4o-mini
- Anthropic Claude 3.5 Sonnet
- Anthropic Claude 3.5 Haiku
- Groq Llama 3.1 70B
- Groq Llama 3.1 8B
- Together AI (various)
- Local LLM (Ollama/LM Studio)

**UI Features**:
- "Add from Preset" button in Models tab
- Preset browser dialog
- Save current model as preset
- Import/Export presets
- Community preset sharing (future)

**Benefits**:
- One-click model addition
- Users can save their configurations
- Share configurations between installations
- Reduce setup friction

**Estimated Effort**: 8-10 hours

---

### Phase 4: Code Cleanup & Refactoring -> **HIGH PRIORITY**
**Problem**: Hardcoded values, some code duplication

**Goals**:
1. **Remove Hardcoding**:
 - Move magic strings to constants
 - Externalize configuration
 - Make everything user-configurable

2. **Improve Reusability**:
 - Extract common patterns
 - Create utility methods
 - Reduce code duplication

3. **Better Separation of Concerns**:
 - Service layer for business logic
 - UI layer for presentation
 - Clear interfaces between components

**See**: CODE-REVIEW.md for detailed audit

**Estimated Effort**: 10-15 hours

---

### Phase 5: Enhanced Features
**Priority**: Medium-Low

#### 5.1 Conversation History
- Track queries and responses
- View history in UI
- Re-use previous queries
- Export conversation logs

#### 5.2 Response Preview & Editing
- Preview response before output
- Edit response before typing
- Approve/reject AI suggestions

#### 5.3 Multi-Turn Conversations
- Maintain conversation context
- Continue previous conversation
- Context window management

#### 5.4 Streaming Responses
- Real-time token streaming
- Progressive output as tokens arrive
- Cancel mid-response

#### 5.5 Response Templates
- Pre-defined response formats
- Template variables
- Custom output transformations

#### 5.6 Hotkey Profiles
- Different hotkey sets for different workflows
- Quick profile switching
- Per-application hotkey profiles (advanced)

---

## Known Issues

### Critical
- None currently

### High Priority
- **Hotkey Conflicts**: Per-model hotkeys can conflict -> **Fixed in Phase 2**
- **Hardcoded Values**: Magic strings throughout code -> **Fixed in Phase 4**

### Medium Priority
- **No Undo**: Can't easily undo AI response output
- **No Response Preview**: Can't review before output

### Low Priority
- **API Key Storage**: Plaintext (consider encryption)
- **No Logging System**: Only Debug.WriteLine
- **No Error Recovery**: Crashes require restart

---

## Release Plan

### v1.0 (Stable Release)
**Target**: After Phase 2, 3, and 4 complete
**Requirements**:
- Core functionality stable
- Global hotkeys system
- Model presets
- Code cleanup complete
- Documentation complete
- Build instructions for Windows
- User guide complete

### v1.1 (Feature Release)
**Target**: After v1.0 + Phase 5.1-5.3
**Focus**: Enhanced user experience
- Conversation history
- Response preview
- Multi-turn conversations

### v1.2 (Advanced Release)
**Target**: After v1.1 + Phase 5.4-5.6
**Focus**: Power user features
- Streaming responses
- Templates
- Hotkey profiles

### v2.0 (Major Release)
**Target**: Future
**Scope**: Plugin system, cloud sync, collaboration features

---

## Development Priorities

### Immediate (Next Session):
1. -> Documentation cleanup (this file!)
2. -> Code review & hardcoding audit
3. -> Global hotkeys system (Phase 2)

### Short Term (Next 2-3 Sessions):
4. Model presets system (Phase 3)
5. Code cleanup & refactoring (Phase 4)
6. Comprehensive testing

### Medium Term:
7. Enhanced features (Phase 5.1-5.3)
8. Error handling & recovery

### Long Term:
10. Advanced features (Phase 5.4-5.6)
11. Plugin system
12. Community features

---

## Feature Requests

### From User Feedback:
- Custom API format support
- Per-model system prompts
- Vision + reasoning workflow
- Global hotkeys (in progress)
- Model presets (planned)

### Ideas for Future:
- Voice input integration
- OCR for screenshots
- Multi-language support
- Browser extension integration
- Mobile companion app
- Team/workspace sharing

---

## How to Contribute

See **CONTRIBUTING.md** for:
- Feature request process
- Bug reporting guidelines
- Pull request workflow
- Code style guidelines

---

## Version History

See **CHANGELOG.md** for detailed version history.

### Recent Changes:
- **2024-01**: v0.9-alpha
 - Multi-model support
 - Custom API format
 - Vision + reasoning workflow
 - Quick fixes and improvements
