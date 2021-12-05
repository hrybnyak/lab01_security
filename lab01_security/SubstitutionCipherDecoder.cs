using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01_security
{
    public static class SubstitutionCipherDecoder
    {
        public const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string Decode(string encoded, string key)
        {
            var result = new StringBuilder();
            for (int i = 0; i < encoded.Length; i++)
            {
                var index = key.IndexOf(encoded[i]);
                result.Append(alphabet[index]);
            }
            return result.ToString();
        }
    }
}
