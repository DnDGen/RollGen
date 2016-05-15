using Moq;
using NUnit.Framework;
using RollGen.Domain.PartialRolls;
using System;

namespace RollGen.Tests.Unit
{
    [TestFixture]
    public class RandomPartialRollFactoryTests
    {
        private PartialRollFactory partialRollFactory;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            partialRollFactory = new RandomPartialRollFactory(mockRandom.Object);
        }

        [Test]
        public void ReturnRandomPartialRoll()
        {
            var partialRoll = partialRollFactory.Build(9266);
            Assert.That(partialRoll, Is.InstanceOf<RandomPartialRoll>());
        }

        [Test]
        public void UseSameInstanceOfRandomForAllPartialRolls()
        {
            mockRandom.Setup(r => r.Next(9266)).Returns(90210);

            var firstPartialRoll = partialRollFactory.Build(42);
            var secondPartialRoll = partialRollFactory.Build(42);

            var firstRoll = firstPartialRoll.d(9266);
            var secondRoll = secondPartialRoll.d(9266);

            Assert.That(firstRoll, Is.EqualTo(secondRoll));
            Assert.That(firstRoll, Is.EqualTo(3788862));
        }
    }
}
