using System;
using Moq;
using NUnit.Framework;

namespace D20Dice.Test.Unit
{
    [TestFixture]
    public class RollTests
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
        public void RollConverts()
        {
            var roll = dice.Roll("9266");
            Assert.That(roll, Is.EqualTo(9266));
        }

        [Test]
        public void RollParsesDieRolls()
        {
            mockRandom.Setup(r => r.Next(6)).Returns(9266);

            var roll = dice.Roll("1d6");
            Assert.That(roll, Is.EqualTo(9267));
        }

        [Test]
        public void RollCalculates()
        {
            var roll = dice.Roll("92+6*6");
            Assert.That(roll, Is.EqualTo(128));
        }

        [Test]
        public void RollParsesDieRollsAndCalculates()
        {
            mockRandom.Setup(r => r.Next(6)).Returns(9266);

            var roll = dice.Roll("1d6*1000");
            Assert.That(roll, Is.EqualTo(9267000));
        }
    }
}