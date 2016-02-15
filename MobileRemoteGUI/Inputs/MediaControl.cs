using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class MediaControl : InputControl, IInputControl
    {
        private ImageButton _actionButton;
        private ImageButton _meterButtonRight;
        private ImageButton _meterButtonLeft;
        private ImageButton _meterButtonTop;
        private ImageButton _meterButtonBottom;
        private BluetoothHidWriter _hidWriter;
        private IGSensor _gSensor;

        public MediaControl()
        {
            InitializeComponent();

            _actionButton.TransparentBackground = true;
            _actionButton.SelectedImage = Properties.Resources.mediaorb;
            _actionButton.DownImage = _actionButton.SelectedImage;
            _actionButton.Image = Properties.Resources.mediaorb_off;
            _actionButton.TabStop = false;
            _actionButton.CanHold = false;
            _actionButton.ButtonClick += new EventHandler(_actionButton_ButtonClick);

            _meterButtonTop.TransparentBackground = true;
            _meterButtonTop.SelectedImage = Properties.Resources.mediacircle_up;
            _meterButtonTop.DownImage = _meterButtonTop.SelectedImage;
            _meterButtonTop.Image = Properties.Resources.mediacircle_up_off;
            _meterButtonTop.TabStop = false;
            _meterButtonTop.CanHold = false;
            _meterButtonTop.ButtonClick += new EventHandler(_meterButtonTop_ButtonClick);

            _meterButtonBottom.TransparentBackground = true;
            _meterButtonBottom.SelectedImage = Properties.Resources.mediacircle_down;
            _meterButtonBottom.DownImage = _meterButtonBottom.SelectedImage;
            _meterButtonBottom.Image = Properties.Resources.mediacircle_down_off;
            _meterButtonBottom.TabStop = false;
            _meterButtonBottom.CanHold = false;
            _meterButtonBottom.ButtonClick += new EventHandler(_meterButtonBottom_ButtonClick);

            _meterButtonLeft.TransparentBackground = true;
            _meterButtonLeft.SelectedImage = Properties.Resources.mediacircle_prev;
            _meterButtonLeft.DownImage = _meterButtonLeft.SelectedImage;
            _meterButtonLeft.Image = Properties.Resources.mediacircle_prev_off;
            _meterButtonLeft.TabStop = false;
            _meterButtonLeft.CanHold = false;
            _meterButtonLeft.ButtonClick += new EventHandler(_meterButtonLeft_ButtonClick);

            _meterButtonRight.TransparentBackground = true;
            _meterButtonRight.SelectedImage = Properties.Resources.mediacircle_next;
            _meterButtonRight.DownImage = _meterButtonRight.SelectedImage;
            _meterButtonRight.Image = Properties.Resources.mediacircle_next_off;
            _meterButtonRight.TabStop = false;
            _meterButtonRight.CanHold = false;
            _meterButtonRight.ButtonClick += new EventHandler(_meterButtonRight_ButtonClick);

            _actionButton.BackColor = this.BackColor;
            _meterButtonTop.BackColor = this.BackColor;
            _meterButtonBottom.BackColor = this.BackColor;
            _meterButtonLeft.BackColor = this.BackColor;
            _meterButtonRight.BackColor = this.BackColor;

            _gSensor = new GSensor();
        }

        public bool RequiresTouchscreen
        {
            get { return false; }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResizeAroundCenter(_actionButton);
        }

        public override void Shutdown()
        {
            //
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            _gSensor.Dispose();
        }

        public Orientation DesiredOrientation
        {
            get { return Orientation.Vertical; }
        }

        public BluetoothHidWriter HidWriter
        {
            get { return _hidWriter; }
            set { _hidWriter = value; }
        }

        public override bool HandleKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                return true;
            }
            return false;
        }

        public override bool HandleKeyDown(KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    {
                        _actionButton.IsSelected = true;
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    {
                        _meterButtonTop.IsSelected = true;
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    {
                        _meterButtonBottom.IsSelected = true;
                        e.Handled = true;
                    }
                    break;
                case Keys.Left:
                    {
                        _meterButtonLeft.IsSelected = true;
                        e.Handled = true;
                    }
                    break;
                case Keys.Right:
                    {
                        _meterButtonRight.IsSelected = true;
                        e.Handled = true;
                    }
                    break;
            }
            return e.Handled;
        }

        public override bool HandleKeyUp(KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Enter:
                    {
                        _actionButton.IsSelected = false;
                        e.Handled = true;
                    }
                    break;
                case Keys.Up:
                    {
                        _meterButtonTop.IsSelected = false;
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    {
                        _meterButtonBottom.IsSelected = false;
                        e.Handled = true;
                    }
                    break;
                case Keys.Left:
                    {
                        _meterButtonLeft.IsSelected = false;
                        e.Handled = true;
                    }
                    break;
                case Keys.Right:
                    {
                        _meterButtonRight.IsSelected = false;
                        e.Handled = true;
                    }
                    break;
            }
            return e.Handled;
        }

        private void InitializeComponent()
        {
            this._meterButtonRight = new ImageButton();
            this._actionButton = new ImageButton();
            this._meterButtonLeft = new ImageButton();
            this._meterButtonTop = new ImageButton();
            this._meterButtonBottom = new ImageButton();
            this.SuspendLayout();
            // 
            // _meterButtonRight
            // 
            this._meterButtonRight.Location = new System.Drawing.Point(326, 155);
            this._meterButtonRight.Name = "_meterButtonRight";
            this._meterButtonRight.Size = new System.Drawing.Size(122, 250);
            this._meterButtonRight.TabIndex = 3;
            // 
            // _actionButton
            // 
            this._actionButton.Location = new System.Drawing.Point(165, 205);
            this._actionButton.Name = "_actionButton";
            this._actionButton.Size = new System.Drawing.Size(150, 150);
            this._actionButton.TabIndex = 0;
            // 
            // _meterButtonLeft
            // 
            this._meterButtonLeft.Location = new System.Drawing.Point(32, 155);
            this._meterButtonLeft.Name = "_meterButtonLeft";
            this._meterButtonLeft.Size = new System.Drawing.Size(122, 250);
            this._meterButtonLeft.TabIndex = 2;
            // 
            // _meterButtonTop
            // 
            this._meterButtonTop.Location = new System.Drawing.Point(115, 72);
            this._meterButtonTop.Name = "_meterButtonTop";
            this._meterButtonTop.Size = new System.Drawing.Size(250, 122);
            this._meterButtonTop.TabIndex = 1;
            // 
            // _meterButtonBottom
            // 
            this._meterButtonBottom.Location = new System.Drawing.Point(115, 366);
            this._meterButtonBottom.Name = "_meterButtonBottom";
            this._meterButtonBottom.Size = new System.Drawing.Size(250, 122);
            this._meterButtonBottom.TabIndex = 0;
            // 
            // MediaControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this._meterButtonBottom);
            this.Controls.Add(this._meterButtonTop);
            this.Controls.Add(this._meterButtonLeft);
            this.Controls.Add(this._meterButtonRight);
            this.Controls.Add(this._actionButton);
            this.Name = "MediaControl";
            this.Size = new System.Drawing.Size(480, 560);
            this.ResumeLayout(false);

        }

        private void _actionButton_ButtonClick(object sender, EventArgs e)
        {
            _hidWriter.SendMediaKey(BluetoothHidWriter.MediaKeys.Play);
        }

        private void _meterButtonLeft_ButtonClick(object sender, EventArgs e)
        {
            _hidWriter.SendMediaKey(BluetoothHidWriter.MediaKeys.Previous);
        }

        private void _meterButtonRight_ButtonClick(object sender, EventArgs e)
        {
            _hidWriter.SendMediaKey(BluetoothHidWriter.MediaKeys.Next);
        }

        private void _meterButtonTop_ButtonClick(object sender, EventArgs e)
        {
            _hidWriter.SendMediaKey(BluetoothHidWriter.MediaKeys.VolumeUp);
        }

        private void _meterButtonBottom_ButtonClick(object sender, EventArgs e)
        {
            _hidWriter.SendMediaKey(BluetoothHidWriter.MediaKeys.VolumeDown);
        }
    }
}