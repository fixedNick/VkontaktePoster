using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Account
    {
        private static List<Account> Accounts = new List<Account>();

        public readonly Marionette marionette;
        public readonly VKAccount vkAccount;
        public Account(Marionette mar, VKAccount acc)
        {
            marionette = mar;
            vkAccount = acc;
            Accounts.Add(this);
        }

        /// <summary>
        /// Method to set free vkAccount and stop driver;
        /// </summary>
        public void StopAccount()
        {
            foreach (var acc in Accounts)
            {
                if (acc.marionette == marionette && acc.vkAccount == vkAccount)
                {
                    Accounts.Remove(acc);
                    break;
                }
            }

            marionette.Exit();
            vkAccount.ClearProducts();
        }

        public static void FreeAccount(Marionette driver)
        {
            foreach(var a in Accounts)
            {
                if(a.marionette.Equals(driver))
                {
                    Accounts.Remove(a);
                    break;
                }
            }
        }

        /// <summary>
        /// Initialize connections btw VKAccount & Product [1:1], Marionette & VKAccount
        /// </summary>
        public static void InitializeRelations()
        {
            var defaultDriverSettings = new DriverSettings(SeleniumDriver.Driver.DriverType.Chrome, Notification.ShowMessageBox, startMaximized: true, headless: false, hidePrompt: false, showExceptions: true);

            // Connect Products to VKAccounts
            var vkAccounts = VKAccount.GetAccounts();

            for(int i = 0, j = 0; i < vkAccounts.Count && j < Product.Products.Count; i++, j++)
            {
                if(vkAccounts[i].IsProductConnected() == true)
                {
                    j--; 
                    continue;
                }

                vkAccounts[i].ConnectProducts(Product.Products[j]);
            }

            // Connect Driver to VKAccount and create Account object
            for( int z = 0; z < vkAccounts.Count; z++ )
            {
                if (vkAccounts[z].IsProductConnected() == false) continue;
                Account account = new Account(new Marionette(defaultDriverSettings), vkAccounts[z]);
            }
        }

        /// <summary>
        /// Starts every account's driver.
        /// TODO: Добавить многопоточный запуск
        /// </summary>
        public static void StartDrivers()
        {
            for(int i = 0; i < Accounts.Count; i++)
            {
                Accounts[i].Start();
            }
        }

        private void Start()
        {
            marionette.Initialize();
            if(Authentication() == false) return;
            StartPosting();
        }

        private bool Authentication()
        {
            // Auth VK
            var authResult = marionette.AuthorizateVkontakte(vkAccount.Credentials);
            if (authResult != Marionette.AuthResult.OK)
            {
                string authMessage = string.Empty;
                switch (authResult)
                {
                    case Marionette.AuthResult.BadCredential:
                        authMessage = "Неверные данные для выхода в аккаунт";
                        break;
                    case Marionette.AuthResult.BadNavigate:
                        authMessage = "Неудалось авторизоваться в связи с навигацией";
                        break;
                    case Marionette.AuthResult.ExceptionFound:
                        authMessage = "При попытке авторизации получено исключение";
                        break;
                    case Marionette.AuthResult.Blocked:
                        authMessage = "К сожалению данный аккаунт был заблокирован";
                        break;
                }
                Notification.ShowNotification($"Неудалось авторизоваться в аккаунте: {vkAccount.Credentials.Login}. Причина: {authMessage}");
                StopAccount();
                return false;
            }

            return true;
        }
        private void StartPosting() => marionette.StartPosting(vkAccount);
    }
}
