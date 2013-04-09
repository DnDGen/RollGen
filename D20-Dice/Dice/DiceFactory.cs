using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public static IDice CreateCryptographicallyStrong()
        {
            var randomWrapper = RandomFactory.CreateCryptographicallyStrong();
            IDice dice = new CoreDice(randomWrapper);

            return dice;
        }
    }
}