namespace MobileSRC.MobileRemote
{
    partial class MouseOptions
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
            this._accelerometerCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // _accelerometerCheckBox
            // 
            this._accelerometerCheckBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._accelerometerCheckBox.Location = new System.Drawing.Point(0, 0);
            this._accelerometerCheckBox.Name = "_accelerometerCheckBox";
            this._accelerometerCheckBox.Text = "Use Accelerometer to Control";
            this._accelerometerCheckBox.Size = new System.Drawing.Size(480, 40);
            // 
            // WaitForConnection
            // 
            _contentPanel.Controls.Add(_accelerometerCheckBox);
            this.AutoScroll = true;
            this.Location = new System.Drawing.Point(0, 52);
            this.ClientSize = new System.Drawing.Size(480, 400);
            this.Text = "Mouse Options...";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox _accelerometerCheckBox;

    }
}