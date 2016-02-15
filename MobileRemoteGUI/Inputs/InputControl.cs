using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal interface IInputControl
    {
        List<MenuItem> MenuItems
        {
            get;
        }

        Orientation DesiredOrientation
        {
            get;
        }

        DockStyle Dock
        {
            get;
            set;
        }

        BluetoothHidWriter HidWriter
        {
            get;
            set;
        }

        bool RequiresTouchscreen
        {
            get;
        }

        void ShowSettings();
        void SelectedClick();
        void Shutdown();
        bool HandleKeyDown(KeyEventArgs e);
        bool HandleKeyUp(KeyEventArgs e);
        bool HandleKeyPress(KeyPressEventArgs e);
    }

    internal class InputControl : UserControl
    {
        private class SizeLocationInfo
        {
            public Point Location;
            public Size Size;
            public SizeLocationInfo(Point location, Size size)
            {
                Location = location;
                Size = size;
            }
        }

        private Dictionary<Control, SizeLocationInfo> _originalLocation = new Dictionary<Control, SizeLocationInfo>();

        public virtual List<MenuItem> MenuItems
        {
            get { return null; }
        }
        public virtual void ShowSettings()
        {
            //
        }
        public virtual void SelectedClick()
        {
            //
        }
        public virtual void Shutdown()
        {
            //
        }
        public virtual bool HandleKeyDown(KeyEventArgs e)
        {
            return false;
        }
        public virtual bool HandleKeyUp(KeyEventArgs e)
        {
            return false;
        }
        public virtual bool HandleKeyPress(KeyPressEventArgs e)
        {
            return false;
        }
        private static bool _escapeNeeded = true;
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            if (Platform.IsWindowsMobileStandard)
            {
                bool bret = false;
                IntPtr hC = L2CAPAPI.ImmGetContext(this.Handle);
                // Open the IME 
                bret = L2CAPAPI.ImmSetOpenStatus(hC, true);
                // Set "multi-press" input mode

                L2CAPAPI.ImmEscape(IntPtr.Zero, hC, L2CAPAPI.IME_ESC_SET_MODE, L2CAPAPI.IM_SPELL);
            }
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            if (Platform.IsWindowsMobileStandard)
            {
                IntPtr hC = L2CAPAPI.ImmGetContext(this.Handle);
                L2CAPAPI.ImmEscape(IntPtr.Zero, hC, L2CAPAPI.IME_ESC_RETAIN_MODE_ICON, new IntPtr(1));
                //L2CAPAPI.ImmSetOpenStatus(this.Handle, false);
            }
        }


        protected void ResizeAroundCenter(Control centerControl)
        {
            ResizeAroundCenter(centerControl, true);
        }

        protected void ResizeAroundCenter(Control centerControl, bool centerVertically)
        {
            SizeF autoScaleFactor = new SizeF((this.Width / 480.0f) / (centerControl.Width / 120f), (this.Height / 560.0f) / (centerControl.Height / 120f));
            float scaleFactor = Math.Max(Math.Min(autoScaleFactor.Width, autoScaleFactor.Height), 0.5f);

            if (autoScaleFactor.Width != 1 || autoScaleFactor.Height != 1)
            {
                // we need to first
                int maxY = 0;
                int minY = Int32.MaxValue;
                int maxX = 0;
                int minX = Int32.MaxValue;

                foreach (Control control in this.Controls)
                {
                    if (!_originalLocation.ContainsKey(control))
                    {
                        _originalLocation.Add(control, new SizeLocationInfo(control.Location, control.Size));
                    }
                    else
                    {
                        control.Location = _originalLocation[control].Location;
                        control.Size = _originalLocation[control].Size;
                    }
                    maxY = Math.Max(control.Bottom, maxY);
                    minY = Math.Min(control.Top, minY);

                    maxX = Math.Max(control.Right, maxX);
                    minX = Math.Min(control.Left, minX);
                }

                bool needsScaling = false;
                int xShift = 0;
                int yShift = 0;
                if ((maxX - minX) > this.Width || (maxY - minY) > this.Height)
                {
                    needsScaling = true;
                }
                else
                {
                    xShift = minX - (this.Width - (maxX - minX)) / 2;
                    yShift = minY - (this.Height - (maxY - minY)) / 2;
                }

                foreach (Control control in this.Controls)
                {
                    if (needsScaling)
                    {
                        int xMidDiff = (int)((this.Width / 2 - control.Left + control.Width / 2) * scaleFactor);
                        control.Height = (int)Math.Round(control.Height * scaleFactor);
                        control.Width = (int)Math.Round(control.Width * scaleFactor);

                        // keep the same distance from the center
                        if (centerVertically)
                        {
                            control.Top = (int)Math.Round(control.Top * scaleFactor);
                        }
                        control.Left = (int)Math.Round(control.Left * scaleFactor);
                    }
                }

                int yOffset = (this.ClientRectangle.Height / 2) - (centerControl.Top + centerControl.Bottom) / 2;
                int xOffset = (this.ClientRectangle.Width / 2) - (centerControl.Left + centerControl.Right) / 2;

                foreach (Control control in this.Controls)
                {
                    if (centerVertically)
                    {
                        control.Top += yOffset;
                    }
                    control.Left += xOffset;
                }
            }
        }
    }

    internal class ConnectButton : PushButton
    {
        public event EventHandler ConnectRequested, DisconnectRequested;

        private BluetoothHidWriter _hidWriter = null;
        private Bitmap _connectedImage, _connectedImageDown, _disconnectedImage, _disconnectedImageDown;

        public ConnectButton()
        {
            this.AllowSelect = false;
        }

        public bool IsConnected
        {
            get
            {
                if (null == _hidWriter)
                {
                    return false;
                }
                else
                {
                    return _hidWriter.IsConnected;
                }
            }
        }

        public BluetoothHidWriter HidWriter
        {
            get { return _hidWriter; }
            set
            {
                if (null != _hidWriter)
                {
                    _hidWriter.Connected -= new EventHandler(_hidWriter_Connected);
                    _hidWriter.Disconnected -= new EventHandler(_hidWriter_Disconnected);
                }

                _hidWriter = value;

                if (null != _hidWriter)
                {
                    _hidWriter.Connected += new EventHandler(_hidWriter_Connected);
                    _hidWriter.Disconnected += new EventHandler(_hidWriter_Disconnected);
                }
            }
        }

        delegate void VoidDelegate();

        void _hidWriter_Disconnected(object sender, EventArgs e)
        {
            this.Invoke(new VoidDelegate(Refresh));
        }

        public void _hidWriter_Connected(object sender, EventArgs e)
        {
            this.Invoke(new VoidDelegate(Refresh));
        }

        public override bool IsSelected
        {
            get
            {
                return false;
            }
            set
            {
                // do nothing
            }
        }

        protected override void OnClick(EventArgs e)
        {
            _isMouseDown = false;
            if (!this.IsConnected)
            {
                if (null != ConnectRequested)
                {
                    ConnectRequested(this, null);
                }
            }
            else
            {
                if (null != DisconnectRequested)
                {
                    DisconnectRequested(this, null);
                }
            }
            base.OnClick(e);
        }

        public override Bitmap Image
        {
            get
            {
                return (this.IsConnected) ? this.ConnectedImage : this.DisconnectedImage;
            }
            set
            {
                base.Image = value;
            }
        }

        public override Bitmap DownImage
        {
            get
            {
                return (this.IsConnected) ? this.ConnectedImageDown : this.DisconnectedImageDown;
            }
            set
            {
                base.Image = value;
            }
        }

        public Bitmap ConnectedImage
        {
            get
            {
                return _connectedImage;
            }
            set
            {
                _connectedImage = value;
            }
        }

        public Bitmap ConnectedImageDown
        {
            get
            {
                return _connectedImageDown;
            }
            set
            {
                _connectedImageDown = value;
            }
        }

        public Bitmap DisconnectedImage
        {
            get
            {
                return _disconnectedImage;
            }
            set
            {
                _disconnectedImage = value;
            }
        }

        public Bitmap DisconnectedImageDown
        {
            get
            {
                return _disconnectedImageDown;
            }
            set
            {
                _disconnectedImageDown = value;
            }
        }
    }

    internal class MeterButton : PictureBox
    {
        public enum Rotation
        {
            ROTATE_0 = 0,
            ROTATE_90 = 90,
            ROTATE_180 = 180,
            ROTATE_270 = 270
        }

        private Bitmap[] _meteredImages;
        private Rotation _rotation = Rotation.ROTATE_0;
        private int _meterAmount = 0;
        private Color _backColor = Color.White;

        public MeterButton(Rotation rotation, params Bitmap[] images)
        {
            Initialize(rotation, images);
        }

        public MeterButton()
        {
            //
        }

        public void Initialize(Rotation rotation, params Bitmap[] images)
        {
            _rotation = rotation;
            _meteredImages = new Bitmap[images.Length];
            for (int i = 0; i < images.Length; ++i)
            {
                if (rotation == Rotation.ROTATE_0)
                {
                    _meteredImages[i] = images[i];
                }
                else if (images[i].Width == images[i].Height)
                {
                    _meteredImages[i] = Imaging.RotateSquareImage((int)rotation, images[i]);
                }
            }
        }

        public void Initialize(params Bitmap[] images)
        {
            _meteredImages = new Bitmap[images.Length];
            for (int i = 0; i < images.Length; ++i)
            {
                if (_rotation == Rotation.ROTATE_0)
                {
                    _meteredImages[i] = images[i];
                }
                else if (images[i].Width == images[i].Height)
                {
                    _meteredImages[i] = Imaging.RotateSquareImage((int)_rotation, images[i]);
                }
                else
                {
                    _meteredImages[i] = Imaging.RotateImage((int)_rotation, images[i]);
                }
            }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public Rotation MeterRotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public int MeterCount
        {
            get { return _meteredImages.Length; }
        }

        public int MeterAmount
        {
            get { return _meterAmount; }
            set
            {
                if (value != _meterAmount)
                {
                    _meterAmount = Math.Max(0, Math.Min(value, _meteredImages.Length - 1));
                    this.Refresh();
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(_backColor);
            if (_meterAmount >= 0 && _meterAmount < _meteredImages.Length)
            {
                Bitmap image = _meteredImages[_meterAmount];

                ImageAttributes attributes = new ImageAttributes();

                Color clr = image.GetPixel(0, 0);
                attributes.SetColorKey(clr, clr);

                e.Graphics.DrawImage(image, this.ClientRectangle, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
            }
        }
    }

    internal class ImageButton : PushButton
    {
        private bool _willSelect = false, _transparentBackground = false, _canHold = true;
        private Color _backColor = Color.White;
        private Timer _timer = new Timer();

        public event EventHandler ButtonClick, ButtonDoubleClick, ButtonHold, ButtonUp, ButtonDown;

        public ImageButton()
        {
            _timer.Interval = 1500;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        public override bool IsSelected
        {
            get
            {
                return base.IsSelected;
            }
            set
            {
                if (base.IsSelected && !value)
                {
                    ButtonClick(this, null);
                }
                base.IsSelected = value;
            }
        }

        public bool TransparentBackground
        {
            get { return _transparentBackground; }
            set { _transparentBackground = value; }
        }

        public bool CanHold
        {
            get { return _canHold; }
            set { _canHold = value; }
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _willSelect = true;
            this.Refresh();
            _timer.Enabled = false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (null != ButtonDown)
            {
                ButtonDown(this, null);
            }
            _timer.Enabled = _canHold;
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            if (null != ButtonDoubleClick)
            {
                ButtonDoubleClick(this, null);
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (_willSelect && !this.IsSelected)
            {
                if (null != ButtonHold)
                {
                    ButtonHold(this, null);
                }
            }
            else if (!this.IsSelected && !_willSelect)
            {
                if (null != ButtonClick)
                {
                    ButtonClick(this, null);
                }
            }
            else if (this.IsSelected && !_willSelect)
            {
                if (null != ButtonUp)
                {
                    ButtonUp(this, null);
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.IsSelected = _willSelect && _canHold;
            _timer.Enabled = false;
            _willSelect = false;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Bitmap img = (this.IsMouseDown) ? this.DownImage : ((this.IsSelected) ? this.SelectedImage : this.Image);

            if (_willSelect)
            {
                img = this.SelectedImage;
            }

            ImageAttributes attributes = new ImageAttributes();

            using (Bitmap bmp = new Bitmap(this.Width, this.Height))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    if (this.TransparentBackground)
                    {
                        g.Clear(_backColor);

                        Color clr = img.GetPixel(0, 0);
                        attributes.SetColorKey(clr, clr);
                    }
                    g.DrawImage(img, this.ClientRectangle, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, attributes);
                }
                e.Graphics.DrawImage(bmp, 0, 0); 
            }
        }
    }

    internal class PushButton : UserControl
    {
        private Bitmap _image, _imageDown, _imageSel;
        protected bool _isMouseDown = false, _isSel = false, _isRightDown = false, _allowSelect = true;
        protected DateTime _mouseDownTime = DateTime.MaxValue;
        private string _text = string.Empty;
        private Timer _timer = new Timer();

        public event EventHandler RightClick;

        public PushButton()
        {
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _isRightDown = true;
            _timer.Enabled = false;
            if (null != RightClick)
            {
                RightClick(this, e);
            }
            _isRightDown = false;

            if (_isMouseDown)
            {
                OnClick(e);
            }
        }

        public virtual bool IsSelected
        {
            get { return _isSel; }
            set
            {
                if (_isSel != value)
                {
                    _isSel = value;
                    this.Refresh();
                }
            }
        }

        public bool IsMouseDown
        {
            get { return _isMouseDown; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public bool AllowSelect
        {
            get { return _allowSelect; }
            set { _allowSelect = value; }
        }

        public virtual Bitmap Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public virtual Bitmap SelectedImage
        {
            get
            {
                return _imageSel;
            }
            set
            {
                _imageSel = value;
            }
        }

        public virtual Bitmap DownImage
        {
            get
            {
                return  _imageDown;
            }
            set
            {
                _imageDown = value;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _timer.Enabled = true;
            _isMouseDown = true;
            PushButtonOnMouseDown(e);
        }

        protected virtual void PushButtonOnMouseDown(MouseEventArgs e)
        {
            _mouseDownTime = DateTime.Now;
            this.Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (!_isRightDown)
            {
                base.OnMouseUp(e);
                _timer.Enabled = false;
                PushButtonOnMouseUp(e);
                _isMouseDown = false;
            }
        }

        protected virtual void PushButtonOnMouseUp(MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                _isMouseDown = false;
                this.Refresh();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (!_isRightDown)
            {
                _isMouseDown = false;
                _timer.Enabled = false;

                PushButtonOnClick(e);
                base.OnClick(e);
            }
            if (!_allowSelect)
            {
                this.Refresh();
            }
        }

        protected virtual void PushButtonOnClick(EventArgs e)
        {
            if (_allowSelect)
            {
                if (Parent is ButtonPanel)
                {
                    ((ButtonPanel)Parent).ClearSelection();
                }

                this.IsSelected = true;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Image img = (this.IsMouseDown) ? this.DownImage : ((this.IsSelected) ? this.SelectedImage : this.Image);

            e.Graphics.DrawImage(img, this.ClientRectangle, new Rectangle(0, 0, img.Width, img.Height), GraphicsUnit.Pixel);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //
        }
    }

    internal class ButtonPanel : Panel
    {
        private bool _isInitialized = false;
        private bool _isVertical = false;
        private int _offset = 0;

        public ButtonPanel()
        {
            //
        }

        public void ClearSelection()
        {
            foreach (Control button in this.Controls)
            {
                if (button is PushButton)
                {
                    ((PushButton)button).IsSelected = false;
                }
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (this.Controls.Count > 0)
            {
                Size size = this.Controls[0].Size;

                if (this.Height > this.Width)
                {
                    if (!_isVertical || !_isInitialized)
                    {
                        // Move around here
                        _isVertical = true;

                        int x = 0, y = _offset;
                        for (int i = this.Controls.Count-1; i >= 0; --i)
                        {
                            this.Controls[i].Top = y;
                            this.Controls[i].Left = x;
                            this.Controls[i].Height = this.Width;

                            y += _offset + size.Height;
                        }
                    }
                }
                else
                {
                    if (_isVertical || !_isInitialized)
                    {
                        // Move around here
                        _isVertical = false;

                        int x = _offset, y = 0;
                        for (int i = this.Controls.Count - 1; i >= 0; --i)
                        {
                            this.Controls[i].Top = y;
                            this.Controls[i].Left = x;
                            this.Controls[i].Width = this.Height;

                            x += _offset + size.Width;
                        }
                    }
                }
            }
        }
    }
}
