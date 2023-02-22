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

namespace whistle_mouse
{
    public partial class Form6 : Form
    {
        string path;
        public Form6(Exception? exception)
        {
            InitializeComponent();
            label1.Text = exception.Message;
        }

        private void Form6_Load(object sender, EventArgs e)
        {
            path = Path.GetDirectoryName(Application.ExecutablePath) + "/config/";
            path = path.Replace("/", ((Char)92).ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Process.Start("explorer.exe", path);
            System.Windows.Forms.Application.Exit();
        }

        private void Form6_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
    }
}
