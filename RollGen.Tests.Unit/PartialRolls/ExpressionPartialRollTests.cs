using Moq;
using NUnit.Framework;
using RollGen.Expressions;
using RollGen.PartialRolls;
using System;
using System.Linq;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class ExpressionPartialRollTests
    {
        private PartialRoll expressionPartialRoll;
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

        private void BuildPartialRoll(string rollExpression)
        {
            expressionPartialRoll = new ExpressionPartialRoll(rollExpression, mockRandom.Object, mockExpressionEvaluator.Object);
        }

        [Test]
        public void ConstructPartialRollWithDieRoll()
        {
            BuildPartialRoll("4d3k2");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("4d3k2"));

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls, Contains.Item(3));
            Assert.That(rolls, Contains.Item(2));
            Assert.That(rolls.Count, Is.EqualTo(2));
        }

        [Test]
        public void ConstructPartialRollWithExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("5+5")).Returns(10);

            BuildPartialRoll("4d3k2+5");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("4d3k2+5"));

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(10));
        }

        [Test]
        public void ConstructPartialRollWithMultipleDieRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("5+17")).Returns(22);

            BuildPartialRoll("4d3k2+5d6");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("4d3k2+5d6"));

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(22));
        }

        [Test]
        public void ReturnAsSumFromExpressionOfJustRoll()
        {
            BuildPartialRoll("42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600"));

            var sum = expressionPartialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(903));
        }

        [Test]
        public void ReturnAsSumFromExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("903+1337")).Returns(2240);

            BuildPartialRoll("42d600+1337");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600+1337"));

            var sum = expressionPartialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(903 + 1337));
        }

        [Test]
        public void ReturnAsSumFromExpressionOfMultipleRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("903+156")).Returns(1059);

            BuildPartialRoll("42d600+13d37");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600+13d37"));

            var sum = expressionPartialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(1059));
        }

        [Test]
        public void ReturnAsIndividualRollsFromExpressionOfJustRoll()
        {
            BuildPartialRoll("42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600"));

            var rolls = expressionPartialRoll.AsIndividualRolls();

            for (var roll = 42; roll > 0; roll--)
                Assert.That(rolls, Contains.Item(roll));

            Assert.That(rolls.Count, Is.EqualTo(42));
        }

        [Test]
        public void ReturnAsIndividualRollsFromExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("42934011+42")).Returns(600);

            //INFO: Not sure how to parse these as individual rolls, so counting as 1 roll
            BuildPartialRoll("9266d90210+42");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42"));

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(600));
        }

        [Test]
        public void ReturnAsIndividualRollsFromExpressionWithMultipleRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("42934011+12075")).Returns(1337);

            BuildPartialRoll("9266d90210+42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42d600"));

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(1337));
        }

        [Test]
        public void ReturnAsAverageFromExpressionOfJustRoll()
        {
            BuildPartialRoll("42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600"));

            var average = expressionPartialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(12621));
        }

        [Test]
        public void ReturnAsAverageFromExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("417947563+42")).Returns(600);

            BuildPartialRoll("9266d90210+42");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42"));

            var average = expressionPartialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(600));
        }

        [Test]
        public void ReturnAsAverageFromExpressionWithMultipleRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("417947563+12621")).Returns(1337);

            BuildPartialRoll("9266d90210+42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42d600"));

            var average = expressionPartialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(1337));
        }

        [Test]
        public void ReturnAsMinimumFromExpressionOfJustRoll()
        {
            BuildPartialRoll("42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600"));

            var average = expressionPartialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(42));
        }

        [Test]
        public void ReturnAsMinimumFromExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("9266+42")).Returns(600);

            BuildPartialRoll("9266d90210+42");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42"));

            var average = expressionPartialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(600));
        }

        [Test]
        public void ReturnAsMinimumFromExpressionWithMultipleRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("9266+42")).Returns(1337);

            BuildPartialRoll("9266d90210+42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42d600"));

            var average = expressionPartialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(1337));
        }

        [Test]
        public void ReturnAsMaximumFromExpressionOfJustRoll()
        {
            BuildPartialRoll("42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("42d600"));

            var average = expressionPartialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(42 * 600));
        }

        [Test]
        public void ReturnAsMaximumFromExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("835885860+42")).Returns(600);

            BuildPartialRoll("9266d90210+42");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42"));

            var average = expressionPartialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(600));
        }

        [Test]
        public void ReturnAsMaximumFromExpressionWithMultipleRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("835885860+25200")).Returns(1337);

            BuildPartialRoll("9266d90210+42d600");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("9266d90210+42d600"));

            var average = expressionPartialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(1337));
        }

        [Test]
        public void ReturnAsAverageUnroundedFromExpressionOfJustRoll()
        {
            BuildPartialRoll("1d2");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d2"));

            var average = expressionPartialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(1.5));
        }

        [Test]
        public void ReturnAsAverageUnroundedFromExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("1.5+3")).Returns(4.5);

            BuildPartialRoll("1d2+3");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d2+3"));

            var average = expressionPartialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(4.5));
        }

        [Test]
        public void ReturnAsAverageUnroundedFromExpressionWithMultipleRolls()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("1.5+9")).Returns(10.5);

            BuildPartialRoll("1d2+3d5");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d2+3d5"));

            var average = expressionPartialRoll.AsPotentialAverage();
            Assert.That(average, Is.EqualTo(10.5));
        }

        [Test]
        public void ReturnAsFalseFromExpressionIfHigh()
        {
            mockRandom.Setup(r => r.Next(2)).Returns(1);

            BuildPartialRoll("1d2");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d2"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueFromExpressionIfExactlyOnAverage()
        {
            mockRandom.Setup(r => r.Next(4)).Returns(1);

            BuildPartialRoll("1d4");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d4"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueFromExpressionIfLow()
        {
            mockRandom.Setup(r => r.Next(2)).Returns(0);

            BuildPartialRoll("1d2");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d2"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseFromExpressionIfHigherThanThreshold()
        {
            mockRandom.Setup(r => r.Next(5)).Returns(1);

            BuildPartialRoll("1d5");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d5"));

            var result = expressionPartialRoll.AsTrueOrFalse(.35);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueFromExpressionIfLowerThanThreshold()
        {
            mockRandom.Setup(r => r.Next(5)).Returns(1);

            BuildPartialRoll("1d5");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d5"));

            var result = expressionPartialRoll.AsTrueOrFalse(.45);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueFromExpressionIfExactlyEqualToThreshold()
        {
            mockRandom.Setup(r => r.Next(5)).Returns(1);

            BuildPartialRoll("1d5");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d5"));

            var result = expressionPartialRoll.AsTrueOrFalse(.4);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueFromBooleanExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<bool>("this > that")).Returns(true);

            BuildPartialRoll("this > that");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("this > that"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseFromBooleanExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<bool>("this <= that")).Returns(false);

            BuildPartialRoll("this <= that");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("this <= that"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueFromBooleanExpressionWithDieRoll()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<bool>("1 > 5")).Returns(true);

            BuildPartialRoll("1d4 > 2d3");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d4 > 2d3"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseFromBooleanExpressionWithDieRoll()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<bool>("1 = 5")).Returns(false);

            BuildPartialRoll("1d4 = 2d3");
            Assert.That(expressionPartialRoll.CurrentRollExpression, Is.EqualTo("1d4 = 2d3"));

            var result = expressionPartialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void TrimExpressionOnConstruction()
        {
            BuildPartialRoll("  4  d   3  k   2    ");

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls, Contains.Item(3));
            Assert.That(rolls, Contains.Item(2));
            Assert.That(rolls.Count(), Is.EqualTo(2));
        }

        [Test]
        public void TrimExpressionWithExpressionOnConstruction()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("  5   + 5 ")).Returns(10);
            BuildPartialRoll("  4  d   3  k   2   + 5 ");

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(10));
        }

        [Test]
        public void TrimExpressionWithMultipleDieRollsOnConstruction()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("   5  +   17       ")).Returns(22);
            BuildPartialRoll("   4  d  3  k  2  +   5  d   6       ");

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(22));
        }

        [Test]
        public void ConstructPartialRollWithExpressionWithNoQuantity()
        {
            mockRandom.Setup(r => r.Next(3)).Returns(9266);
            BuildPartialRoll("d3");

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(9267));
        }

        [Test]
        public void ConstructPartialRollWithExpressionWithMultipleOfSameRoll()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("7+8")).Returns(15);
            BuildPartialRoll("4d3+4d3");

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(15));
        }

        [Test]
        public void ConstructPartialRollWithExpressionWithDieInception()
        {
            mockRandom.Setup(r => r.Next(2)).Returns(1);
            mockRandom.SetupSequence(r => r.Next(3)).Returns(2).Returns(1);

            BuildPartialRoll("1d2d3");

            var rolls = expressionPartialRoll.AsIndividualRolls();
            Assert.That(rolls.Single(), Is.EqualTo(5));
        }

        [Test]
        public void KeepingThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.Keeping(1), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void dThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d(1), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d2ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d2(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d3ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d3(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d4ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d4(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d6ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d6(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d8ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d8(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d10ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d10(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d12ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d12(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void d20ThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.d20(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }

        [Test]
        public void PercentileThrowsNotImplementedException()
        {
            BuildPartialRoll("3d2k1");
            Assert.That(() => expressionPartialRoll.Percentile(), Throws.InstanceOf<NotImplementedException>().With.Message.EqualTo("Cannot yet implement paranthetical expressions"));
        }
    }
}
