using System.Collections.Generic;
using System.Linq;

namespace RollGen
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
            var collection = new RollCollection();
            var collections = GetRollCollectionsRecursive(lower, upper, collection);

            var bestMatchingRanking = collections.Min(c => c.GetRanking(lower, upper));
            var bestMatchingCollection = collections.First(c => c.GetRanking(lower, upper) == bestMatchingRanking);

            return bestMatchingCollection;
        }

        private static IEnumerable<RollCollection> GetRollCollectionsRecursive(int lower, int upper, RollCollection existingCollection)
        {
            var collections = new List<RollCollection>();
            var initialCollections = GetCollectionsFromPrototypes(lower, upper, existingCollection);

            if (initialCollections.Count() == 1 && initialCollections.Single() == existingCollection)
                return initialCollections;

            var tooHigh = initialCollections.Where(c => c.Upper > upper);
            var validCollections = initialCollections.Except(tooHigh);

            foreach (var collection in validCollections)
            {
                var newCollections = GetRollCollectionsRecursive(lower, upper, collection);
                collections.AddRange(newCollections);
            }

            return collections;
        }

        private static IEnumerable<RollCollection> GetCollectionsFromPrototypes(int lower, int upper, RollCollection existingCollection)
        {
            var collections = new List<RollCollection>();

            var newLower = lower - existingCollection.Lower;
            var newUpper = upper - existingCollection.Upper;

            if (newLower >= newUpper)
            {
                existingCollection.Adjustment = lower - existingCollection.Quantities;

                return new[] { existingCollection };
            }

            var diceToIgnore = existingCollection.Rolls.Select(r => r.Die).ToArray();
            var prototypes = GetSingleRollPrototypes(newLower, newUpper, diceToIgnore);

            if (!prototypes.Any())
                return new[] { existingCollection };

            foreach (var prototype in prototypes)
            {
                var collection = new RollCollection();
                collection.Rolls.Add(prototype);
                collection.Rolls.AddRange(existingCollection.Rolls);

                collection.Adjustment = lower - collection.Quantities;

                collections.Add(collection);
            }

            return collections;
        }

        private static IEnumerable<RollPrototype> GetSingleRollPrototypes(int lower, int upper, params int[] diceToIgnore)
        {
            if (upper <= lower)
                return Enumerable.Empty<RollPrototype>();

            var dieCap = upper - lower + 1;
            var possibleDice = RollCollection.StandardDice
                .Where(d => d <= upper || d <= dieCap)
                .Where(d => !diceToIgnore.Contains(d));

            var prototypes = new List<RollPrototype>();

            foreach (var die in possibleDice)
            {
                var quantity = dieCap / die;

                var lowerPrototype = GetRollPrototype(quantity, die);
                prototypes.Add(lowerPrototype);

                var upperPrototype = GetRollPrototype(quantity + 1, die);
                prototypes.Add(upperPrototype);
            }

            return prototypes;
        }

        private static RollPrototype GetRollPrototype(int quantity, int die)
        {
            var prototype = new RollPrototype();
            prototype.Quantity = quantity;
            prototype.Die = die;

            return prototype;
        }
    }
}
