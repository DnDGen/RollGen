using Moq;
using NUnit.Framework;
using RollGen.Domain;
using System;

namespace RollGen.Test.Unit.PartialRolls
{
    [TestFixture]
    public class d2Tests
    {
        private Mock<Random> mockRandom;
        private PartialRoll partialRoll;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
        }

        [Test]
        public void ReturnRollValue()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(2)).Returns(42);

            var roll = partialRoll.d2();
            Assert.That(roll, Is.EqualTo(43));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new RandomPartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(2)).Returns(4).Returns(2);

            var roll = partialRoll.d2();
            Assert.That(roll, Is.EqualTo(8));
        }

        [Test]
        public void AfterRoll_AlwaysReturnZero()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(2)).Returns(42);

            partialRoll.d2();
            var roll = partialRoll.d2();
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void AfterOtherRoll_AlwaysReturnZero()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(2)).Returns(42);

            partialRoll.d(21);
            var roll = partialRoll.d2();
            Assert.That(roll, Is.EqualTo(0));
        }
    }
}