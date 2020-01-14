using Ninject;
using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ExpressionTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [TestCase("1+2-(3*4/5)%6", 1, 1)]
        [TestCase("1d2+3", 4, 5)]
        [TestCase("1d2+3d4", 4, 14)]
        [TestCase("7d6k5", 5, 30)]
        [TestCase("7d8!", 7, 560)]
        public void RollExpression(string expression, int lower, int upper)
        {
            stressor.Stress(() => AssertExpression(expression, lower, upper));
        }

        private void AssertExpression(string expression, int lower, int upper)
        {
            var roll = Dice.Roll(expression).AsSum();
            Assert.That(roll, Is.InRange(lower, upper));
        }
    }
}
