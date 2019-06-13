using RollGen.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RollGen.PartialRolls
{
    internal class ExpressionPartialRoll : PartialRoll
    {
        private readonly Random random;
        private readonly ExpressionEvaluator expressionEvaluator;
        private readonly Regex strictRollRegex;
        private readonly Regex booleanExpressionRegex;

        private ExpressionPartialRoll(Random random, ExpressionEvaluator expressionEvaluator)
        {
            this.random = random;
            this.expressionEvaluator = expressionEvaluator;

            strictRollRegex = new Regex(RegexConstants.StrictRollPattern);
            booleanExpressionRegex = new Regex(RegexConstants.BooleanExpressionPattern);
        }

        public ExpressionPartialRoll(string rollExpression, Random random, ExpressionEvaluator expressionEvaluator)
            : this(random, expressionEvaluator)
        {
            CurrentRollExpression = rollExpression;
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
            if (booleanExpressionRegex.IsMatch(CurrentRollExpression))
                return EvaluateExpressionWithRollsAsTrueOrFalse(CurrentRollExpression);

            var minimum = AsPotentialMinimum();
            var maximum = AsPotentialMaximum();
            var sum = AsSum();
            var difference = minimum - 1;
            var percentage = (sum - difference) / (double)(maximum - difference);

            return percentage <= threshold;
        }

        private bool EvaluateExpressionWithRollsAsTrueOrFalse(string rollExpression)
        {
            var expression = ReplaceRollsInExpression(rollExpression, GetTotalOfRolls);
            var evaluatedExpression = expressionEvaluator.Evaluate<bool>(expression);
            return evaluatedExpression;
        }

        public override PartialRoll d(int die)
        {
            throw new NotImplementedException("Cannot yet implement paranthetical expressions");
        }

        public override PartialRoll Keeping(int amountToKeep)
        {
            throw new NotImplementedException("Cannot yet implement paranthetical expressions");
        }

        private IEnumerable<int> GetIndividualRolls(string rollExpression)
        {
            //INFO: Not sure how to evaluate individual rolls from genuine expressions (1d6+5 or 2d3+4d5), so will compute those as 1 roll
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
