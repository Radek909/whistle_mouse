using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace whistle_mouse
{
    public partial class Form4 : Form
    {

        private static Form4 tollbox;

        public Form4()
        {
            InitializeComponent();
            tollbox = this;
            this.TopLevel = true;
            this.TopMost = true;
        }

        public static int q;

        public int t_index;
       
        public  void load_function(string[] f_name)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(f_name);
        }
        public void sel_f(string s_name)
        {
            label5.Text = s_name;
        }
        public void sel_name(string s_name)
        {
            this.Text= s_name;
        }
        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        public void list_up()
        {
            if (listBox1.Items.Count > 0)    
            {
                if (listBox1.SelectedIndex < listBox1.Items.Count - 1)
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex + 1;
                }
            }
        }
        public void list_down()
        {
            if (listBox1.Items.Count > 0)
            {
                if (listBox1.SelectedIndex >0)
                {
                    listBox1.SelectedIndex = listBox1.SelectedIndex - 1;
                }
            }
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            t_index= listBox1.SelectedIndex;
        }
    }
}
