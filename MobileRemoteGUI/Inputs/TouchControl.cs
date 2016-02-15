using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class TouchControl : InputControl, IInputControl
    {
        public TouchControl()
        {
            InitializeComponent();
        }

        private ImageButton _leftClickButton;
        private ImageButton _rightClickButton;
        private TouchPanel _touchPanel;
        private BluetoothHidWriter _hidWriter;

        public Orientation DesiredOrientation
        {
            get { return Orientation.Vertical; }
        }

        public BluetoothHidWriter HidWriter
        {
            get { return _hidWriter; }
            set { _hidWriter = value; }
        }

        public bool RequiresTouchscreen
        {
            get { return true; }
        }

        public void Shutdown()
        {
            if (null != _hidWriter)
            {
                // TODO: this is a bug. we should check state here
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _touchPanel.Size = new Size(this.Width, (int)(this.Height * .80));

            _leftClickButton.Top = _rightClickButton.Top = _touchPanel.Bottom;
            _leftClickButton.Width = _rightClickButton.Width = this.Width / 2;
            _leftClickButton.Height = _rightClickButton.Height = (int)(this.Height * .2);
            _leftClickButton.Left = 0;
            _rightClickButton.Left = this.Width / 2;
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TouchControl));
            this._rightClickButton = new ImageButton();
            this._leftClickButton = new ImageButton();
            this._touchPanel = new TouchPanel();
            this.SuspendLayout();
            // 
            // _rightClickButton
            // 
            this._rightClickButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._rightClickButton.DownImage = ((System.Drawing.Bitmap)(resources.GetObject("_rightClickButton.DownImage")));
            this._rightClickButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("_rightClickButton.Image")));
            this._rightClickButton.Location = new System.Drawing.Point(241, 465);
            this._rightClickButton.Name = "_rightClickButton";
            this._rightClickButton.SelectedImage = ((System.Drawing.Bitmap)(resources.GetObject("_rightClickButton.SelectedImage")));
            this._rightClickButton.Size = new System.Drawing.Size(239, 95);
            this._rightClickButton.TabIndex = 1;
            this._rightClickButton.ButtonUp += new System.EventHandler(this._rightClickButton_ButtonUp);
            this._rightClickButton.ButtonClick += new System.EventHandler(this._rightClickButton_ButtonClick);
            this._rightClickButton.ButtonDoubleClick += new EventHandler(this._rightClickButton_ButtonDoubleClick);
            this._rightClickButton.ButtonHold += new System.EventHandler(this._rightClickButton_ButtonHold);
            // 
            // _leftClickButton
            // 
            this._leftClickButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._leftClickButton.DownImage = ((System.Drawing.Bitmap)(resources.GetObject("_leftClickButton.DownImage")));
            this._leftClickButton.Image = ((System.Drawing.Bitmap)(resources.GetObject("_leftClickButton.Image")));
            this._leftClickButton.Location = new System.Drawing.Point(0, 465);
            this._leftClickButton.Name = "_leftClickButton";
            this._leftClickButton.SelectedImage = ((System.Drawing.Bitmap)(resources.GetObject("_leftClickButton.SelectedImage")));
            this._leftClickButton.Size = new System.Drawing.Size(239, 95);
            this._leftClickButton.TabIndex = 0;
            this._leftClickButton.ButtonUp += new System.EventHandler(this._rightClickButton_ButtonUp);
            this._leftClickButton.ButtonClick += new System.EventHandler(this._rightClickButton_ButtonClick);
            this._leftClickButton.ButtonDoubleClick += new EventHandler(this._rightClickButton_ButtonDoubleClick);
            this._leftClickButton.ButtonHold += new System.EventHandler(this._rightClickButton_ButtonHold);
            // 
            // _touchPanel
            // 
            this._touchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._touchPanel.Location = new System.Drawing.Point(0, 0);
            this._touchPanel.Name = "_touchPanel";
            this._touchPanel.Size = new System.Drawing.Size(480, 465);
            this._touchPanel.TabIndex = 2;
            this._touchPanel.MouseDelta += new System.EventHandler<TouchPanelEventArgs>(this._touchPanel_MouseDelta);
            this._touchPanel.ButtonClick += new EventHandler(_rightClickButton_ButtonClick);
            this._touchPanel.ButtonDoubleClick += new EventHandler(_touchPanel_ButtonDoubleClick);
            // 
            // TouchControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this._rightClickButton);
            this.Controls.Add(this._leftClickButton);
            this.Controls.Add(this._touchPanel);
            this.Name = "TouchControl";
            this.Size = new System.Drawing.Size(480, 560);
            this.ResumeLayout(false);

        }

        void _touchPanel_ButtonDoubleClick(object sender, EventArgs e)
        {
            if (null != _hidWriter)
            {
                _hidWriter.SendMouseData(MouseButtons.Left, 0, 0);
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
                System.Threading.Thread.Sleep(150);
                _hidWriter.SendMouseData(MouseButtons.Left, 0, 0);
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
            }
        }

        void _rightClickButton_ButtonHold(object sender, EventArgs e)
        {
            if (null != _hidWriter)
            {
                _hidWriter.SendMouseData((sender == _rightClickButton) ? MouseButtons.Right : MouseButtons.Left, 0, 0);
            }
        }

        void _rightClickButton_ButtonUp(object sender, EventArgs e)
        {
            if (null != _hidWriter)
            {
                // TODO: this is a bug. we should check state here
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
            }
        }

        void _rightClickButton_ButtonClick(object sender, EventArgs e)
        {
            if (null != _hidWriter)
            {
                _hidWriter.SendMouseData((sender == _rightClickButton) ? MouseButtons.Right : MouseButtons.Left, 0, 0);
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
            }
        }

        void _rightClickButton_ButtonDoubleClick(object sender, EventArgs e)
        {
            if (null != _hidWriter)
            {
                _hidWriter.SendMouseData((sender == _rightClickButton) ? MouseButtons.Right : MouseButtons.Left, 0, 0);
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
            }
        }

        private void _touchPanel_MouseDelta(object sender, TouchPanelEventArgs e)
        {
            if (null != _hidWriter)
            {
                // We have a mouse delta...We should send it along here.
                MouseButtons buttons = MouseButtons.None;

                if (_leftClickButton.IsSelected)
                {
                    buttons |= MouseButtons.Left;
                }
                if (_rightClickButton.IsSelected)
                {
                    buttons |= MouseButtons.Right;
                }

                if (e.IsScroll)
                {
                    _hidWriter.SendScroll(e.DX, e.DY);
                }
                else
                {
                    _hidWriter.SendMouseData(buttons, e.DX, e.DY);
                }


            }
        }
    }
}
