namespace MobileSRC.MobileRemote
{
    partial class ConnectSuccess
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectSuccess));
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this._addressComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._connectMacroComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this._disconnectMacroComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Select";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Cancel";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // _addressComboBox
            // 
            this._addressComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._addressComboBox.Location = new System.Drawing.Point(0, 40);
            this._addressComboBox.Name = "_addressComboBox";
            this._addressComboBox.Size = new System.Drawing.Size(480, 41);
            this._addressComboBox.TabIndex = 1;
            this._addressComboBox.SelectedIndexChanged += new System.EventHandler(this._addressComboBox_SelectedValueChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(480, 20);
            this.label1.Text = "Select a Machine:";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(480, 40);
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // _connectMacroComboBox
            // 
            this._connectMacroComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._connectMacroComboBox.Location = new System.Drawing.Point(0, 392);
            this._connectMacroComboBox.Name = "_connectMacroComboBox";
            this._connectMacroComboBox.Size = new System.Drawing.Size(480, 41);
            this._connectMacroComboBox.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 352);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(480, 20);
            this.label3.Text = "Connect Macro:";
            // 
            // _disconnectMacroComboBox
            // 
            this._disconnectMacroComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._disconnectMacroComboBox.Location = new System.Drawing.Point(0, 473);
            this._disconnectMacroComboBox.Name = "_disconnectMacroComboBox";
            this._disconnectMacroComboBox.Size = new System.Drawing.Size(480, 41);
            this._disconnectMacroComboBox.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label4.Location = new System.Drawing.Point(0, 433);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(480, 20);
            this.label4.Text = "Disconnect Macro:";
            // 
            // ConnectSuccess
            // 
            this.AutoScroll = true;
            this._contentPanel.Controls.Add(this._disconnectMacroComboBox);
            this._contentPanel.Controls.Add(this.label4);
            this._contentPanel.Controls.Add(this._connectMacroComboBox);
            this._contentPanel.Controls.Add(this.label3);
            this._contentPanel.Controls.Add(this._addressComboBox);
            this._contentPanel.Controls.Add(this.label1);
            this.Text = "Connection Settings";
            this.Size = new System.Drawing.Size(480, 536);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.ComboBox _addressComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox _connectMacroComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox _disconnectMacroComboBox;
        private System.Windows.Forms.Label label4;
    }
}