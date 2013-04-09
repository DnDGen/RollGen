using System;

namespace D20Dice.Dice
{
    public interface IDice
    {
        Int32 d10(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d12(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d2(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d20(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d3(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d4(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d6(Int32 quantity = 1, Int32 bonus = 0);
        Int32 d8(Int32 quantity = 1, Int32 bonus = 0);
        Int32 Percentile(Int32 quantity = 1, Int32 bonus = 0);
    }
}