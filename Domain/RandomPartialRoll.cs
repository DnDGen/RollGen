using System;
using System.Collections.Generic;

namespace RollGen.Domain
{
    public class RandomPartialRoll : PartialRoll
    {
        private int quantity;
        private readonly Random random;

        public RandomPartialRoll(int quantity, Random random)
        {
            this.quantity = quantity;
            this.random = random;
        }

        public override IEnumerable<int> multi_d(int die)
        {
            while (quantity-- > 0)
                yield return random.Next(die) + 1;
        }
    }
}