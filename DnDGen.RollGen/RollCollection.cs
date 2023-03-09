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
                return int.MinValue;

            var rank = 0;
            if (!Rolls.Any())
                return rank;

            rank += Rolls.Count * 100_000_000;
            rank += Quantities * 1_000;
            rank += StandardDice.Max() - Rolls.Max(r => r.Die);

            return rank;
        }

        public long GetRankingForMostEvenDistribution(int lower, int upper)
        {
            if (!Matches(lower, upper))
                return long.MinValue;

            if (!Rolls.Any())
                return 0;

            return ComputeDistribution();
        }

        public long GetAlternativeRankingForMostEvenDistribution(int lower, int upper)
        {
            if (!Matches(lower, upper))
                return long.MinValue;

            if (!Rolls.Any())
                return 0;

            var distribution = ComputeDistribution();
            if (Rolls.Count == 1 || distribution < long.MaxValue)
                return distribution;

            var simplifiedRolls = Rolls.Skip(1).ToList();
            return ComputeDistribution(simplifiedRolls);
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
        public long ComputeDistribution() => ComputeDistribution(Rolls);

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
        public long ComputeDistribution(List<RollPrototype> rollPrototypes)
        {
            var quantities = rollPrototypes.Sum(r => r.Quantity);

            if (quantities == 0)
                return 0;

            if (quantities == 1)
                return 1;

            if (quantities == 2)
                return rollPrototypes.Min(r => r.Die);

            //We want to shortcut that when 1% of the possible iterations for the first die is greater than the max long,
            //Equation for xdy: 0.01y^x = 2^63
            //Solved for x, x = (2ln(10)+63ln(2))/ln(y)
            var quantityLimit = (2 * Math.Log(10) + 63 * Math.Log(2)) / Math.Log(rollPrototypes[0].Die);
            if (rollPrototypes[0].Quantity >= quantityLimit)
                return long.MaxValue;

            var mode = (rollPrototypes.Sum(r => r.Quantity * r.Die) + quantities) / 2;
            var rolls = new Dictionary<int, long>() { { mode, 1 } };
            var remainingMax = Upper - Adjustment;

            for (var i = 0; i < rollPrototypes.Count; i++)
            {
                var nextRolls = Enumerable.Range(1, rollPrototypes[i].Die);
                var q = rollPrototypes[i].Quantity;

                while (q-- > 0)
                {
                    var newRolls = new Dictionary<int, long>();

                    foreach (var r1 in rolls.Where(r => r.Key - remainingMax <= 0))
                    {
                        foreach (var r2 in nextRolls.Where(r => r1.Key - r >= 0))
                        {
                            var newSum = r1.Key - r2;
                            if (!newRolls.ContainsKey(newSum))
                                newRolls[newSum] = 0;

                            newRolls[newSum] += r1.Value;

                            //INFO: This means we went so high that we wrapped around
                            if (newRolls[newSum] < 1)
                                return long.MaxValue;
                        }
                    }

                    remainingMax -= rollPrototypes[i].Die;
                    rolls = newRolls;
                }
            }

            //Since we are subtracting from the mode, the key of 0 is the cumulative number of ways we can roll the mode
            return rolls[0];
        }
    }
}
