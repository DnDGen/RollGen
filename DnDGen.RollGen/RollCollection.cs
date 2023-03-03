using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.RollGen
{
    internal class RollCollection
    {
        public static int[] StandardDice = new[] { 2, 3, 4, 6, 8, 10, 12, 20, 100 };

        public List<RollPrototype> Rolls { get; private set; }
        public int Adjustment { get; set; }

        public int Quantities => Rolls.Sum(r => r.Quantity);
        public int Lower => Quantities + Adjustment;
        public int Upper => Rolls.Sum(r => r.Quantity * r.Die) + Adjustment;
        public int Range => Upper - Lower + 1;

        public RollCollection()
        {
            Rolls = new List<RollPrototype>();
        }

        public string Build()
        {
            var rolls = Rolls.OrderByDescending(r => r.Die).Select(r => r.Build());
            var allRolls = string.Join("+", rolls);

            if (!Rolls.Any())
                return Adjustment.ToString();

            if (Adjustment == 0)
                return allRolls;

            if (Adjustment > 0)
                return $"{allRolls}+{Adjustment}";

            return $"{allRolls}{Adjustment}";
        }

        public bool Matches(int lower, int upper)
        {
            return RangeDifference(lower, upper) == 0
                && !Rolls.Any(r => !r.IsValid);
        }

        public override string ToString()
        {
            return Build();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RollCollection))
                return false;

            var collection = obj as RollCollection;

            return collection.Build() == Build();
        }

        public override int GetHashCode()
        {
            return Build().GetHashCode();
        }

        public int GetRankingForFewestDice(int lower, int upper)
        {
            if (!Matches(lower, upper))
                return int.MaxValue;

            var rank = 0;
            if (!Rolls.Any())
                return rank;

            rank += Rolls.Count * 100_000_000;
            rank += Quantities * 1_000;
            rank += StandardDice.Max() - Rolls.Max(r => r.Die);

            return rank;
        }

        public int GetRankingForMostEvenDistribution(int lower, int upper)
        {
            if (!Matches(lower, upper))
                return int.MaxValue;

            if (!Rolls.Any())
                return 0;

            return ComputeDistribution();
        }

        private int RangeDifference(int lower, int upper)
        {
            var lowerDifference = Math.Abs(lower - Lower);
            var upperDifference = Math.Abs(upper - Upper);

            return lowerDifference + upperDifference;
        }

        /// <summary>
        /// This computes the Distribution (D) of a set of dice rolls. 1dX has a D = 1, or perfect distribution. D = 4 means the average occurs in 4 permutations.
        /// The methodology this follows was explained by Jasper Flick at anydice.com
        /// His explanation:
        /// 
        ///  AnyDice is based on combinatorics and probability theory. It treats dice as sets of (value, probability) tuples. An operation like d2 + d2 goes like this:
        ///
        ///  { (1, 1/2), (2, 1/2) } + { (1, 1/2), (1, 1/2) } = { (1 + 1 = 2, 1 / 2 * 1 / 2 = 1 / 4) + (1 + 2 or 2 + 1 = 3, 1 / 4 + 1 / 4 = 1 / 2) +(2 + 2 = 4, 1 / 4) } = { (2, 1 / 4), (3, 1 / 2), (4, 1 / 4) }
        ///
        ///  10d10 has 10^10 ordered permutations, but you don't care about order when summing so we're dealing with unordered sampling with replacement (19 choose 10) which is only 92378 permutations.You just need to keep track of the probabilities. That's what the set approach does.
        ///  I already showed you d2 + d2, which yields the set for 2d2. To get 3d2 you do the exact same thing but now 2d2 + d2. The same way you can go from d10 + d10 to 10d10. It can be done with a double loop that's blazingly fast, calculating 100d10 in a few microseconds.
        ///
        /// </summary>
        /// <returns></returns>
        public int ComputeDistribution()
        {
            if (Quantities == 0)
                return 0;

            if (Quantities == 1)
                return 1;

            if (Quantities == 2)
                return Rolls.Min(r => r.Die);

            var permutations = 1L;

            try
            {
                permutations = Rolls
                    .Select(r => Convert.ToInt64(Math.Pow(r.Die, r.Quantity)))
                    .Aggregate(1L, (acc, val) => acc * val);
            }
            catch
            {
                return int.MaxValue;
            }

            //var probabilities = Enumerable.Range(1, Rolls[0].Die).ToDictionary(r => r, r => 1d / Rolls[0].Die);
            //var mode = (Rolls.Sum(r => r.Quantity * r.Die) + Quantities) / 2;
            //var remainingMax = permutations;

            //for (var i = 0; i < Rolls.Count; i++)
            //{
            //    if (i == Rolls.Count - 1)
            //        remainingMax = 0;
            //    else
            //        remainingMax /= Convert.ToInt64(Math.Pow(Rolls[i].Die, Rolls[i].Quantity));

            //    var q = Rolls[i].Quantity;
            //    if (i == 0)
            //        q--;

            //    while (q-- > 0)
            //    {
            //        var newProbabilities = Enumerable.Range(1, Rolls[i].Die).ToDictionary(r => r, r => 1d / Rolls[i].Die);
            //        var newTotals = new Dictionary<int, double>();

            //        foreach (var p1 in probabilities)
            //        {
            //            foreach (var p2 in newProbabilities)
            //            {
            //                var newSum = p1.Key + p2.Key;
            //                if (newSum + q * Rolls[i].Die + remainingMax < mode)
            //                    continue;

            //                if (newSum > mode)
            //                    continue;

            //                if (!newTotals.ContainsKey(newSum))
            //                    newTotals[newSum] = 0;

            //                newTotals[newSum] += p1.Value * p2.Value;
            //            }
            //        }

            //        probabilities = newTotals;
            //    }
            //}

            //try
            //{
            //    return Convert.ToInt32(probabilities[mode] * permutations);
            //}
            //catch
            //{
            //    return int.MaxValue;
            //}

            var mode = (Rolls.Sum(r => r.Quantity * r.Die) + Quantities) / 2;
            var rolls = new Dictionary<int, int>() { { mode, 1 } };
            var remainingMax = Upper - Adjustment;

            for (var i = 0; i < Rolls.Count; i++)
            {
                var nextRolls = Enumerable.Range(1, Rolls[i].Die);
                var q = Rolls[i].Quantity;

                while (q-- > 0)
                {
                    var newRolls = new Dictionary<int, int>();

                    foreach (var r1 in rolls.Where(r => r.Key - remainingMax <= 0))
                    {
                        foreach (var r2 in nextRolls.Where(r => r1.Key - r >= 0))
                        {
                            var newSum = r1.Key - r2;
                            if (!newRolls.ContainsKey(newSum))
                                newRolls[newSum] = 0;

                            newRolls[newSum] += r1.Value;
                        }
                    }

                    remainingMax -= Rolls[i].Die;
                    rolls = newRolls;
                }
            }

            return rolls[0];
        }
    }
}
