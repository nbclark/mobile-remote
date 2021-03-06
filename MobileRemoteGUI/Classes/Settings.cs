﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MobileSRC.MobileRemote
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml.Serialization;

    public enum KeyboardRotation
    {
        ROT_0 = 0,
        ROT_90 = 90,
        ROT_270 = 270
    }
    namespace Properties
    {
        public class Settings
        {
            private static Settings _instance = null;
            private static object _lockObject = new object();
            private static string _settingsFile = string.Empty;

            static Settings()
            {
                _settingsFile = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "mobileSRC"), "mobileremote.xml");
            }

            public static Settings Load()
            {
                return Default;
            }

            public static Settings Default
            {
                get
                {
                    lock (_lockObject)
                    {
                        if (null == _instance)
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                            try
                            {
                                using (FileStream fs = File.OpenRead(_settingsFile))
                                {
                                    _instance = (Settings)serializer.Deserialize(fs);
                                }
                            }
                            catch
                            {
                                _instance = new Settings();
                                _instance.Save();
                            }
                        }
                    }
                    return _instance;
                }
            }

            protected Settings()
            {
                this.ConnectionSettings = new ConnectionSettingCollection();
                this.Mouse_UseAccelerometer = true;
                this.Keyboard_DefaultLayout = "default.kbd";
            }

            public void Save()
            {
                if (!Directory.Exists(Path.GetDirectoryName(_settingsFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(_settingsFile));
                }
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                using (FileStream fs = File.Create(_settingsFile))
                {
                    serializer.Serialize(fs, this);
                }
            }

            public bool ConnectionCompatabilityMode
            {
                get;
                set;
            }

            public string Keyboard_DefaultSIP
            {
                get;
                set;
            }

            public string Keyboard_DefaultLayout
            {
                get;
                set;
            }

            public KeyboardRotation Keyboard_Rotation
            {
                get;
                set;
            }

            public bool Mouse_UseAccelerometer
            {
                get;
                set;
            }

            public ConnectionSettingCollection ConnectionSettings
            {
                get;
                set;
            }
        }
    }

    public class ConnectionSettingCollection : List<ConnectionSettingCollection.ConnectionSetting>
    {
        public ConnectionSettingCollection()
        {
        }

        public ConnectionSetting this[ulong address]
        {
            get
            {
                foreach (ConnectionSettingCollection.ConnectionSetting macroPair in this)
                {
                    if (macroPair.Address == address)
                    {
                        return macroPair;
                    }
                }
                return null;
            }
        }

        public class ConnectionSetting
        {
            public ConnectionSetting()
            {
            }
            public ConnectionSetting(ulong address, string connectMacro, string disconnectMacro, bool legacyMode)
            {
                this.Address = address;
                this.ConnectMacro = connectMacro;
                this.DisconnectMacro = disconnectMacro;
                this.LegacyMode = legacyMode;
            }
            public ulong Address
            {
                get;
                set;
            }
            public string ConnectMacro
            {
                get;
                set;
            }
            public string DisconnectMacro
            {
                get;
                set;
            }
            public bool LegacyMode
            {
                get;
                set;
            }
            public override string ToString()
            {
                return this.Address.ToString();
            }
        }
    }

    public class StringPairCollection : List<StringPairCollection.StringPair>
    {
        public StringPairCollection()
        {
        }

        public class StringPair
        {
            public StringPair()
            {
            }
            public StringPair(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
            public string Name
            {
                get;
                set;
            }
            public string Value
            {
                get;
                set;
            }
            public override string ToString()
            {
                return this.Name;
            }
        }
    }
}