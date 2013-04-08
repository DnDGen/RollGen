using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D20_Dice.RandomWrappers
{
    public class RandomWrapper : IRandomWrapper
    {
        private Random random;

        public RandomWrapper(Random random)
        {
            this.random = random;
        }

        public Int32 Next(Int32 min, Int32 max)
        {
            return random.Next(min, max);
        }

        public Int32 Next(Int32 max)
        {
            return random.Next(max);
        }
    }
}