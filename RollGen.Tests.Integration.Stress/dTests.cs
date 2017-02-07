using Ninject;
using NUnit.Framework;
using System;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class dTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [TestCase(1, Limits.Die)]
        [TestCase(Limits.Quantity, 1)]
        public void RollWithLargestDieRollPossible(int quantity, int die)
        {
            Stress(() => AssertRoll(quantity, die));
        }

        [Test]
        public void RollWithLargestDieRollPossible()
        {
            var rootOfLimit = Convert.ToInt32(Math.Floor(Math.Sqrt(Limits.ProductOfQuantityAndDie)));
            Stress(() => AssertRoll(rootOfLimit, rootOfLimit));
        }

        private void AssertRoll(int quantity, int die)
        {
            var roll = Dice.Roll(quantity).d(die).AsSum();
            Assert.That(roll, Is.InRange(quantity, quantity * die));
        }
    }
}