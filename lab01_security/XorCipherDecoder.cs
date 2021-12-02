using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace lab01_security
{
    public class XorCipherDecoder : IXorCipherDecoder
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

        private byte[] StringToByteArray(string hex) =>
            Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();

        private void BruteForceDecode(byte[] text)
        {
            //Key is 55
            for (byte i = byte.MinValue; i < byte.MaxValue; i++)
            {
                List<byte> result = new List<byte>();
                for (int j = 0; j < text.Length; j++)
                {
                    byte xor = (byte)(text[j] ^ i);
                    result.Add(xor);
                }
                var stringResult = Encoding.ASCII.GetString(result.ToArray());
                OutputResults(i, stringResult);
            }
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
            using (var fs = new FileStream(_fileName, FileMode.Open))
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
