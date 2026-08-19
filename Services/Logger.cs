namespace Wraith.Services
{
    /// <summary>
    /// Minimal file logger for crashes and errors, since Debug.WriteLine goes nowhere
    /// in a Release build. Best-effort only: logging failures must never throw.
    /// </summary>
    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Wraith",
            "error.log");

        private static readonly object LockObj = new object();

        public static void LogError(string message, Exception? ex = null)
        {
            try
            {
                lock (LockObj)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(LogPath)!);
                    string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
                    if (ex != null)
                    {
                        entry += Environment.NewLine + ex;
                    }
                    entry += Environment.NewLine;
                    File.AppendAllText(LogPath, entry);
                }
            }
            catch
            {
                // Logging must never crash the app it's trying to diagnose.
            }
        }
    }
}
