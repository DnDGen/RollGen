using System;

namespace D20Dice.RandomWrappers
{
    public interface IRandomWrapper
    {
        Int32 Next(Int32 max);
    }
}