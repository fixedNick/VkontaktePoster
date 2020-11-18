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
            marionette.Exit();

            foreach(var acc in Accounts)
            {
                if(acc.marionette == marionette && acc.vkAccount == vkAccount)
                {
                    Accounts.Remove(acc);
                    break;
                }
            }
        }
    }
}
