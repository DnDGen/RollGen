using Moq;
using NUnit.Framework;
using System;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class StringParseTests
    {
        private IDice dice;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            dice = new CoreDice(mockRandom.Object);
        }

        [Test]
        public void ParseNumber()
        {
            var roll = dice.Roll("3");
            Assert.That(roll, Is.EqualTo(3));
        }

        [Test]
        public void ParseNegativeNumber()
        {
            var roll = dice.Roll("-3");
            Assert.That(roll, Is.EqualTo(-3));
        }

        [Test]
        public void ParseSingleDieRolls()
        {
            dice.Roll("1d4");
            mockRandom.Verify(r => r.Next(4), Times.Once());
        }

        [Test]
        public void AbnormalDiceAllowed()
        {
            dice.Roll("1d7");
            mockRandom.Verify(r => r.Next(7), Times.Once());
        }

        [Test]
        public void ParseMultipleSameDieRolls()
        {
            dice.Roll("2d4");
            mockRandom.Verify(r => r.Next(4), Times.Exactly(2));
        }

        [Test]
        public void ParseInceptionStyleDieRolls()
        {
            mockRandom.Setup(r => r.Next(2)).Returns(1);

            dice.Roll("1d2d3");
            mockRandom.Verify(r => r.Next(2), Times.Once());
            mockRandom.Verify(r => r.Next(3), Times.Exactly(2));
        }

        [Test]
        public void ParseAddition()
        {
            var roll = dice.Roll("1+2");
            Assert.That(roll, Is.EqualTo(3));
        }

        [Test]
        public void ParseAdditionOfNegativeNumbers()
        {
            var roll = dice.Roll("1+-2");
            Assert.That(roll, Is.EqualTo(-1));
        }

        [Test]
        public void ParseDiceAndAddition()
        {
            var roll = dice.Roll("1d2+3");
            Assert.That(roll, Is.EqualTo(4));
        }

        [Test]
        public void EachDieRollIndependent()
        {
            mockRandom.SetupSequence(r => r.Next(2)).Returns(0).Returns(1);

            var roll = dice.Roll("1d2+1d2");
            mockRandom.Verify(r => r.Next(2), Times.Exactly(2));
            Assert.That(roll, Is.EqualTo(3));
        }

        [Test]
        public void ParseSubtraction()
        {
            var roll = dice.Roll("1-2-3");
            Assert.That(roll, Is.EqualTo(-4));
        }

        [Test]
        public void ParseAdditionAndSubtraction()
        {
            var roll = dice.Roll("1-2+3+-4");
            Assert.That(roll, Is.EqualTo(-2));
        }

        [Test]
        public void ParseDiceAndAdditionAndSubtraction()
        {
            mockRandom.Setup(r => r.Next(4)).Returns(1);

            var roll = dice.Roll("1-2+3d4");
            Assert.That(roll, Is.EqualTo(5));
        }

        [Test]
        public void ParseMultiplication()
        {
            var roll = dice.Roll("2*3");
            Assert.That(roll, Is.EqualTo(6));
        }

        [Test]
        public void ParseMultiplicationAndDice()
        {
            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var roll = dice.Roll("1d3*3");
            Assert.That(roll, Is.EqualTo(6));
        }

        [Test]
        public void ParseMultiplicationAndDiceAndAdditionSubtraction()
        {
            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var roll = dice.Roll("1-2d3*4+5");
            Assert.That(roll, Is.EqualTo(-10));
        }

        [Test]
        public void ParseParantheses()
        {
            var roll = dice.Roll("(1+2)");
            Assert.That(roll, Is.EqualTo(3));
        }

        [Test]
        public void ParanthesesCanContainAllOtherThings()
        {
            mockRandom.Setup(r => r.Next(4)).Returns(1);

            var roll = dice.Roll("(1+2*3d4-(5+6))7");
            Assert.That(roll, Is.EqualTo(14));
        }

        [Test]
        public void FollowingImplicitMultiplication()
        {
            var roll = dice.Roll("(2)3");
            Assert.That(roll, Is.EqualTo(6));
        }

        [Test]
        public void PrecedingImplicitMultiplication()
        {
            var roll = dice.Roll("3(2)");
            Assert.That(roll, Is.EqualTo(6));
        }

        [Test]
        public void ParseParanthesesAndDice()
        {
            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var roll = dice.Roll("(1+2)d3");
            Assert.That(roll, Is.EqualTo(6));
        }

        [Test]
        public void ParseParanthesesAndDiceAndMultiplication()
        {
            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var roll = dice.Roll("4(1+2)d3");
            Assert.That(roll, Is.EqualTo(24));
        }

        [Test]
        public void ParseParanthesesAndDiceAndMultiplicationAndAdditionSubtraction()
        {
            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var roll = dice.Roll("6+4(1+2)d3-5");
            Assert.That(roll, Is.EqualTo(25));
        }
    }
}