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
        public void AfterRoll_AlwaysReturnNothing()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(9266)).Returns(42);

            partialRoll.d(9266);

            var rolls = partialRoll.IndividualRolls(9266);
            Assert.That(rolls, Is.Empty);
        }

        [Test]
        public void AfterOtherRoll_AlwaysReturnNothing()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            mockRandom.Setup(r => r.Next(9266)).Returns(42);

            partialRoll.Percentile();

            var rolls = partialRoll.IndividualRolls(9266);
            Assert.That(rolls, Is.Empty);
        }

        [Test]
        public void CanIterateOverRollsMultipleTimes()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(42)).Returns(() => count++);
            partialRoll = new RandomPartialRoll(9266, mockRandom.Object);

            var rolls = partialRoll.IndividualRolls(42);
            Assert.That(rolls.Count(), Is.EqualTo(9266));
            Assert.That(rolls.First(), Is.EqualTo(1));
            Assert.That(rolls.Last(), Is.EqualTo(9266));
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
            Assert.That(() => partialRoll.IndividualRolls(1), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 1000001d1 is too large for RollGen"));
        }

        [Test]
        public void IfDieOverLimit_ThrowArgumentException()
        {
            partialRoll = new RandomPartialRoll(1, mockRandom.Object);
            Assert.That(() => partialRoll.IndividualRolls(Limits.Die + 1), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Die roll of 1d1000001 is too large for RollGen"));
        }

        [Test]
        public void IfAllInputsEqualToLimits_Roll()
        {
            mockRandom.Setup(r => r.Next(It.IsAny<int>())).Returns((int i) => i - 1);

            partialRoll = new RandomPartialRoll(Limits.Quantity, mockRandom.Object);

            var rolls = partialRoll.IndividualRolls(1);
            Assert.That(rolls.Count(), Is.EqualTo(Limits.Quantity));
            Assert.That(rolls, Contains.Item(1));
            Assert.That(rolls.Distinct().Count(), Is.EqualTo(1));

            partialRoll = new RandomPartialRoll(1, mockRandom.Object);

            rolls = partialRoll.IndividualRolls(Limits.Die);
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls, Contains.Item(Limits.Die));

            var rootOfLimit = Convert.ToInt32(Math.Floor(Math.Sqrt(Limits.ProductOfQuantityAndDie)));
            partialRoll = new RandomPartialRoll(rootOfLimit, mockRandom.Object);

            rolls = partialRoll.IndividualRolls(rootOfLimit);
            Assert.That(rolls.Count(), Is.EqualTo(rootOfLimit));
            Assert.That(rolls, Contains.Item(rootOfLimit));
            Assert.That(rolls.Distinct().Count(), Is.EqualTo(1));
        }
    }
}
