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
        public static bool IsTimeBetweenPostsPast(VKAccount acc, string address, ref TimeSpan closestTimeToCheck)
        {
            foreach (var kvp in acc.PostedTime)
            {
                if (kvp.Key.Equals(address))
                {
                    if ((kvp.Value + acc.Times.TIME_BETWEEN_REPEAT_POST) <= DateTime.Now)
                    {
                        // Если аккаунт может постить, то ближайшее время для следующего поста составит время его ожидания
                        if (closestTimeToCheck.TotalSeconds > acc.Times.TIME_BETWEEN_REPEAT_POST.TotalSeconds)
                            closestTimeToCheck = acc.Times.TIME_BETWEEN_REPEAT_POST;
                        return true;
                    }

                    // Если время еще не прошло, проверяем, не меньше ли это время ожидания, чем наше текущее
                    // Если меньше, то переназаначаем значение closestTimeToCheck, возможно именно данное количество времени драйвер будет ждать до перезапуска

                    var timeBeforeNextPost = DateTime.Now.Subtract(kvp.Value + acc.Times.TIME_BETWEEN_REPEAT_POST);
                    if (timeBeforeNextPost.TotalSeconds < closestTimeToCheck.TotalSeconds)
                        closestTimeToCheck = timeBeforeNextPost;

                    return false;
                }
            }

            // Если аккаунт может постить, то ближайшее время для следующего поста составит время его ожидания
            if (closestTimeToCheck.TotalSeconds > acc.Times.TIME_BETWEEN_REPEAT_POST.TotalSeconds)
                closestTimeToCheck = acc.Times.TIME_BETWEEN_REPEAT_POST;

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
        /// Обновляет информацию о дате последнего поста в группе address для VKAccount и информация о количестве постов за день
        /// </summary>
        /// <param name="account">VKAccount</param>
        /// <param name="address">VKCommunity address</param>
        public static void PostMade(VKAccount account, string address)
        {
            if (account.PostedTime.ContainsKey(address))
                account.PostedTime[address] = DateTime.Now;
            else
                account.PostedTime.Add(address, DateTime.Now);

            account.PostedTimesToday[address] = new KeyValuePair<DateTime, int>(DateTime.Now, account.PostedTimesToday[address].Value + 1);
        }

        /// <summary>
        /// Узнает наступил ли ноывй день с момента последнего поста
        /// </summary>
        /// <param name="account">Аккаунт, который совершал постинг</param>
        /// <param name="communityAddress">Сообщество в котором аккаунт совершал пост</param>
        /// <returns>Возвращает TRUE, если новый день наступил, следует обновить данные для аккаунта. Возвращает FALSE, если новый день все еще не наступил</returns>
        public static bool IsNewDayForPosting(VKAccount account, string communityAddress)
        {
            if (account.PostedTimesToday.ContainsKey(communityAddress) == false)
            {
                account.PostedTimesToday.Add(communityAddress, new KeyValuePair<DateTime, int>(DateTime.Now,0));
                return false;
            }

            var currentDay = DateTime.Now.Day;
            var currentMonth = DateTime.Now.Month;
            var lastPostDay = account.PostedTimesToday[communityAddress].Key.Day;
            var lastPostMonth = account.PostedTimesToday[communityAddress].Key.Month;

            if (currentMonth == lastPostMonth && currentDay == lastPostDay)
                return false;
            else return true;
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

            /// ВАЖНО
            /// В данной строчке мы вычитаем время последнего поста из текущего времени и прибавляем ВРЕМЯ МЕЖДУ ПОСТАМИ ДЛЯ АККАУНТА, а не ВРЕМЯ МЕЖДУ ПОСТАМИ ДЛЯ СООБЩЕСТВА
            var btw = TimeSpan.FromSeconds(account.PostedTime[communityAddress].Subtract(DateTime.Now).TotalSeconds + account.Times.TIME_BETWEEN_REPEAT_POST.TotalSeconds);
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
