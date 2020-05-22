using MADAM_Server.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MADAM_Server
{
    public partial class frmSettings : Form
    {
        string savePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            try
            {
                settings.controlIP = IPAddress.Parse(txtControlIP.Text).ToString();
            }

            catch
            {
                MessageBox.Show("Please enter a valid IP address", "Invalid IP entered");
                return;
            }
            XmlSerializer mySerializer = new XmlSerializer(typeof(Settings));
            if (Directory.Exists(savePath + "\\MADAMServer\\"))
            {
                StreamWriter myWriter = new StreamWriter(savePath + "\\MADAMServer\\Settings.XML");
                mySerializer.Serialize(myWriter, settings);
                myWriter.Close();
            }

            else
            {
                Directory.CreateDirectory(savePath + "\\MADAMServer\\");
                StreamWriter myWriter = new StreamWriter(savePath + "\\MADAMServer\\Settings.XML");
                mySerializer.Serialize(myWriter, settings);
                myWriter.Close();
            }

            MessageBox.Show("Settings Saved", "Settings Saved");
            this.Close();
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            Settings settings;

            XmlSerializer mySerializer = new XmlSerializer(typeof(Settings));
            using (FileStream myFileStream = new FileStream(savePath + "\\MADAMServer\\Settings.XML", FileMode.Open))
            {
                settings = (Settings)mySerializer.Deserialize(myFileStream);
                txtControlIP.Text = settings.controlIP;
                myFileStream.Close();
            }
        }
    }
}
