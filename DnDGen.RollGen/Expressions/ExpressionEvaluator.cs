namespace DnDGen.RollGen.Expressions
{
    internal interface ExpressionEvaluator
    {
        T Evaluate<T>(string expression);
        bool IsValid(string expression);
    }
}
