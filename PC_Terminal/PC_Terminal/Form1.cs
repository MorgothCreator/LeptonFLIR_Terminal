using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace PC_Terminal
{
    public partial class Form1 : Form
    {
        String StringReceive = "";
        UInt32 receive_buff_cnt = 0;
        byte[] receive_buff = new byte[9600];
        Color[] colors = new Color[1280];
        Int32 gen_median = 0;

        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BautRateSelector.Text = "921600";
            ComPortSelector.Items.Clear();
            foreach (string s in SerialPort.GetPortNames())
                ComPortSelector.Items.Add(s);
            if (ComPortSelector.Items.Count > 0) ComPortSelector.SelectedIndex = 0;
            else
            {
                MessageBox.Show(this, "There are no COM Ports detected on this computer.\nPlease install a COM Port and restart this app.", "No COM Ports Installed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            int i;
            for (i = 0; i <= 255; i++)
            {
                //black to blue
                colors[i] = Color.FromArgb(0, 0, i);
                // blue to cyan (0,0,255)
                colors[i + 256] = Color.FromArgb(0, i, 255);
                // Cyan to green (0,255,255)
                colors[i + 512] = Color.FromArgb(0, 255, 255 - i);
                // Green to yellow (0,255,0)
                colors[i + 768] = Color.FromArgb(i, 255, 0);
                // Yellow (255,255,0) to Red (255,0,0)
                colors[i + 1024] = Color.FromArgb(255, 255 - i, 0);
            }
            colors[0] = Color.FromArgb(0, 0, 0); //black

            listBox1.SelectedIndex = 0;
        }

        private void BtnOnOff_Click(object sender, EventArgs e)
        {
            if (BtnOnOff.Text == "Open comm")
            {
                try
                {
                    serialPort1.PortName = ComPortSelector.Text;
                    serialPort1.BaudRate = System.Convert.ToInt32(BautRateSelector.Text);
                    serialPort1.DataBits = 8;
                    serialPort1.StopBits = StopBits.One;
                    serialPort1.Parity = Parity.None;
                    serialPort1.Handshake = Handshake.None;
                    serialPort1.ReceivedBytesThreshold = 8;
                    serialPort1.WriteBufferSize = 256;
                    serialPort1.ReadBufferSize = 9600;
                    serialPort1.ReadTimeout = -1;
                    serialPort1.WriteTimeout = -1;
                    serialPort1.DtrEnable = true;
                    serialPort1.RtsEnable = true;
                    serialPort1.Open();
                    timer1.Enabled = true;
                    BtnOnOff.Text = "Close comm";
                    ComPortSelector.Enabled = false;
                    BautRateSelector.Enabled = false;
                }
                catch
                {
                    MessageBox.Show(this, "Cannot open this comm.\nIs possible to be used by another aplication.", "Open comm error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    timer1.Enabled = false;
                    serialPort1.Close();
                    BtnOnOff.Text = "Open comm";
                    ComPortSelector.Enabled = true;
                    BautRateSelector.Enabled = true;
                }
                catch
                {
                    MessageBox.Show(this, "Cannot close this comm.\nIs possible to be unpluged usb adaptor", "Open comm error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                int bytes = serialPort1.BytesToRead;
                if (bytes != 0)
                {
                    byte[] rec_buff = new byte[bytes];
                    serialPort1.Read(rec_buff, 0, bytes);
                    int cnt = 0;
                    for (cnt = 0; cnt < bytes; cnt++)
                    {
                        if (receive_buff_cnt < 9600)
                        {
                            receive_buff[receive_buff_cnt++] = System.Convert.ToByte(rec_buff[cnt]);
                        }
                    }
                    timer2.Enabled = false;
                    timer2.Enabled = true;
                }
            }
            catch
            {
                try
                {
                    timer1.Enabled = false;
                    serialPort1.Close();
                    BtnOnOff.Text = "Open comm";
                    ComPortSelector.Enabled = true;
                    BautRateSelector.Enabled = true;
                }
                catch
                {
                    MessageBox.Show(this, "Cannot close this comm.\nIs possible to be unpluged usb adaptor", "Open comm error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                MessageBox.Show(this, "Com port was closed.", "Comm closed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void timer4_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            int y, x;

            String A = StringReceive;
            StringReceive = "";
            Int32 median = 0; 
            if (listBox1.SelectedIndex == 0)
            {
                median = 65536;
            }
            else if (listBox1.SelectedIndex == 1 || listBox1.SelectedIndex == 2)
            {
                median = 0;
            }
            int cnt = 0;
            for (y = 0; y < 60; y++)
            {
                for (x = 0; x < 80; x++)
                {
                    try
                    {
                        int data;
                        Color newColor;
                        //if (((int)receive_buff[cnt] & 0xC0) != 0)
                            //cnt++;
                        data = (int)(receive_buff[cnt] << 8) + (receive_buff[cnt + 1]);
                        cnt += 2;
                        if (color_palete.Checked == false)
                        {
                            if (listBox1.SelectedIndex == 0)
                            {
                                if (median > data)
                                {
                                    median = data;
                                }
                                data = data - gen_median;
                            }
                            else if (listBox1.SelectedIndex == 1)
                            {
                                if (median < data)
                                {
                                    median = data;
                                }
                                data = 255 - (gen_median - data);
                            }
                            else if (listBox1.SelectedIndex == 2)
                            {
                                median += data;
                                data = data - gen_median;
                                data = data + 128;
                            }
                            else if (listBox1.SelectedIndex == 3)
                            {
                                data = (8192 - data) / System.Convert.ToInt32(numericUpDown3.Value);
                                data += System.Convert.ToInt32(trackBar1.Value);
                                data = 255 - data;
                            }
                            if (data > 255)
                            {
                                data = 255;
                            }
                            if (data < 0)
                            {
                                data = 0;
                            }
                            newColor = Color.FromArgb(data, data, data);
                        }
                        else
                        {
                            if (listBox1.SelectedIndex == 0)
                            {
                                if (median > data)
                                {
                                    median = data;
                                }
                                data = data - gen_median;
                            }
                            else if (listBox1.SelectedIndex == 1)
                            {
                                if (median < data)
                                {
                                    median = data;
                                }
                                data = 1280 - (gen_median - data);
                            }
                            else if (listBox1.SelectedIndex == 2)
                            {
                                median += data;
                                data = data - gen_median;
                                data = data + 128;
                            }
                            else if (listBox1.SelectedIndex == 3)
                            {
                                data = (8192 - data) / System.Convert.ToInt32(numericUpDown3.Value);
                                data += System.Convert.ToInt32(trackBar1.Value) + ((1279 -128) / 2);
                                data = 1279 - data;
                            }
                            if (data > 1279)
                                data = 1279;
                            if (data < 0)
                                data = 0;
                            newColor = colors[data];
                        }
                        if (numericUpDown2.Value == 1)
                        {
                            ((Bitmap)pictureBox1.Image).SetPixel(x, y, newColor);
                        }
                        else if(numericUpDown2.Value == 2)
                        {
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 2), (y * 2), newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 2) + 1, (y * 2), newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 2), (y * 2) + 1, newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 2) + 1, (y * 2) + 1, newColor);
                        }
                        else if(numericUpDown2.Value == 3)
                        {
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3), (y * 3), newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3) + 1, (y * 3), newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3) + 2, (y * 3), newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3), (y * 3) + 1, newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3) + 1, (y * 3) + 1, newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3) + 2, (y * 3) + 1, newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3), (y * 3) + 2, newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3) + 1, (y * 3) + 2, newColor);
                            ((Bitmap)pictureBox1.Image).SetPixel((x * 3) + 2, (y * 3) + 2, newColor);
                        }
                    }
                    catch { };
                }
            }
            if (listBox1.SelectedIndex == 0)
            {
                gen_median = median;
                label6.Text = "Coldest value: " + gen_median;
            }
            else if (listBox1.SelectedIndex == 1)
            {
                gen_median = median;
                label6.Text = "Heatest value: " + gen_median;
            }
            else if (listBox1.SelectedIndex == 2)
            {
                gen_median = median / (80 * 60);
                label6.Text = "Median value: " + gen_median;
            }
            else if (listBox1.SelectedIndex == 3)
            {
                label6.Text = "";
            }
            receive_buff_cnt = 0;
            label1.Text = "" + trackBar1.Value;
            int clt_clear = 0;
            for (; clt_clear < 9600; clt_clear++)
            {
                receive_buff[clt_clear++] = 0;
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDown2.Value == 1)
            {
                pictureBox1.Width = 80;
                pictureBox1.Height = 60;
                //pictureBox1.Location.X = (370 / 2) - 40;
            }
            else if (numericUpDown2.Value == 2)
            {
                pictureBox1.Width = 160;
                pictureBox1.Height = 120;
                //pictureBox1.Location.X = (370 / 2) - 80;
            }
            else if (numericUpDown2.Value == 3)
            {
                pictureBox1.Width = 240;
                pictureBox1.Height = 180;
                //pictureBox1.Location.X = (370 / 2) - 120;
            }
        }
    }
}