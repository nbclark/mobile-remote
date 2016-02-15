using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class TouchPanelEventArgs : EventArgs
    {
        public int DX, DY;
        public bool IsScroll;

        public TouchPanelEventArgs(int dx, int dy, bool isScroll)
        {
            DX = dx;
            DY = dy;
            IsScroll = isScroll;
        }
    }
    internal class TouchPanel : UserControl
    {
        private static Bitmap _backGround = null;
        public event EventHandler<TouchPanelEventArgs> MouseDelta;
        public event EventHandler ButtonClick;
        public event EventHandler ButtonDoubleClick;

        static TouchPanel()
        {
            _backGround = Properties.Resources.touchpanel;
        }

        public TouchPanel()
        {
            this.MouseDown += new MouseEventHandler(TouchPanel_MouseDown);
            this.MouseMove += new MouseEventHandler(TouchPanel_MouseMove);
            this.MouseUp += new MouseEventHandler(TouchPanel_MouseUp);
        }

        List<Point> mousePoints = new List<Point>();
        private int _downTick = 0, _lastClick = 0;

        private void TouchPanel_MouseUp(object sender, MouseEventArgs e)
        {
            long elapsedTime = System.Environment.TickCount - _downTick;

            if (elapsedTime < 100)
            {
                long timeSinceLastTap = System.Environment.TickCount - _lastClick;

                if (null != ButtonClick)
                {
                    ButtonClick(this, null);
                }
                _lastClick = System.Environment.TickCount;
            }
            else
            {
                _lastClick = 0;
            }
            mousePoints.Clear();
            this.Refresh();
        }

        private void TouchPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if ((System.Environment.TickCount - _downTick) < 50)
            {
                return;
            }

            mousePoints.Add(new Point(e.X, e.Y));

            if (null != MouseDelta && mousePoints.Count > 1)
            {
                int dx = (int)((int)mousePoints[mousePoints.Count - 1].X - (int)mousePoints[mousePoints.Count - 2].X);
                int dy = (int)((int)mousePoints[mousePoints.Count - 1].Y - (int)mousePoints[mousePoints.Count - 2].Y);

                if (dx != 0 || dy != 0)
                {
                    MouseDelta(this, new TouchPanelEventArgs(dx, dy, e.X > (this.Right - 60)));
                }
            }

            this.Refresh();
        }

        private void TouchPanel_MouseDown(object sender, MouseEventArgs e)
        {
            _downTick = System.Environment.TickCount;
            mousePoints.Clear();
            mousePoints.Add(new Point(e.X, e.Y));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            this.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (mousePoints.Count > 1)
            {
                using (Pen fingerPen = new Pen(Color.White, 10))
                {
                    e.Graphics.DrawLines(fingerPen, mousePoints.ToArray());
                }
            }
            else
            {
                e.Graphics.DrawImage(_backGround, this.ClientRectangle, 0, 0, _backGround.Width, _backGround.Height, GraphicsUnit.Pixel, new System.Drawing.Imaging.ImageAttributes());
                //e.Graphics.Clear(Color.LightGray);
                //using (SolidBrush blackBrush = new SolidBrush(Color.Black))
                //{
                //    e.Graphics.FillRectangle(blackBrush, this.ClientRectangle);
                //}
                using (SolidBrush whiteBrush = new SolidBrush(Color.Black))
                {
                    //e.Graphics.FillRectangle(whiteBrush, 50, 30, this.Width - 100, 5);
                    e.Graphics.FillRectangle(whiteBrush, this.Width - 30, 50, 8, this.Height - 100);
                }
            }
        }
    }
}
