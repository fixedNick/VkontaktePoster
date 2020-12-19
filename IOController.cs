using Newtonsoft.Json;
using System.IO;

namespace VkontaktePoster
{
    class IOController
    {
        private static string CommunitiesDirectory = "communities";
        private static string ProductsDirectory = "products";

        public static void UpdateSingleItem<T>(T item)
        {
            string path = string.Empty, name = string.Empty;
            string directoryPath = "";

            if (item.GetType().Equals(typeof(VKCommunity)))
            {
                var domainParts = (item as VKCommunity).Address.Split('/');
                name = domainParts[domainParts.Length - 1];
                path = CommunitiesDirectory + "/" + name + ".txt";
                directoryPath = CommunitiesDirectory;
            }
            else if(item.GetType().Equals(typeof(Product)))
            {
                name = (item as Product).ProductID.ToString();
                path = $"{ProductsDirectory}/{name}.txt";
                directoryPath = ProductsDirectory;
            }

            if (Directory.Exists(directoryPath) == false) Directory.CreateDirectory(directoryPath);


            string jsonOutput = JsonConvert.SerializeObject(item, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.Write(jsonOutput);
            }
        }

        //public static void LoadCommunitiesData()
        //{
        //    if (Directory.Exists(CommunitiesDirectory) == false) return;
        //    var files = Directory.GetFiles(CommunitiesDirectory);

        //    foreach (var file in files)
        //    {
        //        using (StreamReader sr = new StreamReader($"{file}"))
        //        {
        //            VKCommunity community = JsonConvert.DeserializeObject<VKCommunity>(sr.ReadToEnd());
        //            VKCommunity.AddCommunity(community.Address, community.Type, community.RepeatTime, community.LimitPerDay);
        //        }
        //    }
        //}

        //public static void LoadProductsData<T>()
        //{
        //    if (Directory.Exists(ProductsDirectory) == false) return;
        //    var files = Directory.GetFiles(ProductsDirectory);

        //    foreach (var file in files)
        //    {
        //        using (StreamReader sr = new StreamReader($"{file}"))
        //        {
        //            Product product = JsonConvert.DeserializeObject<Product>(sr.ReadToEnd());
        //            Product.Products.Add(product);
        //        }
        //    }
        //}

        public static void LoadData<T>()
        {
            string directoryPath = string.Empty;

            if (typeof(T) == typeof(VKCommunity)) directoryPath = CommunitiesDirectory;
            else if (typeof(T) == typeof(Product)) directoryPath = ProductsDirectory;

            if (Directory.Exists(directoryPath) == false) return;
            var files = Directory.GetFiles(directoryPath);
            
            foreach(var file in files)
            {
                var json = File.ReadAllText(file);
                T item = JsonConvert.DeserializeObject<T>(json);
                if (typeof(T) == typeof(VKCommunity)) VKCommunity.AddCommunity(item as VKCommunity);
                else if (typeof(T) == typeof(Product)) new Product(item as Product);
            }
        }
    }
}