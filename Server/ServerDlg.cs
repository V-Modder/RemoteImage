using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Org.Vesic.WinForms;
using RDavey.Net;

namespace Server
{
    public partial class ServerDlg : Form
    {
        private FormState fs = new FormState();
        private AsyncTcpServer svr;
        private List<PictureBox> pictures = new List<PictureBox>();
        private int expectedBytes = 0;
        private int byteCount = 0;
        private Size imgSize;
        private byte[] ba;

        public ServerDlg()
        {
            InitializeComponent();
            #if (!DEBUG)
            this.Size = new System.Drawing.Size(Screen.FromControl(this).Bounds.Width, Screen.FromControl(this).Bounds.Height);
            btn_exit.Location = new Point(Screen.FromControl(this).Bounds.Width - 40, 1);
            fs.Maximize(this);
            #endif
            svr = new AsyncTcpServer(getLocalAddress(), 2345);
            svr.OnRecieve += new RecieveEvent(svr_OnRecieve);
            svr.Start();
        }

        private object ByteArrayToObject(byte[] pArray)
        {
            MemoryStream hStream = new MemoryStream(pArray);
            BinaryFormatter hFormatter = new BinaryFormatter();
            hStream.Position = 0;
            return hFormatter.Deserialize(hStream);
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            svr.Stop();
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (PictureBox p in pictures)
                this.Controls.Remove(p);
            pictures.Clear();
            svr.Stop();
            fs.Restore(this);
        }

        private int GetInt(byte[] ba)
        {
            int r = 0;
            for (int i = 0; i < 4; i++)
                r += ba[i] << ((4 - i - 1) * 8);
            return r;
        }

        private IPAddress getLocalAddress()
        {
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            return null;
        }

        private bool isEmpty(byte[] bytes)
        {
            for (long i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] > 0)
                    return false;
            }
            return true;
        }

        private void svr_OnRecieve(Client ci, byte[] bytes)
        {
            if (byteCount == 0)
            {
                if (isEmpty(bytes))
                    return;
                byte[] b = new byte[4];
                for (int x = 0; x < 4; x++)
                    b[x] = bytes[x];
                byteCount = GetInt(b);
                for (int x = 4; x < 8; x++)
                    b[x - 4] = bytes[x];
                imgSize = new Size();
                imgSize.Width = GetInt(b);
                for (int x = 8; x < 12; x++)
                    b[x - 8] = bytes[x];
                imgSize.Height = GetInt(b);
                
                ba = new byte[byteCount];
                expectedBytes = 0;
            }
            else
            {
                if (isEmpty(bytes))
                    return;
                int i = 0;
                while (i < bytes.Length && expectedBytes < byteCount)
                {
                    ba[expectedBytes++] = bytes[i++];
                }

                if (expectedBytes >= byteCount)
                {
                    //Images.ImageSerializable imgs = new Images.ImageSerializable();
                    //BinaryFormatter formattor = new BinaryFormatter();
                    //MemoryStream ms = new MemoryStream(ba);
                    //Image img = Image.FromStream(ms);
                    //imgs = (Images.ImageSerializable)formattor.Deserialize(ms);
                    //Image img = imgs.Img; 
                    Image img = (Image)ByteArrayToObject(ba);
                    PictureBox p = new PictureBox();
                    p.Location = new System.Drawing.Point((this.Size.Width - imgSize.Width) / 2, (this.Size.Height - imgSize.Height) / 2);
                    p.Name = "newone";
                    p.Size = new Size(imgSize.Width, imgSize.Height);
                    p.Image = img;
                    pictures.Insert(pictures.Count, p);
                    MethodInvoker LabelUpdate = delegate
                    {
                        this.Controls.Add(pictures[pictures.Count - 1]);
                        pictures[pictures.Count - 1].BringToFront();
                    };
                    Invoke(LabelUpdate);
                    byteCount = 0;
                }
            }
        }
    }
}