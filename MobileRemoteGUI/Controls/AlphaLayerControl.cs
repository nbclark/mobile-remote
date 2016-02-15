using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace MobileSRC.MobileRemote
{
    internal class TransparentPictureBox : PictureBox
    {
        private Color _transparentColor = Color.Transparent;
        private AlphaLayerControl _alphaControl = null;

        public Color TransparentColor
        {
            get { return _transparentColor; }
            set { _transparentColor = value; }
        }
        public AlphaLayerControl AlphaControl
        {
            get { return _alphaControl; }
            set { _alphaControl = value; }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (null != _alphaControl && null != _alphaControl._alphaImage)
            {
                Rectangle destRect = this.ClientRectangle;
                Rectangle srcRect = this.RectangleToScreen(this.ClientRectangle);

                e.Graphics.DrawImage(_alphaControl._alphaImage, destRect, srcRect, GraphicsUnit.Pixel);
            }
            System.Drawing.Imaging.ImageAttributes a = new System.Drawing.Imaging.ImageAttributes();
            a.SetColorKey(this.TransparentColor, this.TransparentColor);
            e.Graphics.DrawImage(this.Image, new Rectangle(0, 0, this.Width, this.Height), 0, 0, this.Image.Width, this.Image.Height, GraphicsUnit.Pixel, a);
        }
    }
    internal class AlphaLayerControl : UserControl
    {
        public Bitmap _alphaImage = null;
        public AlphaLayerControl(Graphics parentGraphics, Size size, byte alphaVal)
        {
            try
            {
                _alphaImage = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
                using (Graphics gx = Graphics.FromImage(_alphaImage))
                {
                    IntPtr hdcDst = gx.GetHdc();
                    IntPtr hdcSrc = parentGraphics.GetHdc();
                    Win32.BlendFunction blendFunction = new Win32.BlendFunction();
                    blendFunction.BlendOp = (byte)Win32.BlendOperation.AC_SRC_OVER;   // Only supported blend operation
                    blendFunction.BlendFlags = (byte)Win32.BlendFlags.Zero;           // Documentation says put 0 here
                    blendFunction.SourceConstantAlpha = alphaVal;// Constant alpha factor
                    blendFunction.AlphaFormat = (byte)0; // Don't look for per pixel alpha
                    Win32.AlphaBlend(hdcDst, 0, 0, _alphaImage.Width, _alphaImage.Height, hdcSrc, 0, 0, _alphaImage.Width, _alphaImage.Height, blendFunction);
                    gx.ReleaseHdc(hdcDst);    // Required cleanup to GetHdc()
                    parentGraphics.ReleaseHdc(hdcSrc); // Required cleanup to GetHdc()
                }
            }
            catch
            {
                GC.Collect();
                _alphaImage = new Bitmap(1, 1);
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (null != _alphaImage)
            {
                e.Graphics.DrawImage(_alphaImage, 0, 0);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _alphaImage.Dispose();
        }
    }
}
