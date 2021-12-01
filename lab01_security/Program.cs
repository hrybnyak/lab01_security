using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var block1 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block1.txt").Result;
            var bytesHex = StringToByteArray(block1);
            //XorBruteforce(bytesHex);
        }

        public static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
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

        public static void XorBruteforce(byte[] text)
        {
            //key is 55
            using (var sw = new StreamWriter($"{Environment.CurrentDirectory}/output.txt"))
            {
                for (byte i = 1; i < 255; i++)
                {
                    Console.WriteLine($"Key: {i}");
                    sw.WriteLine($"Key: {i}");
                    Console.WriteLine();
                    sw.WriteLine();
                    List<byte> result = new List<byte>();
                    for (int j = 0; j < text.Length; j++)
                    {
                        byte xor = (byte)(text[j] ^ i);
                        result.Add(xor);
                    }
                    Console.WriteLine(Encoding.ASCII.GetString(result.ToArray()));
                    sw.WriteLine(Encoding.ASCII.GetString(result.ToArray()));
                    Console.WriteLine();
                    sw.WriteLine();
                }
            }
        }
    }
}
