﻿using System;
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

        // TODO - Оптимизация
        // Первый Dictionary не имеет смысла, так как его актуальная информация записана во втором Dictionary, в KeyValuePair указывается время последнего поста,
        // а в int указывается количество постов за день
        public Dictionary<string, DateTime> PostedTime = new Dictionary<string, DateTime>();
        public Dictionary<string, KeyValuePair<DateTime, int>> PostedTimesToday = new Dictionary<string, KeyValuePair<DateTime, int>>();

        // Тип сообщества, используется для быстрого определения типа сообщества драйвером и работы с закрытыми сообществами
        public enum CommunityType
        {
            None,
            Suggest,
            ClosedJoined,
            ClosedWaiting,
            Free,
            Unknown
        }

        // Информация о сообществе, ключ - ссылка на сообщество, значение - какой тип у данного сообщества для данного аккаунта
        public Dictionary<string, CommunityType> CommunitiesData = new Dictionary<string, CommunityType>();

        /// <summary>
        /// Product which posting by this VKAccount
        /// </summary>
        public Product Product { get; private set; }

        public Timestamp Times = Timestamp.DefaultTimestamp;

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
            foreach (var acc in VKAccounts)
            {
                if (acc.Credentials.Login.ToLower().Trim().Equals(login.ToLower().Trim()))
                    return false;
            }
            return true;
        }

        [Newtonsoft.Json.JsonConstructor]
        public VKAccount(VKAccountCredential credentials, Product product, Dictionary<string, DateTime> postedTime, Timestamp times, Dictionary<string, KeyValuePair<DateTime, int>> postedTimesToday)
        {
            Credentials = credentials;
            Product = product;
            PostedTime = postedTime;
            Times = times;
            PostedTimesToday = postedTimesToday;
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

        public static bool AddAccount(VKAccount acc)
        {
            if (IsLoginValid(acc.Credentials.Login) == false) return false;
            VKAccounts.Add(acc);
            return true;
        }

        /// <summary>
        /// Deleting vk account from the list of all accounts
        /// </summary>
        /// <param name="login">login of vk account</param>
        public static void DeleteAccount(string login)
        {
            foreach (var acc in VKAccounts)
            {
                if (acc.Credentials.Login.Equals(login))
                {
                    VKAccounts.Remove(acc);
                    IOController.DeleteFile(acc);
                    break;
                }
            }
        }

        public static void RemoveProductFromAccounts(int id)
        {
            foreach (var acc in VKAccounts)
            {
                if (acc.Product.ProductID.Equals(id))
                    acc.ClearProducts();
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
            IOController.UpdateSingleItem(this);
            return true;
        }

        public bool IsProductConnected()
        {
            if (Product == null) return false;
            return true;
        }

        public static void AssociateCommunities()
        {
            foreach (var acc in VKAccounts)
            {
                foreach (var newCommunity in VKCommunity.Communities)
                {
                    if (acc.CommunitiesData.ContainsKey(newCommunity.Address))
                        continue;

                    try
                    {
                        var accountType = GetAccountCommunityTypeSimilarToVKCommunityType(newCommunity.Type);
                        acc.CommunitiesData.Add(newCommunity.Address, accountType);
                    }
                    catch
                    {
                        acc.CommunitiesData.Add(newCommunity.Address, CommunityType.ClosedWaiting);
                    }
                }
                IOController.UpdateSingleItem(acc);
            }
        }

        /// <summary>
        /// Метод призван возвращать аналогичный типу VKCommunity.CommunityType объект, но типа VKAccount.CommunityType
        /// </summary>
        /// <param name="communityType">Тип сообщества</param>
        /// <returns>Аналогичный типу VKCommunity.CommunityType тип VKAccount.CommunityType</returns>
        public static VKAccount.CommunityType GetAccountCommunityTypeSimilarToVKCommunityType(VKCommunity.CommunityType communityType)
        {
            switch (communityType)
            {
                case VKCommunity.CommunityType.None:
                    return CommunityType.None;
                case VKCommunity.CommunityType.Free:
                    return VKAccount.CommunityType.Free;
                case VKCommunity.CommunityType.Suggest:
                    return VKAccount.CommunityType.Suggest;
                case VKCommunity.CommunityType.Unknown:
                    return VKAccount.CommunityType.Unknown;
                case VKCommunity.CommunityType.ClosedJoined:
                    return VKAccount.CommunityType.ClosedJoined;
                case VKCommunity.CommunityType.ClosedWaiting:
                    return VKAccount.CommunityType.ClosedWaiting;
                default:
                    throw new Exception("Не предполагается использование данного метода с типом сообщества" + communityType);
            }
        }

    }

    struct VKAccountCredential
    {

        public readonly string Login;
        public readonly string Password;

        public VKAccountCredential(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}