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
using System.Runtime.Serialization.Formatters.Binary;

namespace MADAM_Server
{
    public partial class frmMainMenu : Form
    {

        public bool listening = true;
        public Socket listen;
        public List<Device> currentDevices;
        private Thread _connectThread;
        public TcpListener clientListen = new TcpListener(IPAddress.Any, 42073);

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

            currentDevices = GetDevices();
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
                
                returnList = (List<Device>)mySerializer.Deserialize(myReader);
                myReader.Close();
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

        public void ListenForCentralUpdate()
        {
            //make endpoint for listener on localhost, uses port 42069
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress localIP = host.AddressList.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork);

            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 42060);

            //Make TCP/IP socket
            listen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listen.Bind(localEndPoint);
                listen.Listen(100);
                listen.BeginAccept(new AsyncCallback(replyDevices), listen);
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

        private void replyDevices(IAsyncResult ar)
        {
            Socket listener2 = (Socket)ar.AsyncState;
            Socket handler2 = listener2.EndAccept(ar);
            IPAddress remoteAddress = ((IPEndPoint)handler2.RemoteEndPoint).Address;
            sendDevices(currentDevices, remoteAddress.ToString());
        }

        public void sendDevices(List<Device> listToSend, string ip)
        {
            TcpClient client = new TcpClient(ip, 42063);
            NetworkStream nwStream = client.GetStream();
            XmlSerializer mySerializer = new XmlSerializer(typeof(List<Device>));
            mySerializer.Serialize(nwStream, listToSend);
            nwStream.Close();
            client.Close();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            frmMadamServerScan frmscan = new frmMadamServerScan();
            frmscan.Show();
            this.Hide(); 
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
            _connectThread = new Thread(ListenForClient);
            _connectThread.Name = "Socket Connection Thread";
            _connectThread.IsBackground = true;
            _connectThread.Start();
        }

        private void ListenForClient()
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            
            clientListen.Start();
            // Enter the listening loop.
            while (true)
            {
                Console.Write("Waiting for a connection... ");
                // accept request
                TcpClient client = clientListen.AcceptTcpClient();
                NetworkStream stream = client.GetStream();
                XmlSerializer mySerializer = new XmlSerializer(typeof(List<Users>));
                List<Users> inList = (List<Users>)mySerializer.Deserialize(stream);
                stream.Close();
                client.Close();
            }
        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selected = lstDevices.SelectedIndex;
            PopulateDetails(selected);
        }

        public void PopulateDetails(int currentSelection)
        {
            lstDetails.Items.Clear();
            if (currentSelection == -1)
            {
                currentSelection = 1;
            }
            Classes.Device deviceInfo = currentDevices[currentSelection];
            lstDetails.Items.Add(deviceInfo.hostName);
            lstDetails.Items.Add(deviceInfo.ipAddr);
            if (deviceInfo.macAddr != null)
            {
                lstDetails.Items.Add(deviceInfo.macAddr);
            }
            lstDetails.Items.Add("OS " + deviceInfo.osVersion);
            if (deviceInfo.isAd == true)
            {
                List<Users> tempList = deviceInfo.UserList;
                lstDetails.Items.Add("Active Directory Detected");
                foreach (Users u in tempList)
                {
                    lstDetails.Items.Add(u.fullName);
                }
                
            }
            
        }

        private void lstDetails_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void settingsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            frmSettings settings = new frmSettings();
            settings.Show();
        }
    }
}
