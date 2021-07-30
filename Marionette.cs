using System;
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
        public static Boolean StopPostingClicked = false;

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
            driver.StopDriver();
            foreach (var dr in Drivers)
            {
                if (dr == this)
                    Drivers.Remove(this);
            }
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
            Blocked,
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
                        return AuthResult.BadCredential;
                    else if(driver.GetCurrentUrl().ToLower().Contains("login?act=blocked".ToLower()) == true)
                        return AuthResult.Blocked;
                }
            }
            catch { return AuthResult.ExceptionFound; }

            return AuthResult.OK;
        }

        public void StartPosting(VKAccount account)
        {
            for (int groupIndex = 0; groupIndex < VKCommunity.Communities.Count && StopPostingClicked == false; groupIndex++)
            {
                var currentCommunity = VKCommunity.Communities[groupIndex];

                #region Проверка временных промежутков
                if (Timestamp.IsNewDayForPosting(account, currentCommunity.Address) == true)
                {
                    account.PostedTimesToday[currentCommunity.Address] = new KeyValuePair<DateTime, int>(account.PostedTimesToday[currentCommunity.Address].Key, 0);
                    Logger.Write(this, $"Для аккаунта {account.Credentials.Login} в сообществе {currentCommunity.Address} обнулен дневной счетчик. Причина: Новый день");
                    IOController.UpdateSingleItem(account);
                }

                if (Timestamp.IsTimeBetweenPostsPast(account, currentCommunity.Address) == false)
                {
                    var nextPostInMinutes = TimeSpan.Zero;
                    try
                    {
                        nextPostInMinutes = Timestamp.GetTimeBeforeNextPost(account, currentCommunity.Address);
                    }
                    catch(TimestampKeysException ex)
                    {
                        Logger.Write(this, ex.Message);
                    }
                    
                    string loggerText = $"Аккаунту {account.Credentials.Login} не удалось оставить пост в {currentCommunity.Address}. Причина: Не прошло достаточно времени между постами.";
                    if (nextPostInMinutes.TotalSeconds > 0)
                        loggerText += $" Осталось времени до следующего поста: {nextPostInMinutes}";

                    Logger.Write(this, loggerText);
                    continue;
                }

                if (Timestamp.IsPostLimitReached(account, currentCommunity.Address) == true)
                {
                    Logger.Write(this, $"Аккаунту {account.Credentials.Login} не удалось оставить пост в {currentCommunity.Address}. Причина: Достингнут дневной лимит");
                    continue;
                }
                else
                {
                    account.PostedTimesToday[currentCommunity.Address] = new KeyValuePair<DateTime, int>(DateTime.Now, account.PostedTimesToday[currentCommunity.Address].Value + 1);
                    IOController.UpdateSingleItem(account);
                }
                #endregion

                driver.GoToUrl(currentCommunity.Address);

                if(account.CommunitiesData[currentCommunity.Address] == VKAccount.CommunityType.None)
                {
                    /// Если тип сообщества определен для VKCommunity и это не Closed, то устанавливаем его аккаунту
                    /// Иначе проверяем тип сообщества и устанавливаем его аккаунту и сообществу, за исключением Closed типа, при данном типе - мы не переопределяем тип сообщества.
                    if (currentCommunity.Type != VKCommunity.CommunityType.None && currentCommunity.Type != VKCommunity.CommunityType.Closed)
                        account.CommunitiesData[currentCommunity.Address] = VKAccount.GetAccountCommunityTypeSimilarToVKCommunityType(currentCommunity.Type);
                    else
                    {
                        var communityType = GetCommunityType();
                        account.CommunitiesData[currentCommunity.Address] = VKAccount.GetAccountCommunityTypeSimilarToVKCommunityType(communityType);
                        IOController.UpdateSingleItem(account);

                        if (communityType == VKCommunity.CommunityType.None)
                        {
                            currentCommunity.Type = (communityType == VKCommunity.CommunityType.ClosedJoined || communityType == VKCommunity.CommunityType.ClosedWaiting) ? VKCommunity.CommunityType.Closed : communityType;
                            IOController.UpdateSingleItem(currentCommunity);
                        }
                    }

                    Logger.Write(this, $"Для аккаунта {account.Credentials.Login} проверен тип сообщества {currentCommunity.Address}. Результат: {account.CommunitiesData[currentCommunity.Address]}");
                }
                else if(account.CommunitiesData[currentCommunity.Address] == VKAccount.CommunityType.ClosedWaiting)
                {
                    /// Проверяем тип сообщества, не изменился ли он на ClosedJoined
                    /// Если изменился - устанавливаем новое значение для аккаунта и допускаем его к постингу
                    /// Если нет - пропускаем итерацию цикла
                    var communityType = GetCommunityType();
                    if (communityType == VKCommunity.CommunityType.ClosedJoined)
                    {
                        account.CommunitiesData[currentCommunity.Address] = VKAccount.CommunityType.ClosedJoined;
                        IOController.UpdateSingleItem(account);
                        Logger.Write(this, $"Аккаунт {account.Credentials.Login} был принят в сообщество {currentCommunity.Address} - тип сообщества для аккаунта изменен.");
                    }
                    else
                    {
                        Logger.Write(this, $"Аккаунт {account.Credentials.Login} еще не был принят в сообщество {currentCommunity.Address}");
                        continue;
                    }
                }

                if(JoinCommunity(account.CommunitiesData[currentCommunity.Address]) == JoinCommunityResult.Error)
                {
                    Logger.Write(this, $"Аккаунту {account.Credentials.Login} не удалось вступить в сообщество {currentCommunity.Address}. Тип группы: {currentCommunity.Type}");
                    continue;
                }

                Logger.Write(this, $"Аккаунт {account.Credentials.Login} начинает писать пост о продукте {account.Product.Name}");
                MakePost(account.Product);
                Timestamp.PostMade(account, currentCommunity.Address);
                IOController.UpdateSingleItem(account); // TODO - Проверить, требуется ли это
                IOController.UpdateSingleItem(currentCommunity); // TODO - Проверить, требуется ли это
            }

            // TODO
            // тут мы вырубаем драйвер и перекидываем его в режим ожидания
            Exit();
        }

        

        /// <summary>
        /// Метод анализирует стену сообщесто и исходя из этого делает вывод для аккаунта какой тип имеет данное сообщество
        /// </summary>
        /// <returns></returns>
        private VKCommunity.CommunityType GetCommunityType()
        {
            if(driver.FindCss("#join_button", isNullAcceptable: true, useFastSearch: true) != null)
                return VKCommunity.CommunityType.Free; // Free community
            else if(driver.FindCss("#public_subscribe", isNullAcceptable: true, useFastSearch: true) != null)
                return VKCommunity.CommunityType.Suggest; // Public community
            else if(driver.FindCss(".group_closed_text", isNullAcceptable: true, useFastSearch: true) != null)
            {
                // Закрытая группа
                if(driver.FindCss("#join_button", isNullAcceptable: true, useFastSearch: true) != null)
                    return VKCommunity.CommunityType.ClosedWaiting; // Request didnt send // We have to send request
                if(driver.FindCss("#group_wall", isNullAcceptable: true, useFastSearch: true) != null)
                    return VKCommunity.CommunityType.ClosedJoined; // We joined current community
            }

            // Не удалось определить тип группы
            return VKCommunity.CommunityType.Unknown;
        }
        
        private enum JoinCommunityResult
        {
            Successfull, 
            AlreadyJoined,
            Error
        }
        /// <summary>
        /// This method provides account join into community
        /// </summary>
        /// <param name="communityType">Type of the community</param>
        /// <returns>
        /// TRUE - Account joined
        /// FALSE - Account couldn't join community
        /// </returns>
        private JoinCommunityResult JoinCommunity(VKAccount.CommunityType communityType)
        {
            driver.GoToUrl(driver.GetCurrentUrl());

            if (driver.FindCss("#page_actions_btn", isNullAcceptable: true) != null)
                return JoinCommunityResult.AlreadyJoined;

            IWebElement joinButton = null;
            if (communityType == VKAccount.CommunityType.Suggest)
                joinButton = driver.FindCss("#public_subscribe", isNullAcceptable: true);
            else
                joinButton = driver.FindCss("#join_button", isNullAcceptable: true);

            if (joinButton == null) 
                return JoinCommunityResult.Error;

            driver.Click(joinButton);
            return JoinCommunityResult.Successfull;
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

            var photoIcon = driver.FindCss("#page_add_media > div.media_selector.clear_fix > a.ms_item.ms_item_photo._type_photo");

            foreach(var photo in product.Photos)
            {
                driver.Click(photoIcon, true);
                Thread.Sleep(1250);

                driver.KeySend(driver.FindCss("#choose_photo_upload"), photo, allowException: true);
                Thread.Sleep(1250);
            }

            // Waiting for load images
            int loadImagesWaitCircles = 3;
            while (driver.FindCss(".page_attach_progress_wrap", isNullAcceptable: true, refreshPage: false) != null && --loadImagesWaitCircles >= 0) 
                Thread.Sleep(750);


            // CLICK MAKE POST BUTTON
            var sendPostBtn = driver.FindCss("#send_post", isNullAcceptable: true, refreshPage: false);
            if (sendPostBtn != null)
                driver.Click(sendPostBtn);
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
                    i++;
                }
                else driver.KeySend(target, message[i].ToString());
            }
        }
    }
}
