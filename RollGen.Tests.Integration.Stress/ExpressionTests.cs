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

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        public void RollExpression(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpression(expression, lower, upper));
        }

        private void AssertExpression(string expression, int lower, int upper)
        {
            var roll = Dice.Roll(expression).AsSum();
            Assert.That(roll, Is.InRange(lower, upper));
        }

        [TestCase(1, Limits.Die)]
        [TestCase(Limits.Quantity, 1)]
        public void RollWithLargestDieRollPossible(int quantity, int die)
        {
            var roll = $"{quantity}d{die}";
            stressor.Stress(() => AssertExpression(roll, quantity, die * quantity));
        }

        [Test]
        public void RollWithLargestDieRollPossible()
        {
            var rootOfLimit = Convert.ToInt32(Math.Floor(Math.Sqrt(Limits.ProductOfQuantityAndDie)));
            var roll = $"{rootOfLimit}d{rootOfLimit}";

            stressor.Stress(() => AssertExpression(roll, rootOfLimit, rootOfLimit * rootOfLimit));
        }
    }
}
