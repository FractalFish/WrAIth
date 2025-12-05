using BLLMT.Interfaces;
using System.Drawing;

namespace BLLMT.Platforms.Windows
{
    /// <summary>
    /// Windows implementation wrapper for existing ScreenshotService
    /// Adapts Windows Forms Rectangle to platform-agnostic coordinates
    /// </summary>
    public class WindowsScreenshotService : IScreenshotService
    {
        private readonly ScreenshotService _screenshotService;

        public event EventHandler<ScreenshotCapturedEventArgs>? ScreenshotCaptured;
        
        public bool IsCapturing => _screenshotService.IsCapturing;

        public WindowsScreenshotService()
        {
            _screenshotService = new ScreenshotService();
            _screenshotService.ScreenshotCaptured += OnScreenshotCaptured;
        }

        private void OnScreenshotCaptured(object? sender, ScreenshotService.ScreenshotCapturedEventArgs e)
        {
            // Convert Windows-specific event args to platform-agnostic version
            var args = new ScreenshotCapturedEventArgs(
                e.Base64Image,
                e.CaptureArea.X,
                e.CaptureArea.Y,
                e.CaptureArea.Width,
                e.CaptureArea.Height);
            
            ScreenshotCaptured?.Invoke(this, args);
        }

        public void StartCapture()
        {
            _screenshotService.StartCapture();
        }

        public void EndCapture()
        {
            _screenshotService.EndCapture();
        }

        public void CancelCapture()
        {
            _screenshotService.CancelCapture();
        }
    }
}
