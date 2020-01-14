using System.Collections.Generic;

namespace DnDGen.RollGen
{
    public abstract class PartialRoll
    {
        public string CurrentRollExpression { get; protected set; }

        public abstract PartialRoll d(int die);
        public abstract PartialRoll d(string die);
        public abstract PartialRoll Keeping(int amountToKeep);
        public abstract PartialRoll Keeping(string amountToKeep);
        public abstract PartialRoll Explode();

        public abstract int AsSum();
        public abstract IEnumerable<int> AsIndividualRolls();
        public abstract double AsPotentialAverage();
        public abstract int AsPotentialMinimum();
        public abstract int AsPotentialMaximum(bool includeExplode = true);
        public abstract bool AsTrueOrFalse(double threshold = .5);
        public abstract bool AsTrueOrFalse(int threshold);

        public PartialRoll d2() => d(2);
        public PartialRoll d3() => d(3);
        public PartialRoll d4() => d(4);
        public PartialRoll d6() => d(6);
        public PartialRoll d8() => d(8);
        public PartialRoll d10() => d(10);
        public PartialRoll d12() => d(12);
        public PartialRoll d20() => d(20);
        public PartialRoll Percentile() => d(100);
    }
}