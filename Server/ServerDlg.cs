using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Org.Vesic.WinForms;
using Nito.Async.Sockets;
using Nito.Async;

namespace Server
{
    public partial class ServerDlg : Form
    {
        private SimpleServerTcpSocket ListeningSocket;
        private FormState fs = new FormState();
        private Dictionary<SimpleServerChildTcpSocket, ImageState> ChildSockets = new Dictionary<SimpleServerChildTcpSocket, ImageState>();
        private const int port = 2345;

        public ServerDlg()
        {
            InitializeComponent();
            #if (!DEBUG)
            this.Size = new System.Drawing.Size(Screen.FromControl(this).Bounds.Width, Screen.FromControl(this).Bounds.Height);
            btn_exit.Location = new Point(Screen.FromControl(this).Bounds.Width - 40, 1);
            fs.Maximize(this);
            #endif
            ListeningSocket = new SimpleServerTcpSocket();
            ListeningSocket.ConnectionArrived += ListeningSocket_ConnectionArrived;
            ListeningSocket.Listen(port);
        }

        private bool RemoveElement(SimpleServerChildTcpSocket s)
        {
            if(s != null)
                s.Close();
            foreach (PictureBox p in ChildSockets[s].Pictures)
            {
                this.Controls.Remove(p);
                p.Dispose();
                ChildSockets[s].Pictures.Remove(p);
            }
            return ChildSockets.Remove(s);
        }

        private void ResetChildSocket(SimpleServerChildTcpSocket childSocket)
        {
            RemoveElement(childSocket);
        }
        
        private void ResetListeningSocket()
        {
            // Close all child sockets and delete their handles to the ServerDlg
            foreach (SimpleServerChildTcpSocket s in ChildSockets.Keys)
            {
                RemoveElement(s);
                s.AbortiveClose();
            }
            ChildSockets.Clear();

            // Close the listening socket
            ListeningSocket.Close();
            ListeningSocket = null;
        }

        private void ListeningSocket_ConnectionArrived(AsyncResultEventArgs<SimpleServerChildTcpSocket> e)
        {
            // Check for errors
            if (e.Error != null)
            {
                ResetListeningSocket();
                return;
            }

            SimpleServerChildTcpSocket socket = e.Result;

            try
            {
                // Save the new child socket connection
                ChildSockets.Add(socket, new ImageState(ChildSocketState.Connected));
                socket.PacketArrived += (args) => ChildSocket_PacketArrived(socket, args);
                socket.ShutdownCompleted += (args) => ChildSocket_ShutdownCompleted(socket, args);
                // Display the connection information
                //textBoxLog.AppendText("Connection established to " + socket.RemoteEndPoint.ToString() + Environment.NewLine);
            }
            catch (Exception ex)
            {
                ResetChildSocket(socket);
                //textBoxLog.AppendText("Socket error accepting connection: [" + ex.GetType().Name + "] " + ex.Message + Environment.NewLine);
            }
            finally
            {
                //RefreshDisplay();
            }
        }

        private void ChildSocket_PacketArrived(SimpleServerChildTcpSocket socket, AsyncResultEventArgs<byte[]> e)
        {
            try
            {
                // Check for errors
                if (e.Error != null)
                {
                    //textBoxLog.AppendText("Client socket error during Read from " + socket.RemoteEndPoint.ToString() + ": [" + e.Error.GetType().Name + "] " + e.Error.Message + Environment.NewLine);
                    ResetChildSocket(socket);
                }
                else if (e.Result == null)
                {
                    // PacketArrived completes with a null packet when the other side gracefully closes the connection
                    //textBoxLog.AppendText("Socket graceful close detected from " + socket.RemoteEndPoint.ToString() + Environment.NewLine);

                    // Close the socket and remove it from the list
                    ResetChildSocket(socket);
                }
                else
                {
                    // At this point, we know we actually got a message.
                    // Deserialize the message
                    Image message = (Image) Util.Deserialize(e.Result);

                    // Handle the message
                    PictureBox p = new PictureBox();
                    p.Location = new System.Drawing.Point((this.Size.Width - message.Width) / 2, (this.Size.Height - message.Height) / 2);
                    p.Name = "newone";
                    p.Size = new Size(message.Width, message.Height);
                    p.Image = message;
                    ImageState iss = new ImageState();
                    ChildSockets[socket].Pictures.Insert(ChildSockets[socket].Pictures.Count, p);
                    this.Controls.Add(ChildSockets[socket].Pictures[ChildSockets[socket].Pictures.Count - 1]);
                    ChildSockets[socket].Pictures[ChildSockets[socket].Pictures.Count - 1].BringToFront();
                    btn_exit.BringToFront();
                }
            }
            catch (Exception ex)
            {
                //textBoxLog.AppendText("Error reading from socket " + socket.RemoteEndPoint.ToString() + ": [" + ex.GetType().Name + "] " + ex.Message + Environment.NewLine);
                ResetChildSocket(socket);
            }
            finally
            {
                //RefreshDisplay();
            }
        }

        private void ChildSocket_ShutdownCompleted(object sender, AsyncCompletedEventArgs e)
        {
            SimpleServerChildTcpSocket socket = (SimpleServerChildTcpSocket)sender;

            // Check for errors
            if (e.Error != null)
            {
                //textBoxLog.AppendText("Socket error during Shutdown of " + socket.RemoteEndPoint.ToString() + ": [" + e.Error.GetType().Name + "] " + e.Error.Message + Environment.NewLine);
                ResetChildSocket(socket);
            }
            else
            {
                //textBoxLog.AppendText("Socket shutdown completed on " + socket.RemoteEndPoint.ToString() + Environment.NewLine);

                // Close the socket and remove it from the list
                ResetChildSocket(socket);
            }
            // RefreshDisplay();
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            ResetListeningSocket();
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        { 
            fs.Restore(this);
        }
    }
}