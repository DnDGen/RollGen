namespace RollGen.Domain.Expressions
{
    internal interface ExpressionEvaluator
    {
        object Evaluate(string expression);
    }
}
