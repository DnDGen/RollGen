using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ExpressionTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [Test]
        public void RollExpressionWithNoDieRolls()
        {
            var roll = Dice.Roll("1+2-(3*4/5)%6");
            Assert.That(roll, Is.EqualTo(1));
        }

        [Test]
        public void RollExpressionWithADieRoll()
        {
            Stress(AssertExpressionWithADieRoll);
        }

        private void AssertExpressionWithADieRoll()
        {
            var roll = Dice.Roll("1d2+3");
            Assert.That(roll, Is.InRange(4, 5));
        }

        [Test]
        public void RollExpressionWithMultipleDieRolls()
        {
            Stress(AssertExpressionWithMultipleDieRolls);
        }

        private void AssertExpressionWithMultipleDieRolls()
        {
            var roll = Dice.Roll("1d2+3d4");
            Assert.That(roll, Is.InRange(4, 14));
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
            var roll = Dice.Roll($"{quantity}d{die}");
            Assert.That(roll, Is.InRange(quantity, quantity * die));
        }

        [TestCase("3d6+2", 5, 20)]
        [TestCase("9d2+66", 75, 84)]
        [TestCase("1d60 - 4d2", -7, 56)]
        [TestCase("1337d1", 1337, 1337)]
        [TestCase("   1d2   +   1d3   -   1d4  ", -2, 4)]
        [TestCase("1d8 +min(1d4/1d2, 3)", 2, 11)]
        [TestCase("3 d 3-1 d 2", 1, 8)]
        [TestCase("1d3*5-(1d5-1)", 1, 15)]
        [TestCase("1d2*3-(1d3-1)", 1, 6)]
        [TestCase("1d2*3-1d4", -1, 5)]
        public void FullRangeHit(string expression, int minimum, int maximum)
        {
            var expectedCount = maximum - (minimum - 1);
            var rolls = Populate(new HashSet<int>(), () => Dice.Roll(expression), expectedCount);

            Assert.That(rolls.Min(), Is.EqualTo(minimum));
            Assert.That(rolls.Max(), Is.EqualTo(maximum));
            Assert.That(rolls.Count, Is.EqualTo(expectedCount));
        }

        [TestCase("1d2*3d4", 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24)]
        [TestCase("2d3*4", 8, 12, 16, 20, 24)]
        [TestCase("1d3*5", 5, 10, 15)]
        [TestCase("1d2*3", 3, 6)]
        [TestCase("1d3*5-1d4", 1, 2, 3, 4, 6, 7, 8, 9, 11, 12, 13, 14)]
        public void NonContiguousRangeHit(string expression, params int[] expectedRolls)
        {
            var rolls = Populate(new HashSet<int>(), () => Dice.Roll(expression), expectedRolls.Length);

            Assert.That(rolls, Is.EquivalentTo(expectedRolls));
            Assert.That(expectedRolls, Is.EquivalentTo(rolls));
        }
    }
}
