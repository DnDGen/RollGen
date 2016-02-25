namespace RollGen
{
    public interface Dice
    {
        PartialRoll Roll(int quantity = 1);
        object Evaluate(string rolled);
        int Roll(string roll);
        T Evaluate<T>(string rolled);
        string RollExpression(string roll);
    }
}
