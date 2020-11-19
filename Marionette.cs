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
        public static readonly List<Marionette> Drivers = new List<Marionette>();

        private DriverSettings settings;
        private Driver driver;

        public Marionette(DriverSettings driverSettings)
        {
            settings = driverSettings;
        }

        public void AddDriver(Marionette driver) => Drivers.Add(driver);

        /// <summary>
        /// Initialize driver settings and start driver.
        /// </summary>
        public void Initialize()
        {
            driver = new Driver(settings.SeleniumDriverType, settings.DriverNotificationDelegate, settings.StartMaximized, settings.Headless, settings.HidePrompt, settings.ShowExceiptions, settings.DriverFileName);
        }

        /// <summary>
        /// Remove current driver from drivers list and stop it
        /// </summary>
        public void Exit()
        {
            foreach (var dr in Drivers)
            {
                if (dr == this)
                    Drivers.Remove(this);
            }

            driver.StopDriver();
        }

        public AuthResult AuthorizateVkontakte(VKAccountCredential cred)
        {
            string emailInboxCss = "#index_email", passInboxCss = "#index_pass";
            driver.GoToUrl("https://vk.com/", "Добро пожаловать");

            try
            {
                driver.KeySend(driver.FindCss(emailInboxCss), cred.Login, allowException: true);
                driver.KeySend(driver.FindCss(passInboxCss), cred.Password, sendReturnKey: true, allowException: true);

                if (driver.IsURLChangedAfterNavigate() == false)
                    return AuthResult.BadNavigate;
                else
                {
                    if (driver.GetPageSource().Trim().ToLower().Contains("Не удаётся войти.".Trim().ToLower()))
                    {

                        return AuthResult.BadCredential;
                    }
                }
            }
            catch { return AuthResult.ExceptionFound; }

            return AuthResult.OK;
        }

        public enum AuthResult
        {
            OK, 
            BadCredential,
            BadNavigate,
            ExceptionFound
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
