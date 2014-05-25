using Moq;
using NUnit.Framework;
using System;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class d3Tests
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
            dice.d3();
            mockRandom.Verify(r => r.Next(3), Times.Once());
        }

        [Test]
        public void Minimum()
        {
            var roll = dice.d3();
            Assert.That(roll, Is.EqualTo(MIN));
        }

        [Test]
        public void Quantity()
        {
            dice.d3(TWICE);
            mockRandom.Verify(r => r.Next(3), Times.Exactly(TWICE));
        }
    }
}