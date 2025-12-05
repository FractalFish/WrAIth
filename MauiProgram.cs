using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using Microsoft.Maui.Controls.Hosting;

namespace BLLMT
{
    /// <summary>
    /// MAUI application entry point for cross-platform support
    /// This is used on macOS (Mac Catalyst)
    /// </summary>
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            return builder.Build();
        }
    }

    /// <summary>
    /// MAUI Application class
    /// </summary>
    public class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
#if MACCATALYST
            // On macOS, we don't use MainPage - we run as menu bar app
            MainPage = new ContentPage(); // Dummy page, won't be shown
            
            // Initialize macOS app
            Task.Run(async () => await InitializeMacOSApp());
#endif
        }

#if MACCATALYST
        private async Task InitializeMacOSApp()
        {
            try
            {
                // Check and request permissions on startup
                var permissionsGranted = await Platforms.MacCatalyst.PermissionHelper.CheckAndRequestPermissionsAsync();

                if (permissionsGranted)
                {
                    Platforms.MacCatalyst.PermissionHelper.ShowSuccessDialog();
                }

                // Load settings
                var settings = AppSettings.Load();
                
                // Initialize menu bar
                var menuBarManager = Services.PlatformServiceFactory.CreateMenuBarManager();
                menuBarManager.Initialize();
                
                // Initialize services
                var hotkeyManager = Services.PlatformServiceFactory.CreateHotkeyManager();
                var llmService = new LLMService(settings);
                var keyboardSimulator = Services.PlatformServiceFactory.CreateKeyboardSimulator(
                    settings.TypingDelayMs, 
                    settings.TypingVariationMs);
                var keyboardHook = Services.PlatformServiceFactory.CreateKeyboardHook();
                var screenshotService = Services.PlatformServiceFactory.CreateScreenshotService();

                // Set up menu bar callbacks
                menuBarManager.SetSettingsCallback(() =>
                {
                    // TODO: Show settings window
                    menuBarManager.ShowNotification("Settings", "Settings window coming soon!", NotificationType.Info);
                });

                menuBarManager.SetExitCallback(() =>
                {
                    hotkeyManager.Dispose();
                    keyboardHook.Dispose();
                    menuBarManager.Dispose();
                    Environment.Exit(0);
                });

                // Register hotkeys
                var defaultModel = settings.Models.FirstOrDefault(m => m.IsDefault) ?? settings.Models.FirstOrDefault();
                if (defaultModel != null)
                {
                    if (!string.IsNullOrEmpty(defaultModel.TriggerHotkey))
                    {
                        hotkeyManager.RegisterHotkey(defaultModel.TriggerHotkey, () =>
                        {
                            menuBarManager.ShowNotification("Trigger", "Processing clipboard...", NotificationType.Info);
                        });
                    }

                    if (!string.IsNullOrEmpty(defaultModel.OutputHotkey))
                    {
                        hotkeyManager.RegisterHotkey(defaultModel.OutputHotkey, () =>
                        {
                            menuBarManager.ShowNotification("Output", "Starting emulation...", NotificationType.Info);
                        });
                    }
                }

                menuBarManager.UpdateStatus("Ready - Hotkeys registered");
                menuBarManager.ShowNotification("BLLMT", "Application started successfully!", NotificationType.Info);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing macOS app: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
#endif
    }
}
