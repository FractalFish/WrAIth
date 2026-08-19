# WrAIth - Background LLM Typing Assistant

A background Windows application that listens for hotkey inputs, processes clipboard content through an LLM (Large Language Model), and outputs responses at a natural, configurable pace.


> **Responsible Use**: WrAIth is a productivity and accessibility tool. Use it only in contexts where AI assistance is permitted. See [TERMS.md](TERMS.md) for full usage terms.

## Features

- **Background Operation**: Runs invisible in the system tray
- **Configurable Hotkeys**:
 - Trigger hotkey: Reads clipboard and sends to LLM
 - Output hotkey: Types the LLM response character-by-character
 - Abort hotkey: Cancels the current operation
- **LLM Integration**: Supports OpenAI, Anthropic, Groq, and custom APIs
- **Smooth Output**: Types responses at a natural, configurable pace rather than pasting all at once
- **Screenshot Support**: Capture screen regions and send to vision models
- **Model Chaining**: Chain multiple models together (e.g., vision -> reasoning)
- **Multi-Model Support**: Configure multiple models with different hotkeys
- **Native Experience**: System tray integration with NotifyIcon, balloon tip notifications, Windows Forms settings dialog, and direct Win32 API for hotkeys and keyboard simulation

## Quick Start

1. Download `Wraith-windows.zip` from [Releases](https://github.com/yourusername/wraith/releases)
2. Extract and run `Wraith.exe`
3. Configure settings via system tray icon
4. Start using!

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

Right-click the system tray icon and select **Settings** to configure:

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

- Download `Wraith-windows.zip`
- Extract to any folder
- Run `Wraith.exe`

### From Source

```bash
# Clone repository
git clone https://github.com/yourusername/wraith.git
cd wraith

dotnet run -f net10.0-windows

# Build for distribution
dotnet publish -f net10.0-windows -c Release
```

## Requirements

- .NET 10.0 runtime (or use self-contained build)
- Windows 10 or later
- No special permissions needed

## Notes

- Hotkey changes require restarting the application
- Settings are stored in `%APPDATA%\Wraith\settings.json`
- The application must be running for hotkeys to work
- Ensure your API key has sufficient credits/quota

## Troubleshooting

- Restart the application after changing hotkey settings
- Check if another application is using the same hotkey combination
- Run the application as administrator if needed

## Responsible Use

WrAIth is designed for legitimate productivity and accessibility use cases — helping users who benefit from AI-assisted drafting, people with motor impairments, or anyone who wants a faster personal writing workflow.

Using this tool to cheat in job interviews, academic exams, or any context where AI assistance is prohibited is a violation of the [Terms of Use](TERMS.md) and solely the user's responsibility. The author(s) do not condone or support such use.

## Architecture

### Shared Components
- **AppSettings.cs**: Configuration management
- **LLMService.cs**: LLM API integration (OpenAI, Anthropic, etc.)
- **ModelConfig.cs**: Multi-model configuration
- **Services/PlatformServiceFactory.cs**: Platform abstraction factory

### Windows Components (Platforms/Windows/)
- **Form1.cs**: Main Windows Forms application
- **SettingsForm.cs**: Settings UI
- **HotkeyManager.cs**: Win32 hotkey registration
- **KeyboardSimulator.cs**: Win32 keyboard simulation
- **KeyboardHook.cs**: Low-level keyboard hook
- **ScreenshotService.cs**: Screen capture via GDI+

### Platform Abstraction (Interfaces/)
- **IHotkeyManager**: Global hotkey registration
- **IKeyboardSimulator**: Keyboard input simulation
- **IKeyboardHook**: Keystroke interception
- **IScreenshotService**: Screen capture
- **IMenuBarManager**: System tray/menu bar

## Documentation

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
- OpenAI, Anthropic, and other LLM APIs

---

**Made with -> by the WrAIth community**
