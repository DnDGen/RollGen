using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Unit
{
    [TestFixture]
    public class LimitsTests
    {
        [Test]
        public void QuantityLimit()
        {
            Assert.That(Limits.Quantity, Is.EqualTo(10_000));
        }

        [Test]
        public void DieLimit()
        {
            Assert.That(Limits.Die, Is.EqualTo(10_000));
        }

        [Test]
        public void ProductOfLimitsIsValid()
        {
            Assert.That(Limits.Quantity * Limits.Die, Is.LessThanOrEqualTo(int.MaxValue));
        }

        [Test]
        public void ProductOfExplodedLimitsIsValid()
        {
            Assert.That(Limits.Quantity * Limits.Die * 10, Is.LessThanOrEqualTo(int.MaxValue));
        }
    }
}
