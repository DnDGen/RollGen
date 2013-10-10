using System;
using System.Security.Cryptography;

namespace D20Dice.RandomWrappers
{
    public class RandomFactory
    {
        public static IRandomWrapper Create(Random random)
        {
            IRandomWrapper randomWrapper = new RandomWrapper(random);

            return randomWrapper;
        }
    }
}