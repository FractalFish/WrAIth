# Changelog

All notable changes to Wraith will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Planned
- Global hotkeys system (hotkey -> model + action mapping)
- Model presets (built-in and user-defined)
- Code cleanup and refactoring
- Remove hardcoded values
- Conversation history
- Response preview and editing

---

## [0.9.0-alpha] - 2025-01-XX

### Added
- Multi-model support with per-model configuration
- Per-model system prompts
- Per-model hotkeys
- Custom API format system with JSON templates
- Custom API format editor GUI
- Model enable/disable toggle
- Automatic vision capability detection
- Test model connection feature
- Two-stage vision + reasoning workflow
- Interactive screenshot selection with overlay
- Vision model support (OpenAI, Anthropic)
- Model chaining feature
- Per-model clipboard image detection
- API key masking in UI

### Changed
- Renamed "Trigger" hotkey -> "Send Query" for clarity
- Renamed "Append Vision" -> "Vision + Reasoning" for clarity
- Removed "Supports Vision" checkbox (now auto-detected)
- Simplified model testing (always uses text)
- Provider dropdown now includes "Custom" option
- System prompts now per-model instead of global

### Removed
- Global "Supports Vision" checkbox (redundant)
- "Analyze Screenshot" hotkey (dead code, never implemented)
- Duplicate "Shutdown" button from system tray menu
- 28 obsolete documentation files (consolidated)

### Fixed
- Label overlap issue ("Vision + Reasoning" label width)
- Models list visibility (removed custom draw mode temporarily)
- Custom Format button now properly wired up
- Vision model detection logic

---

## [0.8.0-alpha] - 2025-01-XX

### Added
- Basic multi-model support
- Per-model API keys
- Per-model endpoints
- Model management UI (add, remove, set default)

### Changed
- Migrated from single model to multi-model architecture
- Settings form redesigned with tabs

---

## [0.7.0-alpha] - 2024-12-XX

### Added
- Screenshot capture with region selection
- Vision model integration
- Base64 image encoding for vision APIs
- Screenshot start/end hotkeys
- Vision result storage for reasoning stage

---

## [0.6.0-alpha] - 2024-12-XX

### Added
- Anthropic (Claude) API support
- Provider selection (OpenAI, Anthropic, Groq)
- Provider-specific request formatting
- Provider-specific authentication headers

---

## [0.5.0-alpha] - 2024-11-XX

### Added
- Keyboard emulation with realistic typing delays
- Configurable typing speed and variation
- Keystroke-by-keystroke replacement
- Pause/resume emulation
- Abort hotkey

---

## [0.4.0-alpha] - 2024-11-XX

### Added
- Global hotkey registration (Windows)
- Hotkey manager service
- Low-level keyboard hook for interception
- Settings form for configuration

---

## [0.3.0-alpha] - 2024-11-XX

### Added
- OpenAI API integration
- Text completion support
- Clipboard integration (read/write)
- Response queueing system

---

## [0.2.0-alpha] - 2024-10-XX

### Added
- Background service with system tray icon
- System tray context menu
- Application settings storage (JSON)
- macOS compatibility layer (partial)

---

## [0.1.0-alpha] - 2024-10-XX

### Added
- Initial project structure
- Basic WinForms application
- .NET 10 targeting
- Cross-platform foundation (Windows/macOS)

---

## Version Schema

- **Major.Minor.Patch**
 - **Major**: Breaking changes, major features
 - **Minor**: New features, non-breaking changes
 - **Patch**: Bug fixes, documentation

- **-alpha**: Early development, frequent changes
- **-beta**: Feature complete, testing phase
- **-rc**: Release candidate, final testing
- **(no suffix)**: Stable release

---

## Links

- [GitHub Repository](https://github.com/FractalFish/ReroRero)
- [Documentation](./README.md)
- [Roadmap](./ROADMAP.md)
- [Contributing](./CONTRIBUTING.md)
