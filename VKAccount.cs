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
        public static IReadOnlyList<VKAccount> GetAccounts() => VKAccounts.AsReadOnly();

        /// <summary>
        /// Product which posting by this VKAccount
        /// </summary>
        private Product Product = null;

        private Timestamp Timestamp = new Timestamp();

        /// <summary>
        /// VKAccount credentials for a login to vk.com
        /// </summary>
        private VKAccountCredential Credentials;

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

        /// <summary>
        /// Deleting vk account from the list of all accounts
        /// </summary>
        /// <param name="login">login of vk account</param>
        public static void DeleteAccount(string login)
        {
            foreach(var acc in VKAccounts)
            {
                if(acc.Credentials.Login.Equals(login))
                {
                    VKAccounts.Remove(acc);
                    break;
                }
            }
        }

        /// <summary>
        /// Clear data of products connected to this object
        /// </summary>
        public void ClearProducts()
        {
            Product = null;
        }

        /// <summary>
        /// Connect product to this object
        /// </summary>
        /// <param name="product">Product object</param>
        /// <returns>TRUE - OK, FALSE - Another product already connected</returns>
        public bool ConnectProducts(Product product)
        {
            if (Product != null) return false;

            Product = product;
            return true;
        }

        public bool IsProductConnected()
        {
            if (Product == null) return false;
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