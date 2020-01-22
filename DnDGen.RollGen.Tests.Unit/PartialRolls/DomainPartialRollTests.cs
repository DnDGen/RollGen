using DnDGen.RollGen.Expressions;
using DnDGen.RollGen.PartialRolls;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Unit.PartialRolls
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

        private void BuildPartialRoll(double quantity)
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
        public void AddDiePartialRollToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll = partialRoll.d(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266d(42d600)"));
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
        public void AddKeepingPartialRolloRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll = partialRoll.Keeping(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266k(42d600)"));
        }

        [Test]
        public void AddExplodeToRollWithNumericQuantity()
        {
            BuildPartialRoll(9266);
            partialRoll = partialRoll.Explode();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266!"));
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
                .Explode()
                .Keeping(42)
                .Plus(600)
                .Minus(1337)
                .Times(1336)
                .DividedBy(96)
                .Modulos(783)
                .Plus("82d45")
                .Minus("12d34")
                .Times("23d45")
                .DividedBy("34d56")
                .Modulos("45d67");

            var expected = "9266";
            expected += "d2";
            expected += "d3";
            expected += "d4";
            expected += "d6";
            expected += "d8";
            expected += "d10";
            expected += "d12";
            expected += "d20";
            expected += "d100";
            expected += "d90210";
            expected += "k(7d6k5)";
            expected += "d(4d3k2)";
            expected += "!";
            expected += "k42";
            expected += "+600";
            expected += "-1337";
            expected += "*1336";
            expected += "/96";
            expected += "%783";
            expected += "+(82d45)";
            expected += "-(12d34)";
            expected += "*(23d45)";
            expected += "/(34d56)";
            expected += "%(45d67)";
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
        public void AddDiePartialRollToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll = partialRoll.d(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)d(42d600)"));
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
        public void AddKeepingPartialRollToRollQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll = partialRoll.Keeping(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)k(42d600)"));
        }

        [Test]
        public void AddExplodeToRollWithQuantityExpression()
        {
            BuildPartialRoll("7d6k5");
            partialRoll = partialRoll.Explode();
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(7d6k5)!"));
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
                .Explode()
                .Keeping(42)
                .Plus(600)
                .Minus(1337)
                .Times(1336)
                .DividedBy(96)
                .Modulos(783)
                .Plus("82d45")
                .Minus("12d34")
                .Times("23d45")
                .DividedBy("34d56")
                .Modulos("45d67");

            var expected = "(7d6k5)";
            expected += "d2";
            expected += "d3";
            expected += "d4";
            expected += "d6";
            expected += "d8";
            expected += "d10";
            expected += "d12";
            expected += "d20";
            expected += "d100";
            expected += "d90210";
            expected += "k(11d10k9)";
            expected += "d(4d3k2)";
            expected += "!";
            expected += "k42";
            expected += "+600";
            expected += "-1337";
            expected += "*1336";
            expected += "/96";
            expected += "%783";
            expected += "+(82d45)";
            expected += "-(12d34)";
            expected += "*(23d45)";
            expected += "/(34d56)";
            expected += "%(45d67)";
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo(expected));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_NoRoll_Int()
        {
            BuildPartialRoll(9266);
            var sum = partialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_NoRoll_Double()
        {
            BuildPartialRoll(92.66);
            var sum = partialRoll.AsSum<double>();
            Assert.That(sum, Is.EqualTo(92.66));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_1Roll_Int()
        {
            BuildPartialRoll(9266);
            var sum = partialRoll.d2().AsSum();
            Assert.That(sum, Is.EqualTo(9266 * 1.5));
        }

        [Test]
        public void ReturnAsSumFromNumericQuantity_2Rolls_Int()
        {
            BuildPartialRoll(42);
            var sum = partialRoll.d2().d3().AsSum();
            Assert.That(sum, Is.EqualTo(126));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_NoRoll_Int()
        {
            BuildPartialRoll("4d3k2");
            var sum = partialRoll.AsSum();
            Assert.That(sum, Is.EqualTo(5));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_NoRoll_Double()
        {
            BuildPartialRoll("43.2");
            var sum = partialRoll.AsSum<double>();
            Assert.That(sum, Is.EqualTo(43.2));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_1Roll_Int()
        {
            BuildPartialRoll("4d3k2");
            var sum = partialRoll.d2().AsSum();
            Assert.That(sum, Is.EqualTo(7));
        }

        [Test]
        public void ReturnAsSumFromQuantityExpression_2Rolls_Int()
        {
            BuildPartialRoll("4d3k2");
            var sum = partialRoll.d2().d3().AsSum();
            Assert.That(sum, Is.EqualTo(13));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_NoRoll_Int()
        {
            BuildPartialRoll(9266);
            var rolls = partialRoll.AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_NoRoll_Double()
        {
            BuildPartialRoll(92.66);
            var rolls = partialRoll.AsIndividualRolls<double>();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(92.66));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_1Roll_Int()
        {
            BuildPartialRoll(9266);
            var rolls = partialRoll.d2().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(9266));
            Assert.That(rolls.Count(r => r == 1), Is.EqualTo(9266 / 2));
            Assert.That(rolls.Count(r => r == 2), Is.EqualTo(9266 / 2));
        }

        [Test]
        public void ReturnAsIndividualRollsFromNumericQuantity_2Rolls_Int()
        {
            BuildPartialRoll(42);
            var rolls = partialRoll.d2().d3().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(126));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_NoRoll_Int()
        {
            BuildPartialRoll("4d3k2");
            var rolls = partialRoll.AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(5));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_NoRoll_Double()
        {
            BuildPartialRoll("43.2");
            var rolls = partialRoll.AsIndividualRolls<double>();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(43.2));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_1Roll_Int()
        {
            BuildPartialRoll("4d3k2");
            var rolls = partialRoll.d2().AsIndividualRolls();
            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls.Single(), Is.EqualTo(7));
        }

        [Test]
        public void ReturnAsIndividualRollsFromQuantityExpression_2Rolls_Int()
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
        public void ReturnAsMinimumFromNumericQuantity_NoRoll_Int()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMinimumFromNumericQuantity_NoRoll_Double()
        {
            BuildPartialRoll(92.66);
            var average = partialRoll.AsPotentialMinimum<double>();
            Assert.That(average, Is.EqualTo(92.66));
        }

        [Test]
        public void ReturnAsMinimumFromNumericQuantity_1Roll_Int()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMinimumFromNumericQuantity_2Rolls_Int()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).d(600).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_NoRoll_Int()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_NoRoll_Double()
        {
            BuildPartialRoll("43.2");
            var average = partialRoll.AsPotentialMinimum<double>();
            Assert.That(average, Is.EqualTo(43.2));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_1Roll_Int()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsMinimumFromQuantityExpression_2Rolls_Int()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).d(600).AsPotentialMinimum();
            Assert.That(average, Is.EqualTo(2));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_NoRoll_Int()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(9266));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_NoRoll_Double()
        {
            BuildPartialRoll(92.66);
            var average = partialRoll.AsPotentialMaximum<double>();
            Assert.That(average, Is.EqualTo(92.66));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_1Roll_Int()
        {
            BuildPartialRoll(9266);
            var average = partialRoll.d(42).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(9266 * 42));
        }

        [Test]
        public void ReturnAsMaximumFromNumericQuantity_2Rolls_Int()
        {
            BuildPartialRoll(96);
            var average = partialRoll.d(42).d(600).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(96 * 42 * 600));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_NoRoll_Int()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(6));
        }
        [Test]
        public void ReturnAsMaximumFromQuantityExpression_NoRoll_Double()
        {
            BuildPartialRoll("43.2");
            var average = partialRoll.AsPotentialMaximum<double>();
            Assert.That(average, Is.EqualTo(43.2));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_1Roll_Int()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(6 * 42));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_2Rolls_Int()
        {
            BuildPartialRoll("4d3k2");
            var average = partialRoll.d(42).d(600).AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(6 * 42 * 600));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_WithExplode_Int()
        {
            BuildPartialRoll("4d3!");
            var average = partialRoll.AsPotentialMaximum();
            Assert.That(average, Is.EqualTo(120));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_WithExplode_Double()
        {
            BuildPartialRoll("4d3!+2.1");
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("120+2.1")).Returns(122.1);

            var average = partialRoll.AsPotentialMaximum<double>();
            Assert.That(average, Is.EqualTo(120 + 2.1));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_WithoutExplode_Int()
        {
            BuildPartialRoll("4d3!");
            var average = partialRoll.AsPotentialMaximum(false);
            Assert.That(average, Is.EqualTo(12));
        }

        [Test]
        public void ReturnAsMaximumFromQuantityExpression_WithoutExplode_Double()
        {
            BuildPartialRoll("4d3!+2.1");
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("12+2.1")).Returns(14.1);

            var average = partialRoll.AsPotentialMaximum<double>(false);
            Assert.That(average, Is.EqualTo(12 + 2.1));
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh(int die, int roll)
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.d(die).AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_HighQuantity(int die, int roll)
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.d(die).AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly(int die)
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(die)).Returns(die / 2 - 1);

            var result = partialRoll.d(die).AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_HighQuantity(int die)
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(die)).Returns(die / 2 - 1);

            var result = partialRoll.d(die).AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow(int die, int roll)
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.d(die).AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_HighQuantity(int die, int roll)
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.d(die).AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.15);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_HighQuantity()
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.15);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.1);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_HighQuantity()
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.1);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.05);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_HighQuantity()
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(10)).Returns(0);

            var result = partialRoll.d10().AsTrueOrFalse(.05);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(100)).Returns(40);

            var result = partialRoll.Percentile().AsTrueOrFalse(42);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_HighQuantity()
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(100)).Returns(40);

            var result = partialRoll.Percentile().AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(100)).Returns(41);

            var result = partialRoll.Percentile().AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_HighQuantity()
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(100)).Returns(41);

            var result = partialRoll.Percentile().AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold()
        {
            BuildPartialRoll(1);
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var result = partialRoll.Percentile().AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_HighQuantity()
        {
            BuildPartialRoll(2);
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var result = partialRoll.Percentile().AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_WithPositiveBonus(int die, int roll)
        {
            BuildPartialRoll($"1d{die}+1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}+1")).Returns(die + 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}+1")).Returns(roll + 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_WithNegativeBonus(int die, int roll)
        {
            BuildPartialRoll($"1d{die}-1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}-1")).Returns(die - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}-1")).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_WithHighQuantity(int die, int roll)
        {
            BuildPartialRoll($"2d{die}");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_WithKeep(int die, int roll)
        {
            BuildPartialRoll($"2d{die}k1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_WithExplode(int die, int roll)
        {
            BuildPartialRoll($"1d{die}!");
            mockRandom
                .SetupSequence(r => r.Next(die))
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(roll - 1)
                .Returns(0);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 3)]
        [TestCase(4, 4)]
        [TestCase(6, 4)]
        [TestCase(6, 5)]
        [TestCase(6, 6)]
        [TestCase(8, 5)]
        [TestCase(8, 6)]
        [TestCase(8, 7)]
        [TestCase(8, 8)]
        [TestCase(10, 6)]
        [TestCase(10, 7)]
        [TestCase(10, 8)]
        [TestCase(10, 9)]
        [TestCase(10, 10)]
        [TestCase(12, 7)]
        [TestCase(12, 8)]
        [TestCase(12, 9)]
        [TestCase(12, 10)]
        [TestCase(12, 11)]
        [TestCase(12, 12)]
        [TestCase(20, 11)]
        [TestCase(20, 12)]
        [TestCase(20, 13)]
        [TestCase(20, 14)]
        [TestCase(20, 15)]
        [TestCase(20, 16)]
        [TestCase(20, 17)]
        [TestCase(20, 18)]
        [TestCase(20, 19)]
        [TestCase(20, 20)]
        [TestCase(100, 51)]
        [TestCase(100, 52)]
        [TestCase(100, 60)]
        [TestCase(100, 70)]
        [TestCase(100, 80)]
        [TestCase(100, 90)]
        [TestCase(100, 100)]
        public void ReturnAsTrueIfHigh_WithMultipleRolls(int die, int roll)
        {
            BuildPartialRoll($"1d{die}+1d42");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockRandom.Setup(r => r.Next(42)).Returns(21);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}+42")).Returns(die + 42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}+22")).Returns(roll + 22);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.True);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_WithPositiveBonus(int die)
        {
            var roll = die / 2;
            BuildPartialRoll($"1d{die}+1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}+1")).Returns(die + 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}+1")).Returns(roll + 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_WithNegativeBonus(int die)
        {
            var roll = die / 2;
            BuildPartialRoll($"1d{die}-1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}-1")).Returns(die - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}-1")).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_WithHighQuantity(int die)
        {
            BuildPartialRoll($"2d{die}");
            mockRandom.Setup(r => r.Next(die)).Returns(die / 2 - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_WithKeep(int die)
        {
            BuildPartialRoll($"2d{die}k1");
            mockRandom.Setup(r => r.Next(die)).Returns(die / 2 - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_WithExplode(int die)
        {
            BuildPartialRoll($"1d{die}!");
            mockRandom.Setup(r => r.Next(die)).Returns(die / 2 - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2)]
        [TestCase(4)]
        [TestCase(6)]
        [TestCase(8)]
        [TestCase(10)]
        [TestCase(12)]
        [TestCase(20)]
        [TestCase(100)]
        [TestCase(9266)]
        public void ReturnAsFalseIfOnThresholdExactly_WithMultipleRolls(int die)
        {
            var roll = die / 2;
            BuildPartialRoll($"1d{die} + 1d42");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockRandom.Setup(r => r.Next(42)).Returns(20);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1 + 1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die} + 42")).Returns(die + 42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll} + 21")).Returns(roll + 21);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_WithPositiveBonus(int die, int roll)
        {
            BuildPartialRoll($"1d{die}+1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}+1")).Returns(die + 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}+1")).Returns(roll + 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_WithNegativeBonus(int die, int roll)
        {
            BuildPartialRoll($"1d{die}-1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}-1")).Returns(die - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}-1")).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_WithHighQuantity(int die, int roll)
        {
            BuildPartialRoll($"2d{die}");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_WithKeep(int die, int roll)
        {
            BuildPartialRoll($"2d{die}k1");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_WithExplode(int die, int roll)
        {
            BuildPartialRoll($"1d{die}!");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [TestCase(2, 1)]
        [TestCase(3, 1)]
        [TestCase(4, 1)]
        [TestCase(6, 1)]
        [TestCase(6, 2)]
        [TestCase(8, 1)]
        [TestCase(8, 2)]
        [TestCase(8, 3)]
        [TestCase(10, 1)]
        [TestCase(10, 2)]
        [TestCase(10, 3)]
        [TestCase(10, 4)]
        [TestCase(12, 1)]
        [TestCase(12, 2)]
        [TestCase(12, 3)]
        [TestCase(12, 4)]
        [TestCase(12, 5)]
        [TestCase(20, 1)]
        [TestCase(20, 2)]
        [TestCase(20, 3)]
        [TestCase(20, 4)]
        [TestCase(20, 5)]
        [TestCase(20, 6)]
        [TestCase(20, 7)]
        [TestCase(20, 8)]
        [TestCase(20, 9)]
        [TestCase(100, 1)]
        [TestCase(100, 2)]
        [TestCase(100, 10)]
        [TestCase(100, 20)]
        [TestCase(100, 30)]
        [TestCase(100, 40)]
        [TestCase(100, 49)]
        public void ReturnAsFalseIfLow_WithMultipleRolls(int die, int roll)
        {
            BuildPartialRoll($"1d{die}+1d42");
            mockRandom.Setup(r => r.Next(die)).Returns(roll - 1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{die}+42")).Returns(die + 42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"{roll}+1")).Returns(roll + 1);

            var result = partialRoll.AsTrueOrFalse();
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_WithPositiveBonus()
        {
            BuildPartialRoll($"1d10+1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10+1")).Returns(11);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2+1")).Returns(3);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_WithNegativeBonus()
        {
            BuildPartialRoll($"1d10-1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10-1")).Returns(9);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2-1")).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_WithHighQuantity()
        {
            BuildPartialRoll($"2d10");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_WithKeep()
        {
            BuildPartialRoll($"2d10k1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_WithExplode()
        {
            BuildPartialRoll($"1d10!");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomPercentageThreshold_WithMultipleRolls()
        {
            BuildPartialRoll($"1d10+1d8");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockRandom.Setup(r => r.Next(8)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10+8")).Returns(18);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2+2")).Returns(4);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_WithPositiveBonus()
        {
            BuildPartialRoll($"1d10+1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10+1")).Returns(11);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2+1")).Returns(3);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse(.2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_WithNegativeBonus()
        {
            BuildPartialRoll($"1d10-1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10-1")).Returns(9);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2-1")).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse(.2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_WithHighQuantity()
        {
            BuildPartialRoll($"2d10");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_WithKeep()
        {
            BuildPartialRoll($"2d10k1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_WithExplode()
        {
            BuildPartialRoll($"1d10!");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfOnCustomPercentageThresholdExactly_WithMultipleRolls()
        {
            BuildPartialRoll($"1d10+1d6");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockRandom.Setup(r => r.Next(6)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10+6")).Returns(16);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2+2")).Returns(4);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_WithPositiveBonus()
        {
            BuildPartialRoll($"1d10+1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10+1")).Returns(11);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2+1")).Returns(3);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse(.15);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_WithNegativeBonus()
        {
            BuildPartialRoll($"1d10-1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10-1")).Returns(9);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2-1")).Returns(1);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse(.15);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_WithHighQuantity()
        {
            BuildPartialRoll($"2d10");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.15);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_WithKeep()
        {
            BuildPartialRoll($"2d10k1");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.15);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_WithExplode()
        {
            BuildPartialRoll($"1d10!");
            mockRandom.Setup(r => r.Next(10)).Returns(1);

            var result = partialRoll.AsTrueOrFalse(.15);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomPercentageThreshold_WithMultipleRolls()
        {
            BuildPartialRoll($"1d10+1d6");
            mockRandom.Setup(r => r.Next(10)).Returns(1);
            mockRandom.Setup(r => r.Next(6)).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"10+6")).Returns(16);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"2+3")).Returns(5);

            var result = partialRoll.AsTrueOrFalse(.25);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithPositiveBonus()
        {
            BuildPartialRoll($"1d100+1");
            mockRandom.Setup(r => r.Next(100)).Returns(40);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100+1")).Returns(101);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"41+1")).Returns(42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse(43);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithNegativeBonus()
        {
            BuildPartialRoll($"1d100-1");
            mockRandom.Setup(r => r.Next(100)).Returns(40);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100-1")).Returns(99);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"41-1")).Returns(40);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse(41);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithHighQuantity()
        {
            BuildPartialRoll($"2d100");
            mockRandom.Setup(r => r.Next(100)).Returns(40);

            var result = partialRoll.AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithKeep()
        {
            BuildPartialRoll($"2d100k1");
            mockRandom.Setup(r => r.Next(100)).Returns(40);

            var result = partialRoll.AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithExplode()
        {
            BuildPartialRoll($"1d100!");
            mockRandom.Setup(r => r.Next(100)).Returns(40);

            var result = partialRoll.AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithMultipleRolls()
        {
            BuildPartialRoll($"1d100+1d12");
            mockRandom.Setup(r => r.Next(100)).Returns(30);
            mockRandom.Setup(r => r.Next(12)).Returns(9);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"31+10")).Returns(41);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_WithPositiveBonus()
        {
            BuildPartialRoll($"1d100+1");
            mockRandom.Setup(r => r.Next(100)).Returns(41);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100+1")).Returns(101);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"42+1")).Returns(43);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse(43);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_WithNegativeBonus()
        {
            BuildPartialRoll($"1d100-1");
            mockRandom.Setup(r => r.Next(100)).Returns(41);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100-1")).Returns(99);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"42-1")).Returns(41);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse(41);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_WithHighQuantity()
        {
            BuildPartialRoll($"2d100");
            mockRandom.Setup(r => r.Next(100)).Returns(41);

            var result = partialRoll.AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_WithKeep()
        {
            BuildPartialRoll($"2d100k1");
            mockRandom.Setup(r => r.Next(100)).Returns(41);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_WithExplode()
        {
            BuildPartialRoll($"1d100!");
            mockRandom.Setup(r => r.Next(100)).Returns(41);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThresholdExactly_WithMultipleRolls()
        {
            BuildPartialRoll($"1d100+1d12");
            mockRandom.Setup(r => r.Next(100)).Returns(30);
            mockRandom.Setup(r => r.Next(12)).Returns(10);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"31+11")).Returns(42);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithPositiveBonus()
        {
            BuildPartialRoll($"1d100+1");
            mockRandom.Setup(r => r.Next(100)).Returns(42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100+1")).Returns(101);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"43+1")).Returns(44);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithNegativeBonus()
        {
            BuildPartialRoll($"1d100-1");
            mockRandom.Setup(r => r.Next(100)).Returns(42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100-1")).Returns(99);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"43-1")).Returns(42);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1-1")).Returns(0);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithHighQuantity()
        {
            BuildPartialRoll($"2d100");
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var result = partialRoll.AsTrueOrFalse(42 * 2);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithKeep()
        {
            BuildPartialRoll($"2d100k1");
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithExplode()
        {
            BuildPartialRoll($"1d100!");
            mockRandom.Setup(r => r.Next(100)).Returns(42);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithExplode_HighThreshold()
        {
            BuildPartialRoll($"1d100!");
            mockRandom
                .SetupSequence(r => r.Next(100))
                .Returns(99)
                .Returns(2);

            var result = partialRoll.AsTrueOrFalse(102);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsFalseIfLowerThanCustomRollThreshold_WithExplode_HighThreshold()
        {
            BuildPartialRoll($"1d100!");
            mockRandom
                .SetupSequence(r => r.Next(100))
                .Returns(99)
                .Returns(0);

            var result = partialRoll.AsTrueOrFalse(102);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfOnCustomRollThreshold_WithExplode_HighThreshold()
        {
            BuildPartialRoll($"1d100!");
            mockRandom
                .SetupSequence(r => r.Next(100))
                .Returns(99)
                .Returns(1);

            var result = partialRoll.AsTrueOrFalse(102);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfHigherThanCustomRollThreshold_WithMultipleRolls()
        {
            BuildPartialRoll($"1d100+1d12");
            mockRandom.Setup(r => r.Next(100)).Returns(31);
            mockRandom.Setup(r => r.Next(12)).Returns(10);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"1+1")).Returns(2);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"100+12")).Returns(112);
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>($"32+11")).Returns(43);

            var result = partialRoll.AsTrueOrFalse(42);
            Assert.That(result, Is.True);
        }

        [TestCase(.01)]
        [TestCase(.5)]
        [TestCase(.99)]
        public void ReturnAsTrueIfConstant_LowPercentage_Int(double percentage)
        {
            BuildPartialRoll(9266);
            var result = partialRoll.AsTrueOrFalse(percentage);
            Assert.That(result, Is.True);
        }

        [TestCase(.01)]
        [TestCase(.5)]
        [TestCase(.99)]
        public void ReturnAsTrueIfConstant_LowPercentage_Double(double percentage)
        {
            BuildPartialRoll(92.66);
            var result = partialRoll.AsTrueOrFalse(percentage);
            Assert.That(result, Is.True);
        }

        [TestCase(1)]
        [TestCase(1.01)]
        [TestCase(2)]
        public void ReturnAsFalseIfConstant_HighPercentage_Int(double percentage)
        {
            BuildPartialRoll(9266);
            var result = partialRoll.AsTrueOrFalse(percentage);
            Assert.That(result, Is.False);
        }

        [TestCase(1)]
        [TestCase(1.01)]
        [TestCase(2)]
        public void ReturnAsFalseIfConstant_HighPercentage_Double(double percentage)
        {
            BuildPartialRoll(92.66);
            var result = partialRoll.AsTrueOrFalse(percentage);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfConstant_LessThanRoll_Int()
        {
            BuildPartialRoll(9266);
            var result = partialRoll.AsTrueOrFalse(9267);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsFalseIfConstant_LessThanRoll_Double()
        {
            BuildPartialRoll(92.66);
            var result = partialRoll.AsTrueOrFalse(93);
            Assert.That(result, Is.False);
        }

        [Test]
        public void ReturnAsTrueIfConstant_EqualToRoll_Int()
        {
            BuildPartialRoll(9266);
            var result = partialRoll.AsTrueOrFalse(9266);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfConstant_GreaterThanRoll_Int()
        {
            BuildPartialRoll(9266);
            var result = partialRoll.AsTrueOrFalse(9265);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnAsTrueIfConstant_GreaterThanRoll_Double()
        {
            BuildPartialRoll(92.66);
            var result = partialRoll.AsTrueOrFalse(92);
            Assert.That(result, Is.True);
        }

        [Test]
        public void ReturnNumericKeepingWithNumericQuantity()
        {
            BuildPartialRoll(66);

            var keptRolls = partialRoll.d(600).Keeping(42).AsIndividualRolls();
            var expectedRolls = Enumerable.Range(66 - 41, 42);

            Assert.That(keptRolls.Count, Is.EqualTo(42));
            Assert.That(keptRolls, Is.EquivalentTo(expectedRolls));
        }

        [Test]
        public void ReturnNumericKeepingWithQuantityExpression()
        {
            BuildPartialRoll("quantity expression");
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("quantity expression")).Returns(66);

            var keptRolls = partialRoll.d(600).Keeping(42).AsIndividualRolls();
            var expectedRolls = Enumerable.Range(66 - 41, 42);

            Assert.That(keptRolls.Count(), Is.EqualTo(1));
            Assert.That(keptRolls.Single(), Is.EqualTo(expectedRolls.Sum()));
        }

        [Test]
        public void ReturnKeepingExpressionWithNumericQuantity()
        {
            BuildPartialRoll(9266);

            var keptRolls = partialRoll.d(42).Keeping("4d3").AsIndividualRolls();

            Assert.That(keptRolls.Count, Is.EqualTo(1));
            Assert.That(keptRolls.Single(), Is.EqualTo(42 * 7));
        }

        [Test]
        public void ReturnKeepingExpressionWithQuantityExpression()
        {
            BuildPartialRoll("quantity expression");
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("quantity expression")).Returns(9266);

            var keptRolls = partialRoll.d(42).Keeping("4d3").AsIndividualRolls();

            Assert.That(keptRolls.Count(), Is.EqualTo(1));
            Assert.That(keptRolls.Single(), Is.EqualTo(42 * 7));
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


            Assert.That(() => partialRoll.AsSum(),
                Throws.InstanceOf<InvalidOperationException>().With.Message.EqualTo("1d1! is not a valid roll.\n\tExplode: Cannot explode die 1, must be > 1"));
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

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void PlusAddsValueToValue(double value)
        {
            BuildPartialRoll(9266);

            partialRoll.Plus(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266+{value}"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void PlusAddsValueToExpression(double value)
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Plus(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)+{value}"));
        }

        [Test]
        public void PlusAddsExpressionToValue()
        {
            BuildPartialRoll(9266);

            partialRoll.Plus("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266+(42d600)"));
        }

        [Test]
        public void PlusAddsExpressionToExpression()
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Plus("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)+(42d600)"));
        }

        [Test]
        public void PlusAddsOtherPartialRollToValue()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(90210, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(42);

            partialRoll.Plus(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266+(90210d42)"));
        }

        [Test]
        public void PlusAddsOtherPartialRollToExpression()
        {
            BuildPartialRoll("9266d90210");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll.Plus(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)+(42d600)"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void MinusAddsValueToValue(double value)
        {
            BuildPartialRoll(9266);

            partialRoll.Minus(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266-{value}"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void MinusAddsValueToExpression(double value)
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Minus(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)-{value}"));
        }

        [Test]
        public void MinusAddsExpressionToValue()
        {
            BuildPartialRoll(9266);

            partialRoll.Minus("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266-(42d600)"));
        }

        [Test]
        public void MinusAddsExpressionToExpression()
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Minus("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)-(42d600)"));
        }

        [Test]
        public void MinusAddsOtherPartialRollToValue()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(90210, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(42);

            partialRoll.Minus(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266-(90210d42)"));
        }

        [Test]
        public void MinusAddsOtherPartialRollToExpression()
        {
            BuildPartialRoll("9266d90210");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll.Minus(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)-(42d600)"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void TimesAddsValueToValue(double value)
        {
            BuildPartialRoll(9266);

            partialRoll.Times(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266*{value}"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void TimesAddsValueToExpression(double value)
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Times(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)*{value}"));
        }

        [Test]
        public void TimesAddsExpressionToValue()
        {
            BuildPartialRoll(9266);

            partialRoll.Times("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266*(42d600)"));
        }

        [Test]
        public void TimesAddsExpressionToExpression()
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Times("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)*(42d600)"));
        }

        [Test]
        public void TimesAddsOtherPartialRollToValue()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(90210, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(42);

            partialRoll.Times(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266*(90210d42)"));
        }

        [Test]
        public void TimesAddsOtherPartialRollToExpression()
        {
            BuildPartialRoll("9266d90210");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll.Times(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)*(42d600)"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void DividedByAddsValueToValue(double value)
        {
            BuildPartialRoll(9266);

            partialRoll.DividedBy(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266/{value}"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void DividedByAddsValueToExpression(double value)
        {
            BuildPartialRoll("9266d90210");

            partialRoll.DividedBy(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)/{value}"));
        }

        [Test]
        public void DividedByAddsExpressionToValue()
        {
            BuildPartialRoll(9266);

            partialRoll.DividedBy("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266/(42d600)"));
        }

        [Test]
        public void DividedByAddsExpressionToExpression()
        {
            BuildPartialRoll("9266d90210");

            partialRoll.DividedBy("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)/(42d600)"));
        }

        [Test]
        public void DividedByAddsOtherPartialRollToValue()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(90210, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(42);

            partialRoll.DividedBy(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266/(90210d42)"));
        }

        [Test]
        public void DividedByAddsOtherPartialRollToExpression()
        {
            BuildPartialRoll("9266d90210");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll.DividedBy(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)/(42d600)"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void ModulosAddsValueToValue(double value)
        {
            BuildPartialRoll(9266);

            partialRoll.Modulos(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266%{value}"));
        }

        [TestCase(42)]
        [TestCase(13.37)]
        [TestCase(1336L)]
        public void ModulosAddsValueToExpression(double value)
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Modulos(value);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)%{value}"));
        }

        [Test]
        public void ModulosAddsExpressionToValue()
        {
            BuildPartialRoll(9266);

            partialRoll.Modulos("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266%(42d600)"));
        }

        [Test]
        public void ModulosAddsExpressionToExpression()
        {
            BuildPartialRoll("9266d90210");

            partialRoll.Modulos("42d600");
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)%(42d600)"));
        }

        [Test]
        public void ModulosAddsOtherPartialRollToValue()
        {
            BuildPartialRoll(9266);
            var otherPartialRoll = new DomainPartialRoll(90210, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(42);

            partialRoll.Modulos(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"9266%(90210d42)"));
        }

        [Test]
        public void ModulosAddsOtherPartialRollToExpression()
        {
            BuildPartialRoll("9266d90210");
            var otherPartialRoll = new DomainPartialRoll(42, mockRandom.Object, mockExpressionEvaluator.Object);
            otherPartialRoll.d(600);

            partialRoll.Modulos(otherPartialRoll);
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo($"(9266d90210)%(42d600)"));
        }

        [Test]
        public void AsSum_ThrowsException_WhenQuantityIsDecimal()
        {
            BuildPartialRoll(92.66);
            partialRoll.d2();

            Assert.That(() => partialRoll.AsSum<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsSum_ThrowsException_WhenDieIsDecimal()
        {
            BuildPartialRoll(92);
            partialRoll.d("6.6");

            Assert.That(() => partialRoll.AsSum<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsIndividualRolls_ThrowsException_WhenQuantityIsDecimal()
        {
            BuildPartialRoll(92.66);
            partialRoll.d2();

            Assert.That(() => partialRoll.AsIndividualRolls<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsIndividualRolls_ThrowsException_WhenDieIsDecimal()
        {
            BuildPartialRoll(92);
            partialRoll.d("6.6");

            Assert.That(() => partialRoll.AsIndividualRolls<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsMinimum_ThrowsException_WhenQuantityIsDecimal()
        {
            BuildPartialRoll(92.66);
            partialRoll.d2();

            Assert.That(() => partialRoll.AsPotentialMinimum<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsMinimum_ThrowsException_WhenDieIsDecimal()
        {
            BuildPartialRoll(92);
            partialRoll.d("6.6");

            Assert.That(() => partialRoll.AsPotentialMinimum<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsMaximum_ThrowsException_WhenQuantityIsDecimal()
        {
            BuildPartialRoll(92.66);
            partialRoll.d2();

            Assert.That(() => partialRoll.AsPotentialMaximum<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsMaximum_ThrowsException_WhenDieIsDecimal()
        {
            BuildPartialRoll(92);
            partialRoll.d("6.6");

            Assert.That(() => partialRoll.AsPotentialMaximum<double>(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsAverage_ThrowsException_WhenQuantityIsDecimal()
        {
            BuildPartialRoll(92.66);
            partialRoll.d2();

            Assert.That(() => partialRoll.AsPotentialAverage(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsAverage_ThrowsException_WhenDieIsDecimal()
        {
            BuildPartialRoll(92);
            partialRoll.d("6.6");

            Assert.That(() => partialRoll.AsPotentialAverage(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsTrueOrFalse_ThrowsException_WhenQuantityIsDecimal()
        {
            BuildPartialRoll(92.66);
            partialRoll.d2();

            Assert.That(() => partialRoll.AsTrueOrFalse(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }

        [Test]
        public void AsTrueOrFalse_ThrowsException_WhenDieIsDecimal()
        {
            BuildPartialRoll(92);
            partialRoll.d("6.6");
            mockExpressionEvaluator.Setup(e => e.Evaluate<double>("6.6")).Returns((string s) => 6.6);

            Assert.That(() => partialRoll.AsTrueOrFalse(),
                Throws.ArgumentException.With.Message.EqualTo("Cannot have decimal values for die rolls"));
        }
    }
}
