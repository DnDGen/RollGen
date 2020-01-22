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

            var rank = 0;
            if (!Rolls.Any())
                return rank;

            rank += Quantities * 100_000;
            rank += Rolls.Count * 1_000;
            rank += StandardDice.Max() - Rolls.Max(r => r.Die);

            return rank;
        }

        private int RangeDifference(int lower, int upper)
        {
            var lowerDifference = Math.Abs(lower - Lower);
            var upperDifference = Math.Abs(upper - Upper);

            return lowerDifference + upperDifference;
        }
    }
}
