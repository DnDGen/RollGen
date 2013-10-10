using System;
using D20Dice.Dice;
using D20Dice.RandomWrappers;
using Moq;
using NUnit.Framework;

namespace D20Dice.Test.Unit.Dice
{
    [TestFixture]
    public class d6Tests
    {
        private const Int32 MIN = 1;
        private const Int32 TWICE = 2;

        private IDice dice;
        private Mock<IRandomWrapper> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<IRandomWrapper>();
            dice = new CoreDice(mockRandom.Object);
        }

        [Test]
        public void Default()
        {
            dice.d6();
            mockRandom.Verify(r => r.Next(6), Times.Once());
        }

        [Test]
        public void Minimum()
        {
            var roll = dice.d6();
            Assert.That(roll, Is.EqualTo(MIN));
        }

        [Test]
        public void Quantity()
        {
            dice.d6(TWICE);
            mockRandom.Verify(r => r.Next(6), Times.Exactly(TWICE));
        }

        [Test]
        public void Bonus()
        {
            var roll = dice.d6(bonus: 1);
            Assert.That(roll, Is.EqualTo(MIN + 1));
        }

        [Test]
        public void QuantityAndBonus()
        {
            var roll = dice.d6(TWICE, 1);
            mockRandom.Verify(r => r.Next(6), Times.Exactly(TWICE));
            Assert.That(roll, Is.EqualTo(MIN * TWICE + 1));
        }
    }
}