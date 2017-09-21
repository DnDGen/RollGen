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

        public override int AsPotentialMinimum()
        {
            var minimum = GetMinimumRoll(CurrentRollExpression);
            return minimum;
        }

        public override int AsPotentialMaximum()
        {
            var maximum = GetMaximumRoll(CurrentRollExpression);
            return maximum;
        }

        public override bool AsTrueOrFalse(double threshold = .5)
        {
            var minimum = AsPotentialMinimum();
            var maximum = AsPotentialMaximum();
            var sum = AsSum();
            var difference = minimum - 1;
            var percentage = (sum - difference) / (double)(maximum - difference);

            return percentage <= threshold;
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
                var evaluatedExpression = EvaluateExpression(rollExpression, GetTotalOfRolls);
                return new[] { evaluatedExpression };
            }

            var roll = new Roll(rollExpression);
            var rolls = roll.GetRolls(random);

            return rolls;
        }

        private T EvaluateExpression<T>(string rollExpression, Func<string, T> getRoll)
        {
            var expression = ReplaceRollsInExpression(rollExpression, getRoll);
            var evaluatedExpression = expressionEvaluator.Evaluate<T>(expression);
            return evaluatedExpression;
        }

        private string ReplaceRollsInExpression<T>(string rollExpression, Func<string, T> getRoll)
        {
            var expressionWithReplacedRolls = rollExpression;
            var match = strictRollRegex.Match(expressionWithReplacedRolls);

            while (match.Success)
            {
                var matchValue = match.Value.Trim();
                var replacement = getRoll(matchValue);

                expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, matchValue, replacement.ToString());
                match = strictRollRegex.Match(expressionWithReplacedRolls);
            }

            return expressionWithReplacedRolls;
        }

        private int GetTotalOfRolls(string rollExpression)
        {
            var rolls = GetIndividualRolls(rollExpression);
            return rolls.Sum();
        }

        private double GetAverageRoll(string rollExpression)
        {
            if (Roll.CanParse(rollExpression) == false)
                return EvaluateExpression(rollExpression, GetAverageRoll);

            var roll = new Roll(rollExpression);
            var average = roll.GetPotentialAverage();

            return average;
        }

        private int GetMinimumRoll(string rollExpression)
        {
            if (Roll.CanParse(rollExpression) == false)
                return EvaluateExpression(rollExpression, GetMinimumRoll);

            var roll = new Roll(rollExpression);
            var minimum = roll.GetPotentialMinimum();

            return minimum;
        }

        private int GetMaximumRoll(string rollExpression)
        {
            if (Roll.CanParse(rollExpression) == false)
                return EvaluateExpression(rollExpression, GetMaximumRoll);

            var roll = new Roll(rollExpression);
            var maximum = roll.GetPotentialMaximum();

            return maximum;
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