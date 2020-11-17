using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SeleniumDriver;

namespace VkontaktePoster
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Notification.SetupNotificationHandler(Notification.ShowMessageBox);
        }

        /// <summary>
        /// Add account into database
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            string vkLogin = textBox1.Text.Trim();
            string vkPassword = textBox2.Text.Trim();

            if (VKAccount.AddAccount(vkLogin, vkPassword) == false)
            {
                Notification.ShowNotification("Аккаунт с таким логином уже есть в списке");
                return;
            }

            // Account added
            listBox1.Items.Add(vkLogin);
            // TODO: Добавить сохранение аккаунта в файл бд.
        }

        /// <summary>
        /// Delete account from database
        /// </summary>
        private void button2_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex == -1)
            {
                Notification.ShowNotification("Для удаления аккаунта из списка - выберите его");
                return;
            }

            string vkLogin = listBox1.Items[listBox1.SelectedIndex].ToString();
            VKAccount.DeleteAccount(vkLogin);
            
            
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            // TODO: Добавить удаление аккаунта из файла бд.
        }
    }
}
