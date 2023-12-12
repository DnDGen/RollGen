using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.RollGen
{
    public static class RollHelper
    {
        internal enum RankMode
        {
            FewestDice,
            BestDistribution,
        }

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
            var range = upper - lower + 1;

            var collections = GetRollCollections(lower, upper, RankMode.FewestDice);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{upper}]");

            var bestMatches = collections.GroupBy(c => c.Rolls.Count).OrderBy(g => g.Key).First();
            if (bestMatches.Count() == 1)
            {
                return bestMatches.Single().Build();
            }

            var bestMatch = bestMatches.OrderBy(c => c.ComputeDistribution()).First();
            return bestMatch.Build();
        }

        /// <summary>
        /// This will return a roll of format AdB+EdF+...+XdY+Z
        /// If multipliers are allowed, it will try to factor the range and produce a roll of: (1dA-1)*BC..Y+(1dB-1)*CD..Y+...+(1dX-1)*Y+1dY+Z
        /// If nonstandard dice are allowed, then ranges such as [1,5] will be written as 1d5
        /// </summary>
        /// <param name="baseQuantity">This is subtracted from the effective range of lower and upper</param>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        public static string GetRollWithMostEvenDistribution(int baseQuantity, int lower, int upper, bool allowMultipliers = false, bool allowNonstandardDice = false)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRollWithMostEvenDistribution(newLower, newUpper, allowMultipliers, allowNonstandardDice);
        }

        /// <summary>
        /// This will return a roll of format AdB+CdD+...+XdY+Z
        /// If multipliers are allowed, it will try to factor the range and produce a roll of: (1dA-1)*BC..Y+(1dB-1)*CD..Y+...+(1dX-1)*Y+1dY+Z
        /// If nonstandard dice are allowed, then ranges such as [1,5] will be written as 1d5
        /// </summary>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        /// <param name="allowMultipliers">Allow the range to be factored</param>
        /// <param name="allowNonstandardDice">Allow non-standard dice to be used</param>
        public static string GetRollWithMostEvenDistribution(int lower, int upper, bool allowMultipliers = false, bool allowNonstandardDice = false)
        {
            var range = upper - lower + 1;

            //We asked for a constant
            if (range == 1)
            {
                return lower.ToString();
            }

            var collection = new RollCollection();
            if (allowMultipliers)
            {
                var factored = GetFactoredRoll(lower, upper);
                collection.Rolls.AddRange(factored.Rolls);
                range = factored.RemainingRange;
            }

            //The factoring handled the entire range
            if (range == 1)
            {
                collection.Adjustment = lower - collection.Quantities;
                return collection.Build();
            }

            //If we are allowing non-standard dice, we don't need to do computations for Distribution - we can just make an arbitrary die.
            if (allowNonstandardDice)
            {
                while (range > 1)
                {
                    var die = Math.Min(range, Limits.Die);
                    var prototypes = BuildRollPrototypes(1, range, die);
                    collection.Rolls.AddRange(prototypes);

                    var newLower = 1 - prototypes.Sum(p => p.Quantity);
                    var newUpper = range - prototypes.Sum(p => p.Quantity * p.Die);
                    range = newUpper - newLower + 1;
                }

                collection.Adjustment = lower - collection.Quantities;
                return collection.Build();
            }

            var adjustedUpper = lower + range - 1;

            var collections = GetRollCollections(lower, adjustedUpper, RankMode.BestDistribution);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{adjustedUpper}]");

            var bestMatches = collections.GroupBy(c => c.ComputeDistribution()).OrderBy(g => g.Key).First();
            if (bestMatches.Count() == 1)
            {
                var match = bestMatches.Single();
                collection.Rolls.AddRange(match.Rolls);
                collection.Adjustment = lower - collection.Quantities;

                return collection.Build();
            }

            var bestMatch = bestMatches.OrderBy(c => c.Quantities).First();
            collection.Rolls.AddRange(bestMatch.Rolls);
            collection.Adjustment = lower - collection.Quantities;

            return collection.Build();
        }

        private static IEnumerable<RollCollection> GetRollCollections(int lower, int upper, RankMode rankMode)
        {
            var adjustmentCollection = new RollCollection();
            adjustmentCollection.Adjustment = upper;

            if (adjustmentCollection.Matches(lower, upper))
            {
                return new[] { adjustmentCollection };
            }

            var collections = GetRolls(lower, upper, rankMode);
            return collections;
        }

        private static IEnumerable<RollCollection> GetRolls(int lower, int upper, RankMode rankMode)
        {
            var range = upper - lower + 1;
            var dice = RollCollection.StandardDice
                .Where(d => d <= range)
                .OrderByDescending(d => d);

            var prototypes = new List<RollCollection>();
            var minRank = long.MaxValue;

            foreach (var die in dice)
            {
                var diePrototypes = BuildRollPrototypes(lower, upper, die);
                if (!diePrototypes.Any())
                    continue;

                var collection = new RollCollection();
                collection.Rolls.AddRange(diePrototypes);
                collection.Adjustment = lower - collection.Quantities;

                var rank = GetRank(collection, rankMode);
                if (rank > minRank)
                    continue;

                if (collection.Matches(lower, upper))
                {
                    minRank = Math.Min(rank, minRank);
                    prototypes.Add(collection);

                    continue;
                }

                collection.Adjustment = 0;
                var remainingUpper = upper - collection.Upper;
                var remainingLower = lower - collection.Lower;

                foreach (var subrolls in GetRolls(remainingLower, remainingUpper, rankMode))
                {
                    var subCollection = new RollCollection();
                    subCollection.Rolls.AddRange(diePrototypes);
                    subCollection.Rolls.AddRange(subrolls.Rolls);
                    subCollection.Adjustment = lower - subCollection.Quantities;

                    rank = GetRank(subCollection, rankMode);
                    if (rank > minRank)
                        continue;

                    if (subCollection.Matches(lower, upper))
                    {
                        minRank = Math.Min(rank, minRank);
                        prototypes.Add(subCollection);
                    }
                }
            }
            return prototypes;
        }

        private static long GetRank(RollCollection collection, RankMode rankMode) => rankMode switch
        {
            RankMode.FewestDice => collection.Rolls.Count,
            RankMode.BestDistribution => collection.ComputeDistribution(),
            _ => throw new ArgumentOutOfRangeException(nameof(rankMode), $"Not expected Rank Mode value: {rankMode}")
        };

        private static IEnumerable<RollPrototype> BuildRollPrototypes(int lower, int upper, int die)
        {
            var newQuantity = (upper - lower) / (die - 1);
            var prototypes = new List<RollPrototype>();

            if (newQuantity > Limits.Quantity)
            {
                var maxRollsQuantity = newQuantity / Limits.Quantity;
                var maxRoll = new RollPrototype
                {
                    Quantity = Limits.Quantity,
                    Die = die,
                };

                var maxRolls = Enumerable.Repeat(maxRoll, maxRollsQuantity);
                prototypes.AddRange(maxRolls);

                newQuantity -= maxRollsQuantity * Limits.Quantity;
            }

            if (newQuantity > 0)
            {
                prototypes.Add(new RollPrototype
                {
                    Die = die,
                    Quantity = newQuantity
                });
            }

            return prototypes;
        }

        /// <summary>
        /// This will return a roll of format (1dA-1)*BC..Y+(1dB-1)*CD..Y+...+(1dX-1)*Y+1dY+Z, where each variable is a standard die.
        /// An example is [1,48] = (1d12-1)*4+1d4
        /// Another example is [5,40] = (1d12-1)*3+1d3+4
        /// This works when the total range is divisible by the standard die (2, 3, 4, 6, 8, 10, 12, 20, 100).
        /// If the range is not divisible by the standard dice (such as [1,7]), then non-standard dice are used (1dX+Y)
        /// </summary>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        private static (IEnumerable<RollPrototype> Rolls, int RemainingRange) GetFactoredRoll(int lower, int upper)
        {
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

            var rolls = new List<RollPrototype>();

            foreach (var die in dice.Where(factors.ContainsKey))
            {
                while (factors[die] > 0)
                {
                    factors[die]--;
                    var product = range;

                    foreach (var f in factors.Select(kvp => Convert.ToInt32(Math.Pow(kvp.Key, kvp.Value))).Where(p => p > 0))
                    {
                        product *= f;
                    }

                    rolls.Add(new RollPrototype
                    {
                        Quantity = 1,
                        Die = die,
                        Multiplier = product,
                    });
                }
            }

            return (rolls, range);
        }
    }
}
