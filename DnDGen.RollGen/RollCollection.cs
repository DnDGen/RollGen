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

        public IEnumerable<RollPrototype> MultipliedRolls => Rolls.Where(r => r.Multiplier > 1);
        public IEnumerable<RollPrototype> UnmultipliedRolls => Rolls.Where(r => r.Multiplier == 1);
        public int Quantities => UnmultipliedRolls.Sum(r => r.Quantity);
        public int Lower => Quantities + Adjustment;
        public int Upper => MultipliedRolls.Sum(r => r.Quantity * (r.Die - 1) * r.Multiplier)
            + UnmultipliedRolls.Sum(r => r.Quantity * r.Die)
            + Adjustment;

        public RollCollection()
        {
            Rolls = new List<RollPrototype>();
        }

        public string Build()
        {
            if (!Rolls.Any())
                return Adjustment.ToString();

            var rolls = Rolls
                .OrderByDescending(r => r.Multiplier)
                .ThenByDescending(r => r.Die)
                .Select(r => r.Build());
            var allRolls = string.Join("+", rolls);

            if (Adjustment == 0)
                return allRolls;

            if (Adjustment > 0)
                return $"{allRolls}+{Adjustment}";

            return $"{allRolls}{Adjustment}";
        }

        public bool Matches(int lower, int upper)
        {
            return lower == Lower
                && upper == Upper
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
        public long ComputeDistribution()
        {
            if (Quantities == 0)
                return 0;

            if (Quantities == 1)
                return 1;

            if (Quantities == 2)
                return UnmultipliedRolls.Min(r => r.Die);

            var validRolls = UnmultipliedRolls.ToList();
            //We want to shortcut that when 1% of the possible iterations for the first die is greater than the max long,
            //Equation for xdy: 0.01y^x = 2^63
            //Solved for x, x = (2ln(10)+63ln(2))/ln(y)
            var quantityLimit = (2 * Math.Log(10) + 63 * Math.Log(2)) / Math.Log(validRolls[0].Die);
            if (validRolls[0].Quantity >= quantityLimit)
                return long.MaxValue;

            var upper = validRolls.Sum(r => r.Quantity * r.Die);
            var mode = (upper + Quantities) / 2;
            var rolls = new Dictionary<int, long>() { { mode, 1 } };
            var remainingMax = upper;

            //HACK: This means we are going to do too many iterations below
            //The Die Limit is only used when computing extra-large ranges with non-standard dice
            //Otherwise, non-standard dice should always have a quantity of 1
            //Therefore, we can just shortcut this specific usecase
            if (validRolls[0].Die == Limits.Die)
            {
                return validRolls[0].Quantity switch
                {
                    3 => 75_000_000, //0.75% * 10,000^3 = 75,000,000
                    4 => 666_666_670_000,
                    5 => 5_989_583_343_750_000,
                    _ => long.MaxValue
                };
            }

            for (var i = 0; i < validRolls.Count; i++)
            {
                var nextRolls = Enumerable.Range(1, validRolls[i].Die);
                var q = validRolls[i].Quantity;

                while (q-- > 0)
                {
                    var newRolls = new Dictionary<int, long>();

                    foreach (var r1 in rolls.Where(r => r.Key <= remainingMax))
                    {
                        foreach (var r2 in nextRolls.Where(r => r1.Key >= r))
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

                    remainingMax -= validRolls[i].Die;
                    rolls = newRolls;
                }
            }

            //Since we are subtracting from the mode, the key of 0 is the cumulative number of ways we can roll the mode
            return rolls[0];
        }
    }
}
