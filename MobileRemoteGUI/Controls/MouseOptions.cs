using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class MouseOptions : CustomWindow
    {
        public MouseOptions(IGSensor gSensor, bool sensorEnabled)
        {
            InitializeComponent();
            this.ShowOKButton = true;
            this.ShowCancelButton = true;

            _accelerometerCheckBox.Checked = sensorEnabled;
            _accelerometerCheckBox.Enabled = gSensor.IsEnabled;
        }

        public override void SetFocus()
        {
            _accelerometerCheckBox.Focus();
        }

        public override void OnLoad()
        {
            base.OnLoad();
        }

        public bool AccelerometerEnabled
        {
            get { return _accelerometerCheckBox.Checked; }
        }
    }
}