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

        /// <summary>
        /// Initialize connections btw VKAccount & Product [1:1], Marionette & VKAccount
        /// </summary>
        public static void InitializeRelations()
        {
            var defaultDriverSettings = new DriverSettings(SeleniumDriver.Driver.DriverType.Chrome, Notification.ShowMessageBox, true, false, false, true);

            // Connect Products to VKAccounts
            var vkAccounts = VKAccount.GetAccounts();

            int i = -1;
            for(int j = 0; j < Product.Products.Count; j++ )
            {
                while(++i < vkAccounts.Count && vkAccounts[i].ConnectProducts(Product.Products[j]) == false)
                    continue;
            }

            // Connect Driver to VKAccount and create Account object
            for( int z = 0; z < i; z++)
            {
                Account account = new Account(new Marionette(defaultDriverSettings), vkAccounts[z]);
                Accounts.Add(account);
            }
        }
    }
}
