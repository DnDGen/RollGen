using System;

namespace D20Dice
{
    public interface IDice
    {
        IPartialRoll Roll(Int32 quantity = 1);
    }
}