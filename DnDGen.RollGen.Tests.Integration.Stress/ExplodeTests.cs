using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ExplodeTests : StressTests
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
        public void StressExplode(bool common)
        {
            stressor.Stress(() => AssertExplode(common));
        }

        protected void AssertExplode(bool common)
        {
            var quantityLimit = common ? 100 : Limits.Quantity;
            var dieLimit = common ? 100 : Limits.Die;

            var quantity = random.Next(quantityLimit) + 1;
            var die = random.Next(dieLimit - 1) + 2; //INFO: Can't allow d1, as explode fails on that
            var explode = random.Next(die - 1) + 1;
            var percentageThreshold = random.NextDouble();
            var rollThreshold = random.Next(quantity * die) + 1;

            Assert.That(die, Is.InRange(2, dieLimit));

            var roll = GetRoll(quantity, die, explode);

            AssertRoll(roll, quantity, die, explode, percentageThreshold, rollThreshold);
        }

        private void AssertRoll(PartialRoll roll, int quantity, int die, int explode, double percentageThreshold, int rollThreshold)
        {
            var rollMin = explode == 1 && die != 1 ? 2 : 1;
            var min = quantity * rollMin;
            var max = quantity * die;
            var average = (min + max) / 2.0d;

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

        private PartialRoll GetRoll(int quantity, int die, int explode) => dice.Roll(quantity).d(die).ExplodeOn(explode);
    }
}
