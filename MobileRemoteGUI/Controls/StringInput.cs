using System;
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
    internal partial class StringInput : CustomWindow
    {
        public StringInput(string title, string description)
        {
            InitializeComponent();
            this.ShowOKButton = true;
            this.ShowCancelButton = true;

            this.Text = title;
            _stringLabel.Text = description;
        }

        public override void SetFocus()
        {
            _stringTextBox.Focus();
        }

        public string Value
        {
            get
            {
                return _stringTextBox.Text;
            }
        }
    }
}