using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class WaitForConnection : CustomWindow
    {
        private WaitHandle _waitResult = null;
        private BluetoothHidWriter _hidWriter;

        public WaitForConnection()
        {
            InitializeComponent();
            this.ShowOKButton = false;
            this.ShowCancelButton = true;
        }

        public override void SetFocus()
        {
            progressBar1.Focus();
        }

        public DialogResult ShowDialog(MobileRemoteUI parentForm, WaitHandle waitResult, AddressSelector.BluetoothDevice device, BluetoothHidWriter hidWriter)
        {
            _statusLabel.Text = "Connecting...";
            _hidWriter = hidWriter;
            _waitResult = waitResult;
            if (device.Address > 0)
            {
                _existingMachine.Dock = DockStyle.Fill;
                _existingMachine.Visible = true;
                _newMachineLabel.Visible = false;
            }
            else
            {
                _newMachineLabel.Dock = DockStyle.Fill;
                _newMachineLabel.Visible = true;
                _existingMachine.Visible = false;
            }

            timer1.Enabled = true;

            if (waitResult.WaitOne(1000, false))
            {
                //return DialogResult.OK;
            }

            try
            {
                return base.ShowDialog(parentForm);
            }
            finally
            {
                timer1.Enabled = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (null != _waitResult)
            {
                if (_waitResult.WaitOne(1, false))
                {
                    timer1.Enabled = false;

                    if (_hidWriter.IsConnected)
                    {
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        // we have an error here
                        _statusLabel.Text = "Error Connecting: " + string.Format("[{0}] {1}", Program.Debug.ErrorCode, Program.Debug.LastError);
                        _existingMachine.Visible = _newMachineLabel.Visible = false;
                        _connectionErrorLabel.Visible = true;
                        progressBar1.Value = 0;

                        return;
                    }
                }
            }
            progressBar1.Value = (progressBar1.Value + 10) % (progressBar1.Maximum+1);
        }
    }
}