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
            var match = rollRegex.Match(rolled);

            if (match.Success)
            {
                var message = string.Format("Cannot compute unrolled die roll {0}", match.Value);
                throw new ArgumentException(message);
            }

            return Parser.GetParser().Compile(rolled).EvalValue(null);
        }
    }
}
