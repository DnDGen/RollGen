namespace RollGen
{
    public interface Dice
    {
        PartialRoll Roll(int quantity = 1);
        int Roll(string roll);
    }
}