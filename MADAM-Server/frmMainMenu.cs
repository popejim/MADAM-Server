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
using MADAM_Server.Classes;
using System.Xml.Serialization;

namespace MADAM_Server
{
    public partial class frmMainMenu : Form
    {

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

            List<Device> currentDevices = GetDevices();
            if (currentDevices != null)
            {
                foreach (Device d in currentDevices)
                {
                    lstDevices.Items.Add(d.name);
                }
            }

        }

        public List<Device> GetDevices()
        {
            try
            {
                List<Device> returnList;
                string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

                XmlSerializer mySerializer = new XmlSerializer(typeof(List<Device>));
                StreamReader myReader = new StreamReader(savePath + "\\MADAMServer\\Devices.XML");
                myReader.Close();
                returnList = (List<Device>)mySerializer.Deserialize(myReader);
                return returnList;
            }
            catch
            {
                return null;
            }
        }

        public void ListenForCentral()
        {
            //make endpoint for listener on localhost, uses port 42069
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localIP = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 42070);

            //Make TCP/IP socket
            listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listen.Bind(localEndPoint);
                listen.Listen(100);
                listen.BeginAccept(new AsyncCallback(ReplyToCentral), listen);
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

        private void performNewScanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button1_Click(this, e);
        }

        private void btnClientUpdate_Click(object sender, EventArgs e)
        {

        }
    }
}
