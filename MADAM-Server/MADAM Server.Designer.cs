namespace MADAM_Server
{
    partial class frmMadamServer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnScan = new System.Windows.Forms.Button();
            this.txtScanResults = new System.Windows.Forms.TextBox();
            this.txtSubnet = new System.Windows.Forms.TextBox();
            this.lblSubnet = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.cmbInterfaces = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(78, 32);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 23);
            this.btnScan.TabIndex = 0;
            this.btnScan.Text = "Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.btnScan_Click);
            // 
            // txtScanResults
            // 
            this.txtScanResults.Location = new System.Drawing.Point(78, 140);
            this.txtScanResults.Multiline = true;
            this.txtScanResults.Name = "txtScanResults";
            this.txtScanResults.Size = new System.Drawing.Size(326, 257);
            this.txtScanResults.TabIndex = 1;
            // 
            // txtSubnet
            // 
            this.txtSubnet.Location = new System.Drawing.Point(78, 89);
            this.txtSubnet.Name = "txtSubnet";
            this.txtSubnet.Size = new System.Drawing.Size(100, 20);
            this.txtSubnet.TabIndex = 2;
            // 
            // lblSubnet
            // 
            this.lblSubnet.AutoSize = true;
            this.lblSubnet.Location = new System.Drawing.Point(78, 70);
            this.lblSubnet.Name = "lblSubnet";
            this.lblSubnet.Size = new System.Drawing.Size(41, 13);
            this.lblSubnet.TabIndex = 3;
            this.lblSubnet.Text = "Subnet";
            this.lblSubnet.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.btnStop.Location = new System.Drawing.Point(175, 32);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(75, 23);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "Stop";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // cmbInterfaces
            // 
            this.cmbInterfaces.FormattingEnabled = true;
            this.cmbInterfaces.Location = new System.Drawing.Point(346, 32);
            this.cmbInterfaces.Name = "cmbInterfaces";
            this.cmbInterfaces.Size = new System.Drawing.Size(230, 21);
            this.cmbInterfaces.TabIndex = 5;
            this.cmbInterfaces.SelectedIndexChanged += new System.EventHandler(this.cmbInterfaces_SelectedIndexChanged);
            // 
            // frmMadamServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cmbInterfaces);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblSubnet);
            this.Controls.Add(this.txtSubnet);
            this.Controls.Add(this.txtScanResults);
            this.Controls.Add(this.btnScan);
            this.Name = "frmMadamServer";
            this.Text = "MADAM Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.TextBox txtScanResults;
        private System.Windows.Forms.TextBox txtSubnet;
        private System.Windows.Forms.Label lblSubnet;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ComboBox cmbInterfaces;
    }
}

