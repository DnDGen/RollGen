using Moq;
using NUnit.Framework;
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
        public void IfProductOfQuantityAndDieGreaterThanLimit_ThrowArgumentException()
        {
            var rootOfLimit = Convert.ToInt32(Math.Floor(Math.Sqrt(Limits.ProductOfQuantityAndDie)));
            partialRoll = new RandomPartialRoll(rootOfLimit + 2, mockRandom.Object);
            Assert.That(() => partialRoll.d(rootOfLimit), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 46342d46340 is too large for RollGen"));

            partialRoll = new RandomPartialRoll(rootOfLimit, mockRandom.Object);
            Assert.That(() => partialRoll.d(rootOfLimit + 2), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 46340d46342 is too large for RollGen"));
        }

        [Test]
        public void IfQuantityOverLimit_ThrowArgumentException()
        {
            partialRoll = new RandomPartialRoll(Limits.Quantity + 1, mockRandom.Object);
            Assert.That(() => partialRoll.d(1), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 16500001d1 is too large for RollGen"));
        }
    }
}