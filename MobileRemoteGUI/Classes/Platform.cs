using System;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;

namespace MobileSRC.MobileRemote
{
    internal static class Platform
    {
        #region Platform Invokes
        private static class NativeMethods
        {
            [DllImport("coredll.dll")]
            private static extern int SystemParametersInfo(uint uiAction, uint uiParam, StringBuilder pvParam, uint fWiniIni);

            private const uint SPI_GETPLATFORMTYPE = 257;
            private const uint SPI_GETOEMINFO = 258;

            private static string GetSystemParameter(uint uiParam)
            {
                StringBuilder sb = new StringBuilder(128);
                if (SystemParametersInfo(uiParam, (uint)sb.Capacity, sb, 0) == 0)
                    throw new ApplicationException("Failed to get system parameter");
                return sb.ToString();
            }

            public static string GetPlatformType()
            {
                return GetSystemParameter(SPI_GETPLATFORMTYPE);
            }

            public static string GetOEMInfo()
            {
                return GetSystemParameter(SPI_GETOEMINFO);
            }

            // Returns true if the device has a cellular radio
            public static bool GetHasRadio()
            {
                return File.Exists(@"\Windows\Phone.dll");
            }
        }
        #endregion

        private static bool? _isWindowsMobileStandard = null;

        // Returns true if the application is running on a
        // desktop version of the Windows operating system
        public static bool IsDesktopWindows
        {
            get { return Environment.OSVersion.Platform != PlatformID.WinCE; }
        }

        // Returns true if the application is running on a
        // device powered by some form of WIndows CE based
        // operating system
        public static bool IsWindowsCE
        {
            get { return Environment.OSVersion.Platform == PlatformID.WinCE; }
        }

        // Returns true if the application is running on a
        // Windows Mobile Standard (i.e. Smartphone) device
        public static bool IsWindowsMobileStandard
        {
            get
            {
                if (!_isWindowsMobileStandard.HasValue)
                {
                    _isWindowsMobileStandard = IsWindowsCE && (NativeMethods.GetPlatformType() == "SmartPhone");
                }
                return _isWindowsMobileStandard.Value;
            }
        }

        // Returns true if the application is running on a
        // Windows Mobile Classic (i.e. Pocket PC without
        // cellphone) device.
        public static bool IsWindowsMobileClassic
        {
            get { return IsWindowsCE && !NativeMethods.GetHasRadio() && (NativeMethods.GetPlatformType() != "PocketPC"); }
        }

        // Returns true if the application is running on a
        // Windows Mobile Professional (i.e. Pocket PC with
        // cellphone) device.
        public static bool IsWindowsMobileProfessional
        {
            get { return IsWindowsCE && NativeMethods.GetHasRadio() && (NativeMethods.GetPlatformType() == "PocketPC"); }
        }

        // Returns true if the application is running
        // within the Device Emulator on a desktop machine
        public static bool IsEmulator
        {
            get { return IsWindowsCE && (NativeMethods.GetOEMInfo() == "Microsoft DeviceEmulator"); }
        }

        public static bool IsVGA
        {
            get
            {
                return
                    (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width >= 480) ||
                    (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height >= 480);
            }
        }
    }
}