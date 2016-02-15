using System;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class ConnectSuccess : CustomWindow
    {
        private int _oldIndex = -1;
        public ConnectSuccess()
        {
            InitializeComponent();

            foreach (AddressSelector.BluetoothDevice device in AddressSelector.GetPreviousDevices())
            {
                _addressComboBox.Items.Add(device);
            }
            _addressComboBox.Items.Add(new AddressSelector.BluetoothDevice("Pair With New Machine", 0, false));

            _connectMacroComboBox.DisplayMember = "Key";
            _connectMacroComboBox.ValueMember = "Value";
            _disconnectMacroComboBox.DisplayMember = "Key";
            _disconnectMacroComboBox.ValueMember = "Value";

            KeyValuePair<string, string> emptyMacro = new KeyValuePair<string, string>("-none-", string.Empty);
            _connectMacroComboBox.Items.Add(emptyMacro);
            _disconnectMacroComboBox.Items.Add(emptyMacro);

            foreach (string fileName in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "*.mrm"))
            {
                KeyValuePair<string, string> macro = new KeyValuePair<string, string>(Path.GetFileName(fileName), fileName);
                _connectMacroComboBox.Items.Add(macro);
                _disconnectMacroComboBox.Items.Add(macro);
            }
            _addressComboBox.SelectedIndex = 0;
        }

        public override void SetFocus()
        {
            _addressComboBox.Focus();
        }

        private void menuItem1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void menuItem2_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        protected override void OnClose()
        {
            base.OnClose();

            if (DialogResult.OK == this.DialogResult)
            {
                SaveAddressSettings();
                Properties.Settings.Default.Save();
            }
        }

        private void SaveAddressSettings()
        {
            ConnectionSettingCollection.ConnectionSetting setting = null;
            AddressSelector.BluetoothDevice device = (AddressSelector.BluetoothDevice)_addressComboBox.Items[_oldIndex];
            foreach (ConnectionSettingCollection.ConnectionSetting macroPair in Properties.Settings.Default.ConnectionSettings)
            {
                if (macroPair.Address == device.Address)
                {
                    setting = macroPair;
                    break;
                }
            }
            if (null == setting)
            {
                setting = new ConnectionSettingCollection.ConnectionSetting();
                Properties.Settings.Default.ConnectionSettings.Add(setting);
                setting.Address = device.Address;
                setting.LegacyMode = device.LegacyMode;
            }
            setting.ConnectMacro = ((KeyValuePair<string, string>)_connectMacroComboBox.SelectedItem).Value;
            setting.DisconnectMacro = ((KeyValuePair<string, string>)_disconnectMacroComboBox.SelectedItem).Value;
        }

        private void _addressComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            if (_oldIndex > -1)
            {
                // TODO: check if things actually changed
                //if (DialogResult.Yes == MessageBox.Show("Would you like to save changes?", "Save Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    SaveAddressSettings();
                }
            }

            _connectMacroComboBox.SelectedIndex = 0;
            _disconnectMacroComboBox.SelectedIndex = 0;

            AddressSelector.BluetoothDevice device = (AddressSelector.BluetoothDevice)_addressComboBox.SelectedItem;
            foreach (ConnectionSettingCollection.ConnectionSetting macroPair in Properties.Settings.Default.ConnectionSettings)
            {
                if (macroPair.Address == device.Address)
                {
                    foreach (KeyValuePair<string, string> value in _connectMacroComboBox.Items)
                    {
                        if (string.Equals(value.Value, macroPair.ConnectMacro))
                        {
                            _connectMacroComboBox.SelectedItem = value;
                            break;
                        }
                    }
                    foreach (KeyValuePair<string, string> value in _disconnectMacroComboBox.Items)
                    {
                        if (string.Equals(value.Value, macroPair.DisconnectMacro))
                        {
                            _disconnectMacroComboBox.SelectedItem = value;
                            break;
                        }
                    }
                    break;
                }
            }
            _oldIndex = _addressComboBox.SelectedIndex;
        }
    }
}