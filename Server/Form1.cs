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
using System.Threading;
using Org.Vesic.WinForms;
using RedCorona.Net;

namespace Server
{
    public partial class Form1 : Form
    {
        private FormState fs = new FormState();
        private RedCorona.Net.Server server;
        private List<PictureBox> pictures = new List<PictureBox>();
        private int expectedBytes = 0;
        private byte[] ba;

        public Form1()
        {
            InitializeComponent();
            //fs.Maximize(this);
            //this.Size = new System.Drawing.Size(Screen.FromControl(this).Bounds.Width, Screen.FromControl(this).Bounds.Height);
            //btn_exit.Location = new Point(Screen.FromControl(this).Bounds.Width - 40, 1);
            server = new RedCorona.Net.Server(2345, new ClientEvent(ClientConnect));
        }

        bool ClientConnect(RedCorona.Net.Server serv, ClientInfo new_client)
        {
            new_client.Delimiter = "\n";
            new_client.OnReadBytes += new ConnectionReadBytes(ReadData);
            return true; // allow this connection
        }

        public static int GetInt(byte[] ba)
        {
            int r = 0;
            for (int i = 0; i < 4; i++)
                r += ba[i] << ((4 - i - 1) * 8);
            return r;
        }

        private void ReadData(ClientInfo ci, byte[] bytes, int len)
        {
            if (expectedBytes == 0)
            {
                expectedBytes = GetInt(bytes);
                ba = new byte[expectedBytes];
                int cx = 0;
            }
            else
            {
                ba[--expectedBytes] = bytes;
                if(expectedBytes == 0)
                {
                    Image img = Imageconverter.GetImageFromByteArray(b);
                    PictureBox p = new PictureBox();
                    p.Location = new System.Drawing.Point((int)((this.Size.Width - img.HorizontalResolution) / 2), (int)((this.Size.Height - img.VerticalResolution) / 2));
                    p.Name = "newone";
                    p.Size = new Size((int)img.HorizontalResolution, (int)img.VerticalResolution);
                    p.Image = img;
                    pictures.Insert(pictures.Count, p);
                    this.BeginInvoke((Action)(() =>
                    {
                        this.Controls.Add(pictures[pictures.Count - 1]);
                    }));
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            fs.Restore(this);
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}