using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ExpressionTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpressionAsSum(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpressionAsSum(expression, lower, upper));
        }

        private void AssertExpressionAsSum(string expression, int lower, int upper)
        {
            var roll = Dice.Roll(expression).AsSum();
            Assert.That(roll, Is.InRange(lower, upper));
        }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpressionAsIndividualRolls(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpressionAsIndividualRolls(expression, lower, upper));
        }

        private void AssertExpressionAsIndividualRolls(string expression, int lower, int upper)
        {
            var rolls = Dice.Roll(expression).AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.InRange(lower, upper));
        }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpressionAsMinimum(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpressionAssMinimum(expression, lower, upper));
        }

        private void AssertExpressionAssMinimum(string expression, int lower, int upper)
        {
            var min = Dice.Roll(expression).AsPotentialMinimum();
            Assert.That(min, Is.EqualTo(lower));
        }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpressionAsMaximum(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpressionAsMaximum(expression, lower, upper));
        }

        private void AssertExpressionAsMaximum(string expression, int lower, int upper)
        {
            var max = Dice.Roll(expression).AsPotentialMaximum();
            Assert.That(max, Is.EqualTo(upper));
        }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpressionAsAverage(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpressionAsAverage(expression, lower, upper));
        }

        private void AssertExpressionAsAverage(string expression, int lower, int upper)
        {
            var roll = Dice.Roll(expression).AsPotentialAverage();
            var average = (lower + upper) / 2d;

            Assert.That(roll, Is.EqualTo(average));
        }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpressionAsTrueOrFalse(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpressionAsTrueOrFalse(expression, lower, upper));
        }

        private void AssertExpressionAsTrueOrFalse(string expression, int lower, int upper)
        {
            var roll = Dice.Roll(expression);
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(upper) + 1;

            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, "Roll");
        }
    }
}
