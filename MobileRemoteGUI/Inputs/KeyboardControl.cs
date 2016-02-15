using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    using Classes;
    internal class KeyboardControl : InputControl, IInputControl
    {
        private BluetoothHidWriter _hidWriter;
        private string _currentLayout = string.Empty;

        internal class KeyButtonCollection : IXmlSerializable
        {
            private KeyButton[][] _keys;

            public KeyButtonCollection(KeyButton[][] keys)
            {
                _keys = keys;
            }

            public KeyButton[][] Keys
            {
                get { return _keys; }
            }

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                throw new NotImplementedException();
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                reader.MoveToContent();

                int row = 0;
                if (reader.IsStartElement("KeyButtonCollection") || reader.ReadToDescendant("KeyButtonCollection"))
                {
                    int rows = Convert.ToInt32(reader.GetAttribute("rows"));
                    _keys = new KeyButton[rows][];

                    if (reader.ReadToDescendant("KeyButtonRow"))
                    {
                        do
                        {
                            int col = 0;
                            int cols = Convert.ToInt32(reader.GetAttribute("cols"));
                            _keys[row] = new KeyButton[cols];

                            if (reader.ReadToDescendant("KeyButton"))
                            {
                                do
                                {
                                    _keys[row][col] = new KeyButton();
                                    _keys[row][col].ReadXml(reader);
                                    col++;
                                } while (reader.ReadToNextSibling("KeyButton"));
                            }
                            row++;
                        } while (reader.ReadToNextSibling("KeyButtonRow"));
                    }
                }
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                writer.WriteStartElement("KeyButtonCollection");
                writer.WriteAttributeString("rows", _keys.GetLength(0).ToString());

                for (int i = 0; i < _keys.GetLength(0); ++i)
                {
                    writer.WriteStartElement("KeyButtonRow");
                    writer.WriteAttributeString("cols", _keys[i].GetLength(0).ToString());
                    for (int j = 0; j < _keys[i].GetLength(0); ++j)
                    {
                        writer.WriteStartElement("KeyButton");
                        writer.WriteAttributeString("row", i.ToString());
                        writer.WriteAttributeString("col", j.ToString());
                        _keys[i][j].WriteXml(writer);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            #endregion
        }

        internal class KeyButton : IXmlSerializable
        {
            public string Key, ShiftKey;
            public short KeyWidth;
            public int Col;
            public KeyButtonType Type;
            public Bitmap DownImage, DownShiftImage;
            public Bitmap Icon;
            public Rectangle Bounds;
            public HidKeys HidKey;
            public bool IsChar = false;
            private string _iconType = string.Empty, _iconName = string.Empty;

            public KeyButton()
            {
            }
            
            public KeyButton(KeyButtonType type, string key, string shiftKey, HidKeys hidKey, short keyWidth, Type bitmapType, string propertyName)
                : this(type, key, shiftKey, hidKey, keyWidth)
            {
                if (null == bitmapType)
                {
                    bitmapType = typeof(Properties.Resources);
                }
                _iconType = bitmapType.AssemblyQualifiedName;
                _iconName = propertyName;

                Initialize();
            }

            public KeyButton(string key, string shiftKey, short keyWidth)
                : this(KeyButtonType.Tap, key, shiftKey, HidKeys.None, keyWidth)
            {
                this.IsChar = true;
                this.HidKey = HidCodes.GetHidKey(key[0]);
            }

            public KeyButton(KeyButtonType type, string key, string shiftKey, HidKeys hidKey, short keyWidth)
            {
                Type = type;
                Key = key;
                ShiftKey = shiftKey;
                KeyWidth = keyWidth;
                HidKey = hidKey;
            }

            private void Initialize()
            {
                if (!string.IsNullOrEmpty(_iconType) && !string.IsNullOrEmpty(_iconName))
                {
                    try
                    {
                        Icon = (Bitmap)System.Type.GetType(_iconType).GetProperty(_iconName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic).GetValue(null, null);
                    }
                    catch
                    {
                    }
                }
                else if (!string.IsNullOrEmpty(_iconName))
                {
                    Icon = Inputs.CustomInputControl.GetBitmap(Path.Combine(Path.Combine(Utils.GetWorkingDirectory(), "KeyboardLayouts"), _iconName.Replace("{res}", Platform.IsVGA ? "vga" : "qvga")));
                }
            }

            public bool IsHidden
            {
                get { return (this.Type & KeyButtonType.Hidden) != 0; }
            }

            public string Name
            {
                get { return _iconName; }
            }

            #region IXmlSerializable Members

            public System.Xml.Schema.XmlSchema GetSchema()
            {
                throw new NotImplementedException();
            }

            public void ReadXml(System.Xml.XmlReader reader)
            {
                //key, shiftkey, keywidth, scancode, type
                this.Type = (KeyButtonType)Enum.Parse(typeof(KeyButtonType), reader.GetAttribute("Type"), true);
                this.Key = reader.GetAttribute("Key");
                this.ShiftKey = reader.GetAttribute("ShiftKey");
                this.KeyWidth = short.Parse(reader.GetAttribute("KeyWidth"));
                this.IsChar = Convert.ToBoolean(reader.GetAttribute("IsChar"));
                this.HidKey = (HidKeys)Enum.Parse(typeof(HidKeys), reader.GetAttribute("HidKey"), true);
                this._iconName = reader.GetAttribute("IconName");
                this._iconType = reader.GetAttribute("IconType");

                Initialize();
            }

            public void WriteXml(System.Xml.XmlWriter writer)
            {
                writer.WriteAttributeString("Type", Type.ToString());
                writer.WriteAttributeString("Key", Key.ToString());
                writer.WriteAttributeString("ShiftKey", ShiftKey.ToString());
                writer.WriteAttributeString("KeyWidth", KeyWidth.ToString());
                writer.WriteAttributeString("HidKey", HidKey.ToString());
                writer.WriteAttributeString("IsChar", IsChar.ToString());
                writer.WriteAttributeString("IconName", _iconName);
                writer.WriteAttributeString("IconType", _iconType);
            }

            #endregion
        }

        internal enum KeyButtonType : uint
        {
            Tap = 1,
            Hold = 2,
            Shift = KeyButtonType.Tap | KeyButtonType.Hold | 4,
            Alternate = KeyButtonType.Tap | KeyButtonType.Hold | 8,
            Control = KeyButtonType.Tap | KeyButtonType.Hold | 16,
            Windows = KeyButtonType.Tap | KeyButtonType.Hold | 32,
            Input = 64,
            Hidden = 128
        }

        private KeyButton[][] _keys;
        private Bitmap _keyImage, _keyShiftImage, _previousInputImage;
        private Font _shiftFont;
        private Timer _timer = new Timer();
        private Dictionary<Point, KeyButton> _keyButtonHash;
        private bool _tickRefresh = false;
        private Bitmap _key, _keyDown, _keySet;
        //private Dictionary<uint, KeyButton> _charToButton = new Dictionary<uint, KeyButton>();
        //private Dictionary<uint, KeyButton> _scanToButton = new Dictionary<uint, KeyButton>();
        private int _fullKeyHeight, _fullKeyWidth, _keySpacing, _keyHeight, _keyWidth, _startX, _startY;
        private Rectangle _eraseRegion = new Rectangle(0, 0, 0, 0);
        private KeyButton _activeButton = null, _downKey = null;
        private int _row, _col;
        private List<KeyButton> _setButtons = new List<KeyButton>();
        private List<KeyButton> _unsetButtons = new List<KeyButton>();
        private StringBuilder _enteredKeys = new StringBuilder();
        private bool _isShiftSet = false, _isAltSet = false, _isCtrlSet = false, _isWinSet = false;
        private KeyboardRotation _rotation = KeyboardRotation.ROT_0;
        private List<MenuItem> _menuItems = new List<MenuItem>();

        // doublekeymode will have 2 keys per button and double-tapping the same button will send the 2nd key

        public override void SelectedClick()
        {
            InputSelector.Enabled = !InputSelector.Enabled;
            IntPtr hwndSip = Win32.FindWindow("SipWndClass", "");
            IntPtr hwndSip2 = Win32.FindWindow("MS_SIPBUTTON", "MS_SIPBUTTON");

            Rectangle rect = InputSelector.Bounds;
            int offset = rect.Bottom - (this.Bottom);

            Win32.SetWindowPos(hwndSip, IntPtr.Zero, rect.Left, rect.Top - offset, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
            Win32.ShowWindow(hwndSip2, Win32.WindowShowStyle.Hide);
        }

        public override void ShowSettings()
        {
            InputSelector.Enabled = false;
            using (SipSelector selector = new SipSelector())
            {
                DialogResult dialogResult = selector.ShowDialog(MobileRemoteUI.Instance);
                if (DialogResult.OK == dialogResult)
                {
                    if (null != selector.SelectedMethod)
                    {
                        InputSelector.CurrentInputMethod = selector.SelectedMethod;
                    }
                    if (_currentLayout != selector.KeyboardLayout)
                    {
                        LoadLayout(selector.KeyboardLayout);
                        Properties.Settings.Default.Keyboard_DefaultLayout = selector.KeyboardLayout;
                    }
                    Properties.Settings.Default.Keyboard_Rotation = selector.Rotation;
                    GenerateKeyboard(selector.Rotation);

                    this.Refresh();
                    Properties.Settings.Default.Save();
                }
            }
        }

        public override List<MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
        }

        void KeyboardControl_GotFocus(object sender, EventArgs e)
        {
            //if (Platform.IsWindowsMobileStandard)
            //{
            //    uint ret = 0;
            //    bool bret = false;
            //    IntPtr hC = L2CAPAPI.ImmGetContext(MobileRemoteUI.Instance.Handle);
            //    // Open the IME 
            //    bret = L2CAPAPI.ImmSetOpenStatus(hC, true);
            //    // Set "multi-press" input mode
            //    ret = L2CAPAPI.ImmEscape(IntPtr.Zero, hC, L2CAPAPI.IME_ESC_SET_MODE, L2CAPAPI.IM_SPELL);
            //}
        }

        void KeyboardControl_LostFocus(object sender, EventArgs e)
        {
            if (Platform.IsWindowsMobileStandard)
            {
                //L2CAPAPI.ImmSetOpenStatus(this.Handle, false);
            }
            else
            {
                InputSelector.Enabled = false;
            }
        }

        private const int ScanCodeUp = 103;
        private const int ScanCodeDown = 108;
        private const int ScanCodeLeft = 105;
        private const int ScanCodeRight = 106;
        private const int ScanCodeEscape = 1;
        private const int ScanCodeDelete = 111;

        public KeyboardControl()
        {
            this.GotFocus += new EventHandler(KeyboardControl_GotFocus);
            this.LostFocus += new EventHandler(KeyboardControl_LostFocus);

            _key = Properties.Resources.key;
            _keyDown = Properties.Resources.keydown;
            _keySet = Properties.Resources.keyset;

            /*
            _keys = new KeyButton[7][];

            _keys[0] = new KeyButton[]
            {
                new KeyButton(KeyButtonType.Tap, "tab", "tab", HidKeys.Tab, 1, typeof(Properties.Resources), "tab"),
                new KeyButton(KeyButtonType.Tap, "escape", "escape", HidKeys.Escape, 1, typeof(Properties.Resources), "escape"),
                new KeyButton(KeyButtonType.Tap | KeyButtonType.Hidden, "up", "up", HidKeys.Up, 1),
                new KeyButton(KeyButtonType.Tap | KeyButtonType.Hidden, "down", "down", HidKeys.Down, 1),
                new KeyButton(KeyButtonType.Tap | KeyButtonType.Hidden, "left", "left", HidKeys.Left, 1),
                new KeyButton(KeyButtonType.Tap | KeyButtonType.Hidden, "right", "right", HidKeys.Right, 1),
                new KeyButton(KeyButtonType.Tap, "delete", "delete", HidKeys.Delete, 1, typeof(Properties.Resources), "delete"),
                new KeyButton(KeyButtonType.Input, "input", "input", HidKeys.None, 7)
            };

            _keys[1] = new KeyButton[]
            {
                new KeyButton("-","_",1),
                new KeyButton("=","+",1),
                new KeyButton("[","{",1),
                new KeyButton("]","}",1),
                new KeyButton("\\","|",1),
                new KeyButton(";",":",1),
                new KeyButton("'","\"",1),
                new KeyButton(",","<",1),
                new KeyButton(".",">",1),
                new KeyButton("/","?",1)
            };

            _keys[2] = new KeyButton[]
            {
                new KeyButton("1","!",1),
                new KeyButton("2","@",1),
                new KeyButton("3","#",1),
                new KeyButton("4","$",1),
                new KeyButton("5","%",1),
                new KeyButton("6","^",1),
                new KeyButton("7","&",1),
                new KeyButton("8","*",1),
                new KeyButton("9","(",1),
                new KeyButton("0",")",1)
            };

            _keys[3] = new KeyButton[]
            {
                new KeyButton("q","Q",1),
                new KeyButton("w","W",1),
                new KeyButton("e","E",1),
                new KeyButton("r","R",1),
                new KeyButton("t","T",1),
                new KeyButton("y","Y",1),
                new KeyButton("u","U",1),
                new KeyButton("i","I",1),
                new KeyButton("o","O",1),
                new KeyButton("p","P",1)
            };

            _keys[4] = new KeyButton[]
            {
                new KeyButton("a","A",1),
                new KeyButton("s","S",1),
                new KeyButton("d","D",1),
                new KeyButton("f","F",1),
                new KeyButton("g","G",1),
                new KeyButton("h","H",1),
                new KeyButton("j","J",1),
                new KeyButton("k","K",1),
                new KeyButton("l","L",1),
                new KeyButton(KeyButtonType.Tap, "backspace", "backspace", HidKeys.Back, 1, typeof(Properties.Resources), "backspace"),
            };

            _keys[5] = new KeyButton[]
            {
                new KeyButton("z","Z",1),
                new KeyButton("x","X",1),
                new KeyButton("c","C",1),
                new KeyButton("v","V",1),
                new KeyButton("b","B",1),
                new KeyButton("n","N",1),
                new KeyButton("m","M",1),
                new KeyButton(KeyButtonType.Tap, "enter", "enter", HidKeys.Enter, 3, typeof(Properties.Resources), "enter"),
            };

            _keys[6] = new KeyButton[]
            {
                new KeyButton(KeyButtonType.Shift, "shift", "shift", HidKeys.Shift, 2, typeof(Properties.Resources), "shift"),
                new KeyButton(KeyButtonType.Control, "ctrl", "ctrl", HidKeys.Control, 2),
                new KeyButton(KeyButtonType.Alternate, "alt", "alt", HidKeys.Alt, 2),
                new KeyButton(KeyButtonType.Tap, " ", " ", HidKeys.Space, 3, typeof(Properties.Resources), "space"),
                new KeyButton(KeyButtonType.Windows, "windows", "windows", HidKeys.LWin, 1, typeof(Properties.Resources), "windows"),
            };
            if (Platform.IsEmulator)
            {
                using (StreamWriter fs = File.CreateText(@"\kybd.txt"))
                {
                    using (XmlTextWriter writer = new XmlTextWriter(fs))
                    {
                        writer.Formatting = Formatting.Indented;
                        new KeyButtonCollection(_keys).WriteXml(writer);
                    }
                }
                using (FileStream fs = File.OpenRead(@"\kybd.txt"))
                {
                    using (XmlTextReader writer = new XmlTextReader(fs))
                    {
                        KeyButtonCollection xx = new KeyButtonCollection(null);
                        xx.ReadXml(writer);

                        _keys = xx.Keys;
                    }
                }
            }
            */

            this.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.ShiftFont = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);

            _timer.Interval = 500;
            _timer.Tick += new EventHandler(_timer_Tick);

            LoadLayout(Properties.Settings.Default.Keyboard_DefaultLayout);
        }

        private void LoadLayout(string layout)
        {
            _currentLayout = layout;
            using (FileStream fs = File.OpenRead(Path.Combine(Path.Combine(Utils.GetWorkingDirectory(), "KeyboardLayouts"), layout)))
            {
                using (XmlTextReader writer = new XmlTextReader(fs))
                {
                    KeyButtonCollection xx = new KeyButtonCollection(null);
                    xx.ReadXml(writer);

                    _keys = xx.Keys;
                }
            }

            _keyButtonHash = new Dictionary<Point, KeyButton>();

            int x = 0, y = 0;
            for (int i = 0; i < _keys.Length; ++i)
            {
                if (null != _keys[i])
                {
                    for (int j = 0; j < _keys[i].Length; ++j)
                    {
                        for (int k = 0; k < _keys[i][j].KeyWidth; ++k)
                        {
                            if (k == 0)
                            {
                                _keys[i][j].Col = x;

                                /*
                                if (0 != (_keys[i][j].Type & KeyButtonType.Tap) && _keys[i][j].Key.Length > 0 && !_charToButton.ContainsKey(_keys[i][j].Key[0]))
                                {
                                    _charToButton.Add(_keys[i][j].Key[0], _keys[i][j]);
                                }
                                else if (0 != (_keys[i][j].Type & KeyButtonType.Hold) && !_charToButton.ContainsKey((uint)_keys[i][j].ScanCode))
                                {
                                    _charToButton.Add((uint)_keys[i][j].ScanCode, _keys[i][j]);
                                }
                                else if (!_scanToButton.ContainsKey((uint)_keys[i][j].ScanCode))
                                {
                                    _scanToButton.Add((uint)_keys[i][j].ScanCode, _keys[i][j]);
                                }
                                else
                                {
                                    //
                                }
                                if (0 != (_keys[i][j].Type & KeyButtonType.Tap) && _keys[i][j].ShiftKey.Length > 0 && !_charToButton.ContainsKey(_keys[i][j].ShiftKey[0]))
                                {
                                    _charToButton.Add(_keys[i][j].ShiftKey[0], _keys[i][j]);
                                }
                                */
                                if (!_keys[i][j].IsHidden)// && !_keys[i][j].OnKeyboard)
                                {
                                    bool addMenu = false;
                                    TaggedMenuItem menuItem = new TaggedMenuItem();
                                    if (_keys[i][j].Name.Length > 1)
                                    {
                                        menuItem.Text = char.ToUpper(_keys[i][j].Name[0]) + _keys[i][j].Name.Substring(1);
                                        addMenu = true;
                                    }
                                    else if (0 != (_keys[i][j].Type & KeyButtonType.Hold))
                                    {
                                        menuItem.Text = _keys[i][j].Type.ToString();
                                        addMenu = true;
                                    }

                                    if (addMenu)
                                    {
                                        menuItem.Tag = _keys[i][j];
                                        menuItem.Click += new EventHandler(menuItem_Click);
                                        _menuItems.Add(menuItem);
                                    }
                                }
                            }
                            if (!_keys[i][j].IsHidden)
                            {
                                Point p = new Point(x, y);
                                _keyButtonHash.Add(p, _keys[i][j]);
                                x++;
                            }
                        }
                    }
                }
                x = 0;
                y++;
            }
        }

        void menuItem_Click(object sender, EventArgs e)
        {
            TaggedMenuItem menuItem = (TaggedMenuItem)sender;
            HandleKeyPress(menuItem.Tag as KeyButton, true);
        }

        public override string ToString()
        {
            return "Keyboard";
        }

        public bool RequiresTouchscreen
        {
            get { return false; }
        }

        public override void Shutdown()
        {
            _isAltSet = _isCtrlSet = _isShiftSet = _isWinSet = false;
            _setButtons.Clear();
            _hidWriter.SetModifierState((byte)0);

            this.Refresh();
        }

        public Font ShiftFont
        {
            get { return _shiftFont; }
            set { _shiftFont = value; }
        }

        private Size _previousSize = Size.Empty;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            if (null != this.Parent && _previousSize != this.Size)
            {
                GenerateKeyboard(Properties.Settings.Default.Keyboard_Rotation);
            }
        }

        public void GenerateKeyboard(KeyboardRotation rotation)
        {
            if (null != _previousInputImage)
            {
                _previousInputImage.Dispose();
            }
            _previousInputImage = null;
            _previousSize = this.Size;
            _rotation = (MobileRemoteUI.IsLandscape) ? KeyboardRotation.ROT_0 : rotation;
            Cursor.Current = Cursors.WaitCursor;

            double scaleFactor = (double)this.Width / 480;

            if (MobileRemoteUI.IsLandscape)
            {
                scaleFactor = (double)this.Width / 640;
            }

            if (null != _keyImage)
            {
                _keyImage.Dispose();
            }
            if (null != _keyShiftImage)
            {
                _keyShiftImage.Dispose();
            }
            for (int i = 0; i < _keys.Length; ++i)
            {
                if (null != _keys[i])
                {
                    for (int j = 0; j < _keys[i].Length; ++j)
                    {
                        if (null != _keys[i][j].DownImage)
                        {
                            _keys[i][j].DownImage.Dispose();
                            _keys[i][j].DownImage = null;
                        }
                        if (null != _keys[i][j].DownShiftImage)
                        {
                            _keys[i][j].DownShiftImage.Dispose();
                            _keys[i][j].DownShiftImage = null;
                        }
                    }
                }
            }

            _keyImage = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            _keyShiftImage = new Bitmap(this.Width, this.Height, System.Drawing.Imaging.PixelFormat.Format16bppRgb565);

            for (int index = 0; index < 2; ++index)
            {
                int scrnWidth = this.Width;
                int scrnHeight = this.Height;

                // TODO
                if (_rotation != 0)
                {
                    scrnWidth = this.Height;
                    scrnHeight = this.Width;
                }

                using (Bitmap bmp = new Bitmap(scrnWidth, scrnHeight))
                {
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        using (SolidBrush fontBrush = new SolidBrush(Color.Black))
                        {
                            using (SolidBrush shiftBrush = new SolidBrush(Color.Gray))
                            {
                                using (SolidBrush whiteBrush = new SolidBrush(Color.White))
                                {
                                    g.FillRectangle(shiftBrush, 0, 0, bmp.Width, bmp.Height);
                                    _fullKeyHeight = bmp.Height / (_keys.Length);
                                    _keySpacing = Math.Max(2, (bmp.Height - (_fullKeyHeight * _keys.Length)) / _keys.Length);
                                    _keyHeight = _fullKeyHeight - _keySpacing;

                                    // TODO: we probably shouldn't hardcode 10 here
                                    _fullKeyWidth = bmp.Width / 10;
                                    _keyWidth = _fullKeyWidth - _keySpacing;

                                    _startY = _fullKeyHeight + (bmp.Height - _fullKeyHeight * (_keys.Length));
                                    _startX = (bmp.Width - (_fullKeyWidth * 10) + _keySpacing) / 2;
                                    _startY = (bmp.Height - (_fullKeyHeight * _keys.Length) + _keySpacing) / 2;

                                    int x = _startX, y = _startY;

                                    for (int i = 0; i < _keys.Length; ++i)
                                    {
                                        x = _startX;
                                        if (null != _keys[i])
                                        {
                                            for (int j = 0; j < _keys[i].Length; ++j)
                                            {
                                                if (_keys[i][j].IsHidden)
                                                {
                                                    continue;
                                                }
                                                Rectangle unRotatedBounds = new Rectangle(x, y, _keyWidth * _keys[i][j].KeyWidth + (_keys[i][j].KeyWidth - 1) * _keySpacing, _keyHeight);
                                                if (_keys[i][j].Type == KeyButtonType.Input)
                                                {
                                                    _inputRect = unRotatedBounds;
                                                }
                                                else
                                                {
                                                    g.DrawImage(_key, unRotatedBounds, new Rectangle(0, 0, _key.Width, _key.Height), GraphicsUnit.Pixel);

                                                    string key = (index == 0) ? _keys[i][j].Key : _keys[i][j].ShiftKey;
                                                    string shiftKey = (index == 0) ? _keys[i][j].ShiftKey : _keys[i][j].Key;

                                                    if (null != _keys[i][j].Icon)
                                                    {
                                                        int width = _keys[i][j].Icon.Width;
                                                        int height = _keys[i][j].Icon.Height;

                                                        int scaledWidth = (int)(width * scaleFactor);
                                                        int scaledHeight = (int)(height * scaleFactor);

                                                        System.Drawing.Imaging.ImageAttributes a = new System.Drawing.Imaging.ImageAttributes();
                                                        a.SetColorKey(_keys[i][j].Icon.GetPixel(0, 0), _keys[i][j].Icon.GetPixel(0, 0));
                                                        g.DrawImage(_keys[i][j].Icon, new Rectangle(x + (unRotatedBounds.Width - scaledWidth) / 2, y + (unRotatedBounds.Height - scaledHeight) / 2, scaledWidth, scaledHeight), 0, 0, width, height, GraphicsUnit.Pixel, a);
                                                    }
                                                    else
                                                    {
                                                        SizeF charSize = g.MeasureString(key, this.Font);
                                                        SizeF shiftCharSize = g.MeasureString(shiftKey, this.ShiftFont);

                                                        bool overlaps = false;

                                                        if ((y + _keyHeight - charSize.Height) < (y + shiftCharSize.Height))
                                                        {
                                                            overlaps = true;
                                                        }

                                                        if (shiftKey != key && !overlaps)
                                                        {
                                                            g.DrawString(key, this.Font, fontBrush, x + (_keyWidth * _keys[i][j].KeyWidth / 2) - charSize.Width / 2, y + _keyHeight - charSize.Height);
                                                            g.DrawString(shiftKey, this.ShiftFont, shiftBrush, x + (_keyWidth * _keys[i][j].KeyWidth / 2) - shiftCharSize.Width / 2, y);
                                                        }
                                                        else
                                                        {
                                                            g.DrawString(key, this.Font, fontBrush, x + (_keyWidth * _keys[i][j].KeyWidth / 2) - charSize.Width / 2, y + (_keyHeight - charSize.Height) / 2);
                                                        }
                                                    }

                                                    int startCol = x;
                                                    using (Bitmap buffer = new Bitmap(_keys[i][j].KeyWidth * _fullKeyWidth + 20, _fullKeyHeight + 20))
                                                    {
                                                        using (Graphics bufferG = Graphics.FromImage(buffer))
                                                        {
                                                            bufferG.Clear(Color.FromArgb(35, 68, 255));
                                                            bufferG.FillRectangle(whiteBrush, new Rectangle(4, 4, buffer.Width - 8, buffer.Height - 8));
                                                            if (null != _keys[i][j].Icon)
                                                            {
                                                                int width = _keys[i][j].Icon.Width;
                                                                int height = _keys[i][j].Icon.Height;

                                                                int scaledWidth = (int)(width * scaleFactor);
                                                                int scaledHeight = (int)(height * scaleFactor);

                                                                System.Drawing.Imaging.ImageAttributes a = new System.Drawing.Imaging.ImageAttributes();
                                                                a.SetColorKey(_keys[i][j].Icon.GetPixel(0, 0), _keys[i][j].Icon.GetPixel(0, 0));

                                                                bufferG.DrawImage(_keys[i][j].Icon, new Rectangle((buffer.Width - scaledWidth) / 2, (buffer.Height - scaledHeight) / 2, scaledWidth, scaledHeight), 0, 0, width, height, GraphicsUnit.Pixel, a);
                                                            }
                                                            else
                                                            {
                                                                SizeF downCharSize = bufferG.MeasureString(key, this.Font);
                                                                SizeF downShiftCharSize = bufferG.MeasureString(shiftKey, this.ShiftFont);

                                                                bufferG.DrawString(key, this.Font, fontBrush, 0 + (buffer.Width / 2) - downCharSize.Width / 2, (buffer.Height - downCharSize.Height) / 2);
                                                            }
                                                        }

                                                        if (index == 0)
                                                        {
                                                            // TODO
                                                            if (_rotation == 0)
                                                            {
                                                                _keys[i][j].DownImage = new Bitmap(buffer);
                                                            }
                                                            else
                                                            {
                                                                _keys[i][j].DownImage = new Bitmap(buffer.Height, buffer.Width);
                                                                Imaging.RotateImage((int)_rotation, buffer, _keys[i][j].DownImage);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (key == shiftKey)
                                                            {
                                                                _keys[i][j].DownShiftImage = _keys[i][j].DownImage;
                                                            }
                                                            else
                                                            {
                                                                // TODO
                                                                if (_rotation == 0)
                                                                {
                                                                    _keys[i][j].DownShiftImage = new Bitmap(buffer);
                                                                }
                                                                else
                                                                {
                                                                    _keys[i][j].DownShiftImage = new Bitmap(buffer.Height, buffer.Width);
                                                                    Imaging.RotateImage((int)_rotation, buffer, _keys[i][j].DownShiftImage);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                // sin 270 = -1
                                                // cos 270 = 0
                                                if (index == 0)
                                                {
                                                    // TODO
                                                    if (_rotation != 0)
                                                    {
                                                        if (_rotation == KeyboardRotation.ROT_270)
                                                        {
                                                            _keys[i][j].Bounds = new Rectangle(unRotatedBounds.Y, this.Height - unRotatedBounds.Right, unRotatedBounds.Height, unRotatedBounds.Width);
                                                        }
                                                        else
                                                        {
                                                            _keys[i][j].Bounds = new Rectangle(this.Width - unRotatedBounds.Bottom, unRotatedBounds.Left, unRotatedBounds.Height, unRotatedBounds.Width);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        _keys[i][j].Bounds = unRotatedBounds;
                                                    }
                                                }
                                                x += _keys[i][j].KeyWidth * _fullKeyWidth;
                                            }
                                        }
                                        x = _keySpacing;
                                        y += _fullKeyHeight;
                                    }
                                }
                            }
                        }
                        if (index == 0)
                        {
                            // TODO
                            if (_rotation == 0)
                            {
                                _keyImage = new Bitmap(bmp);
                            }
                            else
                            {
                                Imaging.RotateImage((int)_rotation, bmp, _keyImage);
                            }
                        }
                        else
                        {
                            // TODO
                            if (_rotation == 0)
                            {
                                _keyShiftImage = new Bitmap(bmp);
                            }
                            else
                            {
                                Imaging.RotateImage((int)_rotation, bmp, _keyShiftImage);
                            }
                        }
                    }
                }
            }
            _eraseRegion = this.Bounds;
            Cursor.Current = Cursors.Default;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.DrawImage((_isShiftSet) ? _keyShiftImage : _keyImage, 0, 0);
            DrawModifiers(e.Graphics);
            DrawPreviousInput(false, null);
        }

        private Rectangle _inputRect = Rectangle.Empty;
        private void DrawPreviousInput(bool dataChanged, KeyButton activeButton)
        {
            if (Rectangle.Empty != _inputRect)
            {
                int startX = _inputRect.Left;
                int startY = _inputRect.Top;

                // TODO
                if (_rotation != 0)
                {
                    if (_rotation == KeyboardRotation.ROT_270)
                    {
                        startY = this.Height - _inputRect.Right;
                        startX = _inputRect.Top;
                    }
                    else
                    {
                        startY = _inputRect.Left;
                        startX = this.Width - _inputRect.Bottom;
                    }
                }
                using (Graphics g = this.CreateGraphics())
                {
                    if (dataChanged || null == _previousInputImage)
                    {
                        if (null != _previousInputImage)
                        {
                            _previousInputImage.Dispose();
                        }
                        int width = this.Width;
                        int height = _startY;

                        width = _inputRect.Width;
                        height = _inputRect.Height;

                        using (Bitmap previousInputImage = new Bitmap(width, height))
                        {
                            using (Graphics inputG = Graphics.FromImage(previousInputImage))
                            {
                                inputG.Clear(Color.Gray);

                                using (SolidBrush brush = new SolidBrush(this.ForeColor))
                                {
                                    string str = _enteredKeys.ToString();
                                    SizeF size = inputG.MeasureString(str, this.Font);

                                    while (size.Width > width)
                                    {
                                        _enteredKeys.Remove(0, 2);
                                        str = _enteredKeys.ToString();
                                        size = inputG.MeasureString(str, this.Font);
                                    }

                                    inputG.DrawString(str, this.Font, brush, 5, previousInputImage.Height / 2 - size.Height / 2);

                                    // TODO
                                    if (_rotation != 0)
                                    {
                                        _previousInputImage = new Bitmap(height, width);
                                        Imaging.RotateImage((int)_rotation, previousInputImage, _previousInputImage);
                                    }
                                    else
                                    {
                                        _previousInputImage = new Bitmap(previousInputImage);
                                    }
                                }
                            }
                        }
                    }

                    if (null != activeButton)
                    {
                        Region r = new Region(new Rectangle(startX, startY, _previousInputImage.Width, _previousInputImage.Height));
                        r.Exclude(_eraseRegion);
                        g.Clip = r;
                    }
                    g.DrawImage(_previousInputImage, startX, startY);
                }
            }
        }

        private void DrawModifiers(Graphics g)
        {
            Image backImage = (_isShiftSet) ? _keyShiftImage : _keyImage;

            foreach (KeyButton button in _unsetButtons)
            {
                // Draw a little more than our image
                Rectangle bounds = new Rectangle(button.Bounds.Left - 4, button.Bounds.Top - 4, button.Bounds.Width + 8, button.Bounds.Height + 8);
                g.DrawImage(backImage, bounds, bounds, GraphicsUnit.Pixel);
            }

            foreach (KeyButton button in _setButtons)
            {
                using (Pen pen = new Pen(Color.FromArgb(35, 68, 255), 3))
                {
                    g.DrawRectangle(pen, new Rectangle(button.Bounds.Left + 3, button.Bounds.Top + 3, button.Bounds.Width - 6, button.Bounds.Height - 6));
                }
            }
        }

        private void DrawKey(bool sameKey)
        {
            Image backImage = (_isShiftSet) ? _keyShiftImage : _keyImage;
            if (null != backImage)
            {
                using (Graphics g = this.CreateGraphics())
                {
                    g.DrawImage(backImage, _eraseRegion, _eraseRegion, GraphicsUnit.Pixel);

                    //if (_eraseRegion.Left <= _startY)
                    //{
                    //    DrawPreviousInput(false, null);
                    //}
                    if (_eraseRegion.IntersectsWith(_inputRect))
                    {
                        DrawPreviousInput(false, null);
                    }

                    if (sameKey)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    else
                    {
                        if (_tickRefresh)
                        {
                            this.Refresh();
                            _tickRefresh = false;
                        }
                    }

                    DrawModifiers(g);

                    if (null != _activeButton && 0 != (_activeButton.Type & KeyButtonType.Tap) && !_activeButton.IsHidden)
                    {
                        Image downImage = (_isShiftSet) ? _activeButton.DownShiftImage : _activeButton.DownImage;
                        int startCol = _activeButton.Col;

                        int x = (_activeButton.Bounds.Left) - _activeButton.Bounds.Width, y = _activeButton.Bounds.Top;

                        // TODO
                        if (_rotation == 0)
                        {
                            x = _activeButton.Bounds.Left;
                            y = _activeButton.Bounds.Top - _activeButton.Bounds.Height;
                        }
                        else if (_rotation == KeyboardRotation.ROT_90)
                        {
                            x = _activeButton.Bounds.Right;
                            y = _activeButton.Bounds.Top;
                        }

                        if (x < 0)
                        {
                            x = 0;
                        }
                        if (y < 0)
                        {
                            y = _activeButton.Bounds.Top + _activeButton.Bounds.Height;
                        }

                        int popX = x - 10;
                        int popY = y - 10;

                        if (popX < 0)
                        {
                            popX = popX + 10;
                        }
                        if (popY < 0)
                        {
                            popY = popY + 10;
                        }

                        // TODO
                        if (popX + _activeButton.Bounds.Width + 20 > this.Right)
                        {
                            popX = this.Right - (_activeButton.Bounds.Width + 20);
                        }
                        if (popY + _activeButton.Bounds.Height + 20 > this.Bottom)
                        {
                            popY = this.Bottom - (_activeButton.Bounds.Height + 20);
                        }

                        _eraseRegion = new Rectangle(popX, popY, _activeButton.Bounds.Width + 20, _activeButton.Bounds.Height + 20);
                        g.DrawImage(downImage, _eraseRegion, new Rectangle(0, 0, downImage.Width, downImage.Height), GraphicsUnit.Pixel);
                    }
                }
            }
        }

        private KeyButton GetActiveButton(int x, int y, out int row, out int col)
        {
            col = -1;
            row = -1;

            if (x < 0)
            {
                return null;
            }

            int startX = _inputRect.Left;
            int startY = _inputRect.Top;

            // TODO
            if (_rotation != 0)
            {
                if (_rotation == KeyboardRotation.ROT_270)
                {
                    x = x - _startY;
                    y = y + _startX;

                    row = x / _fullKeyHeight;
                    col = (int)Math.Floor((double)(this.Height - y) / _fullKeyWidth);
                }
                else
                {
                    x = Math.Min(this.Width, Math.Max(0, x + _startY));
                    y = Math.Min(this.Height, Math.Max(0, y - _startX));

                    row = (int)Math.Floor((double)(this.Width - x) / _fullKeyHeight);
                    col = y / _fullKeyWidth;
                }
            }
            else
            {
                row = y / _fullKeyHeight;
                col = x / _fullKeyWidth;
            }

            Point p = new Point(col, row);

            if (_keyButtonHash.ContainsKey(p))
            {
                return _keyButtonHash[p];
            }

            return null;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            KeyButton button = GetActiveButton(e.X, e.Y, out _row, out _col);

            if (null != button)
            {
                bool sameKey = _downKey == button;
                _activeButton = button;
                DrawKey(sameKey && _timer.Enabled);
            }
            _downKey = null;
            _timer.Enabled = false;

        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            HandleKeyPress(_activeButton, true);

            // do hit detection here
        }

        /*
        private KeyButton MapScanToButton(uint scanCode)
        {
            if (_scanToButton.ContainsKey(scanCode))
            {
                return _scanToButton[scanCode];
            }
            return null;
        }

        private KeyButton MapCharToButton(uint keyChar)
        {
            if (_charToButton.ContainsKey(keyChar))
            {
                return _charToButton[keyChar];
            }
            return null;
        }
        */

        private void HandleKeyPress(KeyButton activeButton, bool sendData)
        {
            HandleKeyPress(activeButton, false, sendData);
        }

        private void HandleKeyPress(KeyButton activeButton, bool hidePress, bool sendData)
        {
            if (!hidePress)
            {
                _activeButton = null;
            }
            if (null != activeButton)
            {
                // send click here
                if (0 != (activeButton.Type & KeyButtonType.Hold))
                {
                    if (_setButtons.Contains(activeButton))
                    {
                        _unsetButtons.Add(activeButton);
                        _setButtons.Remove(activeButton);
                    }
                    else
                    {
                        _unsetButtons.Remove(activeButton);
                        _setButtons.Add(activeButton);
                    }
                }

                switch (activeButton.Type & ~KeyButtonType.Hidden)
                {
                    case KeyButtonType.Alternate:
                        {
                            _isAltSet = !_isAltSet;
                            DrawKey(false);
                        }
                        break;
                    case KeyButtonType.Control:
                        {
                            _isCtrlSet = !_isCtrlSet;
                            DrawKey(false);
                        }
                        break;
                    case KeyButtonType.Shift:
                        {
                            _isShiftSet = !_isShiftSet;

                            if (!hidePress)
                            {
                                _tickRefresh = false;
                                this.Refresh();
                            }
                            else
                            {
                                _tickRefresh = true;
                            }
                        }
                        break;
                    case KeyButtonType.Windows:
                        {
                            _isWinSet = !_isWinSet;
                            DrawKey(false);
                        }
                        break;
                    case KeyButtonType.Tap:
                        {
                            HandleTapKeyPress(activeButton, sendData);

                            _downKey = activeButton;

                            _timer.Enabled = false;
                            _timer.Enabled = true;
                        }
                        break;
                }

                if (0 != (activeButton.Type & KeyButtonType.Hold))
                {
                    BluetoothHidWriter.KeyModifiers modifierState = GetModifierState();
                    _hidWriter.SetModifierState((byte)modifierState);
                }
            }
        }

        private BluetoothHidWriter.KeyModifiers GetModifierState()
        {
            BluetoothHidWriter.KeyModifiers modifierState = 0;

            foreach (KeyButton button in _setButtons)
            {
                switch (button.Type)
                {
                    case KeyButtonType.Alternate:
                        {
                            modifierState |= BluetoothHidWriter.KeyModifiers.LALT;
                        }
                        break;
                    case KeyButtonType.Control:
                        {
                            modifierState |= BluetoothHidWriter.KeyModifiers.LCTRL;
                        }
                        break;
                    case KeyButtonType.Shift:
                        {
                            modifierState |= BluetoothHidWriter.KeyModifiers.LSHIFT;
                        }
                        break;
                    case KeyButtonType.Windows:
                        {
                            modifierState |= BluetoothHidWriter.KeyModifiers.LGUI;
                        }
                        break;
                }
            }

            return modifierState;
        }

        private void HandleTapKeyPress(KeyButton activeButton, bool sendData)
        {
            // get all the set buttons here
            // Build modifiers here

            BluetoothHidWriter.KeyModifiers modifierState = GetModifierState();
            char keyChar = (char)0;

            if (activeButton.HidKey == HidKeys.Enter)
            {
                _enteredKeys.Remove(0, _enteredKeys.Length);
            }
            else if (activeButton.HidKey == HidKeys.Back)
            {
                if (_enteredKeys.Length > 0)
                {
                    _enteredKeys.Remove(_enteredKeys.Length - 1, 1);
                }
            }
            else
            {
                if (activeButton.IsChar)
                {
                    char c = '\0';
                    if (0 != (modifierState & BluetoothHidWriter.KeyModifiers.LSHIFT))
                    {
                        c = activeButton.ShiftKey[0];
                    }
                    else
                    {
                        c = activeButton.Key[0];
                    }
                    //if (char.IsLetterOrDigit(c))
                    {
                        keyChar = c;
                        _enteredKeys.Append(c);
                    }
                }
            }
            DrawPreviousInput(true, activeButton);

            if (sendData)
            {
                _hidWriter.SendKeyPress((byte)modifierState, activeButton.HidKey);
            }

            for (int i = 0; i < _setButtons.Count; ++i)
            {
                if (0 != (_setButtons[i].Type & KeyButtonType.Hold) && _setButtons[i].Type != KeyButtonType.Shift)
                {
                    _unsetButtons.Add(_setButtons[i]);
                    _setButtons.RemoveAt(i);
                    --i;
                }
            }
            using (Graphics g = this.CreateGraphics())
            {
                DrawModifiers(g);
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            _timer.Enabled = false;
            _downKey = null;

            if (_tickRefresh)
            {
                this.Refresh();
                _tickRefresh = false;
            }

            DrawKey(false);
            //DrawPreviousInput(true, null);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // do hit detection here
            KeyButton activeButton = GetActiveButton(e.X, e.Y, out _row, out _col);

            if (_activeButton != activeButton)
            {
                _activeButton = activeButton;
                DrawKey(false);
            }
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

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.Font = new System.Drawing.Font("Tahoma", 14F, System.Drawing.FontStyle.Bold);
            this.Name = "KeyboardControl";
            this.Size = new System.Drawing.Size(560, 480);
            this.ResumeLayout(false);

        }

        public override bool HandleKeyDown(KeyEventArgs e)
        {
            return false;
        }

        public override bool HandleKeyUp(KeyEventArgs e)
        {
            e.Handled = true;
            _hidWriter.SendKeyPress((byte)((e.Modifiers & Keys.Shift) != 0 ? BluetoothHidWriter.KeyModifiers.LSHIFT : 0), (HidKeys)e.KeyCode);

            char keyChar = HidCodes.GetHidChar((HidKeys)e.KeyCode, (e.Modifiers & Keys.Shift) != 0);
            
            if ((HidKeys)e.KeyCode == HidKeys.Enter)
            {
                _enteredKeys.Remove(0, _enteredKeys.Length);
            }
            else if ((HidKeys)e.KeyCode == HidKeys.Back)
            {
                if (_enteredKeys.Length > 0)
                {
                    _enteredKeys.Remove(_enteredKeys.Length - 1, 1);
                }
            }
            else
            {
                if (keyChar != 0)
                {
                    _enteredKeys.Append(keyChar);
                }
            }
            DrawPreviousInput(true, null);

            return true;
        }

        public override bool HandleKeyPress(KeyPressEventArgs e)
        {
            e.Handled = true;
            return true;
            bool isShift = false;

            try
            {
                HidKeys hidKey = HidCodes.GetHidKey(e.KeyChar, out isShift);
                _hidWriter.SendKeyPress((byte)(isShift ? BluetoothHidWriter.KeyModifiers.LSHIFT : 0), hidKey);

                _enteredKeys.Append(e.KeyChar);
                DrawPreviousInput(true, null);

                return true;
            }
            catch
            {
                return false;
            }
        }
        /*
        public bool HandleKeyUp(KeyEventArgs e)
        {
            e.Handled = true;
            e.KeyCode
            KeyButton button = MapCharToButton((uint)e.KeyCode);

            if (null == button || 0 == (button.Type & KeyButtonType.Tap))
            {
                if (0 != (e.Modifiers & Keys.Control))
                {
                    KeyButton modifierButton = MapCharToButton((uint)Keys.Control);

                    if (null != modifierButton)
                    {
                        HandleKeyPress(modifierButton, true);
                    }
                }
                if (0 != (e.Modifiers & Keys.Alt))
                {
                    KeyButton modifierButton = MapCharToButton((uint)Keys.Alt);

                    if (null != modifierButton)
                    {
                        HandleKeyPress(modifierButton, true);
                    }
                }
                if (null != button && button.Type == KeyButtonType.Shift)
                {
                    bool isCaps = !(L2CAPAPI.GetKeyState((int)Keys.CapsLock) != 0);
                    bool isShift = !(L2CAPAPI.GetKeyState((int)Keys.ShiftKey) != 0);

                    if (isCaps == isShift && null != _downKey)
                    {
                        isCaps = isShift = false;
                    }

                    KeyButton modifierButton = MapCharToButton((uint)Keys.ShiftKey);

                    if (null != modifierButton && (_isShiftSet != (isCaps || isShift)))
                    {
                        HandleKeyPress(modifierButton, null != _downKey, true);
                    }
                }
                if (0 != (e.Modifiers & Keys.LWin) || 0 != (e.Modifiers & Keys.RWin))
                {
                    KeyButton modifierButton = MapCharToButton((uint)Keys.LWin);

                    if (null != modifierButton)
                    {
                        HandleKeyPress(modifierButton, true);
                    }
                }
            }
            else if (!char.IsLetterOrDigit((char)e.KeyCode) && Keys.Enter != e.KeyCode)// && Keys.Back != e.KeyCode)
            {
                uint x0 = Win32.MapVirtualKey((uint)e.KeyCode, 0);
                uint x1 = Win32.MapVirtualKey((uint)e.KeyCode, 1);
                uint x2 = Win32.MapVirtualKey((uint)e.KeyCode, 2);
                uint x3 = Win32.MapVirtualKey((uint)e.KeyCode, 3);

                uint scanCode = 0;

                switch (e.KeyCode)
                {
                    case Keys.Up:
                        {
                            scanCode = ScanCodeUp;
                        }
                        break;
                    case Keys.Down:
                        {
                            scanCode = ScanCodeDown;
                        }
                        break;
                    case Keys.Left:
                        {
                            scanCode = ScanCodeLeft;
                        }
                        break;
                    case Keys.Right:
                        {
                            scanCode = ScanCodeRight;
                        }
                        break;
                }

                if (scanCode != 0)
                {
                    button = MapScanToButton(scanCode);

                    Program.Debug.WriteLine(string.Format("OnKeyUp: [scanButton: {0}, keyChar: {1}]", (null != button) ? button.ScanCode.ToString() : "null", scanCode));
                    HandleHardwarePress(button, (char)scanCode);
                }
            }
            return e.Handled;
        }
        public bool HandleKeyPress(KeyPressEventArgs e)
        {
            if (null == _activeButton)
            {
                KeyButton button = MapCharToButton((uint)e.KeyChar);
                KeyButton scanButton = MapScanToButton((uint)e.KeyChar);

                if (null == button && null != scanButton)
                {
                    Program.Debug.WriteLine(string.Format("OnKeyPress: [scanButton: {0}, keyChar: {1}]", scanButton.ScanCode, e.KeyChar));
                    HandleHardwarePress(scanButton, e.KeyChar);
                }
                else
                {
                    Program.Debug.WriteLine(string.Format("OnKeyPress: [button: {0}, keyChar: {1}]", (null != button) ? button.Key : "null", e.KeyChar));
                    HandleHardwarePress(button, e.KeyChar);
                }
            }
            return true;
        }

        public void HandleHardwarePress(KeyButton button, char keyChar)
        {
            bool isScanCode = false;
            if (null != button && 0 != (button.Type & KeyButtonType.Tap))
            {
                bool shiftToggle = (_isShiftSet != (button.ShiftKey.Length == 1 && button.ShiftKey[0] == keyChar));
                isScanCode = button.IsKeyCode;

                if (button.ShiftKey == button.Key)
                {
                    shiftToggle = false;
                }

                if (shiftToggle)
                {
                    HandleKeyPress(MapCharToButton((uint)Keys.ShiftKey), false);
                    System.Threading.Thread.Sleep(100);
                }

                bool sameKey = _downKey == button;
                _activeButton = button;

                DrawKey(sameKey && _timer.Enabled);
                HandleKeyPress(button, false);

                _downKey = button;
                _timer.Enabled = true;
            }
            else if (keyChar != 0)
            {
                _enteredKeys.Append(keyChar);
                DrawPreviousInput(true, null);
            }
            _hidWriter.SendKeyPress((byte)GetModifierState(), keyChar, isScanCode, keyChar);
        }
        */
        //protected override void OnKeyUp(KeyEventArgs e)
        //{
        //    base.OnKeyUp(e);
        //    KeyButton button = MapCharToButton((uint)e.KeyCode);

        //    if (null != button)
        //    {
        //        bool isCaps = L2CAPAPI.GetKeyState((int)Keys.CapsLock) != 0;
        //        bool isShift = L2CAPAPI.GetKeyState((int)Keys.ShiftKey) != 0;

        //        if (button.Type == KeyButtonType.Shift)
        //        {
        //            isShift = !isShift;
        //            isCaps = !isCaps;
        //        }

        //        bool shouldShiftSet = (isCaps || isShift);

        //        if (_isShiftSet != shouldShiftSet)
        //        {
        //            if (null == _downKey)
        //            {
        //                System.Threading.Thread.Sleep(100);
        //            }
        //            HandleKeyPress(MapCharToButton((uint)Keys.ShiftKey), (null != _downKey), true);
        //        }
        //    }

        //    if (null != _downKey)
        //    {
        //        _timer.Enabled = false;
        //        _timer.Enabled = true;
        //    }
        //}
    }
}