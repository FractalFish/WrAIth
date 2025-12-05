namespace BLLMT.Interfaces
{
    /// <summary>
    /// Platform-specific screenshot service interface
    /// </summary>
    public interface IScreenshotService
    {
        /// <summary>
        /// Event fired when a screenshot is captured
        /// </summary>
        event EventHandler<ScreenshotCapturedEventArgs>? ScreenshotCaptured;
        
        /// <summary>
        /// Start screenshot capture (mark starting point)
        /// </summary>
        void StartCapture();
        
        /// <summary>
        /// End screenshot capture and capture the region
        /// </summary>
        void EndCapture();
        
        /// <summary>
        /// Cancel an in-progress screenshot capture
        /// </summary>
        void CancelCapture();
        
        /// <summary>
        /// Indicates if a capture is in progress
        /// </summary>
        bool IsCapturing { get; }
    }
    
    /// <summary>
    /// Event args for screenshot captured events
    /// </summary>
    public class ScreenshotCapturedEventArgs : EventArgs
    {
        public string Base64Image { get; }
        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }

        public ScreenshotCapturedEventArgs(string base64Image, int x, int y, int width, int height)
        {
            Base64Image = base64Image;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
