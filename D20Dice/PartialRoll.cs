using System;

namespace D20Dice
{
    public class PartialRoll : IPartialRoll
    {
        private Int32 quantity;
        private Random random;

        public PartialRoll(Int32 quantity, Random random)
        {
            this.quantity = quantity;
            this.random = random;
        }

        public Int32 d2()
        {
            return d(2);
        }

        public Int32 d3()
        {
            return d(3);
        }

        public Int32 d4()
        {
            return d(4);
        }

        public Int32 d6()
        {
            return d(6);
        }

        public Int32 d8()
        {
            return d(8);
        }

        public Int32 d10()
        {
            return d(10);
        }

        public Int32 d12()
        {
            return d(12);
        }

        public Int32 d20()
        {
            return d(20);
        }

        public Int32 Percentile()
        {
            return d(100);
        }

        public Int32 d(Int32 die)
        {
            var roll = 0;

            while (quantity-- > 0)
                roll += random.Next(die) + 1;

            return roll;
        }
    }
}