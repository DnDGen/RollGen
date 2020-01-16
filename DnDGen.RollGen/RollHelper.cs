using System.Collections.Generic;
using System.Linq;

namespace DnDGen.RollGen
{
    public static class RollHelper
    {
        public static string GetRoll(int baseQuantity, int lower, int upper)
        {
            var newLower = lower - baseQuantity;
            var newUpper = upper - baseQuantity;

            return GetRoll(newLower, newUpper);
        }

        public static string GetRoll(int lower, int upper)
        {
            var collection = GetRollCollection(lower, upper);

            return collection.Build();
        }

        internal static RollCollection GetRollCollection(int lower, int upper)
        {
            var adjustmentCollection = new RollCollection();
            adjustmentCollection.Adjustment = upper;

            if (adjustmentCollection.Matches(lower, upper))
            {
                return adjustmentCollection;
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

                    clone.Rolls.Add(additionalPrototype);
                    clone.Adjustment = lower - clone.Quantities;

                    clones.Add(clone);
                }

                collections.AddRange(clones);

                var dieCollection = new RollCollection();
                var diePrototype = BuildRollPrototype(lower, upper, die);

                dieCollection.Rolls.Add(diePrototype);
                dieCollection.Adjustment = lower - dieCollection.Quantities;

                collections.Add(dieCollection);
            }

            var matchingCollections = collections.Where(c => c.Matches(lower, upper));
            var bestMatchingRanking = matchingCollections.Min(c => c.GetRanking(lower, upper));
            var bestMatchingCollection = matchingCollections.First(c => c.GetRanking(lower, upper) == bestMatchingRanking);

            return bestMatchingCollection;
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
