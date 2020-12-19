using Newtonsoft.Json;
using System.IO;

namespace VkontaktePoster
{
    class IOController<T> 
    {
        private static string CommunitiesDirectory = "communities";

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

            if (Directory.Exists(directoryPath) == false) Directory.CreateDirectory(directoryPath);


            string jsonOutput = JsonConvert.SerializeObject(item, Formatting.Indented);
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                sw.WriteAsync(jsonOutput);
            }
        }

        public static void LoadCommunitiesData()
        {
            string directoryPath = "communities";
            if (Directory.Exists(directoryPath) == false) return;

            var files = Directory.GetFiles(directoryPath);

            foreach (var file in files)
            {
                using (StreamReader sr = new StreamReader($"{file}"))
                {
                    VKCommunity community = JsonConvert.DeserializeObject<VKCommunity>(sr.ReadToEnd());
                    VKCommunity.AddCommunity(community.Address, community.Type, community.RepeatTime);
                }
            }
        }
    }
}
