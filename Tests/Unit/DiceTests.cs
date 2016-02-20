using Moq;
using NUnit.Framework;
using RollGen.Domain;
using System;

namespace RollGen.Test.Unit
{
    [TestFixture]
    public class DiceTests
    {
        private Dice dice;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            dice = new RandomDice(mockRandom.Object);
        }

        [Test]
        public void ReturnPartialRoll()
        {
            Assert.That(dice.Roll(), Is.InstanceOf<PartialRoll>());
        }

        [Test]
        public void PartialRollUsesRandom()
        {
            mockRandom.Setup(r => r.Next(9266)).Returns(90210);

            var roll = dice.Roll().d(9266);
            Assert.That(roll, Is.EqualTo(90211));
            mockRandom.Verify(r => r.Next(9266), Times.Once);
        }

        [Test]
        public void PartialRollUsesQuantity()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(42)).Returns(() => count++);

            var roll = dice.Roll(9266).d(42);
            Assert.That(roll, Is.EqualTo(42934011));
            mockRandom.Verify(r => r.Next(42), Times.Exactly(9266));
        }

        [TestCase("9266", 9266)]
        [TestCase("9266+90210", 9266 + 90210)]
        [TestCase("9266-90210", 9266 - 90210)]
        [TestCase("9266*90210", 9266 * 90210)]
        public void ComputeExpression(string expression, int result)
        {
            var roll = dice.Roll(expression);
            Assert.That(roll, Is.EqualTo(result));
        }

        [Test]
        public void RollFromString()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(90210)).Returns(() => count++);

            var roll = dice.Roll("9266d90210");
            Assert.That(roll, Is.EqualTo(42934011));
            mockRandom.Verify(r => r.Next(90210), Times.Exactly(9266));
        }

        [Test]
        public void RollFromStringWithBonus()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(90210)).Returns(() => count++);

            var roll = dice.Roll("9266d90210+42");
            Assert.That(roll, Is.EqualTo(42934053));
            mockRandom.Verify(r => r.Next(90210), Times.Exactly(9266));
        }

        [Test]
        public void RollFromStringWithMultiplier()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(90210)).Returns(() => count++);

            var roll = dice.Roll("9266d90210*42");
            Assert.That(roll, Is.EqualTo(1803228462));
            mockRandom.Verify(r => r.Next(90210), Times.Exactly(9266));
        }

        [Test]
        public void RollMultipleRolls()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(90210)).Returns(() => count++);
            mockRandom.Setup(r => r.Next(600)).Returns(() => count++);

            var roll = dice.Roll("9266d90210+42d600");
            Assert.That(roll, Is.EqualTo(43324086));
            mockRandom.Verify(r => r.Next(90210), Times.Exactly(9266));
            mockRandom.Verify(r => r.Next(600), Times.Exactly(42));
        }

        [Test]
        public void RollMultipleSameRollString()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(629)).Returns(() => count++);

            var roll = dice.RolledString("7d629%7d629").Split('%');
            Assert.That(!roll[0].Equals(roll[1]));
            mockRandom.Verify(r => r.Next(629), Times.Exactly(14));
        }
    }
}