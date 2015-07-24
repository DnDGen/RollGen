using Moq;
using NUnit.Framework;
using RollGen.Domain;
using System;

namespace RollGen.Test.Unit
{
    [TestFixture]
    public class PercentileTests
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
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var roll = partialRoll.Percentile();
            Assert.That(roll, Is.EqualTo(43));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new PartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(100)).Returns(4).Returns(2);

            var roll = partialRoll.Percentile();
            Assert.That(roll, Is.EqualTo(8));
        }

        [Test]
        public void AfterRoll_AlwaysReturnZero()
        {
            partialRoll = new PartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            partialRoll.Percentile();
            var roll = partialRoll.Percentile();
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void AfterOtherRoll_AlwaysReturnZero()
        {
            partialRoll = new PartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            partialRoll.d(21);
            var roll = partialRoll.Percentile();
            Assert.That(roll, Is.EqualTo(0));
        }
    }
}