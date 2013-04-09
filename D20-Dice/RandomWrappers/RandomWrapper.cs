using System;

namespace D20Dice.RandomWrappers
{
    public class RandomWrapper : IRandomWrapper
    {
        private Random random;

        public RandomWrapper(Random random)
        {
            this.random = random;
        }

        public Int32 Next(Int32 max)
        {
            return random.Next(max);
        }
    }
}