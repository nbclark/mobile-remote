using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class CustomWindow : Panel
    {
        public enum VerticalAlignment
        {
            Top = 0,
            Bottom = 1,
            Middle = 2
        }

        private DialogResult _dialogResult = DialogResult.None;
        private MainMenu _menu = null;
        private ManualResetEvent _waitHandle = new ManualResetEvent(false);
        private string _text = string.Empty;
        private bool _showMenuBar = true;
        private int _widthPadding = 0;
        private bool _isLoaded = false;
        private static bool _inUse = false;

        public CustomWindow()
        {
            InitializeComponent();
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        public static bool InUse
        {
            get { return _inUse; }
        }

        public bool ShowOKButton
        {
            get { return titleBar1.ShowOKButton; }
            set { titleBar1.ShowOKButton = value; }
        }

        public bool ShowCancelButton
        {
            get { return titleBar1.ShowCancelButton; }
            set { titleBar1.ShowCancelButton = value; }
        }

        public bool ShowMenuBar
        {
            get { return _showMenuBar; }
            set { _showMenuBar = value; }
        }

        public MainMenu Menu
        {
            get { return _menu; }
            set { _menu = value; }
        }

        public DialogResult DialogResult
        {
            get { return _dialogResult; }
            set { _dialogResult = value; }
        }

        protected virtual void OnClose()
        {
            //
        }

        protected virtual void OnClosed()
        {
            //
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
        }

        public void Close()
        {
            OnClose();
            _waitHandle.Set();
        }

        public virtual void SetFocus()
        {
            //
        }

        public virtual void OnLoad()
        {
            //
        }

        public virtual bool IsFullScreen
        {
            get { return false; }
        }

        public virtual bool IsAnimated
        {
            get { return false; }
        }

        public int WidthPadding
        {
            get { return _widthPadding; }
            set { _widthPadding = value; }
        }

        public virtual VerticalAlignment Alignment
        {
            get { return VerticalAlignment.Middle; }
        }

        protected void ScaleWindow(MobileRemoteUI parentForm, bool fitToContents)
        {
            if (fitToContents)
            {
                ScaleToContents();
            }
            else
            {
                this._contentPanel.Dock = DockStyle.None;
                this._contentPanel.Width = this.Width - 4;
                this._contentPanel.Height = this.Height - 4 - ((_showMenuBar) ? this.titleBar1.Height : 0);
                this._contentPanel.Location = new Point(2, ((_showMenuBar) ? this.titleBar1.Bottom : 0) + 2);
                this._contentPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }
            PerformLayout(parentForm);
        }

        protected void ScaleToContents()
        {
            this._contentPanel.Dock = DockStyle.Fill;
            int maxBottom = 0;

            foreach (Control c in this._contentPanel.Controls)
            {
                maxBottom = Math.Max(maxBottom, this._contentPanel.Top + c.Bottom);
            }

            //maxBottom = Math.Max(this._contentPanel.Bottom, maxBottom);

            int yDiff = (this._contentPanel.Bottom - maxBottom) - 1;

            this.Height -= yDiff;
            this.Top += yDiff / 2;
        }

        private Point _endLocation = new Point();
        private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private int _yDiff = 50;

        void _timer_Tick(object sender, EventArgs e)
        {
            int newY = Math.Max(_endLocation.Y, this.Location.Y - _yDiff);
            this.Location = new Point(_endLocation.X, newY);

            if (newY == _endLocation.Y)
            {
                _timer.Enabled = false;
            }
        }

        protected void PerformLayout(MobileRemoteUI parentForm)
        {
            Point location;
            int xStart = (parentForm.Width - this.Width) / 2;
            switch (this.Alignment)
            {
                case VerticalAlignment.Top:
                    {
                        location = new Point(xStart, 0);
                    }
                    break;
                case VerticalAlignment.Bottom:
                    {
                        location = new Point(xStart, parentForm.Bottom - this.Height);
                    }
                    break;
                default :
                    {
                        location = new Point(xStart, parentForm.Height / 2 - this.Height / 2);
                    }
                    break;
            }

            if (this.IsAnimated)
            {
                _endLocation = location;

                _yDiff = (parentForm.Bottom - _endLocation.Y) / 8;

                _timer.Interval = 20;

                this.Location = new Point(xStart, parentForm.Bottom);

                if (_isLoaded)
                {
                    _timer.Enabled = true;
                }
            }
            else
            {
                this.Location = location;
            }
        }

        private bool _alreadySized = false;

        public virtual DialogResult ShowDialog(MobileRemoteUI parentForm)
        {
            return ShowDialog(parentForm, false);
        }

        public virtual DialogResult ShowDialog(MobileRemoteUI parentForm, bool fitToContents)
        {
            // TODO: figure out scaling for this form
            // perhaps have a global xScale and yScale factor

            _inUse = true;

            this.titleBar1.Height = (int)(MobileRemoteUI.ScaleFactor.Height * this.titleBar1.Height);

            if (Platform.IsWindowsMobileStandard)
            {
                this.Dock = DockStyle.Fill;
            }
            else
            {
                this.Height = Math.Min(parentForm.Height, (int)(MobileRemoteUI.ScaleFactor.Height * this.Height));
                this.Width = Math.Min(parentForm.Width - _widthPadding, (int)(MobileRemoteUI.ScaleFactor.Width * (this.titleBar1.Width - _widthPadding)));

                this.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            }

            this.titleBar1.Visible = _showMenuBar;

            if (!_alreadySized)
            {
                _alreadySized = true;
                ScaleWindow(parentForm, fitToContents);
            }

            titleBar1.Text = this.Text;

            using (Graphics gxSrc = parentForm.CreateGraphics())
            {
                Cursor.Current = Cursors.WaitCursor;
                parentForm.SuspendLayout();

                AlphaLayerControl alphaControl = null;
                if (!Platform.IsWindowsMobileStandard)
                {
                    alphaControl = new AlphaLayerControl(gxSrc, parentForm.ClientSize, 50);

                    alphaControl.Location = new Point(0, 0);
                    alphaControl.Size = parentForm.ClientSize;
                    alphaControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                    alphaControl.Click += new EventHandler(alphaControl_Click);

                    parentForm.Controls.Add(alphaControl);
                    parentForm.Controls.SetChildIndex(alphaControl, 0);

                    AlphaLayerSet(alphaControl);
                }
                parentForm.Controls.Add(this);
                parentForm.Controls.SetChildIndex(this, 0);

                //parentForm.ResumeLayout(true);

                Cursor.Current = Cursors.Default;

                this.SetFocus();

                try
                {
                    MainMenu previousMenu = null;
                    if (Platform.IsWindowsMobileStandard)
                    {
                        previousMenu = parentForm.Menu;

                        parentForm.Menu = new MainMenu();
                        parentForm.Menu.MenuItems.Add(new MenuItem());
                        parentForm.Menu.MenuItems.Add(new MenuItem());

                        parentForm.Menu.MenuItems[0].Text = "OK";
                        parentForm.Menu.MenuItems[0].Click += new EventHandler(okButton_Click);
                        parentForm.Menu.MenuItems[1].Text = "Cancel";
                        parentForm.Menu.MenuItems[1].Click += new EventHandler(cancelButton_Click);
                    }
                    try
                    {
                        OnLoad();
                        _isLoaded = true;

                        HookFocus(_contentPanel.Controls);
                        parentForm.ResumeLayout();

                        if (this.IsAnimated)
                        {
                            _timer.Enabled = true;
                        }
                        while (!_waitHandle.WaitOne(20, false))
                        {
                            Application.DoEvents();
                        }
                        _waitHandle.Reset();
                        return this.DialogResult;
                    }
                    finally
                    {
                        if (null != previousMenu)
                        {
                            parentForm.SuspendLayout();
                            parentForm.Menu = previousMenu;
                            parentForm.ResumeLayout();
                        }
                    }
                }
                finally
                {
                    _isLoaded = false;
                    _timer.Enabled = false;

                    parentForm.SuspendLayout();
                    parentForm.Controls.Remove(this);

                    if (null != alphaControl)
                    {
                        parentForm.Controls.Remove(alphaControl);
                    }
                    parentForm.ResumeLayout(true);

                    parentForm.FocusActive();
                    OnClosed();

                    _inUse = false;
                }
            }
        }

        private void HookFocus(ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                if (c.TabStop)
                {
                    c.LostFocus += new EventHandler(c_LostFocus);
                }
                if (c.Controls.Count > 0)
                {
                    HookFocus(c.Controls);
                }
            }
        }

        void c_LostFocus(object sender, EventArgs e)
        {
            if (this._isLoaded)
            {
                if (null == FindFocus(_contentPanel))
                {
                    SetFocus();
                }
            }
        }

        protected virtual void AlphaLayerSet(AlphaLayerControl alphaControl)
        {
        }

        void alphaControl_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void okButton_Click(object sender, EventArgs e)
        {
            titleBar1_Closing(this, new CancelEventArgs(false));
        }

        Control FindFocus(Control c)
        {
            if (c.Focused)
            {
                return c;
            }
            foreach (Control child in c.Controls)
            {
                Control hasFocus = FindFocus(child);

                if (null != hasFocus)
                {
                    return hasFocus;
                }
            }
            return null;
        }

        void cancelButton_Click(object sender, EventArgs e)
        {
            titleBar1_Closing(this, new CancelEventArgs(true));
        }

        void titleBar1_Closing(object sender, CancelEventArgs e)
        {
            if (e.Cancel)
            {
                this.DialogResult = DialogResult.Cancel;
            }
            else
            {
                this.DialogResult = DialogResult.OK;
            }
            Close();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            _waitHandle.Set();
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}