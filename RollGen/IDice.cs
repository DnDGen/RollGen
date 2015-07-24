using System;

namespace RollGen
{
    public interface IDice
    {
        IPartialRoll Roll(Int32 quantity = 1);
    }
}