namespace MobileSRC.MobileRemote
{
    partial class StringInput
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
            this._stringLabel = new System.Windows.Forms.Label();
            this._stringTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _stringTextBox
            // 
            this._stringTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._stringTextBox.Location = new System.Drawing.Point(0, 0);
            this._stringLabel.Size = new System.Drawing.Size(480, 30);
            this._stringTextBox.Name = "_stringTextBox";
            this._stringTextBox.TabIndex = 1;
            // 
            // _stringLabel
            // 
            this._stringLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._stringLabel.Location = new System.Drawing.Point(0, 0);
            this._stringLabel.Name = "_stringLabel";
            this._stringLabel.Text = "Enter Value:";
            this._stringLabel.Size = new System.Drawing.Size(480, 30);
            // 
            // WaitForConnection
            // 
            _contentPanel.Controls.Add(_stringTextBox);
            _contentPanel.Controls.Add(_stringLabel);
            this.AutoScroll = true;
            this.Location = new System.Drawing.Point(0, 52);
            this.ClientSize = new System.Drawing.Size(480, 400);
            this.Text = "Enter Value...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label _stringLabel;
        private System.Windows.Forms.TextBox _stringTextBox;

    }
}