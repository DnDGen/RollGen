using System;

namespace D20Dice
{
    public static class DiceFactory
    {
        public static IDice Create()
        {
            var random = new Random();
            return new CoreDice(random);
        }
    }
}