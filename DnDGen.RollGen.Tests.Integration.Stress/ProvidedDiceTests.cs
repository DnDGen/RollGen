using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class ProvidedDiceTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        protected abstract int die { get; }

        protected abstract PartialRoll GetRoll(int quantity);

        protected void AssertRoll()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(quantity * die) + 1;

            var roll = GetRoll(quantity);

            AssertRoll(roll, quantity, percentageThreshold, rollThreshold);
        }

        private void AssertRoll(PartialRoll roll, int quantity, double percentageThreshold, int rollThreshold)
        {
            var average = quantity * (die + 1) / 2.0d;

            Assert.That(roll.AsSum(), Is.InRange(quantity, quantity * die));
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(quantity));
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(quantity * die));
            Assert.That(roll.AsPotentialMaximum(true), Is.EqualTo(quantity * die));
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average));
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, "Roll");

            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(quantity));
            Assert.That(rolls, Has.All.InRange(1, die));
        }
    }
}