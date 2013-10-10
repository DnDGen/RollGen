using System;
using D20Dice.RandomWrappers;

namespace D20Dice.Dice
{
    public class DiceFactory
    {
        public static IDice Create(Random random)
        {
            var randomWrapper = RandomFactory.Create(random);
            IDice dice = new CoreDice(randomWrapper);

            return dice;
        }
    }
}