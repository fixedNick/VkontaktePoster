using Newtonsoft.Json;
using System.IO;

namespace VkontaktePoster
{
    class IOController<T> 
    {
        private static string CommunitiesDirectory = "communities";
        private static string ProductsDirectory = "products";

        public static void UpdateSingleItem(T item)
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

        public static void LoadCommunitiesData()
        {
            if (Directory.Exists(CommunitiesDirectory) == false) return;
            var files = Directory.GetFiles(CommunitiesDirectory);

            foreach (var file in files)
            {
                using (StreamReader sr = new StreamReader($"{file}"))
                {
                    VKCommunity community = JsonConvert.DeserializeObject<VKCommunity>(sr.ReadToEnd());
                    VKCommunity.AddCommunity(community.Address, community.Type, community.RepeatTime, community.LimitPerDay);
                }
            }
        }
    }
}
