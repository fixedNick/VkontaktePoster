using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class VKCommunity
    {
        public enum CommunityType : int
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

        public int LimitPerDay { get; private set; }


        [JsonConstructor]
        public VKCommunity(string address, int type, int repeatTime, int limitPerDay)
        {
            Address = address;
            RepeatTime = repeatTime;
            Type = (CommunityType) type;
            LimitPerDay = limitPerDay;
        }

        public VKCommunity(string address) : this(address, CommunityType.None, Timestamp.CURRENT_REPEAT_TIME, Timestamp.CURRENT_LIMIT_PER_DAY) { }
        public VKCommunity(string address, int repeatTime) : this(address, CommunityType.None, repeatTime, Timestamp.CURRENT_LIMIT_PER_DAY) { }
        public VKCommunity(string address, int repeatTime, int limitPerDay) : this(address, CommunityType.None, repeatTime, limitPerDay) { }
        public VKCommunity(string address, CommunityType type, int repeatTime, int limitPerDay)
        {
            Address = address;
            RepeatTime = repeatTime;
            Type = type;
            LimitPerDay = limitPerDay;
        }

        /// <summary>
        /// Add new VkCommunity to the main list
        /// </summary>
        /// <param name="address">Community URL</param>
        /// <returns>TRUE if community has been added and FALSE if community already exists in main list</returns>
        public static bool AddCommunity(string address) => AddCommunity(address, CommunityType.None, Timestamp.CURRENT_REPEAT_TIME, Timestamp.CURRENT_LIMIT_PER_DAY);
        public static bool AddCommunity(string address, CommunityType type, int repeatTime, int limitPerDay)
        {
            foreach (var com in Communities)
            {
                if (com.Address.Equals(address))
                    return false;
            }

            VKCommunity community = new VKCommunity(address, type, repeatTime, limitPerDay);
            Communities.Add(community);
            return true;
        }
        public static bool AddCommunity(VKCommunity community)
        {
            foreach(var com in Communities)
            {
                if (com.Address.Equals(community.Address))
                    return false;
            }

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
                    IOController.DeleteFile(com);
                    break;
                }
            }
        }
    }
}
