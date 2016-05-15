using Ninject;
using NUnit.Framework;
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
        [TestCase(6, 7)]
        [TestCase(1, Limits.Die)]
        [TestCase(Limits.Quantity, 1)]
        public void FullRangeHit(int quantity, int die)
        {
            var expectedCount = die * quantity - (quantity - 1);
            var rolls = Populate(new HashSet<int>(), () => Dice.Roll(quantity).d(die), expectedCount);

            Assert.That(rolls.Min(), Is.EqualTo(quantity));
            Assert.That(rolls.Max(), Is.EqualTo(die * quantity));
            Assert.That(rolls.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void RollWithLargestDieRollPossible()
        {
            Stress(AssertRollWithLargestDieRollPossible);
        }

        private void AssertRollWithLargestDieRollPossible()
        {
            var roll = Dice.Roll(Limits.Quantity).d(Limits.Die);
            Assert.That(roll, Is.InRange(Limits.Quantity, Limits.Quantity * Limits.Die));
        }
    }
}