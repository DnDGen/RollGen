using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RollGen.Domain
{
    public class DomainDice : Dice
    {
        private Regex rollRegex;
        private ExpressionEvaluator expressionEvaluator;
        private PartialRollFactory partialRollFactory;

        public DomainDice(ExpressionEvaluator expressionEvaluator, PartialRollFactory partialRollFactory)
        {
            this.expressionEvaluator = expressionEvaluator;
            this.partialRollFactory = partialRollFactory;

            rollRegex = new Regex("\\d* *d *\\d+");
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
            var expression = RollExpression(roll);
            return Evaluate<int>(expression);
        }

        public T Evaluate<T>(string rolled)
        {
            var rawRoll = Evaluate(rolled);

            if (rawRoll is T)
                return (T)rawRoll;

            return (T)Convert.ChangeType(rawRoll, typeof(T));
        }

        public string RollExpression(string expression)
        {
            var matches = rollRegex.Matches(expression);

            foreach (var match in matches)
            {
                var matchValue = match.ToString();
                var matchIndex = expression.IndexOf(matchValue);

                var rolls = GetIndividualRolls(matchValue);
                var sumOfRolls = string.Join(" + ", rolls);
                var sumOfRollsInParentheses = string.Format("({0})", sumOfRolls);

                expression = expression.Remove(matchIndex, matchValue.Length);
                expression = expression.Insert(matchIndex, sumOfRollsInParentheses);
            }

            return expression.Trim();
        }

        private IEnumerable<int> GetIndividualRolls(string roll)
        {
            var sections = roll.Split('d');
            var quantity = 1;

            if (string.IsNullOrEmpty(sections[0]) == false)
                quantity = Convert.ToInt32(sections[0]);

            var die = Convert.ToInt32(sections[1]);

            return Roll(quantity).IndividualRolls(die);
        }
    }
}