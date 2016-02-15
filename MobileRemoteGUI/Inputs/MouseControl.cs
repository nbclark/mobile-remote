using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal class MouseControl : InputControl, IInputControl
    {
        private ImageButton _actionButton;
        private MeterButton _meterButtonRight;
        private MeterButton _meterButtonLeft;
        private MeterButton _meterButtonTop;
        private MeterButton _meterButtonBottom;
        private BluetoothHidWriter _hidWriter;
        private Timer timer1;
        private IGSensor _gSensor;
        private bool _gSensorEnabled = false;

        public MouseControl()
        {
            InitializeComponent();

            Bitmap arrow0 = Properties.Resources.arrow_0;
            Bitmap arrow1 = Properties.Resources.arrow_1;
            Bitmap arrow2 = Properties.Resources.arrow_2;
            Bitmap arrow3 = Properties.Resources.arrow_3;

            _meterButtonTop.MeterRotation = MeterButton.Rotation.ROTATE_270;
            _meterButtonBottom.MeterRotation = MeterButton.Rotation.ROTATE_90;
            _meterButtonLeft.MeterRotation = MeterButton.Rotation.ROTATE_180;
            _meterButtonRight.MeterRotation = MeterButton.Rotation.ROTATE_0;

            _meterButtonTop.Initialize(arrow0, arrow1, arrow2, arrow3);
            _meterButtonBottom.Initialize(arrow0, arrow1, arrow2, arrow3);
            _meterButtonLeft.Initialize(arrow0, arrow1, arrow2, arrow3);
            _meterButtonRight.Initialize(arrow0, arrow1, arrow2, arrow3);

            _actionButton.TransparentBackground = true;
            _actionButton.SelectedImage = Properties.Resources.orb_on;
            _actionButton.DownImage = _actionButton.SelectedImage;
            _actionButton.Image = Properties.Resources.orb_off;

            _meterButtonTop.MeterAmount = 0;
            _meterButtonBottom.MeterAmount = 0;
            _meterButtonLeft.MeterAmount = 0;
            _meterButtonRight.MeterAmount = 0;

            _meterButtonTop.BackColor = this.BackColor;
            _meterButtonBottom.BackColor = this.BackColor;
            _meterButtonLeft.BackColor = this.BackColor;
            _meterButtonRight.BackColor = this.BackColor;
            _actionButton.BackColor = this.BackColor;
            _actionButton.TabStop = false;
            _actionButton.CanHold = false;
            _actionButton.ButtonClick += new EventHandler(_actionButton_ButtonClick);
            _actionButton.ButtonDoubleClick += new EventHandler(_actionButton_ButtonDoubleClick);
            _actionButton.ButtonDown += new EventHandler(_actionButton_ButtonDown);

            _gSensor = new GSensor();
            _gSensorEnabled = _gSensor.IsEnabled;
        }

        public override void ShowSettings()
        {
            using (MouseOptions selector = new MouseOptions(_gSensor, _gSensorEnabled))
            {
                if (DialogResult.OK == selector.ShowDialog(MobileRemoteUI.Instance))
                {
                    timer1.Enabled = _gSensorEnabled = selector.AccelerometerEnabled;
                }
            }
        }

        public bool RequiresTouchscreen
        {
            get { return false; }
        }

        void _actionButton_ButtonDown(object sender, EventArgs e)
        {
            LeftMouseDown();
        }

        void _actionButton_ButtonClick(object sender, EventArgs e)
        {
            LeftMouseUp();
        }

        void _actionButton_ButtonDoubleClick(object sender, EventArgs e)
        {
            LeftMouseDown();
            LeftMouseUp();
        }

        private bool HasFocus()
        {
            if (this.Focused)
            {
                return true;
            }
            else
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl.Focused)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (_gSensor.IsEnabled)
            {
                timer1.Enabled = _gSensorEnabled;
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (!this.HasFocus())
            {
                timer1.Enabled = false;
            }
        }

        public void Shutdown()
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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            ResizeAroundCenter(_actionButton);
        }

        private double _threshHold = 1; //2.0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.HasFocus())
            {
                timer1.Enabled = false;
            }
            AccelerationVector vector = _gSensor.GetAccelerationVector();

            double maxValue = 9.8 - _threshHold;

            double xValue = Math.Abs(vector.X);

            Dictionary<MeterButton[], double> pairs = new Dictionary<MeterButton[],double>();
            pairs.Add(new MeterButton[] { _meterButtonLeft, _meterButtonRight }, vector.X);
            pairs.Add(new MeterButton[] { _meterButtonTop, _meterButtonBottom }, vector.Y);

            foreach (MeterButton[] pair in pairs.Keys)
            {
                double vectorValue = pairs[pair];
                double scalarValue = Math.Abs(vectorValue);

                if (scalarValue >= _threshHold)
                {
                    MeterButton button = (vectorValue < 0) ? pair[0] : pair[1];
                    MeterButton offButton = (vectorValue < 0) ? pair[1] : pair[0];

                    scalarValue = scalarValue - _threshHold;

                    int count = button.MeterCount;
                    double interval = maxValue / count;

                    double testValue = 0;
                    int index = 0;

                    for (int i = 0; i < count; ++i)
                    {
                        if (scalarValue > testValue)
                        {
                            index = i;
                            testValue += interval;
                        }
                        else
                        {
                            break;
                        }
                    }

                    button.MeterAmount = index;
                    offButton.MeterAmount = 0;
                }
                else
                {
                    pair[0].MeterAmount = pair[1].MeterAmount = 0;
                }
            }

            short dx = 0, dy = 0;
            int xMeter = 0, yMeter = 0;

            if (_meterButtonBottom.MeterAmount > 0)
            {
                dy = 1;
                yMeter = _meterButtonBottom.MeterAmount;
            }
            if (_meterButtonTop.MeterAmount > 0)
            {
                dy = -1;
                yMeter = _meterButtonTop.MeterAmount;
            }
            if (_meterButtonRight.MeterAmount > 0)
            {
                dx = 1;
                xMeter = _meterButtonRight.MeterAmount;
            }
            if (_meterButtonLeft.MeterAmount > 0)
            {
                dx = -1;
                xMeter = _meterButtonLeft.MeterAmount;
            }

            if (dx != 0 || dy != 0)
            {
                MoveMouse(dx, dy, xMeter, yMeter);
            }
        }

        private short _pixelCount = 5;

        private void MoveMouse(short dx, short dy, int xMeterAmount, int yMeterAmount)
        {
            if (null != _hidWriter)
            {
                // move the mouse faster the more you tilt it
                short finalDx = (short)(_pixelCount * dx * Math.Pow(xMeterAmount, 2));
                short finalDy = (short)(_pixelCount * dy * Math.Pow(yMeterAmount, 2));

                _hidWriter.SendMouseData(_actionButton.IsSelected ? MouseButtons.Left : MouseButtons.None, (int)finalDx, (int)finalDy);
            }
        }

        private void LeftMouseDown()
        {
            if (null != _hidWriter)
            {
                _hidWriter.SendMouseData(MouseButtons.Left, 0, 0);
            }
        }

        private void LeftMouseUp()
        {
            if (null != _hidWriter)
            {
                _hidWriter.SendMouseData(MouseButtons.None, 0, 0);
            }
        }

        private int _up = 0, _down = 0, _left = 0, _right = 0;

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
            timer1.Enabled = false;

            switch (e.KeyData)
            {
                case Keys.Enter:
                    {
                        e.Handled = true;
                        timer1.Enabled = _gSensorEnabled && this.HasFocus();
                        _actionButton.IsSelected = true;
                        LeftMouseDown();
                    }
                    break;
                case Keys.Up:
                    {
                        e.Handled = true;
                        _up++;
                        _meterButtonTop.MeterAmount = 1 + (_up / 3);
                        MoveMouse(0, (short)-1, 0, _meterButtonTop.MeterAmount);
                    }
                    break;
                case Keys.Down:
                    {
                        e.Handled = true;
                        _down++;
                        _meterButtonBottom.MeterAmount = 1 + (_down / 3);
                        MoveMouse(0, 1, 0, _meterButtonBottom.MeterAmount);
                    }
                    break;
                case Keys.Left:
                    {
                        e.Handled = true;
                        _left++;
                        _meterButtonLeft.MeterAmount = 1 + (_left / 3);
                        MoveMouse(-1, 0, _meterButtonLeft.MeterAmount, 0);
                    }
                    break;
                case Keys.Right:
                    {
                        e.Handled = true;
                        _right++;
                        _meterButtonRight.MeterAmount = 1 + (_right / 3);
                        MoveMouse(1, 0, _meterButtonRight.MeterAmount, 0);
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
                        e.Handled = true;
                        _actionButton.IsSelected = false;
                        LeftMouseUp();
                    }
                    break;
                case Keys.Up:
                    {
                        e.Handled = true;
                        _up = 0;
                        _meterButtonTop.MeterAmount = 0;
                    }
                    break;
                case Keys.Down:
                    {
                        e.Handled = true;
                        _down = 0;
                        _meterButtonBottom.MeterAmount = 0;
                    }
                    break;
                case Keys.Left:
                    {
                        e.Handled = true;
                        _left = 0;
                        _meterButtonLeft.MeterAmount = 0;
                    }
                    break;
                case Keys.Right:
                    {
                        e.Handled = true;
                        _right = 0;
                        _meterButtonRight.MeterAmount = 0;
                    }
                    break;
            }
            timer1.Enabled = _gSensorEnabled && this.HasFocus();

            return e.Handled;
        }

        private void InitializeComponent()
        {
            this._meterButtonRight = new MeterButton();
            this._actionButton = new ImageButton();
            this._meterButtonLeft = new MeterButton();
            this._meterButtonTop = new MeterButton();
            this._meterButtonBottom = new MeterButton();
            this.timer1 = new System.Windows.Forms.Timer();
            this.SuspendLayout();
            // 
            // _meterButtonRight
            // 
            this._meterButtonRight.Location = new System.Drawing.Point(332, 230);
            this._meterButtonRight.Name = "_meterButtonRight";
            this._meterButtonRight.Size = new System.Drawing.Size(115, 100);
            // 
            // _actionButton
            // 
            this._actionButton.Location = new System.Drawing.Point(180, 220);
            this._actionButton.Name = "_actionButton";
            this._actionButton.Size = new System.Drawing.Size(120, 120);
            this._actionButton.TabIndex = 0;
            // 
            // _meterButtonLeft
            // 
            this._meterButtonLeft.Location = new System.Drawing.Point(32, 230);
            this._meterButtonLeft.Name = "_meterButtonLeft";
            this._meterButtonLeft.Size = new System.Drawing.Size(115, 100);
            // 
            // _meterButtonTop
            // 
            this._meterButtonTop.Location = new System.Drawing.Point(190, 72);
            this._meterButtonTop.Name = "_meterButtonTop";
            this._meterButtonTop.Size = new System.Drawing.Size(100, 115);
            // 
            // _meterButtonBottom
            // 
            this._meterButtonBottom.Location = new System.Drawing.Point(190, 372);
            this._meterButtonBottom.Name = "_meterButtonBottom";
            this._meterButtonBottom.Size = new System.Drawing.Size(100, 115);
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // MouseControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this._meterButtonBottom);
            this.Controls.Add(this._meterButtonTop);
            this.Controls.Add(this._meterButtonLeft);
            this.Controls.Add(this._meterButtonRight);
            this.Controls.Add(this._actionButton);
            this.Name = "MouseControl";
            this.Size = new System.Drawing.Size(480, 560);
            this.ResumeLayout(false);

        }
    }
}