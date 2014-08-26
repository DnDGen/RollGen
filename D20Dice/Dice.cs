using System;

namespace D20Dice
{
    public class Dice : IDice
    {
        private Random random;

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