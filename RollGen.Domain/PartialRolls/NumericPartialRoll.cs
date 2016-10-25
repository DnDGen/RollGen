using RollGen.Domain.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RollGen.Domain.PartialRolls
{
    internal class NumericPartialRoll : PartialRoll
    {
        private readonly Random random;
        private readonly ExpressionEvaluator expressionEvaluator;
        private readonly Regex strictRollRegex;

        private NumericPartialRoll(Random random, ExpressionEvaluator expressionEvaluator)
        {
            this.random = random;
            this.expressionEvaluator = expressionEvaluator;

            strictRollRegex = new Regex(RegexConstants.StrictRollPattern);
        }

        public NumericPartialRoll(int quantity, Random random, ExpressionEvaluator expressionEvaluator)
            : this(random, expressionEvaluator)
        {
            CurrentRollExpression = $"{quantity}";
        }

        public override int AsSum()
        {
            var rolls = GetIndividualRolls(CurrentRollExpression);
            return rolls.Sum();
        }

        public override IEnumerable<int> AsIndividualRolls()
        {
            var rolls = GetIndividualRolls(CurrentRollExpression);
            return rolls;
        }

        public override double AsPotentialAverage()
        {
            var average = GetAverageRoll(CurrentRollExpression);
            return average;
        }

        public override bool AsTrueOrFalse()
        {
            var average = AsPotentialAverage();
            var sum = AsSum();
            return sum >= average;
        }

        public override PartialRoll d(int die)
        {
            CurrentRollExpression += $"d{die}";
            return this;
        }

        public override PartialRoll Keeping(int amountToKeep)
        {
            CurrentRollExpression += $"k{amountToKeep}";
            return this;
        }

        private IEnumerable<int> GetIndividualRolls(string rollExpression)
        {
            //INFO: Not sure how to evaluate individual rolls from genuine expressions (6d5d4k3d2k1), so will compute those as 1 roll
            if (Roll.CanParse(rollExpression) == false)
            {
                var evaluatedExpression = EvaluateExpressionWithRollsAsTotals(rollExpression);
                return new[] { evaluatedExpression };
            }

            var roll = new Roll(rollExpression);
            var rolls = roll.GetRolls(random);

            return rolls;
        }

        private int EvaluateExpressionWithRollsAsTotals(string rollExpression)
        {
            var expression = ReplaceRollsInExpressionWithTotal(rollExpression);
            var evaluatedExpression = expressionEvaluator.Evaluate<int>(expression);
            return evaluatedExpression;
        }

        private double EvaluateExpressionWithRollsAsAverage(string rollExpression)
        {
            var expression = ReplaceRollsInExpressionWithAverage(rollExpression);
            var evaluatedExpression = expressionEvaluator.Evaluate<double>(expression);
            return evaluatedExpression;
        }

        private string ReplaceRollsInExpressionWithTotal(string rollExpression)
        {
            var expressionWithReplacedRolls = rollExpression;
            var match = strictRollRegex.Match(expressionWithReplacedRolls);

            while (match.Success)
            {
                var matchValue = match.Value.Trim();
                var replacement = CreateTotalOfRolls(matchValue);

                expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, matchValue, replacement.ToString());
                match = strictRollRegex.Match(expressionWithReplacedRolls);
            }

            return expressionWithReplacedRolls;
        }

        private string ReplaceRollsInExpressionWithAverage(string rollExpression)
        {
            var expressionWithReplacedRolls = rollExpression;
            var match = strictRollRegex.Match(expressionWithReplacedRolls);

            while (match.Success)
            {
                var matchValue = match.Value.Trim();
                var replacement = GetAverageRoll(matchValue);

                expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, matchValue, replacement.ToString());
                match = strictRollRegex.Match(expressionWithReplacedRolls);
            }

            return expressionWithReplacedRolls;
        }

        private int CreateTotalOfRolls(string rollExpression)
        {
            var rolls = GetIndividualRolls(rollExpression);
            return rolls.Sum();
        }

        private double GetAverageRoll(string rollExpression)
        {
            if (Roll.CanParse(rollExpression) == false)
                return EvaluateExpressionWithRollsAsAverage(rollExpression);

            var roll = new Roll(rollExpression);
            var average = roll.GetPotentialAverage();

            return average;
        }

        private string ReplaceFirst(string source, string target, string replacement)
        {
            var index = source.IndexOf(target);
            source = source.Remove(index, target.Length);
            source = source.Insert(index, replacement);
            return source;
        }
    }
}