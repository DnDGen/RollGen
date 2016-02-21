using Moq;
using NUnit.Framework;
using RollGen.Domain;
using System;
using System.Linq;

namespace RollGen.Test.Unit.PartialRolls
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

            var rolls = partialRoll.IndividualRolls(9266).ToList();
            Assert.That(rolls, Contains.Item(43));
            Assert.That(rolls.Count, Is.EqualTo(1));
        }

        [Test]
        public void RollQuantity()
        {
            partialRoll = new RandomPartialRoll(2, mockRandom.Object);
            mockRandom.SetupSequence(r => r.Next(7)).Returns(4).Returns(2);

            var rolls = partialRoll.IndividualRolls(7).ToList();
            Assert.That(rolls, Contains.Item(5));
            Assert.That(rolls, Contains.Item(3));
            Assert.That(rolls.Count, Is.EqualTo(2));
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
    }
}
