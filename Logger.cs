using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Logger
    {

        private static readonly string MarionetteLog = "marionette_log.txt"; 
        private static readonly string AccountLog = "account_log.txt"; 
        private static readonly string CommunityLog = "community_log.txt"; 
        private static readonly string ProductLog = "product_log.txt";
        private static readonly string OtherLog = "other_log.txt";

        public static readonly string LogsDirectory = "logs";

        public static void Write(object type, string log)
        {
            string path = string.Empty;
            if(type.GetType().Equals(typeof(Marionette)))
                path = $"{LogsDirectory}/{MarionetteLog}";
            else if(type.GetType().Equals(typeof(VKAccount)))
                path = $"{LogsDirectory}/{AccountLog}";
            else if (type.GetType().Equals(typeof(VKAccount)))
                path = $"{LogsDirectory}/{CommunityLog}";
            else if (type.GetType().Equals(typeof(VKAccount)))
                path = $"{LogsDirectory}/{ProductLog}";
            else path = $"{LogsDirectory}/{OtherLog}";


            IOController.WriteLog(path, log);
        }
    }
}
