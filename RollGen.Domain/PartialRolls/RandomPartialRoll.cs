using System;
using System.Collections.Generic;

namespace RollGen.Domain.PartialRolls
{
    internal class RandomPartialRoll : PartialRoll
    {
        private int quantity;
        private readonly Random random;

        public RandomPartialRoll(int quantity, Random random)
        {
            this.quantity = quantity;
            this.random = random;
        }

        public override IEnumerable<int> IndividualRolls(int die)
        {
            if (quantity > Limits.Quantity || die > Limits.Die || die * (long)quantity > Limits.ProductOfQuantityAndDie)
                throw new ArgumentException($"Die roll of {quantity}d{die} is too large for RollGen");

            var rolls = new List<int>();

            while (quantity-- > 0)
            {
                var roll = random.Next(die) + 1;
                rolls.Add(roll);
            }

            return rolls;
        }
    }
}