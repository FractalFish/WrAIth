namespace Wraith.Interfaces
{
    /// <summary>
    /// Platform-specific menu bar/system tray manager
    /// </summary>
    public interface IMenuBarManager : IDisposable
    {
        /// <summary>
        /// Initialize the menu bar/system tray icon
        /// </summary>
        void Initialize();
        
        /// <summary>
        /// Show a notification
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <param name="message">Notification message</param>
        /// <param name="type">Notification type (Info, Warning, Error)</param>
        void ShowNotification(string title, string message, NotificationType type = NotificationType.Info);
        
        /// <summary>
        /// Update the status text in the menu
        /// </summary>
        /// <param name="status">Status text</param>
        void UpdateStatus(string status);
        
        /// <summary>
        /// Set the settings callback
        /// </summary>
        /// <param name="callback">Action to invoke when settings is clicked</param>
        void SetSettingsCallback(Action callback);
        
        /// <summary>
        /// Set the exit callback
        /// </summary>
        /// <param name="callback">Action to invoke when exit is clicked</param>
        void SetExitCallback(Action callback);
    }
    
    public enum NotificationType
    {
        Info,
        Warning,
        Error
    }
}
