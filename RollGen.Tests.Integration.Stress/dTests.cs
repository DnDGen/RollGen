using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class dTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [TestCase(1, 9266)]
        [TestCase(1, 42)]
        [TestCase(2, 42)]
        [TestCase(3, 42)]
        [TestCase(1, 7)]
        [TestCase(2, 7)]
        [TestCase(3, 7)]
        [TestCase(4, 7)]
        [TestCase(5, 7)]
        [TestCase(1, 60000)]
        [TestCase(Limits.Quantity, 1)]
        public void FullRangeHit(int quantity, int die)
        {
            var expectedCount = die * quantity - (quantity - 1);
            var rolls = Populate(new HashSet<int>(), () => Dice.Roll(quantity).d(die).AsSum(), expectedCount);

            Assert.That(rolls.Min(), Is.EqualTo(quantity));
            Assert.That(rolls.Max(), Is.EqualTo(die * quantity));
            Assert.That(rolls.Count, Is.EqualTo(expectedCount));
        }

        [TestCase(1, Limits.Die)]
        [TestCase(Limits.Quantity, 1)]
        public void RollWithLargestDieRollPossible(int quantity, int die)
        {
            Stress(() => AssertRollWithLargestDieRollPossible(quantity, die));
        }

        [Test]
        public void RollWithLargestDieRollPossible()
        {
            var rootOfLimit = Convert.ToInt32(Math.Floor(Math.Sqrt(Limits.ProductOfQuantityAndDie)));
            Stress(() => AssertRollWithLargestDieRollPossible(rootOfLimit, rootOfLimit));
        }

        private void AssertRollWithLargestDieRollPossible(int quantity, int die)
        {
            var roll = Dice.Roll(quantity).d(die).AsSum();
            Assert.That(roll, Is.InRange(quantity, quantity * die));
        }
    }
}