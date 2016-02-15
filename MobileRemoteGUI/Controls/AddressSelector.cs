using System;
using Microsoft.Win32;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class AddressSelector : CustomWindow
    {
        public AddressSelector()
        {
            InitializeComponent();
            _addressComboBox.SelectedIndexChanged += new EventHandler(_addressComboBox_SelectedIndexChanged);

            foreach (BluetoothDevice device in GetPreviousDevices())
            {
                _addressComboBox.Items.Add(device);
            }
            _addressComboBox.Items.Add(new BluetoothDevice("Pair With New Machine", 0, false));
            _addressComboBox.SelectedIndex = 0;
        }

        void _addressComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            BluetoothDevice device = (BluetoothDevice)_addressComboBox.SelectedItem;
            _legacyCheckBox.Checked = device.LegacyMode;
        }

        internal class BluetoothDevice
        {
            public string Name;
            public UInt64 Address;
            public bool LegacyMode;

            public BluetoothDevice(string name, UInt64 address, bool legacyMode)
            {
                Name = name;
                Address = address;
                LegacyMode = legacyMode;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public bool LegacyMode
        {
            get { return _legacyCheckBox.Checked; }
        }

        public BluetoothDevice SelectedDevice
        {
            get { return _addressComboBox.SelectedItem as BluetoothDevice; }
        }

        public static void AddNewAddress(UInt64 address, bool legacyMode)
        {
            if (!Utils.CheckRegistration())
            {
                return;
            }
            List<BluetoothDevice> devices = GetPreviousDevices();

            string szName = string.Empty;// address.ToString("C");

            try
            {
                int num;
                byte[] szString = new byte[0x1f0];
                if (0 == L2CAPAPI.BthRemoteNameQuery(ref address, 0xf8, out num, szString) || num == 0)
                {
                    szName = Encoding.Unicode.GetString(szString, 0, (num - 1) * 2); ;
                }
            }
            catch
            {
            }

            if (string.IsNullOrEmpty(szName))
            {
                using (StringInput si = new StringInput("Enter Name", "Enter the name of this connection..."))
                {
                    si.ShowDialog(MobileRemoteUI.Instance);
                    szName = si.Value;
                }
            }

            StringBuilder sbAddresses = new StringBuilder();
            sbAddresses.Append(String.Format("{0}={1}={2}", szName, address, legacyMode));

            List<string> names = new List<string>();
            List<ulong> addrs = new List<ulong>();
            List<bool> legModes = new List<bool>();

            for (int i = 0; i < devices.Count; ++i)
            {
                if (names.Contains(devices[i].Name) || addrs.Contains(devices[i].Address))
                {
                    devices.RemoveAt(i);
                    i--;
                }
                else
                {
                    names.Add(devices[i].Name);
                    addrs.Add(devices[i].Address);
                    legModes.Add(devices[i].LegacyMode);
                }
            }

            for (int i = 0; i < devices.Count; ++i)
            {
                if (devices[i].Address != address)
                {
                    sbAddresses.Append(",");
                    sbAddresses.Append(String.Format("{0}={1}={2}", devices[i].Name, devices[i].Address, devices[i].LegacyMode));
                }
            }

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\MobileSrc\MobileRemote"))
            {
                key.SetValue("PreviousAddresses", sbAddresses.ToString(), RegistryValueKind.String);
            }
        }

        public static List<BluetoothDevice> GetPreviousDevices()
        {
            List<BluetoothDevice> devices = new List<BluetoothDevice>();
            if (!Utils.CheckRegistration())
            {
                return devices;
            }
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\MobileSrc\MobileRemote"))
            {
                string[] addresses = Convert.ToString(key.GetValue("PreviousAddresses", "")).Split(',');

                foreach (string address in addresses)
                {
                    string[] parts = address.Split('=');

                    if (parts.Length > 1)
                    {
                        try
                        {
                            UInt64 bthAddress = UInt64.Parse(parts[1]);
                            devices.Add(new BluetoothDevice(parts[0], bthAddress, (parts.Length > 2) ? Convert.ToBoolean(parts[2]) : false));
                        }
                        catch
                        {
                        }
                    }
                }
            }
            return devices;
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
    }
}