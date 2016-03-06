namespace RollGen
{
    public interface Dice
    {
        PartialRoll Roll(int quantity = 1);
        object Evaluate(string expression);
        int Roll(string roll);
        T Evaluate<T>(string expression);
        string ReplaceRollsWithSum(string expression);
        bool ContainsRoll(string expression);
        string ReplaceExpressionWithTotal(string expression, bool lenient = false);
    }
}
