namespace MobileSRC.MobileRemote
{
    partial class TitleBar
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
            this.titleLabel1 = new TitleLabel();
            this._cancelButton = new ImageButton();
            this._okButton = new ImageButton();
            this.SuspendLayout();
            // 
            // titleLabel1
            // 
            this.titleLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titleLabel1.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.titleLabel1.ForeColor = System.Drawing.Color.White;
            this.titleLabel1.Location = new System.Drawing.Point(0, 0);
            this.titleLabel1.Name = "titleLabel1";
            this.titleLabel1.Size = new System.Drawing.Size(320, 80);
            this.titleLabel1.TabIndex = 0;
            this.titleLabel1.TabStop = false;
            // 
            // _cancelButton
            // 
            this._cancelButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._cancelButton.Location = new System.Drawing.Point(400, 0);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(80, 80);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.ButtonClick += new System.EventHandler(this._cancelButton_Click);
            this._cancelButton.TabStop = false;
            // 
            // _okButton
            // 
            this._okButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._okButton.Location = new System.Drawing.Point(320, 0);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(80, 80);
            this._okButton.TabIndex = 2;
            this._okButton.ButtonClick += new System.EventHandler(this._okButton_Click);
            this._okButton.TabStop = false;
            // 
            // TitleBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(192F, 192F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.titleLabel1);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "TitleBar";
            this.TabStop = false;
            this.Size = new System.Drawing.Size(480, 80);
            this.ResumeLayout(false);

        }

        #endregion

        private TitleLabel titleLabel1;
        private ImageButton _cancelButton;
        private ImageButton _okButton;
    }
}
