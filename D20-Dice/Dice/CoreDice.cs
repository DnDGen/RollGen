using System;
using D20Dice.RandomWrappers;

namespace D20Dice.Dice
{
    public class CoreDice : IDice
    {
        private IRandomWrapper random;

        public CoreDice(IRandomWrapper random)
        {
            this.random = random;
        }

        private Int32 Roll(Int32 die)
        {
            return random.Next(die) + 1;
        }

        private Int32 Roll(Int32 quantity, Int32 die, Int32 bonus)
        {
            var roll = bonus;

            while (quantity-- > 0)
                roll += Roll(die);

            return roll;
        }

        public Int32 d2(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 2, bonus);
        }

        public Int32 d3(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 3, bonus);
        }

        public Int32 d4(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 4, bonus);
        }

        public Int32 d6(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 6, bonus);
        }

        public Int32 d8(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 8, bonus);
        }

        public Int32 d10(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 10, bonus);
        }

        public Int32 d12(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 12, bonus);
        }

        public Int32 d20(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 20, bonus);
        }

        public Int32 Percentile(Int32 quantity = 1, Int32 bonus = 0)
        {
            return Roll(quantity, 100, bonus);
        }
    }
}