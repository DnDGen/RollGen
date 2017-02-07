using Moq;
using NUnit.Framework;
using RollGen.Domain.PartialRolls;
using System;
using System.Linq;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class RollTests
    {
        private Roll roll;
        private string rollExpression = "92d66k42";
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            roll = new Roll(rollExpression);
            mockRandom = new Mock<Random>();

            var count = 0;
            mockRandom.Setup(r => r.Next(66)).Returns((int max) => count++ % max);
        }

        [Test]
        public void RollIsInitialized()
        {
            Assert.That(roll.AmountToKeep, Is.EqualTo(42));
            Assert.That(roll.Die, Is.EqualTo(66));
            Assert.That(roll.Quantity, Is.EqualTo(92));
            Assert.That(roll.IsValid, Is.True);
        }

        [TestCase("1d2", 1, 2, 0)]
        [TestCase(" 1 d 2 ", 1, 2, 0)]
        [TestCase("1d2k3", 1, 2, 3)]
        [TestCase(" 1 d 2 k 3 ", 1, 2, 3)]
        [TestCase("d2", 1, 2, 0)]
        [TestCase(" d 2 ", 1, 2, 0)]
        [TestCase("1230d456k789", 1230, 456, 789)]
        [TestCase(" 1230 d 456 k 789", 1230, 456, 789)]
        public void ParseExpression(string expression, int quantity, int die, int toKeep)
        {
            roll = new Roll(expression);
            Assert.That(roll.AmountToKeep, Is.EqualTo(toKeep));
            Assert.That(roll.Die, Is.EqualTo(die));
            Assert.That(roll.Quantity, Is.EqualTo(quantity));
        }

        [TestCase("", false)]
        [TestCase("1", false)]
        [TestCase("1d2", true)]
        [TestCase("1d", false)]
        [TestCase(" 1 d 2 ", true)]
        [TestCase("1d2k3", true)]
        [TestCase(" 1 d 2 k 3 ", true)]
        [TestCase("d2", true)]
        [TestCase(" d 2 ", true)]
        [TestCase("Not parsable", false)]
        [TestCase("1d2+3", false)]
        [TestCase("1+2-3", false)]
        [TestCase("1230d456k789", true)]
        [TestCase(" 1230 d 456 k 789", true)]
        [TestCase(" 12 30 d 4 56 k 7 8 9", false)]
        public void CanParse(string expression, bool canParse)
        {
            Assert.That(Roll.CanParse(expression), Is.EqualTo(canParse));
        }

        [TestCase(1, 1, 1, true)]
        [TestCase(2, 1, 1, true)]
        [TestCase(1, 2, 1, true)]
        [TestCase(0, 1, 1, false)]
        [TestCase(1, 0, 1, false)]
        [TestCase(-1, 1, 1, false)]
        [TestCase(1, -1, 1, false)]
        [TestCase(Limits.Quantity, 1, 1, true)]
        [TestCase(Limits.Quantity + 1, 1, 1, false)]
        [TestCase(1, Limits.Die, 1, true)]
        [TestCase(46340, 46340, 1, true)]
        [TestCase(46342, 46340, 1, false)]
        [TestCase(46340, 46342, 1, false)]
        [TestCase(46341, 46341, 1, false)]
        [TestCase(1, 1, 0, true)]
        [TestCase(1, 1, -1, false)]
        public void IsValid(int quantity, int die, int amountToKeep, bool isValid)
        {
            roll.Quantity = quantity;
            roll.Die = die;
            roll.AmountToKeep = amountToKeep;

            Assert.That(roll.IsValid, Is.EqualTo(isValid));
        }

        [Test]
        public void IfGettingRollsAndRollIsNotValid_ThrowArgumentException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetRolls(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342k42 is not a valid roll.  It might be too large for RollGen or involve values that are too low."));
        }

        [Test]
        public void IfGettingSumAndRollIsNotValid_ThrowArgumentException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetRolls(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342k42 is not a valid roll.  It might be too large for RollGen or involve values that are too low."));
        }

        [Test]
        public void IfGettingAverageAndRollIsNotValid_ThrowArgumentException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetRolls(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342k42 is not a valid roll.  It might be too large for RollGen or involve values that are too low."));
        }

        [Test]
        public void IfGettingTrueOrFalseAndRollIsNotValid_ThrowArgumentException()
        {
            roll.Quantity = 46341;
            roll.Die = 46342;

            Assert.That(() => roll.GetRolls(mockRandom.Object), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("46341d46342k42 is not a valid roll.  It might be too large for RollGen or involve values that are too low."));
        }

        [Test]
        public void GetRolls()
        {
            roll.AmountToKeep = 0;

            var rolls = roll.GetRolls(mockRandom.Object);

            for (var individualRoll = 66; individualRoll > 0; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 27 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
            }

            Assert.That(rolls.Count(), Is.EqualTo(92));
        }

        [Test]
        public void GetRollsAndKeep()
        {
            var rolls = roll.GetRolls(mockRandom.Object);

            for (var individualRoll = 66; individualRoll > 25; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 27 ? 2 : 1;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
            }

            Assert.That(rolls.Count(), Is.EqualTo(42));
        }

        [Test]
        public void GetRollsAndKeepDuplicates()
        {
            roll.Quantity *= 2;

            var rolls = roll.GetRolls(mockRandom.Object);

            for (var individualRoll = 66; individualRoll > 47; individualRoll--)
            {
                Assert.That(rolls, Contains.Item(individualRoll));

                var expectedCount = individualRoll < 53 && individualRoll > 48 ? 3 : 2;
                Assert.That(rolls.Count(r => r == individualRoll), Is.EqualTo(expectedCount), $"Roll of {individualRoll}");
            }

            Assert.That(rolls.Count(), Is.EqualTo(42));
        }

        [Test]
        public void GetSumOfRolls()
        {
            roll.AmountToKeep = 0;
            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(2562));
        }

        [Test]
        public void GetSumOfRollsAndKeep()
        {
            var sum = roll.GetSum(mockRandom.Object);
            Assert.That(sum, Is.EqualTo(1912));
        }

        [Test]
        public void GetPotentialAverageRoll()
        {
            roll.AmountToKeep = 0;
            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(3082));
        }

        [Test]
        public void GetPotentialAverageRollAndKeep()
        {
            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(1407));
        }

        [Test]
        public void GetPotentialMinimumRoll()
        {
            roll.AmountToKeep = 0;
            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(92));
        }

        [Test]
        public void GetPotentialMinimumRollAndKeep()
        {
            var minimum = roll.GetPotentialMinimum();
            Assert.That(minimum, Is.EqualTo(42));
        }

        [Test]
        public void GetPotentialMaximumRoll()
        {
            roll.AmountToKeep = 0;
            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(92 * 66));
        }

        [Test]
        public void GetPotentialMaximumRollAndKeep()
        {
            var maximum = roll.GetPotentialMaximum();
            Assert.That(maximum, Is.EqualTo(42 * 66));
        }

        [Test]
        public void GetUnroundedPotentialAverageRoll()
        {
            roll.Quantity = 3;
            roll.Die = 2;
            roll.AmountToKeep = 0;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(4.5));
        }

        [Test]
        public void GetUnroundedPotentialAverageRollAndKeep()
        {
            roll.Quantity = 3;
            roll.Die = 2;
            roll.AmountToKeep = 1;

            var average = roll.GetPotentialAverage();
            Assert.That(average, Is.EqualTo(1.5));
        }

        [Test]
        public void GetTrueIfHigh()
        {
            roll.AmountToKeep = 0;
            mockRandom.Setup(r => r.Next(66)).Returns(34);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetTrueIfHighAndKeep()
        {
            mockRandom.Setup(r => r.Next(66)).Returns(34);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetFalseIfLow()
        {
            roll.AmountToKeep = 0;
            mockRandom.Setup(r => r.Next(66)).Returns(32);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetFalseIfLowAndKeep()
        {
            mockRandom.Setup(r => r.Next(66)).Returns(32);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetTrueIfOnAverageExactly()
        {
            roll.Quantity = 3;
            roll.Die = 3;
            roll.AmountToKeep = 0;

            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void GetTrueIfOnAverageExactlyAndKeep()
        {
            roll.Quantity = 3;
            roll.Die = 3;
            roll.AmountToKeep = 1;

            mockRandom.Setup(r => r.Next(3)).Returns(1);

            var result = roll.GetTrueOrFalse(mockRandom.Object);
            Assert.That(result, Is.True);
        }

        [Test]
        public void RollString()
        {
            Assert.That(roll.ToString(), Is.EqualTo("92d66k42"));
        }

        [Test]
        public void RollStringWithoutKeep()
        {
            roll.AmountToKeep = 0;
            Assert.That(roll.ToString(), Is.EqualTo("92d66"));
        }
    }
}
