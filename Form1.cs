using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
                textBox1.Clear();
                textBox2.Clear();
                return;
            }

            // Account added
            listBox1.Items.Add(vkLogin);
            textBox1.Clear();
            textBox2.Clear();
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

        /// <summary>
        /// Add community to the main list
        /// </summary>
        private void button10_Click(object sender, EventArgs e)
        {
            string communityURL = textBox6.Text;
            if(communityURL.Contains('.') == false)
            {
                Notification.ShowNotification("Неверный формат ссылки.");
                textBox6.Clear();
                return;
            }

            if(VKCommunity.AddCommunity(communityURL) == false)
            {
                Notification.ShowNotification("Данное сообщество уже есть в списке");
                textBox6.Clear();
                return;
            }

            listBox5.Items.Add(communityURL);
            textBox6.Clear();
            // TODO: Добавить сохранение в бж
        }

        /// <summary>
        /// Delete community from the main list
        /// </summary>
        private void button9_Click(object sender, EventArgs e)
        {
            if(listBox5.SelectedIndex == -1)
            {
                Notification.ShowNotification("Для удаления сообщества - выберите его в списке");
                return;
            }
            VKCommunity.DeleteCommunity(listBox5.Items[listBox5.SelectedIndex].ToString());

            listBox5.Items.RemoveAt(listBox5.SelectedIndex);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(StartAccountsThread);
        }

        private void StartAccountsThread(object obj)
        {
            Account.InitializeRelations();
            Account.StartDrivers();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string name = textBox4.Text;
            string desc = textBox3.Text;
            if(Int32.TryParse(textBox9.Text, out int price) == false)
            {
                Notification.ShowNotification("Неверно указана цена товара.");
                return;
            }

            List<string> photos = new List<string>();

            foreach (var ph in listBox3.Items)
                photos.Add(ph.ToString());

            new Product(name, price, desc, photos);

            listBox2.Items.Add(name);
        }
    }
}
