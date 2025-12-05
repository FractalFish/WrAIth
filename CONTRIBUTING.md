# Contributing to BLLMT

Thank you for your interest in contributing to BLLMT! This document provides guidelines and instructions for contributing.

## ?? Getting Started

1. **Fork the repository** on GitHub
2. **Clone your fork** locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/bllmt.git
   cd bllmt
   ```
3. **Create a branch** for your feature or fix:
   ```bash
   git checkout -b feature/my-new-feature
   ```

## ??? Development Setup

### Prerequisites

**Windows:**
- Visual Studio 2022 (or later) with .NET 10 workload
- .NET 10 SDK
- Git

**macOS:**
- Xcode 14+ (from App Store)
- .NET 10 SDK
- Git

### Building

**Windows:**
```powershell
dotnet restore
dotnet build -f net10.0-windows
dotnet run -f net10.0-windows
```

**macOS:**
```bash
dotnet restore
dotnet workload install maui-maccatalyst
dotnet build -f net10.0-maccatalyst
dotnet run -f net10.0-maccatalyst
```

## ?? How to Contribute

### Reporting Bugs

1. **Check existing issues** to avoid duplicates
2. **Create a new issue** with:
   - Clear, descriptive title
   - Platform (Windows/macOS) and version
   - Steps to reproduce
   - Expected vs actual behavior
   - Screenshots/logs if applicable
   - .NET version (`dotnet --version`)

### Suggesting Features

1. **Check existing issues** for similar requests
2. **Create a new issue** labeled "enhancement" with:
   - Clear description of the feature
   - Use case / why it's needed
   - Proposed implementation (optional)
   - Platform considerations (Windows/macOS/both)

### Submitting Pull Requests

1. **Create an issue first** (unless it's a trivial fix)
2. **Follow the code style** (see below)
3. **Test on both platforms** if possible
4. **Update documentation** if needed
5. **Write clear commit messages**:
   ```
   Add screenshot capture for Linux
   
   - Implement X11 screen capture using libX11
   - Add Wayland support via wlr-screencopy
   - Update README with Linux instructions
   ```
6. **Submit the PR** with:
   - Reference to the issue it fixes
   - Description of changes
   - Testing done (platforms, scenarios)
   - Screenshots for UI changes

## ?? Code Style

### General Guidelines

- **Use C# conventions** (PascalCase for public, camelCase for private)
- **Follow existing patterns** in the codebase
- **Add XML comments** for public APIs
- **Use meaningful names** for variables and methods
- **Keep methods focused** (single responsibility)
- **Avoid platform-specific code** outside `Platforms/` folders

### Platform Abstraction

When adding platform-specific functionality:

1. **Create an interface** in `Interfaces/`:
   ```csharp
   public interface IMyService
   {
       void DoSomething();
   }
   ```

2. **Implement for each platform**:
   - `Platforms/Windows/WindowsMyService.cs`
   - `Platforms/MacCatalyst/MacMyService.cs`

3. **Register in PlatformServiceFactory**:
   ```csharp
   public static IMyService CreateMyService()
   {
       #if WINDOWS
           return new WindowsMyService();
       #elif MACCATALYST
           return new MacMyService();
       #endif
   }
   ```

### Code Organization

```
BLLMT/
??? Interfaces/          # Platform abstractions
??? Services/            # Shared business logic
??? Platforms/
?   ??? Windows/        # Windows-specific implementations
?   ??? MacCatalyst/    # macOS-specific implementations
??? Models/             # Data models
??? *.cs               # Shared code
```

### Testing

- **Test on your platform** before submitting
- **Note which platforms tested** in PR description
- **Include edge cases** in testing
- **Verify error handling** works properly

## ?? Documentation

Update documentation when adding features:

- **README.md** - For user-facing features
- **Code comments** - For complex logic
- **XML docs** - For public APIs
- **SETUP-GUIDE.md** - For setup changes
- **Platform-specific READMEs** - For platform-specific features

## ?? Review Process

1. **Automated checks** run on PR submission
2. **Maintainer review** (usually within 1-2 days)
3. **Address feedback** if any changes requested
4. **Merge** once approved

## ?? Priority Issues

Looking for something to work on? Check issues labeled:
- `good first issue` - Great for new contributors
- `help wanted` - We need community help
- `bug` - Something broken
- `enhancement` - New features

## ?? Recognition

Contributors are recognized in:
- GitHub contributors list
- Release notes (for significant contributions)
- README (for major features)

## ?? Code of Conduct

### Our Standards

- **Be respectful** and inclusive
- **Welcome newcomers** and help them learn
- **Focus on the code** not the person
- **Accept constructive criticism** gracefully
- **Show empathy** towards other contributors

### Unacceptable Behavior

- Harassment or discriminatory comments
- Personal attacks or trolling
- Publishing private information
- Any conduct that would be inappropriate in a professional setting

## ?? Questions?

- **General questions**: Open an issue with the "question" label
- **Security issues**: Email [maintainer email] directly
- **Platform-specific help**: Check platform-specific README files

## ?? Roadmap

Interested in major features? Check our roadmap:

### High Priority
- [ ] Linux support
- [ ] Cross-platform settings UI (MAUI)
- [ ] Auto-update functionality

### Medium Priority
- [ ] Plugin system
- [ ] Model marketplace
- [ ] Cloud settings sync

### Low Priority
- [ ] Mobile support (iOS/Android)
- [ ] Browser extension

Want to work on something not listed? Propose it in an issue!

## ?? License

By contributing, you agree that your contributions will be licensed under the MIT License.

---

**Thank you for contributing to BLLMT!** ??

Every contribution, no matter how small, helps make BLLMT better for everyone.
