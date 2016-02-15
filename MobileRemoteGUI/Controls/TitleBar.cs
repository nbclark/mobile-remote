using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class TitleBar : UserControl
    {
        private static Bitmap _close = null, _closeDown = null, _ok = null, _okDown = null;

        static TitleBar()
        {
            _close = Properties.Resources.closebutton;
            _closeDown = Properties.Resources.closebuttondown;

            _ok = Properties.Resources.okbutton;
            _okDown = Properties.Resources.okbuttondown;
        }

        public event EventHandler<CancelEventArgs> Closing;

        public TitleBar()
        {
            InitializeComponent();

            _cancelButton.Image = _close;
            _cancelButton.DownImage = _closeDown;
            _cancelButton.SelectedImage = _closeDown;

            _okButton.Image = _ok;
            _okButton.DownImage = _okDown;
            _okButton.SelectedImage = _okDown;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _cancelButton.Width = _cancelButton.Height;
            _okButton.Width = _okButton.Height;
        }

        public bool ShowOKButton
        {
            get { return _okButton.Visible; }
            set { _okButton.Visible = value; }
        }

        public bool ShowCancelButton
        {
            get { return _cancelButton.Visible; }
            set { _cancelButton.Visible = value; }
        }

        public new string Text
        {
            get { return titleLabel1.Text; }
            set { titleLabel1.Text = value; }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            if (null != Closing)
            {
                Closing(this, new CancelEventArgs(true));
            }
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            if (null != Closing)
            {
                Closing(this, new CancelEventArgs(false));
            }
        }
    }
}
