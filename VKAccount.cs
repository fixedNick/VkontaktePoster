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
        public static bool IsLoginValid(string login)
        {
            foreach(var acc in VKAccounts)
            {
                if (acc.Credentials.Login.ToLower().Trim().Equals(login.ToLower().Trim()))
                    return false;
            }
            return true;
        }

        public VKAccount(string login, string password)
        {
            if (IsLoginValid(login) == false ) throw new Exception("Login already exists"); 
            
            Credentials = new VKAccountCredential(login, password);
            VKAccounts.Add(this);
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