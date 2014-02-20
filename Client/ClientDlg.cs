using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Nito.Async.Sockets;
using Nito.Async;

namespace Client
{
    public partial class ClientDlg : Form
    {
        /// <summary>
        /// The connected state of the socket.
        /// </summary>
        private enum SocketState
        {
            /// <summary>
            /// The socket is closed; we are not trying to connect.
            /// </summary>
            Closed,

            /// <summary>
            /// The socket is attempting to connect.
            /// </summary>
            Connecting,

            /// <summary>
            /// The socket is connected.
            /// </summary>
            Connected,

            /// <summary>
            /// The socket is attempting to disconnect.
            /// </summary>
            Disconnecting
        }
        private SocketState ClientSocketState;
        private SimpleClientTcpSocket ClientSocket;
        private const int port = 2345;

        public ClientDlg()
        {
            InitializeComponent();
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (ClientSocketState == SocketState.Connected)
                ResetSocket();
            else
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
            Image i = SnippingTool.Snip();
            if (i != null)
                SendImage(i);
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void btn_windowsnipping_Click(object sender, EventArgs e)
        {
            this.Hide();
            this.ShowInTaskbar = false;
            System.Threading.Thread.Sleep(300);
            Image i = WindowSnipping.Snip();
            if (i != null)
                SendImage(i);
            this.ShowInTaskbar = true;
            this.Show();
        }

        private void ClientSocket_ConnectCompleted(AsyncCompletedEventArgs e)
        {
            try
            {
                // Check for errors
                if (e.Error != null)
                {
                    ResetSocket();
                    lbl_connected.ForeColor = Color.Red;
                    lbl_connected.Text = e.Error.Message;
                    RefreshDisplay(true);
                    return;
                }

                // Adjust state
                ClientSocketState = SocketState.Connected;
                RefreshDisplay(false);
            }
            catch (Exception ex)
            {
                ResetSocket();
                lbl_connected.ForeColor = Color.Red;
                lbl_connected.Text = ex.Message;
                RefreshDisplay(true);
            }
        }

        private void ClientSocket_WriteCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // Check for errors
            if (e.Error != null)
            {
                // Note: WriteCompleted may be called as the result of a normal write or a keepalive packet.
                ResetSocket();

                // If you want to get fancy, you can tell if the error is the result of a write failure or a keepalive
                //  failure by testing e.UserState, which is set by normal writes.
                lbl_connected.ForeColor = Color.Red;
                lbl_connected.Text = e.Error.Message;
                RefreshDisplay(true);
            }
            else
            {
                //Do the things to do if write was fine
                RefreshDisplay(false);
            }
        }

        private void ClientSocket_ShutdownCompleted(AsyncCompletedEventArgs e)
        {
            // Check for errors
            if (e.Error != null)
            {
                ResetSocket();
                lbl_connected.ForeColor = Color.Red;
                lbl_connected.Text = e.Error.Message;
                RefreshDisplay(true);
            }
            else
            {
                ResetSocket();
                RefreshDisplay(false);
            }
        }

        private void ClientSocket_PacketArrived(AsyncResultEventArgs<byte[]> e)
        {
            try
            {
                // Check for errors
                if (e.Error != null)
                {
                    ResetSocket();
                    lbl_connected.ForeColor = Color.Red;
                    lbl_connected.Text = e.Error.Message;
                    RefreshDisplay(true);
                }
                else if (e.Result == null)
                {
                    // PacketArrived completes with a null packet when the other side gracefully closes the connection
                    lbl_connected.ForeColor = Color.Red;
                    lbl_connected.Text = e.Error.Message;
                    RefreshDisplay(true);

                    // Close the socket and handle the state transition to disconnected.
                    ResetSocket();
                }
            }
            catch (Exception ex)
            {
                ResetSocket();
                lbl_connected.ForeColor = Color.Red;
                lbl_connected.Text = "hallo";
                RefreshDisplay(true);
            }
        }

        private void Connect()
        {
            if (ClientSocketState == SocketState.Closed)
            {
                try
                {
                    // Read the IP address
                    IPAddress serverIPAddress;
                    if (!IPAddress.TryParse(txt_serverip.Text, out serverIPAddress))
                    {
                        MessageBox.Show("Invalid IP address: " + txt_serverip.Text);
                        txt_serverip.Focus();
                        return;
                    }
                    txt_serverip.Text = serverIPAddress.ToString();

                    // Begin connecting to the remote IP
                    ClientSocket = new SimpleClientTcpSocket();
                    ClientSocket.ConnectCompleted += ClientSocket_ConnectCompleted;
                    ClientSocket.PacketArrived += ClientSocket_PacketArrived;
                    ClientSocket.WriteCompleted += (args) => ClientSocket_WriteCompleted(ClientSocket, args);
                    ClientSocket.ShutdownCompleted += ClientSocket_ShutdownCompleted;
                    ClientSocket.ConnectAsync(serverIPAddress, port);
                    ClientSocketState = SocketState.Connecting;
                    RefreshDisplay(false);
                }
                catch (Exception ex)
                {
                    ResetSocket();
                    lbl_connected.ForeColor = Color.Red;
                    lbl_connected.Text = ex.Message;
                    RefreshDisplay(true);
                }
            }
        }

        private byte[] ObjectToByteArray(Object pObject)
        {
            MemoryStream hStream = new MemoryStream();
            BinaryFormatter hFormatter = new BinaryFormatter();
            hFormatter.Serialize(hStream, pObject);
            return hStream.ToArray();
        }

        private void RefreshDisplay(bool isError)
        {
            // We can only send messages if we have a connection
            btn_picture.Enabled = (ClientSocketState == SocketState.Connected);
            btn_snipping.Enabled = (ClientSocketState == SocketState.Connected);
            btn_windowsnipping.Enabled = (ClientSocketState == SocketState.Connected);
            if (ClientSocketState == SocketState.Connected)
                btn_connect.Text = "Trennen";
            else
                btn_connect.Text = "Verbinden";

            // Display status
            if (!isError)
            {
                lbl_connected.ForeColor = Color.Black;
                switch (ClientSocketState)
                {
                    case SocketState.Closed:
                        lbl_connected.Text = "Getrennt";
                        break;
                    case SocketState.Connecting:
                        lbl_connected.Text = "Verbinde...";
                        break;
                    case SocketState.Connected:
                        lbl_connected.Text = "Verbunden mit " + ClientSocket.RemoteEndPoint.ToString();
                        break;
                    case SocketState.Disconnecting:
                        lbl_connected.Text = "Trennen";
                        break;
                }
            }
        }

        private void ResetSocket()
        {
            // Close the socket
            if (ClientSocket != null)
            {
                ClientSocket.Close();
                ClientSocket = null;
            }

            // Indicate there is no socket connection
            ClientSocketState = SocketState.Closed;
            RefreshDisplay(false);
        }

        private void SendImage(Image message)
        {
            try
            {
                byte[] imgArray = ObjectToByteArray(message);
                ClientSocket.WriteAsync(imgArray);
                RefreshDisplay(false);
            }
            catch (Exception e)
            {
                lbl_connected.ForeColor = Color.Red;
                lbl_connected.Text = e.Message;
                RefreshDisplay(true);
            }
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
