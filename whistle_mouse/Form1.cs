using FftSharp;
using Microsoft.VisualBasic;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using InputManager;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Net.Http.Headers;
using NAudio.Gui;
using static System.Windows.Forms.LinkLabel;
using System.IO;
using System.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;
using System.Reflection.Metadata.Ecma335;
using System.Security.Policy;
using System.Diagnostics;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;

namespace whistle_mouse
{
    public partial class Form1 : Form
    {
        //Radek Ciesielski---------------------
        //-------------------------------------
        //Whistle Mouse------------------------

        //device
        NAudio.Wave.WaveInEvent? Wave;

        readonly double[] AudioValues;
        readonly double[] FftValues;

        readonly int SampleRate = 44100;
        readonly int BitDepth = 16;
        readonly int ChannelCount = 1;
        readonly int BufferMilliseconds = 20;

        public static Form1 fr1;
        public Form1()
        {
            InitializeComponent();
            fr1 = this;
            AudioValues = new double[SampleRate * BufferMilliseconds / 1000];
            double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
            double[] fftMag = FftSharp.Transform.FFTmagnitude(paddedAudio);
            FftValues = new double[fftMag.Length];
            double fftPeriod = FftSharp.Transform.FFTfreqPeriod(SampleRate, fftMag.Length);
        }

        //config variable********************************
        public static string lang = "en";
        public static bool start_windows = false;
        public static int device_number = 0;
        public static int out_device_number = 0;
        public static int f_left = 950;
        public static int f_right = 1250;
        public static int f_menu = 1450;
        public static int f_min = 800;
        public static int f_max = 1800;
        public static int w_buff = 6;
        public static int f_wait_d = 10;
        public static int timer_int = 20;

        //work variable*********************************
        public static string path;
        int[] buff;
        int[] buff1;
        int f_wait = 0;
        bool h_wait = false;
        int i_func = 0;
        bool i_func_mouse = false;
        bool first_offset = false;

        string f_act = "0";
        int f_act_ind = 0;

        string double_f = "";
        int tim_df, tim_df_f;
        int double_long, double_short;
        bool istimer_f = false;

        double[] func_1 = new double[3];

        public static string[] lang_txt;
        public static string[] device_list;
        public static string[] out_device_list;

        public static int lng_frm1;
        public static int lng_frm3;
        public static int lng_func;

        string[] f_code = new string[1];
        string[] f_name = new string[1];
        string[] f_command = new string[1];
        int f_find;
        string function;
        bool f_dynamic_graph = false;
        int screenWidth;
        int screenHeight;

        MMDeviceEnumerator deviceiterator = new MMDeviceEnumerator();
        MMDeviceCollection ndevices;
        float mute;

        int x_return, x_return2;
        int y_return, y_return2;
        int scroll_return;
        int dt_function;

        int[] t_ms = new int[1];
        int[] t_px = new int[1];
        int m_timer = 1;
        int scr_timer = 0;
        int[] scr_ms = new int[1];
        int[] scr_count = new int[1];

        bool moving;
        Point offset;
        Point original;
        Point pt = new Point();

        Form4 frm4 = new Form4();
        string[] tbox_name = new string[1];
        string[] tbox_command = new string[1];
        int[] tbox_speed = new int[1];
        int[] tbox_speed_count = new int[1];

        Form2 frm2;
        Form3 frm3;
        Form7 frm7;

        int d_state = 0;
        bool start_prg = true;

        int f_visual = 0;
        int def_pan_w = 0;
        int def_pan_h = 0;
        int def_pan1_h = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            //verify double process
            Process process = Process.GetCurrentProcess();
            var dupl = Process.GetProcessesByName(process.ProcessName);

            if (dupl.Length > 1)
            {
                Application.Exit();
            }

            //notify
            notifyIcon1.Visible = false;
            notifyIcon1.Text = "Whistle Mouse";
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            //open and hide tool box
            frm4.Show();
            frm4.Hide();

            //program path******************************************

            path = Path.GetDirectoryName(Application.ExecutablePath) + "/config/";
            //config load*****************************************
            cfg_load();

            //set buffor size***************************************
            buff = new int[w_buff];
            buff1 = new int[w_buff];

            //lang load*****************************************
            lang_load();

            //get in device********************************************
            if (NAudio.Wave.WaveIn.DeviceCount == 0)
            {
                MessageBox.Show(lang_txt[1 + lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                System.Windows.Forms.Application.Exit();
            }

            device_list = new string[NAudio.Wave.WaveIn.DeviceCount];
            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                var caps = NAudio.Wave.WaveIn.GetCapabilities(i);
                device_list[i] = caps.ProductName;

            }

            if (device_number + 1 > NAudio.Wave.WaveIn.DeviceCount)
            {
                MessageBox.Show(lang_txt[2 + lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                //open config
                device_number = 0;
                if (formIsExist("Form2") == false)
                {
                    frm2 = new Form2();
                    frm2.Show();
                }
                return;
            }

            label1.Text = device_list[device_number];

            if (Wave is not null)
            {
                Wave.StopRecording();
                Wave.Dispose();

                for (int i = 0; i < AudioValues.Length; i++)
                    AudioValues[i] = 0;
            }

            Wave = new NAudio.Wave.WaveInEvent()
            {
                DeviceNumber = device_number,
                WaveFormat = new NAudio.Wave.WaveFormat(SampleRate, BitDepth, ChannelCount),
                BufferMilliseconds = BufferMilliseconds
            };

            Wave.DataAvailable += WaveIn_DataAvailable;
            Wave.StartRecording();
            FormClosed += FftMonitorForm_FormClosed;

            //get out device**************************************
            ndevices = deviceiterator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            if (ndevices.Count == 0)
            {
                MessageBox.Show(lang_txt[1 + lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                //  System.Windows.Forms.Application.Exit();
                out_device_number = -1;// no audio output
            }
            else
            {
                out_device_list = new string[ndevices.Count];
                for (int i = 0; i < ndevices.Count; i++)
                {
                    out_device_list[i] = ndevices[i].DeviceFriendlyName;
                }
                if (out_device_number + 1 > ndevices.Count)
                {
                    MessageBox.Show(lang_txt[2 + lng_frm1], "Whistle mouse", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    //open config
                    out_device_number = 0;
                    if (formIsExist("Form2") == false)
                    {
                        frm2 = new Form2();
                        frm2.Show();
                    }
                    return;
                }
            }

            //function load*********************************************
            function_load();

            //set config and language************************************
            timer1.Enabled = true;

            //set button size and position
            panel1.Location = new Point(0, panel1.Location.Y);
            panel2.Location = new Point(238, panel2.Location.Y);
            panel3.Location = new Point(480, panel3.Location.Y);

            int[] fr_button = new int[3];
            fr_button[0] = f_left;
            fr_button[1] = f_right;
            fr_button[2] = f_menu;
            Array.Sort(fr_button);

            int tmp;
            tmp = (((((fr_button[1] - fr_button[0]) / 2) + fr_button[0]) - f_min) * panel4.Width) / (f_max - f_min);
            panel1.Width = tmp;

            panel2.Location = new Point(panel1.Width, panel2.Location.Y);
            tmp = ((((fr_button[2] - fr_button[1]) / 2) + fr_button[1] - f_min) * panel4.Width) / (f_max - f_min);
            panel2.Width = tmp - panel1.Width;

            panel3.Location = new Point(panel1.Width + panel2.Width, panel3.Location.Y);
            panel3.Width = panel4.Width - (panel1.Width + panel2.Width);

            label4.Text = f_left.ToString() + " Hz";
            label5.Text = f_right.ToString() + " Hz";
            label6.Text = f_menu.ToString() + " Hz";

            int[] bt_width = new int[3];
            int[] bt_lok = new int[3];
            bt_width[0] = panel1.Width;
            bt_width[1] = panel2.Width;
            bt_width[2] = panel3.Width;

            bt_lok[0] = panel1.Location.X;
            bt_lok[1] = panel2.Location.X;
            bt_lok[2] = panel3.Location.X;

            if (fr_button[0] == f_right)
            {
                panel2.Width = bt_width[0];
                panel2.Location = new Point(bt_lok[0], panel2.Location.Y);
            }
            if (fr_button[0] == f_menu)
            {
                panel3.Width = bt_width[0];
                panel3.Location = new Point(bt_lok[0], panel3.Location.Y);
            }

            if (fr_button[1] == f_left)
            {
                panel1.Width = bt_width[1];
                panel1.Location = new Point(bt_lok[1], panel1.Location.Y);
            }
            if (fr_button[1] == f_menu)
            {
                panel3.Width = bt_width[1];
                panel3.Location = new Point(bt_lok[1], panel3.Location.Y);
            }

            if (fr_button[2] == f_left)
            {
                panel1.Width = bt_width[2];
                panel1.Location = new Point(bt_lok[2], panel1.Location.Y);
            }
            if (fr_button[2] == f_right)
            {
                panel2.Width = bt_width[2];
                panel2.Location = new Point(bt_lok[2], panel2.Location.Y);
            }

            //set timer interval refresh
            timer1.Interval = timer_int;
            timer4.Interval = timer_int;

            //set lang
            toolStripStatusLabel1.Text = lang_txt[4 + lng_frm1];
            checkBox1.Text = lang_txt[6 + lng_frm1];
            checkBox2.Text = lang_txt[8 + lng_frm1];
            button1.Text = lang_txt[9 + lng_frm1];
            button2.Text = lang_txt[10 + lng_frm1];
            button3.Text = lang_txt[11 + lng_frm1];
            ustawieniaToolStripMenuItem.Text = lang_txt[12 + lng_frm1];
            funkcjeToolStripMenuItem.Text = lang_txt[13 + lng_frm1];
            informacjeToolStripMenuItem.Text = lang_txt[14 + lng_frm1];
            wyjœcieToolStripMenuItem.Text = lang_txt[15 + lng_frm1];
            frm4.sel_name(lang_txt[16 + lng_frm1]);

            //get screen resolution to max position function
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            screenHeight = Screen.PrimaryScreen.Bounds.Height;

            //legend generate
            legend_generate();

            //tool box mount
            frm4.load_function(tbox_name);

            //get state from file
            if (start_prg == true)
            {
                def_pan_w = panel4.Width;
                def_pan_h = panel4.Height;
                def_pan1_h = panel1.Height;
                start_prg = false;
                this.TopLevel = true;
                load_state();
            }

            //set function label color
            label7.BackColor = Color.DarkBlue;
        }

        private void function_load()
        {
            //set default data when data file is wrong
            t_ms[0]=0; t_px[0]=10;
            scr_ms[0] = 1; scr_count[0] = 1;
            tbox_speed[0] = 1; tbox_speed_count[0] = 1;

            //load function from file to variable
            string tmp;
            string tmp2;
            
            tmp = File.ReadAllText(Form1.path + "program_function.txt").Replace(((Char)13).ToString(), ""); ;
            tmp2 = File.ReadAllText(Form1.path + "function.txt").Replace(((Char)13).ToString(), ""); ;
            tmp = tmp + tmp2;
            tmp = tmp.Trim((char)10);

            string[] f_list = tmp.Split((Char)10);

            int s_count = -1;//scroll
            int m_count = -1;//mouse speed
            int f_count = -1;//function
            int tbox_count = -1;//tool box
            int ts_count = -1;//tool box speed

            for (int i = 0; i < f_list.Length; i++)
            {
                //if error
                string tmp_f_list = f_list[i];
                tmp_f_list = tmp_f_list.Replace(";", "");

                if (f_list[i].Length - tmp_f_list.Length == 0)
                {
                    f_list[i] = "//;;" + f_list[i];
                }
                else if (f_list[i].Length - tmp_f_list.Length == 1)
                {
                    f_list[i] = "//;" + f_list[i];
                }

                string[] f_tmp = f_list[i].Split(";");

                //listing function to variable
                if (f_tmp[0] == "MOUSE")

                {
                    m_count = m_count + 1;
                    Array.Resize(ref t_ms, m_count + 1);
                    Array.Resize(ref t_px, m_count + 1);

                    t_ms[m_count] = int.Parse(IsNumeric(f_tmp[1], "100"));
                    t_px[m_count] = int.Parse(IsNumeric(f_tmp[2], "1"));

                }
                else if (f_tmp[0] == "SCROLL")
                {
                    s_count = s_count + 1;
                    Array.Resize(ref scr_ms, s_count + 1);
                    Array.Resize(ref scr_count, s_count + 1);

                    scr_ms[s_count] = int.Parse(IsNumeric(f_tmp[1], "100"));
                    scr_count[s_count] = int.Parse(IsNumeric(f_tmp[2], "1"));
                }
                else if (f_tmp[0] == "TBOX_SPEED")
                {
                    ts_count = ts_count + 1;
                    Array.Resize(ref tbox_speed, ts_count + 1);
                    Array.Resize(ref tbox_speed_count, ts_count + 1);

                    tbox_speed[ts_count] = int.Parse(IsNumeric(f_tmp[1], "100"));
                    tbox_speed_count[ts_count] = int.Parse(IsNumeric(f_tmp[2], "1"));
                }
                else if (f_tmp[0] == "T_BOX")
                {
                    tbox_count = tbox_count + 1;
                    Array.Resize(ref tbox_name, tbox_count + 1);
                    Array.Resize(ref tbox_command, tbox_count + 1);

                    tbox_name[tbox_count] = f_tmp[1];
                    tbox_command[tbox_count] = f_tmp[2];
                }
                else if (f_tmp[0] == "F_TIME")
                {
                    if (f_tmp[1] == "FRQ3_LONG") { f_wait_d = int.Parse(IsNumeric(f_tmp[2], "19")); }
                    if (f_tmp[1] == "FRQ_SHORT") { double_short = int.Parse(IsNumeric(f_tmp[2], "19")); }
                    if (f_tmp[1] == "FRQ_TIMER") { tim_df_f = int.Parse(IsNumeric(f_tmp[2], "19")); }
                }
                else if (f_tmp[0] == "//")
                {
                    //com
                }
                else
                {
                    //master function
                    f_count = f_count + 1;
                    Array.Resize(ref f_code, f_count + 1);
                    Array.Resize(ref f_name, f_count + 1);
                    Array.Resize(ref f_command, f_count + 1);

                    f_code[f_count] = IsNumeric(f_tmp[0], "1");
                    f_name[f_count] = f_tmp[1];
                    f_command[f_count] = f_tmp[2];
                }
            }
        }
        public string IsNumeric(string value, string err)
        {
            //chceck numeric. If not then return err string
            string tmp = value;
            if (tmp.All(char.IsNumber) == false)
            {
                tmp = err;
            }
            return tmp;
        }

        private void legend_generate()
        {
            //this procedure generate legend from function file
            Panel l_pan = panel6.Controls.Find("l_pan", true).FirstOrDefault() as Panel;
            panel6.Controls.Remove(l_pan);

            Panel pan = new Panel();
            pan.Name = "l_pan";
            pan.AutoSize = true;

            panel6.Controls.Add(pan);

            int y_loc = -1;
            int x_loc = 0;
            for (int ii = 0; ii < f_code.Length; ii++)
            {
                y_loc = y_loc + 1;
                if (y_loc == 4) { y_loc = 0; x_loc = x_loc + 1; }

                for (int i = 0; i < f_code[ii].Length; i++)
                {
                    int c = int.Parse(f_code[ii].Substring(i, 1));
                    Panel pan_c = new Panel();

                    if (c == 1) { pan_c.BackColor = Color.Green; }
                    if (c == 2) { pan_c.BackColor = Color.Orange; }
                    if (c == 3) { pan_c.BackColor = Color.Red; }

                    pan_c.Height = 12;
                    pan_c.Width = 12;
                    pan_c.Location = new Point((x_loc * 240) + (i * 13), y_loc * 23);
                    pan.Controls.Add(pan_c);

                }

                Label lab = new Label();
                lab.AutoSize = false;
                lab.Width = 240 - (f_code[ii].Length * 13);
                lab.Font = new Font("Segoe UI", 8, FontStyle.Bold);
                lab.Location = new Point((x_loc * 240) + (f_code[ii].Length * 13), y_loc * 23);
                lab.Text = lng_name(f_name[ii]);

                pan.Controls.Add(lab);

            }
        }


        private void cfg_load()
        {
            //load configuration**************************************************************************************

            StreamReader sr = new StreamReader(path + "cfg.txt");
           
            lang = sr.ReadLine();//language
            start_windows = bool.Parse(sr.ReadLine());//start this program on start windows
            device_number = int.Parse(sr.ReadLine());//select device
            f_left = int.Parse(sr.ReadLine());//forward frequency
            f_right = int.Parse(sr.ReadLine());//backward frequency
            f_menu = int.Parse(sr.ReadLine());//function frequency
            f_min = int.Parse(sr.ReadLine());//min frequency
            f_max = int.Parse(sr.ReadLine());//max frequency
            w_buff = int.Parse(sr.ReadLine());//buffor length
            out_device_number = int.Parse(sr.ReadLine());//output device
            timer_int = int.Parse(sr.ReadLine());//timer ms

            sr.Close();
        }
        private void FftMonitorForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            //close and dispose device*******************************************************

            Wave.StopRecording();
            Wave.Dispose();

        }
        private void lang_load()
        {
            //language load from file*****************************************************************

            lang_txt = File.ReadAllLines(path + lang + ".txt");

            //form2 - configuration
            //line 1-14

            //form1 - default, message, all form name
            lng_frm1 = 13;

            //form3 - default and message
            lng_frm3 = 30;

            //function name
            lng_func = 40;
        }
        void WaveIn_DataAvailable(object? sender, NAudio.Wave.WaveInEventArgs e)
        {
            //data device***************************************************
            for (int i = 0; i < e.Buffer.Length / 2; i++)
                AudioValues[i] = BitConverter.ToInt16(e.Buffer, i * 2);
        }

        private void generate_graph(string code)
        {
            //this procedure generate color box actual run function
            Panel f_pan = panel5.Controls.Find("c_pan", true).FirstOrDefault() as Panel;
            panel5.Controls.Remove(f_pan);

            Panel pan = new Panel();
            pan.Name = "c_pan";

            pan.AutoSize = true;
            pan.Dock = DockStyle.Right;
            panel5.Controls.Add(pan);

            for (int i = 0; i < code.Length; i++)
            {
                int c = int.Parse(code.Substring(i, 1));
                Panel pan_c = new Panel();

                if (c == 1) { pan_c.BackColor = Color.Green; }
                if (c == 2) { pan_c.BackColor = Color.Orange; }
                if (c == 3) { pan_c.BackColor = Color.Red; }

                pan_c.Height = 25;
                pan_c.Width = 25;

                pan_c.Location = new Point(i * 26, 0);

                pan.Controls.Add(pan_c);
            }

        }

        private void save_state()
        {
            //save state position, always on top and style
            using (StreamWriter writer = new StreamWriter(path + "state.txt"))
            {
                writer.WriteLine(checkBox2.Checked);
                writer.WriteLine(d_state.ToString());
                writer.WriteLine(this.Location.X.ToString());
                writer.WriteLine(this.Location.Y.ToString());

            }
        }
        private void load_state()
        {
            //load state
            StreamReader sr = new StreamReader(path + "state.txt");
            bool top = bool.Parse(sr.ReadLine());
            d_state = int.Parse(sr.ReadLine());//STYLE

            if (d_state != 0)
            {
                this.Location = new Point(int.Parse(sr.ReadLine()), int.Parse(sr.ReadLine()));
            }

            sr.Close();

            if (d_state == 1) { button1_Click(null, null); }
            if (d_state == 2) { button2_Click(null, null); }
            if (d_state == 3) { button3_Click(null, null); }

            checkBox2.Checked = top;

            if (checkBox2.Checked == true)
            {
                this.TopMost = true;
            }
            else
            {
                this.TopMost = false;
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            //master timer to get frequency and recognition function

            //get peak frequency*********************************

            double[] paddedAudio = FftSharp.Pad.ZeroPad(AudioValues);
            double[] fftMag = FftSharp.Transform.FFTpower(paddedAudio);
            Array.Copy(fftMag, FftValues, fftMag.Length);

            // find the frequency peak
            int peakIndex = 0;
            for (int i = 0; i < fftMag.Length; i++)
            {
                if (fftMag[i] > fftMag[peakIndex])
                    peakIndex = i;
            }

            double fftPeriod = FftSharp.Transform.FFTfreqPeriod(SampleRate, fftMag.Length);
            double peakFrequency = fftPeriod * peakIndex;

            int loud_level;
            loud_level = (int)fftMag[peakIndex];
            if (loud_level <= 40) { loud_level = 40; }
            if (loud_level >= 90) { loud_level = 90; }
            toolStripProgressBar1.Value = loud_level;
            toolStripStatusLabel2.Text = lang_txt[5 + lng_frm1] + " - " + (((loud_level - 40) * 100) / 50).ToString();

            //Buffer***************************************************

            buff1 = buff;
            int v = buff[0];
            Array.Copy(buff, 1, buff1, 0, buff.Length - 1);
            buff1[buff1.Length - 1] = v;
            buff = buff1;
            buff[w_buff - 1] = (int)peakFrequency;

            //arrow move**********************************************

            int buff_peak;
            int ar_pos;
            buff_peak = ((buff[w_buff - 1] + buff[w_buff - 2]) / 2);
            ar_pos = ((buff_peak - f_min) * panel4.Width) / (f_max - f_min);
            if (buff_peak < f_min || buff_peak > f_max) { pictureBox1.Location = new Point(0, pictureBox1.Location.Y); label3.Text = ""; }
            else
            {
                label3.Text = ((int)peakFrequency).ToString() + "Hz";
                pictureBox1.Location = new Point(ar_pos, pictureBox1.Location.Y);

            }

            //detection frequency****************************************
            Array.Clear(func_1);

            //out of range frequency - go function
            for (int ii = 0; ii < w_buff; ii++)
            {
                //function type 11,22,33,31,13,1111 etc
                if ((buff[ii] <= f_min) || buff[ii] >= f_max)
                {
                    if (first_offset == false) { return; }

                    first_offset = false;
                    if (double_long > 0 && double_long < double_short)
                    {
                        label7.BackColor = Color.DarkOrange;
                        double_f = double_f + f_act;
                        double_long = 0;
                        tim_df = 0;
                        timer4.Enabled = true;

                    }

                    if (double_long > double_short)
                    {
                        label7.BackColor = Color.DarkBlue;
                        double_long = 0;
                        timer4.Enabled = false;
                        double_f = "";
                        tim_df = 0;

                    }

                    //applay function
                    if (f_act != "0" && f_act_ind > 1)
                    {

                        f_find = Array.IndexOf(f_code, f_act);
                        if (f_find != -1)
                        {
                            function = f_command[f_find];
                            label7.Text = lng_name(f_name[f_find]);
                            go_function(function, f_name[f_find], f_act);
                            generate_graph(f_act);
                            int tmp_h = label7.Height;
                            int tmp_w = label7.Width;
                            label7.AutoSize = false;
                            label7.Height = tmp_h;
                            label7.Width = tmp_w;

                            timer2.Enabled = true;
                        }
                    }
                    //reset function
                    scr_timer = 0;
                    m_timer = 1;
                    timer3.Interval = 1;
                    timer3.Enabled = false;
                    f_dynamic_graph = false;
                    f_wait = 0;
                    h_wait = false;
                    f_act = "0";
                    f_act_ind = 0;
                    return;
                }

                //mediana
                func_1[0] = func_1[0] + (buff[ii] - f_left);//backward
                func_1[1] = func_1[1] + (buff[ii] - f_right);//forward
                func_1[2] = func_1[2] + (buff[ii] - f_menu);//function
            }

            int tmps;
            for (int ii = 0; ii < 3; ii++)
            {
                tmps = ii + 1;
                func_1[ii] = func_1[ii] / w_buff;
                if (func_1[ii] < 0) { func_1[ii] = func_1[ii] * -1; }
                func_1[ii] = double.Parse(((int)func_1[ii]).ToString() + "," + tmps);

            }

            Array.Sort(func_1);
            func_1[0] = (int)(((decimal)func_1[0] % 1) * 100);
            func_1[0] = func_1[0] / 10; // this is actual whistle frequency 1 or 2 or 3
            first_offset = true;
           
            //detection function************************************************

            if (h_wait == true) { return; }


            if (f_act == "0")
            {
                f_act = func_1[0].ToString();
                f_act_ind = 1;
            }
            else
            {
                if (f_act.Length == f_act_ind)
                {

                    if (f_act.Substring(f_act_ind - 1) != func_1[0].ToString())
                    {
                        double_long = 0;
                        f_act = f_act + func_1[0].ToString();
                        f_act_ind = f_act_ind + 1;
                    }
                }

            }


            if (f_act_ind > 1) { return; }

            //function find***********************************************

            f_find = Array.IndexOf(f_code, f_act);
            if (f_find != -1)
            {
                function = f_command[f_find];
            }
            else
            {
                function = "";
            }


            //double function  11 or 22*******************************************************
            if (func_1[0] != 0)
            {
                double_long = double_long + 1;

            }


            //Dynamic Function*****************************************************************

            if (f_code[f_find] == "3")
            {
                function = "";
                if (f_wait == 1)
                {

                    function = f_command[f_find];
                    go_function(function, f_name[f_find], f_act);
                    label7.Text = lng_name(f_name[f_find]);
                    generate_graph(f_act);

                    int tmp_h = label7.Height;
                    int tmp_w = label7.Width;
                    label7.AutoSize = false;
                    label7.Height = tmp_h;
                    label7.Width = tmp_w;

                    timer2.Enabled = true;

                    h_wait = true;

                }
                if (f_wait == 0) { f_wait = f_wait_d; return; }
                f_wait = f_wait - 1;
            }
            else
            {
                f_wait = 0;
            }

            if (function == "BACKWARD" || function == "FORWARD")
            {
                pt = MousePosition;
                if (f_dynamic_graph == false)
                {
                    scroll_return = 0;

                    x_return = pt.X; y_return = pt.Y;

                    if (double_f.Length == 0)
                    {
                        x_return2 = pt.X; y_return2 = pt.Y;
                    }

                    label7.Text = lng_name(f_name[f_find]);
                    generate_graph(f_act);
                    f_dynamic_graph = true;
                }
            }

            if (checkBox1.Checked == false) { return; }

            if (function == "FORWARD")
            {
                dt_function = 1;
                timer3.Enabled = true;
            }

            if (function == "BACKWARD")
            {
                dt_function = 2;
                timer3.Enabled = true;
            }
        }


        public string lng_name(string name)
        {
            // replace name function from function list to set language
            string l_name = name;

            if (l_name.Contains("LNG_") == true)

            {
                l_name = l_name.Replace("LNG_", "");
                l_name = lang_txt[int.Parse(IsNumeric(l_name, "1")) + lng_func];
            }

            return l_name;
        }

        private void go_function(string function, string f_name, string f_af)
        {
            //master procedure with all function

            if (checkBox1.Checked == false && f_af != "3") { return; }

            //static function***************************************

            //Whistle function
            if (frm4.Visible == false || f_af == "000")
            {
                //back position mouse and scrool
                if (f_af.Substring(0, 1) != "3")
                {
                    if (istimer_f == true)
                    {
                        istimer_f = false;
                        scroll_return = scroll_return * 2;
                        x_return = x_return2;
                        y_return = y_return2;
                    }

                    if (i_func != 2)
                    {
                        Mouse.Move(x_return, y_return);
                    }
                    else if (scroll_return != 0)
                    {
                        if (scroll_return < 0)
                        {
                            for (int i = scroll_return; i < 0; i++)
                            {
                                Mouse.Scroll(Mouse.ScrollDirection.Up);
                            }
                        }
                        if (scroll_return > 0)
                        {
                            for (int i = 0; i < scroll_return; i++)
                            {
                                Mouse.Scroll(Mouse.ScrollDirection.Down);
                            }

                        }
                        scroll_return = 0;
                    }

                }

                //listing function
                string function_tmp = function;
                string[] f_list = function_tmp.Split("/*/");

                for (int i_f = 0; i_f < f_list.Length; i_f++)
                {
                    function = f_list[i_f];


                    //run function
                    if (function == "T_BOX")
                    {
                        frm4.Show();
                    }

                    if (function == "T_BOX_CLOSE")
                    {
                        frm4.Hide();
                    }

                    int mute_ONOFF = 0;//0- auto, 1 - ON, 2 - OFF
                    if (function == "STANDBY")
                    {
                        //mute + simulate on
                        if (checkBox1.Checked == true)
                        {
                            checkBox1.Checked = false;
                            mute_ONOFF = 1;
                            function = "MUTE";

                        }
                        else
                        {
                            checkBox1.Checked = true;
                            mute_ONOFF = 2;
                            function = "MUTE";
                        }
                    }
                    if (function == "STANDBY0")
                    {
                        checkBox1.Checked = false;
                    }
                    if (function == "STANDBY1")
                    {
                        checkBox1.Checked = true;
                    }

                    if (function == "MUTE0")
                    {
                        mute_ONOFF = 1;
                        function = "MUTE";
                    }
                    if (function == "MUTE1")
                    {
                        mute_ONOFF = 2;
                        function = "MUTE";
                    }

                    if (function == "RIGHT_MOUSE_CLICK")
                    {
                        Mouse.PressButton(Mouse.MouseKeys.Right);
                        i_func_mouse = false;
                    }

                    if (function == "MOUSE_CLICK")
                    {
                        Mouse.PressButton(Mouse.MouseKeys.Left);
                        i_func_mouse = false;
                    }

                    if (function == "DOUBLE_MOUSE_CLICK")
                    {
                        Mouse.ButtonDown(Mouse.MouseKeys.Left);
                        Mouse.ButtonUp(Mouse.MouseKeys.Left);
                        Mouse.ButtonDown(Mouse.MouseKeys.Left);
                        Mouse.ButtonUp(Mouse.MouseKeys.Left);
                        i_func_mouse = false;
                    }

                    if (function == "SCROLL")
                    {
                        toolStripStatusLabel3.Text = "SCROLL";

                        if (i_func != 2) { i_func = 2; }
                        else
                        {
                            function = "FLIP";
                            int n_find = Array.IndexOf(f_command, function);
                            if (n_find != -1)
                            {
                                label7.Text = lng_name(this.f_name[n_find]);

                            }

                        }
                    }

                    if (function == "FLIP")
                    {

                        if (i_func == 0 || i_func == 2) { i_func = 1; toolStripStatusLabel3.Text = "MOUSE - X"; } else { i_func = 0; toolStripStatusLabel3.Text = "MOUSE - Y"; }
                    }


                    if (function == "MOUSE_CLICK_HOLD")
                    {
                        if (i_func_mouse == false) { i_func_mouse = true; Mouse.ButtonDown(Mouse.MouseKeys.Left); } else { i_func_mouse = false; Mouse.ButtonUp(Mouse.MouseKeys.Left); }
                    }

                    if (function == "RIGHT_MOUSE_CLICK_HOLD")
                    {
                        if (i_func_mouse == false) { i_func_mouse = true; Mouse.ButtonDown(Mouse.MouseKeys.Right); } else { i_func_mouse = false; Mouse.ButtonUp(Mouse.MouseKeys.Right); }
                    }



                    if (function == "MAX_BACKWARD")
                    {
                        pt = MousePosition;
                        if (i_func == 0)
                        {
                            Mouse.Move(pt.X, 0);
                        }
                        if (i_func == 1)
                        {
                            Mouse.Move(0, pt.Y);
                        }
                        if (i_func == 2)
                        {
                            for (int i = 0; i < 200; i++)
                            {
                                Mouse.Scroll(Mouse.ScrollDirection.Up);
                            }
                        }

                    }

                    if (function == "MAX_FORWARD")
                    {
                        pt = MousePosition;
                        if (i_func == 0)
                        {
                            Mouse.Move(pt.X, screenHeight);
                        }
                        if (i_func == 1)
                        {
                            Mouse.Move(screenWidth, pt.Y);
                        }
                        if (i_func == 2)
                        {
                            for (int i = 0; i < 200; i++)
                            {
                                Mouse.Scroll(Mouse.ScrollDirection.Down);
                            }
                        }
                    }

                    if (function.Length > 5)
                    {
                        if (function.Substring(0, 4) == "CORD")
                        {
                            string f_params = function.Substring(5);
                            int f_coma = f_params.IndexOf(",", 0);
                            Mouse.Move(int.Parse(IsNumeric(f_params.Substring(0, f_coma), "100")), int.Parse(IsNumeric(f_params.Substring(f_coma + 1), "100")));
                        }
                        if (function.Substring(0, 4) == "FILE")
                        {
                            string f_params = function.Substring(5);
                            Process.Start(f_params);
                        }
                        if (function.Substring(0, 4) == "WAIT")
                        {
                            string f_params = function.Substring(5);
                            System.Threading.Thread.Sleep(int.Parse(IsNumeric(f_params, "100")));
                        }
                    }

                    if (function.Length > 8)
                    {
                        if (function.Substring(0, 7) == "SENDKEY")
                        {
                            string f_params = function.Substring(8);
                            SendKeys.Send(f_params);
                        }
                    }
                    if (function == "VOL+")
                    {
                        float tmp_vol = 0;

                        tmp_vol = get_volume();

                        if (tmp_vol > 0.9f && tmp_vol < 1f)
                        {

                            set_volume(1f);


                        }
                        if (tmp_vol < 0.9f)
                        {

                            set_volume(get_volume() + 0.1f);

                        }


                    }


                    if (function == "VOL-")
                    {
                        float tmp_vol = 0;


                        tmp_vol = get_volume();



                        if (tmp_vol < 0.1f && tmp_vol > 0f)
                        {

                            set_volume(0f);


                        }
                        if (tmp_vol > 0.1f)
                        {


                            set_volume(get_volume() - 0.1f);


                        }
                    }


                    if (function == "MUTE")
                    {
                        float tmp_mute = 0;

                        tmp_mute = get_volume();

                        if (tmp_mute != 0)
                        {
                            if (mute_ONOFF == 1) { goto LAB; }

                            mute = tmp_mute;

                            set_volume(0f);


                        }

                        if (tmp_mute == 0)
                        {
                            if (mute_ONOFF == 2) { goto LAB; }
                            if (mute == 0) { mute = 0.5f; }

                            set_volume(mute);

                        }
                    LAB:;
                        mute_ONOFF = 0;//auto
                    }
                    label2.Text = toolStripStatusLabel3.Text;
                }
            }
            else
            {
                //Tool box function
                if (f_af == "32")
                {
                    if (frm4.t_index != -1)
                    {
                        go_function(tbox_command[frm4.t_index], tbox_name[frm4.t_index], "000");
                        frm4.sel_f(tbox_name[frm4.t_index]);
                    }
                }
                if (f_af == "323")
                {
                    frm4.Hide();
                }
            }
        }

        private void set_volume(float vol)
        {
            if (out_device_number == -1) { return; }
            //foreach (MMDevice device in ndevices)
            //{//}
            ndevices[out_device_number].AudioEndpointVolume.MasterVolumeLevelScalar = vol;

        }
        public float get_volume()
        {
            float vol = 0;
            if (out_device_number == -1) { return vol; }
            //foreach (MMDevice device in ndevices)
            //{  }
            vol = ndevices[out_device_number].AudioEndpointVolume.MasterVolumeLevelScalar;


            return vol;
        }
        private void ustawieniaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //load configuration form**********************
            if (formIsExist("Form2") == false)
            {
                frm2 = new Form2();
                frm2.Show();
            }
        }
        public void refresh_frm1()
        {
            Form1_Load(null, null);
        }
        private void funkcjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //load configuration form**********************
            if (formIsExist("Form3") == false)
            {
                frm3 = new Form3();
                frm3.Show();
            }
        }
        public bool formIsExist(string frmOpen)
        {
            // check when form is open
            
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Name == frmOpen)
                {
                    return true;
                }
            }

            return false;
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            //timer to move mouse scrool and tool box list

            //is toolbox or not
            if (frm4.Visible == false)
            {
                //mouse
                if (i_func != 2)
                {
                    for (int i = 0; i < t_ms.Length; i++)
                    {
                        if (m_timer > t_ms[i]) { m_timer = m_timer + t_px[i]; break; }
                    }


                    if (dt_function == 1)
                    {
                        if (i_func == 0)
                        {

                            Mouse.Move(x_return, y_return + m_timer);
                        }
                        if (i_func == 1)
                        {

                            Mouse.Move(x_return + m_timer, y_return);
                        }

                    }
                    if (dt_function == 2)
                    {
                        if (i_func == 0)
                        {

                            Mouse.Move(x_return, y_return - m_timer);
                        }
                        if (i_func == 1)
                        {

                            Mouse.Move(x_return - m_timer, y_return);
                        }

                    }
                }
                //scroll
                else
                {

                    if (scr_timer == 0)
                    {
                        timer3.Interval = scr_ms[scr_timer];
                    }
                    if (scr_timer < scr_count.Length - 1)
                    {

                        if (m_timer >= scr_count[scr_timer])

                        {
                            scr_timer = scr_timer + 1;
                            timer3.Interval = scr_ms[scr_timer];
                            m_timer = 0;
                        }

                        m_timer = m_timer + 1;
                    }
                    if (dt_function == 2)
                    {
                        Mouse.Scroll(Mouse.ScrollDirection.Up);
                        // label1.Text = scroll_return.ToString();
                        scroll_return = scroll_return + 1;
                    }

                    if (dt_function == 1)
                    {
                        Mouse.Scroll(Mouse.ScrollDirection.Down);
                        scroll_return = scroll_return - 1;
                    }
                }
            }
            else
            {
                //is toolbox
                if (scr_timer == 0)
                {
                    timer3.Interval = tbox_speed[scr_timer];
                }
                if (scr_timer < tbox_speed_count.Length - 1)
                {

                    if (m_timer >= tbox_speed_count[scr_timer])

                    {
                        scr_timer = scr_timer + 1;
                        timer3.Interval = tbox_speed[scr_timer];
                        m_timer = 0;
                    }

                    m_timer = m_timer + 1;
                }
                if (dt_function == 2)
                {
                    frm4.list_down();
                }

                if (dt_function == 1)
                {
                    frm4.list_up();
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //set style 1
            this.TransparencyKey = DefaultBackColor;
            panel4.Width = 380;
            Form1_Load(null, null);
            label2.Visible = true;
            label1.Visible = false;
            statusStrip1.Visible = false;
            checkBox1.Visible = false;
            menuStrip1.Visible = false;
            panel6.Visible = false;
            button1.Visible = false;
            label8.Visible = true;
            button2.Visible = false;
            checkBox2.Visible = false;
            button3.Visible = false;
            this.FormBorderStyle = FormBorderStyle.None;
            //notify icon
            notifyIcon1.Visible = true;
            this.ShowInTaskbar = false;
            d_state = 1;
            save_state();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!moving)
                return;

            int x = original.X + MousePosition.X - offset.X;
            int y = original.Y + MousePosition.Y - offset.Y;
            this.Location = new Point(x, y);
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            moving = true;
            offset = MousePosition;
            original = this.Location;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //notify icon
            notifyIcon1.Visible = true;
            this.ShowInTaskbar = false;
            this.Visible = false;
            d_state = 3;
            save_state();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //set top most
            if (checkBox2.Checked == true)
            {

                this.TopMost = true;
            }
            else
            {

                this.TopMost = false;

            }
            save_state();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void backToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (d_state != 3)
            {
                panel1_MouseDoubleClick(null, null);
            }
            else
            {
                notifyIcon1.Visible = false;
                this.ShowInTaskbar = true;
                this.Visible = true;
                d_state = 0;
                save_state();

            }
        }

        private void wyjœcieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void informacjeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //load configuration form**********************
            if (formIsExist("Form7") == false)
            {
                frm7 = new Form7();
                frm7.Show();
            }

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //run animation when function is selected
            if (f_visual > 10)
            {

                label7.Width = label7.Width - 1;
            }
            else
            {

                label7.Width = label7.Width + 1;
            }
            f_visual = f_visual + 1;
            if (f_visual == 21) { f_visual = 0; timer2.Enabled = false; label7.AutoSize = true; }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            moving = false;
            save_state();
        
        }

        private void panel1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            //return to default style
            this.TransparencyKey = Color.Empty;
            panel4.Width = def_pan_w;
            panel4.Height = def_pan_h;
            Form1_Load(null, null);
            label1.Visible = true;
            statusStrip1.Visible = true;
            checkBox1.Visible = true;
            menuStrip1.Visible = true;
            panel6.Visible = true;
            button1.Visible = true;
            button2.Visible = true;
            label2.Visible = false;
            label3.Visible = true;
            label4.Visible = true;
            label5.Visible = true;
            label6.Visible = true;
            label8.Visible = false;
            panel5.Visible = true;
            checkBox2.Visible = true;
            button3.Visible = true;
            panel1.Height = def_pan1_h;
            panel2.Height = def_pan1_h;
            panel3.Height = def_pan1_h;
            pictureBox1.Height = def_pan1_h;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //notify icon
            notifyIcon1.Visible = false;
            this.ShowInTaskbar = true;
            d_state = 0;
            save_state();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            //set style 2
            this.TransparencyKey = DefaultBackColor;
            panel4.Width = 200;
            Form1_Load(null, null);
            label1.Visible = false;
            statusStrip1.Visible = false;
            checkBox1.Visible = false;
            menuStrip1.Visible = false;
            panel6.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label8.Visible = false;
            panel5.Visible = false;
            checkBox2.Visible = false;
            button3.Visible = false;
            panel1.Height = 20;
            panel2.Height = 20;
            panel3.Height = 20;
            pictureBox1.Height = 20;
            this.FormBorderStyle = FormBorderStyle.None;
            //notify icon
            notifyIcon1.Visible = true;
            this.ShowInTaskbar = false;
            d_state = 2;
            save_state();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            //timer find function 11,22,33 etc
            tim_df = tim_df + 1;

            if (tim_df > (tim_df_f / 2))
            {
                label7.BackColor = Color.IndianRed;
            }
       
            if (tim_df > tim_df_f)
            {
                label7.BackColor = Color.DarkBlue;
                double_long = 0;
                timer4.Enabled = false;
                tim_df = 0;
                if (double_f.Length < 2)
                {
                    double_f = "";
                }
                else
                {
                    label7.BackColor = Color.DarkBlue;
                    first_offset = true;
                    f_act = double_f;
                    f_act_ind = double_f.Length;
                    istimer_f = true;
                    double_f = "";
                }
            }

        }
    }
}