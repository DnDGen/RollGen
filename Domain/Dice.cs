using System;

namespace RollGen.Domain
{
    public class Dice : IDice
    {
        private readonly Random random;

        public Dice(Random random)
        {
            this.random = random;
        }

        public IPartialRoll Roll(Int32 quantity = 1)
        {
            return new PartialRoll(quantity, random);
        }
    }
}