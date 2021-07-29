using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Logger
    {

        private static readonly string MarionetteLog = ""; 
        private static readonly string AccountLog = ""; 
        private static readonly string CommunityLog = ""; 
        private static readonly string ProductLog = "";
        private static readonly string OtherLog = "";

        public static readonly string LogsDirectory = "logs";

        public void Write(object type, string log)
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
