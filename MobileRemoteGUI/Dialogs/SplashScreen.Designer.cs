namespace MobileSRC.MobileRemote
{
    partial class SplashScreen
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
            this._splashPicture = new System.Windows.Forms.PictureBox();
            this._timer = new System.Windows.Forms.Timer();
            this._progressImage = new TransparentPictureBox();
            this.SuspendLayout();
            // 
            // _splashPicture
            // 
            this._splashPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splashPicture.Location = new System.Drawing.Point(0, 0);
            this._splashPicture.Name = "_splashPicture";
            this._splashPicture.Size = new System.Drawing.Size(480, 640);
            this._splashPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            // 
            // _timer
            // 
            this._timer.Enabled = true;
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // _progressImage
            // 
            this._progressImage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._progressImage.Location = new System.Drawing.Point(0, 560);
            this._progressImage.Name = "_progressImage";
            this._progressImage.Size = new System.Drawing.Size(480, 80);
            this._progressImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 640);
            this.ControlBox = false;
            this.Controls.Add(this._progressImage);
            this.Controls.Add(this._splashPicture);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "SplashScreen";
            this.Text = "Launching MobileRemote...";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.PictureBox _splashPicture;
        private System.Windows.Forms.Timer _timer;
        private TransparentPictureBox _progressImage;
    }
}