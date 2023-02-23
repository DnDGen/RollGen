using DnDGen.RollGen.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DnDGen.RollGen.PartialRolls
{
    internal class DomainPartialRoll : PartialRoll
    {
        private readonly Random random;
        private readonly ExpressionEvaluator expressionEvaluator;
        private readonly Regex strictRollRegex;
        private readonly Regex booleanExpressionRegex;

        private DomainPartialRoll(Random random, ExpressionEvaluator expressionEvaluator)
        {
            this.random = random;
            this.expressionEvaluator = expressionEvaluator;

            strictRollRegex = new Regex(RegexConstants.StrictRollPattern);
            booleanExpressionRegex = new Regex(RegexConstants.BooleanExpressionPattern);
        }

        public DomainPartialRoll(double quantity, Random random, ExpressionEvaluator expressionEvaluator)
            : this(random, expressionEvaluator)
        {
            CurrentRollExpression = $"{quantity}";
        }

        public DomainPartialRoll(string quantity, Random random, ExpressionEvaluator expressionEvaluator)
            : this(random, expressionEvaluator)
        {
            CurrentRollExpression = $"({quantity})";
        }

        public override T AsSum<T>()
        {
            var rolls = GetIndividualRolls<T>(CurrentRollExpression);
            var values = rolls.Select(r => Utils.ChangeType<double>(r));
            var sum = values.Sum();

            return Utils.ChangeType<T>(sum);
        }

        public override IEnumerable<T> AsIndividualRolls<T>()
        {
            var rolls = GetIndividualRolls<T>(CurrentRollExpression);
            return rolls;
        }

        public override double AsPotentialAverage()
        {
            var average = GetAverageRoll(CurrentRollExpression);
            return average;
        }

        public override T AsPotentialMinimum<T>()
        {
            var minimum = GetMinimumRoll<T>(CurrentRollExpression);
            return minimum;
        }

        public override T AsPotentialMaximum<T>(bool includeExplode = true)
        {
            var maximum = GetMaximumRoll<T>(CurrentRollExpression, includeExplode);
            return maximum;
        }

        /// <summary>
        /// Return the value as True or False, depending on if it is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, on a roll of a 1d10 with threshold = .7, rolling a 7 produces False, while 8 produces True.
        /// </summary>
        /// <param name="threshold">The non-inclusive lower-bound percentage of success</param>
        /// <returns></returns>
        public override bool AsTrueOrFalse(double threshold = .5)
        {
            if (booleanExpressionRegex.IsMatch(CurrentRollExpression))
                return EvaluateExpressionWithRollsAsTrueOrFalse(CurrentRollExpression);

            var minimumAdjustment = AsPotentialMinimum<double>() - 1;
            var range = AsPotentialMaximum<double>(false) - minimumAdjustment;

            if (range == 1)
            {
                return threshold < 1;
            }

            var product = range * threshold;
            var rollThreshold = product + minimumAdjustment;

            if (IsInteger(rollThreshold))
                rollThreshold += .5;

            rollThreshold = Math.Ceiling(rollThreshold);
            var roll = Convert.ToInt32(rollThreshold);
            return AsTrueOrFalse(roll);
        }

        private bool IsInteger(double value)
        {
            return Math.Abs(value % 1) <= (double.Epsilon * 100);
        }

        /// <summary>
        /// Return the value as True or False, depending on if it is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, on a roll of a 1d10 with threshold = 7, rolling a 6 produces False, while 7 produces True.
        /// </summary>
        /// <param name="threshold">The inclusive lower-bound roll value of success</param>
        /// <returns></returns>
        public override bool AsTrueOrFalse(int threshold)
        {
            if (booleanExpressionRegex.IsMatch(CurrentRollExpression))
                return EvaluateExpressionWithRollsAsTrueOrFalse(CurrentRollExpression);

            var sum = AsSum<double>();
            return sum >= threshold;
        }

        private bool EvaluateExpressionWithRollsAsTrueOrFalse(string rollExpression)
        {
            var expression = ReplaceRollsInExpression(rollExpression, e => GetTotalOfRolls<double>(e));
            var evaluatedExpression = expressionEvaluator.Evaluate<bool>(expression);
            return evaluatedExpression;
        }

        public override PartialRoll d(int die)
        {
            CurrentRollExpression += $"d{die}";
            return this;
        }

        public override PartialRoll d(string die)
        {
            CurrentRollExpression += $"d({die})";
            return this;
        }

        public override PartialRoll Keeping(int amountToKeep)
        {
            CurrentRollExpression += $"k{amountToKeep}";
            return this;
        }

        public override PartialRoll Keeping(string amountToKeep)
        {
            CurrentRollExpression += $"k({amountToKeep})";
            return this;
        }

        public override PartialRoll Explode()
        {
            CurrentRollExpression += "!";
            return this;
        }

        public override PartialRoll Transforming(int rollToTransform, int? transformTarget = null)
        {
            CurrentRollExpression += $"t{rollToTransform}";

            if (transformTarget != null)
                CurrentRollExpression += $":{transformTarget}";

            return this;
        }

        public override PartialRoll Transforming(string rollToTransform, string transformTarget)
        {
            CurrentRollExpression += $"t({rollToTransform})";

            if (transformTarget != null)
                CurrentRollExpression += $":({transformTarget})";

            return this;
        }

        public override PartialRoll ExplodeOn(int rollToExplode)
        {
            CurrentRollExpression += $"e{rollToExplode}";
            return this;
        }

        public override PartialRoll ExplodeOn(string rollToExplode)
        {
            CurrentRollExpression += $"e({rollToExplode})";
            return this;
        }

        private IEnumerable<T> GetIndividualRolls<T>(string rollExpression)
        {
            var trimmedExpression = rollExpression;

            while (CanTrimParanthesesFromExpression(trimmedExpression))
            {
                trimmedExpression = trimmedExpression[1..^1];
            }

            //INFO: Not sure how to evaluate individual rolls from genuine expressions (6d5d4k3d2k1), so will compute those as 1 roll
            if (Roll.CanParse(trimmedExpression))
            {
                var roll = new Roll(trimmedExpression);
                var rolls = roll.GetRolls(random);

                return rolls.Select(r => Utils.ChangeType<T>(r));
            }

            //INFO: Not sure how to evaluate individual rolls from genuine expressions (6d5d4k3d2k1), so will compute those as 1 roll
            var evaluatedExpression = EvaluateExpression(trimmedExpression, e => GetTotalOfRolls<T>(e));
            return new[] { evaluatedExpression };
        }

        private bool CanTrimParanthesesFromExpression(string rollExpression)
        {
            var wrappedInParantheses = rollExpression.StartsWith('(') && rollExpression.EndsWith(')');
            if (!wrappedInParantheses)
                return false;

            var trimmed = rollExpression[1..^1];
            if (!trimmed.Intersect(new[] { '(', ')' }).Any())
                return true;

            var startCount = 0;
            var endCount = 0;

            for (var i = 0; i < trimmed.Length; i++)
            {
                if (trimmed[i] == '(')
                    startCount++;
                else if (trimmed[i] == ')')
                    endCount++;

                if (endCount > startCount)
                    return false;
            }

            return true;
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

            while (CanTrimParanthesesFromExpression(expressionWithReplacedRolls))
            {
                expressionWithReplacedRolls = expressionWithReplacedRolls[1..^1];
            }

            if (expressionEvaluator.IsValid(expressionWithReplacedRolls))
                return expressionWithReplacedRolls;

            //1. Replace paranthetical expressions
            while (expressionWithReplacedRolls.Contains('('))
            {
                var openParanthesisIndex = expressionWithReplacedRolls.IndexOf('(');
                var innerExpressionLength = GetInnerExpressionLength(expressionWithReplacedRolls, openParanthesisIndex);
                var innerExpression = expressionWithReplacedRolls.Substring(openParanthesisIndex + 1, innerExpressionLength);

                //var innerExpressionWithReplacedRolls = EvaluateExpression(innerExpression, getRoll);
                var replacement = ReplaceRollsInExpression(innerExpression, getRoll);

                if (expressionEvaluator.IsValid(replacement))
                {
                    replacement = expressionEvaluator.Evaluate<T>(replacement).ToString();

                    if (NeedLeadingMultiplier(expressionWithReplacedRolls))
                    {
                        replacement = $"*{replacement}";
                    }

                    if (NeedFollowingMultiplier(expressionWithReplacedRolls))
                    {
                        replacement = $"{replacement}*";
                    }

                    expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, $"({innerExpression})", replacement);
                }
                //INFO: This means we might be inside a function declaration
                else
                {
                    //This means we didn't change anything and shoud move on
                    if (replacement == innerExpression)
                        break;

                    expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, $"({innerExpression})", $"({replacement})");
                }

                if (expressionEvaluator.IsValid(expressionWithReplacedRolls))
                    return expressionWithReplacedRolls;
            }

            //2. Replace rolls
            var match = strictRollRegex.Match(expressionWithReplacedRolls);

            while (match.Success)
            {
                var matchIndex = expressionWithReplacedRolls.IndexOf(match.Value);
                if ((matchIndex > 0
                        && expressionWithReplacedRolls[matchIndex - 1] == '.')
                    || (expressionWithReplacedRolls.Length > matchIndex + match.Value.Length
                        && expressionWithReplacedRolls[matchIndex + match.Value.Length] == '.'))
                {
                    throw new ArgumentException($"Cannot have decimal values for die rolls: {expressionWithReplacedRolls}");
                }

                var matchValue = match.Value.Trim();
                var replacement = getRoll(matchValue);

                expressionWithReplacedRolls = ReplaceFirst(expressionWithReplacedRolls, matchValue, replacement.ToString());
                match = strictRollRegex.Match(expressionWithReplacedRolls);
            }

            return expressionWithReplacedRolls;
        }

        private bool NeedLeadingMultiplier(string expression)
        {
            var openParanthesisIndex = expression.IndexOf('(');
            if (openParanthesisIndex < 1)
                return false;

            var leadingCharacter = expression[openParanthesisIndex - 1];

            return char.IsDigit(leadingCharacter) || leadingCharacter == ')';
        }

        private bool NeedFollowingMultiplier(string expression)
        {
            var openParanthesisIndex = expression.IndexOf('(');
            if (openParanthesisIndex < 0)
                return false;

            var innerExpressionLength = GetInnerExpressionLength(expression, openParanthesisIndex);
            var followingIndex = openParanthesisIndex + innerExpressionLength + 2;

            if (followingIndex >= expression.Length)
                return false;

            var followingCharacter = expression[followingIndex];

            return char.IsDigit(followingCharacter) || followingCharacter == '(';
        }

        private int GetInnerExpressionLength(string expression, int openingIndex)
        {
            var innerCount = 0;
            var closingIndex = 0;
            for (var i = openingIndex + 1; i < expression.Length; i++)
            {
                if (innerCount == 0 && expression[i] == ')')
                {
                    closingIndex = i;
                    break;
                }

                if (expression[i] == '(')
                {
                    innerCount++;
                }
                else if (expression[i] == ')')
                {
                    innerCount--;
                }
            }

            if (closingIndex == 0)
            {
                throw new InvalidOperationException($"No closing paranthesis found for expression '{expression}'");
            }

            return closingIndex - openingIndex - 1;
        }

        private T GetTotalOfRolls<T>(string rollExpression)
        {
            var rolls = GetIndividualRolls<int>(rollExpression);
            var sum = rolls.Sum();
            return Utils.ChangeType<T>(sum);
        }

        private double GetAverageRoll(string rollExpression)
        {
            if (!Roll.CanParse(rollExpression))
                return EvaluateExpression(rollExpression, GetAverageRoll);

            var roll = new Roll(rollExpression);
            var average = roll.GetPotentialAverage();

            return average;
        }

        private T GetMinimumRoll<T>(string rollExpression)
        {
            if (!Roll.CanParse(rollExpression))
                return EvaluateExpression<T>(rollExpression, e => GetMinimumRoll<T>(e));

            var roll = new Roll(rollExpression);
            var minimum = roll.GetPotentialMinimum();

            return Utils.ChangeType<T>(minimum);
        }

        private T GetMaximumRoll<T>(string rollExpression, bool includeExplode)
        {
            if (!Roll.CanParse(rollExpression))
                return EvaluateExpression<T>(rollExpression, e => GetMaximumRoll<T>(e, includeExplode));

            var roll = new Roll(rollExpression);
            var maximum = roll.GetPotentialMaximum(includeExplode);

            return Utils.ChangeType<T>(maximum);
        }

        private string ReplaceFirst(string source, string target, string replacement)
        {
            var index = source.IndexOf(target);
            source = source.Remove(index, target.Length);
            source = source.Insert(index, replacement);
            return source;
        }

        public override PartialRoll Plus(string expression)
        {
            CurrentRollExpression += $"+({expression})";
            return this;
        }

        public override PartialRoll Plus(double value)
        {
            CurrentRollExpression += $"+{value}";
            return this;
        }

        public override PartialRoll Minus(string expression)
        {
            CurrentRollExpression += $"-({expression})";
            return this;
        }

        public override PartialRoll Minus(double value)
        {
            CurrentRollExpression += $"-{value}";
            return this;
        }

        public override PartialRoll Times(string expression)
        {
            CurrentRollExpression += $"*({expression})";
            return this;
        }

        public override PartialRoll Times(double value)
        {
            CurrentRollExpression += $"*{value}";
            return this;
        }

        public override PartialRoll DividedBy(string expression)
        {
            CurrentRollExpression += $"/({expression})";
            return this;
        }

        public override PartialRoll DividedBy(double value)
        {
            CurrentRollExpression += $"/{value}";
            return this;
        }

        public override PartialRoll Modulos(string expression)
        {
            CurrentRollExpression += $"%({expression})";
            return this;
        }

        public override PartialRoll Modulos(double value)
        {
            CurrentRollExpression += $"%{value}";
            return this;
        }
    }
}