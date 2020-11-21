﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
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

        public void StartPosting(Product product)
        {
            for(int groupIndex = 0; groupIndex < VKCommunity.Communities.Count; groupIndex++)
            {
                var currentCommunity = VKCommunity.Communities[groupIndex];

                driver.GoToUrl(currentCommunity.Address);

                if (currentCommunity.Type == VKCommunity.CommunityType.None)
                {
                    currentCommunity.Type = GetCommunityType();
                    if (currentCommunity.Type != VKCommunity.CommunityType.ClosedJoined)
                    {
                        if (JoinCommunity(currentCommunity.Type) == false)
                        {
                            Notification.ShowNotification($"Не удалось вступить в группу {currentCommunity.Address} | Тип группы: {currentCommunity.Type}");
                            continue;
                        }
                    }
                }

                MakePost(product);
            }
        }

        private VKCommunity.CommunityType GetCommunityType()
        {
            if(driver.FindCss("#join_button", isNullAcceptable: true) != null)
                return VKCommunity.CommunityType.Free; // Free community
            else if(driver.FindCss("#public_subscribe", isNullAcceptable: true) != null)
                return VKCommunity.CommunityType.Suggest; // Public community
            else if(driver.FindCss(".group_closed_text", isNullAcceptable: true) != null)
            {
                // Закрытая группа
                if(driver.FindCss("#join_button", isNullAcceptable: true) != null)
                    return VKCommunity.CommunityType.ClosedWaiting; // Request didnt send // We have to send request
                if(driver.FindCss("#group_wall", isNullAcceptable: true) != null)
                    return VKCommunity.CommunityType.ClosedJoined; // We joined current community
            }

            // Не удалось определить тип группы
            return VKCommunity.CommunityType.Unknown;
        }

        /// <summary>
        /// This method provides account join into community
        /// </summary>
        /// <param name="communityType">Type of the community</param>
        /// <returns>
        /// TRUE - Account joined
        /// FALSE - Account couldn't join community
        /// </returns>
        private bool JoinCommunity(VKCommunity.CommunityType communityType)
        {
            IWebElement joinButton = null;
            if (communityType == VKCommunity.CommunityType.Suggest)
                joinButton = driver.FindCss("#public_subscribe", isNullAcceptable: true);
            else
                joinButton = driver.FindCss("#join_button", isNullAcceptable: true);

            if (joinButton == null) 
                return false;

            driver.Click(joinButton);
            return true;
        }

        private void MakePost(Product product)
        {
            var postField = driver.FindCss("#post_field", isNullAcceptable: true);
            if(postField == null)
            {
                Notification.ShowNotification($"Не удалось найти поле для ввода информации о продукте. {product.Name}");
                return;
            }

            WriteMessageLetterByLetter(product.Name, postField); driver.KeySend(postField, "", sendReturnKey: true);
            WriteMessageLetterByLetter(product.Description, postField); driver.KeySend(postField, "", sendReturnKey: true);
            WriteMessageLetterByLetter($"Цена: {product.Price} рублей", postField);

            // sending photos
            
            var uploadPhotoButton = driver.FindCss(".ms_item.ms_item_photo._type_photo", isNullAcceptable: true);
            if(uploadPhotoButton == null)
            {
                Notification.ShowNotification("Не удалось найти элемент для выбора фотографий");
                return;
            }
            
            foreach(var ph in product.Photos)
            {
                driver.Click(uploadPhotoButton);

                var photoInput = driver.FindCss("#choose_photo_upload", isNullAcceptable: true);
                if (photoInput == null)
                {
                    Notification.ShowNotification("Не удалось найти элемент для отправки фотографий");
                    return;
                }

                driver.KeySend(photoInput, ph);
            }

            // Waiting for load images
            int loadImagesWaitCircles = 3;
            while (driver.FindCss(".page_attach_progress_wrap", isNullAcceptable: true) != null && --loadImagesWaitCircles >= 0) 
                Thread.Sleep(750);
        }

        /// <summary>
        /// Method to fix sending messages into post field and search new lines.
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="target">Post field element (input)</param>
        private void WriteMessageLetterByLetter(string message, IWebElement target)
        {
            for(int i = 0; i < message.Length; i++)
            {
                if (message[i] == '\\' && message[i + 1] == 'n')
                {
                    driver.KeySend(target, "", sendReturnKey: true);
                    i += 2;
                }
                else driver.KeySend(target, message[i].ToString());
            }
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
