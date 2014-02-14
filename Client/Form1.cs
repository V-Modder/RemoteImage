using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Drawing.Imaging;
using RedCorona.Net;

namespace Client
{
    public partial class Form1 : Form
    {
        private ClientInfo client;

        public Form1()
        {
            InitializeComponent();
        }

        public static byte[] UintToBytes(uint val)
        {
            byte[] res = new byte[4];
            for (int i = 3; i >= 0; i--)
            {
                res[i] = (byte)val; val >>= 8;
            }
            return res;
        }

        void SendImage(Image message)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                message.Save(memoryStream, message.RawFormat);
                byte[] ba = memoryStream.ToArray();
                client.Send(UintToBytes((uint)ba.Length));
                for (int i = 0; i < ba.Length; i++)
                {
                    byte[] b = new byte[1];
                    b[0] = ba[i];
                    client.Send(b);
                }
            }
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
            if(i != null)
                SendImage(i);
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void client_OnClose(ClientInfo ci)
        {
            client = null;
            lbl_connected.Text = "Getrennt";
            btn_connect.Text = "Verbinden";
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            Socket sock = Sockets.CreateTCPSocket(txt_serverip.Text, 2345);
            client = new ClientInfo(sock, false);
            client.OnClose +=new ConnectionClosed(client_OnClose);
            if (!client.Closed)
            {
                lbl_connected.Text = "Verbunden";
                btn_connect.Text = "Trennen";
            }
            else
            {
                lbl_connected.Text = "Getrennt";
                btn_connect.Text = "Verbinden";
            }
        }
    }
}
