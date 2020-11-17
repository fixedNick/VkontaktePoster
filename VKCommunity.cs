using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class VKCommunity
    {
        private static List<VKCommunity> Communities = new List<VKCommunity>();
        public string Address = "";

        public VKCommunity(string address) => Address = address;

        /// <summary>
        /// Add new VkCommunity to the main list
        /// </summary>
        /// <param name="address">Community URL</param>
        /// <returns>TRUE if community has been added and FALSE if community already exists in main list</returns>
        public static bool AddCommunity(string address)
        {
            foreach(var com in Communities)
            {
                if (com.Equals(address)) return false;
            }

            VKCommunity community = new VKCommunity(address);
            Communities.Add(community);
            return true;
        }

        public static void DeleteCommunity(string address)
        {
            foreach(var com in Communities)
            {
                if(com.Equals(address))
                {
                    Communities.Remove(com);
                    break;
                }
            }
        }
    }
}
