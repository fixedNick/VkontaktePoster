using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Timestamp
    {
        public static readonly int DEFAULT_REPEAT_TIME = 3600 * 4;
        public static readonly int DEFAULT_LIMIT_PER_DAY = 4;

        public static int CURRENT_REPEAT_TIME = DEFAULT_REPEAT_TIME;
        public static int CURRENT_LIMIT_PER_DAY = DEFAULT_LIMIT_PER_DAY;

        public TimeSpan TIME_BETWEEN_REPEAT_POST;
        public int POST_LIMIT_PER_DAY;

        public static readonly Timestamp DefaultTimestamp = new Timestamp(TimeSpan.FromSeconds(DEFAULT_REPEAT_TIME), DEFAULT_LIMIT_PER_DAY);

        public Timestamp()
        {
            TIME_BETWEEN_REPEAT_POST = TimeSpan.FromSeconds(DEFAULT_REPEAT_TIME);
            POST_LIMIT_PER_DAY = DEFAULT_LIMIT_PER_DAY;
        }

        public Timestamp(TimeSpan repeats, int limit)
        {
            TIME_BETWEEN_REPEAT_POST = repeats;
            POST_LIMIT_PER_DAY = limit;
        }

        /// <summary>
        /// Проверяет, прошло ли достаточно времени после последнего поста в группе
        /// </summary>
        /// <param name="acc">VKAccount</param>
        /// <param name="address">VKCommunity address</param>
        /// <returns></returns>
        public static bool IsTimeBetweenPostsPast(VKAccount acc, string address)
        {
            foreach (var kvp in acc.PostedTime)
            {
                if (kvp.Key.Equals(address))
                {
                    if ((kvp.Value + acc.Times.TIME_BETWEEN_REPEAT_POST) <= DateTime.Now)
                        return true;
                    return false;
                }
            }
            return true;
        }

        public static bool IsPostLimitReached(VKAccount acc, string address)
        {
            if(acc.PostedTimesToday.ContainsKey(address))
            {
                if(acc.PostedTimesToday[address].Key.Date.Equals(DateTime.Now.Date))
                {
                    if (acc.PostedTimesToday[address].Value >= acc.Times.POST_LIMIT_PER_DAY)
                        return true;
                    else
                        return false;
                }

                acc.PostedTimesToday[address] = new KeyValuePair<DateTime, int>(DateTime.Now, 0);
                return false;
            }
            
            acc.PostedTimesToday.Add(address, new KeyValuePair<DateTime, int>(DateTime.Now, 0));
            return false;
        }

        /// <summary>
        /// Обновляет информацию о дате последнего поста в группе address для VKAccount
        /// </summary>
        /// <param name="account">VKAccount</param>
        /// <param name="address">VKCommunity address</param>
        public static void PostMade(VKAccount account, string address)
        {
            if (account.PostedTime.ContainsKey(address))
                account.PostedTime[address] = DateTime.Now;
            else
                account.PostedTime.Add(address, DateTime.Now);
        }

        /// <summary>
        /// Определяет сколько времени осталось аккаунту до следующего постав в сообществе
        /// </summary>
        /// <param name="account">аккаунт</param>
        /// <param name="communityAddress">адрес сообщество</param>
        /// <returns>Возвращает 0 секунд, если доступен постинг и N секунд до постинга, если он не доступен.</returns>
        public static TimeSpan GetTimeBeforeNextPost(VKAccount account, string communityAddress)
        {
            if (account.PostedTime.ContainsKey(communityAddress) == false) 
                throw new TimestampKeysException($"У аккаунта {account.Credentials.Login} не найден ключ сообщества {communityAddress}");

            var btw = account.PostedTime[communityAddress].Subtract(DateTime.Now).Add(TimeSpan.FromSeconds(VKCommunity.GetCommunity(communityAddress).RepeatTime));
            return btw.TotalSeconds <= 0 ? new TimeSpan(0) : btw;
        }
    }

    class TimestampKeysException : Exception
    {
        public TimestampKeysException(string message) : base(message)
        {
        }
    }
}
