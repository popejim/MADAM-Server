using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MADAM_Server
{
    public partial class frmMainMenu : Form
    {

        volatile int keepGoing = 1;
        public bool listening = true;
        public Socket listen;
        private Thread _connectThread;
        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void frmMainMenu_Load(object sender, EventArgs e)
        {
            _connectThread = new Thread(ListenForCentral);
            _connectThread.Name = "Socket Connection Thread";
            _connectThread.IsBackground = true;
            _connectThread.Start();
        }

        public void ListenForCentral()
        {
            //make endpoint for listener on localhost, uses port 42069
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localIP = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 42069);

            //Make TCP/IP socket
            listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listen.Bind(localEndPoint);
                listen.Listen(100);
                while (listening == true)
                {
                    listen.BeginAccept(new AsyncCallback(ReplyToCentral), listen);
                }      

                
                
                return;
                //code to do after replying
                
            }

            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Form frmSettings = new frmSettings();
            frmSettings.ShowDialog();
            this.Enabled = true;
        }

        private void ReplyToCentral(IAsyncResult ar)
        {
            // Get the socket that handles the client request.  
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);
            IPAddress remoteAddress = ((IPEndPoint)handler.RemoteEndPoint).Address;
            TcpClient client = new TcpClient(remoteAddress.ToString(), 42069);
            NetworkStream stream = client.GetStream();
            Thread.Sleep(2000);
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("Server");
            stream.Write(bytesToSend, 0, bytesToSend.Length);
            stream.Close();
            client.Close();
            listening = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            frmMadamServerScan frmscan = new frmMadamServerScan();
            frmscan.Show(); 
            _connectThread = new Thread(ListenForCentral);
            _connectThread.Name = "Socket Connection Thread";
            _connectThread.IsBackground = true;
            _connectThread.Start();
        }
    }
}
