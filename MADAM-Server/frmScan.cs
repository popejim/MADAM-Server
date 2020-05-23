using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MADAM_Server.Classes;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using RestSharp;
using System.Management;
using System.Management.Instrumentation;
using System.Xml.Serialization;
using System.IO;

namespace MADAM_Server
{
    //server application for MADAM msp documentation. This sits on a companies windows server and reports back
    //to the central server. Detects network interfaces, uses netmask from this to scan correct subnets.
    public partial class frmMadamServerScan : Form
    {
        //imports dll for using arp resolution
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        public static extern int SendARP(int DestIP, int SrcIP, byte[] pMacAddr, ref uint PhyAddrLen);
        public List<Device> deviceList = new List<Device>(); 
        public bool hasStarted;
        public bool listening = true;
        Thread scanThread;
        NetworkInterface[] adapters = new NetworkInterface[10];
        List<string> subnetList = new List<string>();
        List<string> maskList = new List<string>();
        private Thread _listenThread;
        public bool hasClosed;

        public frmMadamServerScan()
        {
            InitializeComponent();
            GetSubnetMask();
            cmbInterfaces.SelectedIndex = 0;
            //setup new thread for socket listener
            _listenThread = new Thread(listenForUdp);
            _listenThread.Name = "Socket Connection Thread";
            _listenThread.IsBackground = true;
            //_listenThread.Start();
        }
        
        //returns a mac address by using arp resolution for a given IP address
        private string GetMacUsingARP(string ip)
        {
            //IPAddress IP = IPAddress.Parse(IPAddr);
            byte[] macAddr = new byte[6];
            uint macAddrLen = (uint)macAddr.Length;
            IPAddress ipAddr = IPAddress.Parse(ip);
            try
            {

                if (SendARP(BitConverter.ToInt32(ipAddr.GetAddressBytes(),0), 0, macAddr, ref macAddrLen) != 0)
                {
                    throw new Exception("ARP command failed");
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
                string[] str = new string[(int)macAddrLen];
                for (int i = 0; i < macAddrLen; i++)str[i] = macAddr[i].ToString("x2");
                return string.Join(":", str);
        }
        private void btnScan_Click(object sender, EventArgs e)
        {
            scanThread = new Thread(scan);
            scanThread.Start();
            hasStarted = true;
            //disables buttons while running
            if (scanThread.IsAlive)
            {
                btnScan.Enabled = false;
                btnStop.Enabled = true;
            }
            
        }
        private async Task<List<PingReply>> PingAsync(string subnet)
        {
            List<string> allip = new List<string>();
            for (int i = 1; i < 255; i++)
            {
                string subnetn = "." + i.ToString();
                allip.Add(subnet + subnetn);
                Console.WriteLine(i);
            }
            Ping pingSender = new Ping();
            var tasks = allip.Select(ip => new Ping().SendPingAsync(ip, 1000));
            var results = await Task.WhenAll(tasks);
            return results.ToList();
        }

        public void scan()
        {
            string subnet = txtSubnet.Text;
            IPHostEntry ipHostEntry;
            Ping ping;
            IPAddress addr;
            int count = 0;
            ping = new Ping();
            List<PingReply> pingReply = PingAsync(subnet).Result;

            //on successful ping, make new instance of a device
            foreach (PingReply r in pingReply)
            {
                if (r.Status == IPStatus.Success)
                {
                    try
                    {
                        addr = IPAddress.Parse(r.Address.ToString());
                        Device device = new Device();

                        try
                        {
                            
                            System.Diagnostics.Process.Start("cmd.exe","/C ipconfig /flushdns");
                            ipHostEntry = Dns.GetHostEntry(addr.ToString());
                            
                            device.hostName = ipHostEntry.HostName.ToString();
                            
                            if (ipHostEntry.HostName.ToString().Contains("."))
                            {
                                device.name = ipHostEntry.HostName.ToString().Substring(0, device.hostName.IndexOf('.'));
                            }

                            else
                            {
                                device.name = device.hostName;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            device.hostName = "Unknown Device";
                            device.name = "Unknown Device";
                        }

                        device.macAddr = GetMacUsingARP(addr.ToString());
                        device.Manufacturer = macApiLookup(device.macAddr);
                        device.ipAddr = addr.ToString();
                        device.osVersion = getOsVersion(addr.ToString());
                        if (device.Manufacturer.Contains("HUAWEI"))
                        {
                            device.osVersion = "Android OS";
                        }
                        else if (device.Manufacturer.Contains("Google"))
                        {
                            device.osVersion = "Chromecast";
                        }

                        //add details to the text box and sleep to not lock the UI. Increases count of successful devices found.
                        AppendTextBox(device.ipAddr + " " + device.name + " Is up " + " OS: " + device.osVersion + " Mac address: " + device.macAddr + " NIC: " + device.Manufacturer + Environment.NewLine + Environment.NewLine);
                        Thread.Sleep(100);
                        deviceList.Add(device);
                        count++;
                    }

                    catch
                    {
                        MessageBox.Show("Uhoh, something broke!", "Scan Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            MessageBox.Show("Scan Complete");
            SaveDevices(deviceList);
            if (InvokeRequired)
            {
                btnScan.BeginInvoke((Action)delegate() { btnScan.Enabled = true; });
                btnStop.BeginInvoke((Action)delegate () { btnStop.Enabled = false; });
                hasStarted = false;
                return;
            }
        }
        
        private void SaveDevices(List<Device>listIn)
        {
            string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
     
                XmlSerializer mySerializer = new XmlSerializer(typeof(List<Device>));
                if (Directory.Exists(savePath + "\\MADAMServer\\"))
                {
                    StreamWriter myWriter = new StreamWriter(savePath + "\\MADAMServer\\Devices.XML");
                    mySerializer.Serialize(myWriter, listIn);
                    myWriter.Close();
                }

                else
                {
                    Directory.CreateDirectory(savePath + "\\MADAMServer\\");
                    StreamWriter myWriter = new StreamWriter(savePath + "\\MADAMServer\\Devices.XML");
                    mySerializer.Serialize(myWriter, listIn);
                    myWriter.Close();
                }
            
            
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            hasStarted = false;
            
            btnStop.Enabled = false;
            btnScan.Enabled = true;
        }

        public void AppendTextBox(string value)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>(AppendTextBox), new object[] { value });
                return;
            }
            txtScanResults.Text += value;
        }

        private void GetSubnetMask()
        {
            int i = 0;
            
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (Convert.ToString(adapter.OperationalStatus) == "Up")
                {
                    cmbInterfaces.Items.Add(adapter.Name);
                    adapters[i] = adapter;
                    i++;

                    foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                    {

                        if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            maskList.Add(Convert.ToString(unicastIPAddressInformation.IPv4Mask));
                            subnetList.Add(Convert.ToString(unicastIPAddressInformation.Address));
                        }
                    }
                }
            }
        }

        private void cmbInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            calculateSubnet(subnetList[cmbInterfaces.SelectedIndex], maskList[cmbInterfaces.SelectedIndex]);
        }

        private string getOsVersion(string ipAddr)
        {
            try
            {
                using (var reg = RegistryKey.OpenRemoteBaseKey(RegistryHive.LocalMachine, ipAddr))
                using (var key = reg.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\"))
                {
                    return string.Format("Name:{0}, Version:{1}", key.GetValue("ProductName"), key.GetValue("CurrentVersion"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
                return string.Format("Unknown OS");
            }
        }

        private void calculateSubnet(string ipAddress, string subNetMask)
        {
            string[] ipOct = ipAddress.Split('.');
            string[] snOct = subNetMask.Split('.');

            string[] result = new string[4];
            for (int i = 0; i < 4; i++)
            {
                byte ipOctet = Convert.ToByte(ipOct[i]);
                byte snOctet = Convert.ToByte(snOct[i]);
                result[i] = (ipOctet & snOctet).ToString();
            }

            string resultIP = string.Join(".", result);

            if (snOct[1] == "0")
            {
                string final = resultIP.Remove(resultIP.Length - 6);
                final = final + ".0.0";
                txtSubnet.Text = final;
            }

            else if (snOct[2] == "0")
            {
                string final = resultIP.Remove(resultIP.Length - 4);
                final = final + ".0";
                txtSubnet.Text = final;
            }

            else if (snOct[3] == "0")
            {
                string final = resultIP.Remove(resultIP.Length - 2);
                txtSubnet.Text = final;
            }
        }

        private void frmMadamServer_Load(object sender, EventArgs e)
        {
        }

        private string macApiLookup(string mac)
        {
            if (mac == null)
            {
                return "Unknown NIC";
            }

            else
            {
                var client = new RestClient("https://api.macvendors.com/" + mac);
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                return response.Content;
            }
        }

        private void listenForUdp()
        {

            Socket listen = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint incomingEndPoint = new IPEndPoint(IPAddress.Any, 42069);
            EndPoint ep = (EndPoint)incomingEndPoint;
            if (listen.IsBound == false)
            {
                listen.Bind(incomingEndPoint);
            }
            
            while (listening == true)
            {
                byte[] data = new byte[1024];
                int recv = listen.ReceiveFrom(data, ref ep);
                string stringData = Encoding.ASCII.GetString(data, 0, recv);
                string clientIp = ep.ToString();
                clientIp = clientIp.Substring(0, clientIp.LastIndexOf(":"));
                Console.WriteLine(stringData);
                AppendTextBox(stringData);

                if (stringData == "find")
                {
                    TcpClient client = new TcpClient(clientIp.ToString(), 42069);
                    NetworkStream stream = client.GetStream();
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes("here");
                    stream.Write(bytesToSend, 0, bytesToSend.Length);
                    stream.Close();
                    client.Close();
                    listen.Close();
                }
            }
        }

        private void frmMadamServerScan_FormClosing(object sender, FormClosingEventArgs e)
        {
            _listenThread.Abort();
        }
    }
}
