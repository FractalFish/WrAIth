using System.Runtime.InteropServices;
using BLLMT.Interfaces;
using CoreGraphics;
using Foundation;
using AppKit;
using ImageIO;
using UniformTypeIdentifiers;

namespace BLLMT.Platforms.MacCatalyst
{
    /// <summary>
    /// macOS implementation of IScreenshotService using CGWindowListCreateImage
    /// Requires Screen Recording permissions
    /// </summary>
    public class MacScreenshotService : IScreenshotService
    {
        private CGPoint _startPoint;
        private bool _isCapturing = false;

        public event EventHandler<ScreenshotCapturedEventArgs>? ScreenshotCaptured;
        public bool IsCapturing => _isCapturing;

        [DllImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static extern IntPtr CGWindowListCreateImage(
            CGRect screenBounds,
            CGWindowListOption windowOption,
            uint windowID,
            CGWindowImageOption imageOption);

        public void StartCapture()
        {
            // Get current mouse position
            var mouseLocation = NSEvent.CurrentMouseLocation;
            
            // Convert from Cocoa coordinates (bottom-left origin) to screen coordinates
            var screen = NSScreen.MainScreen;
            _startPoint = new CGPoint(
                mouseLocation.X,
                screen.Frame.Height - mouseLocation.Y);
            
            _isCapturing = true;
            Log($"Screenshot capture started at ({_startPoint.X}, {_startPoint.Y})");
        }

        public void EndCapture()
        {
            if (!_isCapturing)
            {
                Log("EndCapture called but no capture in progress");
                return;
            }

            // Get current mouse position
            var mouseLocation = NSEvent.CurrentMouseLocation;
            var screen = NSScreen.MainScreen;
            var endPoint = new CGPoint(
                mouseLocation.X,
                screen.Frame.Height - mouseLocation.Y);

            Log($"Screenshot capture ended at ({endPoint.X}, {endPoint.Y})");

            // Calculate rectangle
            nfloat x = Math.Min(_startPoint.X, endPoint.X);
            nfloat y = Math.Min(_startPoint.Y, endPoint.Y);
            nfloat width = Math.Abs(endPoint.X - _startPoint.X);
            nfloat height = Math.Abs(endPoint.Y - _startPoint.Y);

            // Minimum size check
            if (width < 10 || height < 10)
            {
                Log($"Screenshot too small ({width}x{height}), ignoring");
                _isCapturing = false;
                return;
            }

            var captureRect = new CGRect(x, y, width, height);
            Log($"Capturing rectangle: X={x}, Y={y}, W={width}, H={height}");

            try
            {
                // Capture the screen region
                IntPtr imageRef = CGWindowListCreateImage(
                    captureRect,
                    CGWindowListOption.OnScreenOnly,
                    0, // All windows
                    CGWindowImageOption.Default);

                if (imageRef != IntPtr.Zero)
                {
                    using (var cgImage = new CGImage(imageRef))
                    {
                        // Convert to base64
                        string base64Image = ConvertToBase64(cgImage);
                        
                        Log($"Screenshot captured successfully: {base64Image.Length} bytes (base64)");

                        // Raise event
                        ScreenshotCaptured?.Invoke(this, new ScreenshotCapturedEventArgs(
                            base64Image,
                            (int)x, (int)y, (int)width, (int)height));
                    }

                    CFRelease(imageRef);
                }
                else
                {
                    Log("ERROR: Failed to capture screenshot");
                }
            }
            catch (Exception ex)
            {
                Log($"Error capturing screenshot: {ex.Message}");
            }
            finally
            {
                _isCapturing = false;
            }
        }

        public void CancelCapture()
        {
            if (_isCapturing)
            {
                Log("Screenshot capture cancelled");
                _isCapturing = false;
            }
        }

        private string ConvertToBase64(CGImage cgImage)
        {
            using (var data = new NSMutableData())
            {
                var imageDestination = CGImageDestination.Create(data, UTType.PNG, 1);
                if (imageDestination != null)
                {
                    imageDestination.AddImage(cgImage);
                    imageDestination.Close();
                    return data.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                }
            }

            throw new InvalidOperationException("Failed to convert image to base64");
        }

        [DllImport("/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation")]
        private static extern void CFRelease(IntPtr cf);

        private void Log(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            System.Diagnostics.Debug.WriteLine($"[{timestamp}] [MacScreenshotService] {message}");
            Console.WriteLine($"[{timestamp}] [MacScreenshotService] {message}");
        }
    }
}
