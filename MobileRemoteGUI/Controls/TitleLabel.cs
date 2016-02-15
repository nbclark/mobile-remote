using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class TitleLabel : UserControl
    {
        private string _text = string.Empty;
        private Bitmap _backgroundImage;
        public TitleLabel()
        {
            _backgroundImage = Properties.Resources.titlebar;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_backgroundImage, this.ClientRectangle, new Rectangle(0, 0, _backgroundImage.Width, _backgroundImage.Height), GraphicsUnit.Pixel);
            using (SolidBrush brush = new SolidBrush(this.ForeColor))
            {
                SizeF size = e.Graphics.MeasureString(this.Text, this.Font);
                e.Graphics.DrawString(this.Text, this.Font, brush, 5, this.Height / 2 - size.Height / 2);
            }
            //base.OnPaint(e);
        }
    }
}
