using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.RollGen
{
    public static class RollHelper
    {
        public static string GetRollWithFewestDice(int baseQuantity, int lower, int upper)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRollWithFewestDice(newLower, newUpper);
        }

        public static string GetRollWithFewestDice(int lower, int upper)
        {
            var collections = GetRollCollections(lower, upper);
            if (!collections.Any())
                throw new ArgumentException($"Cannot generate a valid roll for range [{lower},{upper}]");

            var bestMatchingRanking = collections.Min(c => c.GetRankingForFewestDice(lower, upper));
            var bestMatchingCollection = collections.First(c => c.GetRankingForFewestDice(lower, upper) == bestMatchingRanking);

            return bestMatchingCollection.Build();
        }

        public static string GetRollWithMostEvenDistribution(int baseQuantity, int lower, int upper)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRollWithMostEvenDistribution(newLower, newUpper);
        }

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
    }
}
