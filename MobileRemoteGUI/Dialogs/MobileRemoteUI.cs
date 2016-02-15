using System;
using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace MobileSRC.MobileRemote
{
    using Inputs;
    internal partial class MobileRemoteUI : Form
    {
        public static ManualResetEvent MobileRemoteLoadEvent = new ManualResetEvent(false);
        public static ManualResetEvent MobileRemoteReturnEvent = new ManualResetEvent(false);
        internal class InputInterfaceControl
        {
            public Bitmap Up, Down, Sel; 
            public IInputControl Control;
            public string Text;
            public EventHandler Click;
            public object Tag;

            public InputInterfaceControl(string text, IInputControl control, Bitmap up, Bitmap down, Bitmap sel)
            {
                Text = text;
                Control = control;
                Up = up;
                Down = down;
                Sel = sel;
            }
        }

        private static SizeF _scaleFactor;
        private static MobileRemoteUI _instance;

        static MobileRemoteUI()
        {
            _scaleFactor = new SizeF((float)Screen.PrimaryScreen.Bounds.Width / 480, (float)Screen.PrimaryScreen.Bounds.Height / 640);
        }

        public static MobileRemoteUI Instance
        {
            get { return _instance; }
        }

        private BluetoothHidWriter _hidWriter;
        private KeyboardControl _keyboardControl = new KeyboardControl();
        private TouchControl _touchPanel = new TouchControl();
        private MouseControl _mouseControl = new MouseControl();
        //private MediaControl _mediaControl = new MediaControl();
        //private PowerPointControl _presentControl = new PowerPointControl();
        private IInputControl _activePanel = null;
        private Dictionary<object, InputInterfaceControl> _inputList = new Dictionary<object, InputInterfaceControl>();
        private List<InputInterfaceControl> _inputs;
        private MainMenu _mainMenu = new MainMenu();
        private ContextMenu _mainContextMenu = new ContextMenu();
        private MenuItem _leftMenu = new MenuItem(), _connectionMenuItem = new MenuItem(), _startRecorder = new MenuItem(), _stopRecorder = new MenuItem(), _loadRecorder = new MenuItem(), _controlSettingsMenu = new MenuItem(), _registerMenu = new MenuItem();
        private L2CAPAPI.RadioMode _initialRadioMode = L2CAPAPI.RadioMode.Off;
        IntPtr _mOldWndProc;
        Win32.WndProcDelegate _mProc;

        public MobileRemoteUI()
        {
            this.IsLoaded = false;
            Properties.Settings.Load();
            L2CAPAPI.BthGetMode(out _initialRadioMode);
            _instance = this;
            this.Load += new EventHandler(MobileRemoteUI_Load);
            this.GotFocus += new EventHandler(MobileRemoteUI_GotFocus);
            _inputs = new List<InputInterfaceControl>(
                new InputInterfaceControl[]
                {
                    new InputInterfaceControl("Keyboard", _keyboardControl, Properties.Resources.keyboard, Properties.Resources.keyboard_down, Properties.Resources.keyboard_sel),
                    new InputInterfaceControl("Mouse", _mouseControl, Properties.Resources.mouse, Properties.Resources.mouse_down, Properties.Resources.mouse_sel),
                    new InputInterfaceControl("TouchPad", _touchPanel, Properties.Resources.touch, Properties.Resources.touch_down, Properties.Resources.touch_sel),
                    //new InputInterfaceControl("Media", _mediaControl, Properties.Resources.media, Properties.Resources.media_down, Properties.Resources.media_sel),
                    //new InputInterfaceControl("Present", _presentControl, Properties.Resources.present, Properties.Resources.present_down, Properties.Resources.present_sel)
                }
            );
            InitializeComponent();

            _hidWriter = new BluetoothHidWriter();
            _hidWriter.Connected += new EventHandler(_hidWriter_Connected);
            _hidWriter.Disconnected += new EventHandler(_hidWriter_Disconnected);

            //_presentControl.HidWriter = _hidWriter;
            //_mediaControl.HidWriter = _hidWriter;
            _mouseControl.HidWriter = _hidWriter;
            _touchPanel.HidWriter = _hidWriter;
            _keyboardControl.HidWriter = _hidWriter;
            _connectButton.HidWriter = _hidWriter;

            XmlSerializer xs = new XmlSerializer(typeof(Classes.CustomInput));
            foreach (string customInput in Directory.GetFiles(Path.Combine(Utils.GetWorkingDirectory(), "CustomInputs")))
            {
                using (StreamReader fs = File.OpenText(customInput))
                {
                    Classes.CustomInput ci = (Classes.CustomInput)xs.Deserialize(fs);
                    ci.ScaleToResolution();
                    
                    CustomInputControl input = new CustomInputControl(ci, _hidWriter);
                    InputInterfaceControl interfaceControl = new InputInterfaceControl
                        (
                            ci.Name, input,
                            CustomInputControl.GetBitmap(ci.Image),
                            CustomInputControl.GetBitmap(ci.DownImage),
                            CustomInputControl.GetBitmap(ci.SelectedImage)
                        );

                    _inputs.Add(interfaceControl);
                }
            }

            Menu menu = null;

            if (!Platform.IsWindowsMobileClassic)
            {
                int totalButtonsVisible = (buttonPanel1.Width / buttonPanel1.Height) - 2;

                if (_inputs.Count < totalButtonsVisible)
                {
                    // great...
                    buttonPanel1.Controls.Remove(_inputsButton);
                }
                else
                {
                    totalButtonsVisible--;
                }

                int count = 0;
                foreach (InputInterfaceControl control in _inputs)
                {
                    PushButton button = new PushButton();
                    button.DownImage = control.Down;
                    button.Image = control.Up;
                    button.Name = control.Text;
                    button.Text = control.Text;
                    button.SelectedImage = control.Sel;
                    button.Size = new System.Drawing.Size(buttonPanel1.Height, buttonPanel1.Height);
                    button.TabIndex = 11;
                    button.Dock = DockStyle.Left;
                    button.Click += new EventHandler(button_Click);
                    button.RightClick += new EventHandler(_controlSettingsMenu_Click);

                    if (_inputList.Count < totalButtonsVisible)
                    {
                    }
                    else
                    {
                        button.Visible = false;
                    }
                    buttonPanel1.Controls.Add(button);
                    buttonPanel1.Controls.SetChildIndex(button, count++);

                    control.Click = button_Click;
                    control.Tag = button;

                    _inputList.Add(button, control);
                }
                ((PushButton)_inputs[0].Tag).IsSelected = true;

                menu = _mainContextMenu;
            }
            else
            {
                this.buttonPanel1.Visible = false;

                _leftMenu = new MenuItem();
                _leftMenu.Text = "";

                MenuItem rightMenu = new MenuItem();
                rightMenu.Text = "Menu";

                _mainMenu.MenuItems.Add(_leftMenu);
                _mainMenu.MenuItems.Add(rightMenu);

                int i = 0;
                foreach (InputInterfaceControl control in _inputs)
                {
                    if (!control.Control.RequiresTouchscreen)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Text = String.Format("&{1}", i++, control.Text);
                        menuItem.Click += new EventHandler(menuItem_Click);
                        rightMenu.MenuItems.Add(menuItem);

                        control.Click = menuItem_Click;
                        control.Tag = menuItem;

                        _inputList.Add(menuItem, control);
                    }
                }
                ((MenuItem)_inputs[0].Tag).Checked = true;

                MenuItem seperatorMenu1 = new MenuItem();
                seperatorMenu1.Text = "-";
                rightMenu.MenuItems.Add(seperatorMenu1);

                this.Menu = _mainMenu;
                menu = rightMenu;
            }


            MenuItem connectionSettingsMenu = new MenuItem();
            connectionSettingsMenu.Text = "Connection &Settings";
            connectionSettingsMenu.Click += new EventHandler(connectionSettingsMenu_Click);

            _controlSettingsMenu.Text = "Control Settings";
            _controlSettingsMenu.Click += new EventHandler(_controlSettingsMenu_Click);

            MenuItem recorderMenu = new MenuItem();
            recorderMenu.Text = "&Recorder";

            _startRecorder.Text = "&Create Recording";
            _startRecorder.Click += new EventHandler(startRecorder_Click);

            _stopRecorder.Text = "&Stop Recording";
            _stopRecorder.Enabled = false;
            _stopRecorder.Click += new EventHandler(stopRecorder_Click);

            _loadRecorder.Text = "&Load Recording";
            _loadRecorder.Enabled = true;
            _loadRecorder.Popup += new EventHandler(_loadRecorder_Popup);

            MenuItem nullMenuItem = new MenuItem();
            nullMenuItem.Text = "--No Saved Recordings--";
            _loadRecorder.MenuItems.Add(nullMenuItem);

            recorderMenu.MenuItems.Add(_loadRecorder);
            recorderMenu.MenuItems.Add(_startRecorder);
            recorderMenu.MenuItems.Add(_stopRecorder);

            _connectionMenuItem.Text = "&Connect";
            _connectionMenuItem.Click += new EventHandler(connect_Click);

            _registerMenu.Text = "Re&gister";
            _registerMenu.Click += new EventHandler(_registerMenu_Click);

            MenuItem aboutMenuItem = new MenuItem();
            aboutMenuItem.Text = "&About";
            aboutMenuItem.Click += new EventHandler(aboutMenuItem_Click);

            MenuItem exitMenu = new MenuItem();
            exitMenu.Text = "E&xit";
            exitMenu.Click += new EventHandler(exitMenu_Click);

            menu.MenuItems.Add(recorderMenu);
            menu.MenuItems.Add(_connectionMenuItem);
            menu.MenuItems.Add(connectionSettingsMenu);
            menu.MenuItems.Add(_controlSettingsMenu);
            menu.MenuItems.Add(_registerMenu);
            menu.MenuItems.Add(aboutMenuItem);
            menu.MenuItems.Add(exitMenu);

            SetActivePanel(_inputs[0]);
            this.Closing += new CancelEventHandler(MobileRemoteUI_Closing);
            this.IsLoaded = true;
        }

        public bool IsLoaded
        {
            get;
            set;
        }

        public IEnumerator Inputs
        {
            get { return _inputs.GetEnumerator(); }
        }

        public int ButtonSize
        {
            get { return buttonPanel1.Height; }
        }

        void MobileRemoteUI_Closing(object sender, CancelEventArgs e)
        {
            L2CAPAPI.RadioMode currentRadioMode = 0;
            L2CAPAPI.BthGetMode(out currentRadioMode);

            if (currentRadioMode != _initialRadioMode)
            {
                L2CAPAPI.BthSetMode(_initialRadioMode);
            }
            _hidWriter.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            _scaleFactor = new SizeF((float)Screen.PrimaryScreen.Bounds.Width / 480, (float)Screen.PrimaryScreen.Bounds.Height / 640);

            buttonPanel1.Dock = DockStyle.Bottom;
            buttonPanel1.Height = Platform.IsVGA ? 80 : 40;// (int)Math.Min(80, (((double)Math.Min(640, Math.Max(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height)) / 640) * 80));
        }

        void aboutMenuItem_Click(object sender, EventArgs e)
        {
            using (About about = new About())
            {
                about.ShowDialog();
            }
        }

        void _registerMenu_Click(object sender, EventArgs e)
        {
            ShowRegistrationDialog();
        }

        private class TaggedMenuItem : MenuItem
        {
            public object Tag;
        }

        void _loadRecorder_Popup(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            menuItem.MenuItems.Clear();
            string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "*.mrm");

            Array.Sort<string>(files);

            foreach (string macro in files)
            {
                TaggedMenuItem macroMenuItem = new TaggedMenuItem();
                macroMenuItem.Text = Path.GetFileNameWithoutExtension(macro);
                macroMenuItem.Tag = macro;
                macroMenuItem.Click += new EventHandler(macroMenuItem_Click);

                menuItem.MenuItems.Add(macroMenuItem);
            }

            if (menuItem.MenuItems.Count == 0)
            {
                MenuItem nullMenuItem = new MenuItem();
                nullMenuItem.Text = "--No Saved Recordings--";
                menuItem.MenuItems.Add(nullMenuItem);
            }
        }

        void macroMenuItem_Click(object sender, EventArgs e)
        {
            TaggedMenuItem macroMenuItem = (TaggedMenuItem)sender;
            ExecuteMacro(macroMenuItem.Tag as string);
        }

        void connectionSettingsMenu_Click(object sender, EventArgs e)
        {
            ShowNewConnectionSettings();
        }

        void stopRecorder_Click(object sender, EventArgs e)
        {
            try
            {
                _stopRecorder.Enabled = !(_startRecorder.Enabled = true);
                BluetoothHidWriter.RecorderList data = _hidWriter.StopRecorder();

                string fileName = string.Empty;

                if (Platform.IsWindowsMobileStandard)
                {
                    // TODO: allow for other macros
                    using (StringInput stringInput = new StringInput("Enter Name", "Enter Recorded File Name:"))
                    {
                        if (DialogResult.OK == stringInput.ShowDialog(this) && !string.IsNullOrEmpty(stringInput.Value))
                        {
                            fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), string.Format("{0}.mrm", stringInput.Value));
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "MobileRemote Macro|.mrm";

                        if (DialogResult.OK == sfd.ShowDialog())
                        {
                            fileName = sfd.FileName;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    using (FileStream fs = File.Create(fileName))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] binaryData = data.Serialize();
                            bw.Write(binaryData);
                            bw.Flush();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("There was an error saving your recording", "Error Saving Recording", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
            }
        }

        void startRecorder_Click(object sender, EventArgs e)
        {
            _stopRecorder.Enabled = !(_startRecorder.Enabled = false);
            _hidWriter.StartRecorder();
        }

        void _controlSettingsMenu_Click(object sender, EventArgs e)
        {
            if (_inputList.ContainsKey(sender))
            {
                _inputList[sender].Control.ShowSettings();
            }
            else if (null != _activePanel)
            {
                _activePanel.ShowSettings();
            }
        }

        public ButtonPanel ButtonPanel
        {
            get { return this.buttonPanel1; }
        }

        void SetIMEMode()
        {
            uint ret = 0;
            bool bret = false;
            IntPtr hC = L2CAPAPI.ImmGetContext(MobileRemoteUI.Instance.Handle);
            // Open the IME 
            bret = L2CAPAPI.ImmSetOpenStatus(hC, true);
            // Set "multi-press" input mode
            ret = (uint)L2CAPAPI.ImmEscape(IntPtr.Zero, hC, L2CAPAPI.IME_ESC_SET_MODE, L2CAPAPI.IM_SPELL);
        }

        void MobileRemoteUI_GotFocus(object sender, EventArgs e)
        {
            SetIMEMode();
        }

        void MobileRemoteUI_Load(object sender, EventArgs e)
        {
            _mProc = new Win32.WndProcDelegate(WndProc);
            _mOldWndProc = Win32.SetWindowLong(this.Handle, Win32.GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(_mProc));

            System.Windows.Forms.Timer shownTimer = new System.Windows.Forms.Timer();
            shownTimer.Interval = 100;
            shownTimer.Tick += new EventHandler(shownTimer_Tick);

            shownTimer.Enabled = true;
            MobileRemoteLoadEvent.Set();

            SetIMEMode();
        }

        protected virtual int WndProc(IntPtr hwnd, uint msg, uint wParam, int lParam)
        {
            if (msg == Win32.WM_HOTKEY && (lParam & 0xffff0000) == 0x1b0000)
            {
                if ((lParam & 0x1000) == 0)
                {
                    // This is key down message of [Back] key
                }
                else
                {
                    // This is key up message of [Back] key
                    OnKeyPress(new KeyPressEventArgs((char)Keys.Back));
                }
                return 1;
            }

            return Win32.CallWindowProc(_mOldWndProc, hwnd, msg, wParam, lParam);
        }

        void shownTimer_Tick(object sender, EventArgs e)
        {
            if (this.Visible && !Platform.IsEmulator)
            {
                MobileRemoteReturnEvent.WaitOne();
                ((System.Windows.Forms.Timer)sender).Enabled = false;

                if (!Utils.CheckRegistration())
                {
                    ShowRegistrationDialog();
                }
                if (Utils.CheckRegistration())
                {
                    _registerMenu.Parent.MenuItems.Remove(_registerMenu);
                }
                ShowConnectDialog(true);
            }
        }

        void exitMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        void connect_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.Checked)
            {
                _hidWriter.Disconnect();
            }
            else
            {
                ShowConnectDialog(false);
            }
        }

        private void button_Click(object sender, EventArgs e)
        {
            if (_activePanel == _inputList[sender].Control)
            {
                _inputList[sender].Control.SelectedClick();
            }
            else
            {
                InputSelector.Enabled = false;
            }
            SetActivePanel(_inputList[sender]);
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            foreach (MenuItem siblingMenuItem in menuItem.Parent.MenuItems)
            {
                if (_inputList.ContainsKey(menuItem))
                {
                    if (siblingMenuItem.Checked)
                    {
                        siblingMenuItem.Checked = false;
                    }
                }
            }
            menuItem.Checked = true;
            SetActivePanel(_inputList[menuItem]);
        }

        public static SizeF ScaleFactor
        {
            get { return _scaleFactor; }
        }

        public static bool IsLandscape
        {
            get { return (Screen.PrimaryScreen.Bounds.Width >= Screen.PrimaryScreen.Bounds.Height); }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (null != _activePanel)
            {
                SetOrientation(_activePanel.DesiredOrientation);
            }
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            SetOrientation(Win32.RotationMode.DMDO_0);

            if (null != _activePanel)
            {
                _activePanel.Shutdown();
            }
        }
        
        private void SetOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                if (Screen.PrimaryScreen.Bounds.Width < Screen.PrimaryScreen.Bounds.Height)
                {
                    if (MobileRemoteUI.IsLandscape)
                    {
                        SetOrientation(Win32.RotationMode.DMDO_0);
                    }
                    else
                    {
                        SetOrientation(Win32.RotationMode.DMDO_90);
                    }
                }
            }
            else
            {
                if (Screen.PrimaryScreen.Bounds.Width > Screen.PrimaryScreen.Bounds.Height)
                {
                    if (MobileRemoteUI.IsLandscape)
                    {
                        SetOrientation(Win32.RotationMode.DMDO_0);
                        //SetOrientation(Win32.RotationMode.DMDO_90);
                    }
                    else
                    {
                        SetOrientation(Win32.RotationMode.DMDO_0);
                    }
                }
            }
        }

        private void SetOrientation(Win32.RotationMode mode)
        {
            return;
            Win32.DEVMODE devMode = new Win32.DEVMODE();
            devMode.dmSize = (short)Marshal.SizeOf(typeof(Win32.DEVMODE));
            devMode.dmFields = 0x00800000;
            devMode.dmDisplayOrientation = (int)mode;

            Win32.ChangeDisplaySettingsEx(null, ref devMode, IntPtr.Zero, 0, IntPtr.Zero);
        }

        public void SelectInput(InputInterfaceControl inputControl)
        {
            inputControl.Control.SelectedClick();
            SetActivePanel(inputControl);
        }

        private void SetActivePanel(InputInterfaceControl control)
        {
            SetActivePanel(control.Tag, control.Control);
        }

        private void SetActivePanel(object button, IInputControl control)
        {
            if (control != _activePanel)
            {
                if (null != _activePanel)
                {
                    _activePanel.Shutdown();
                }
                _activePanel = control;

                control.Dock = DockStyle.Fill;

                SetOrientation(control.DesiredOrientation);

                if (!panel1.Controls.Contains((Control)control))
                {
                    //panel1.Controls.Clear();
                    panel1.Controls.Add((Control)control);
                }
                ((Control)control).BringToFront();

                if (Platform.IsWindowsMobileStandard)
                {
                    _keyboardControl.Focus();
                }
                else
                {
                    ((Control)control).Focus();
                }

                _leftMenu.Text = "";
                _leftMenu.MenuItems.Clear();
                if (null != control.MenuItems && control.MenuItems.Count > 0)
                {
                    _leftMenu.Text = control.ToString();
                    foreach (MenuItem menuItem in control.MenuItems)
                    {
                        _leftMenu.MenuItems.Add(menuItem);
                    }
                }
            }
        }

        void ExecuteMacro(string macroFile)
        {
            if (File.Exists(macroFile))
            {
                BluetoothHidWriter.RecorderList data = new BluetoothHidWriter.RecorderList();
                using (FileStream fs = File.OpenRead(macroFile))
                {
                    using (BinaryReader bw = new BinaryReader(fs))
                    {
                        data.Deserialize(bw);
                    }
                }
                Cursor.Current = Cursors.WaitCursor;
                _hidWriter.ExecuteRecorder(data, 10);
                Cursor.Current = Cursors.Default;
            }
        }

        void _hidWriter_Connected(object sender, EventArgs e)
        {
            this.Invoke(new ThreadStart(_hidWriter_Connected_Invoke));
        }

        void _hidWriter_Connected_Invoke()
        {
            AddressSelector.AddNewAddress(_hidWriter.BluetoothAddress, false);

            CheckConnection();

            ConnectionSettingCollection.ConnectionSetting setting = Properties.Settings.Default.ConnectionSettings[_hidWriter.BluetoothAddress];
            if (null == setting)
            {
                ShowNewConnectionSettings();
            }
            setting = Properties.Settings.Default.ConnectionSettings[_hidWriter.BluetoothAddress];

            if (null != setting)
            {
                // Execute the connect macro here
                if (!string.IsNullOrEmpty(setting.ConnectMacro))
                {
                    ExecuteMacro(setting.ConnectMacro);
                }
            }
        }

        void _hidWriter_Disconnected(object sender, EventArgs e)
        {
            this.Invoke(new ThreadStart(CheckConnection));
        }

        void ShowNewConnectionSettings()
        {
            using (ConnectSuccess selector = new ConnectSuccess())
            {
                if (DialogResult.OK == selector.ShowDialog(this))
                {
                }
            }
        }

        void CheckConnection()
        {
            if (_hidWriter.IsConnected)
            {
                _connectionMenuItem.Checked = true;
                _connectionMenuItem.Text = "&Disconnect";
            }
            else
            {
                _connectionMenuItem.Checked = false;
                _connectionMenuItem.Text = "&Connect";
            }
        }

        public void FocusActive()
        {
            panel1.Controls[0].Focus();
            this.Refresh();
        }

        private void ShowRegistrationDialog()
        {
            using (Register registerDialog = new Register())
            {
                if (DialogResult.OK == registerDialog.ShowDialog())
                {
                    // we have successfully registered
                }
            }
        }

        private void ShowConnectDialog(bool autoConnect)
        {
            L2CAPAPI.RadioMode radioMode = L2CAPAPI.RadioMode.Off;
            L2CAPAPI.BthGetMode(out radioMode);

            if (radioMode != L2CAPAPI.RadioMode.Discoverable)
            {
                if (autoConnect || (DialogResult.Yes == MessageBox.Show("MobileRemote requires that Bluetooth is enabled.  Would you like to enable Bluetooth on your phone?", "Bluetooth Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)))
                {
                    L2CAPAPI.BthSetMode(L2CAPAPI.RadioMode.Discoverable);
                }
                else
                {
                    return;
                }
            }

            using (AddressSelector selector = new AddressSelector())
            {
                bool firstConnect = autoConnect && (selector.SelectedDevice.Address != 0);
                if (firstConnect || (DialogResult.OK == selector.ShowDialog(this)))
                {
                    if (null != selector.SelectedDevice)
                    {
                        bool legacyMode = selector.LegacyMode;
                        int oldMode = 0;

                        if (legacyMode)
                        {
                            oldMode = Utils.SetCompatabilityMode();
                        }
                        WaitHandle result = _hidWriter.ConnectAsync(selector.SelectedDevice.Address);
                        try
                        {
                            using (WaitForConnection waitDialog = new WaitForConnection())
                            {
                                if (DialogResult.OK != waitDialog.ShowDialog(this, result, selector.SelectedDevice, _hidWriter))
                                {
                                    _hidWriter.Disconnect();
                                }
                            }
                        }
                        finally
                        {
                            if (legacyMode)
                            {
                                Utils.UnsetCompatabilityMode(oldMode);
                            }
                        }
                    }
                }
            }
        }

        private void connectButton1_ConnectRequested(object sender, EventArgs e)
        {
            ShowConnectDialog(false);
        }

        private void connectButton1_DisconnectRequested(object sender, EventArgs e)
        {
            ConnectionSettingCollection.ConnectionSetting setting = Properties.Settings.Default.ConnectionSettings[_hidWriter.BluetoothAddress];

            if (null != setting)
            {
                // Execute the disconnect macro here
                if (!string.IsNullOrEmpty(setting.DisconnectMacro))
                {
                    ExecuteMacro(setting.DisconnectMacro);
                }
            }
            _hidWriter.Disconnect();
        }

        private void _optionsButton_Click(object sender, System.EventArgs e)
        {
            using (CustomMenu menu = new CustomMenu())
            {
                menu.ShowDialog(this, _mainContextMenu);
            }
        }

        private void _inputsButton_Click(object sender, System.EventArgs e)
        {
            using (InputControlSelector selector = new InputControlSelector())
            {
                DialogResult dialogResult = selector.ShowDialog(MobileRemoteUI.Instance);
                if (DialogResult.OK == dialogResult)
                {
                    this.Refresh();
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!CustomWindow.InUse)
            {
                Program.Debug.WriteLine(string.Format("OnKeyDown: {0},{1},{2},{3}", (int)e.KeyCode, (int)e.KeyData, (int)e.KeyValue, (int)e.Modifiers));
                if (!_activePanel.HandleKeyDown(e))
                {
                    _keyboardControl.HandleKeyDown(e);
                }
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (!CustomWindow.InUse)
            {
                Program.Debug.WriteLine(string.Format("OnKeyUp: {0},{1},{2},{3}", (int)e.KeyCode, (int)e.KeyData, (int)e.KeyValue, (int)e.Modifiers));
                if (!_activePanel.HandleKeyUp(e))
                {
                    _keyboardControl.HandleKeyUp(e);
                }
            }
        }
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            if (!CustomWindow.InUse)
            {
                Program.Debug.WriteLine(string.Format("OnKeyPress: {0}", (int)e.KeyChar));
                if (!_activePanel.HandleKeyPress(e))
                {
                    _keyboardControl.HandleKeyPress(e);
                }
            }
        }
        public KeyboardControl Keyboard
        {
            get { return _keyboardControl; }
        }
    }
}