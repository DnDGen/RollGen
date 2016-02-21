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
            mockRandom.Setup(r => r.Next(66)).Returns(() => count++);

            var roll = dice.Roll("92d66");
            Assert.That(roll, Is.EqualTo(4278));
            mockRandom.Verify(r => r.Next(66), Times.Exactly(92));
        }

        [Test]
        public void RollFromStringSpaces()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(66)).Returns(() => count++);

            var roll = dice.Roll("  92    d  66   ");
            Assert.That(roll, Is.EqualTo(4278));
            mockRandom.Verify(r => r.Next(66), Times.Exactly(92));
        }

        [Test]
        public void RollFromStringNoQuantity()
        {
            mockRandom.Setup(r => r.Next(90210)).Returns(629);

            var roll = dice.Roll("1d90210");
            var rollWithoutQuantity = dice.Roll("d90210");

            Assert.That(rollWithoutQuantity, Is.EqualTo(roll));
            Assert.That(rollWithoutQuantity, Is.EqualTo(630));
            mockRandom.Verify(r => r.Next(90210), Times.Exactly(2));
        }

        [Test]
        public void RollFromStringWithBonus()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(66)).Returns(() => count++);

            var roll = dice.Roll("92d66+42");
            Assert.That(roll, Is.EqualTo(4320));
            mockRandom.Verify(r => r.Next(66), Times.Exactly(92));
        }

        [Test]
        public void RollFromStringWithMultiplier()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(66)).Returns(() => count++);

            var roll = dice.Roll("92d66*42");
            Assert.That(roll, Is.EqualTo(179676));
            mockRandom.Verify(r => r.Next(66), Times.Exactly(92));
        }

        [Test]
        public void RollMultipleRolls()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(66)).Returns(() => count++);
            mockRandom.Setup(r => r.Next(600)).Returns(() => count++);

            var roll = dice.Roll("92d66+42d600");
            Assert.That(roll, Is.EqualTo(9045));
            mockRandom.Verify(r => r.Next(66), Times.Exactly(92));
            mockRandom.Verify(r => r.Next(600), Times.Exactly(42));
        }

        [Test]
        public void RollMultipleOfSameRoll()
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(629)).Returns(() => count++);

            var roll = dice.RollString("7d629%7d629");
            Assert.That(roll, Is.EqualTo("(1 + 2 + 3 + 4 + 5 + 6 + 7)%(8 + 9 + 10 + 11 + 12 + 13 + 14)"));
            mockRandom.Verify(r => r.Next(629), Times.Exactly(14));
        }

        [Test]
        public void GetRawCompiledValue()
        {
            var roll = dice.CompileRaw("15/40");
            Assert.That(Convert.ToInt32(roll), Is.EqualTo(0));
            Assert.That(Convert.ToDouble(roll), Is.EqualTo(0.375));
        }

        [Test]
        public void GetDecimalCompiledValue()
        {
            var roll = dice.Roll<double>("15/40");
            Assert.That(roll, Is.EqualTo(0.375));
        }

        [Test]
        public void ReturnDefaultIfCastIsInvalid()
        {
            var roll = dice.Roll<DiceTests>("15/40");
            Assert.That(roll, Is.Null);
        }

        [TestCase("1d2", "(1)")]
        [TestCase("2d3", "(1 + 4)")]
        [TestCase("1+2d3", "1+(1 + 4)")]
        [TestCase("1d2+3", "(1)+3")]
        [TestCase("1d2+3d4", "(1)+(5 + 9 + 13)")]
        [TestCase("1+2d3-4d6*5", "1+(1 + 4)-(13 + 19 + 25 + 31)*5")]
        [TestCase("1+2d3-4d5*6", "1+(1 + 4)-(11 + 16 + 21 + 26)*6")]
        [TestCase("1+2d3-2d3/4", "1+(1 + 4)-(7 + 10)/4")]
        public void ReplaceRollsInString(string roll, string rolled)
        {
            var count = 0;
            mockRandom.Setup(r => r.Next(It.IsAny<int>())).Returns((int d) => count++ * d);

            var result = dice.RollString(roll);
            Assert.That(result, Is.EqualTo(rolled));
        }
    }
}