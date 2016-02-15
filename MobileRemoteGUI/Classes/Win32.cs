using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace MobileSRC.MobileRemote
{
    internal static class Win32
    {
        public struct BlendFunction
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }

        public enum BlendOperation : byte
        {
            AC_SRC_OVER = 0x00
        }

        public enum BlendFlags : byte
        {
            Zero = 0x00
        }

        public enum SourceConstantAlpha : byte
        {
            Transparent = 0x00,
            Opaque = 0xFF
        }

        public enum AlphaFormat : byte
        {
            AC_SRC_ALPHA = 0x01
        }

        [DllImport("coredll.dll")]
        extern public static Int32 AlphaBlend(IntPtr hdcDest, Int32 xDest, Int32 yDest, Int32 cxDest, Int32 cyDest, IntPtr hdcSrc, Int32 xSrc, Int32 ySrc, Int32 cxSrc, Int32 cySrc, BlendFunction blendFunction);

        [DllImport("coredll.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern Int32 SendMessage(IntPtr hwnd, Int32 wMsg, Int32 wParam, Int32 lParam);

        public static readonly int LVM_FIRST = 0x1000;
        public static readonly int LVM_SETICONSPACING = LVM_FIRST + 53;

        [DllImport("coredll.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);

        [DllImport("coredll.dll")]
        public static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, int dwflags, IntPtr lParam);

        [DllImport("coredll.dll", EntryPoint = "FindWindowW", SetLastError = true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("coredll.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, WindowShowStyle nCmdShow);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("coredll.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("coredll.dll")]
        public static extern bool GetMenuItemRect(IntPtr hWnd, IntPtr hMenu, uint uItem, out RECT lpRect);

        public enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>Enumeration of the different ways of showing a window using 
        /// ShowWindow</summary>
        public enum WindowShowStyle : uint
        {
            /// <summary>Hides the window and activates another window.</summary>
            /// <remarks>See SW_HIDE</remarks>
            Hide = 0,
            /// <summary>Activates and displays a window. If the window is minimized 
            /// or maximized, the system restores it to its original size and 
            /// position. An application should specify this flag when displaying 
            /// the window for the first time.</summary>
            /// <remarks>See SW_SHOWNORMAL</remarks>
            ShowNormal = 1,
            /// <summary>Activates the window and displays it as a minimized window.</summary>
            /// <remarks>See SW_SHOWMINIMIZED</remarks>
            ShowMinimized = 2,
            /// <summary>Activates the window and displays it as a maximized window.</summary>
            /// <remarks>See SW_SHOWMAXIMIZED</remarks>
            ShowMaximized = 3,
            /// <summary>Maximizes the specified window.</summary>
            /// <remarks>See SW_MAXIMIZE</remarks>
            Maximize = 3,
            /// <summary>Displays a window in its most recent size and position. 
            /// This value is similar to "ShowNormal", except the window is not 
            /// actived.</summary>
            /// <remarks>See SW_SHOWNOACTIVATE</remarks>
            ShowNormalNoActivate = 4,
            /// <summary>Activates the window and displays it in its current size 
            /// and position.</summary>
            /// <remarks>See SW_SHOW</remarks>
            Show = 5,
            /// <summary>Minimizes the specified window and activates the next 
            /// top-level window in the Z order.</summary>
            /// <remarks>See SW_MINIMIZE</remarks>
            Minimize = 6,
            /// <summary>Displays the window as a minimized window. This value is 
            /// similar to "ShowMinimized", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWMINNOACTIVE</remarks>
            ShowMinNoActivate = 7,
            /// <summary>Displays the window in its current size and position. This 
            /// value is similar to "Show", except the window is not activated.</summary>
            /// <remarks>See SW_SHOWNA</remarks>
            ShowNoActivate = 8,
            /// <summary>Activates and displays the window. If the window is 
            /// minimized or maximized, the system restores it to its original size 
            /// and position. An application should specify this flag when restoring 
            /// a minimized window.</summary>
            /// <remarks>See SW_RESTORE</remarks>
            Restore = 9,
            /// <summary>Sets the show state based on the SW_ value specified in the 
            /// STARTUPINFO structure passed to the CreateProcess function by the 
            /// program that started the application.</summary>
            /// <remarks>See SW_SHOWDEFAULT</remarks>
            ShowDefault = 10,
            /// <summary>Windows 2000/XP: Minimizes a window, even if the thread 
            /// that owns the window is hung. This flag should only be used when 
            /// minimizing windows from a different thread.</summary>
            /// <remarks>See SW_FORCEMINIMIZE</remarks>
            ForceMinimized = 11
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            short dmOrientation;
            short dmPaperSize;
            short dmPaperLength;
            short dmPaperWidth;
            short dmScale;
            short dmCopies;
            short dmDefaultSource;
            short dmPrintQuality;
            short dmColor;
            short dmDuplex;
            short dmYResolution;
            short dmTTOption;
            short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmDisplayOrientation;
        }
        public enum RotationMode
        {
            DMDO_0 = 0,
            DMDO_90 = 1,
            DMDO_180 = 2,
            DMDO_270 = 4
        }
        [FlagsAttribute]
        public enum FormatMessageFlags : int
        {
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000
        }

        public const int GWL_WNDPROC = -4;
        public const uint WM_HOTKEY = 0x0312;

        [DllImport("coredll.dll")]
        public extern static IntPtr SetWindowLong(IntPtr hwnd,
        int nIndex,
        IntPtr dwNewLong);

        public static string FormatWin32Message(int win32ErrorCode)
        {
            StringBuilder sbMsg = new StringBuilder(1024);

            if (FormatMessage(FormatMessageFlags.FORMAT_MESSAGE_FROM_SYSTEM, IntPtr.Zero, win32ErrorCode, 0, sbMsg, sbMsg.Capacity, null) != 0)
            {
                return sbMsg.ToString();
            }
            else
            {
                return "Unrecognized Win32 error code";
            }
        }

        [DllImport("coredll", EntryPoint = "FormatMessageW", SetLastError = true)]
        public extern static int FormatMessage(FormatMessageFlags dwFlags, IntPtr lpSource,
            int dwMessageId, int dwLanguageId, System.Text.StringBuilder lpBuffer, int nSize, IntPtr[] Arguments);

        [DllImport("coredll.dll")]
        public extern static int CallWindowProc(IntPtr lpPrevWndFunc,
        IntPtr hwnd,
        uint msg,
        uint wParam,
        int lParam);

        [DllImport("coredll.dll")]
        public extern static int PostMessage(IntPtr hwnd,
        uint msg,
        uint wParam,
        uint lParam);

        public delegate int WndProcDelegate(IntPtr hwnd, uint msg, uint wParam, int lParam);

        public static readonly uint MAPVK_VK_TO_VSC = 0x00;
        public static readonly uint MAPVK_VSC_TO_VK = 0x01;
        public static readonly uint MAPVK_VK_TO_CHAR = 0x02;
        public static readonly uint MAPVK_VSC_TO_VK_EX = 0x03;
        public static readonly uint MAPVK_VK_TO_VSC_EX = 0x04;
    }
}
