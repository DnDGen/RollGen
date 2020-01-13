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

        private PartialRoll GetRoll(int quantity, int die)
        {
            return Dice.Roll(quantity).d(die);
        }

        private void AssertRollAsSum()
        {
            var quantity = Random.Next(1000) + 1;
            var die = Random.Next(100_000) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsSum(), Is.InRange(quantity, quantity * die));
        }

        private void AssertRollAsIndividualRolls()
        {
            var quantity = Random.Next(1000) + 1;
            var die = Random.Next(100_000) + 1;

            var roll = GetRoll(quantity, die);
            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(quantity));
            Assert.That(rolls, Has.All.InRange(1, die));
        }

        private void AssertRollAsAverage()
        {
            var quantity = Random.Next(1000) + 1;
            var die = Random.Next(100_000) + 1;

            var roll = GetRoll(quantity, die);
            var average = quantity * (die + 1) / 2.0d;
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average));
        }

        private void AssertRollAsMinimum()
        {
            var quantity = Random.Next(1000) + 1;
            var die = Random.Next(100_000) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(quantity));
        }

        private void AssertRollAsMaximum()
        {
            var quantity = Random.Next(1000) + 1;
            var die = Random.Next(100_000) + 1;

            var roll = GetRoll(quantity, die);
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(die * quantity));
        }

        [TestCase("1d2", "3d4", 1, 24)]
        [TestCase("3d4-1", "5d6+2", 2, 352)]
        public void RollWithExpression(string quantityExpression, string dieExpression, int lower, int upper)
        {
            stressor.Stress(() => AssertRoll(quantityExpression, dieExpression, lower, upper));
        }

        private void AssertRoll(string quantity, string die, int lower, int upper)
        {
            var roll = Dice.Roll(quantity).d(die).AsSum();
            Assert.That(roll, Is.InRange(lower, upper));
        }
    }
}