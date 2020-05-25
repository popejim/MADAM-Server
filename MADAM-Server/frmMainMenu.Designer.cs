namespace MADAM_Server
{
    partial class frmMainMenu
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
            this.btnClientUpdate = new System.Windows.Forms.Button();
            this.lstDevices = new System.Windows.Forms.ListBox();
            this.lstDetails = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // btnScan
            // 
            this.btnScan.Location = new System.Drawing.Point(12, 39);
            this.btnScan.Name = "btnScan";
            this.btnScan.Size = new System.Drawing.Size(75, 51);
            this.btnScan.TabIndex = 1;
            this.btnScan.Text = "Perform New Scan";
            this.btnScan.UseVisualStyleBackColor = true;
            this.btnScan.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnClientUpdate
            // 
            this.btnClientUpdate.Location = new System.Drawing.Point(12, 431);
            this.btnClientUpdate.Name = "btnClientUpdate";
            this.btnClientUpdate.Size = new System.Drawing.Size(88, 48);
            this.btnClientUpdate.TabIndex = 2;
            this.btnClientUpdate.Text = "Check For Client Update";
            this.btnClientUpdate.UseVisualStyleBackColor = true;
            this.btnClientUpdate.Click += new System.EventHandler(this.btnClientUpdate_Click);
            // 
            // lstDevices
            // 
            this.lstDevices.FormattingEnabled = true;
            this.lstDevices.Location = new System.Drawing.Point(173, 39);
            this.lstDevices.Name = "lstDevices";
            this.lstDevices.Size = new System.Drawing.Size(367, 199);
            this.lstDevices.TabIndex = 3;
            this.lstDevices.SelectedIndexChanged += new System.EventHandler(this.lstDevices_SelectedIndexChanged);
            // 
            // lstDetails
            // 
            this.lstDetails.FormattingEnabled = true;
            this.lstDetails.Location = new System.Drawing.Point(173, 245);
            this.lstDetails.Name = "lstDetails";
            this.lstDetails.Size = new System.Drawing.Size(367, 238);
            this.lstDetails.TabIndex = 4;
            // 
            // frmMainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 510);
            this.Controls.Add(this.lstDetails);
            this.Controls.Add(this.lstDevices);
            this.Controls.Add(this.btnClientUpdate);
            this.Controls.Add(this.btnScan);
            this.Name = "frmMainMenu";
            this.Text = "MADAM Server";
            this.Load += new System.EventHandler(this.frmMainMenu_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnScan;
        private System.Windows.Forms.Button btnClientUpdate;
        private System.Windows.Forms.ListBox lstDevices;
        private System.Windows.Forms.ListBox lstDetails;
    }
}