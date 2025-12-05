using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BLLMT
{
    public class ScreenshotService
    {
        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private System.Drawing.Point? _startPoint = null;
        private bool _isCapturing = false;

        public event EventHandler<ScreenshotCapturedEventArgs>? ScreenshotCaptured;

        public void StartCapture()
        {
            GetCursorPos(out POINT cursorPos);
            _startPoint = new System.Drawing.Point(cursorPos.X, cursorPos.Y);
            _isCapturing = true;
            Log($"Screenshot capture started at ({_startPoint.Value.X}, {_startPoint.Value.Y})");
        }

        public void EndCapture()
        {
            if (!_isCapturing || _startPoint == null)
            {
                Log("EndCapture called but no capture in progress");
                return;
            }

            GetCursorPos(out POINT cursorPos);
            System.Drawing.Point endPoint = new System.Drawing.Point(cursorPos.X, cursorPos.Y);
            
            Log($"Screenshot capture ended at ({endPoint.X}, {endPoint.Y})");

            // Calculate rectangle (handle any direction of drag)
            int x = Math.Min(_startPoint.Value.X, endPoint.X);
            int y = Math.Min(_startPoint.Value.Y, endPoint.Y);
            int width = Math.Abs(endPoint.X - _startPoint.Value.X);
            int height = Math.Abs(endPoint.Y - _startPoint.Value.Y);

            // Minimum size check
            if (width < 10 || height < 10)
            {
                Log($"Screenshot too small ({width}x{height}), ignoring");
                _isCapturing = false;
                _startPoint = null;
                return;
            }

            Rectangle captureRect = new Rectangle(x, y, width, height);
            Log($"Capturing rectangle: X={x}, Y={y}, W={width}, H={height}");

            try
            {
                // Capture the screen region
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(x, y, 0, 0, new System.Drawing.Size(width, height), CopyPixelOperation.SourceCopy);
                    }

                    // Convert to base64 for API
                    string base64Image = ConvertToBase64(bitmap);
                    
                    Log($"Screenshot captured successfully: {base64Image.Length} bytes (base64)");

                    // Raise event with the captured screenshot
                    ScreenshotCaptured?.Invoke(this, new ScreenshotCapturedEventArgs(base64Image, captureRect));
                }
            }
            catch (Exception ex)
            {
                Log($"Error capturing screenshot: {ex.Message}");
            }
            finally
            {
                _isCapturing = false;
                _startPoint = null;
            }
        }

        public void CancelCapture()
        {
            if (_isCapturing)
            {
                Log("Screenshot capture cancelled");
                _isCapturing = false;
                _startPoint = null;
            }
        }

        public bool IsCapturing => _isCapturing;

        private string ConvertToBase64(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Save as PNG for best quality
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                byte[] imageBytes = ms.ToArray();
                return Convert.ToBase64String(imageBytes);
            }
        }

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [ScreenshotService] {message}");
            Console.WriteLine($"[{timestamp}] [ScreenshotService] {message}");
        }

        public class ScreenshotCapturedEventArgs : EventArgs
        {
            public string Base64Image { get; }
            public Rectangle CaptureArea { get; }

            public ScreenshotCapturedEventArgs(string base64Image, Rectangle captureArea)
            {
                Base64Image = base64Image;
                CaptureArea = captureArea;
            }
        }
    }
}
