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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string vkLogin = textBox1.Text.Trim();
            string vkPassword = textBox2.Text.Trim();

            if(VKAccount.AddAccount(vkLogin, vkPassword) == false) 
        }
    }
}
