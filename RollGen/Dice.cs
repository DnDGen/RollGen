namespace RollGen
{
    public abstract class Dice
    {
        public abstract PartialRoll Roll(int quantity = 1);
        public abstract string RolledString(string roll);
        public abstract int Compiled(string rolled);
        public int Roll(string roll) => Compiled(RolledString(roll));
    }
}