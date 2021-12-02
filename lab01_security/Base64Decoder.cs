using System;
using System.Collections.Generic;
using System.Text;

namespace lab01_security
{
    public static class Base64Decoder
    {
        public static string Decode(string encodedText)
        {
            var bytes = GetBytesFromBinaryString(encodedText);
            var base64Text = Encoding.ASCII.GetString(bytes);
            var convertFromBase64Text = Convert.FromBase64String(base64Text);
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
