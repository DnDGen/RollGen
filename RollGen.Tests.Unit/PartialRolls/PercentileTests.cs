using Moq;
using NUnit.Framework;
using RollGen.Domain.PartialRolls;
using System;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class PercentileTests
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
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var roll = partialRoll.Percentile();
            Assert.That(roll, Is.EqualTo(43));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new RandomPartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(100)).Returns(4).Returns(2);

            var roll = partialRoll.Percentile();
            Assert.That(roll, Is.EqualTo(8));
        }

        [Test]
        public void IfQuantityOverLimit_ThrowArgumentException()
        {
            partialRoll = new RandomPartialRoll(Limits.Quantity + 1, mockRandom.Object);
            Assert.That(() => partialRoll.Percentile(), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 16500001d100 is too large for RollGen"));
        }
    }
}