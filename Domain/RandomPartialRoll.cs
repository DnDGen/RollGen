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

        public int d2()
        {
            return d(2);
        }

        public int d3()
        {
            return d(3);
        }

        public int d4()
        {
            return d(4);
        }

        public int d6()
        {
            return d(6);
        }

        public int d8()
        {
            return d(8);
        }

        public int d10()
        {
            return d(10);
        }

        public int d12()
        {
            return d(12);
        }

        public int d20()
        {
            return d(20);
        }

        public int Percentile()
        {
            return d(100);
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