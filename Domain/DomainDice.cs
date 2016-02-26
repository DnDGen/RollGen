using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RollGen.Domain
{
    public class DomainDice : Dice
    {
        private Regex rollRegex;
        private Regex expressionRegex;
        private ExpressionEvaluator expressionEvaluator;
        private PartialRollFactory partialRollFactory;

        public DomainDice(ExpressionEvaluator expressionEvaluator, PartialRollFactory partialRollFactory)
        {
            this.expressionEvaluator = expressionEvaluator;
            this.partialRollFactory = partialRollFactory;

            rollRegex = new Regex("((\\d* +)|(\\d+ *)|(^))d *\\d+");
            expressionRegex = new Regex("([-+]?[0-9]*\\.?[0-9]+[%\\/\\+\\-\\*])+([-+]?[0-9]*\\.?[0-9]+)");
        }

        public PartialRoll Roll(int quantity = 1)
        {
            return partialRollFactory.Build(quantity);
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
            var expression = ReplaceExpressionWithTotal(roll);
            return Evaluate<int>(expression);
        }

        public T Evaluate<T>(string expression)
        {
            var rawEvaluatedExpression = Evaluate(expression);

            if (rawEvaluatedExpression is T)
                return (T)rawEvaluatedExpression;

            return (T)Convert.ChangeType(rawEvaluatedExpression, typeof(T));
        }

        public string ReplaceRollsWithSum(string expression)
        {
            expression = Replace(expression, rollRegex, s => CreateSumOfRolls(s));

            return expression.Trim();
        }

        private string CreateSumOfRolls(string roll)
        {
            var rolls = GetIndividualRolls(roll);
            var sumOfRolls = string.Join(" + ", rolls);
            return string.Format("({0})", sumOfRolls);
        }

        private IEnumerable<int> GetIndividualRolls(string roll)
        {
            var sections = roll.Split('d');
            var die = Convert.ToInt32(sections[1]);
            var quantity = 1;

            if (string.IsNullOrEmpty(sections[0]) == false)
                quantity = Convert.ToInt32(sections[0]);

            return Roll(quantity).IndividualRolls(die);
        }

        public bool ContainsRoll(string expression)
        {
            var match = rollRegex.Match(expression);
            return match.Success;
        }

        public string ReplaceExpressionWithTotal(string expression)
        {
            expression = Replace(expression, rollRegex, s => CreateTotalOfRolls(s));
            expression = Replace(expression, expressionRegex, Evaluate);

            return expression;
        }

        private int CreateTotalOfRolls(string roll)
        {
            var rolls = GetIndividualRolls(roll);
            return rolls.Sum();
        }

        private string Replace(string expression, Regex regex, Func<string, object> createReplacement)
        {
            var matches = regex.Matches(expression);

            foreach (var match in matches)
            {
                var matchValue = match.ToString().Trim();
                var matchIndex = expression.IndexOf(matchValue);
                var replacement = createReplacement(matchValue);

                expression = expression.Remove(matchIndex, matchValue.Length);
                expression = expression.Insert(matchIndex, replacement.ToString());
            }

            return expression;
        }
    }
}