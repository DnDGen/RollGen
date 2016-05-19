using System;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Domain.PartialRolls
{
    internal class RandomPartialRoll : PartialRoll
    {
        private readonly int quantity;
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

            var rolls = Enumerable.Repeat(die, quantity).Select(x => random.Next(x) + 1);

            return rolls;
        }
    }
}