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
        public readonly IDictionary<int, string> _bestInEachGeneration = new Dictionary<int, string>();
        public readonly IDictionary<int, List<string>> _bestInEachGenerationMultipleKeys = new Dictionary<int, List<string>>();
        public readonly int _numberOfGenerations;
        public readonly int _populationSize;
        public readonly int _mutationSize;
        public readonly double _bigramWeigth;
        public readonly double _trigramWeigth;

        public GeneticAlgorithm(
            IDictionary<string, double> languageBigramFrequencies,
            IDictionary<string, double> languageTrigramFrequenies,
            int numberOfGenerations = 400,
            int populationSize = 45,
            double bigramWeigth = 1.0,
            double trigramWeigth = 1.0,
            double mutationPercent = 0.4)
        {
            _languageBigramFrequencies = 
                GeneticAlgorithmFrequencyHelper.FillMissingNgramFrequencies(languageBigramFrequencies, GeneticAlgorithmFrequencyHelper.AllBigrams);
            _languageTrigramFrequenies =
                GeneticAlgorithmFrequencyHelper.FillMissingNgramFrequencies(languageTrigramFrequenies, GeneticAlgorithmFrequencyHelper.AllTrigrams);
            _numberOfGenerations = numberOfGenerations;
            _populationSize = populationSize;
            _bigramWeigth = bigramWeigth;
            _trigramWeigth = trigramWeigth;
            _mutationSize = (int)(_populationSize * mutationPercent);
            _mutationSize = _mutationSize % 2 == 0 ? _mutationSize : _mutationSize + 1;
        }

        public void DecodeSubstitutionCipher(string encoded)
        {
            var population = GenerateFirstPopulation();
            for (int i = 0; i < _numberOfGenerations; i++)
            {
                var evaluatedPopulation = EvaluatePopulation(encoded, population);
                _bestInEachGeneration.Add(i, evaluatedPopulation[0].Item1);
                var parents1 = SelectParents(evaluatedPopulation);
                var withoutParents1 = evaluatedPopulation.Where(ep => !parents1.Contains(ep.Item1)).ToList();
                var parents2 = SelectParents(withoutParents1);
                var crossovered = Crossover(parents1, parents2).Distinct().ToList();
                crossovered = EvaluatePopulation(encoded, crossovered)
                    .Select(v => v.Item1).ToList();
                var childer1 = crossovered.Take(_populationSize - _mutationSize);
                var shouldMutate = crossovered.Skip(_populationSize - _mutationSize)
                    .Take(_mutationSize).ToList();
                var childer2 = Mutate(shouldMutate);
                population = new List<string>();
                population.AddRange(childer1);
                population.AddRange(childer2);
                if (population.Count != _populationSize)
                {
                    throw new InvalidOperationException();
                }
            }
            PrintResults(encoded);
        }

        public void DecodeSubstitutionCipher(string encoded, int numberOfKeys)
        {
            var rand = new Random();
            try
            {
                var population = GenerateFirstPopulation(numberOfKeys);
                for (int i = 0; i < _numberOfGenerations; i++)
                {
                    var evaluated = EvaluatePopulation(encoded, population);
                    _bestInEachGenerationMultipleKeys.Add(i, evaluated[0].Item1);
                    var parents1 = SelectParents(evaluated, (int)(_populationSize * 0.2));
                    var withoutParents1 = evaluated.Where(ep => !parents1.Any(p => p.All(ep.Item1.Contains))).ToList();
                    var parents2 = SelectParents(withoutParents1, (int)(_populationSize * 0.2));
                    var crossover = Crossover(parents1, parents2).Distinct(new SequenceComparer()).ToList();
                    //crossover = SelectParents(EvaluatePopulation(encoded, crossover), _populationSize);
                    crossover = EvaluatePopulation(encoded, crossover).Select(t => t.Item1).Take(_populationSize).ToList();
                    var index = rand.Next(0, _populationSize - _mutationSize);
                    var children1 = crossover.Take(index).ToList();
                    children1.AddRange(crossover.Skip(index + _mutationSize).Take(_populationSize - (index + _mutationSize)));
                    var shouldMutate = crossover.Skip(index)
                        .Take(_mutationSize).ToList();
                    var children2 = Mutate(shouldMutate);
                    population = new List<List<string>>();
                    population.AddRange(children1);
                    population.AddRange(children2);
                    if (population.Count != _populationSize)
                    {
                        throw new InvalidOperationException("Breading problem");
                    }
                }
                PrintResultsMulitpleKeys(encoded);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Last successful generation: {_bestInEachGenerationMultipleKeys.OrderByDescending(k => k.Key).ToList()[0].Key}");
                PrintResultsMulitpleKeys(encoded);
            }
        }

        private void PrintResultsMulitpleKeys(string encoded)
        {
            Console.WriteLine($"Number of generations: {_numberOfGenerations}");
            Console.WriteLine($"Population size: {_populationSize}");
            Console.WriteLine($"Mutation size: {_mutationSize}");
            foreach (var key in _bestInEachGenerationMultipleKeys.Keys)
            {
                if (key % 10 == 0)
                {
                    Console.WriteLine($"Generation: {key}");
                    Console.WriteLine($"Keys:");
                    foreach(var stringKey in _bestInEachGenerationMultipleKeys[key])
                    {
                        Console.WriteLine(stringKey);
                    }
                    var decoded = SubstitutionCipherDecoder.Decode(encoded, _bestInEachGenerationMultipleKeys[key]);
                    var score = GeneticAlgorithmFrequencyHelper.FitnessFunction(decoded, _languageBigramFrequencies, _languageTrigramFrequenies);
                    Console.WriteLine($"Score {score}");
                    Console.WriteLine($"Decoded: {decoded}");
                }
            }
        }

        private void PrintResults(string encoded)
        {
            foreach (var key in _bestInEachGeneration.Keys.OrderBy(k => k))
            {
                var decoded = SubstitutionCipherDecoder.Decode(encoded, _bestInEachGeneration[key]);
                var score = GeneticAlgorithmFrequencyHelper.FitnessFunction(decoded, _languageBigramFrequencies, _languageTrigramFrequenies);
                Console.WriteLine($"Generation: {key}");
                Console.WriteLine($"Key: {_bestInEachGeneration[key]}");
                Console.WriteLine($"Score: {score}");
                Console.WriteLine($"Decoded: {decoded}");
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

        private List<List<string>> GenerateFirstPopulation(int numberOfKeys)
        {
            var result = new List<List<string>>();
            for (int i = 0; i < _populationSize; i++)
            {
                var gene = new List<string>();
                for (int j = 0; j < numberOfKeys; j++)
                {
                    gene.Add(GenerateKey(SubstitutionCipherDecoder.Alphabet));
                }
                result.Add(gene);
            }
            return result;
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

        private List<Tuple<string,double>> EvaluatePopulation(string encoded, List<string> population)
        {
            var result = new List<Tuple<string, double>>();
            for (int i = 0; i < population.Count; i++)
            {
                var decoded = SubstitutionCipherDecoder.Decode(encoded, population[i]);
                var fitnessFunctionScore = GeneticAlgorithmFrequencyHelper.FitnessFunction
                    (decoded, _languageBigramFrequencies, _languageTrigramFrequenies);
                result.Add(new (population[i], fitnessFunctionScore));
            }
            return result.OrderBy(v => v.Item2).ToList();
        }

        private List<Tuple<List<string>, double>> EvaluatePopulation(string encoded, List<List<string>> population)
        {
            var result = new List<Tuple<List<string>, double>>();
            for (int i = 0; i < population.Count; i++)
            {
                var decoded = SubstitutionCipherDecoder.Decode(encoded, population[i]);
                var fitnessFunctionScore = GeneticAlgorithmFrequencyHelper.FitnessFunction
                    (decoded, _languageBigramFrequencies, _languageTrigramFrequenies);
                result.Add(new(population[i], fitnessFunctionScore));
            }
            return result.OrderBy(v => v.Item2).ToList();
        }


        private List<string> SelectParents(List<Tuple<string, double>> evaluatedPopulation)
        {
            var numberOfParents = (int) (_populationSize * 0.3);
            return RouletteWheelParentsSelection(numberOfParents, evaluatedPopulation);
            
        }

        private List<List<string>> SelectParents(List<Tuple<List<string>, double>> evaluatedPopulation, int numberOfParents)
        {
            return RouletteWheelParentsSelection(numberOfParents, evaluatedPopulation);
        }

        private List<string> RouletteWheelParentsSelection(
            int numberOfParents,
            List<Tuple<string, double>> others)
        {
            var rouletteWheel = BuildRouletteWheel(others);
            var maxValue = (int)rouletteWheel[rouletteWheel.Count - 1].Item2;
            var rand = new Random();
            var selected = new List<string>();
            var numberOfAttempts = 0;
            if (others.Count < numberOfParents)
            {
                throw new InvalidOperationException("Number of available parents shouldn't be less than number of selected");
            }
            else if (rouletteWheel.Count == numberOfParents)
            {
                return rouletteWheel.Select(t => t.Item1).ToList();
            }
            while (selected.Count != numberOfParents)
            {
                numberOfAttempts++;
                var generatedValue = rand.Next(0, maxValue);
                var element = rouletteWheel.FirstOrDefault(p => p.Item2 > generatedValue);
                if (!selected.Contains(element.Item1))
                {
                    numberOfAttempts = 0;
                    selected.Add(element.Item1);
                }
                if (numberOfAttempts == 150)
                {
                    throw new ArgumentException("Selection is too long");
                }
            }
            return selected;
        }

        private List<List<string>> RouletteWheelParentsSelection(
            int numberOfParents,
            List<Tuple<List<string>, double>> others)
        {
            var rouletteWheel = BuildRouletteWheel(others);
            var maxValue = (int)rouletteWheel[rouletteWheel.Count - 1].Item2;
            var rand = new Random();
            var selected = new List<List<string>>();
            var numberOfAttempts = 0;
            if (rouletteWheel.Count < numberOfParents)
            {
                throw new InvalidOperationException();
            }
            else if (rouletteWheel.Count == numberOfParents)
            {
                return rouletteWheel.Select(t => t.Item1).ToList();
            }
            while (selected.Count != numberOfParents)
            {
                numberOfAttempts++;
                var generatedValue = rand.Next(0, maxValue);
                var element = rouletteWheel.FirstOrDefault(p => p.Item2 > generatedValue);
                if (!selected.Any(s => s.All(element.Item1.Contains)))
                {
                    numberOfAttempts = 0;
                    selected.Add(element.Item1);
                }
                if (numberOfAttempts == 50)
                {
                    throw new ArgumentException();
                }
            }
            return selected;
        }

        private List<Tuple<string, double>> BuildRouletteWheel(List<Tuple<string, double>> others)
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

        private List<Tuple<List<string>, double>> BuildRouletteWheel(List<Tuple<List<string>, double>> others)
        {
            var othersOnRoulette = new List<Tuple<List<string>, double>>();
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

        private List<string> Crossover (List<string> parents1, List<string> parents2)
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

        private List<List<string>> Crossover(List<List<string>> parents1, List<List<string>> parents2)
        {
            var children = new List<List<string>>();
            for (int i = 0; i < parents1.Count; i++)
            {
                for (int j = 0; j < parents2.Count; j++)
                {
                    var child1 = new List<string>();
                    var child2 = new List<string>();
                    for (int k = 0; k < parents2[j].Count; k++)
                    {
                        var crossover = Crossover(parents1[i][k], parents2[j][k]);
                        child1.Add(crossover[0]);
                        child2.Add(crossover[1]);
                    }
                    children.Add(child1);
                    children.Add(child2);
                }
            }
            return children;
        }


        private List<string> Crossover(string parent1, string parent2)
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


        private List<string> Mutate(List<string> parents)
        {
            var mutated = new List<string>();
            for (int i = 0; i < parents.Count; i += 2)
            {
                mutated.AddRange(Mutate(parents[i], parents[i + 1]));
            }
            return mutated;
        }

        private List<List<string>> Mutate(List<List<string>> parents)
        {
            var result = new List<List<string>>();
            for (int i = 0; i < parents.Count / 2; i ++)
            {
                var firstChild = new List<string>();
                var secondChild = new List<string>();
                for (int j = 0; j < parents[i].Count; j++)
                {
                    var mutated = Mutate(parents[i][j], parents[parents.Count - i - 1][j]);
                    firstChild.Add(mutated[0]);
                    secondChild.Add(mutated[1]);
                }
                result.Add(firstChild);
                result.Add(secondChild);
            }
            return result;
        } 

        private List<string> Mutate(string parent1, string parent2)
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