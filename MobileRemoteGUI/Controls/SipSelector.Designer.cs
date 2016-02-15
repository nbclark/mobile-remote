namespace MobileSRC.MobileRemote
{
    partial class SipSelector
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
            this._orientationComboBox = new System.Windows.Forms.ComboBox();
            this._sipLabel = new System.Windows.Forms.Label();
            this._orientationLabel = new System.Windows.Forms.Label();
            this._layoutLabel = new System.Windows.Forms.Label();
            this._sipComboBox = new System.Windows.Forms.ComboBox();
            this._layoutComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _sipComboBox
            // 
            this._sipComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._sipComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._sipComboBox.Location = new System.Drawing.Point(0, 0);
            this._sipComboBox.Name = "_sipComboBox";
            this._sipComboBox.TabIndex = 1;
            this._sipComboBox.Size = new System.Drawing.Size(480, 40);
            // 
            // _layoutLabel
            // 
            this._layoutLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._layoutLabel.Location = new System.Drawing.Point(0, 0);
            this._layoutLabel.Name = "_layoutLabel";
            this._layoutLabel.Text = "Select Keyboard Layout:";
            this._layoutLabel.Size = new System.Drawing.Size(480, 40);
            // 
            // _layoutComboBox
            // 
            this._layoutComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._layoutComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._layoutComboBox.Location = new System.Drawing.Point(0, 0);
            this._layoutComboBox.Name = "_layoutComboBox";
            this._layoutComboBox.TabIndex = 2;
            this._layoutComboBox.Size = new System.Drawing.Size(480, 40);
            // 
            // _orientationComboBox
            // 
            this._orientationComboBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._orientationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._orientationComboBox.Location = new System.Drawing.Point(0, 0);
            this._orientationComboBox.Name = "_orientationComboBox";
            this._orientationComboBox.TabIndex = 0;
            this._orientationComboBox.Size = new System.Drawing.Size(480, 40);
            // 
            // _orientationLabel
            // 
            this._orientationLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._orientationLabel.Location = new System.Drawing.Point(0, 0);
            this._orientationLabel.Name = "_layoutLabel";
            this._orientationLabel.Text = "Select Keyboard Orientation:";
            this._orientationLabel.Size = new System.Drawing.Size(480, 40);
            // 
            // _sipLabel
            // 
            this._sipLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._sipLabel.Location = new System.Drawing.Point(0, 0);
            this._sipLabel.Name = "_sipLabel";
            this._sipLabel.Text = "Select SIP Mode:";
            this._sipLabel.Size = new System.Drawing.Size(480, 40);
            // 
            // WaitForConnection
            // 
            _contentPanel.Controls.Add(_sipComboBox);
            _contentPanel.Controls.Add(_sipLabel);
            _contentPanel.Controls.Add(_orientationComboBox);
            _contentPanel.Controls.Add(_orientationLabel);
            _contentPanel.Controls.Add(_layoutComboBox);
            _contentPanel.Controls.Add(_layoutLabel);
            this.AutoScroll = true;
            this.Location = new System.Drawing.Point(0, 52);
            this.ClientSize = new System.Drawing.Size(480, 400);
            this.Text = "Select Input...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _sipLabel;
        private System.Windows.Forms.Label _orientationLabel;
        private System.Windows.Forms.ComboBox _orientationComboBox;
        private System.Windows.Forms.ComboBox _sipComboBox;
        private System.Windows.Forms.ComboBox _layoutComboBox;
        private System.Windows.Forms.Label _layoutLabel;

    }
}