using Moq;
using NUnit.Framework;
using RollGen.Domain.PartialRolls;
using System;
using System.Linq;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class IndividualRollsTests
    {
        private Mock<Random> mockRandom;
        private PartialRoll partialRoll;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
        }

        [Test]
        public void ReturnRollValues()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(9266)).Returns(42);

            var rolls = partialRoll.IndividualRolls(9266);
            Assert.That(rolls, Contains.Item(43));
            Assert.That(rolls.Count(), Is.EqualTo(1));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new RandomPartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(7)).Returns(4).Returns(2);

            var rolls = partialRoll.IndividualRolls(7);
            Assert.That(rolls, Contains.Item(5));
            Assert.That(rolls, Contains.Item(3));
            Assert.That(rolls.Count(), Is.EqualTo(2));
        }

        [Test]
        public void CanIterateOverRollsMultipleTimes()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(9266)).Returns(() => count++);
            partialRoll = new RandomPartialRoll(42, mockRandom.Object);

            var rolls = partialRoll.IndividualRolls(9266).OrderBy(x => x).ToList();
            Assert.That(rolls.Count, Is.EqualTo(42));

            for (var i = 0; i < 42; i++)
            {
                Assert.That(rolls[i], Is.EqualTo(i + 1));
            }
        }

        [Test]
        public void IfProductOfQuantityAndDieGreaterThanLimit_ThrowArgumentException()
        {
            var rootOfLimit = Convert.ToInt32(Math.Floor(Math.Sqrt(Limits.ProductOfQuantityAndDie)));
            partialRoll = new RandomPartialRoll(rootOfLimit + 2, mockRandom.Object);
            Assert.That(() => partialRoll.IndividualRolls(rootOfLimit), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 46342d46340 is too large for RollGen"));

            partialRoll = new RandomPartialRoll(rootOfLimit, mockRandom.Object);
            Assert.That(() => partialRoll.IndividualRolls(rootOfLimit + 2), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 46340d46342 is too large for RollGen"));
        }

        [Test]
        public void IfQuantityOverLimit_ThrowArgumentException()
        {
            partialRoll = new RandomPartialRoll(Limits.Quantity + 1, mockRandom.Object);
            Assert.That(() => partialRoll.IndividualRolls(1), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 16500001d1 is too large for RollGen"));
        }
    }
}
