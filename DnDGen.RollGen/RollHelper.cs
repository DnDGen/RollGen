using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.RollGen
{
    public static class RollHelper
    {
        /// <summary>
        /// This will return a roll of format XdY+Z
        /// </summary>
        /// <param name="baseQuantity">This is subtracted from the effective range of lower and upper</param>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        /// <returns></returns>
        public static string GetRollWithFewestDice(int baseQuantity, int lower, int upper)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRollWithFewestDice(newLower, newUpper);
        }

        /// <summary>
        /// This will return a roll of format XdY+Z
        /// </summary>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        public static string GetRollWithFewestDice(int lower, int upper)
        {
            var collections = GetRollCollections(lower, upper);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{upper}]");

            var bestMatchingRanking = collections.Min(c => c.GetRankingForFewestDice(lower, upper));
            var bestMatchingCollection = collections.First(c => c.GetRankingForFewestDice(lower, upper) == bestMatchingRanking);

            return bestMatchingCollection.Build();
        }

        /// <summary>
        /// This will return a roll of format AdB+EdF+...+XdY+Z
        /// </summary>
        /// <param name="baseQuantity">This is subtracted from the effective range of lower and upper</param>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        public static string GetRollWithMostEvenDistribution(int baseQuantity, int lower, int upper)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRollWithMostEvenDistribution(newLower, newUpper);
        }

        /// <summary>
        /// This will return a roll of format AdB+CdD+...+XdY+Z
        /// </summary>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        public static string GetRollWithMostEvenDistribution(int lower, int upper)
        {
            var collections = GetRollCollections(lower, upper);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{upper}]");

            var bestMatchingRanking = collections.Min(c => c.GetRankingForMostEvenDistribution(lower, upper));
            var bestMatchingCollection = collections.First(c => c.GetRankingForMostEvenDistribution(lower, upper) == bestMatchingRanking);

            return bestMatchingCollection.Build();
        }

        private static IEnumerable<RollCollection> GetRollCollections(int lower, int upper)
        {
            var adjustmentCollection = new RollCollection();
            adjustmentCollection.Adjustment = upper;

            if (adjustmentCollection.Matches(lower, upper))
            {
                return new[] { adjustmentCollection };
            }

            var range = upper - lower + 1;
            var dice = RollCollection.StandardDice
                .Where(d => d <= range)
                .OrderByDescending(d => d);

            var collections = new List<RollCollection>();
            var canBeCloned = collections.Where(c => !c.Matches(lower, upper));

            foreach (var die in dice)
            {
                var clones = new List<RollCollection>();

                foreach (var collection in canBeCloned)
                {
                    var remainingUpper = upper - collection.Upper;
                    var remainingLower = lower - collection.Lower;
                    var remainingRange = remainingUpper - remainingLower + 1;

                    if (remainingRange < die)
                        continue;

                    var clone = new RollCollection();
                    clone.Rolls.AddRange(collection.Rolls);

                    var additionalPrototype = BuildRollPrototype(remainingLower, remainingUpper, die);
                    if (additionalPrototype.Quantity > Limits.Quantity)
                        continue;

                    clone.Rolls.Add(additionalPrototype);
                    clone.Adjustment = lower - clone.Quantities;

                    clones.Add(clone);
                }

                collections.AddRange(clones);

                var dieCollection = new RollCollection();
                var diePrototype = BuildRollPrototype(lower, upper, die);

                if (diePrototype.Quantity <= Limits.Quantity)
                {
                    dieCollection.Rolls.Add(diePrototype);
                    dieCollection.Adjustment = lower - dieCollection.Quantities;

                    collections.Add(dieCollection);
                }
            }

            var matchingCollections = collections.Where(c => c.Matches(lower, upper));

            return matchingCollections;
        }

        private static RollPrototype BuildRollPrototype(int lower, int upper, int die)
        {
            var newQuantity = (upper - lower) / (die - 1);

            return new RollPrototype
            {
                Die = die,
                Quantity = newQuantity
            };
        }

        /// <summary>
        /// This will return a roll of format (1dA-1)*BC..Y+(1dB-1)*CD..Y+...+(1dX-1)*Y+1dY+Z, where each variable is a standard die.
        /// An example is [1,48] = (1d12-1)*4+1d4
        /// Another example is [5,40] = (1d6-1)*6+1d6+4
        /// This works when the total range is divisible by the standard die (2, 3, 4, 6, 8, 10, 12, 20, 100).
        /// If the range is not divisible by the standard dice (such as 1-7), then the Most Even distribution is used.
        /// </summary>
        /// <param name="baseQuantity">This is subtracted from the effective range of lower and upper</param>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        public static string GetRollWithPerfectDistribution(int baseQuantity, int lower, int upper)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRollWithPerfectDistribution(newLower, newUpper);
        }

        /// <summary>
        /// This will return a roll of format (1dA-1)*BC..Y+(1dB-1)*CD..Y+...+(1dX-1)*Y+1dY+Z, where each variable is a standard die.
        /// An example is [1,48] = (1d12-1)*4+1d4
        /// Another example is [5,40] = (1d6-1)*6+1d6+4
        /// This works when the total range is divisible by the standard die (2, 3, 4, 6, 8, 10, 12, 20, 100).
        /// If the range is not divisible by the standard dice (such as [1,7]), then the Most Even distribution is used.
        /// </summary>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        public static string GetRollWithPerfectDistribution(int lower, int upper)
        {
            if (lower == upper)
                return lower.ToString();

            var range = upper - lower + 1;
            var dice = RollCollection.StandardDice
                .Where(d => d <= range)
                .OrderByDescending(d => d)
                .ToArray();
            var factors = new Dictionary<int, int>();

            foreach (var die in dice)
            {
                while (range % die == 0)
                {
                    if (!factors.ContainsKey(die))
                        factors[die] = 0;

                    factors[die]++;
                    range /= die;
                }
            }

            if (range != 1)
                return GetRollWithMostEvenDistribution(lower, upper);

            var formula = string.Empty;

            foreach (var die in dice.Where(factors.ContainsKey))
            {
                while (factors[die] > 0)
                {
                    factors[die]--;
                    var product = 1;

                    foreach (var f in factors.Select(kvp => Convert.ToInt32(Math.Pow(kvp.Key, kvp.Value))).Where(p => p > 0))
                    {
                        product *= f;
                    }

                    if (formula != string.Empty)
                        formula += "+";

                    if (product > 1)
                    {
                        formula += $"(1d{die}-1)*{product}";
                    }
                    else
                    {
                        formula += $"1d{die}";
                    }
                }
            }

            var difference = lower - 1;
            if (difference > 0)
                formula += $"+{difference}";
            else if (difference < 0)
                formula += difference.ToString();

            return formula;
        }
    }
}
