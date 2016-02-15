using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class SplashScreen : Form
    {
        private Bitmap[] _waitImages = new Bitmap[4];
        private int _index = 0;

        public SplashScreen()
        {
            InitializeComponent();

            _waitImages[0] = Properties.Resources.waitingorb_0;
            _waitImages[1] = Properties.Resources.waitingorb_1;
            _waitImages[2] = Properties.Resources.waitingorb_2;
            _waitImages[3] = Properties.Resources.waitingorb_3;
            this._splashPicture.Image = Properties.Resources.mobileremote_splash;
            this._progressImage.Image = _waitImages[0];
        }

        void _timer_Tick(object sender, System.EventArgs e)
        {
            _index = (_index + 1) % 12;
            this._progressImage.Image = _waitImages[_index / 3];
            if (MobileRemoteUI.MobileRemoteLoadEvent.WaitOne(1, false))
            {
                this.Close();
                MobileRemoteUI.MobileRemoteReturnEvent.Set();
            }
        }

        internal class TransparentPictureBox : PictureBox
        {
            private Rectangle? _rect = null;

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                try
                {
                    PictureBox pb = (PictureBox)this.Parent.Controls[1];
                    Rectangle imageRectangle = (Rectangle)typeof(PictureBox).GetProperty("_ImageRectangle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(pb, null);
                    if (_rect == null || _rect.Value != imageRectangle)
                    {
                        _rect = imageRectangle;

                        e.Graphics.DrawImage(pb.Image, this.ClientRectangle, new Rectangle(this.Left - imageRectangle.Left, this.Top - imageRectangle.Top, this.Width, this.Height), GraphicsUnit.Pixel);
                    }
                }
                catch
                {
                    e.Graphics.Clear(this.BackColor);
                }
            }
            protected override void OnPaint(PaintEventArgs e)
            {
                ImageAttributes attributes = new ImageAttributes();
                Color clr = ((Bitmap)this.Image).GetPixel(0, 0);
                attributes.SetColorKey(clr, clr);
                e.Graphics.DrawImage(this.Image, new Rectangle((this.Width - this.Image.Width) / 2, (this.Height - this.Image.Height) / 2, this.Image.Width, this.Image.Height), 0, 0, this.Image.Width, this.Image.Height, GraphicsUnit.Pixel, attributes);
            }
        }
    }
}