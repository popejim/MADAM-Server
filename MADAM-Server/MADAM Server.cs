using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace MADAM_Server
{
    public partial class frmMadamServer : Form
    {
        public frmMadamServer()
        {
            InitializeComponent();
        }

        private void btnScan_Click(object sender, EventArgs e)
        {
            string subnet = "192.168.0";

            IPHostEntry ipHostEntry;
            Ping ping;
            PingReply pingReply;
            IPAddress addr;

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

                        txtScanResults.AppendText(subnet + subnetn + ipHostEntry.HostName.ToString() + "Up");
                    }

                    catch
                    {

                    }
                }
            }
        }
    }
}
