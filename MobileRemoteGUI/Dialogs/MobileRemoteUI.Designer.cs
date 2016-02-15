namespace MobileSRC.MobileRemote
{
    partial class MobileRemoteUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MobileRemoteUI));
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonPanel1 = new MobileSRC.MobileRemote.ButtonPanel();
            this._connectButton = new MobileSRC.MobileRemote.ConnectButton();
            this._optionsButton = new PushButton();
            this._inputsButton = new PushButton();
            this.buttonPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(480, 560);
            // 
            // buttonPanel1
            // 
            this.buttonPanel1.BackColor = System.Drawing.Color.LightGray;
            this.buttonPanel1.Controls.Add(this._connectButton);
            this.buttonPanel1.Controls.Add(this._optionsButton);
            this.buttonPanel1.Controls.Add(this._inputsButton);
            this.buttonPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel1.Location = new System.Drawing.Point(0, 560);
            this.buttonPanel1.Name = "buttonPanel1";
            this.buttonPanel1.Size = new System.Drawing.Size(480, 80);
            // 
            // _connectButton
            // 
            this._connectButton.ConnectedImage = Properties.Resources.connected;// ((System.Drawing.Bitmap)(resources.GetObject("_connectButton.ConnectedImage")));
            this._connectButton.ConnectedImageDown = Properties.Resources.connected_down;// (((System.Drawing.Bitmap)(resources.GetObject("_connectButton.ConnectedImageDown")));
            this._connectButton.DisconnectedImage = Properties.Resources.disconnected;// (((System.Drawing.Bitmap)(resources.GetObject("_connectButton.DisconnectedImage")));
            this._connectButton.DisconnectedImageDown = Properties.Resources.disconnected_down;// (((System.Drawing.Bitmap)(resources.GetObject("_connectButton.DisconnectedImageDown")));
            this._connectButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._connectButton.Location = new System.Drawing.Point(0, 0);
            this._connectButton.Name = "_connectButton";
            this._connectButton.Size = new System.Drawing.Size(80, 80);
            this._connectButton.TabIndex = 14;
            this._connectButton.DisconnectRequested += new System.EventHandler(this.connectButton1_DisconnectRequested);
            this._connectButton.ConnectRequested += new System.EventHandler(this.connectButton1_ConnectRequested);
            // 
            // _optionsButton
            // 
            this._optionsButton.Image = Properties.Resources.settings;
            this._optionsButton.SelectedImage = Properties.Resources.settings_sel;
            this._optionsButton.DownImage = Properties.Resources.settings_down;
            this._optionsButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._optionsButton.Location = new System.Drawing.Point(0, 0);
            this._optionsButton.Name = "_optionsButton";
            this._optionsButton.Size = new System.Drawing.Size(80, 80);
            this._optionsButton.TabIndex = 15;
            this._optionsButton.Click += new System.EventHandler(_optionsButton_Click);
            this._optionsButton.AllowSelect = false;
            // 
            // _inputsButton
            // 
            this._inputsButton.Image = Properties.Resources.inputs;
            this._inputsButton.SelectedImage = Properties.Resources.inputs;
            this._inputsButton.DownImage = Properties.Resources.inputs_down;
            this._inputsButton.Dock = System.Windows.Forms.DockStyle.Left;
            this._inputsButton.Location = new System.Drawing.Point(0, 0);
            this._inputsButton.Name = "_inputsButton";
            this._inputsButton.Size = new System.Drawing.Size(80, 80);
            this._inputsButton.TabIndex = 0;
            this._inputsButton.Click += new System.EventHandler(_inputsButton_Click);
            this._inputsButton.AllowSelect = false;
            // 
            // MobileRemoteUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 640);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonPanel1);
            this.KeyPreview = true;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(MobileRemoteUI_KeyPress);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "MobileRemoteUI";
            this.Text = "MobileRemote";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Activated += new System.EventHandler(this.Form1_Activated);
            this.buttonPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        void MobileRemoteUI_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)System.Windows.Forms.Keys.Escape)
            {
                e.Handled = true;
            }
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private ButtonPanel buttonPanel1;
        private ConnectButton _connectButton;
        private PushButton _optionsButton;
        private PushButton _inputsButton;
    }
}

