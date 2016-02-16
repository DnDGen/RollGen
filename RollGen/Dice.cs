namespace RollGen
{
    public abstract class Dice
    {
        public abstract PartialRoll Roll(int quantity = 1);
        public abstract string RolledString(string roll);
        public abstract object CompiledObj(string rolled);
        public int Compiled(string rolled) => System.Convert.ToInt32(CompiledObj(rolled));
        public int Roll(string roll) => Compiled(RolledString(roll));
    }
}