namespace MobileSRC.MobileRemote
{
    partial class InputControlSelector
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
            this._layoutPanel = new FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // _sipComboBox
            // 
            this._layoutPanel.Location = new System.Drawing.Point(0, 0);
            this._layoutPanel.Name = "_layoutPanel";
            this._layoutPanel.TabIndex = 1;
            this._layoutPanel.LayoutStyle = LayoutStyle.FlowLayout;
            this._layoutPanel.Size = new System.Drawing.Size(480, 400);
            this._layoutPanel.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right | System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom;
            // 
            // WaitForConnection
            // 
            _contentPanel.Controls.Add(_layoutPanel);
            this.AutoScroll = true;
            this.Location = new System.Drawing.Point(0, 52);
            this.ClientSize = new System.Drawing.Size(480, 400);
            this.Text = "Select Input...";
            this.ResumeLayout(false);

        }

        #endregion

        private FlowLayoutPanel _layoutPanel;

    }
}