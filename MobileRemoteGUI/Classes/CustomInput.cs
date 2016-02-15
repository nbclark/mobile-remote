using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote.Classes
{
    public class CustomInput 
    {
        private string _name, _image, _downImage, _selectedImage;
        private int _width, _height;
        private List<CustomInputControl> _controls;
        private CustomInputTargetResolution _resolution = CustomInputTargetResolution.VGA;

        public CustomInput()
        {
            this.Controls = new List<CustomInputControl>();
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public string Image
        {
            get { return _image; }
            set { _image = value; }
        }
        public string DownImage
        {
            get { return _downImage; }
            set { _downImage = value; }
        }
        public string SelectedImage
        {
            get { return _selectedImage; }
            set { _selectedImage = value; }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        public List<CustomInputControl> Controls
        {
            get { return _controls; }
            set { _controls = value; }
        }
        public CustomInputTargetResolution TargetResolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }
        public void ScaleToResolution()
        {
            if ((this.TargetResolution == CustomInputTargetResolution.VGA) != Platform.IsVGA)
            {
                double scaleFactor = (this.TargetResolution == CustomInputTargetResolution.QVGA) ? 2 : 0.5;

                foreach (CustomInputControl control in this.Controls)
                {
                    control.ScaleToResolution(scaleFactor);
                }
            }
        }

        public class CustomInputControl
        {
            private string _name, _inputDisplay;
            private string _image, _downImage, _selectedImage;
            private CustomInputType _inputType;
            private HidKeys _inputData, _catchKey;
            private byte _modifierState;
            private int _top, _left, _width, _height;
            private bool _transparentBackground = true;
            private CustomInputAlignment _hAlignment = CustomInputAlignment.TopLeft;
            private CustomInputAlignment _vAlignment = CustomInputAlignment.TopLeft;

            public CustomInputControl()
            {
                this.Controls = new List<CustomInputControl>();
            }

            public void ScaleToResolution(double scaleFactor)
            {
                this.Width = (int)(this.Width * scaleFactor);
                this.Height = (int)(this.Height * scaleFactor);
                this.Top = (int)(this.Top * scaleFactor);
                this.Left = (int)(this.Left * scaleFactor);

                foreach (CustomInputControl control in this.Controls)
                {
                    control.ScaleToResolution(scaleFactor);
                }
            }

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
            public int Top
            {
                get { return _top; }
                set { _top = value; }
            }
            public int Left
            {
                get { return _left; }
                set { _left = value; }
            }
            public int Width
            {
                get { return _width; }
                set { _width = value; }
            }
            public int Height
            {
                get { return _height; }
                set { _height = value; }
            }
            public CustomInputType InputType
            {
                get { return _inputType; }
                set { _inputType = value; }
            }
            public HidKeys CatchKey
            {
                get { return _catchKey; }
                set { _catchKey = value; }
            }
            public HidKeys InputData
            {
                get { return _inputData; }
                set { _inputData = value; }
            }
            public byte ModifierState
            {
                get { return _modifierState; }
                set { _modifierState = value; }
            }
            public string InputDisplay
            {
                get { return _inputDisplay; }
                set { _inputDisplay = value; }
            }
            public string Image
            {
                get { return _image; }
                set { _image = value; }
            }
            public string DownImage
            {
                get { return _downImage; }
                set { _downImage = value; }
            }
            public string SelectedImage
            {
                get { return _selectedImage; }
                set { _selectedImage = value; }
            }
            public bool TransparentBackground
            {
                get { return _transparentBackground; }
                set { _transparentBackground = value; }
            }
            public List<CustomInputControl> Controls
            {
                get;
                set;
            }
            public AnchorStyles Anchor
            {
                get;
                set;
            }
            public DockStyle Dock
            {
                get;
                set;
            }
            public CustomInputAlignment HAlignment
            {
                get { return _hAlignment; }
                set { _hAlignment = value; }
            }
            public CustomInputAlignment VAlignment
            {
                get { return _vAlignment; }
                set { _vAlignment = value; }
            }
        }
    }

    public enum CustomInputAlignment
    {
        TopLeft,
        Middle
    }
    public enum CustomInputTargetResolution
    {
        VGA,
        QVGA
    }
    public enum CustomInputType
    {
        Container,
        // Mouse Input
        LeftMouseButton,
        RightMouseButton,
        TouchPad,
        TouchPadWithScroll,
        Accelerometer,
        MouseDPad,

        // Media Buttons
        PlayPause,
        Stop,
        Forward,
        Back,
        VolumeUp,
        VolumeDown,
        Mute,

        // Keyboard Functions
        ShiftKey,
        AltKey,
        WinKey,
        ScanCode,
        VirtualKey,
        KeyCombination,
    }

}
