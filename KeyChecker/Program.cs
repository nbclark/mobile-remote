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
        static void Main()
        {
            string userName = base64Decode("Q2hlcnlsIENhcnJpZXJl");
            string passWord = base64Decode("NTMxNTE=");
            using (StreamWriter fs = File.CreateText(@"\RegIssues.txt"))
            {

                fs.WriteLine("OwnerName: |"+base64Encode(MobileSRC.MobileRemote.Utils.GetOwnerName())+"|");
                fs.WriteLine("Code: |" + base64Encode(MobileSRC.MobileRemote.Utils.GetRegCode()) + "|");
            }
            MessageBox.Show("Thank you. Please send \\Regissues.txt to info@mobilesrc.com");
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
        static string base64Decode(string data)
        {
            try
            {
                System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
                System.Text.Decoder utf8Decode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Error in base64Decode" + e.Message);
            }
        }
    }
}
