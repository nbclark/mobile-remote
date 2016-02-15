using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class SipSelector : CustomWindow
    {
        public SipSelector()
        {
            InitializeComponent();
            this.ShowOKButton = true;
            this.ShowCancelButton = true;

            if (!Platform.IsWindowsMobileStandard)
            {
                _sipComboBox.DisplayMember = "Name";
                _sipComboBox.ValueMember = "Clsid";
                _sipComboBox.DataSource = InputSelector.InputMethods;
            }
            else
            {
                base._contentPanel.Controls.Remove(_sipLabel);
                base._contentPanel.Controls.Remove(_sipComboBox);
            }

            _orientationComboBox.Items.Add("Landscape: Top on Left");
            _orientationComboBox.Items.Add("Landscape: Top on Right");
            _orientationComboBox.Items.Add("Portrait");

            switch (Properties.Settings.Default.Keyboard_Rotation)
            {
                case KeyboardRotation.ROT_270:
                    {
                        _orientationComboBox.SelectedIndex = 0;
                    }
                    break;
                case KeyboardRotation.ROT_90:
                    {
                        _orientationComboBox.SelectedIndex = 1;
                    }
                    break;
                default:
                    {
                        _orientationComboBox.SelectedIndex = 2;
                    }
                    break;
            }

            foreach (string file in Directory.GetFiles(Path.Combine(Utils.GetWorkingDirectory(), "KeyboardLayouts"), "*.kbd"))
            {
                _layoutComboBox.Items.Add(Path.GetFileName(file));
            }
            _layoutComboBox.Text = Properties.Settings.Default.Keyboard_DefaultLayout;
        }

        public override void SetFocus()
        {
            if (!Platform.IsWindowsMobileStandard)
            {
                _sipComboBox.SelectedValue = InputSelector.CurrentInputMethod.Clsid;
                _sipComboBox.Focus();
            }
            else
            {
                _orientationComboBox.Focus();
            }
        }

        public override void OnLoad()
        {
            try
            {
                base.OnLoad();

                if (!Platform.IsWindowsMobileStandard)
                {
                    _sipComboBox.SelectedValue = InputSelector.CurrentInputMethod.Clsid;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public InputMethod SelectedMethod
        {
            get { return _sipComboBox.SelectedItem as InputMethod; }
        }

        public string KeyboardLayout
        {
            get { return _layoutComboBox.Text; }
        }

        public KeyboardRotation Rotation
        {
            get
            {
                switch (_orientationComboBox.SelectedIndex)
                {
                    case 0:
                        {
                            return KeyboardRotation.ROT_270;
                        }
                    case 1:
                        {
                            return KeyboardRotation.ROT_90;
                        }
                    default:
                        {
                            return KeyboardRotation.ROT_0;
                        }
                }
            }
        }
    }
}