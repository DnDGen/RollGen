using System;

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

        public int d(int die)
        {
            var roll = 0;

            while (quantity-- > 0)
                roll += random.Next(die) + 1;

            return roll;
        }
    }
}