using System;

namespace RollGen
{
    public abstract class Dice
    {
        public abstract PartialRoll Roll(int quantity = 1);
        public abstract string RollString(string roll);
        public abstract object CompileRaw(string rolled);
        public int Roll(string roll) => Roll<int>(roll);

        public T Roll<T>(string roll)
        {
            var rollString = RollString(roll);
            var rawRoll = CompileRaw(rollString);

            if (rawRoll is T)
                return (T)rawRoll;

            try
            {
                return (T)Convert.ChangeType(rawRoll, typeof(T));
            }
            catch (InvalidCastException)
            {
                return default(T);
            }
        }
    }
}