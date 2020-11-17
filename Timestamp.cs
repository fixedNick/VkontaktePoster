using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Timestamp
    {
        private const int DEFAULT_REPEAT_TIME = 3600 * 4;
        private const int DEFAULT_LIMIT_PER_DAY = 4;

        public TimeSpan TIME_BETWEEN_REPEAT_POST;
        public int POST_LIMIT_PER_DAY;

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
    }
}
