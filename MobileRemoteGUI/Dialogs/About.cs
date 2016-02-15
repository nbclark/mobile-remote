using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this._splashPicture.Image = Properties.Resources.mobileremote_splash;
        }
    }
}