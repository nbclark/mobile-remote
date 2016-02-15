using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MobileSRC.MobileRemote.Inputs
{
    internal class CustomInputControl : InputControl, IInputControl
    {
        private BluetoothHidWriter _hidWriter;

        private class HidButton : ImageButton
        {
            protected Classes.CustomInput.CustomInputControl _controlDesc;
            protected CustomInputControl _parent;

            public HidButton(CustomInputControl parent, Classes.CustomInput.CustomInputControl controlDesc)
            {
                _parent = parent;
                _controlDesc = controlDesc;

                this.CatchKey = controlDesc.CatchKey;
                this.ModifierState = controlDesc.ModifierState;

                this.ButtonClick += new EventHandler(HidButton_ButtonClick);
                this.ButtonDoubleClick += new EventHandler(HidButton_ButtonClick);
                this.ButtonDown += new EventHandler(HidButton_ButtonDown);
                this.ButtonUp += new EventHandler(HidButton_ButtonUp);
                this.ButtonHold += new EventHandler(HidButton_ButtonHold);
            }

            protected BluetoothHidWriter HidWriter
            {
                get { return _parent.HidWriter; }
            }

            protected virtual void HidButton_ButtonHold(object sender, EventArgs e)
            {
                //
            }

            protected virtual void HidButton_ButtonUp(object sender, EventArgs e)
            {
                //
            }

            protected virtual void HidButton_ButtonDown(object sender, EventArgs e)
            {
                //
            }

            protected virtual void HidButton_ButtonClick(object sender, EventArgs e)
            {
                //
            }

            public Classes.CustomInput.CustomInputControl Description
            {
                get
                {
                    return _controlDesc;
                }
            }

            public Classes.HidKeys CatchKey
            {
                get;
                set;
            }
            public byte ModifierState
            {
                get;
                set;
            }
        }

        private class MouseButtonHidButton : HidButton
        {
            public MouseButtonHidButton(CustomInputControl parent, Classes.CustomInput.CustomInputControl controlDesc)
                : base(parent, controlDesc)
            {
                this.CanHold = true;
                this.Image = Properties.Resources.mouse;
                this.DownImage = Properties.Resources.mouse_down;
                this.SelectedImage = Properties.Resources.mouse_sel;
            }
        }

        private class KeyboardHidButton : HidButton
        {
            public KeyboardHidButton(CustomInputControl parent, Classes.CustomInput.CustomInputControl controlDesc)
                : base(parent, controlDesc)
            {
                this.CanHold = false;
            }

            protected override void HidButton_ButtonClick(object sender, EventArgs e)
            {
                this.HidWriter.SendKeyPress(this.Description.ModifierState, this.Description.InputData);
            }
        }

        private class ContainerPanel : Panel
        {
            private Classes.CustomInput.CustomInputControl _desc;
            public ContainerPanel(CustomInputControl parent, Classes.CustomInput.CustomInputControl controlDesc)
            {
                _desc = controlDesc;

                this.Width = controlDesc.Width;
                this.Height = controlDesc.Height;
                this.Left = controlDesc.Left;
                this.Top = controlDesc.Top;

                this.Anchor = controlDesc.Anchor;
                this.Dock = controlDesc.Dock;

                ParseChildren(parent, controlDesc.Controls, this.Controls);
            }

            public void SetAnchor()
            {
                this.Anchor = _desc.Anchor;
                this.Dock = _desc.Dock;

                foreach (Control control in this.Controls)
                {
                    if (control is ContainerPanel)
                    {
                        ((ContainerPanel)control).SetAnchor();
                    }
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                OnResize(e);
            }

            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                if (null == this.Parent)
                {
                    return;
                }
                //if (!MobileRemoteUI.Instance.IsLoaded || !this.Visible)
                //{
                //    return;
                //}

                ////this.SetAnchor();

                if (DockStyle.None == this.Dock)
                {
                    switch (_desc.HAlignment)
                    {
                        case Classes.CustomInputAlignment.TopLeft:
                            {
                                // do nothing
                                this.Left = _desc.Left;
                            }
                            break;
                        case Classes.CustomInputAlignment.Middle:
                            {
                                // do nothing
                                int widthDiff = (this.Parent.Width - this.Width);
                                this.Left = widthDiff / 2;
                            }
                            break;
                    }
                    switch (_desc.VAlignment)
                    {
                        case Classes.CustomInputAlignment.TopLeft:
                            {
                                // do nothing
                                this.Top = _desc.Top;
                            }
                            break;
                        case Classes.CustomInputAlignment.Middle:
                            {
                                // do nothing
                                int widthDiff = (this.Parent.Height - this.Height);
                                this.Top = widthDiff / 2;
                            }
                            break;
                    }
                }
            }
        }

        private static Bitmap BuildCustomBitmap(Bitmap backgroundImage, string displayName, Font font, Color foreColor)
        {
            Bitmap returnBmp = (Bitmap)backgroundImage.Clone();

            using (Graphics g = Graphics.FromImage(returnBmp))
            {
                SizeF size = g.MeasureString(displayName, font);
                using (SolidBrush brush = new SolidBrush(foreColor))
                {
                    g.DrawString(displayName, font, brush, (backgroundImage.Width - size.Width) / 2, (backgroundImage.Height - size.Height) / 2);
                }
            }

            return returnBmp;
        }

        private static Bitmap BuildCustomBitmap(Bitmap backgroundImage, Bitmap foregroundImage)
        {
            Bitmap returnBmp = (Bitmap)backgroundImage.Clone();

            using (Graphics g = Graphics.FromImage(returnBmp))
            {
                g.DrawImage(foregroundImage, (backgroundImage.Width - foregroundImage.Width) / 2, (backgroundImage.Height - foregroundImage.Height) / 2);
            }

            return returnBmp;
        }

        public static Bitmap GetBitmap(string imagePath)
        {
            return new Bitmap(Path.Combine(Path.Combine(Utils.GetWorkingDirectory(), "CustomInputs"), imagePath.Replace("{res}", Platform.IsVGA ? "vga" : "qvga")));
        }

        public CustomInputControl(Classes.CustomInput inputLayout, BluetoothHidWriter hidWriter)
        {
            this.BackColor = SystemColors.Window;
            this.Height = inputLayout.Height;
            this.Width = inputLayout.Width;
            this.HidWriter = hidWriter;

            ParseChildren(this, inputLayout.Controls, this.Controls);
        }

        public static void ParseChildren(CustomInputControl parent, List<Classes.CustomInput.CustomInputControl> controls, ControlCollection controlContainer)
        {
            Bitmap image_down = Properties.Resources.Custom_Button_Down;
            Bitmap image_up = Properties.Resources.Custom_Button_Up;

            foreach (Classes.CustomInput.CustomInputControl controlDesc in controls)
            {
                HidButton imageButton = null;
                Control control = imageButton;

                switch (controlDesc.InputType)
                {
                    case Classes.CustomInputType.Container:
                        {
                            control = new ContainerPanel(parent, controlDesc);
                        }
                        break;
                    case Classes.CustomInputType.KeyCombination:
                    case Classes.CustomInputType.VirtualKey:
                    case Classes.CustomInputType.ScanCode:
                        {
                            control = imageButton = new KeyboardHidButton(parent, controlDesc);

                            if (!string.IsNullOrEmpty(controlDesc.Image))
                            {
                                imageButton.Image = GetBitmap(controlDesc.Image);
                            }
                            else
                            {
                                imageButton.Image = BuildCustomBitmap(image_up, controlDesc.InputDisplay, parent.Font, Color.Black);
                            }
                            if (!string.IsNullOrEmpty(controlDesc.DownImage))
                            {
                                imageButton.DownImage = GetBitmap(controlDesc.DownImage);
                            }
                            else
                            {
                                imageButton.DownImage = BuildCustomBitmap(image_down, controlDesc.InputDisplay, parent.Font, Color.White);
                            }
                            imageButton.SelectedImage = imageButton.DownImage;
                            imageButton.TransparentBackground = controlDesc.TransparentBackground;
                            imageButton.BackColor = parent.BackColor;
                            imageButton.CanHold = false;
                        }
                        break;
                    case Classes.CustomInputType.ShiftKey:
                    case Classes.CustomInputType.AltKey:
                    case Classes.CustomInputType.WinKey:
                        {
                        }
                        break;
                    case Classes.CustomInputType.LeftMouseButton:
                        {
                            imageButton = new MouseButtonHidButton(parent, controlDesc);
                        }
                        break;
                    case Classes.CustomInputType.RightMouseButton:
                        {
                            imageButton = new MouseButtonHidButton(parent, controlDesc);
                        }
                        break;
                    case Classes.CustomInputType.MouseDPad:
                        {
                            // add a control here to map dpad to mouse
                        }
                        break;
                    case Classes.CustomInputType.Accelerometer:
                        {
                            // add a control here to map accelerometer to mouse
                        }
                        break;
                    case Classes.CustomInputType.TouchPad:
                    case Classes.CustomInputType.TouchPadWithScroll:
                        {
                            control = new TouchControl();
                            ((TouchControl)control).HidWriter = parent.HidWriter;
                        }
                        break;
                    default:
                        {
                            // why are we here?
                        }
                        break;
                }
                control.Width = controlDesc.Width;
                control.Height = controlDesc.Height;
                control.Left = controlDesc.Left;
                control.Top = controlDesc.Top;
                control.Tag = controlDesc;

                if (!(control is ContainerPanel))
                {
                    control.Anchor = controlDesc.Anchor;
                    control.Dock = controlDesc.Dock;
                }

                controlContainer.Add(control);
            }
        }

        #region IInputControl Members

        public override bool HandleKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                return true;
            }
            return false;
        }

        private static HidButton FindMatchingButton(ControlCollection controls, KeyEventArgs e)
        {
            foreach (Control c in controls)
            {
                if (c is HidButton)
                {
                    HidButton button = (HidButton)c;
                    if (button.CatchKey == (Classes.HidKeys)e.KeyCode)
                    {
                        return button;
                    }
                }
                else if (c is ContainerPanel)
                {
                    HidButton button = FindMatchingButton(((ContainerPanel)c).Controls, e);

                    if (null != button)
                    {
                        return button;
                    }
                }
            }
            return null;
        }

        private HidButton FindMatchingButton(KeyEventArgs e)
        {
            return FindMatchingButton(this.Controls, e);
        }

        public override bool HandleKeyDown(KeyEventArgs e)
        {
            HidButton button = FindMatchingButton(e);

            if (null != button)
            {
                button.IsSelected = true;
                e.Handled = true;
            }

            return e.Handled;
        }

        public override bool HandleKeyUp(KeyEventArgs e)
        {
            HidButton button = FindMatchingButton(e);

            if (null != button)
            {
                button.IsSelected = false;
                e.Handled = true;
            }

            return e.Handled;
        }

        public System.Windows.Forms.Orientation DesiredOrientation
        {
            get { return Orientation.Vertical; }
        }

        public BluetoothHidWriter HidWriter
        {
            get
            {
                return _hidWriter;
            }
            set
            {
                _hidWriter = value;
            }
        }

        public bool RequiresTouchscreen
        {
            get { return true; }
        }

        #endregion
    }
}
