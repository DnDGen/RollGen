using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class KeepTests : StressTests
    {
        private Dice dice;
        private Random random;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
            random = GetNewInstanceOf<Random>();
        }

        [Test]
        public void StressKeep()
        {
            stressor.Stress(AssertKeep);
        }

        protected void AssertKeep()
        {
            var quantity = random.Next(Limits.Quantity) + 1;
            var die = random.Next(Limits.Die) + 1;
            var keep = random.Next(quantity - 1) + 1;
            var percentageThreshold = random.NextDouble();
            var rollThreshold = random.Next(quantity * die) + 1;

            var roll = GetRoll(quantity, die, keep);

            AssertRoll(roll, quantity, die, keep, percentageThreshold, rollThreshold);
        }

        private void AssertRoll(PartialRoll roll, int quantity, int die, int keep, double percentageThreshold, int rollThreshold)
        {
            var qMin = Math.Min(quantity, keep);
            var average = qMin * (die + 1) / 2.0d;
            var min = qMin;
            var max = qMin * die;

            Assert.That(roll.AsSum(), Is.InRange(min, max));
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(min));
            Assert.That(roll.AsPotentialMaximum(false), Is.EqualTo(max));
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(max));
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average));
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, "Roll");

            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(qMin));
            Assert.That(rolls, Has.All.InRange(1, max));
        }

        private PartialRoll GetRoll(int quantity, int die, int keep) => dice.Roll(quantity).d(die).Keeping(keep);
    }
}
