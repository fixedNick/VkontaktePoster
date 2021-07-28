using SeleniumDriver;
using System;

namespace VkontaktePoster
{
    /// <summary>
    /// Settings to start new SeleniumDriver
    /// Usually used in Marionette.cs
    /// </summary>
    class DriverSettings
    {
        public readonly Driver.DriverType SeleniumDriverType;
        public readonly Driver.NotificationDelegate DriverNotificationDelegate;
        public readonly Boolean StartMaximized;
        public readonly Boolean Headless;
        public readonly Boolean HidePrompt;
        public readonly Boolean ShowExceiptions;
        public readonly String DriverFileName;

        public DriverSettings(Driver.DriverType type, Driver.NotificationDelegate notificationHandler, bool startMaximized, bool headless, bool hidePrompt, bool showExceptions, string driverName = "chromedriver.exe")
        {
            SeleniumDriverType = type;
            DriverNotificationDelegate = notificationHandler;
            StartMaximized = startMaximized;
            Headless = headless;
            HidePrompt = hidePrompt;
            ShowExceiptions = showExceptions;
            DriverFileName = driverName;
        }
    }
}
