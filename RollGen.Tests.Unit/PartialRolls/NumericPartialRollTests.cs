using Moq;
using NUnit.Framework;
using RollGen.Domain.Expressions;
using RollGen.Domain.PartialRolls;
using System;
using System.Linq;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class NumericPartialRollTests
    {
        private PartialRoll numericPartialRoll;
        private Mock<ExpressionEvaluator> mockExpressionEvaluator;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            mockExpressionEvaluator = new Mock<ExpressionEvaluator>();

            var count = 0;
            mockRandom.Setup(r => r.Next(It.IsAny<int>())).Returns((int max) => count++ % max);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>(It.IsAny<string>())).Returns((string s) => DefaultValue(s));
        }

        private int DefaultValue(string source)
        {
            var output = 0;
            if (int.TryParse(source, out output))
                return output;

            throw new ArgumentException($"{source} was not configured to be evaluated");
        }

        private void BuildPartialRoll(int quantity)
        {
            numericPartialRoll = new NumericPartialRoll(quantity, mockRandom.Object, mockExpressionEvaluator.Object);
        }

        [Test]
        public void ConstructPartialRollWithQuantity()
        {
            BuildPartialRoll(9266);
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266"));

            var sum = numericPartialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsSumFromQuantity()
        {
            BuildPartialRoll(9266);
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266"));

            numericPartialRoll = numericPartialRoll.d2();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d2"));

            var sum = numericPartialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(9266 * 1.5));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantity()
        {
            BuildPartialRoll(9266);
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266"));

            numericPartialRoll = numericPartialRoll.d3();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d3"));

            var rolls = numericPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Count(r => r == 1), Is.EqualTo(3089));
            Assert.That(rolls.Count(r => r == 2), Is.EqualTo(3089));
            Assert.That(rolls.Count(r => r == 3), Is.EqualTo(3088));
            Assert.That(rolls.Count, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsAverageFromQuantity()
        {
            BuildPartialRoll(1);
            var average = numericPartialRoll.d3().AsPotentialAverage();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsAverageUnroundedFromQuantity()
        {
            BuildPartialRoll(1);
            var average = numericPartialRoll.d2().AsPotentialAverage();
            Assert.That(average, Is.EqualTo(1.5));
        }

        [Test]
        public void ReturnAsMinimumFromQuantity()
        {
            BuildPartialRoll(9266);
            var average = numericPartialRoll.d(90210).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMaximumFromQuantity()
        {
            BuildPartialRoll(9266);
            var average = numericPartialRoll.d(90210).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(9266 * 90210));
        }

        [Test]
        public void ReturnAsTrueIfHigh()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(2)).Returns(1);

            numericPartialRoll = numericPartialRoll.d2();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d2"));

            var result = numericPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnAverageExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(3)).Returns(1);

            numericPartialRoll = numericPartialRoll.d3();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d3"));

            var result = numericPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseIfLow()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(2)).Returns(0);

            numericPartialRoll = numericPartialRoll.d2();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d2"));

            var result = numericPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnDieWithQuantity()
        {
            BuildPartialRoll(9266);

            var result = numericPartialRoll.d(90210);
            var sum = result.AsSum();
            Assert.That(sum, Is.EqualTo(9266 * 9267 / 2));
        }

        [Test]
        public void ReturnKeepingWithQuantity()
        {
            BuildPartialRoll(9266);

            var result = numericPartialRoll.d(90210);
            var keptRolls = result.Keeping(42).AsIndividualRolls();

            for (var roll = 9266; roll > 9266 - 42; roll--)
                Assert.That(keptRolls, Contains.Item(roll));

            Assert.That(keptRolls.Count, Is.EqualTo(42));
        }

        [Test]
        public void KeepDuplicateHighestRolls()
        {
            BuildPartialRoll(4);
            mockRandom.SetupSequence(r => r.Next(6)).Returns(5).Returns(1).Returns(2).Returns(5);

            var result = numericPartialRoll.d6();
            var keptRolls = result.Keeping(3).AsIndividualRolls();

            Assert.That(keptRolls, Contains.Item(6));
            Assert.That(keptRolls, Contains.Item(3));
            Assert.That(keptRolls.Count, Is.EqualTo(3));
            Assert.That(keptRolls.Count(r => r == 6), Is.EqualTo(2));
        }

        [Test]
        public void KeepingUpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d2().Keeping(90210);
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d2k90210"));
        }

        [Test]
        public void dUpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d(90210);
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void d2UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d2();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d2"));
        }

        [Test]
        public void d3UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d3();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d3"));
        }

        [Test]
        public void d4UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d4();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d4"));
        }

        [Test]
        public void d6UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d6();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d6"));
        }

        [Test]
        public void d8UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d8();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d8"));
        }

        [Test]
        public void d10UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d10();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d10"));
        }

        [Test]
        public void d12UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d12();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d12"));
        }

        [Test]
        public void d20UpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.d20();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d20"));
        }

        [Test]
        public void PercentileUpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            numericPartialRoll = numericPartialRoll.Percentile();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("9266d100"));
        }
    }
}
