using System;

namespace D20Dice
{
    public class CoreDice : IDice
    {
        private Random random;

        public CoreDice(Random random)
        {
            this.random = random;
        }

        private Int32 Roll(Int32 die)
        {
            return random.Next(die) + 1;
        }

        private Int32 Roll(Int32 quantity, Int32 die)
        {
            var roll = 0;

            while (quantity-- > 0)
                roll += Roll(die);

            return roll;
        }

        public Int32 d2(Int32 quantity = 1)
        {
            return Roll(quantity, 2);
        }

        public Int32 d3(Int32 quantity = 1)
        {
            return Roll(quantity, 3);
        }

        public Int32 d4(Int32 quantity = 1)
        {
            return Roll(quantity, 4);
        }

        public Int32 d6(Int32 quantity = 1)
        {
            return Roll(quantity, 6);
        }

        public Int32 d8(Int32 quantity = 1)
        {
            return Roll(quantity, 8);
        }

        public Int32 d10(Int32 quantity = 1)
        {
            return Roll(quantity, 10);
        }

        public Int32 d12(Int32 quantity = 1)
        {
            return Roll(quantity, 12);
        }

        public Int32 d20(Int32 quantity = 1)
        {
            return Roll(quantity, 20);
        }

        public Int32 Percentile(Int32 quantity = 1)
        {
            return Roll(quantity, 100);
        }
    }
}