namespace RollGen.Domain.Expressions
{
    internal interface ExpressionEvaluator
    {
        T Evaluate<T>(string expression);
    }
}
