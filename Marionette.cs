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

        /// <summary>
        /// Results of authentication
        /// OK - All is fine
        /// BadCredentials - Cannot login by this credentials
        /// BadNavigate - Driver didnt navigated from vk.com to vk.com/feed
        /// ExceptionFound - Driver got an exception
        /// </summary>
        public enum AuthResult
        {
            OK,
            BadCredential,
            BadNavigate,
            ExceptionFound
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

        public Boolean StartPosting(Product product)
        {
            for(int groupIndex = 0; groupIndex < VKCommunity.Communities.Count; groupIndex++)
            {
                var currentCommunity = VKCommunity.Communities[groupIndex];

                driver.GoToUrl(currentCommunity.Address);
                if(driver.IsURLChangedAfterNavigate() == false)
                {
                    Notification.ShowNotification($"Неудалось перейти к группе {currentCommunity.Address}");
                    continue;
                }

                if (currentCommunity.Type == VKCommunity.CommunityType.None)
                    currentCommunity.Type = GetCommunityType();
            }
        }

        private VKCommunity.CommunityType GetCommunityType()
        {
            // .group_closed_text - Закрытая группа
            // Если есть #join_button - то еще не подали заявку
            // Если нет, то значит подали
            // Если нет #join_button - проверяем наличие #group_wall, если есть - мы в группе, если нет - мы не в группе
            // TODO: Узнать, что будет, если нас приняли в комьюнити
            // #public_subscribe - Предложка
            // #join_button - Открытая
            if(driver.FindCss("#join_button", isNullAcceptable: true) != null)
            {
                // Это открытая группа
                return VKCommunity.CommunityType.Free;
            }
            else if(driver.FindCss("#public_subscribe", isNullAcceptable: true) != null)
            {
                // Это паблик группа
                return VKCommunity.CommunityType.Suggest;
            }
            else if(driver.FindCss(".group_closed_text", isNullAcceptable: true) != null)
            {
                // Закрытая группа
                if(driver.FindCss("#join_button", isNullAcceptable: true) != null)
                {
                    // Заявка в группу не подана
                    // Подаем заявку
                    return VKCommunity.CommunityType.ClosedWaiting;
                }
                
                if(driver.FindCss("#group_wall", isNullAcceptable: true) != null)
                {
                    // Мы вступили в данную группу
                    return VKCommunity.CommunityType.ClosedJoined;
                }
            }

            // Неудалось определить тип группы
            return VKCommunity.CommunityType.Unknown;
        }

        //public bool MakePost(Product product)
        //{
        //}
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
