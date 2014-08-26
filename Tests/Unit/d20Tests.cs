using System;
using Moq;
using NUnit.Framework;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class d20Tests
    {
        private Mock<Random> mockRandom;
        private IPartialRoll partialRoll;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
        }

        [Test]
        public void ReturnRollValue()
        {
            partialRoll = new PartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(20)).Returns(42);

            var roll = partialRoll.d20();
            Assert.That(roll, Is.EqualTo(43));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new PartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(20)).Returns(4).Returns(2);

            var roll = partialRoll.d20();
            Assert.That(roll, Is.EqualTo(8));
        }

        [Test]
        public void AfterRoll_AlwaysReturnZero()
        {
            partialRoll = new PartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(20)).Returns(42);

            partialRoll.d20();
            var roll = partialRoll.d20();
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void AfterOtherRoll_AlwaysReturnZero()
        {
            partialRoll = new PartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(20)).Returns(42);

            partialRoll.d(21);
            var roll = partialRoll.d20();
            Assert.That(roll, Is.EqualTo(0));
        }
    }
}