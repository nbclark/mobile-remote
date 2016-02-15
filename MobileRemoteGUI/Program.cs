using System;
using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace MobileSRC.MobileRemote
{
    static class Program
    {
        public static TextTraceListener Debug;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [MTAThread]
        static void Main()
        {
            try
            {
                using (Debug = new TextTraceListener(@"\mobileremote.log"))
                {
                    if (null != Properties.Resources.ResourceManager)
                    {
                        Thread splashThread = new Thread(new ThreadStart(ShowSplash));
                        splashThread.Start();
                        Application.Run(new MobileRemoteUI());
                    }
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                Program.Debug.WriteLine(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error Launching MobileRemote");
            }
        }

        static void ShowSplash()
        {
            using (SplashScreen splash = new SplashScreen())
            {
                splash.TopMost = true;
                Application.Run(splash);
            }
        }
    }
    class TextTraceListener : TraceListener
    {
        System.IO.StreamWriter _textWriter;
        private string _lastError = "Unknown";
        private int _errorCode = 0;

        public string LastError
        {
            get { return _lastError; }
        }
        public int ErrorCode
        {
            get { return _errorCode; }
        }

        public TextTraceListener(string fileName)
        {
            _textWriter = new System.IO.StreamWriter(fileName, true);
        }
        public override void Write(string message)
        {
            _textWriter.Write(message);
            Flush();
        }
        public override void WriteLine(string message)
        {
            _textWriter.WriteLine(message);
            Flush();
        }
        public void WriteError(string message, int error)
        {
            this._lastError = message;
            this._errorCode = error;
            WriteLine(message);
            WriteLine(string.Format("Failed with error code: {0}", error));
        }
        public override void Flush()
        {
            _textWriter.Flush();
        }
        public override void Close()
        {
            _textWriter.Close();
        }
    }
}