namespace MobileSRC.MobileRemote
{
    partial class RoundedBar
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._okButton = new TransparentPictureBox();
            this._cancelButton = new TransparentPictureBox();
            this.SuspendLayout();
            // 
            // _okButton
            // 
            this._okButton.Dock = System.Windows.Forms.DockStyle.Left;
            this._okButton.Location = new System.Drawing.Point(0, 0);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(6, 10);
            this._okButton.TransparentColor = System.Drawing.Color.Black;
            // 
            // _cancelButton
            // 
            this._cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._cancelButton.Location = new System.Drawing.Point(400, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(6, 10);
            this._cancelButton.TransparentColor = System.Drawing.Color.Black;
            // 
            // RoundedBar
            // 
            this.BackColor = System.Drawing.Color.White;
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "RoundedBar";
            this.Size = new System.Drawing.Size(480, 5);
            this.ResumeLayout(false);

        }

        #endregion

        private TransparentPictureBox _okButton;
        private TransparentPictureBox _cancelButton;
    }
}
