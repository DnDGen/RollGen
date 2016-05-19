using NUnit.Framework;

namespace RollGen.Tests.Unit
{
    [TestFixture]
    public class LimitsTests
    {
        [Test]
        public void ProductLimit()
        {
            Assert.That(Limits.ProductOfQuantityAndDie, Is.EqualTo(int.MaxValue));
        }

        [Test]
        public void QuantityLimit()
        {
            Assert.That(Limits.Quantity, Is.EqualTo(16500000));
        }

        [Test]
        public void DieLimit()
        {
            Assert.That(Limits.Die, Is.EqualTo(int.MaxValue));
        }
    }
}
