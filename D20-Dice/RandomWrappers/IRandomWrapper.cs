using System;

namespace D20_Dice.RandomWrappers
{
    public interface IRandomWrapper
    {
        Int32 Next(Int32 min, Int32 max);
        Int32 Next(Int32 max);
    }
}