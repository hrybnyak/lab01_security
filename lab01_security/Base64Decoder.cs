using System;
using System.Collections.Generic;
using System.Text;

namespace lab01_security
{
    public static class Base64Decoder
    {
        public static string ConvertFromBinaryAndBase64(string encodedText)
        {
            var fromBinaryToBytes = GetBytesFromBinaryString(encodedText);
            return DecodeBase64(Encoding.ASCII.GetString(fromBinaryToBytes));
        }

        public static string DecodeBase64(string encodedText)
        {
            var convertFromBase64Text = Convert.FromBase64String(encodedText);
            return Encoding.ASCII.GetString(convertFromBase64Text);
        }

        public static byte[] GetBytesFromBinaryString(string binary)
        {
            var list = new List<byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                string t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }
    }
}
