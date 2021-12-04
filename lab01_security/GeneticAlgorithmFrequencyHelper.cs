using System.Collections.Generic;
using System.Linq;

namespace lab01_security
{
    public static class GeneticAlgorithmFrequencyHelper
    {
        public const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        
        public static readonly IList<string> AllBigrams = GenerateAllBigrams(alphabet);
        public static readonly IList<string> AllTrigrams = GenerateAllTrigrams(alphabet);

        public static IList<string> GenerateAllBigrams(string alphabet)
        {
            var result = new List<string>();
            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    result.Add($"{alphabet[i]}{alphabet[j]}");
                }
            }
            return result;
        }

        public static IList<string> GenerateAllTrigrams(string alphabet)
        {
            var result = new List<string>();
            for (int i = 0; i < alphabet.Length; i++)
            {
                for (int j = 0; j < alphabet.Length; j++)
                {
                    for (int k = 0; k < alphabet.Length; k++)
                    {
                        result.Add($"{alphabet[i]}{alphabet[j]}{alphabet[k]}");
                    }
                }
            }
            return result;
        }

        public static IDictionary<string, double> CalculateBigramsFrequencies(string text)
        {
            var result = new Dictionary<string, double>();
            var numberOfBigrams = text.Length - 1;
            for (int i = 0; i < numberOfBigrams; i++)
            {
                var bigram = $"{text[i]}{text[i + 1]}";
                if (result.ContainsKey(bigram))
                {
                    result[bigram] += 1.0;
                }
                else
                {
                    result.Add(bigram, 1.0);
                }
            }
            var frequencies = result.Select(keyValuePair => new KeyValuePair<string, double>(keyValuePair.Key, keyValuePair.Value / (double)numberOfBigrams))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            return FillMissingNgramFrequencies(frequencies, AllBigrams);
        }

        public static IDictionary<string, double> CalculateTrigramsFrequencies(string text)
        {
            var result = new Dictionary<string, double>();
            var numberOfTrigrams = text.Length - 2;
            for (int i = 0; i < numberOfTrigrams; i++)
            {
                var trigram = $"{text[i]}{text[i + 1]}{text[i+2]}";
                if (result.ContainsKey(trigram))
                {
                    result[trigram] += 1.0;
                }
                else
                {
                    result.Add(trigram, 1.0);
                }
            }
            var frequencies = result.Select(keyValuePair => new KeyValuePair<string, double>(keyValuePair.Key, keyValuePair.Value / (double)numberOfTrigrams))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            return FillMissingNgramFrequencies(frequencies, AllBigrams);
        }

        public static IDictionary<string, double> FillMissingNgramFrequencies(IDictionary<string, double> frequencies, IList<string> allNgrams)
        {
            foreach (var ngram in allNgrams)
            {
                if (frequencies.ContainsKey(ngram))
                {
                    continue;
                }
                frequencies.Add(ngram, 0.0);
            }
            return frequencies;
        }
    }

}
