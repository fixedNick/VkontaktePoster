using Newtonsoft.Json;
using System;
using System.IO;

namespace VkontaktePoster
{
    class IOController
    {
        private static string CommunitiesDirectory = "communities";
        private static string ProductsDirectory = "products";
        private static string VKAccountsDirectory = "vkAccounts";

        public static void UpdateSingleItem<T>(T item)
        {
            string path = string.Empty, name = string.Empty;
            string directoryPath = "";

            if (item.GetType().Equals(typeof(VKCommunity)))
            {
                var domainParts = (item as VKCommunity).Address.Split('/');
                name = domainParts[domainParts.Length - 1];
                path = $"{CommunitiesDirectory}/{name}.txt";
                directoryPath = CommunitiesDirectory;
            }
            else if (item.GetType().Equals(typeof(Product)))
            {
                name = (item as Product).ProductID.ToString();
                path = $"{ProductsDirectory}/{name}.txt";
                directoryPath = ProductsDirectory;
            }
            else if(item.GetType().Equals(typeof(VKAccount)))
            {
                name = (item as VKAccount).Credentials.Login;
                path = $"{VKAccountsDirectory}/{name}.txt";
                directoryPath = VKAccountsDirectory;
            }

            if (Directory.Exists(directoryPath) == false) Directory.CreateDirectory(directoryPath);


            string jsonOutput = JsonConvert.SerializeObject(item, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(jsonOutput);
            }
        }

        public static void LoadData<T>()
        {
            string directoryPath = string.Empty;

            if (typeof(T) == typeof(VKCommunity)) directoryPath = CommunitiesDirectory;
            else if (typeof(T) == typeof(Product)) directoryPath = ProductsDirectory;
            else if (typeof(T) == typeof(VKAccount)) directoryPath = VKAccountsDirectory;

            if (Directory.Exists(directoryPath) == false) return;
            var files = Directory.GetFiles(directoryPath);
            
            foreach(var file in files)
            {
                var json = File.ReadAllText(file);
                T item = JsonConvert.DeserializeObject<T>(json);
                if (typeof(T) == typeof(VKCommunity)) VKCommunity.AddCommunity(item as VKCommunity);
                else if (typeof(T) == typeof(Product)) new Product(item as Product);
                else if (typeof(T) == typeof(VKAccount)) VKAccount.AddAccount(item as VKAccount);
            }
        }

        internal static void DeleteFile<T>(T item)
        {
            string fileName = string.Empty, directory = string.Empty;

            if (typeof(T) == typeof(Product))
            {
                fileName = (item as Product).ProductID + ".txt";
                directory = ProductsDirectory;
            }
            else if(typeof(T) == typeof(VKAccount))
            {
                fileName = (item as VKAccount).Credentials.Login + ".txt";
                directory = VKAccountsDirectory;
            }
            else if(typeof(T) == typeof(VKCommunity))
            {
                fileName = (item as VKCommunity).Address + ".txt";
                directory = CommunitiesDirectory;
            }

            if (File.Exists($"{directory}/{fileName}"))
                File.Delete($"{directory}/{fileName}"); 
        }
    }
}