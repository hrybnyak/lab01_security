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
            //    var text = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/input.txt").Result;
            //    var decodedText1 = Base64Decoder.ConvertFromBinaryAndBase64(text);
            //    Console.WriteLine(decodedText1);

            //    var block1 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block1.txt").Result;
            //    var xorDecoder = new XorCipherDecoder(true, true, $"{Environment.CurrentDirectory}/output.txt");
            //    xorDecoder.XorDecode(block1, 55);

            //    var block2 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block2.txt").Result;
            //    var decodedFromBase64 = Base64Decoder.DecodeBase64(block2);
            //RepetingKeyCipherDecoder.CalculateIndexOfCoincidence(decodedFromBase64);
            ////RepetingKeyCipherDecoder.Decrypt(decodedFromBase64, 3);
            //var keys = new List<byte> { 76, 48, 108 };
            //Console.WriteLine($"Key: {Encoding.ASCII.GetString(keys.ToArray())}");
            //Console.WriteLine(RepetingKeyCipherDecoder.DecryptWithKeys(decodedFromBase64, 3, keys));
            Console.WriteLine(SubstitutionCipherDecoder.Decode("GSVJFRXPYILDMULCQFNKHLEVIGSRIGVVMOZABWLTH", "ZYXWVUTSRQPONMLKJIHGFEDCBA"));

        }
    }
}
