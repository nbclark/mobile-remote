using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace KeyCheck
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (StreamWriter fs = File.CreateText(@"\RegIssues.log"))
            {

                fs.WriteLine(base64Encode(MobileSRC.MobileRemote.Utils.GetOwnerName()));
                fs.WriteLine(base64Encode(MobileSRC.MobileRemote.Utils.GetRegCode()));
            }
            MessageBox.Show("Thank you. Please send \\Regissues.log to info@mobilesrc.com");
        }
        static string base64Encode(string data)
        {
            try
            {
                byte[] encData_byte = new byte[data.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Encode" + e.Message);
            }
        }
    }
}
