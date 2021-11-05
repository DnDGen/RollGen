using System.Collections.Generic;

namespace DnDGen.RollGen
{
    public abstract class PartialRoll
    {
        public string CurrentRollExpression { get; protected set; }

        public abstract PartialRoll Plus(string expression);
        public abstract PartialRoll Plus(double value);
        public PartialRoll Plus(PartialRoll roll) => Plus(roll.CurrentRollExpression);
        public abstract PartialRoll Minus(string expression);
        public abstract PartialRoll Minus(double value);
        public PartialRoll Minus(PartialRoll roll) => Minus(roll.CurrentRollExpression);
        public abstract PartialRoll Times(string expression);
        public abstract PartialRoll Times(double value);
        public PartialRoll Times(PartialRoll roll) => Times(roll.CurrentRollExpression);
        public abstract PartialRoll DividedBy(string expression);
        public abstract PartialRoll DividedBy(double value);
        public PartialRoll DividedBy(PartialRoll roll) => DividedBy(roll.CurrentRollExpression);
        public abstract PartialRoll Modulos(string expression);
        public abstract PartialRoll Modulos(double value);
        public PartialRoll Modulos(PartialRoll roll) => Modulos(roll.CurrentRollExpression);

        public abstract PartialRoll d(int die);
        public abstract PartialRoll d(string die);
        public PartialRoll d(PartialRoll roll) => d(roll.CurrentRollExpression);
        public abstract PartialRoll Keeping(int amountToKeep);
        public abstract PartialRoll Keeping(string amountToKeep);
        public PartialRoll Keeping(PartialRoll roll) => Keeping(roll.CurrentRollExpression);
        public abstract PartialRoll Explode();
        public abstract PartialRoll ExplodeOn(int rollToExplode);
        public abstract PartialRoll ExplodeOn(string rollToExplode);
        public PartialRoll ExplodeOn(PartialRoll roll) => ExplodeOn(roll.CurrentRollExpression);
        public abstract PartialRoll Transforming(int rollToTransform);
        public abstract PartialRoll Transforming(string rollToTransform);
        public PartialRoll Transforming(PartialRoll roll) => Transforming(roll.CurrentRollExpression);

        public int AsSum() => AsSum<int>();
        public abstract T AsSum<T>();
        public IEnumerable<int> AsIndividualRolls() => AsIndividualRolls<int>();
        public abstract IEnumerable<T> AsIndividualRolls<T>();
        public abstract double AsPotentialAverage();
        public int AsPotentialMinimum() => AsPotentialMinimum<int>();
        public abstract T AsPotentialMinimum<T>();
        public int AsPotentialMaximum(bool includeExplode = true) => AsPotentialMaximum<int>(includeExplode);
        public abstract T AsPotentialMaximum<T>(bool includeExplode = true);

        /// <summary>
        /// Return the value as True or False, depending on if it is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, on a roll of a 1d10 with threshold = .7, rolling a 7 produces False, while 8 produces True.
        /// </summary>
        /// <param name="threshold">The non-inclusive lower-bound percentage of success</param>
        /// <returns></returns>
        public abstract bool AsTrueOrFalse(double threshold = .5);

        /// <summary>
        /// Return the value as True or False, depending on if it is higher or lower then the threshold.
        /// A value less than the threshold is false.
        /// A value equal to or higher than the threshold is true.
        /// As an example, on a roll of a 1d10 with threshold = 7, rolling a 6 produces False, while 7 produces True.
        /// </summary>
        /// <param name="threshold">The inclusive lower-bound roll value of success</param>
        /// <returns></returns>
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