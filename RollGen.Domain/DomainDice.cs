using RollGen.Domain.Expressions;
using RollGen.Domain.PartialRolls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RollGen.Domain
{
    internal class DomainDice : Dice
    {
        private Regex rollRegex;
        private Regex lenientRollRegex;
        private Regex expressionRegex;
        private ExpressionEvaluator expressionEvaluator;
        private PartialRollFactory partialRollFactory;

        public DomainDice(ExpressionEvaluator expressionEvaluator, PartialRollFactory partialRollFactory)
        {
            this.expressionEvaluator = expressionEvaluator;
            this.partialRollFactory = partialRollFactory;

            const string roll_common_regex = "d *\\d+(?: *k *\\d+)?";
            rollRegex = new Regex($"(?:(?:\\d* +)|(?:\\d+ *)|^){roll_common_regex}");
            lenientRollRegex = new Regex($"\\d* *{roll_common_regex}");
            expressionRegex = new Regex("(?:[-+]?\\d*\\.?\\d+[%/\\+\\-\\*])+(?:[-+]?\\d*\\.?\\d+)");
        }

        public PartialRoll Roll(int quantity = 1)
        {
            if (quantity > Limits.Quantity)
                throw new ArgumentException("Cannot roll more than 46,340 die rolls in a single roll");

            return partialRollFactory.Build(quantity);
        }

        public string ReplaceWrappedExpressions<T>(string source, string expressionOpen = "{", string expressionClose = "}", char? expressionOpenEscape = '\\')
        {
            var pattern = $"{Regex.Escape(expressionOpen)}(.*?){Regex.Escape(expressionClose)}";

            if (expressionOpenEscape != null)
                pattern = $"(?:[^{Regex.Escape(expressionOpenEscape.ToString())}]|^)" + pattern;

            var regex = new Regex(pattern);

            foreach (Match match in regex.Matches(source))
            {
                var matchGroupValue = match.Groups[1].Value;
                var lenientReplacedDice = ReplaceDiceExpression(matchGroupValue, true);
                var unevaluatedMatch = Evaluate(lenientReplacedDice);
                var target = expressionOpen + matchGroupValue + expressionClose;
                var replacement = BooleanOrType<T>(unevaluatedMatch).ToString();

                source = ReplaceFirst(source, target, replacement);
            }

            if (expressionOpenEscape == null)
                return source;

            return source.Replace(expressionOpenEscape + expressionOpen, expressionOpen);
        }

        public object Evaluate(string expression)
        {
            var match = rollRegex.Match(expression);

            if (match.Success)
            {
                var message = string.Format("Cannot evaluate unrolled die roll {0}", match.Value);
                throw new ArgumentException(message);
            }

            return expressionEvaluator.Evaluate(expression);
        }

        public int Roll(string roll)
        {
            var expression = ReplaceExpressionWithTotal(roll, true);
            return Evaluate<int>(expression);
        }

        public T Evaluate<T>(string expression)
        {
            var evaluation = Evaluate(expression);
            return ChangeType<T>(evaluation);
        }

        public T ChangeType<T>(object rawEvaluatedExpression)
        {
            if (rawEvaluatedExpression is T)
                return (T)rawEvaluatedExpression;

            return (T)Convert.ChangeType(rawEvaluatedExpression, typeof(T));
        }

        /// <summary>Returns a string of the provided object as boolean, if it is one, otherwise of Type T.</summary>
        public string BooleanOrType<T>(object rawEvaluatedExpression)
        {
            if (rawEvaluatedExpression is bool)
                return rawEvaluatedExpression.ToString();

            return ChangeType<T>(rawEvaluatedExpression).ToString();
        }

        public string ReplaceRollsWithSum(string expression)
        {
            expression = Replace(expression, rollRegex, s => CreateSumOfRolls(s));

            return expression.Trim();
        }

        private string CreateSumOfRolls(string roll)
        {
            var rolls = GetIndividualRolls(roll);

            if (rolls.Any() == false)
                return "0";

            if (rolls.Count() == 1)
                return rolls.First().ToString();

            var sumOfRolls = string.Join(" + ", rolls);
            return $"({sumOfRolls})";
        }

        private IEnumerable<int> GetIndividualRolls(string roll)
        {
            var sections = roll.Split('d', 'k');
            var die = Convert.ToInt32(sections[1]);
            var quantity = 1;

            if (!string.IsNullOrEmpty(sections[0]))
                quantity = Convert.ToInt32(sections[0]);

            var partialRoll = Roll(quantity);
            var individualRolls = partialRoll.IndividualRolls(die);

            if (sections.Length == 3 && !string.IsNullOrEmpty(sections[2]))
                individualRolls = partialRoll.KeepIndividualRolls(individualRolls, Convert.ToInt32(sections[2]));

            return individualRolls;
        }

        public bool ContainsRoll(string expression, bool lenient = false)
        {
            if (lenient)
                return lenientRollRegex.IsMatch(expression);

            return rollRegex.IsMatch(expression);
        }

        private string ReplaceDiceExpression(string expression, bool lenient = false)
        {
            if (lenient)
                return Replace(expression, lenientRollRegex, s => CreateTotalOfRolls(s));

            return Replace(expression, rollRegex, s => CreateTotalOfRolls(s));
        }

        public string ReplaceExpressionWithTotal(string expression, bool lenient = false)
        {
            expression = ReplaceDiceExpression(expression, lenient);
            expression = Replace(expression, expressionRegex, Evaluate);

            return expression;
        }

        private int CreateTotalOfRolls(string roll)
        {
            var rolls = GetIndividualRolls(roll);
            return rolls.Sum();
        }

        private string ReplaceFirst(string source, string target, string replacement)
        {
            var index = source.IndexOf(target);
            source = source.Remove(index, target.Length);
            source = source.Insert(index, replacement);
            return source;
        }

        private string Replace(string expression, Regex regex, Func<string, object> createReplacement)
        {
            var matches = regex.Matches(expression);

            foreach (Match match in matches)
            {
                var matchValue = match.Value.Trim();
                var replacement = createReplacement(matchValue);
                expression = ReplaceFirst(expression, matchValue, replacement.ToString());
            }

            return expression;
        }
    }
}