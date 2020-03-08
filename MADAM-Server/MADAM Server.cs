using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Net.Sockets;

namespace MADAM_Server
{
    //server application for MADAM msp documentation. This sits on a companies windows server and reports back
    //to the central server. Detects network interfaces, uses netmask from this to scan correct subnets.
    public partial class frmMadamServer : Form
    {

        Thread scanThread;
        NetworkInterface[] adapters = new NetworkInterface[10];
        List<string> subnetList = new List<string>();
        List<string> maskList = new List<string>();
        public frmMadamServer()
        {
            InitializeComponent();
            GetSubnetMask();
            cmbInterfaces.SelectedIndex = 0;
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            scanThread = new Thread(scan);
            scanThread.Start();

            if (scanThread.IsAlive)
            {
                btnScan.Enabled = false;
                btnStop.Enabled = true;
            }
            
        }

        private void scan()
        {
            string subnet = txtSubnet.Text;

            IPHostEntry ipHostEntry;
            Ping ping;
            PingReply pingReply;
            IPAddress addr;
            int count = 0;
            if (txtSubnet.Text == "")
            {

            }
            else
            {
                for (int i = 1; i < 255; i++)
                {
                    string subnetn = "." + i.ToString();
                    ping = new Ping();
                    pingReply = ping.Send(subnet + subnetn);
                    

                    if (pingReply.Status == IPStatus.Success)
                    {
                        try
                        {
                            addr = IPAddress.Parse(subnet + subnetn);
                            ipHostEntry = Dns.GetHostEntry(addr);
                            AppendTextBox(subnet + subnetn + " " + ipHostEntry.HostName.ToString() + " Up " + System.Environment.NewLine);
                            Thread.Sleep(1000);
                            count ++;
                        }

                        catch
                        {

                        }
                    }

                    if (i == 254)
                    {
                        MessageBox.Show(this, string.Format("Scan on network {0} complete, found {1} devices",subnet,count), "Scan Complete!");

                    }
                }
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            scanThread.Suspend();
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
    }
}
