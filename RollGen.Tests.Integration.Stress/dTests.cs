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
        [TestCase(6, 7)]
        public void FullRangeHit(int quantity, int die)
        {
            var expectedCount = die * quantity - (quantity - 1);
            var rolls = Populate(new HashSet<int>(), () => Dice.Roll(quantity).d(die), expectedCount);

            Assert.That(rolls.Min(), Is.EqualTo(quantity));
            Assert.That(rolls.Max(), Is.EqualTo(die * quantity));
            Assert.That(rolls.Count, Is.EqualTo(expectedCount));
        }

        [TestCase(1, 1)]
        [TestCase(10, 1)]
        [TestCase(100, 1)]
        [TestCase(1000, 1)]
        [TestCase(10000, 1)]
        [TestCase(100000, 1)]
        [TestCase(1000000, 1)]
        [TestCase(10000000, 1)] //INFO: Any more than this causes the list of rolls to throw an OutOfMemoryException, so this is the inclusive upper limit of supported rolls
        [TestCase(1, 10)]
        [TestCase(1, 100)]
        [TestCase(1, 1000)]
        [TestCase(1, 10000)]
        [TestCase(1, 100000)]
        [TestCase(1, 1000000)]
        [TestCase(1, 10000000)]
        [TestCase(1, 100000000)]
        [TestCase(1, 1000000000)] //INFO: Any more than this is a long, not an int, so this is the inclusive upper limit of supported rolls
        [TestCase(10, 10)]
        [TestCase(100, 100)]
        [TestCase(1000, 1000)]
        [TestCase(10000, 10000)]
        public void RollWithALargeDieRoll(int quantity, int die)
        {
            Stress(() => AssertRollWithALargeDieRoll(quantity, die));
        }

        private void AssertRollWithALargeDieRoll(int quantity, int die)
        {
            var roll = Dice.Roll(quantity).d(die);
            Assert.That(roll, Is.InRange(quantity, quantity * die));
        }

        [TestCase(100000, 100000)]
        public void CauseArithmeticOverflow(int quantity, int die)
        {
            Stress(() => AssertArithmeticOverflow(quantity, die));
        }

        private void AssertArithmeticOverflow(int quantity, int die)
        {
            Assert.That(() => Dice.Roll(quantity).d(die), Throws.InstanceOf<OverflowException>());
        }
    }
}