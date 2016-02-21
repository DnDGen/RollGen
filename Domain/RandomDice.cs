using Albatross.Expression;
using System;

namespace RollGen.Domain
{
    public class RandomDice : Dice
    {
        private readonly Random random;

        public RandomDice(Random random)
        {
            this.random = random;
        }

        public override PartialRoll Roll(int quantity = 1)
        {
            return new RandomPartialRoll(quantity, random);
        }

        public override object Compute(string rolled)
        {
            var unrolledDieRolls = rollRegex.Matches(rolled);

            if (unrolledDieRolls.Count > 0)
            {
                var message = string.Format("Cannot compute unrolled die roll {0}", unrolledDieRolls[0]);
                throw new ArgumentException(message);
            }

            return Parser.GetParser().Compile(rolled).EvalValue(null);
        }
    }
}
