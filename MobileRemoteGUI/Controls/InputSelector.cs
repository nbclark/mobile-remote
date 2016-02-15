using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.WindowsCE.Forms;

namespace MobileSRC.MobileRemote
{
    internal partial class InputControlSelector : CustomWindow
    {
        private int _count = 0;
        public InputControlSelector()
        {
            InitializeComponent();
            this.ShowOKButton = true;
            this.ShowCancelButton = true;

            int buttonSize = MobileRemoteUI.Instance.ButtonSize;

            IEnumerator enumerator = MobileRemoteUI.Instance.Inputs;
            while (enumerator.MoveNext())
            {
                MobileRemoteUI.InputInterfaceControl control = (MobileRemoteUI.InputInterfaceControl)enumerator.Current;
                PushButton button = new PushButton();
                button.DownImage = control.Down;
                button.Image = control.Up;
                button.Name = control.Text;
                button.Text = control.Text;
                button.SelectedImage = control.Sel;
                button.Size = new System.Drawing.Size(buttonSize, buttonSize);
                button.TabIndex = _count;
                button.Click += new EventHandler(button_Click);
                button.Tag = control;

                _layoutPanel.Controls.Add(button);
                _count++;
            }
        }

        void button_Click(object sender, EventArgs e)
        {
            PushButton b = (PushButton)sender;

            this.Close();
            MobileRemoteUI.Instance.SelectInput(b.Tag as MobileRemoteUI.InputInterfaceControl);
        }

        public override void SetFocus()
        {
            _layoutPanel.Controls[0].Focus();
        }

        public override void OnLoad()
        {
            try
            {
                base.OnLoad();
                
                int buttonSize = MobileRemoteUI.Instance.ButtonSize;
                int i = 0, j = 0;
                for (i = 2; i < this._contentPanel.Width / buttonSize; ++i)
                {
                    for (j = 2; j < this._contentPanel.Height / buttonSize; ++j)
                    {
                        if ((j * i) >= _count)
                        {
                            break;
                        }
                    }
                    if ((j * i) >= _count)
                    {
                        break;
                    }
                }
                int height = j * buttonSize;
                int width = i * buttonSize;
                this._layoutPanel.Width = width;
                this._layoutPanel.Left = (this._contentPanel.Width - width) / 2;

                this._layoutPanel.Height = height;
                this._layoutPanel.Top = (this._contentPanel.Height - height) / 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}