using Albatross.Expression;

namespace RollGen.Domain
{
    public class AlbatrossExpressionEvaluator : ExpressionEvaluator
    {
        private IParser parser;

        public AlbatrossExpressionEvaluator(IParser parser)
        {
            this.parser = parser;
        }

        public object Evaluate(string expression)
        {
            return parser.Compile(expression).EvalValue(null);
        }
    }
}
