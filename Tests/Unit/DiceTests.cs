using Moq;
using NUnit.Framework;
using RollGen.Domain;
using System;
using System.Collections.Generic;

namespace RollGen.Test.Unit
{
    [TestFixture]
    public class DiceTests
    {
        private Dice dice;
        private Mock<ExpressionEvaluator> mockExpressionEvaluator;
        private Mock<PartialRollFactory> mockPartialRollFactory;
        private Mock<PartialRoll> mockPartialRoll;

        [SetUp]
        public void Setup()
        {
            mockExpressionEvaluator = new Mock<ExpressionEvaluator>();
            mockPartialRollFactory = new Mock<PartialRollFactory>();
            dice = new DomainDice(mockExpressionEvaluator.Object, mockPartialRollFactory.Object);

            mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<int>())).Returns(mockPartialRoll.Object);
        }

        [Test]
        public void ReturnPartialRoll()
        {
            var mockDefaultPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(1)).Returns(mockDefaultPartialRoll.Object);

            var partialRoll = dice.Roll();
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockDefaultPartialRoll.Object));
        }

        [Test]
        public void ReturnPartialRollWithQuantity()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(42)).Returns(mockPartialRollWithQuantity.Object);

            var partialRoll = dice.Roll(42);
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockPartialRollWithQuantity.Object));
        }

        [Test]
        public void EvaluateExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate("expression")).Returns(9266);

            var roll = dice.Roll("expression");
            Assert.That(roll, Is.EqualTo(9266));
        }

        [Test]
        public void RollExpression()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });
            mockExpressionEvaluator.Setup(e => e.Evaluate("(90210)")).Returns(90210);

            var roll = dice.Roll("92d66");
            Assert.That(roll, Is.EqualTo(90210));
        }

        [Test]
        public void TrimRolledExpression()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });

            var expression = dice.RollExpression("  92    d  66   ");
            Assert.That(expression, Is.EqualTo("(90210)"));
        }

        [Test]
        public void HaveSummedSpacesInExpression()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210, 42 });

            var expression = dice.RollExpression("  92    d  66   ");
            Assert.That(expression, Is.EqualTo("(90210 + 42)"));
        }

        [Test]
        public void RollExpressionWithSpaces()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });
            mockExpressionEvaluator.Setup(e => e.Evaluate("(90210)")).Returns(600);

            var roll = dice.Roll("  92    d  66   ");
            Assert.That(roll, Is.EqualTo(600));
        }

        [Test]
        public void RollExpressionWithNoQuantity()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(1)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(90210)).Returns(new[] { 9266 });
            mockExpressionEvaluator.Setup(e => e.Evaluate("(9266)")).Returns(9266);

            var rollWithoutQuantity = dice.Roll("d90210");
            var roll = dice.Roll("1d90210");

            Assert.That(rollWithoutQuantity, Is.EqualTo(roll));
            Assert.That(rollWithoutQuantity, Is.EqualTo(9266));
        }

        [Test]
        public void RollExpressionWithBonus()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });
            mockExpressionEvaluator.Setup(e => e.Evaluate("(90210)+42")).Returns(600);

            var roll = dice.Roll("92d66+42");
            Assert.That(roll, Is.EqualTo(600));
        }

        [Test]
        public void RollExpressionWithMultiplier()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });
            mockExpressionEvaluator.Setup(e => e.Evaluate("(90210)*42")).Returns(600);

            var roll = dice.Roll("92d66*42");
            Assert.That(roll, Is.EqualTo(600));
        }

        [Test]
        public void RollExpressionWithMultipleRolls()
        {
            var mockFirstPartialRoll = new Mock<PartialRoll>();
            var mockSecondPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockFirstPartialRoll.Object);
            mockPartialRollFactory.Setup(f => f.Build(42)).Returns(mockSecondPartialRoll.Object);

            mockFirstPartialRoll.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });
            mockSecondPartialRoll.Setup(r => r.IndividualRolls(600)).Returns(new[] { 1337 });

            mockExpressionEvaluator.Setup(e => e.Evaluate("(90210)+(1337)")).Returns(1234);

            var roll = dice.Roll("92d66+42d600");
            Assert.That(roll, Is.EqualTo(1234));
        }

        [Test]
        public void RollExpressionWithMultipleOfSameRoll()
        {
            var mockFirstPartialRoll = new Mock<PartialRoll>();
            var mockSecondPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.SetupSequence(f => f.Build(7)).Returns(mockFirstPartialRoll.Object).Returns(mockSecondPartialRoll.Object);

            mockFirstPartialRoll.Setup(r => r.IndividualRolls(629)).Returns(new[] { 9266, 90210 });
            mockSecondPartialRoll.Setup(r => r.IndividualRolls(629)).Returns(new[] { 42, 600 });

            var roll = dice.RollExpression("7d629%7d629");
            Assert.That(roll, Is.EqualTo("(9266 + 90210)%(42 + 600)"));
        }

        [Test]
        public void GetRawEvaluatedValue()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate("expression")).Returns(.375);

            var roll = dice.Evaluate("expression");
            Assert.That(roll, Is.InstanceOf<object>());
            Assert.That(Convert.ToInt32(roll), Is.EqualTo(0));
            Assert.That(Convert.ToDouble(roll), Is.EqualTo(0.375));
        }

        [Test]
        public void GetDecimalEvaluatedValue()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate("expression")).Returns(.375);

            var roll = dice.Evaluate<double>("expression");
            Assert.That(roll, Is.EqualTo(0.375));
        }

        [Test]
        public void ThrowExceptionIfCastIsInvalid()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate("expression")).Returns(.375);

            Assert.That(() => dice.Evaluate<DiceTests>("expression"), Throws.InstanceOf<InvalidCastException>());
        }

        [TestCase("1d2", "(1 + 0)")]
        [TestCase("2d3", "(2 + 2 + 0)")]
        [TestCase("1+2d3", "1+(2 + 2 + 0)")]
        [TestCase("1d2+3", "(1 + 0)+3")]
        [TestCase("1d2+3d4", "(1 + 0)+(3 + 4 + 3 + 0)")]
        [TestCase("1+2d3-4d6*5", "1+(2 + 2 + 0)-(5 + 8 + 9 + 8 + 5 + 0)*5")]
        [TestCase("1+2d3-4d5*6", "1+(2 + 2 + 0)-(4 + 6 + 6 + 4 + 0)*6")]
        [TestCase("1+2d3-2d3/4", "1+(2 + 2 + 0)-(2 + 2 + 0)/4")]
        public void ReplaceRollsInExpression(string roll, string rolled)
        {
            mockPartialRoll.Setup(r => r.IndividualRolls(It.IsAny<int>())).Returns((int d) => BuildIndividualRolls(d));

            var result = dice.RollExpression(roll);
            Assert.That(result, Is.EqualTo(rolled));
        }

        [TestCase("1d2", true)]
        [TestCase("2d3", true)]
        [TestCase("1+2d3", true)]
        [TestCase("1d2+3", true)]
        [TestCase("1d2+3d4", true)]
        [TestCase("1+2d3-4d6*5", true)]
        [TestCase("1+2d3-4d5*6", true)]
        [TestCase("1+2d3-2d3/4", true)]
        [TestCase("d2", true)]
        [TestCase("I want to roll a d2.", true)]
        [TestCase("1+2", false)]
        [TestCase("  1  d     2    ", true)]
        [TestCase("one d two", false)]
        [TestCase("other things", false)]
        public void ExpressionContainsRoll(string expression, bool containsRoll)
        {
            Assert.That(dice.ContainsRoll(expression), Is.EqualTo(containsRoll));
        }

        private IEnumerable<int> BuildIndividualRolls(int die)
        {
            var count = 1;
            var rolls = new List<int>();

            while (die-- > 0)
                rolls.Add(count++ * die);

            return rolls;
        }

        [Test]
        public void ThrowExceptionIfYouTryToEvaluateAnExpressionWithUnrolledDieRolls()
        {
            Assert.That(() => dice.Evaluate("1+2d3-45d67"), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot evaluate unrolled die roll 2d3"));
        }
    }
}