using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
            this._splashPicture.Image = Properties.Resources.mobileremote_splash;
            _ownerTextBox.Text = Utils.GetOwnerName();
            this.DialogResult = DialogResult.Cancel;
        }

        private void _passTextBox_GotFocus(object sender, EventArgs e)
        {
            InputSelector.Enabled = true;
        }

        private void _passTextBox_LostFocus(object sender, EventArgs e)
        {
            InputSelector.Enabled = false;
        }

        private void Register_Load(object sender, EventArgs e)
        {
            _passTextBox.Focus();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("MobileRemote will function normally, but you cannot save paired connections.", "MobileRemote Demo", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            this.Close();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                int registrationCode = Int32.Parse(_passTextBox.Text);
                if (Utils.CheckRegistration(registrationCode))
                {
                    Utils.SetRegistration(registrationCode);
                    MessageBox.Show("MobileRemote successfully activated", "MobileRemote Activated", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
            }
            catch
            {
                //
            }
            MessageBox.Show("Please check your registration code and try again.", "Invalid Code", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }
    }
}