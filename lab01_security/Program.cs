using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace lab01_security
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/input.txt").Result;
            var bytes = GetBytesFromBinaryString(text);
            var base64Text = Encoding.ASCII.GetString(bytes);
            var convertFromBase64Text = Convert.FromBase64String(base64Text);
            var convertedText = Encoding.ASCII.GetString(convertFromBase64Text);
            Console.WriteLine(convertedText);
        }

        public static Byte[] GetBytesFromBinaryString(String binary)
        {
            var list = new List<Byte>();

            for (int i = 0; i < binary.Length; i += 8)
            {
                String t = binary.Substring(i, 8);

                list.Add(Convert.ToByte(t, 2));
            }

            return list.ToArray();
        }
    }
}
