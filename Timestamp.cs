using System;
using System.Collections.Generic;
using System.Linq;
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

        public static Timestamp DefaultTimestamp = new Timestamp(TimeSpan.FromSeconds(DEFAULT_REPEAT_TIME), DEFAULT_LIMIT_PER_DAY);

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

        /// <summary>
        /// Обновляет информацию о дате последнего поста в группе address для VKAccount
        /// </summary>
        /// <param name="account">VKAccount</param>
        /// <param name="address">VKCommunity address</param>
        internal static void PostMade(VKAccount account, string address)
        {
            if (account.PostedTime.ContainsKey(address))
                account.PostedTime[address] = DateTime.Now;
            else
                account.PostedTime.Add(address, DateTime.Now);
        }
    }
}
