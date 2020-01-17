using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class dTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void RollAsSum()
        {
            stressor.Stress(AssertRollAsSum);
        }

        [Test]
        public void RollAsIndividualRolls()
        {
            stressor.Stress(AssertRollAsIndividualRolls);
        }

        [Test]
        public void RollAsMinimum()
        {
            stressor.Stress(AssertRollAsMinimum);
        }

        [Test]
        public void RollAsMaximum()
        {
            stressor.Stress(AssertRollAsMaximum);
        }

        [Test]
        public void RollAsAverage()
        {
            stressor.Stress(AssertRollAsAverage);
        }

        [Test]
        public void RollAsTrueOrFalse()
        {
            stressor.Stress(AssertRollAsTrueOrFalse);
        }

        private PartialRoll GetRoll(int quantity, int die)
        {
            return Dice.Roll(quantity).d(die);
        }

        private void AssertRollAsSum()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsSum(), Is.InRange(quantity, quantity * die));
        }

        private void AssertRollAsIndividualRolls()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;

            var roll = GetRoll(quantity, die);
            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(quantity));
            Assert.That(rolls, Has.All.InRange(1, die));
        }

        private void AssertRollAsAverage()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;

            var roll = GetRoll(quantity, die);
            var average = quantity * (die + 1) / 2.0d;
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average));
        }

        private void AssertRollAsMinimum()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(quantity));
        }

        private void AssertRollAsMaximum()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(die * quantity));
        }

        protected void AssertRollAsTrueOrFalse()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;

            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(quantity * die) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, "Roll");
        }
    }
}