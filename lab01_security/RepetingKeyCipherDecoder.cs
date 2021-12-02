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

        }

        public static void CalculateIndexOfCoincidence(string encoded)
        {
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
    }
}
