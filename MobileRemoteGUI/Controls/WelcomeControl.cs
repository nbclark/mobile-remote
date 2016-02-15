using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class WelcomeControl : PictureBox, IInputControl
    {
        private BluetoothHidWriter _hidWriter;

        public WelcomeControl()
        {
            InitializeComponent();
        }

        public bool RequiresTouchscreen
        {
            get { return false; }
        }

        public void Shutdown()
        {
        }

        public void ShowSettings()
        {
            //
        }

        public void SelectedClick()
        {
            //
        }

        public Orientation DesiredOrientation
        {
            get { return Orientation.Horizontal; }
        }

        public BluetoothHidWriter HidWriter
        {
            get { return _hidWriter; }
            set { _hidWriter = value; }
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WelcomeControl));
            this.SuspendLayout();
            // 
            // WelcomeControl
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            //this.Image = ((System.Drawing.Image)(resources.GetObject("$this.Image")));
            this.Size = new System.Drawing.Size(467, 70);
            this.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.ResumeLayout(false);

        }

        #region IInputControl Members



        #endregion

        #region IInputControl Members


        public bool HandleKeyDown(KeyEventArgs e)
        {
            return false;
        }

        public bool HandleKeyUp(KeyEventArgs e)
        {
            return false;
        }

        public bool HandleKeyPress(KeyPressEventArgs e)
        {
            return false;
        }

        #endregion
    }
}