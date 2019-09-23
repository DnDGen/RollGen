using Moq;
using NUnit.Framework;
using RollGen.Expressions;
using RollGen.PartialRolls;
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
            if (int.TryParse(source, out var output))
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
        public void ReturnAsFalseIfHigh()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(4)).Returns(2);

            numericPartialRoll = numericPartialRoll.d4();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d4"));

            var result = numericPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfOnAverageExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(4)).Returns(1);

            numericPartialRoll = numericPartialRoll.d4();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d4"));

            var result = numericPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfLow()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(4)).Returns(0);

            numericPartialRoll = numericPartialRoll.d4();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d4"));

            var result = numericPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfLowerThanThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            numericPartialRoll = numericPartialRoll.d10();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d10"));

            var result = numericPartialRoll.AsTrueOrFalse(.15);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnThresholdExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            numericPartialRoll = numericPartialRoll.d10();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d10"));

            var result = numericPartialRoll.AsTrueOrFalse(.1);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseIfHigherThanThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            numericPartialRoll = numericPartialRoll.d10();
            Assert.That(numericPartialRoll.CurrentRollExpression, Is.EqualTo("1d10"));

            var result = numericPartialRoll.AsTrueOrFalse(.05);
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


        [Test]
        public void KeepException()
        {
            BuildPartialRoll(2);
            numericPartialRoll.Explode();
            Assert.That(() => numericPartialRoll.Keeping(1), Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void ExplodeException()
        {
            BuildPartialRoll(2);
            numericPartialRoll.Keeping(1);
            Assert.That(() => numericPartialRoll.Explode(), Throws.InstanceOf<InvalidOperationException>());
        }

        [TestCase(1, 1, new[] { 1, 666 }, ExpectedResult = 1)] // 1d1, shouldn't explode
        [TestCase(1, 6, new[] { 1, 666 }, ExpectedResult = 1)] // Single, no Explode
        [TestCase(1, 6, new[] { 6, 1, 666 }, ExpectedResult = 7)] // Single, Explode once
        [TestCase(1, 6, new[] { 6, 6, 1, 666 }, ExpectedResult = 13)] // Single, Explode twice
        [TestCase(3, 6, new[] { 3, 4, 2, 666 }, ExpectedResult = 9)] // Multiple, no Explode
        [TestCase(3, 6, new[] { 1, 6, 2, 2, 666 }, ExpectedResult = 11)] // Multiple, Explode once
        [TestCase(3, 6, new[] { 5, 6, 6, 1, 2, 666 }, ExpectedResult = 20)] // Multiple, Explode twice in a row
        [TestCase(3, 6, new[] { 6, 1, 6, 4, 2, 666 }, ExpectedResult = 19)] // Multiple, Explode twice not in a row
        public int ExplodeRoll(int quantity, int die, int[] rolls)
        {
            var seq = mockRandom.SetupSequence(r => r.Next(die));
            foreach (var roll in rolls)
            {
                seq.Returns(roll - 1);
            }

            BuildPartialRoll(quantity);
            numericPartialRoll.d(die).Explode();

            return numericPartialRoll.AsSum();
        }
    }
}
