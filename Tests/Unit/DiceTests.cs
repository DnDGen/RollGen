using System;
using Moq;
using NUnit.Framework;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class DiceTests
    {
        private IDice dice;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            dice = new Dice(mockRandom.Object);
        }

        [Test]
        public void ReturnPartialRoll()
        {
            Assert.That(dice.Roll(), Is.InstanceOf<IPartialRoll>());
        }

        [Test]
        public void PartialRollUsesRandom()
        {
            dice.Roll().d(9266);
            mockRandom.Verify(r => r.Next(9266), Times.Once);
        }

        [Test]
        public void PartialRollUsesQuantity()
        {
            dice.Roll(9266).d(42);
            mockRandom.Verify(r => r.Next(42), Times.Exactly(9266));
        }
    }
}