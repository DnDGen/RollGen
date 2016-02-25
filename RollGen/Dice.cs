namespace RollGen
{
    public interface Dice
    {
        PartialRoll Roll(int quantity = 1);
        object Evaluate(string expression);
        int Roll(string roll);
        T Evaluate<T>(string expression);
        string RollExpression(string expression);
        bool ContainsRoll(string expression);
    }
}
