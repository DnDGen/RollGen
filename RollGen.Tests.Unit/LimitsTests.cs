using NUnit.Framework;

namespace RollGen.Tests.Unit
{
    [TestFixture]
    public class LimitsTests
    {
        [TestCase(Limits.Quantity, 46340)]
        [TestCase(Limits.Die, 46340)]
        public void Constant(int constant, int value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }
    }
}
