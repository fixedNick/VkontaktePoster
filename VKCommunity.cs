using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class VKCommunity
    {
        public enum CommunityType
        {
            None,
            Suggest,
            ClosedWaiting,
            ClosedJoined,
            Free,
            Unknown
        }

        public readonly static List<VKCommunity> Communities = new List<VKCommunity>();
        public string Address = "";

        private CommunityType _type = CommunityType.None;
        public CommunityType Type
        {
            get => _type;
            set
            {
                if (_type != CommunityType.None && _type != CommunityType.ClosedWaiting)
                    throw new Exception("Попытка переопределить тип группы, когда тип уже установлен");
                _type = value;
            }
        }

        /// <summary>
        /// Time of repeat post to current community
        /// </summary>
        private int _repeatTime;
        public int RepeatTime
        {
            get { return _repeatTime; }
            private set { _repeatTime = value; }
        }


        public VKCommunity(string address) => Address = address;
        public VKCommunity(string address, int repeatTime)
        {
            Address = address;
            RepeatTime = repeatTime;
        }

        /// <summary>
        /// Add new VkCommunity to the main list
        /// </summary>
        /// <param name="address">Community URL</param>
        /// <returns>TRUE if community has been added and FALSE if community already exists in main list</returns>
        public static bool AddCommunity(string address)
        {
            foreach (var com in Communities)
            {
                if (com.Address.Equals(address))
                    return false;
            }

            VKCommunity community = new VKCommunity(address);
            Communities.Add(community);
            return true;
        }

        /// <summary>
        /// Delete community from main list
        /// </summary>
        /// <param name="address">Community URL</param>
        public static void DeleteCommunity(string address)
        {
            foreach (var com in Communities)
            {
                if (com.Address.Equals(address))
                {
                    Communities.Remove(com);
                    break;
                }
            }
        }
    }
}
