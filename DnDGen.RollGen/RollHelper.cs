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
            var maxDie = RollCollection.StandardDice.Max();
            var rangeLimit = Limits.Quantity * maxDie - Limits.Quantity + 1;
            var roll = string.Empty;

            var maxCollection = new RollCollection();
            while (range > rangeLimit)
            {
                var die = maxDie;
                var quantity = Math.Min(Limits.Quantity, range / maxDie);

                if (range > Limits.Die)
                {
                    die = Limits.Die;
                    quantity = range / Limits.Die;
                }

                maxCollection.Rolls.Add(new RollPrototype { Die = die, Quantity = quantity });

                var newLower = 1 - quantity;
                var newUpper = range - die * quantity;
                range = newUpper - newLower + 1;
            }

            maxCollection.Adjustment = lower - maxCollection.Quantities;

            if (maxCollection.Rolls.Any())
            {
                roll = maxCollection.Build();
            }

            var adjustedUpper = lower + range - 1;

            var collections = GetRollCollections(lower, adjustedUpper, RankMode.FewestDice);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{upper}]");

            var bestMatches = collections.GroupBy(c => c.Rolls.Count).OrderBy(g => g.Key).First();
            if (bestMatches.Count() == 1)
            {
                return bestMatches.Single().Build();
            }

            var bestMatch = bestMatches.OrderBy(c => c.ComputeDistribution()).First();

            if (roll == string.Empty)
                return bestMatch.Build();
            else
                return $"{roll}+{bestMatch.Build()}";
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
            var roll = string.Empty;
            var range = upper - lower + 1;

            //We asked for a constant
            if (range == 1)
            {
                return lower.ToString();
            }

            if (allowMultipliers)
            {
                var factored = GetFactoredRoll(lower, upper);
                roll = factored.Roll;
                range = factored.RemainingRange;
            }

            //The factoring handled the entire range
            if (range == 1)
            {
                if (lower > 1)
                    return $"{roll}+{lower - 1}";
                else if (lower < 1)
                    return $"{roll}{lower - 1}";
                else
                    return roll;
            }

            //If we are allowing non-standard dice, we don't need to do computations for Distribution - we can just make an arbitrary die.
            if (allowNonstandardDice)
            {
                var nonStandardCollection = new RollCollection();
                while (range > 1)
                {
                    var die = range;
                    var quantity = 1;

                    if (range > Limits.Die)
                    {
                        die = Limits.Die;
                        quantity = range / Limits.Die;
                    }

                    nonStandardCollection.Rolls.Add(new RollPrototype { Die = die, Quantity = quantity });

                    var newLower = 1 - quantity;
                    var newUpper = range - die * quantity;
                    range = newUpper - newLower + 1;
                }

                nonStandardCollection.Adjustment = lower - nonStandardCollection.Quantities;

                var nonStandardRoll = nonStandardCollection.Build();

                if (roll == string.Empty)
                    return nonStandardRoll;
                else
                    return $"{roll}+{nonStandardRoll}";
            }

            var adjustedUpper = lower + range - 1;

            var collections = GetRollCollections(lower, adjustedUpper, RankMode.BestDistribution);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{adjustedUpper}]");

            var bestMatches = collections.GroupBy(c => c.ComputeDistribution()).OrderBy(g => g.Key).First();
            if (bestMatches.Count() == 1)
            {
                if (roll == string.Empty)
                    return bestMatches.Single().Build();
                else
                    return $"{roll}+{bestMatches.Single().Build()}";
            }

            var bestMatch = bestMatches.OrderBy(c => c.Quantities).First();

            if (roll == string.Empty)
                return bestMatch.Build();
            else
                return $"{roll}+{bestMatch.Build()}";
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
                var diePrototype = BuildRollPrototype(lower, upper, die);
                var collectionPrototype = new RollCollection();
                collectionPrototype.Rolls.Add(diePrototype);
                collectionPrototype.Adjustment = lower - collectionPrototype.Quantities;

                if (collectionPrototype.Matches(lower, upper))
                {
                    var rank = rankMode switch
                    {
                        RankMode.FewestDice => collectionPrototype.Rolls.Count,
                        RankMode.BestDistribution => collectionPrototype.ComputeDistribution(),
                        _ => throw new ArgumentOutOfRangeException(nameof(rankMode), $"Not expected Rank Mode value: {rankMode}")
                    };

                    if (rank > minRank)
                        continue;

                    minRank = Math.Min(rank, minRank);
                    prototypes.Add(collectionPrototype);

                    continue;
                }

                collectionPrototype.Adjustment = 0;
                var remainingUpper = upper - collectionPrototype.Upper;
                var remainingLower = lower - collectionPrototype.Lower;

                foreach (var subrolls in GetRolls(remainingLower, remainingUpper, rankMode))
                {
                    var subCollection = new RollCollection();
                    subCollection.Rolls.Add(diePrototype);
                    subCollection.Rolls.AddRange(subrolls.Rolls);
                    subCollection.Adjustment = lower - subCollection.Quantities;

                    if (subCollection.Matches(lower, upper))
                    {
                        var rank = rankMode switch
                        {
                            RankMode.FewestDice => subCollection.Rolls.Count,
                            RankMode.BestDistribution => subCollection.ComputeDistribution(),
                            _ => throw new ArgumentOutOfRangeException(nameof(rankMode), $"Not expected Rank Mode value: {rankMode}")
                        };

                        if (rank > minRank)
                            continue;

                        minRank = Math.Min(rank, minRank);
                        prototypes.Add(subCollection);
                    }
                }
            }

            return prototypes;
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
        /// If the range is not divisible by the standard dice (such as [1,7]), then non-standard dice are used (1dX+Y)
        /// Or, if non-standard are not allowed, will use the Most Even distribution.
        /// </summary>
        /// <param name="lower">The inclusive lower range</param>
        /// <param name="upper">The inclusive upper range</param>
        private static (string Roll, int RemainingRange) GetFactoredRoll(int lower, int upper)
        {
            var range = upper - lower + 1;
            if (lower == upper)
                return (lower.ToString(), range);

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

            var formula = string.Empty;

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

            return (formula, range);
        }
    }
}
