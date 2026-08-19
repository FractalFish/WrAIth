using Wraith.Services;

namespace Wraith
{
#if WINDOWS
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the Windows application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.SetUnhandledExceptionMode(System.Windows.Forms.UnhandledExceptionMode.CatchException);
            System.Windows.Forms.Application.ThreadException += (sender, e) => HandleFatalException(e.Exception);
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => HandleFatalException(e.ExceptionObject as Exception);

            // Windows Forms application
            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(new Form1());
        }

        private static void HandleFatalException(Exception? ex)
        {
            Logger.LogError("Unhandled exception - application will exit", ex);
            System.Windows.Forms.MessageBox.Show(
                $"Wraith hit an unexpected error and needs to close.\n\n{ex?.Message}\n\nDetails were written to %APPDATA%\\Wraith\\error.log",
                "Wraith - Unexpected Error",
                System.Windows.Forms.MessageBoxButtons.OK,
                System.Windows.Forms.MessageBoxIcon.Error);
        }
    }
#else
    internal static class Program
    {
        static void Main()
        {
            throw new PlatformNotSupportedException("This platform is not supported. Build with -f net10.0-windows");
        }
    }
#endif
}
