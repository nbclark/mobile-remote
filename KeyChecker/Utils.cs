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
        private static readonly int CValue = 47;
        private static readonly int Variant = 4177;

        public static string GetOwnerName()
        {
            return Convert.ToString(Microsoft.Win32.Registry.GetValue(@"HKEY_CURRENT_USER\ControlPanel\Owner", "Name", string.Empty));
        }

        public static int GetRadioCOD()
        {
            return Convert.ToInt32(Microsoft.Win32.Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Bluetooth\sys", "COD", 0));
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
