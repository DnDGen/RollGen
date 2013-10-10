using System;
using D20Dice.Dice;
using D20Dice.RandomWrappers;
using Moq;
using NUnit.Framework;

namespace D20Dice.Test.Unit.Dice
{
    [TestFixture]
    public class d10Tests
    {
        private const Int32 MIN = 1;
        private const Int32 TWICE = 2;
        private const Int32 BONUS = 3;

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
            dice.d10();
            mockRandom.Verify(r => r.Next(10), Times.Once());
        }

        [Test]
        public void Minimum()
        {
            var roll = dice.d10();
            Assert.That(roll, Is.EqualTo(MIN));
        }

        [Test]
        public void Quantity()
        {
            dice.d10(TWICE);
            mockRandom.Verify(r => r.Next(10), Times.Exactly(TWICE));
        }

        [Test]
        public void Bonus()
        {
            var roll = dice.d10(bonus: BONUS);
            Assert.That(roll, Is.EqualTo(MIN + BONUS));
        }

        [Test]
        public void QuantityAndBonus()
        {
            var roll = dice.d10(TWICE, BONUS);
            mockRandom.Verify(r => r.Next(10), Times.Exactly(TWICE));
            Assert.That(roll, Is.EqualTo(MIN * TWICE + BONUS));
        }
    }
}