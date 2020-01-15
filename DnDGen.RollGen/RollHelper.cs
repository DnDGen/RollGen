using System;
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
            var collection = new RollCollection();
            //var collections = GetRollCollectionsRecursive(lower, upper, collection);
            var collections = MessAround(lower, upper);

            var bestMatchingRanking = collections.Min(c => c.GetRanking(lower, upper));
            var bestMatchingCollection = collections.First(c => c.GetRanking(lower, upper) == bestMatchingRanking);

            return bestMatchingCollection;
        }

        private static IEnumerable<RollCollection> MessAround(int lower, int upper)
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

            foreach (var die in dice)
            {
                foreach (var collection in collections)
                {
                    if (collection.Matches(lower, upper))
                        continue;

                    var remainingUpper = upper - collection.Upper;
                    var remainingLower = lower - collection.Lower;
                    var remainingRange = remainingUpper - remainingLower + 1;

                    if (remainingRange < die)
                        continue;

                    //TODO: Deal with multiple rolls possible, such as (1d10 + 1d2) and (2d6)

                    var additionalPrototype = BuildRollPrototype(remainingLower, remainingUpper, die);

                    collection.Rolls.Add(additionalPrototype);
                    collection.Adjustment = lower - collection.Quantities;
                }

                var dieCollection = new RollCollection();
                var diePrototype = BuildRollPrototype(lower, upper, die);

                dieCollection.Rolls.Add(diePrototype);
                dieCollection.Adjustment = lower - dieCollection.Quantities;

                collections.Add(dieCollection);
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

        private static IEnumerable<RollCollection> GetRollCollectionsRecursive(int lower, int upper, RollCollection existingCollection)
        {
            if (existingCollection.Matches(lower, upper))
            {
                return new[] { existingCollection };
            }

            var collectionsWithAddedRolls = AddRolls(lower, upper, existingCollection);

            if (collectionsWithAddedRolls.FirstOrDefault() == existingCollection)
            {
                return new[] { existingCollection };
            }

            var matchingCollections = new List<RollCollection>();
            var validCollections = collectionsWithAddedRolls.Where(c => c.Upper <= upper);

            foreach (var collection in validCollections)
            {
                var newCollections = GetRollCollectionsRecursive(lower, upper, collection);
                newCollections = newCollections.Where(c => c.Matches(lower, upper));

                matchingCollections.AddRange(newCollections);
            }

            return matchingCollections;
        }

        private static IEnumerable<RollCollection> AddRolls(int lower, int upper, RollCollection existingCollection)
        {
            var newLower = lower - existingCollection.Lower;
            var newUpper = upper - existingCollection.Upper;

            if (newLower >= newUpper)
            {
                existingCollection.Adjustment = lower - existingCollection.Quantities;

                yield return existingCollection;
                yield break;
            }

            var diceToIgnore = existingCollection.Rolls.Select(r => r.Die).ToArray();
            var prototypes = GetSingleRollPrototypes(newLower, newUpper, diceToIgnore);

            if (!prototypes.Any())
            {
                yield return existingCollection;
                yield break;
            }

            var range = upper - lower + 1;

            foreach (var prototype in prototypes)
            {
                var collection = new RollCollection();
                collection.Rolls.Add(prototype);
                collection.Rolls.AddRange(existingCollection.Rolls);

                if (collection.Range > range)
                    continue;

                collection.Adjustment = lower - collection.Quantities;

                if (collection.Upper > upper)
                    continue;

                yield return collection;
            }

            yield break;
        }

        private static IEnumerable<RollPrototype> GetSingleRollPrototypes(int lower, int upper, params int[] diceToIgnore)
        {
            if (upper <= lower)
                yield break;

            var range = upper - lower + 1;
            var possibleDice = RollCollection.StandardDice
                .Where(d => d <= range)
                .Where(d => !diceToIgnore.Contains(d));

            foreach (var die in possibleDice)
            {
                var quantity = range / die;
                var prototype = new RollPrototype
                {
                    Quantity = quantity++,
                    Die = die
                };

                do
                {
                    yield return prototype;

                    prototype = new RollPrototype
                    {
                        Quantity = quantity++,
                        Die = die
                    };
                }
                while (prototype.Range <= range);
            }

            yield break;
        }
    }
}
