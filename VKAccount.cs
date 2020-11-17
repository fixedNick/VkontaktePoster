using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class VKAccount
    {
        /// <summary>
        /// List of VKAccounts
        /// </summary>
        private static List<VKAccount> VKAccounts = new List<VKAccount>();

        /// <summary>
        /// List of Products which posting by this VKAccount
        /// </summary>
        public List<Product> Products = new List<Product>();

        public readonly Timestamp Timestamp = new Timestamp();

        /// <summary>
        /// VKAccount credentials for a login to vk.com
        /// </summary>
        public readonly VKAccountCredential Credentials;

        /// <summary>
        /// Method check is current login already in VKAccounts list or not
        /// </summary>
        /// <param name="login">login to check</param>
        /// <returns>Returns TRUE if login is free and FALSE if login already exists</returns>
        private static bool IsLoginValid(string login)
        {
            foreach(var acc in VKAccounts)
            {
                if (acc.Credentials.Login.ToLower().Trim().Equals(login.ToLower().Trim()))
                    return false;
            }
            return true;
        }

        public VKAccount(string login, string password) => Credentials = new VKAccountCredential(login, password);

        /// <summary>
        /// Creating new vk account, and add it to the all accounts list
        /// </summary>
        /// <param name="login">Account login</param>
        /// <param name="pass">Account password</param>
        /// <returns>FALSE if account already exists and TRUE if account has been added</returns>
        public static bool AddAccount(string login, string pass)
        {
            if (IsLoginValid(login) == false) return false;

            VKAccount acc = new VKAccount(login, pass);
            VKAccounts.Add(acc);
            return true;
        }
    }

    struct VKAccountCredential {
    
        public readonly string Login;
        public readonly string Password;

        public VKAccountCredential(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}