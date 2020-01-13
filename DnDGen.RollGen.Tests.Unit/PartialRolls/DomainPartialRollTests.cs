using Moq;
using NUnit.Framework;
using RollGen.Expressions;
using RollGen.PartialRolls;
using System;
using System.Linq;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class DomainPartialRollTests
    {
        private PartialRoll partialRoll;
        private Mock<ExpressionEvaluator> mockExpressionEvaluator;
        private Mock<Random> mockRandom;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            mockExpressionEvaluator = new Mock<ExpressionEvaluator>();

            var count = 0;
            mockRandom.Setup(r => r.Next(It.IsAny<int>())).Returns((int max) => count++ % max);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>(It.IsAny<string>())).Returns((string s) => DefaultIntValue(s));
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>(It.IsAny<string>())).Returns((string s) => DefaultDoubleValue(s));
        }

        private int DefaultIntValue(string source)
        {
            if (int.TryParse(source, out var output))
                return output;

            throw new ArgumentException($"{source} was not configured to be evaluated");
        }

        private double DefaultDoubleValue(string source)
        {
            if (double.TryParse(source, out var output))
                return output;

            throw new ArgumentException($"{source} was not configured to be evaluated");
        }

        private void BuildPartialRoll(int quantity)
        {
            partialRoll = new DomainPartialRoll(quantity, mockRandom.Object, mockExpressionEvaluator.Object);
        }

        private void BuildPartialRoll(string quantity)
        {
            partialRoll = new DomainPartialRoll(quantity, mockRandom.Object, mockExpressionEvaluator.Object);
        }

        [Test]
        public void ConstructPartialRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266"));
        }

        [Test]
        public void ConstructPartialRollWithQuantityExpression()
        {
            BuildPartialRoll("9266d90210k42");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(9266d90210k42)"));
        }

        [Test]
        public void AddD2ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d2();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d2"));
        }

        [Test]
        public void AddD3ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d3();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d3"));
        }

        [Test]
        public void AddD4ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d4();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d4"));
        }

        [Test]
        public void AddD6ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d6();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d6"));
        }

        [Test]
        public void AddD8ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d8();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d8"));
        }

        [Test]
        public void AddD10ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d10();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d10"));
        }

        [Test]
        public void AddD12ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d12();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d12"));
        }

        [Test]
        public void AddD20ToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d20();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d20"));
        }

        [Test]
        public void AddPercentileToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.Percentile();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d100"));
        }

        [Test]
        public void AddNumericDieToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d(90210);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d90210"));
        }

        [Test]
        public void AddDieExpressionToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d("4d3k2");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d(4d3k2)"));
        }

        [Test]
        public void AddNumericKeepingToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.Keeping(90210);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266k90210"));
        }

        [Test]
        public void AddKeepingExpressionToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.Keeping("4d3k2");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266k(4d3k2)"));
        }

        [Test]
        public void ChainDiceToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll
                .d2()
                .d3()
                .d4()
                .d6()
                .d8()
                .d10()
                .d12()
                .d20()
                .Percentile()
                .d(90210)
                .Keeping("7d6k5")
                .d("4d3k2")
                .Keeping(42);

            var expected = "9266d2d3d4d6d8d10d12d20d100d90210k(7d6k5)d(4d3k2)k42";
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo(expected));
        }

        [Test]
        public void AddD2ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d2();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d2"));
        }

        [Test]
        public void AddD3ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d3();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d3"));
        }

        [Test]
        public void AddD4ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d4();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d4"));
        }

        [Test]
        public void AddD6ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d6();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d6"));
        }

        [Test]
        public void AddD8ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d8();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d8"));
        }

        [Test]
        public void AddD10ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d10();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d10"));
        }

        [Test]
        public void AddD12ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d12();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d12"));
        }

        [Test]
        public void AddD20ToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d20();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d20"));
        }

        [Test]
        public void AddPercentileToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.Percentile();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d100"));
        }

        [Test]
        public void AddNumericDieToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d(90210);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d90210"));
        }

        [Test]
        public void AddDieExpressionToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.d("4d3k2");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d(4d3k2)"));
        }

        [Test]
        public void AddNumericKeepingToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.Keeping(90210);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)k90210"));
        }

        [Test]
        public void AddKeepingExpressionToRollQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.Keeping("4d3k2");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)k(4d3k2)"));
        }

        [Test]
        public void ChainDiceToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll
                .d2()
                .d3()
                .d4()
                .d6()
                .d8()
                .d10()
                .d12()
                .d20()
                .Percentile()
                .d(90210)
                .Keeping("11d10k9")
                .d("4d3k2")
                .Keeping(42);

            var expected = "(7d6k5)d2d3d4d6d8d10d12d20d100d90210k(11d10k9)d(4d3k2)k42";
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_NoRoll()
        {
            BuildPartialRoll(9266);
            var sum = partialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_1Roll()
        {
            BuildPartialRoll(9266);
            var sum = partialRoll.d2().AsSum();
            Assert.That(sum, Is.EqualTo(9266 * 1.5));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_2Rolls()
        {
            BuildPartialRoll(9266);
            var sum = partialRoll.d2().d3().AsSum();
            Assert.That(sum, Is.EqualTo(9266 * 1.5 * 2));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_NoRoll()
        {
            BuildPartialRoll("4d3k2");
            var sum = partialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(5));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_1Roll()
        {
            BuildPartialRoll("4d3k2");
            var sum = partialRoll.d2().AsSum();
            Assert.That(sum, Is.EqualTo(7));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_2Rolls()
        {
            BuildPartialRoll("4d3k2");
            var sum = partialRoll.d2().d3().AsSum();
            Assert.That(sum, Is.EqualTo(13));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_NoRoll()
        {
            BuildPartialRoll(9266);
            var rolls = partialRoll.AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_1Roll()
        {
            BuildPartialRoll(9266);
            var rolls = partialRoll.d2().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(9266));
            Assert.That(rolls.Count(r => r == 1), Is.EqualTo(9266 / 2));
            Assert.That(rolls.Count(r => r == 2), Is.EqualTo(9266 / 2));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_2Rolls()
        {
            BuildPartialRoll(9266);
            var rolls = partialRoll.d2().d3().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(9266 * 1.5 * 2));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_NoRoll()
        {
            BuildPartialRoll("4d3k2");
            var rolls = partialRoll.AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(5));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_1Roll()
        {
            BuildPartialRoll("4d3k2");
            var rolls = partialRoll.d2().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(7));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_2Rolls()
        {
            BuildPartialRoll("4d3k2");
            var rolls = partialRoll.d2().d3().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(13));
        }

        [Test]
        public void ReturnAsAverageFromNumericQuantity_NoRoll()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(9266));
        }

        [TestCase(1, 1, 1)]
        [TestCase(1, 2, 1.5)]
        [TestCase(1, 3, 2)]
        [TestCase(1, 4, 2.5)]
        [TestCase(1, 6, 3.5)]
        [TestCase(1, 8, 4.5)]
        [TestCase(1, 10, 5.5)]
        [TestCase(1, 12, 6.5)]
        [TestCase(1, 20, 10.5)]
        [TestCase(1, 100, 50.5)]
        [TestCase(1, 9266, 4633.5)]
        [TestCase(2, 1, 2)]
        [TestCase(2, 2, 3)]
        [TestCase(2, 3, 4)]
        [TestCase(2, 4, 5)]
        [TestCase(2, 6, 7)]
        [TestCase(2, 8, 9)]
        [TestCase(2, 10, 11)]
        [TestCase(2, 12, 13)]
        [TestCase(2, 20, 21)]
        [TestCase(2, 100, 101)]
        [TestCase(2, 9266, 9267)]
        [TestCase(3, 1, 3)]
        [TestCase(3, 2, 4.5)]
        [TestCase(3, 3, 6)]
        [TestCase(3, 4, 7.5)]
        [TestCase(3, 6, 10.5)]
        [TestCase(3, 8, 13.5)]
        [TestCase(3, 10, 16.5)]
        [TestCase(3, 12, 19.5)]
        [TestCase(3, 20, 31.5)]
        [TestCase(3, 100, 151.5)]
        [TestCase(3, 9266, 13900.5)]
        [TestCase(42, 1, 42)]
        [TestCase(42, 2, 63)]
        [TestCase(42, 3, 84)]
        [TestCase(42, 4, 105)]
        [TestCase(42, 6, 147)]
        [TestCase(42, 8, 189)]
        [TestCase(42, 10, 231)]
        [TestCase(42, 12, 273)]
        [TestCase(42, 20, 441)]
        [TestCase(42, 100, 2121)]
        [TestCase(42, 9266, 194607)]
        public void ReturnAsAverageFromNumericQuantity_1Roll(int quantity, int die, double average)
        {
            BuildPartialRoll(quantity);
            var potentialAverage = partialRoll.d(die).AsPotentialAverage();
            Assert.That(potentialAverage, Is.EqualTo(average));
        }

        [Test]
        public void ReturnAsAverageFromNumericQuantity_2Rolls()
        {
            BuildPartialRoll(1);
            var average = partialRoll.d3().d2().AsPotentialAverage();
            Assert.That(average, Is.EqualTo(3));
        }

        [Test]
        public void ReturnAsAverageFromQuantityExpression_NoRoll()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(4));
        }

        [Test]
        public void ReturnAsAverageFromQuantityExpression_1Roll()
        {
            BuildPartialRoll("4d3k2");
            var potentialAverage = partialRoll.d2().AsPotentialAverage();
            Assert.That(potentialAverage, Is.EqualTo(6));
        }

        [Test]
        public void ReturnAsAverageFromQuantityExpression_2Rolls()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d2().d3().AsPotentialAverage();
            Assert.That(average, Is.EqualTo(12));
        }

        [Test]
        public void ReturnAsMinimumFromNumericQuantity_NoRoll()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMinimumFromNumericQuantity_1Roll()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMinimumFromNumericQuantity_2Rolls()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).d(600).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_NoRoll()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_1Roll()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_2Rolls()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).d(600).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_NoRoll()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_1Roll()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(9266 * 42));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_2Rolls()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).d(600).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(9266 * 42 * 600));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_NoRoll()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(6));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_1Roll()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(6 * 42));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_2Rolls()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).d(600).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(6 * 42 * 600));
        }

        [Test]
        public void ReturnAsTrueIfHigh()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(4)).Returns(2);

            var result = partialRoll.d4().AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnThresholdExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(4)).Returns(1);

            var result = partialRoll.d4().AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseIfLow()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(4)).Returns(0);

            var result = partialRoll.d4().AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.15);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomThresholdExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.1);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.05);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnNumericKeepingWithNumericQuantity()
        {
            BuildPartialRoll(9266);

            var keptRolls = partialRoll.d(90210).Keeping(42).AsIndividualRolls();

            Assert.That(keptRolls.Count, Is.EqualTo(42));
            for (var roll = 9266; roll > 9266 - 42; roll--)
                Assert.That(keptRolls, Contains.Item(roll));
        }

        [Test]
        public void ReturnNumericKeepingWithQuantityExpression()
        {
            BuildPartialRoll("quantity expression");
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("quantity expression")).Returns(9266);

            var keptRolls = partialRoll.d(90210).Keeping(42).AsIndividualRolls();
            var expectedRolls = Enumerable.Range(9266 - 41, 42);

            Assert.That(keptRolls.Count(), Is.EqualTo(1));
            Assert.That(keptRolls.Single(), Is.EqualTo(388311).And.EqualTo(expectedRolls.Sum()));
        }

        [Test]
        public void ReturnKeepingExpressionWithNumericQuantity()
        {
            BuildPartialRoll(9266);

            var keptRolls = partialRoll.d(90210).Keeping("4d3").AsIndividualRolls();

            Assert.That(keptRolls.Count, Is.EqualTo(1));
            Assert.That(keptRolls.Single(), Is.EqualTo(64869));
        }

        [Test]
        public void ReturnKeepingExpressionWithQuantityExpression()
        {
            BuildPartialRoll("quantity expression");
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("quantity expression")).Returns(9266);

            var keptRolls = partialRoll.d(90210).Keeping("4d3").AsIndividualRolls();

            Assert.That(keptRolls.Count(), Is.EqualTo(1));
            Assert.That(keptRolls.Single(), Is.EqualTo(64869));
        }

        [Test]
        public void KeepDuplicateHighestRolls()
        {
            BuildPartialRoll(4);
            mockRandom.SetupSequence(r => r.Next(6)).Returns(5).Returns(1).Returns(2).Returns(5);

            var keptRolls = partialRoll.d6().Keeping(3).AsIndividualRolls();
            Assert.That(keptRolls, Contains.Item(6)
                .And.Contains(3));
            Assert.That(keptRolls.Count(), Is.EqualTo(3));
            Assert.That(keptRolls.Count(r => r == 6), Is.EqualTo(2));
            Assert.That(keptRolls.Count(r => r == 3), Is.EqualTo(1));
        }

        [Test]
        public void KeepingUpdatesCurrentRoll()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.d2().Keeping(90210);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d2k90210"));
        }

        [Test]
        public void KeepThrowsException_IfInvalidRoll()
        {
            BuildPartialRoll(1);
            partialRoll.d(1).Explode();
            Assert.That(() => partialRoll.AsSum(), Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("1d1! is not a valid roll.  It might be too large for RollGen or involve values that are too low."));
        }

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
            partialRoll.d(die).Explode();

            return partialRoll.AsSum();
        }

        [TestCase("1", 1)]
        [TestCase("(1)", 1)]
        [TestCase("(2)3", 6)]
        [TestCase("(2)(3)", 6)]
        [TestCase("2(3)", 6)]
        [TestCase("(3)(2)", 6)]
        [TestCase("1d2", 1)]
        [TestCase("(1d2)", 1)]
        [TestCase("3(1d2)", 3)]
        [TestCase("(3)(1d2)", 3)]
        [TestCase("(1d2)3", 3)]
        [TestCase("(1d2)(3)", 3)]
        [TestCase("((1d2))", 1)]
        [TestCase("(1d2) + (3d4)", 10)]
        [TestCase("(1d2)d3", 2)]
        [TestCase("1d(2d3)", 3)]
        [TestCase("1d2 + 1", 2)]
        [TestCase("(1d2+1)", 2)]
        [TestCase("((1d2)+1)", 2)]
        [TestCase("((1d2+1))", 2)]
        [TestCase("(1d2+1) + (3d4+1)", 12)]
        [TestCase("(1d2+1)d3", 5)]
        [TestCase("1d(2d3+1)", 3)]
        public void ParantheticalExpression(string expression, int expectedSum)
        {
            BuildPartialRoll(expression);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("1 + 1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("1 + 9")).Returns(10);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("9+1")).Returns(10);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("3+1")).Returns(4);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("2 + 10")).Returns(12);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("1*3")).Returns(3);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("2*3")).Returns(6);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("3*2")).Returns(6);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("3*1")).Returns(3);

            var sum = partialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(expectedSum));
        }

        [Test]
        public void ParantheticalExpressionThrowsException_WhenParanthesesMismatched()
        {
            BuildPartialRoll("5*((1d2) + 1");
            Assert.That(() => partialRoll.AsSum(),
                Throws.InvalidOperationException.With.Message.EqualTo($"No closing paranthesis found for expression '(5*((1d2) + 1)'"));
        }
    }
}
