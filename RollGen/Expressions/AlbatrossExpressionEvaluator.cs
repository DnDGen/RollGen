using Albatross.Expression;
using System;
using System.Text.RegularExpressions;

namespace RollGen.Expressions
{
    internal class AlbatrossExpressionEvaluator : ExpressionEvaluator
    {
        private const string CommonRollRegexPattern = "d *\\d+(?: *k *\\d+)?";

        private IParser parser;
        private Regex strictRollRegex;

        public AlbatrossExpressionEvaluator(IParser parser)
        {
            this.parser = parser;
            strictRollRegex = new Regex(RegexConstants.StrictRollPattern);
        }

        public T Evaluate<T>(string expression)
        {
            var match = strictRollRegex.Match(expression);

            if (match.Success)
                throw new ArgumentException($"Cannot evaluate unrolled die roll {match.Value}");

            var unevaluatedMatch = parser.Compile(expression).EvalValue(null);
            var evaluatedExpression = Utils.BooleanOrType<T>(unevaluatedMatch);

            return Utils.ChangeType<T>(evaluatedExpression);
        }
    }
}
