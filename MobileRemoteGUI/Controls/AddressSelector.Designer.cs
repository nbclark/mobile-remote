namespace MobileSRC.MobileRemote
{
    partial class AddressSelector
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
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this._addressComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._legacyCheckBox = new System.Windows.Forms.CheckBox();
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
            // _legacyCheckBox
            // 
            this._legacyCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._legacyCheckBox.Location = new System.Drawing.Point(0, 30);
            this._legacyCheckBox.Name = "_legacyCheckBox";
            this._legacyCheckBox.Text = "Connect in Legacy Mode";
            this._legacyCheckBox.Size = new System.Drawing.Size(480, 41);
            this._legacyCheckBox.TabIndex = 1;
            // 
            // _addressComboBox
            // 
            this._addressComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._addressComboBox.Location = new System.Drawing.Point(0, 30);
            this._addressComboBox.Name = "_addressComboBox";
            this._addressComboBox.Size = new System.Drawing.Size(480, 41);
            this._addressComboBox.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(480, 30);
            this.label1.Text = "Select a Machine:";
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(480, 108);
            this.label2.Text = "If you wish to pair with a new machine, please select \'Pair With New Machine\' fro" +
                "m the drop down and press Select.";
            // 
            // AddressSelector
            // 
            this.AutoScroll = true;
            this._contentPanel.Controls.Add(this.label2);
            this._contentPanel.Controls.Add(this._legacyCheckBox);
            this._contentPanel.Controls.Add(this._addressComboBox);
            this._contentPanel.Controls.Add(this.label1);
            this.Text = "Select Address...";
            this.Size = new System.Drawing.Size(480, 536);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.ComboBox _addressComboBox;
        private System.Windows.Forms.CheckBox _legacyCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}