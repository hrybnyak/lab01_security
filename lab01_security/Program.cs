using Newtonsoft.Json;
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
            //var text = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/input.txt").Result;
            //var decodedText1 = Base64Decoder.ConvertFromBinaryAndBase64(text);
            //Console.WriteLine(decodedText1);

            //var block1 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block1.txt").Result;
            //var xorDecoder = new XorCipherDecoder(true, true, $"{Environment.CurrentDirectory}/output.txt");
            //xorDecoder.XorDecode(block1, 55);

            //var block2 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block2.txt").Result;
            //var decodedFromBase64 = Base64Decoder.DecodeBase64(block2);
            //////RepetingKeyCipherDecoder.CalculateIndexOfCoincidence(decodedFromBase64);
            //////RepetingKeyCipherDecoder.Decrypt(decodedFromBase64, 3);
            //var keys = new List<byte> { 76, 48, 108 };
            //Console.WriteLine($"Key: {Encoding.ASCII.GetString(keys.ToArray())}");
            //Console.WriteLine(RepetingKeyCipherDecoder.DecryptWithKeys(decodedFromBase64, 3, keys));

            //var block3 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block3.txt").Result;
            var bigramFrequencyJson = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/bigramFrequency.json").Result;
            var trigramFrequencyJson = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/trigramFrequency.json").Result;
            var bigramFrequency = JsonConvert.DeserializeObject<Dictionary<string, double>>(bigramFrequencyJson);
            var trigramFrequency = JsonConvert.DeserializeObject<Dictionary<string, double>>(trigramFrequencyJson);
            ////geneticAlgorithm.DecodeSubstitutionCipher(block3);
            //Console.WriteLine(SubstitutionCipherDecoder.Decode(block3, "EKMFLGDQVZNTOWYHXUSPAIBRCJ"));
            var block4 = File.ReadAllTextAsync($"{Environment.CurrentDirectory}/resources/block4.txt").Result;
            //RepetingKeyCipherDecoder.CalculateIndexOfCoincidence(block4);
            var geneticAlgorithm = new GeneticAlgorithm(bigramFrequency, trigramFrequency, 400, 64, 1, 1, 0.4);
            geneticAlgorithm.DecodeSubstitutionCipher(block4, 4);
        }
    }
}
