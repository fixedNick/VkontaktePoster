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
            Config.CreateDefaultConfig();
            Notification.SetupNotificationHandler(Notification.ShowMessageBox);
            IOController.LoadData<VKCommunity>();
            IOController.LoadData<Product>();
            IOController.LoadData<VKAccount>();

            foreach (var com in VKCommunity.Communities)
                listBox5.Items.Add(com.Address);

            foreach (var prod in Product.Products)
                listBox2.Items.Add($"[{prod.ProductID}] {prod.Name} [{prod.Price} rub]");

            foreach (var acc in VKAccount.GetAccounts())
            {
                listBox1.Items.Add(acc.Credentials.Login);
            }
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

            IOController.UpdateSingleItem(new VKAccount(vkLogin, vkPassword));
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

            var prod = new Product(name, price, desc, photos);

            listBox2.Items.Add($"[{prod.ProductID}] {name} [{price} rub]");

            listBox3.Items.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox9.Clear();

            IOController.UpdateSingleItem(prod);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog()
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*JPEG)|*.BMP;*.JPG;*.GIF;*.JPEG",
                Multiselect = true
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var files = dialog.FileNames;

                foreach (var photoToAdd in files)
                {
                    bool photoExists = false;
                    foreach (var addedphoto in listBox3.Items)
                    {
                        if (addedphoto.ToString().Trim().ToLower().Equals(photoToAdd.ToLower().Trim()))
                        {
                            photoExists = true;
                            break;
                        }
                    }

                    if (photoExists) continue;
                    listBox3.Items.Add(photoToAdd);
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(listBox3.SelectedIndex == -1)
            {
                Notification.ShowNotification("Для удаления выберите фото в списке");
                return;
            }

            listBox3.Items.RemoveAt(listBox3.SelectedIndex);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if(Int32.TryParse(textBox7.Text, out int res)) Timestamp.CURRENT_LIMIT_PER_DAY = res;
            if (Int32.TryParse(textBox5.Text, out res)) Timestamp.CURRENT_REPEAT_TIME = res;

            foreach(var acc in VKAccount.GetAccounts())
                acc.Times = new Timestamp(TimeSpan.FromSeconds(Timestamp.CURRENT_REPEAT_TIME), Timestamp.CURRENT_LIMIT_PER_DAY);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex == -1)
            {
                Notification.ShowNotification("Выберите продукт для удаления");
                return;
            }

            var id = Convert.ToInt32(listBox2.Items[listBox2.SelectedIndex].ToString().Split(']')[0].Split('[')[1]);
            Product.DeleteProduct(id);
            listBox2.Items.Remove(listBox2.Items[listBox2.SelectedIndex]);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach(var dr in Marionette.Drivers)
                dr.Exit();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            foreach (var dr in Marionette.Drivers)
                dr.Exit();
        }
    }
}
