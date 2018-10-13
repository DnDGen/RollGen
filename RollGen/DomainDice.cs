using RollGen.Expressions;
using RollGen.PartialRolls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RollGen
{
    internal class DomainDice : Dice
    {
        private PartialRollFactory partialRollFactory;
        private readonly Regex expressionRegex = new Regex(RegexConstants.ExpressionWithoutDieRollsPattern);
        private readonly Regex strictRollRegex = new Regex(RegexConstants.StrictRollPattern);
        private readonly Regex lenientRollRegex = new Regex(RegexConstants.LenientRollPattern);

        public DomainDice(PartialRollFactory partialRollFactory)
        {
            this.partialRollFactory = partialRollFactory;
        }

        public PartialRoll Roll(int quantity = 1)
        {
            if (quantity > Limits.Quantity)
                throw new ArgumentException($"Cannot roll more than {Limits.Quantity} die rolls at a time");

            return partialRollFactory.Build(quantity);
        }

        public PartialRoll Roll(string rollExpression)
        {
            return partialRollFactory.Build(rollExpression);
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
                var replacement = Evaluate<T>(matchGroupValue);

                var target = expressionOpen + matchGroupValue + expressionClose;
                source = ReplaceFirst(source, target, replacement);
            }

            if (expressionOpenEscape == null)
                return source;

            return source.Replace(expressionOpenEscape + expressionOpen, expressionOpen);
        }

        private string Evaluate<T>(string expression)
        {
            var partialRoll = partialRollFactory.Build(expression);

            if (typeof(T) == typeof(bool))
                return partialRoll.AsTrueOrFalse().ToString();

            return partialRoll.AsSum().ToString();
        }

        public string ReplaceRollsWithSumExpression(string expression, bool lenient = false)
        {
            var rollRegex = GetRollRegex(lenient);
            expression = Replace(expression, rollRegex, s => GetRollAsSumExpression(s));

            return expression.Trim();
        }

        private Regex GetRollRegex(bool lenient)
        {
            return lenient ? lenientRollRegex : strictRollRegex;
        }

        private string GetRollAsSumExpression(string roll)
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
            var partialRoll = partialRollFactory.Build(roll);
            return partialRoll.AsIndividualRolls();
        }

        public bool ContainsRoll(string expression, bool lenient = false)
        {
            var regex = GetRollRegex(lenient);
            return regex.IsMatch(expression);
        }

        private string ReplaceDiceExpression(string expression, bool lenient = false)
        {
            var regex = GetRollRegex(lenient);
            return Replace(expression, regex, s => CreateTotalOfRolls(s));
        }

        public string ReplaceExpressionWithTotal(string expression, bool lenient = false)
        {
            expression = ReplaceDiceExpression(expression, lenient);
            expression = Replace(expression, expressionRegex, s => Evaluate<int>(s));

            return expression;
        }

        private int CreateTotalOfRolls(string roll)
        {
            var partialRoll = partialRollFactory.Build(roll);
            return partialRoll.AsSum();
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
            var expressionWithReplacedRolls = expression;
            var match = regex.Match(expressionWithReplacedRolls);

            while (match.Success)
            {
                var matchValue = match.Value.Trim();
                var replacement = createReplacement(matchValue);

                expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, matchValue, replacement.ToString());
                match = regex.Match(expressionWithReplacedRolls);
            }

            return expressionWithReplacedRolls;
        }
    }
}