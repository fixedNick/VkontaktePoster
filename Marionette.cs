using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SeleniumDriver;

namespace VkontaktePoster
{
    class Marionette
    {
        private DriverSettings settings;
        private Driver driver;

        public Marionette(DriverSettings driverSettings)
        {
            settings = driverSettings;
        }

        /// <summary>
        /// Initialize driver settings and start driver.
        /// </summary>
        public void Initialize()
        {
            driver = new Driver(settings.SeleniumDriverType, settings.DriverNotificationDelegate, settings.StartMaximized, settings.Headless, settings.HidePrompt, settings.ShowExceiptions, settings.DriverFileName);
        }
    }

    /// <summary>
    /// Settings to start new SeleniumDriver
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
