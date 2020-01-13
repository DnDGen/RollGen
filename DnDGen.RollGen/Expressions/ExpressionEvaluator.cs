namespace RollGen.Expressions
{
    internal interface ExpressionEvaluator
    {
        T Evaluate<T>(string expression);
    }
}
