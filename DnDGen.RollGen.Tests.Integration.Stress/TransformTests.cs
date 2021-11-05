using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class TransformTests : StressTests
    {
        private Dice dice;
        private Random random;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
            random = GetNewInstanceOf<Random>();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void StressTransform(bool common)
        {
            stressor.Stress(() => AssertTransform(common));
        }

        protected void AssertTransform(bool common)
        {
            var quantityLimit = common ? 100 : Limits.Quantity;
            var dieLimit = common ? 100 : Limits.Die;

            var quantity = random.Next(quantityLimit) + 1;
            var die = random.Next(dieLimit) + 1;
            var transform = random.Next(die - 1) + 1;
            var percentageThreshold = random.NextDouble();
            var rollThreshold = random.Next(quantity * die) + 1;

            var roll = GetRoll(quantity, die, transform);

            AssertRoll(roll, quantity, die, transform, percentageThreshold, rollThreshold);
        }

        private void AssertRoll(PartialRoll roll, int quantity, int die, int transform, double percentageThreshold, int rollThreshold)
        {
            var rollMin = transform == 1 && die != 1 ? 2 : 1;
            var min = quantity * rollMin;
            var max = quantity * die;
            var average = (min + max) / 2.0d;

            Assert.That(min, Is.LessThanOrEqualTo(max), roll.CurrentRollExpression);
            Assert.That(roll.AsSum(), Is.InRange(min, max), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(min), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum(false), Is.EqualTo(max), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(max), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average), roll.CurrentRollExpression);
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, "Roll");

            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(quantity));
            Assert.That(rolls, Has.All.InRange(rollMin, max));
        }

        private PartialRoll GetRoll(int quantity, int die, int transform) => dice.Roll(quantity).d(die).Transforming(transform);
    }
}
