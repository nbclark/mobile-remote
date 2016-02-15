using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MobileSRC.MobileRemote
{
    internal static class L2CAPAPI
    {
        public struct btCONFIGEXTENSION
        {
            public byte type;
            public byte length;
            //public fixed byte ucData[1];
        }

        public struct btFLOWSPEC
        {
            public ushort flags;
            public ushort service_type;
            public uint token_rate;
            public uint token_bucket_size;
            public uint peak_bandwidth;
            public uint latency;
            public uint delay_variation;
        }

        public enum RadioMode
        {
            Off = 0,
            Connectable = 1,
            Discoverable = 2
        }

        [DllImport("coredll.dll")]
        public static extern IntPtr ImmGetContext(IntPtr hWnd);
        [DllImport("coredll.dll")]
        public static extern Boolean ImmSetOpenStatus(IntPtr hWnd, Boolean fOpen);
        [DllImport("coredll.dll")]
        public static extern Int32 ImmEscape(IntPtr hKL, IntPtr hIMC, UInt32 uEscape, IntPtr lpData);
        [DllImport("coredll.dll", EntryPoint = "GetFocus")]
        public static extern IntPtr GetFocus();
        [DllImport("coredll.dll", EntryPoint = "GetKeyState")]
        public static extern short GetKeyState(int nVirtKey);

        public static readonly UInt32 IME_ESC_SET_MODE = 0x0800;
        public static readonly UInt32 IME_ESC_RETAIN_MODE_ICON = 0x0800 + 4;
        public static readonly IntPtr IM_NUMBERS = (IntPtr)2;
        public static readonly IntPtr IM_SPELL = (IntPtr)0;
        [DllImport("BthUtil.dll")]
        public static extern int BthGetMode(out RadioMode dwMode);
        [DllImport("BthUtil.dll")]
        public static extern int BthSetMode(RadioMode dwMode);
        [DllImport("btdrt.dll", SetLastError = true)]
        public static extern int BthRemoteNameQuery(ref ulong pba, int cBuffer, out int pcRequired, byte[] szString);
        [DllImport("mobileremoteapi.dll")]
        public static extern bool _ImmSetOpenStatus(IntPtr hIMC, bool fOpen);
        [DllImport("mobileremoteapi.dll")]
        public static extern IntPtr _ImmGetContext(IntPtr hWnd);
        [DllImport("mobileremoteapi.dll")]
        public static extern long _ImmEscape(IntPtr hKL, IntPtr hIMC, uint uiEscape, IntPtr lpVoid);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPLoad();
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPUnload();
        [DllImport("mobileremoteapi.dll")]
        public static extern int UnregisterServices(ref UInt64 pba);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPConnect(ref UInt64 pba, ushort usPSM, ushort usInMTU, ref ushort pCID, ref ushort pusOutMTU);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPAccept(ushort usPSM, ref UInt64 pba, ref ushort pusCID, ref ushort pusOutMTU);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPListen(ushort usPSM, ushort usInMTU);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPWrite(ushort cid, uint cBuffer, byte[] pBuffer);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPRead(ushort cid, uint cBuffer, ref uint pRequired, byte[] pBuffer);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPCloseCID(ushort cid);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPClosePSM(ushort psm);
       // [DllImport("mobileremoteapi.dll")]
        //public static extern int L2CAPConfigReq(ushort usCID, ushort usInMTU, ushort usOutFlushTO, btFLOWSPEC* pOutFlow, int cOptNum, btCONFIGEXTENSION** ppExtendedOptions);
        [DllImport("mobileremoteapi.dll")]
        public static extern int L2CAPPing(ref Int64 pbt, uint cBufferIn, byte[] pBufferIn, ref uint pcBufferOut, byte[] pBufferOut);
        [DllImport("mobileremoteapi.dll")]
        public static extern int SDPRegister();
        [DllImport("mobileremoteapi.dll")]
        public static extern int SDPCleanup();
    }

    internal class BluetoothHidWriter : IDisposable
    {
        private ushort _cidCtrl = 0, _cidIntr = 0;
        private Thread _intrThread = null, _ctrlThread = null;
        private int _state = 0;
        static byte[] keycode2hidp = new byte[]{
0,41,30,31,32,33,34,35,36,37,38,39,45,46,42,43,20, //16
26,8,21,23,28,24,12,18,19,47,48,40,224,4,22,7, // 32
9,10,11,13,14,15,51,52,53,225,49,29,27,6,25,5, // 48
17,16,54,55,56,229,85,226,44,57,58,59,60,61,62,63, // 64
64,65,66,67,83,71,95,96,97,86,92,93,94,87,89,90, // 80
91,98,99,0,148,100,68,69,135,146,147,138,136,139,140,88, // 96
228,84,70,230,0,74,82,75,80,79,77,81,78,73,76, // 112
0,127,129,128,102,103,0,72,0,133,144,145,137,227,231,101,120,
121,118,122,119,124,116,125,126,123,117,0,251,0,248,0,
0,0,0,0,0,0,240,0,249,0,0,0,0,0,241,242,
0,236,0,235,232,234,233,0,0,0,0,0,0,250,0,0,247,
245,246,0,0,0,0,104,105,106,107,108,109,110,111,112,113,
114,115,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };
        
        public static byte[] asciiMap = new byte[]
{
        0x03,
        0x1E,
        0x30,
        0x46,
        0x20,
        0x12,
        0x21,
        0x22,
        0x0E,
        0x0F,
        0x1C,
        0x25,
        0x26,
        0x1C,
        0x31,
        0x18,
        0x19,
        0x10,
        0x13,
        0x1F,
        0x14,
        0x16,
        0x2F,
        0x11,
        0x2D,
        0x15,
        0x2C,
        0x1A,
        0x2B,
        0x1B,
        0x07,
        0x0C,
        0x39,
        0x02,
        0x28,
        0x04,
        0x05,
        0x06,
        0x08,
        0x28,
        0x0A,
        0x0B,
        0x09,
        0x0D,
        0x33,
        0x0C,
        0x34,
        0x35,
        0x0B,
        0x02,
        0x03,
        0x04,
        0x05,
        0x06,
        0x07,
        0x08,
        0x09,
        0x0A,
        0x27,
        0x27,
        0x33,
        0x0D,
        0x34,
        0x35,
        0x03,
        0x1E,
        0x30,
        0x2E,
        0x20,
        0x12,
        0x21,
        0x22,
        0x23,
        0x17,
        0x24,
        0x25,
        0x26,
        0x32,
        0x31,
        0x18,
        0x19,
        0x10,
        0x13,
        0x1F,
        0x14,
        0x16,
        0x2F,
        0x11,
        0x2D,
        0x15,
        0x2C,
        0x1A,
        0x2B,
        0x1B,
        0x07,
        0x0C,
        0x29,
        0x1E,
        0x30,
        0x2E,
        0x20,
        0x12,
        0x21,
        0x22,
        0x23,
        0x17,
        0x24,
        0x25,
        0x26,
        0x32,
        0x31,
        0x18,
        0x19,
        0x10,
        0x13,
        0x1F,
        0x14,
        0x16,
        0x2F,
        0x11,
        0x2D,
        0x15,
        0x2C,
        0x1A,
        0x2B,
        0x1B,
        0x29,
        0x0E,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00,
        0x00
        };

        public static ushort L2CAP_PSM_HIDP_CTRL = 0x11;
        public static ushort L2CAP_PSM_HIDP_INTR = 0x13;
        public static ushort ERROR_SUCCESS = 0;

        public event EventHandler Connected, Disconnected;

        private UInt64 _bluetoothAddress = 0;
        private bool _sdpRegistered = false, _l2capLoaded = false, _connected = false;
        private Thread _connectThread = null;
        private ConnectThreadClass _connectorClass = null;
        private RecorderList _macroBytes = null;
        private bool _isRecording = false;
        private DateTime _recordingStart = DateTime.MinValue;

        public BluetoothHidWriter()
        {
            Initialize();
        }

        public UInt64 BluetoothAddress
        {
            get { return _bluetoothAddress; }
        }

        public bool IsConnected
        {
            get { return _connected; }
        }

        internal class RecorderList : List<KeyValuePair<TimeSpan, byte[]>>
        {
            public byte[] Serialize()
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter bw = new BinaryWriter(ms))
                    {
                        bw.Write(this.Count);

                        foreach (KeyValuePair<TimeSpan, byte[]> key in this)
                        {
                            // write the timespan
                            bw.Write(key.Key.Ticks);

                            // write the data length
                            bw.Write(key.Value.Length);

                            // write the data
                            bw.Write(key.Value);
                        }
                        return ms.ToArray();
                    }
                }
            }

            public void Deserialize(BinaryReader br)
            {
                int count = br.ReadInt32();

                for (int i = 0; i < count; ++i)
                {
                    TimeSpan ts = new TimeSpan(br.ReadInt64());
                    int bufferLength = br.ReadInt32();
                    byte[] entryData = br.ReadBytes(bufferLength);

                    this.Add(new KeyValuePair<TimeSpan, byte[]>(ts, entryData));
                }
            }
        }

        public void StartRecorder()
        {
            _macroBytes = new RecorderList();
            _recordingStart = DateTime.Now;
            _isRecording = true;
        }

        public RecorderList StopRecorder()
        {
            return _macroBytes;
        }

        public void ExecuteRecorder(RecorderList data, int speedMultiplier)
        {
            TimeSpan previousTimeSpan = TimeSpan.Zero;
            foreach (KeyValuePair<TimeSpan, byte[]> item in data)
            {
                TimeSpan key = item.Key;
                if (previousTimeSpan != TimeSpan.Zero)
                {
                    Thread.Sleep((int)((key - previousTimeSpan).TotalMilliseconds / speedMultiplier));
                }
                WriteRawData(_cidIntr, (uint)item.Value.Length, item.Value);

                previousTimeSpan = key;
            }
        }

        private void Initialize()
        {
            _sdpRegistered = _l2capLoaded = false;
            int iErr = 0;

            try
            {
                iErr = L2CAPAPI.SDPRegister();

                if (0 == iErr)
                {
                    _sdpRegistered = true;
                    int result = 0;

                    if (0 == (result = L2CAPAPI.L2CAPLoad()))
                    {
                        _l2capLoaded = true;
                    }
                    else if (0 == (result = L2CAPAPI.L2CAPLoad()))
                    {
                        _l2capLoaded = true;
                    }
                    else
                    {
                        iErr = result;
                        Program.Debug.WriteError("L2CAPLoad failed", result);
                    }
                }
                else
                {
                    Program.Debug.WriteError("SDPRegister failed", iErr);
                }
            }
            catch (Exception e)
            {
                Program.Debug.WriteError("Initialize failed: " + e.ToString(), 0);
            }
            if (!_l2capLoaded)
            {
                // we failed to load our driver here...
                MessageBox.Show("Failed to load driver: " + iErr);
            }
        }

        public WaitHandle ConnectAsync(UInt64 bluetoothAddress)
        {
            lock (this)
            {
                if (null != _connectThread)
                {
                    Disconnect();
                    if (null != _connectThread)
                    {
                        _connectThread.Abort();
                        _connectThread = null;
                    }
                }
            }

            _connectorClass = new ConnectThreadClass(this, bluetoothAddress);
            _connectThread = new Thread(new ThreadStart(_connectorClass.ConnectThread));
            _connectThread.IsBackground = true;

            _connectThread.Start();

            return _connectorClass.WaitHandle;
        }

        private class ConnectThreadClass
        {
            private UInt64 _bluetoothAddress;
            private ManualResetEvent _waitHandle = new ManualResetEvent(false);
            private BluetoothHidWriter _hidWriter;

            public ConnectThreadClass(BluetoothHidWriter hidWriter, UInt64 bluetoothAddress)
            {
                _hidWriter = hidWriter;
                _bluetoothAddress = bluetoothAddress;
            }

            public WaitHandle WaitHandle
            {
                get { return _waitHandle; }
            }

            public void ConnectThread()
            {
                _hidWriter.Connect(_bluetoothAddress);
                _waitHandle.Set();
            }
        }

        public bool Connect(UInt64 bluetoothAddress)
        {
            if (_connected)
            {
                Disconnect();
            }

            Program.Debug.WriteLine("Connecting to: " + bluetoothAddress);

            //_connected = true;
            //return true;

            _bluetoothAddress = bluetoothAddress;

            int iErr = 0;
            ushort outmtu = 0, intrCid = 0, ctrlCid = 0;

            _connected = false;

            if (0 != _bluetoothAddress)
            {
                UInt64 ba = _bluetoothAddress;

                Program.Debug.WriteLine("Connect CTRL: " + bluetoothAddress);
                if (0 == (iErr = L2CAPAPI.L2CAPConnect(ref ba, L2CAP_PSM_HIDP_CTRL, 48, ref ctrlCid, ref outmtu)))
                {
                    Program.Debug.WriteLine("Connect INTR: " + bluetoothAddress);
                    if (0 == (iErr = L2CAPAPI.L2CAPConnect(ref ba, L2CAP_PSM_HIDP_INTR, 48, ref intrCid, ref outmtu)))
                    {
                        _connected = true;
                    }
                    else
                    {
                        Program.Debug.WriteError("Connect INTR failed", iErr);
                    }
                }
                else
                {
                    Program.Debug.WriteError("Connect CTRL failed", iErr);
                }
            }
            else
            {
                if (!_connected)
                {
                    _connected = false;

                    UInt64 ba = 0;
                    if (0 == (iErr = L2CAPAPI.L2CAPListen(L2CAP_PSM_HIDP_CTRL, 48)))
                    {
                        if (0 == (iErr = L2CAPAPI.L2CAPListen(L2CAP_PSM_HIDP_INTR, 48)))
                        {
                            if (0 == (iErr = L2CAPAPI.L2CAPAccept(L2CAP_PSM_HIDP_CTRL, ref ba, ref ctrlCid, ref outmtu)))
                            {
                                if (0 == (iErr = L2CAPAPI.L2CAPAccept(L2CAP_PSM_HIDP_INTR, ref ba, ref intrCid, ref outmtu)))
                                {
                                    _connected = true;
                                    _bluetoothAddress = ba;
                                }
                                else
                                {
                                    Program.Debug.WriteError("Accept INTR failed", iErr);
                                }
                            }
                            else
                            {
                                Program.Debug.WriteError("Accept CTRL failed", iErr);
                            }
                        }
                        else
                        {
                            Program.Debug.WriteError("Listen CTRL failed", iErr);
                        }
                    }
                    else
                    {
                        Program.Debug.WriteError("Listen INTR failed", iErr);
                    }

                    if (!_connected)
                    {
                        Program.Debug.WriteLine("Disconnecting: " + bluetoothAddress);
                        iErr = L2CAPAPI.L2CAPClosePSM(L2CAP_PSM_HIDP_INTR);
                        iErr = L2CAPAPI.L2CAPClosePSM(L2CAP_PSM_HIDP_CTRL);
                    }
                }
            }

            if (_connected)
            {
                Program.Debug.WriteLine("Connected. Launching listeners: " + bluetoothAddress);
                // write our stuff here
                //unsafe
                //{
                //    L2CAPAPI.btFLOWSPEC flowSpec = new L2CAPAPI.btFLOWSPEC();
                //    flowSpec.token_rate = 900;
                //    flowSpec.token_bucket_size = 20;
                //    flowSpec.peak_bandwidth = 900;
                //    flowSpec.latency = 10;
                //    flowSpec.delay_variation = 10;
                //    flowSpec.service_type = 1;

                //    iErr = L2CAPAPI.L2CAPConfigReq(_cidIntr, 48, 0xFF, &flowSpec, 0, null);
                //}

                ThreadStart listenThreadIntr = new ThreadStart(ListenOnIntr);
                ThreadStart listenThreadCtrl = new ThreadStart(ListenOnCtrl);

                _cidIntr = intrCid;
                _cidCtrl = ctrlCid;

                _intrThread = new Thread(listenThreadIntr);
                _intrThread.IsBackground = true;
                _ctrlThread = new Thread(listenThreadCtrl);
                _ctrlThread.IsBackground = true;

                _intrThread.Start();
                _ctrlThread.Start();

                _state = 1;

                Program.Debug.WriteLine("Firing connected event");
                if (null != Connected)
                {
                    Connected(this, null);
                }
            }
            return _connected;
        }

        public int WriteRawData(ushort cid, uint length, byte[] pkg)
        {
            if (0 != cid && _state == 1)
            {
                if (_isRecording)
                {
                    // TODO: need to have higher resolution than milliseconds...perhaps have a list of byte[]'s
                    TimeSpan timeSpan = DateTime.Now.Subtract(_recordingStart);
                    //while (timeSpan.Ticks == 0 || _macroBytes.ContainsKey(timeSpan))
                    //{
                    //    timeSpan = timeSpan.Add(new TimeSpan(0, 0, 0, 0, 1));
                    //}
                    try
                    {
                        Program.Debug.WriteLine(pkg[4].ToString());
                        _macroBytes.Add(new KeyValuePair<TimeSpan, byte[]>(timeSpan, (byte[])pkg.Clone()));
                    }
                    catch (Exception e)
                    {
                        Program.Debug.WriteLine(e.Message);
                        Program.Debug.WriteLine(e.StackTrace);
                        Program.Debug.WriteLine(timeSpan.ToString());

                        foreach (KeyValuePair<TimeSpan, byte[]> key in _macroBytes)
                        {
                            Program.Debug.WriteLine(key.Key.ToString());
                        }
                    }
                }
                return L2CAPAPI.L2CAPWrite(cid, length, pkg);
            }
            return -1;
        }

        public void SendMouseData(MouseButtons buttons, int dx, int dy)
        {
            if (0 != _cidIntr && _state == 1)
            {
                byte[] pkg = new byte[10];

                int iErr = 0;

                pkg[0] = 0xa1; // send a report
                pkg[1] = 0x02; // mouse report id
                pkg[2] = 0x00; // buttons

                if (0 != (buttons & MouseButtons.Left))
                {
                    pkg[2] |= 1;
                }
                if (0 != (buttons & MouseButtons.Right))
                {
                    pkg[2] |= 2;
                }
                int displacement = (dx << 12) | (dy);
                pkg[3] = (byte)((displacement >> 16) & 0xFF); // x displacement
                pkg[4] = (byte)((displacement >> 8) & 0xFF); // y displacement
                pkg[5] = (byte)((displacement & 0xFF) >> 0); // z displacement
                pkg[3] = (byte)(dx & 0xFF);
                pkg[4] = (byte)(((dx & 0xF00) >> 8) | ((dy & 0x00F) << 4));
                pkg[5] = (byte)((dy & 0xFF0) >> 4);
                pkg[6] = 0; // scroll

                iErr = WriteRawData(_cidIntr, 9, pkg);
            }
        }

        public void SendScroll(int dx, int dy)
        {
            if (0 != _cidIntr && _state == 1 && dy != 0)
            {
                byte[] pkg = new byte[10];

                int iErr = 0;

                pkg[0] = 0xa1; // send a report
                pkg[1] = 0x02; // mouse report id
                pkg[2] = 0; // buttons
                pkg[3] = 0; // x displacement
                pkg[4] = 0; // y displacement
                pkg[5] = 0;
                pkg[6] = (byte)((dy > 0) ? -1 : 1);

                iErr = WriteRawData(_cidIntr, 9, pkg);

                //System.Threading.Thread.Sleep(1000);

                //pkg[0] = 0xa1; // send a report
                //pkg[1] = 0x02; // mouse report id
                //pkg[2] = 0x00; // buttons
                //pkg[3] = 0; // x displacement
                //pkg[4] = 0; // y displacement
                //pkg[5] = 0;


                //iErr = L2CAPAPI.L2CAPWrite(_cidIntr, 10, pkg);
            }
        }

        public enum KeyModifiers
        {
            LCTRL = 0x01,
            LSHIFT = 0x02,
            LALT = 0x04,
            LGUI = 0x08,
            RCTRL = 0x10,
            RSHIFT = 0x20,
            RALT = 0x40,
            RGUI = 0x80,
        }

        public void SetModifierState(byte modifierState)
        {
            if (0 != _cidIntr && _state == 1)
            {
                byte[] pkg = new byte[10];
                int iErr = 0;

                // this is for the mouse -- it works
                pkg[0] = 0xa1; // send a report
                pkg[1] = 0x01; // kybd report id
                pkg[2] = modifierState; // modifiers
                pkg[3] = 0; // keycode
                pkg[4] = 0x00; // 
                pkg[5] = 0x00; // 

                int retVal = iErr = WriteRawData(_cidIntr, 10, pkg);
            }
        }

        public void SendKeyPress(byte modifierState, Classes.HidKeys hidKey)
        {
            try
            {
                SendReportKey(0x01, modifierState, modifierState, (byte)Classes.HidCodes.GetHidCode(hidKey));
            }
            catch
            {
            }
            return;
            /*
            char blahChar = keyChar;

            //#define KEY_LCTRL     0x01  //HID Usage ID: E0|Modifier
            //#define KEY_LSHIFT    0x02  //HID Usage ID: E1|Modifier
            //#define KEY_LALT      0x04  //HID Usage ID: E2|Modifier
            //#define KEY_LGUI      0x08  //HID Usage ID: E3|Modifier
            //#define KEY_RCTRL     0x10  //HID Usage ID: E4|Modifier
            //#define KEY_RSHIFT    0x20  //HID Usage ID: E5|Modifier
            //#define KEY_RALT      0x40  //HID Usage ID: E6|Modifier
            //#define KEY_RGUI      0x80  //HID Usage ID: E7|Modifier

            //if (0 != _cidIntr && _state == 1)
            {
                byte asciiChar = (byte)keyChar;

                if (!isKeyCode)
                {
                    if (asciiMap.Length > keyChar)
                    {
                        asciiChar = asciiMap[keyChar];
                    }
                    else
                    {
                        return;
                    }
                }
                if (keycode2hidp.Length > asciiChar)
                {
                    int code2 = Classes.HidCodes.GetHidCode(Keys.A);
                    bool x;
                    int code1 = Classes.HidCodes.GetHidCode(keyChar, out x);
                    SendReportKey(0x01, modifierState, modifierState, keycode2hidp[asciiChar]);
                }
            }
            */
        }

        public enum MediaKeys
        {
            Play = 0xCD,
            Next = 0xB5,
            Previous = 0xB6,
            Mute = 0xE2,
            VolumeUp = 0xE9,
            VolumeDown = 0xEA,
            ChannelUp = 0x9C,
            ChannelDown = 0x9D,
            Record = 0xB2
        }

        public void SendMediaKey(MediaKeys mediaKey)
        {
            SendReportKey(0x03, (byte)mediaKey, 0, 0);
        }

        protected void SendReportKey(byte reportId, byte preModifierState, byte postModifierState, byte keyCode)
        {
            if (0 != _cidIntr && _state == 1)
            {
                byte[] pkg = new byte[10];
                int iErr = 0;

                // this is for the mouse -- it works
                pkg[0] = 0xa1;
                pkg[1] = reportId;
                pkg[2] = preModifierState;
                pkg[3] = 0;
                pkg[4] = keyCode;
                pkg[5] = 0x00; 
                pkg[6] = 0x00; 
                pkg[7] = 0x00; 
                pkg[8] = 0x00; 
                pkg[9] = 0x00;

                int retVal = iErr = WriteRawData(_cidIntr, 10, pkg);

                pkg[0] = 0xa1;
                pkg[1] = reportId;
                pkg[2] = postModifierState;
                pkg[3] = 0x00;
                pkg[4] = 0x00;
                pkg[5] = 0x00;
                pkg[6] = 0x00; 
                pkg[7] = 0x00; 
                pkg[8] = 0x00; 
                pkg[9] = 0x00;

                retVal = iErr = WriteRawData(_cidIntr, 10, pkg);
            }
        }

        public void Disconnect()
        {
            _state = 0;

            if (_connected)
            {
                _connected = false;
            }

            L2CAPAPI.L2CAPClosePSM(L2CAP_PSM_HIDP_INTR);
            L2CAPAPI.L2CAPClosePSM(L2CAP_PSM_HIDP_CTRL);

            if (0 != _cidCtrl)
            {
                L2CAPAPI.L2CAPCloseCID(_cidCtrl);
                _cidCtrl = 0;
            }
            if (0 != _cidIntr)
            {
                L2CAPAPI.L2CAPCloseCID(_cidIntr);
                _cidIntr = 0;
            }
            _bluetoothAddress = 0;

            if (null != Disconnected)
            {
                Disconnected(this, null);
            }

            if (null != _intrThread)
            {
                if (Thread.CurrentThread != _intrThread)
                {
                    _intrThread.Abort();
                }
            }
            if (null != _ctrlThread)
            {
                if (Thread.CurrentThread != _ctrlThread)
                {
                    _ctrlThread.Abort();
                }
            }
            lock (this)
            {
                if (null != _connectThread)
                {
                    _connectThread.Abort();
                    _connectThread = null;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            Disconnect();

            if (_l2capLoaded)
            {
                L2CAPAPI.L2CAPUnload();
            }
            if (_sdpRegistered)
            {
                L2CAPAPI.SDPCleanup();
            }
        }

        #endregion

        private void ListenOnIntr()
        {
            Program.Debug.WriteLine("ListenOnIntr started");
            ushort intrCid = _cidIntr;

            for (; ; )
            {
                byte[] ucBuffer = new byte[1024];
                uint cSize = 0;

                Program.Debug.WriteLine("Try Read INTR");
                int iErr = L2CAPAPI.L2CAPRead(intrCid, (uint)ucBuffer.Length, ref cSize, ucBuffer);
                Program.Debug.WriteLine("Read INTR : " + iErr);

                if (iErr != 0)
                {
                    Disconnect();
                    break;
                }
            }
        }

        private void ListenOnCtrl()
        {
            Program.Debug.WriteLine("ListenOnCtrl started");
            ushort ctrlCid = _cidCtrl;

            for (; ; )
            {
                byte[] ucBuffer = new byte[1024];
                uint cSize = 0;

                Program.Debug.WriteLine("Try Read CTRL");
                int iErr = L2CAPAPI.L2CAPRead(ctrlCid, (uint)ucBuffer.Length, ref cSize, ucBuffer);
                Program.Debug.WriteLine(String.Format("Read CTRL : {0} [cSize={1}, ucBuffer[0] = {2}]", iErr, cSize, (cSize > 0) ? ucBuffer[0] : -1));
                if (iErr != 0)
                {
                    Disconnect();
                    break;
                }

                if (cSize > 0 && 0 != (ucBuffer[0] & 0x70))
                {
                    byte[] szBuffer = new byte[1];
                    szBuffer[0] = 0;

                    iErr = L2CAPAPI.L2CAPWrite(ctrlCid, 1, szBuffer);
                    Program.Debug.WriteLine("Write CTRL: Responding to 0x70: " + iErr);

                    _state = 1;
                }
                else if (cSize > 0 && 0 != (ucBuffer[0] & 0x90))
                {
                    byte[] szBuffer = new byte[1];
                    szBuffer[0] = 0;

                    iErr = L2CAPAPI.L2CAPWrite(ctrlCid, 1, szBuffer);
                    Program.Debug.WriteLine("Write CTRL: Responding to 0x90: " + iErr);

                    _state = 1;
                }
                else
                {
                    byte[] szBuffer = new byte[1];
                    szBuffer[0] = 0;

                    iErr = L2CAPAPI.L2CAPWrite(ctrlCid, 1, szBuffer);
                    Program.Debug.WriteLine("Write CTRL: No Response");
                }
            }
        }
    }
}
