using System;
using System.IO;

namespace lab01_security
{
    class Program
    {
        static void Main(string[] args)
        {
            var text = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/input.txt").Result;
            var decodedText1 = Base64Decoder.Decode(text);
            Console.WriteLine(decodedText1);
            //var block1 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block1.txt").Result;
            
            //var xorDecoder = new XorCipherDecoder(true, true, $"{Environment.CurrentDirectory}/output.txt");
            //xorDecoder.BruteForceDecode(block1);
            var block2 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block2.txt").Result;
            //;RepetingKeyCipherDecoder.CalculateIndexOfCoincidence(block2);
            RepetingKeyCipherDecoder.Decrypt(block2, 3);
        }






    }
}
