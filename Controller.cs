using Newtonsoft.Json;
using System.IO;

namespace VkontaktePoster
{
    class Controller<T> : IController<T>
    {
        private string CommunitiesDirectory = "communities";

        public void UpdateSingleItem(T item)
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
    }
}
