using System.Collections.Generic;
using System.IO;

namespace VkontaktePoster
{
    static class Config
    {
        private static readonly string ConfigPath = "config.txt";
        private static readonly KeyValuePair<string, string>[] DefaultProps =
        {
            new KeyValuePair<string, string>("NextProductID", "0")
        };

        public static void CreateDefaultConfig()
        {
            if (File.Exists(ConfigPath) == true) return;

            foreach(var prop in DefaultProps)
            {
                using (StreamWriter sw = new StreamWriter(ConfigPath, true))
                {
                    sw.WriteLine($"{prop.Key}={prop.Value}");
                }
            }
        }

        public static string GetConfigProperty(string propName)
        {
            using (StreamReader sr = new StreamReader(ConfigPath))
            {
                var propVal = sr.ReadLine().Split('=');
                if (propVal[0].Equals(propName)) return propVal[1];
            }

            throw new System.Exception("Propery didint found");
        }

        public static void SetConfigProperty(string prop, string val)
        {
            var lines = File.ReadAllLines(ConfigPath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Split('=')[0].Equals(prop))
                    lines[i] = $"{prop}={val}";
            }
            File.WriteAllText(ConfigPath, "");

            foreach (var line in lines)
            {
                using (StreamWriter sw = new StreamWriter(ConfigPath, true))
                {
                    sw.WriteLine(line);
                }
            }
        }
    }
}
