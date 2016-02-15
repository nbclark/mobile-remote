using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class PowerPointControl : InputControl, IInputControl
    {
        private ImageButton _actionButton;
        private ImageButton _meterButtonRight;
        private ImageButton _meterButtonLeft;
        private BluetoothHidWriter _hidWriter;
        private TouchControl _touchControl;
        private IGSensor _gSensor;

        public PowerPointControl()
        {
            InitializeComponent();

            _actionButton.TransparentBackground = true;
            _actionButton.SelectedImage = Properties.Resources.powerpoint;
            _actionButton.DownImage = _actionButton.SelectedImage;
            _actionButton.Image = Properties.Resources.powerpoint_off;
            _actionButton.TabStop = false;
            _actionButton.CanHold = false;
            _actionButton.ButtonClick += new EventHandler(_actionButton_ButtonClick);

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

            ResizeAroundCenter(_actionButton, false);
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
            set
            {
                _hidWriter = value;
                _touchControl.HidWriter = _hidWriter;
            }
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
                        _meterButtonLeft.IsSelected = true;
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    {
                        _meterButtonRight.IsSelected = true;
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
                        _meterButtonLeft.IsSelected = false;
                        e.Handled = true;
                    }
                    break;
                case Keys.Down:
                    {
                        _meterButtonRight.IsSelected = false;
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
            this._meterButtonRight = new MobileSRC.MobileRemote.ImageButton();
            this._actionButton = new MobileSRC.MobileRemote.ImageButton();
            this._meterButtonLeft = new MobileSRC.MobileRemote.ImageButton();
            this._touchControl = new MobileSRC.MobileRemote.TouchControl();
            this.SuspendLayout();
            // 
            // _meterButtonRight
            // 
            this._meterButtonRight.Location = new System.Drawing.Point(326, 3);
            this._meterButtonRight.Name = "_meterButtonRight";
            this._meterButtonRight.Size = new System.Drawing.Size(122, 250);
            this._meterButtonRight.TabIndex = 2;
            // 
            // _actionButton
            // 
            this._actionButton.Location = new System.Drawing.Point(165, 53);
            this._actionButton.Name = "_actionButton";
            this._actionButton.Size = new System.Drawing.Size(150, 150);
            this._actionButton.TabIndex = 1;
            // 
            // _meterButtonLeft
            // 
            this._meterButtonLeft.Location = new System.Drawing.Point(32, 3);
            this._meterButtonLeft.Name = "_meterButtonLeft";
            this._meterButtonLeft.Size = new System.Drawing.Size(122, 250);
            this._meterButtonLeft.TabIndex = 0;
            // 
            // _touchControl
            // 
            this._touchControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._touchControl.Location = new System.Drawing.Point(4, 259);
            this._touchControl.Name = "_touchControl";
            this._touchControl.Size = new System.Drawing.Size(473, 298);
            this._touchControl.TabIndex = 3;
            // 
            // PowerPointControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this._touchControl);
            this.Controls.Add(this._meterButtonLeft);
            this.Controls.Add(this._meterButtonRight);
            this.Controls.Add(this._actionButton);
            this.Name = "PowerPointControl";
            this.Size = new System.Drawing.Size(480, 560);
            this.ResumeLayout(false);

        }

        private void _actionButton_ButtonClick(object sender, EventArgs e)
        {
            _hidWriter.SendKeyPress(0, Classes.HidKeys.F5);
        }

        private void _meterButtonLeft_ButtonClick(object sender, EventArgs e)
        {
            KeyEventArgs ke = new KeyEventArgs(Keys.Left);
            MobileRemoteUI.Instance.Keyboard.HandleKeyDown(ke);
            MobileRemoteUI.Instance.Keyboard.HandleKeyUp(ke);
        }

        private void _meterButtonRight_ButtonClick(object sender, EventArgs e)
        {
            KeyEventArgs ke = new KeyEventArgs(Keys.Right);
            MobileRemoteUI.Instance.Keyboard.HandleKeyDown(ke);
            MobileRemoteUI.Instance.Keyboard.HandleKeyUp(ke);
        }
    }
}