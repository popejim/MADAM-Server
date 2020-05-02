using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MADAM_Server
{
    public partial class frmMainMenu : Form
    {
        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void frmMainMenu_Load(object sender, EventArgs e)
        {
            
        }

        public void ListenForCentral()
        {

        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            Form frmSettings = new frmSettings();
            frmSettings.ShowDialog();
            this.Enabled = true;
        }
    }
}
