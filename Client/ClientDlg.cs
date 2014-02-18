using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Client
{
    public partial class ClientDlg : Form
    {
        private TcpClient cln = new TcpClient();
        private System.Windows.Forms.Timer tmr_Check = new System.Windows.Forms.Timer();

        public ClientDlg()
        {
            InitializeComponent();
            cln.SendBufferSize = 1024;
            tmr_Check.Enabled = false;
            tmr_Check.Interval = 1000;
            tmr_Check.Tick += new EventHandler(tmr_Check_Tick);
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void btn_picture_Click(object sender, EventArgs e)
        {
            // Configure open file dialog box 
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.Filter = "";

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }

            dlg.Filter = String.Format("{0}{1}{2} ({3})|{3}", dlg.Filter, sep, "All Files", "*.*");

            dlg.DefaultExt = ".png"; // Default file extension 

            // Show open file dialog box 
            DialogResult result = dlg.ShowDialog();

            // Process open file dialog box results 
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SendImage(Image.FromFile(dlg.FileName));
            }
        }

        private void btn_snipping_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            System.Threading.Thread.Sleep(300);
            Image i = SnippingTool.Snip(Screen.FromControl(this));
            if (i != null)
                SendImage(i);
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void ChangeUI()
        {
            MethodInvoker LabelUpdate = delegate
            {
                if (IsConnected(cln.Client))
                {
                    lbl_connected.ForeColor = Color.Black;
                    lbl_connected.Text = "Verbunden";
                    btn_connect.Text = "Trennen";
                    btn_picture.Enabled = true;
                    btn_snipping.Enabled = true;
                }
                else
                {
                    tmr_Check.Stop();
                    lbl_connected.ForeColor = Color.Black;
                    lbl_connected.Text = "Getrennt";
                    btn_connect.Text = "Verbinden";
                    btn_picture.Enabled = false;
                    btn_snipping.Enabled = false;
                }
            };
            Invoke(LabelUpdate);
        }

        private void Connect()
        {
            lbl_connected.ForeColor = Color.Black;
            lbl_connected.Text = "Verbinde...";
            new Thread(delegate()
            {
                try
                {
                    cln = new TcpClient(txt_serverip.Text, 2345);
                    MethodInvoker LabelUpdate = delegate
                    {
                        tmr_Check.Start();
                    };
                    Invoke(LabelUpdate);
                    ChangeUI();
                }
                catch (Exception ee)
                {
                    tmr_Check.Stop();
                    
                    string s;
                    if (ee.Message.Contains(":2345"))
                        s = ee.Message.Remove(ee.Message.IndexOf(":2345"), 5);
                    else
                        s = ee.Message;
                    MethodInvoker LabelUpdate = delegate
                    {
                        lbl_connected.ForeColor = Color.Red;
                        lbl_connected.Text = s;
                    };
                    Invoke(LabelUpdate);
                    cln.Close();
                }
            }).Start();
        }

        private bool IsConnected(Socket socket)
        {
            try
            {
                return !(socket.Poll(1, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException) { return false; }
        }

        private byte[] ObjectToByteArray(Object pObject)
        {
            MemoryStream hStream = new MemoryStream();
            BinaryFormatter hFormatter = new BinaryFormatter();
            hFormatter.Serialize(hStream, pObject);
            return hStream.ToArray();
        }

        private string[] write(byte[] b)
        {
            List<string> a = new List<string>();
            for (int i = 0; i < b.Length; i++)
            {
                a.Add(Convert.ToInt32(b[i]).ToString() + "\n");
            }
            return a.ToArray();
        }

        private void SendImage(Image message)
        {
            try
            {
                byte[] imgArray = ObjectToByteArray(message);
                byte[] arr = new byte[1024];
                byte[] ar = new byte[12];
                StreamWriter sw = new StreamWriter(@"C:\client.txt");
                sw.Write(write(imgArray));
                sw.Close();
                Array.Copy(UintToBytes((uint)imgArray.Length), 0, ar, 0, 4);
                Array.Copy(UintToBytes((uint)message.Width), 0, ar, 4, 4);
                Array.Copy(UintToBytes((uint)message.Height), 0, ar, 8, 4);
                //MessageBox.Show(imgArray.Length.ToString());
                cln.Client.Send(ar, 12, SocketFlags.None);
                int i;
                for (i = 0; i < imgArray.Length; i++)
                {
                    if (i % 1024 == 0 && i != 0)
                    {
                        cln.Client.Send(arr);
                        if (imgArray.Length - i < 1024)
                        {
                            arr = new byte[imgArray.Length - i];
                        }
                        else
                        {
                            arr = new byte[1024];
                        }
                    }
                    arr[i % 1024] = imgArray[i];
                }
                if (i % 1024 != 0)
                {
                    Thread.Sleep(4000);
                    cln.Client.Send(arr);
                }
            }
            catch (Exception e)
            {
                lbl_connected.ForeColor = Color.Red;
                lbl_connected.Text = e.Message;
            }
        }

        private void tmr_Check_Tick(object sender, EventArgs e)
        {
            ChangeUI();
        }

        private void txt_serverip_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                Connect();
        }

        private byte[] UintToBytes(uint val)
        {
            byte[] res = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                res[i] = (byte)val; val >>= 8;
            }
            return res;
        }
    }
}
