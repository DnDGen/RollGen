using Albatross.Expression;
using System;
using System.Text.RegularExpressions;

namespace DnDGen.RollGen.Expressions
{
    internal class AlbatrossExpressionEvaluator : ExpressionEvaluator
    {
        private readonly IParser parser;
        private readonly Regex strictRollRegex;

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

            try
            {
                var unevaluatedMatch = parser.Compile(expression).EvalValue(null);
                var evaluatedExpression = Utils.BooleanOrType<T>(unevaluatedMatch);

                return Utils.ChangeType<T>(evaluatedExpression);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Expression '{expression}' is invalid", e);
            }
        }

        public bool IsValid(string expression)
        {
            //HACK: The "IsValidExpression" method doesn't always handle this correctly and return TRUE when it shouldn't.
            //So, have to use ugly error catching instead.
            //return parser.IsValidExpression(expression);

            var match = strictRollRegex.Match(expression);
            if (match.Success)
                return false;

            try
            {
                var unevaluatedMatch = parser.Compile(expression).EvalValue(null);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
