using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace whistle_mouse
{
    public partial class Form2 : Form
    {
       

        public Form2()
        {

            InitializeComponent();
            
            

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            this.TopLevel = true;
            this.TopMost = true;
            //devices list***********************************

            comboBox2.Items.Clear();
            for (int i = 0; i < Form1.device_list.Length; i++)
            {
                comboBox2.Items.Add(Form1.device_list[i]);
              
            }
            comboBox2.SelectedIndex = Form1.device_number;

            comboBox3.Items.Clear();
            for (int i = 0; i < Form1.out_device_list.Length; i++)
            {
                comboBox3.Items.Add(Form1.out_device_list[i]);

            }
            comboBox3.SelectedIndex = Form1.out_device_number;
            //language load**********************************
            comboBox1.Items.Clear();
            string line;
            StreamReader sr = new StreamReader(Form1.path + "lang.txt");
            while ((line = sr.ReadLine()) != null)
            {
                comboBox1.Items.Add(line.ToString());
            }
            sr.Close();
            comboBox1.SelectedItem = Form1.lang;

            label10.Text = Form1.lang_txt[1];
            label1.Text = Form1.lang_txt[2];
            label2.Text = Form1.lang_txt[3];
            label3.Text = Form1.lang_txt[4];
            label4.Text = Form1.lang_txt[5];


            label6.Text = Form1.lang_txt[6];
            label7.Text = Form1.lang_txt[7];
            label14.Text = Form1.lang_txt[8];
           

            checkBox1.Text = Form1.lang_txt[9];
            label9.Text = Form1.lang_txt[10];

            button1.Text= Form1.lang_txt[11];
            button2.Text= Form1.lang_txt[12];

            this.Text= Form1.lang_txt[12 + Form1.lng_frm1];

            //config load*************************************
           
            checkBox1.Checked = Form1.start_windows;
            textBox1.Text = Form1.f_left.ToString();
            textBox2.Text = Form1.f_right.ToString();
            textBox3.Text = Form1.f_menu.ToString();
            textBox4.Text = Form1.f_min.ToString();
            textBox5.Text = Form1.f_max.ToString();
         
            textBox6.Text = Form1.w_buff.ToString();
          
            textBox8.Text = Form1.timer_int.ToString();
          
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //set default*********************************
            checkBox1.Checked = false;
            textBox1.Text = "950";
            textBox2.Text ="1250";
            textBox3.Text = "1450";
            textBox4.Text = "800";
            textBox5.Text = "1700";
        
            textBox6.Text = "10";
            
            textBox8.Text = "15";
        }
        public string IsNumeric(string value,string err)
        {
            string tmp = value;
            if (tmp.All(char.IsNumber)==false)
            {
                tmp = err;
            }
            return tmp;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //save and applay***************************************

            if (comboBox1.SelectedIndex < 0) { comboBox1.SelectedIndex = 0; }
            if (comboBox2.SelectedIndex < 0) { comboBox2.SelectedIndex = 0; }
            if (comboBox3.SelectedIndex < 0) { comboBox3.SelectedIndex = 0; }

            using (StreamWriter writer = new StreamWriter(Form1.path + "cfg.txt"))
            {
                writer.WriteLine(comboBox1.SelectedItem);
                writer.WriteLine(checkBox1.Checked.ToString());
                writer.WriteLine(comboBox2.SelectedIndex.ToString());
                writer.WriteLine(IsNumeric(textBox1.Text,"950"));
                writer.WriteLine(IsNumeric(textBox2.Text, "1250"));
                writer.WriteLine(IsNumeric(textBox3.Text, "1450"));
                writer.WriteLine(IsNumeric(textBox4.Text, "800"));
                writer.WriteLine(IsNumeric(textBox5.Text, "1700"));
               
                writer.WriteLine(IsNumeric(textBox6.Text, "10"));
                writer.WriteLine(comboBox3.SelectedIndex.ToString());
                writer.WriteLine(IsNumeric(textBox8.Text, "15"));
              

            }
            //start program when windows start

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkBox1.Checked == true)
                rk.SetValue("Whistle Mouse", Application.ExecutablePath);
            else
                rk.DeleteValue("Whistle Mouse", false);

            Form1.fr1.refresh_frm1();

            MessageBox.Show(Form1.lang_txt[3+Form1.lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            Form2_Load(null, null);
        }

       
    }
}
