namespace MobileSRC.MobileRemote
{
    partial class About
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
            this._splashPicture = new System.Windows.Forms.PictureBox();
            this._versionLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _splashPicture
            // 
            this._splashPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this._splashPicture.Location = new System.Drawing.Point(0, 0);
            this._splashPicture.Name = "_splashPicture";
            this._splashPicture.Size = new System.Drawing.Size(480, 496);
            this._splashPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            // 
            // _versionLabel
            // 
            this._versionLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._versionLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this._versionLabel.Location = new System.Drawing.Point(0, 496);
            this._versionLabel.Name = "_versionLabel";
            this._versionLabel.Size = new System.Drawing.Size(480, 40);
            this._versionLabel.Text = "mobileSRC MobileRemote 1.1";
            this._versionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 536);
            this.Controls.Add(this._splashPicture);
            this.Controls.Add(this._versionLabel);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.Name = "About";
            this.Text = "About MobileRemote";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _splashPicture;
        private System.Windows.Forms.Label _versionLabel;
    }
}