using Ninject;
using NUnit.Framework;
using System;

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

        [TestCase(1, 1)]
        //[TestCase(10, 1)]
        //[TestCase(100, 1)]
        //[TestCase(1000, 1)]
        //[TestCase(10000, 1)]
        //[TestCase(100000, 1)]
        //[TestCase(1000000, 1)]
        //[TestCase(1, 10)]
        //[TestCase(1, 100)]
        //[TestCase(1, 1000)]
        //[TestCase(1, 10000)]
        //[TestCase(1, 100000)]
        //[TestCase(1, 1000000)]
        [TestCase(10, 10)]
        [TestCase(100, 100)]
        [TestCase(1000, 1000)]
        [TestCase(10000, 10000)]
        public void RollExpressionWithALargeDieRoll(int quantity, int die)
        {
            Stress(() => AssertExpressionWithALargeDieRoll(quantity, die));
        }

        private void AssertExpressionWithALargeDieRoll(int quantity, int die)
        {
            var rollExpression = string.Format("{0}d{1}", quantity, die);
            var roll = Dice.Roll(rollExpression);
            Assert.That(roll, Is.InRange(quantity, quantity * die));
        }

        [TestCase(100000, 100000)]
        public void CauseArithmeticOverflow(int quantity, int die)
        {
            var rollExpression = string.Format("{0}d{1}", quantity, die);
            Assert.That(() => Dice.Roll(rollExpression), Throws.InstanceOf<OverflowException>());
        }
    }
}
