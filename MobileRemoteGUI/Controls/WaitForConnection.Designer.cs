namespace MobileSRC.MobileRemote
{
    partial class WaitForConnection
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
            this.components = new System.ComponentModel.Container();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this._statusLabel = new System.Windows.Forms.Label();
            this._existingMachine = new System.Windows.Forms.Label();
            this._newMachineLabel = new System.Windows.Forms.Label();
            this._connectionErrorLabel = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBar1.Location = new System.Drawing.Point(0, 20);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(240, 20);
            // 
            // label1
            // 
            this._statusLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._statusLabel.Location = new System.Drawing.Point(0, 0);
            this._statusLabel.Name = "label1";
            this._statusLabel.Size = new System.Drawing.Size(240, 40);
            this._statusLabel.Text = "Currently waiting for a response...";
            // 
            // _existingMachine
            // 
            this._existingMachine.Dock = System.Windows.Forms.DockStyle.Fill;
            this._newMachineLabel.Location = new System.Drawing.Point(0, 141);
            this._existingMachine.Name = "_existingMachine";
            this._newMachineLabel.Size = new System.Drawing.Size(240, 127);
            this._existingMachine.Text = "You are attempting to connect to a previously paired machine.  Please make sure t" +
                "he machine is on and has Bluetooth turned on.";
            // 
            // _newMachineLabel
            // 
            this._newMachineLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._newMachineLabel.Location = new System.Drawing.Point(0, 141);
            this._newMachineLabel.Name = "_newMachineLabel";
            this._newMachineLabel.Size = new System.Drawing.Size(240, 127);
            this._newMachineLabel.Text = "You are attempting to connect to a new machine. Please follow the pairing instruc" +
                "tions:\r\n1) If already paired, remove pairing\r\n2)Pair with host machine\r\n3)Enter passcode on pc and device\r\n4)That's all!";
            // 
            // _connectionErrorLabel
            // 
            this._connectionErrorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._connectionErrorLabel.Location = new System.Drawing.Point(0, 141);
            this._connectionErrorLabel.Name = "_connectionErrorLabel";
            this._connectionErrorLabel.Size = new System.Drawing.Size(240, 127);
            this._connectionErrorLabel.Text = "There was an error connecting to the computer. Please ensure that your computer is on, has Bluetooth enabled, and has an active pairing with this machine.";
            this._connectionErrorLabel.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // WaitForConnection
            // 
            _contentPanel.Controls.Add(_existingMachine);
            _contentPanel.Controls.Add(_newMachineLabel);
            _contentPanel.Controls.Add(_connectionErrorLabel);
            _contentPanel.Controls.Add(_statusLabel);
            _contentPanel.Controls.Add(progressBar1);
            this.AutoScroll = true;
            this.Location = new System.Drawing.Point(0, 52);
            this.ClientSize = new System.Drawing.Size(480, 400);
            this.Text = "Connecting...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label _statusLabel;
        private System.Windows.Forms.Label _existingMachine;
        private System.Windows.Forms.Label _newMachineLabel;
        private System.Windows.Forms.Label _connectionErrorLabel;
        private System.Windows.Forms.Timer timer1;

    }
}