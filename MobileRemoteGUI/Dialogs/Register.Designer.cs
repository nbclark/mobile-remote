namespace MobileSRC.MobileRemote
{
    partial class Register
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
            this._ownerLabel = new System.Windows.Forms.Label();
            this._ownerTextBox = new System.Windows.Forms.TextBox();
            this._passLabel = new System.Windows.Forms.Label();
            this._passTextBox = new System.Windows.Forms.TextBox();
            this._splashPicture = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuItem1);
            this.mainMenu1.MenuItems.Add(this.menuItem2);
            // 
            // menuItem1
            // 
            this.menuItem1.Text = "Demo Mode";
            this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Text = "Register";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // _ownerLabel
            // 
            this._ownerLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this._ownerLabel.Location = new System.Drawing.Point(3, 3);
            this._ownerLabel.Name = "_ownerLabel";
            this._ownerLabel.Size = new System.Drawing.Size(107, 40);
            this._ownerLabel.Text = "Owner";
            // 
            // _ownerTextBox
            // 
            this._ownerTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ownerTextBox.Location = new System.Drawing.Point(116, 3);
            this._ownerTextBox.Name = "_ownerTextBox";
            this._ownerTextBox.ReadOnly = true;
            this._ownerTextBox.Size = new System.Drawing.Size(361, 41);
            this._ownerTextBox.TabIndex = 1;
            // 
            // _passLabel
            // 
            this._passLabel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this._passLabel.Location = new System.Drawing.Point(3, 50);
            this._passLabel.Name = "_passLabel";
            this._passLabel.Size = new System.Drawing.Size(107, 40);
            this._passLabel.Text = "Code";
            // 
            // _passTextBox
            // 
            this._passTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._passTextBox.Location = new System.Drawing.Point(116, 50);
            this._passTextBox.Name = "_passTextBox";
            this._passTextBox.Size = new System.Drawing.Size(361, 41);
            this._passTextBox.TabIndex = 2;
            this._passTextBox.GotFocus += new System.EventHandler(this._passTextBox_GotFocus);
            this._passTextBox.LostFocus += new System.EventHandler(this._passTextBox_LostFocus);
            // 
            // _splashPicture
            // 
            this._splashPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._splashPicture.Location = new System.Drawing.Point(0, 97);
            this._splashPicture.Name = "_splashPicture";
            this._splashPicture.Size = new System.Drawing.Size(480, 439);
            this._splashPicture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            // 
            // Register
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(480, 536);
            this.Controls.Add(this._splashPicture);
            this.Controls.Add(this._passLabel);
            this.Controls.Add(this._passTextBox);
            this.Controls.Add(this._ownerTextBox);
            this.Controls.Add(this._ownerLabel);
            this.Location = new System.Drawing.Point(0, 52);
            this.Menu = this.mainMenu1;
            this.Name = "Register";
            this.Text = "Register MobileRemote";
            this.Load += new System.EventHandler(this.Register_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.Label _ownerLabel;
        private System.Windows.Forms.TextBox _ownerTextBox;
        private System.Windows.Forms.Label _passLabel;
        private System.Windows.Forms.TextBox _passTextBox;
        private System.Windows.Forms.PictureBox _splashPicture;
    }
}