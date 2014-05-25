using Moq;
using NUnit.Framework;
using System;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class d10Tests
    {
        private const Int32 MIN = 1;
        private const Int32 TWICE = 2;

        private IDice dice;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            dice = new Dice(mockRandom.Object);
        }

        [Test]
        public void Default()
        {
            dice.d10();
            mockRandom.Verify(r => r.Next(10), Times.Once());
        }

        [Test]
        public void Minimum()
        {
            var roll = dice.d10();
            Assert.That(roll, Is.EqualTo(MIN));
        }

        [Test]
        public void Quantity()
        {
            dice.d10(TWICE);
            mockRandom.Verify(r => r.Next(10), Times.Exactly(TWICE));
        }
    }
}