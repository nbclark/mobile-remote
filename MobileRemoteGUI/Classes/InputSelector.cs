using System;
using Microsoft.WindowsCE.Forms;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace MobileSRC.MobileRemote
{
    internal class InputSelector
    {
        private static InputPanel _inputPanel = null;

        public static InputMethod CurrentInputMethod
        {
            get { return (null == _inputPanel) ? null : _inputPanel.CurrentInputMethod; }
            set
            {
                if (null != _inputPanel)
                {
                    _inputPanel.CurrentInputMethod = value;
                }
            }
        }

        public static InputPanel.InputMethodCollection InputMethods
        {
            get { return (null == _inputPanel) ? null : _inputPanel.InputMethods; }
        }

        public static Rectangle Bounds
        {
            get { return (null == _inputPanel) ? Rectangle.Empty : _inputPanel.Bounds; }
        }

        public static bool Enabled
        {
            get { return (null == _inputPanel) ? false : _inputPanel.Enabled; }
            set
            {
                if (null != _inputPanel)
                {
                    _inputPanel.Enabled = value;
                }
            }
        }

        static InputSelector()
        {
            try
            {
                _inputPanel = new InputPanel();
            }
            catch
            {
            }
        }
    }
}
