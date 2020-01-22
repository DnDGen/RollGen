using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ExplodeTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void StressExplode()
        {
            stressor.Stress(AssertExplode);
        }

        protected void AssertExplode()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die - 1) + 2; //INFO: Can't allow d1, as explode fails on that
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(quantity * die) + 1;

            Assert.That(die, Is.InRange(2, Limits.Die));

            var roll = GetRoll(quantity, die);

            AssertRoll(roll, quantity, die, percentageThreshold, rollThreshold);
        }

        private void AssertRoll(PartialRoll roll, int quantity, int die, double percentageThreshold, int rollThreshold)
        {
            var average = quantity * (die + 1) / 2.0d;
            var min = quantity;
            var max = quantity * die;

            Assert.That(roll.AsSum(), Is.InRange(min, max * 10));
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(min));
            Assert.That(roll.AsPotentialMaximum(false), Is.EqualTo(max));
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(max * 10));
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average));
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, "Roll");

            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.AtLeast(quantity));
            Assert.That(rolls, Has.All.InRange(1, die));
        }

        private PartialRoll GetRoll(int quantity, int die) => Dice.Roll(quantity).d(die).Explode();
    }
}
