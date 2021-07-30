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
            if (type.GetType().Equals(typeof(Marionette)))
                path = $"{LogsDirectory}/{MarionetteLog}";
            else if (type.GetType().Equals(typeof(VKAccount)))
                path = $"{LogsDirectory}/{AccountLog}";
            else if (type.GetType().Equals(typeof(VKCommunity)))
                path = $"{LogsDirectory}/{CommunityLog}";
            else if (type.GetType().Equals(typeof(Product)))
                path = $"{LogsDirectory}/{ProductLog}";
            else
            {
                path = $"{LogsDirectory}/{OtherLog}";
                log = $"[{type.GetType()}] {log}";
            }

            var day = DateTime.Now.Day <= 9 ? 0.ToString() + DateTime.Now.Day.ToString() : DateTime.Now.Day.ToString();
            var month = DateTime.Now.Month <= 9 ? 0.ToString() + DateTime.Now.Month.ToString() : DateTime.Now.Month.ToString();
            var h = DateTime.Now.Hour <= 9 ? 0.ToString() + DateTime.Now.Hour.ToString() : DateTime.Now.Hour.ToString();
            var m = DateTime.Now.Minute <= 9 ? 0.ToString() + DateTime.Now.Minute.ToString() : DateTime.Now.Minute.ToString();
            var s = DateTime.Now.Second <= 9 ? 0.ToString() + DateTime.Now.Second.ToString() : DateTime.Now.Second.ToString();

            log = $"[{day}/{month}/{DateTime.Now.Year} {h}/{m}/{s}] {log}"; 


            IOController.WriteLog(path, log);
        }
    }
}
