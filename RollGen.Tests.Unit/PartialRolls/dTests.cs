using Moq;
using NUnit.Framework;
using RollGen.Domain;
using RollGen.Domain.PartialRolls;
using System;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class dTests
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
            mockRandom.Setup(r => r.Next(9266)).Returns(42);

            var roll = partialRoll.d(9266);
            Assert.That(roll, Is.EqualTo(43));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new RandomPartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(7)).Returns(4).Returns(2);

            var roll = partialRoll.d(7);
            Assert.That(roll, Is.EqualTo(8));
        }

        [Test]
        public void AfterRoll_AlwaysReturnZero()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(9266)).Returns(42);

            partialRoll.d(9266);
            var roll = partialRoll.d(9266);
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void AfterOtherRoll_AlwaysReturnZero()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(9266)).Returns(42);

            partialRoll.Percentile();
            var roll = partialRoll.d(9266);
            Assert.That(roll, Is.EqualTo(0));
        }

        [Test]
        public void IfDieGreaterThanLimit_ThrowArgumentException()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            Assert.That(() => partialRoll.d(Limits.Die + 1), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot roll a die larger than 46,340"));
        }

        [Test]
        public void IfDieEqualToLimit_Roll()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(Limits.Die)).Returns(9266);

            var roll = partialRoll.d(Limits.Die);
            Assert.That(roll, Is.EqualTo(9267));
        }
    }
}