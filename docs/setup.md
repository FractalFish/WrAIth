# Wraith - Setup Guide

## Quick Start

```powershell
# Build and run
dotnet run -f net10.0-windows

# Or publish
dotnet publish -f net10.0-windows -c Release
```

---

## Architecture Overview

```
+---------------------------------------+
|         Your Application             |
|         (Form1, Services)            |
+---------------------------------------+
                |
+---------------------------------------+
|     Platform Interfaces              |
|  (IHotkeyManager, IKeyboard, etc)    |
+---------------------------------------+
                |
+---------------------------------------+
|              Windows Impl            |
+---------------------------------------+
```

### Key Components

1. **Interfaces/** - Platform-agnostic service contracts
2. **Services/** - Shared business logic & platform factory
3. **Platforms/Windows/** - Windows-specific implementations

---

## Dependencies

### Windows (.NET 10.0-windows)
- Windows Forms
- System.Drawing
- Native Win32 APIs (user32.dll, kernel32.dll)

---

## Development Setup

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 (17.8+) or VS Code
- Windows 10/11

### Clone and Build

```bash
git clone https://github.com/yourusername/wraith.git
cd wraith

dotnet build -f net10.0-windows
```

---

## First Run Experience

1. Launch Wraith.exe
2. Icon appears in system tray
3. Configure settings (API key, hotkeys)
4. Start using immediately!

**No permissions needed** - Windows allows hotkey registration by default.

---

## Building for Distribution

```powershell
# Self-contained (includes .NET runtime)
dotnet publish -f net10.0-windows -c Release -r win-x64 --self-contained

# Framework-dependent (smaller, requires .NET 10)
dotnet publish -f net10.0-windows -c Release -r win-x64 --no-self-contained

# Output: bin/Release/net10.0-windows/win-x64/publish/
```

**Result:** Single executable, ~5MB (framework-dependent) or ~70MB (self-contained)

---

## Testing

```powershell
dotnet test -f net10.0-windows
```

### Manual Testing Checklist

- [ ] App launches successfully
- [ ] Settings can be opened and saved
- [ ] API key is stored securely
- [ ] Hotkeys are registered
- [ ] Clipboard processing works
- [ ] LLM API calls succeed
- [ ] Typing emulation works
- [ ] Screenshot capture works
- [ ] Notifications appear
- [ ] App exits cleanly

---

## Configuration

Settings location: `%APPDATA%\Wraith\settings.json`

### Example settings.json

```json
{
  "Models": [
    {
      "Id": "...",
      "Name": "GPT-4o Mini",
      "Provider": "OpenAI",
      "ApiKey": "sk-...",
      "Model": "gpt-4o-mini",
      "Endpoint": "https://api.openai.com/v1/chat/completions",
      "SystemPrompt": "You are a helpful assistant.",
      "SupportsVision": false,
      "IsDefault": true,
      "TriggerHotkey": "Control+Shift+Q",
      "OutputHotkey": "Control+Shift+W",
      "AbortHotkey": "Control+Shift+E"
    }
  ],
  "TypingDelayMs": 50,
  "TypingVariationMs": 20
}
```

---

## Troubleshooting

**Hotkeys not working:**
- Check if another app is using the same hotkey
- Try running as administrator
- Restart the application

**Typing not working:**
- Ensure target app accepts input
- Check keyboard simulator settings
- Verify output hotkey is correct

---

## Contributing

1. Fork the repository
2. Create your feature branch
3. Test on Windows
4. Submit a pull request

### Code Style
- Use interfaces for platform-specific code
- Put platform code in `Platforms/Windows/`
- Shared logic goes in `Services/`
- Follow existing naming conventions

---

## License

[Your License Here]

---

## What's Next?

- [ ] Cloud sync for settings
- [ ] Model chaining improvements
- [ ] Plugin system

---

**Built with .NET 10.**
