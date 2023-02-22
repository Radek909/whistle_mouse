using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace whistle_mouse
{
    public partial class Form3 : Form
    {

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            this.TopLevel = true;
            this.TopMost = true;

            //language load**********************************


            tabControl1.TabPages[0].Text = Form1.lang_txt[1+Form1.lng_frm3];
            tabControl1.TabPages[1].Text = Form1.lang_txt[2 + Form1.lng_frm3];

            linkLabel1.Text= Form1.lang_txt[3 + Form1.lng_frm3];
            linkLabel2.Text = Form1.lang_txt[3 + Form1.lng_frm3];

            button4.Text= Form1.lang_txt[4 + Form1.lng_frm3];
            button3.Text = Form1.lang_txt[4 + Form1.lng_frm3];

            label7.Text = Form1.lang_txt[5 + Form1.lng_frm3];

            button1.Text = Form1.lang_txt[6 + Form1.lng_frm3];
            button2.Text = Form1.lang_txt[7 + Form1.lng_frm3];
            button5.Text = Form1.lang_txt[7 + Form1.lng_frm3];

            label4.Text = Form1.lang_txt[8 + Form1.lng_frm3];
            label10.Text = Form1.lang_txt[9+ Form1.lng_frm3];


            this.Text = Form1.lang_txt[13 + Form1.lng_frm1];
            //load function set***************************
            textBox2.Text= File.ReadAllText(Form1.path  + "program_function.txt");
            textBox1.Text = File.ReadAllText(Form1.path + "function.txt");

            
        }
        Point pt = new Point();
        private void timer1_Tick(object sender, EventArgs e)
        {
            GetCursorPos(ref pt);
            textBox3.Text = pt.X.ToString();
            textBox4.Text = pt.Y.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //unlock
            textBox2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s = Properties.Resources.program_function;
            //restroe function program****************************************
            DialogResult dr = MessageBox.Show(Form1.lang_txt[7 + Form1.lng_frm1], "Whistle mouse", MessageBoxButtons.YesNo);
            switch (dr)
            {
                case DialogResult.Yes:
                    textBox2.Text = s;
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string s = Properties.Resources.function;
            //restroe function****************************************
            DialogResult dr = MessageBox.Show(Form1.lang_txt[7 + Form1.lng_frm1], "Whistle mouse", MessageBoxButtons.YesNo);
            switch (dr)
            {
                case DialogResult.Yes:
                    textBox1.Text = s;
                    break;
                case DialogResult.No:
                    break;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //save function *********************************
            string tmp = textBox1.Text;
            tmp = tmp.Replace(((Char)13).ToString(), "");
            tmp = tmp.Trim((char)10);
            tmp = tmp.Replace(((Char)10).ToString(), ((Char)13).ToString() + ((Char)10).ToString());
            tmp = tmp + ((Char)13).ToString() + ((Char)10).ToString();

            File.WriteAllText(Form1.path + "function.txt", tmp,Encoding.UTF8);
          
            MessageBox.Show(Form1.lang_txt[3 + Form1.lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            Form1.fr1.refresh_frm1();

           

            Form3_Load(null, null);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //save function *********************************
            if (textBox2.Enabled == false) { return; }
            string tmp = textBox2.Text;
            tmp = tmp.Replace(((Char)13).ToString(), "");
            tmp = tmp.Trim((char)10);
            tmp = tmp.Replace(((Char)10).ToString(), ((Char)13).ToString()+ ((Char)10).ToString());
            tmp = tmp + ((Char)13).ToString() + ((Char)10).ToString();

            File.WriteAllText(Form1.path + "program_function.txt", tmp, Encoding.UTF8);
            Form1.fr1.refresh_frm1();

        
            MessageBox.Show(Form1.lang_txt[3 + Form1.lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            Form3_Load(null, null);
        }

    

        private void html_page()
        {

            string tmp_path = Form1.path;
            tmp_path = tmp_path + "info/tbw-"+Form1.lang+"/lista_funkcji.html";
            tmp_path = tmp_path.Replace("/", ((Char)92).ToString());

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = true;
            process.StartInfo.FileName = tmp_path;
            process.Start();
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            html_page();
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            html_page();
        }
    }
}
