using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab01_security
{
    public class GeneticAlgorithm
    {
        public readonly IDictionary<string, double> _languageBigramFrequencies;
        public readonly IDictionary<string, double> _languageTrigramFrequenies;
        public readonly int _numberOfGenerations;
        public readonly int _populationSize;
        public readonly int _bestPopulationSize;
        public readonly int _childerSize;
        public readonly int _mutationSize;
        public readonly double _bigramWeigth;
        public readonly double _trigramWeigth;

        public GeneticAlgorithm(
            IDictionary<string, double> languageBigramFrequencies,
            IDictionary<string, double> languageTrigramFrequenies,
            int numberOfGenerations = 500,
            int populationSize = 80,
            double bigramWeigth = 1.0,
            double trigramWeigth = 1.0,
            double bestPopulationSizePercent = 0.2,
            double mutationPercent = 0.05)
        {
            _languageBigramFrequencies = 
                GeneticAlgorithmFrequencyHelper.FillMissingNgramFrequencies(languageBigramFrequencies, GeneticAlgorithmFrequencyHelper.AllBigrams);
            _languageTrigramFrequenies =
                GeneticAlgorithmFrequencyHelper.FillMissingNgramFrequencies(languageTrigramFrequenies, GeneticAlgorithmFrequencyHelper.AllTrigrams);
            _numberOfGenerations = numberOfGenerations;
            _populationSize = populationSize;
            _bigramWeigth = bigramWeigth;
            _trigramWeigth = trigramWeigth;
            _bestPopulationSize = (int) (populationSize * bestPopulationSizePercent);
            _childerSize = _populationSize - _bestPopulationSize;
            _mutationSize = (int)(_childerSize * mutationPercent);
            _mutationSize = _mutationSize % 2 == 0 ? _mutationSize : _mutationSize + 1;
        }

        public void DecodeSubstitutionCipher(string encoded)
        {
            var population = GenerateFirstPopulation();
            for (int i = 0; i < _numberOfGenerations; i++)
            {
                var evaluatedPopulation = EvaluatePopulation(encoded, population);
                var bestPopulation = SelectBest(evaluatedPopulation);
                var parents = SelectParents(evaluatedPopulation);
                var crossovered = Crossover(bestPopulation, parents).Distinct().ToList();
                var childer1 = crossovered.Take(_childerSize - _mutationSize);
                var shouldMutate = crossovered.TakeLast(_mutationSize).ToList();
                var childer2 = Mutate(shouldMutate);
                population = new List<string>();
                population.AddRange(bestPopulation);
                population.AddRange(childer1);
                population.AddRange(childer2);
                if (population.Count != _populationSize)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private List<string> GenerateFirstPopulation() {
            var population = new List<string>();
            for (int i = 0; i < _populationSize; i++)
            {
                population.Add(GenerateKey(SubstitutionCipherDecoder.Alphabet));
            }
            return population;
        }

        private string GenerateKey(string alphabet)
        {
            var sb = new StringBuilder();
            var rand = new Random();
            var selectionCharacters = alphabet;

            for (int i = 0; i < alphabet.Length; i++)
            {
                var index = rand.Next(0, selectionCharacters.Length);
                var ch = selectionCharacters[index];
                selectionCharacters = selectionCharacters.Remove(index, 1);
                sb.Append(ch);
            }

            return sb.ToString();
        }

        private IList<Tuple<string,double>> EvaluatePopulation(string encoded, IList<string> population)
        {
            var result = new List<Tuple<string, double>>();
            for (int i = 0; i < population.Count; i++)
            {
                var decoded = SubstitutionCipherDecoder.Decode(encoded, population[i]);
                var fitnessFunctionScore = GeneticAlgorithmFrequencyHelper.FitnessFunction
                    (decoded, _languageBigramFrequencies, _languageTrigramFrequenies);
                Console.WriteLine($"{population[i]}: {fitnessFunctionScore}");
                Console.WriteLine(decoded);
                result.Add(new (population[i], fitnessFunctionScore));
            }
            return result.OrderByDescending(v => v.Item2).ToList();
        }

        private IList<string> SelectBest(IList<Tuple<string, double>> evaluatedPopulation)
            => evaluatedPopulation.Take(_bestPopulationSize)
            .Select(v => v.Item1)
            .ToList();

        private IList<string> SelectParents(IList<Tuple<string, double>> evaluatedPopulation)
        {
            var numberOfParents = (_childerSize) / _bestPopulationSize;
            var others = evaluatedPopulation.TakeLast(_childerSize).ToList();
            return RouletteWheelParentsSelection(numberOfParents, others);
            
        }

        private IList<string> RouletteWheelParentsSelection(
            int numberOfParents,
            IList<Tuple<string, double>> others)
        {
            var rouletteWheel = BuildRouletteWheel(others);
            var maxValue = (int)rouletteWheel[rouletteWheel.Count - 1].Item2;
            var rand = new Random();
            var selected = new List<string>();
            while (selected.Count != numberOfParents)
            {
                var generatedValue = rand.Next(0, maxValue);
                var element = rouletteWheel.FirstOrDefault(p => p.Item2 > generatedValue);
                if (!selected.Contains(element.Item1))
                {
                    selected.Add(element.Item1);
                }
            }
            return selected;
        }

        private IList<Tuple<string, double>> BuildRouletteWheel(IList<Tuple<string, double>> others)
        {
            var othersOnRoulette = new List<Tuple<string, double>>();
            for (int i = 0; i < others.Count; i++)
            {
                if (i == 0)
                {
                    othersOnRoulette.Add(others[i]);
                }
                else
                {
                    var element = others[i];
                    var sum = othersOnRoulette[i - 1].Item2 + element.Item2;
                    othersOnRoulette.Add(new(element.Item1, sum));
                }
            }
            return othersOnRoulette;
        }

        private IList<string> Crossover (IList<string> parents1, IList<string> parents2)
        {
            var children = new List<string>();
            for (int i = 0; i < parents1.Count; i++)
            {
                for (int j = 0; j < parents2.Count; j++)
                {
                    children.AddRange(Crossover(parents1[i], parents2[j]));
                }
            }
            return children;
        }

        private IList<string> Crossover(string parent1, string parent2)
        {
            var rand = new Random();
            var startIndex = rand.Next(0, parent1.Length);
            var endIndex = rand.Next(startIndex, parent1.Length);
            return new List<string>
            {
                CrossoverForOneChild(parent1, parent2, startIndex, endIndex),
                CrossoverForOneChild(parent2, parent1, startIndex, endIndex)
            };
        }

        private string CrossoverForOneChild(
            string parent1, string parent2,
            int startIndex, int endIndex)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var charToInsert = parent1[i];
                var charAtPosition = parent2[i];
                var position = parent2.IndexOf(charToInsert);
                parent2 = parent2.Remove(position, 1).Insert(position, charAtPosition.ToString());
                parent2 = parent2.Remove(i, 1).Insert(i, charToInsert.ToString());
            }
            return parent2;
        }


        private IList<string> Mutate(IList<string> parents)
        {
            var mutated = new List<string>();
            for (int i = 0; i < parents.Count; i += 2)
            {
                mutated.AddRange(Mutate(parents[i], parents[i + 1]));
            }
            return mutated;
        }

        private IList<string> Mutate(string parent1, string parent2)
        {
            var bitString = GenerateBinaryString(parent1.Length);
            return new List<string>
            {
                MutateForOneChild(parent1, parent2, bitString),
                MutateForOneChild(parent2, parent1, bitString)
            };
        }

        private string MutateForOneChild(
            string parent1, string parent2, 
            string bitString)
        {
            for (int i = 0; i < bitString.Length; i++)
            {
                if (bitString[i] == '1')
                {
                    bitString = bitString.Remove(i, 1).Insert(i, parent1[i].ToString());
                }
            }
            parent2 = string.Concat(parent2.Where(ch => !bitString.Contains(ch)));
            for (int i = 0; i < bitString.Length; i++)
            {
                if (bitString[i] == '0')
                {
                    bitString = bitString.Remove(i, 1).Insert(i, parent2[0].ToString());
                    parent2 = parent2.Remove(0, 1);
                }
            }
            return bitString;
        }

        private string GenerateBinaryString(int length)
        {
            var rand = new Random();
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rand.Next() % 2);
            }
            return sb.ToString();
        }
    }
}