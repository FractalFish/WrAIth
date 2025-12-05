namespace BLLMT
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
            // Windows Forms application
            ApplicationConfiguration.Initialize();
            System.Windows.Forms.Application.Run(new Form1());
        }
    }
#elif MACCATALYST
    public class Program
    {
        /// <summary>
        /// The main entry point for the macOS application.
        /// </summary>
        static void Main(string[] args)
        {
            // MAUI application for macOS
            var app = MauiProgram.CreateMauiApp();
            app.Run();
        }
    }
#else
    internal static class Program
    {
        static void Main()
        {
            throw new PlatformNotSupportedException("This platform is not supported. Build with -f net10.0-windows or -f net10.0-maccatalyst");
        }
    }
#endif
}