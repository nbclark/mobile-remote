using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Text;

namespace MobileSRC.MobileRemote
{
    internal static class Utils
    {
        private static readonly string SettingsPath = @"HKEY_CURRENT_USER\Software\mobileSRC\MobileRemote";
        private static readonly string RegKey = @"RegistrationCode";
        private static readonly int CValue = 12;
        private static readonly int Variant = 4177;
        private static readonly int CODCompatabilityMode = 9536;
        [StructLayout(LayoutKind.Sequential)]

        internal struct BLUETOOTH_FIND_RADIO_PARAMS
        {
            public int dwSize;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct BLUETOOTH_RADIO_INFO
        {
            internal int dwSize;
            internal long address;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0xf8)]
            internal string szName;
            internal uint ulClassofDevice;
            internal ushort lmpSubversion;
            [MarshalAs(UnmanagedType.U2)]
            internal short manufacturer;
        }
        [StructLayout(LayoutKind.Sequential, Size = 60)]
        internal struct WSAQUERYSET
        {
            public int dwSize;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszServiceInstanceName;
            public IntPtr lpServiceClassId;
            private IntPtr lpVersion;
            private IntPtr lpszComment;
            public int dwNameSpace;
            private IntPtr lpNSProviderId;
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpszContext;
            private int dwNumberOfProtocols;
            private IntPtr lpafpProtocols;
            private IntPtr lpszQueryString;
            public int dwNumberOfCsAddrs;
            public IntPtr lpcsaBuffer;
            private int dwOutputFlags;
            public IntPtr lpBlob;
        }
        internal enum WSAESETSERVICEOP
        {
            RNRSERVICE_REGISTER,
            RNRSERVICE_DEREGISTER,
            RNRSERVICE_DELETE
        }
        [Flags]
        internal enum LookupFlags : uint
        {
            Containers = 2,
            FlushCache = 0x1000,
            ResService = 0x8000,
            ReturnAddr = 0x100,
            ReturnBlob = 0x200,
            ReturnName = 0x10
        }
        public enum SocketType
        {
            Dgram = 2,
            Raw = 3,
            Rdm = 4,
            Seqpacket = 5,
            Stream = 1,
            Unknown = -1
        }
        public enum ProtocolType
        {
            Ggp = 3,
            Icmp = 1,
            Idp = 0x16,
            Igmp = 2,
            IP = 0,
            IPv6 = 0x29,
            Ipx = 0x3e8,
            ND = 0x4d,
            Pup = 12,
            Raw = 0xff,
            Spx = 0x4e8,
            SpxII = 0x4e9,
            Tcp = 6,
            Udp = 0x11,
            Unknown = -1,
            Unspecified = 0
        }

        [DllImport("btdrt.dll", SetLastError = true)]
        public static extern int BthReadLocalAddr(byte[] pba);
        [DllImport("ws2.dll", SetLastError = true)]
        public static extern int WSALookupServiceBegin(ref WSAQUERYSET pQuerySet, LookupFlags dwFlags, out int lphLookup);
        [DllImport("ws2.dll", SetLastError = true)]
        public static extern int WSALookupServiceEnd(int hLookup);
        [DllImport("ws2.dll", SetLastError = true)]
        public static extern int WSALookupServiceNext(int hLookup, LookupFlags dwFlags, ref int lpdwBufferLength, byte[] pResults);
        [DllImport("ws2.dll", SetLastError = true)]
        public static extern int WSASetService(ref WSAQUERYSET lpqsRegInfo, WSAESETSERVICEOP essoperation, int dwControlFlags);
 
        public static ulong GetLocalRadioAddress()
        {
            byte[] addr = new byte[8];
            BthReadLocalAddr(addr);
            return BitConverter.ToUInt64(addr, 0);
        }

        public static void UnregisterServices()
        {
            ulong addr = GetLocalRadioAddress();
            L2CAPAPI.UnregisterServices(ref addr);
        }

        public static string GetOwnerName()
        {
            return Convert.ToString(Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\ControlPanel\Owner", "Name", string.Empty));
        }

        public static int GetRadioCOD()
        {
            return Convert.ToInt32(Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Bluetooth\sys", "COD", 0));
        }

        public static bool GetInCompatabilityMode()
        {
            return (CODCompatabilityMode == GetRadioCOD());
        }

        public static int SetCompatabilityMode()
        {
            int radioCOD = GetRadioCOD();
            if (!GetInCompatabilityMode())
            {
                L2CAPAPI.RadioMode radioMode = L2CAPAPI.RadioMode.Off;
                L2CAPAPI.BthGetMode(out radioMode);

                if (radioMode != L2CAPAPI.RadioMode.Off)
                {
                    L2CAPAPI.BthSetMode(L2CAPAPI.RadioMode.Off);
                }
                try
                {
                    Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Bluetooth\sys", "COD", CODCompatabilityMode);
                    L2CAPAPI.BthSetMode(L2CAPAPI.RadioMode.Discoverable);
                }
                finally
                {
                    //Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Bluetooth\sys", "COD", radioCOD);
                }
            }
            return radioCOD;
        }

        public static void UnsetCompatabilityMode(int oldMode)
        {
            Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Bluetooth\sys", "COD", oldMode);
        }

        public static string GetRegCode()
        {
            string customer = GetOwnerName();
            if (customer.Length > 10)
            {
                customer = customer.Substring(0, 5) + customer.Substring(customer.Length - 5, 5);
            }

            int value = 0;
            for (int i = 0; i < customer.Length; ++i)
            {
                value += (int)customer[i];
            }

            value *= CValue;
            value += Variant;

            return value.ToString();
        }

        public static bool CheckRegistration()
        {
            return CheckRegistration(GetOwnerName(), Convert.ToInt32(Microsoft.Win32.Registry.GetValue(SettingsPath, RegKey, 0)));
        }

        public static bool CheckRegistration(string ownerName)
        {
            return true;
            try
            {
                return CheckRegistration(ownerName, Convert.ToInt32(Microsoft.Win32.Registry.GetValue(SettingsPath, RegKey, 0)));
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckRegistration(int regCode)
        {
            return CheckRegistration(GetOwnerName(), regCode);
        }

        public static bool CheckRegistration(string ownerName, int regCode)
        {
            string customer = ownerName;
            if (customer.Length > 10)
            {
                customer = customer.Substring(0, 5) + customer.Substring(customer.Length - 5, 5);
            }

            int value = 0;
            for (int i = 0; i < customer.Length; ++i)
            {
                value += (int)customer[i];
            }

            value *= CValue;
            value += Variant;

            return (value == regCode);
        }

        public static void SetRegistration(int regCode)
        {
            Microsoft.Win32.Registry.SetValue(SettingsPath, RegKey, regCode, Microsoft.Win32.RegistryValueKind.DWord);
        }
    }
}
