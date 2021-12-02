using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01_security
{
    public class RepetingKeyCipherDecoder
    {
        public static void Decrypt(string encoded, int keyLength)
        {
            var convertFromBase64Text = Convert.FromBase64String(encoded);
            encoded = Encoding.ASCII.GetString(convertFromBase64Text);
            var separatedEncodedStrings = SeperateEncodedString(encoded, keyLength);
            var xorDecoded = XorBruteForce(separatedEncodedStrings);
            //keys are 76, 48, 108
            var list = new List<string> { xorDecoded[0][76], xorDecoded[1][48], xorDecoded[2][108] };
            Console.WriteLine(AssembleResult(list));
        }

        public static void CalculateIndexOfCoincidence(string encoded)
        {
            var convertFromBase64Text = Convert.FromBase64String(encoded);
            encoded = Encoding.ASCII.GetString(convertFromBase64Text);
            var temp = encoded;
            for (int i = 1; i < encoded.Length / 2; i++)
            {
                temp = temp[temp.Length - 1] + temp[0..^1];
                var coincidence = CalculateCoincidence(encoded, temp);
                Console.WriteLine($"{coincidence}");
            }
        }

        private static double CalculateCoincidence(string first, string second)
        {
            if (first.Length != second.Length)
            {
                throw new InvalidOperationException("Can only calculate coincodence on strings of the same size");
            }
            int count = 0;
            for (int i = 0; i < first.Length; i++)
            {
                if (first[i] == second[i])
                {
                    count++;
                }
            }
            return (double)count / first.Length;
        }

        private static Dictionary<int, string> SeperateEncodedString(string encoded, int keyLength)
        {
            var dictionary = new Dictionary<int, string>();
            for (int i = 0; i < encoded.Length; i++)
            {
                if (dictionary.ContainsKey(i % keyLength))
                {
                    dictionary[i % keyLength] += encoded[i];
                }
                else
                {
                    dictionary[i % keyLength] = string.Empty + encoded[i];
                }
            }
            return dictionary;
        }

        private static string AssembleResult(List<string> results)
        {
            StringBuilder result = new StringBuilder();
            
            for (int i = 0; i < results[0].Length; i++)
            {
                for (int j = 0; j < results.Count; j++)
                {
                    if (results[j].Length - 1 >= i)
                    {
                        result.Append(results[j][i]);
                    }
                }
            }
            return result.ToString();
        }

        private static Dictionary<int, Dictionary<byte, string>> XorBruteForce(Dictionary<int, string> separatedEncodedString)
        {
            var xorDecoder = new XorCipherDecoder(false, false);
            var result = new Dictionary<int, Dictionary<byte, string>>();
            foreach (var key in separatedEncodedString.Keys)
            {
                var xorResult = xorDecoder.BruteForceDecode(Encoding.ASCII.GetBytes(separatedEncodedString[key]));
                result.Add(key, xorResult);
            }
            return result;
        }
    }
}
