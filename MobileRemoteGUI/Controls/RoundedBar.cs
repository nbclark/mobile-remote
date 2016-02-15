using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class RoundedBar : UserControl
    {
        private static Bitmap _close = null, _ok = null;

        static RoundedBar()
        {
            _close = Properties.Resources.titlebarcornerright;
            _ok = Properties.Resources.titlebarcornerleft;
        }

        public RoundedBar()
        {
            InitializeComponent();

            _cancelButton.Image = _close;
            _okButton.Image = _ok;
        }

        public AlphaLayerControl AlphaControl
        {
            set
            {
                _cancelButton.AlphaControl = value;
                _okButton.AlphaControl = value;
            }
        }
    }
}
