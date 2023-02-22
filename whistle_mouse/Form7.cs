using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace whistle_mouse
{
    public partial class Form7 : Form
    {
        public Form7()
        {
            InitializeComponent();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            this.Text = Form1.lang_txt[14 + Form1.lng_frm1];
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            string tmp_path = Form1.path;
            tmp_path = tmp_path + "info/tbw-"+Form1.lang+"/index.html";
            tmp_path = tmp_path.Replace("/", ((Char)92).ToString());

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = tmp_path;
            process.Start();
        }
    }
}
