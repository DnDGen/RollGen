using Ninject;
using NUnit.Framework;
using System;

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
        public void Roll()
        {
            stressor.Stress(AssertRoll);
        }

        [TestCase("1d2", "3d4", 1, 24)]
        [TestCase("3d4-1", "5d6+2", 2, 352)]
        public void RollWithExpression(string quantityExpression, string dieExpression, int lower, int upper)
        {
            stressor.Stress(() => AssertRoll(quantityExpression, dieExpression, lower, upper));
        }

        private void AssertRoll()
        {
            var quantity = Random.Next(1000) + 1;
            var die = Random.Next(100_000) + 1;

            var roll = Dice.Roll(quantity).d(die).AsSum();
            Assert.That(roll, Is.InRange(quantity, quantity * die));
        }

        private void AssertRoll(string quantity, string die, int lower, int upper)
        {
            var roll = Dice.Roll(quantity).d(die).AsSum();
            Assert.That(roll, Is.InRange(lower, upper));
        }
    }
}