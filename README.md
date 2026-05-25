# BLLMT - Background LLM Typing Assistant

A **cross-platform** background application that listens for hotkey inputs, processes clipboard content through an LLM (Large Language Model), and outputs responses at a natural, configurable pace.


> **Responsible Use**: BLLMT is a productivity and accessibility tool. Use it only in contexts where AI assistance is permitted. See [TERMS.md](TERMS.md) for full usage terms.

## Features

- **Background Operation**: Runs invisible in the system tray (Windows) or menu bar (macOS)
- **Configurable Hotkeys**: 
  - Trigger hotkey: Reads clipboard and sends to LLM
  - Output hotkey: Types the LLM response character-by-character
  - Abort hotkey: Cancels the current operation
- **LLM Integration**: Supports OpenAI, Anthropic, Groq, and custom APIs
- **Smooth Output**: Types responses at a natural, configurable pace rather than pasting all at once
- **Screenshot Support**: Capture screen regions and send to vision models
- **Model Chaining**: Chain multiple models together (e.g., vision ? reasoning)
- **Multi-Model Support**: Configure multiple models with different hotkeys
- **Native Experience**: Uses platform-native UI on each OS

## Platform-Specific Features

### Windows
- System tray integration with NotifyIcon
- Balloon tip notifications
- Windows Forms settings dialog
- Direct Win32 API for hotkeys and keyboard simulation

### macOS
- Menu bar integration with NSStatusBar
- Notification Center notifications
- Automatic permission management
- Native macOS dialogs and UI
- Universal binary (Intel + Apple Silicon)

## ?? Quick Start

### Windows

1. Download `BLLMT-windows.zip` from [Releases](https://github.com/yourusername/bllmt/releases)
2. Extract and run `BLLMT.exe`
3. Configure settings via system tray icon
4. Start using!

### macOS

1. Download `BLLMT-mac.dmg` from [Releases](https://github.com/yourusername/bllmt/releases)
2. Open DMG and drag BLLMT to Applications
3. Launch BLLMT (right-click ? Open first time)
4. **Follow permission prompts** - BLLMT guides you through:
   - ? Accessibility (for hotkeys)
   - ? Screen Recording (for screenshots)
5. Configure settings via menu bar icon
6. Start using!

**macOS Setup Video:** [Coming soon]

## Usage


### Basic Workflow

1. Copy text to clipboard
2. Press the **Trigger Hotkey** (default: `Ctrl+Shift+Q`)
   - Application reads clipboard content
   - Sends to configured LLM with your system prompt
   - Response is queued
3. Position cursor where you want the text
4. Press the **Output Hotkey** (default: `Ctrl+Shift+W`)
   - Application types the response character-by-character
5. Press the **Abort Hotkey** (default: `Ctrl+Shift+E`) to cancel typing

### Configuration

Right-click the system tray icon (Windows) or menu bar icon (macOS) and select **Settings** to configure:

#### API Settings
- **API Provider**: Select OpenAI, Anthropic, or Custom
- **API Key**: Your LLM provider API key
- **Model**: Model name (e.g., `gpt-4o-mini`, `claude-3-5-sonnet-20241022`)
- **API Endpoint**: API endpoint URL
- **System Prompt**: Instructions for how the LLM should respond

#### Hotkey Settings
- **Trigger Hotkey**: Hotkey to process clipboard (e.g., `Control+Shift+Q`)
- **Output Hotkey**: Hotkey to start typing response (e.g., `Control+Shift+W`)
- **Abort Hotkey**: Hotkey to cancel operation (e.g., `Control+Shift+E`)

#### Typing Settings
- **Typing Delay**: Base delay between characters in milliseconds (default: 50ms)
- **Typing Variation**: Random variation in typing delay (default: 20ms)

### API Configuration Examples

#### OpenAI
- Provider: `OpenAI`
- Model: `gpt-4o-mini` or `gpt-4o`
- Endpoint: `https://api.openai.com/v1/chat/completions`
- API Key: Your OpenAI API key

#### Anthropic
- Provider: `Anthropic`
- Model: `claude-3-5-sonnet-20241022`
- Endpoint: `https://api.anthropic.com/v1/messages`
- API Key: Your Anthropic API key

#### Custom (OpenAI-compatible)
- Provider: `Custom`
- Model: Your model name
- Endpoint: Your API endpoint
- API Key: Your API key

## Installation

### From Releases (Recommended)

**Windows:**
- Download `BLLMT-windows.zip`
- Extract to any folder
- Run `BLLMT.exe`

**macOS:**
- Download `BLLMT-mac.dmg`
- Open and drag to Applications
- Run from Applications folder

### From Source

```bash
# Clone repository
git clone https://github.com/yourusername/bllmt.git
cd bllmt

# Windows
dotnet run -f net10.0-windows

# macOS
dotnet run -f net10.0-maccatalyst

# Build for distribution
dotnet publish -f net10.0-windows -c Release    # Windows
dotnet publish -f net10.0-maccatalyst -c Release # macOS
```

### Via Homebrew (macOS only)

```bash
brew tap yourusername/bllmt
brew install bllmt
```

## Requirements

### Windows
- ? .NET 10.0 runtime (or use self-contained build)
- ? Windows 10 or later
- ? No special permissions needed

### macOS
- ? .NET 10.0 runtime (or use self-contained build)
- ? macOS 11.0 (Big Sur) or later
- ? Accessibility permission (for hotkeys)
- ? Screen Recording permission (for screenshots)

**Note:** macOS app automatically requests permissions with helpful dialogs!

## Notes

- Hotkey changes require restarting the application
- Settings are stored in:
  - Windows: `%APPDATA%\BLLMT\settings.json`
  - macOS: `~/Library/Application Support/BLLMT/settings.json`
- The application must be running for hotkeys to work
- Ensure your API key has sufficient credits/quota

## Troubleshooting

**Windows:**
- Restart the application after changing hotkey settings
- Check if another application is using the same hotkey combination
- Run the application as administrator if needed

**macOS:**
- **Hotkeys not working?** Grant Accessibility permission in System Settings > Privacy & Security > Accessibility
- **Screenshots not working?** Grant Screen Recording permission in System Settings > Privacy & Security > Screen Recording
- **App won't launch?** Run: `xattr -dr com.apple.quarantine /Applications/BLLMT.app`
- **Permission issues?** Run the included setup script: `bash Scripts/setup-macos.sh`

See [README-macOS.md](README-macOS.md) for detailed macOS troubleshooting.


## Responsible Use

BLLMT is designed for legitimate productivity and accessibility use cases — helping users who benefit from AI-assisted drafting, people with motor impairments, or anyone who wants a faster personal writing workflow.

Using this tool to cheat in job interviews, academic exams, or any context where AI assistance is prohibited is a violation of the [Terms of Use](TERMS.md) and solely the user's responsibility. The author(s) do not condone or support such use.
## Architecture

The application now uses a **cross-platform architecture**:

### Shared Components
- **AppSettings.cs**: Configuration management
- **LLMService.cs**: LLM API integration (OpenAI, Anthropic, etc.)
- **ModelConfig.cs**: Multi-model configuration
- **Services/PlatformServiceFactory.cs**: Platform abstraction factory

### Platform-Specific Components

**Windows (Platforms/Windows/):**
- **Form1.cs**: Main Windows Forms application
- **SettingsForm.cs**: Settings UI
- **HotkeyManager.cs**: Win32 hotkey registration
- **KeyboardSimulator.cs**: Win32 keyboard simulation
- **KeyboardHook.cs**: Low-level keyboard hook
- **ScreenshotService.cs**: Screen capture via GDI+

**macOS (Platforms/MacCatalyst/):**
- **MauiProgram.cs**: MAUI application entry
- **MacHotkeyManager.cs**: CGEventTap hotkey registration
- **MacKeyboardSimulator.cs**: CGEventPost keyboard simulation
- **MacKeyboardHook.cs**: CGEventTap keyboard monitoring
- **MacScreenshotService.cs**: Screen capture via CGWindowList
- **MacMenuBarManager.cs**: NSStatusBar menu bar integration
- **PermissionHelper.cs**: Automatic permission management

### Platform Abstraction (Interfaces/)
- **IHotkeyManager**: Global hotkey registration
- **IKeyboardSimulator**: Keyboard input simulation
- **IKeyboardHook**: Keystroke interception
- **IScreenshotService**: Screen capture
- **IMenuBarManager**: System tray/menu bar

This architecture allows the same business logic to run on both platforms while using native APIs for the best performance and user experience.

## Documentation

- [SETUP-GUIDE.md](SETUP-GUIDE.md) - Comprehensive setup for both platforms
- [README-macOS.md](README-macOS.md) - macOS-specific information
- [DISTRIBUTION-STRATEGY.md](DISTRIBUTION-STRATEGY.md) - Distribution options
- [TERMS.md](TERMS.md) - Terms of Use


## Related Projects

- **[WrAIth Extension](https://github.com/FractalFish/WrAIth-Extension)** - Browser companion for WrAIth. Same hotkey workflow, directly in Chrome.
## License

MIT License - see [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## Credits

Built with:
- [.NET 10](https://dotnet.microsoft.com/)
- [.NET MAUI](https://dotnet.microsoft.com/apps/maui) (for macOS)
- OpenAI, Anthropic, and other LLM APIs

---

**Made with ?? by the BLLMT community**
