# Quick Reference: Building & Running Wraith

## Quick Commands

```powershell
# Development
dotnet run -f net10.0-windows

# Release Build
dotnet publish -f net10.0-windows -c Release

# Self-Contained (includes .NET)
dotnet publish -f net10.0-windows -c Release --self-contained -r win-x64
```

---

## Common Tasks

### Add a New Platform Service

1. Define interface in `Interfaces/`
2. Implement in `Platforms/Windows/`
3. Add factory method in `Services/PlatformServiceFactory.cs`

### View Logs

```powershell
# Visual Studio Output window
# Or use Debug Viewer
```

---

## Debug Tricks

- Run in Visual Studio with F5
- Breakpoints work normally
- Output window shows logs

### Platform-Specific Code

```csharp
#if WINDOWS
    // Windows-only code
#else
    // Fallback
#endif
```

### Check Platform at Runtime

```csharp
using Wraith.Services;

if (PlatformServiceFactory.IsWindows())
{
    // Windows-specific logic
}
```

---

## Distribution Checklist

- [ ] Build release: `dotnet publish -f net10.0-windows -c Release`
- [ ] Test on clean Windows machine
- [ ] Verify hotkeys work
- [ ] Check API calls succeed
- [ ] Test typing emulation
- [ ] Zip the publish folder
- [ ] Create GitHub release
- [ ] Document system requirements

---

## UI Customization

### Change System Tray Icon

1. Add icon to project
2. Update `Form1.Designer.cs`:
```csharp
this.notifyIcon.Icon = new System.Drawing.Icon("path/to/icon.ico");
```

---

## File Sizes

- Framework-dependent: ~5-10 MB
- Self-contained: ~70-80 MB
- Single-file: ~75-85 MB

---

## Emergency Fixes

### Hotkeys Stop Working

```powershell
# Restart app
# Try different hotkey combination
# Run as administrator (if needed)
```

---

## Pro Tips

### Development
- Use `#if DEBUG` for debug-only code
- Log platform in startup: `Log($"Platform: {PlatformServiceFactory.GetPlatformName()}")`

### Performance
- Use `Invoke` for UI thread access
- Async/await for API calls

### User Experience
- Show progress for long operations
- Clear error messages with solutions
- Save settings immediately
- Restore state on restart

---

## Getting Help

1. Check logs (see "View Logs" above)
2. Search [Issues](https://github.com/FractalFish/WrAIth/issues)
3. Review documentation
4. Ask in [Discussions](https://github.com/FractalFish/WrAIth/discussions)
5. Create new issue with:
 - .NET version
 - Windows version
 - Steps to reproduce
 - Error logs

---

**Happy Coding!**
