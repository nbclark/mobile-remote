using System;
using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class CustomMenu : CustomWindow
    {
        private ImageLabel _clickedButton = null;
        private MobileRemoteUI _ui = null;
        private static Font _font = new System.Drawing.Font(FontFamily.GenericSerif, 9F, System.Drawing.FontStyle.Bold);
        private static Bitmap _menuUp = Properties.Resources.menu_up;
        private static Bitmap _menuDown = Properties.Resources.menu_down;
        private static Bitmap _expand = Properties.Resources.menu_expand;

        public CustomMenu()
        {
            InitializeComponent();
            this.WidthPadding = 30;
        }

        public override void SetFocus()
        {
            this._contentPanel.Controls[0].Focus();
        }

        public override bool IsFullScreen
        {
            get
            {
                return true;
            }
        }

        public override bool IsAnimated
        {
            get
            {
                return true;
            }
        }

        public override VerticalAlignment Alignment
        {
            get
            {
                return VerticalAlignment.Bottom;
            }
        }

        private List<ImageLabel> _currentLabels = null;
        private List<ImageLabel> _labels = null;
        private int _selectedIndex = 0;
        private RoundedBar _roundedBar = new RoundedBar();

        private void AddMenuItems(MobileRemoteUI parentForm, Bitmap expand, Bitmap menuDown, Bitmap menuUp, Font font, List<ImageLabel> labels, int y, Menu.MenuItemCollection menuItems, IList container)
        {
            _clickedButton = null;
            foreach (MenuItem menuItem in menuItems)
            {
                // TODO: handle disabled menu items
                ImageLabel label = new ImageLabel(this);
                label.Height = (int)(60 * MobileRemoteUI.ScaleFactor.Height);
                label.Width = this.Width + 4;
                label.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                label.Location = new Point(-2, y);
                label.Click += new EventHandler(label_Click);
                label.Text = menuItem.Text.Replace("&", "");
                label.Font = font;
                label.MenuItem = menuItem;
                label.SelectedImage = menuDown;
                label.RegularImage = menuUp;
                label.ExpandImage = expand;
                label.ItemActivated += new EventHandler(label_ItemActivated);
                label.Tag = menuItem;

                labels.Add(label);

                if (labels != container)
                {
                    container.Add(label);
                }

                y += label.Height + 1;
            }
        }

        public DialogResult ShowDialog(MobileRemoteUI parentForm, Menu menu)
        {
            this._contentPanel.Controls.Clear();
            _labels = new List<ImageLabel>();
            _ui = parentForm;

            //_roundedBar.Dock = DockStyle.Top;
            //this.Controls.Add(_roundedBar);

            AddMenuItems(parentForm, _expand, _menuDown, _menuUp, _font, _labels, 5, menu.MenuItems, this._contentPanel.Controls);
            this._contentPanel.BackColor = Color.Black;

            _currentLabels = _labels;

            HandleSelections(_labels[0]);

            return base.ShowDialog(parentForm, true);
        }

        protected override void AlphaLayerSet(AlphaLayerControl alphaControl)
        {
            _roundedBar.AlphaControl = alphaControl;
        }

        void label_Click(object sender, EventArgs e)
        {
            HandleSelections((ImageLabel)sender);
        }

        void HandleSelections(ImageLabel selectedImage)
        {
            foreach (ImageLabel label in _currentLabels)
            {
                if (selectedImage != label)
                {
                    label.Selected = false;
                }
            }
            selectedImage.Selected = true;
            _selectedIndex = _currentLabels.IndexOf(selectedImage);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            int iDir = 0;
            if (e.KeyCode == Keys.Down)
            {
                iDir = 1;
            }
            if (e.KeyCode == Keys.Up)
            {
                iDir = -1;
            }
            if (e.KeyCode == Keys.Left)
            {
                // do nothing
                // TODO
            }
            if (e.KeyCode == Keys.Right)
            {
                ProcessChildChange(_currentLabels[_selectedIndex]);
            }
            if (iDir != 0)
            {
                _selectedIndex = (_selectedIndex + _currentLabels.Count + iDir) % _currentLabels.Count;
                HandleSelections(_currentLabels[_selectedIndex]);
            }
            base.OnKeyDown(e);
        }

        void label_ItemActivated(object sender, EventArgs e)
        {
            this._clickedButton = (ImageLabel)sender;

            System.Reflection.MethodInfo x = this._clickedButton.MenuItem.GetType().GetMethod("OnPopup", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            x.Invoke(this._clickedButton.MenuItem, new object[] { null });
            if (this._clickedButton.MenuItem.MenuItems.Count > 0)
            {
                ProcessChildChange(this._clickedButton);
                this._clickedButton = null;
            }
            else
            {
                this.Close();
            }
        }

        void ProcessChildChange(ImageLabel parentLabel)
        {
            _currentLabels = new List<ImageLabel>();

            this._contentPanel.SuspendLayout();
            this._contentPanel.Controls.Clear();
            AddMenuItems(_ui, _expand, _menuDown, _menuUp, _font, _currentLabels, 5, parentLabel.MenuItem.MenuItems, this._contentPanel.Controls);

            foreach (ImageLabel button in _currentLabels)
            {
                this._contentPanel.Controls.Add(button);
            }
            ScaleWindow(_ui, true);

            if (_currentLabels.Count > 0)
            {
                HandleSelections(_currentLabels[0]);
            }
            this._contentPanel.ResumeLayout();
            _selectedIndex = 0;
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            if (null != _clickedButton)
            {
                MenuItem menuItem = (MenuItem)_clickedButton.Tag;
                System.Reflection.MethodInfo x = menuItem.GetType().GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                x.Invoke(menuItem, new object[] { null });
            }
        }

        private class ImageLabel : PictureBox
        {
            public string Text;
            public Font Font;
            public Bitmap RegularImage, SelectedImage, ExpandImage;
            private bool _isSelected = false;
            private CustomMenu _parentMenu = null;
            public event EventHandler ItemActivated;
            public MenuItem MenuItem;

            public ImageLabel(CustomMenu parentMenu)
            {
                _parentMenu = parentMenu;
            }

            public bool Selected
            {
                get { return _isSelected; }
                set
                {
                    if (_isSelected != value)
                    {
                        _isSelected = value;

                        if (this.Visible)
                        {
                            if (value)
                            {
                                this.Focus();
                            }
                            this.Refresh();
                        }
                    }
                }
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                this.Focus();
                base.OnClick(e);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                Point p = new Point(e.X, e.Y);

                if (this.ClientRectangle.Contains(p))
                {
                    if (null != ItemActivated)
                    {
                        ItemActivated(this, new EventArgs());
                    }
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                if (Keys.Enter == e.KeyCode)
                {
                    if (null != ItemActivated)
                    {
                        ItemActivated(this, new EventArgs());
                    }
                }
                else
                {
                    _parentMenu.OnKeyDown(e);
                }
            }

            protected override void OnPaintBackground(PaintEventArgs e)
            {
                //base.OnPaintBackground(e);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                Color foreColor = SystemColors.ControlText;
                if (this.Selected)
                {
                    foreColor = SystemColors.HighlightText;
                    e.Graphics.DrawImage(this.SelectedImage, this.ClientRectangle, new Rectangle(0, 0, this.SelectedImage.Width, this.SelectedImage.Height), GraphicsUnit.Pixel);
                }
                else
                {
                    e.Graphics.DrawImage(this.RegularImage, this.ClientRectangle, new Rectangle(0, 0, this.RegularImage.Width, this.RegularImage.Height), GraphicsUnit.Pixel);
                }

                if (this.MenuItem.MenuItems.Count > 0)
                {
                    ImageAttributes attributes = new ImageAttributes();

                    Color clr = this.ExpandImage.GetPixel(0, 0);
                    attributes.SetColorKey(clr, clr);
                    e.Graphics.DrawImage(this.ExpandImage, new Rectangle(this.Right - this.ExpandImage.Width - 5, (this.Height - this.ExpandImage.Height) / 2, this.ExpandImage.Width, this.ExpandImage.Height), 0, 0, this.ExpandImage.Width, this.ExpandImage.Height, GraphicsUnit.Pixel, attributes);
                }

                using (SolidBrush b = new SolidBrush(this.Parent.ForeColor))
                {
                    SizeF size = e.Graphics.MeasureString(this.Text, this.Font);
                    e.Graphics.DrawString(this.Text, this.Font, b, 20, (this.Height - size.Height) / 2);
                }
            }
        }
    }
}