using Moq;
using NUnit.Framework;
using System;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class PercentileTests
    {
        private const Int32 MIN = 1;
        private const Int32 TWICE = 2;

        private IDice dice;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            dice = new CoreDice(mockRandom.Object);
        }

        [Test]
        public void Default()
        {
            dice.Percentile();
            mockRandom.Verify(r => r.Next(100), Times.Once());
        }

        [Test]
        public void Minimum()
        {
            var roll = dice.Percentile();
            Assert.That(roll, Is.EqualTo(MIN));
        }

        [Test]
        public void Quantity()
        {
            dice.Percentile(TWICE);
            mockRandom.Verify(r => r.Next(100), Times.Exactly(TWICE));
        }
    }
}