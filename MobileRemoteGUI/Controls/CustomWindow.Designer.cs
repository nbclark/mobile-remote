namespace MobileSRC.MobileRemote
{
    partial class CustomWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.titleBar1 = new TitleBar();
            this._contentPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // titleBar1
            // 
            this.titleBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.titleBar1.Location = new System.Drawing.Point(0, 0);
            this.titleBar1.Name = "titleBar1";
            this.titleBar1.Size = new System.Drawing.Size(480, 80);
            this.titleBar1.TabIndex = 0;
            this.titleBar1.Closing += new System.EventHandler<System.ComponentModel.CancelEventArgs>(titleBar1_Closing);
            // 
            // _contentPanel
            // 
            this._contentPanel.Location = new System.Drawing.Point(5, 85);
            this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._contentPanel.Name = "_contentPanel";
            this._contentPanel.Size = new System.Drawing.Size(470, 548);
            this._contentPanel.GotFocus += new System.EventHandler(_contentPanel_GotFocus);
            this._contentPanel.LostFocus += new System.EventHandler(_contentPanel_LostFocus);
            // 
            // CustomWindow
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 640);
            this.Controls.Add(this._contentPanel);
            this.Controls.Add(this.titleBar1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "CustomWindow";
            this.Text = "CustomWindow";
            this.ResumeLayout(false);

        }

        void _contentPanel_GotFocus(object sender, System.EventArgs e)
        {
            //
        }

        void _contentPanel_LostFocus(object sender, System.EventArgs e)
        {
            this.SetFocus();
        }

        #endregion

        private TitleBar titleBar1;
        protected System.Windows.Forms.Panel _contentPanel;

    }
}