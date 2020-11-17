using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkontaktePoster
{
    class Notification
    {
        public delegate void ShowNotificationDelegate(string message);
        public static ShowNotificationDelegate ShowNotification;

        public static void SetupNotificationHandler(ShowNotificationDelegate method) => ShowNotification = method;

        public static void ShowMessageBox(string text) => System.Windows.Forms.MessageBox.Show(text);
    }
}
