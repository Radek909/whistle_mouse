using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace whistle_mouse
{
    public partial class Form5 : Form
    {
        string path;
        public Form5(Exception exp)
        {
            InitializeComponent();
            label1.Text =exp.Message;
        }

        private void Form5_Load(object sender, EventArgs e)
        {
            path = Path.GetDirectoryName(Application.ExecutablePath) + "/config/";
            path = path.Replace("/", ((Char)92).ToString());
            

        }

        private void Form5_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

   
        private void button1_Click(object sender, EventArgs e)
        {
           
            Process.Start("explorer.exe", path);
            System.Windows.Forms.Application.Exit();
        }
    }
}
