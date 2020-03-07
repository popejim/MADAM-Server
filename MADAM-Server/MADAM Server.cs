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
    public partial class frmMadamServer : Form
    {

        Thread scanThread;
        public frmMadamServer()
        {
            InitializeComponent();
            GetSubnetMask();
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

                            AppendTextBox(subnet + subnetn + ipHostEntry.HostName.ToString() + "Up");
                            Thread.Sleep(1000);
                        }

                        catch
                        {

                        }
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
            Array adapters;
        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
                if (Convert.ToString(adapter.OperationalStatus) == "Up")
                {
                    cmbInterfaces.Items.Add(adapter.Name);
                }
            
            foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
            {

                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    Console.WriteLine(unicastIPAddressInformation.IPv4Mask);
                    
                }
            }
        }
    }

        private void cmbInterfaces_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
