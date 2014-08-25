using System;
using Moq;
using NUnit.Framework;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class RollIndexTests
    {
        private IDice dice;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            dice = new Dice(mockRandom.Object);
        }

        [Test]
        public void CallRandom()
        {
            dice.RollIndex(9266);
            mockRandom.Verify(r => r.Next(9266), Times.Once());
        }

        [Test]
        public void Minimum()
        {
            var roll = dice.RollIndex(9266);
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void Maximum()
        {
            mockRandom.Setup(r => r.Next(9266)).Returns(9265);

            var roll = dice.RollIndex(9266);
            Assert.That(roll, Is.EqualTo(9265));
        }
    }
}