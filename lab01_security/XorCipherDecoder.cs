using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lab01_security
{
    public class XorCipherDecoder 
    {
        public readonly bool _printResult = true;
        public readonly bool _saveResultsToFile = true;
        public readonly string _fileName;

        public XorCipherDecoder(bool printResult = true, bool saveResultsToFile = true, string fileName = "output.txt")
        {
            _printResult = printResult;
            _saveResultsToFile = saveResultsToFile;
            _fileName = fileName;
        }

        public void BruteForceDecode(string encodedText)
        {
            var bytes = StringToByteArray(encodedText);
            BruteForceDecode(bytes);
        }

        public void XorDecode(string encodedText, byte key)
        {
            var bytes = StringToByteArray(encodedText);
            XorDecode(bytes, key);
        }

        public byte[] StringToByteArray(string hex) =>
            Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();

        public Dictionary<byte, string> BruteForceDecode(byte[] text)
        {
            var dictionary = new Dictionary<byte, string>();
            //Key is 55
            for (byte i = byte.MinValue; i < byte.MaxValue; i++)
            {
                StringBuilder sb = new StringBuilder();
                for (int j = 0; j < text.Length; j++)
                {
                    char xor = (char)(text[j] ^ i);
                    sb.Append(xor);
                }
                var result = sb.ToString();
                OutputResults(i, result);
                dictionary.Add(i, result);
            }
            return dictionary;
        }

        public string XorDecode(byte[] text, byte key)
        {
            string result = string.Empty;
            StringBuilder sb = new StringBuilder();
            for (int j = 0; j < text.Length; j++)
            {
                char xor = (char)(text[j] ^ key);
                sb.Append(xor);
            }
            result = sb.ToString();
            OutputResults(key, result);
            return result;
        }

        private void OutputResults(int key, string result)
        {
            if (_printResult)
            {
                PrintResults(key, result);
            }
            if (_saveResultsToFile)
            {
                SaveResults(key, result);
            }
        }

        private void PrintResults(int key, string result)
        {
            Console.WriteLine($"Key: {key}");
            Console.WriteLine();
            Console.WriteLine(result);
            Console.WriteLine();
        }

        private void SaveResults(int key, string result)
        {
            using (var fs = new FileStream(_fileName, FileMode.OpenOrCreate))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine($"Key: {key}");
                    sw.WriteLine();
                    sw.WriteLine(result);
                    sw.WriteLine();
                }
            }
        }

    }
}
