using Moq;
using NUnit.Framework;
using RollGen.Domain;
using RollGen.Domain.Expressions;
using RollGen.Domain.PartialRolls;
using System;
using System.Collections.Generic;

namespace RollGen.Tests.Unit
{
    [TestFixture]
    public class DiceTests
    {
        private Dice dice;
        private Mock<ExpressionEvaluator> mockExpressionEvaluator;
        private Mock<PartialRollFactory> mockPartialRollFactory;
        private Mock<PartialRoll> mockPartialRoll;
        private Queue<int> quantities;

        [SetUp]
        public void Setup()
        {
            mockExpressionEvaluator = new Mock<ExpressionEvaluator>();
            mockPartialRollFactory = new Mock<PartialRollFactory>();
            dice = new DomainDice(mockExpressionEvaluator.Object, mockPartialRollFactory.Object);

            mockPartialRoll = new Mock<PartialRoll>();
            quantities = new Queue<int>();

            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<int>())).Returns((int q) => BuildMockPartialRoll(q));
            mockExpressionEvaluator.Setup(e => e.Evaluate(It.IsAny<string>())).Returns((string e) => ParseExpression(e));
            mockPartialRoll.Setup(r => r.IndividualRolls(It.IsAny<int>())).Returns((int d) => BuildIndividualRolls(d));
        }

        private PartialRoll BuildMockPartialRoll(int quantity)
        {
            quantities.Enqueue(quantity);
            return mockPartialRoll.Object;
        }

        private int ParseExpression(string expression)
        {
            var value = 0;
            if (int.TryParse(expression, out value))
                return value;

            throw new ArgumentException("This expression was not set up to be parsed");
        }

        private IEnumerable<int> BuildIndividualRolls(int die)
        {
            var rolls = new List<int>();
            var quantity = quantities.Dequeue();

            while (quantity-- > 0)
                rolls.Add(die--);

            return rolls;
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

            var roll = dice.Roll("92d66");
            Assert.That(roll, Is.EqualTo(90210));
        }

        [Test]
        public void TrimExpressionWithReplacedRolls()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });

            var expression = dice.ReplaceRollsWithSum("  92    d  66   ");
            Assert.That(expression, Is.EqualTo("90210"));
        }

        [Test]
        public void HaveSummedSpacesInExpression()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210, 42 });

            var expression = dice.ReplaceRollsWithSum("  92    d  66   ");
            Assert.That(expression, Is.EqualTo("(90210 + 42)"));
        }

        [Test]
        public void RollExpressionWithSpaces()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });

            var roll = dice.Roll("  92    d  66   ");
            Assert.That(roll, Is.EqualTo(90210));
        }

        [Test]
        public void DoNotRollInvalidExpression()
        {
            var exception = new Exception();
            mockExpressionEvaluator.Setup(e => e.Evaluate("invalid expression")).Throws(exception);

            Assert.That(() => dice.Roll("invalid expression"), Throws.Exception.EqualTo(exception));
        }

        [Test]
        public void RollExpressionWithNoQuantity()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(1)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(90210)).Returns(new[] { 9266 });

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
            mockExpressionEvaluator.Setup(e => e.Evaluate("90210+42")).Returns(600);

            var roll = dice.Roll("92d66+42");
            Assert.That(roll, Is.EqualTo(600));
        }

        [Test]
        public void RollExpressionWithMultiplier()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(92)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.IndividualRolls(66)).Returns(new[] { 90210 });
            mockExpressionEvaluator.Setup(e => e.Evaluate("90210*42")).Returns(600);

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

            mockExpressionEvaluator.Setup(e => e.Evaluate("90210+1337")).Returns(1234);

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

            var roll = dice.ReplaceRollsWithSum("7d629%7d629");
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

        [TestCase("1d2", "2")]
        [TestCase("2d3", "(3 + 2)")]
        [TestCase("1+2d3", "1+(3 + 2)")]
        [TestCase("1d2+3", "2+3")]
        [TestCase("6d9k5", "(9 + 8 + 7 + 6 + 5)")]
        [TestCase("6d9k7", "(9 + 8 + 7 + 6 + 5 + 4)")]
        [TestCase("6d9k6", "(9 + 8 + 7 + 6 + 5 + 4)")]
        [TestCase("6d9k1", "9")]
        [TestCase("6d9k0", "0")]
        [TestCase("1d2+3d4", "2+(4 + 3 + 2)")]
        [TestCase("1+2d3-4d6*5", "1+(3 + 2)-(6 + 5 + 4 + 3)*5")]
        [TestCase("1+2d3-4d5*6", "1+(3 + 2)-(5 + 4 + 3 + 2)*6")]
        [TestCase("1+2d3-2d3/4", "1+(3 + 2)-(3 + 2)/4")]
        [TestCase("1+2d3k1-2d3k0/4", "1+3-0/4")]
        [TestCase("1+2d3-2d3*4/5", "1+(3 + 2)-(3 + 2)*4/5")]
        [TestCase("d2", "2")]
        [TestCase("I want to roll a d2.", "I want to roll a 2.")]
        [TestCase("I want to roll 1 d2.", "I want to roll 2.")]
        [TestCase("1+2", "1+2")]
        [TestCase("  1  d     2    ", "2")]
        [TestCase("  6  d     9      k   5  ", "(9 + 8 + 7 + 6 + 5)")]
        [TestCase("one d two", "one d two")]
        [TestCase("other things", "other things")]
        [TestCase("Contains 1d6+2 ghouls and 2d4 zombies", "Contains 6+2 ghouls and (4 + 3) zombies")]
        [TestCase("Contains 1d6+2 ghouls, 1d4+1 skeletons, and 2d4 zombies", "Contains 6+2 ghouls, 4+1 skeletons, and (4 + 3) zombies")]
        [TestCase("Hydra (1d4+4 heads)", "Hydra (4+4 heads)")]
        [TestCase("Hydra (10+2 heads)", "Hydra (10+2 heads)")]
        [TestCase("6d6 fire damage", "(6 + 5 + 4 + 3 + 2 + 1) fire damage")]
        [TestCase("I hit for 1d4", "I hit for 4")]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree",
            "I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree")]
        [TestCase("I am 1d100% confident", "I am 100% confident")]
        [TestCase("Fred2 has 4d16 health.", "Fred2 has (16 + 15 + 14 + 13) health.")]
        [TestCase("Fred2", "Fred2")]
        [TestCase("Fred2k", "Fred2k")]
        [TestCase("I have 2d3k copper pieces.", "I have (3 + 2)k copper pieces.")]
        [TestCase("I have 7d8k3 copper pieces.", "I have (8 + 7 + 6) copper pieces.")]
        public void ReplaceRollsInExpressionWithSums(string roll, string rolled)
        {
            var result = dice.ReplaceRollsWithSum(roll);
            Assert.That(result, Is.EqualTo(rolled));
        }

        [TestCase("1d2", true)]
        [TestCase("2d3", true)]
        [TestCase("1+2d3", true)]
        [TestCase("1d2+3", true)]
        [TestCase("6d9k5", true)]
        [TestCase("6d9k7", true)]
        [TestCase("6d9k6", true)]
        [TestCase("6d9k1", true)]
        [TestCase("6d9k0", true)]
        [TestCase("1d2+3d4", true)]
        [TestCase("1+2d3-4d6*5", true)]
        [TestCase("1+2d3-4d5*6", true)]
        [TestCase("1+2d3-2d3/4", true)]
        [TestCase("1+2d3k1-2d3k0/4", true)]
        [TestCase("1+2d3-2d3*4/5", true)]
        [TestCase("d2", true)]
        [TestCase("I want to roll a d2.", true)]
        [TestCase("I want to roll 1 d2.", true)]
        [TestCase("1+2", false)]
        [TestCase("  1  d     2    ", true)]
        [TestCase("  6  d     9      k   5  ", true)]
        [TestCase("one d two", false)]
        [TestCase("other things", false)]
        [TestCase("Contains 1d6+2 ghouls and 2d4 zombies", true)]
        [TestCase("Contains 1d6+2 ghouls, 1d4+1 skeletons, and 2d4 zombies", true)]
        [TestCase("Hydra (1d4+4 heads)", true)]
        [TestCase("Hydra (10+2 heads)", false)]
        [TestCase("6d6 fire damage", true)]
        [TestCase("I hit for 1d4", true)]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree", false)]
        [TestCase("I am 1d100% confident", true)]
        [TestCase("Fred2 has 4d16 health.", true)]
        [TestCase("Fred2", false)]
        [TestCase("Fred2k", false)]
        [TestCase("I have 2d3k copper pieces.", true)]
        [TestCase("I have 7d8k3 copper pieces.", true)]
        public void ExpressionContainsRoll(string expression, bool containsRoll)
        {
            Assert.That(dice.ContainsRoll(expression), Is.EqualTo(containsRoll));
        }

        [TestCase("{6d9k5}", "1")]
        [TestCase("{6d9}k5", "1k5")]
        [TestCase("{1+2d3}-{2d3}", "1-2")]
        [TestCase("{1+2d3-2d3*4/5}", "1")]
        [TestCase("{d2}", "1")]
        [TestCase("I want to roll a {d2}.", "I want to roll a 1.")]
        [TestCase("I want to roll {1 d2}.", "I want to roll 1.")]
        [TestCase("1+2", "1+2")]
        [TestCase("{1+2}", "1")]
        [TestCase(" { 1  d     2   } ", " 1 ")]
        [TestCase("{  1  d     2    }", "1")]
        [TestCase("{  6  d     9      k   5  }", "1")]
        [TestCase("one d two", "one d two")]
        [TestCase("other things", "other things")]
        [TestCase("Contains {1d6+2} ghouls and {2d4} zombies", "Contains 1 ghouls and 2 zombies")]
        [TestCase("Contains {1d6+2} ghouls, {1d4+1} skeletons, and {2d4} zombies", "Contains 1 ghouls, 2 skeletons, and 3 zombies")]
        [TestCase("Hydra ({1d4+4} heads)", "Hydra (1 heads)")]
        [TestCase("Hydra ({10+2} heads)", "Hydra (1 heads)")]
        [TestCase("{6d6} fire damage", "1 fire damage")]
        [TestCase("I hit for {1d4}", "I hit for 1")]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree",
            "I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree")]
        [TestCase("I am {1d100}% confident", "I am 1% confident")]
        [TestCase("Fred2 has {4d16} health.", "Fred2 has 1 health.")]
        [TestCase("Fred2", "Fred2")]
        [TestCase("Fred2k", "Fred2k")]
        [TestCase("I have {2d3}k copper pieces.", "I have 1k copper pieces.")]
        [TestCase("I have {7d8k3} copper pieces.", "I have 1 copper pieces.")]
        [TestCase("Bark \\{", "Bark {")]
        [TestCase("The druid attacks with his Flame Blade, dealing {1d8+min(4/2, 10)} damage.", "The druid attacks with his Flame Blade, dealing 1 damage.")]
        [TestCase("{d5+d2}", "1")]
        public void ReplaceWrappedExpressions(string roll, string rolled)
        {
            var count = 1;
            mockExpressionEvaluator.Setup(e => e.Evaluate(It.IsAny<string>())).Returns(() => count++);

            var newExpression = dice.ReplaceWrappedExpressions<int>(roll);
            Assert.That(newExpression, Is.EqualTo(rolled));
        }

        [Test]
        public void ReplaceWrappedExpressionsWithInt()
        {
            var expression = "The druid attacks with his Flame Blade, dealing { 1d8 + min(4 / 2, 10)} damage.";
            mockExpressionEvaluator.Setup(e => e.Evaluate(" 8 + min(4 / 2, 10)")).Returns(9266);

            var result = dice.ReplaceWrappedExpressions<int>(expression);
            Assert.That(result, Is.EqualTo("The druid attacks with his Flame Blade, dealing 9266 damage."));
        }

        [Test]
        public void ReplaceWrappedExpressionsWithDouble()
        {
            var expression = "Bark {1d1/2}";
            mockExpressionEvaluator.Setup(e => e.Evaluate("1/2")).Returns(0.5);

            var result = dice.ReplaceWrappedExpressions<double>(expression);
            Assert.That(result, Is.EqualTo("Bark 0.5"));
        }

        [TestCase("Bark <1d4+4>", "Bark 9266", "<", ">")]
        [TestCase("Bark [[1d4+4]]", "Bark 9266", "[[", "]]")]
        [TestCase("Bark {1d4+4}", "Bark {1d4+4}", "[[", "]]")]
        public void ReplaceWrappedExpressionsAlternateWrap(string roll, string rolled, string openexpr, string closeexpr)
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate("4+4")).Returns(9266);

            var result = dice.ReplaceWrappedExpressions<int>(roll, openexpr, closeexpr);
            Assert.That(result, Is.EqualTo(rolled));
        }

        [Test]
        public void ReplaceWrappedExpressionsNoEscape()
        {
            var expression = "Bark \\{1d4+4}";
            mockExpressionEvaluator.Setup(e => e.Evaluate("4+4")).Returns(9266);

            var result = dice.ReplaceWrappedExpressions<int>(expression, openexprescape: null);
            Assert.That(result, Is.EqualTo("Bark \\9266"));
        }

        [Test]
        public void ThrowExceptionIfYouTryToEvaluateAnExpressionWithUnrolledDieRolls()
        {
            Assert.That(() => dice.Evaluate("1+2d3-45d67"), Throws.InstanceOf<ArgumentException>().With.Message.EqualTo("Cannot evaluate unrolled die roll 2d3"));
        }

        [TestCase("1d2", "2")]
        [TestCase("2d3", "5")]
        [TestCase("1+2d3", "1")]
        [TestCase("1d2+3", "1")]
        [TestCase("1d2+3d4", "1")]
        [TestCase("6d9k5", "35")]
        [TestCase("6d9k7", "39")]
        [TestCase("6d9k6", "39")]
        [TestCase("6d9k1", "9")]
        [TestCase("6d9k0", "0")]
        [TestCase("1+2d3-4d6*5", "1")]
        [TestCase("1+2d3-4d5*6", "1")]
        [TestCase("1+2d3-2d3/4", "1")]
        [TestCase("1+2d3-2d3*4/5", "1")]
        [TestCase("1+2d3k1-2d3k0/4", "1")]
        [TestCase("d2", "2")]
        [TestCase("I want to roll a d2.", "I want to roll a 2.")]
        [TestCase("I want to roll 1 d2.", "I want to roll 2.")]
        [TestCase("1+2", "1")]
        [TestCase("  1  d     2    ", "  2    ")]
        [TestCase("one d two", "one d two")]
        [TestCase("other things", "other things")]
        [TestCase("Contains 1d6+2 ghouls and 2d4 zombies", "Contains 1 ghouls and 7 zombies")]
        [TestCase("Contains 1d6+2 ghouls, 1d4+1 skeletons, and 2d4 zombies", "Contains 1 ghouls, 2 skeletons, and 7 zombies")]
        [TestCase("Hydra (1d4+4 heads)", "Hydra (1 heads)")]
        [TestCase("Hydra (10+2 heads)", "Hydra (1 heads)")]
        [TestCase("6d6 fire damage", "21 fire damage")]
        [TestCase("I hit for 1d4", "I hit for 4")]
        [TestCase("I hit for 1d4+1", "I hit for 1")]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree",
            "I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree")]
        [TestCase("I am 1d100% confident", "I am 100% confident")]
        [TestCase("Fred2 has 4d16 health.", "Fred2 has 58 health.")]
        [TestCase("Fred2", "Fred2")]
        [TestCase("Fred2k", "Fred2k")]
        [TestCase("I have 2d3k copper pieces.", "I have 5k copper pieces.")]
        [TestCase("I have 7d8k3 copper pieces.", "I have 21 copper pieces.")]
        public void ReplaceExpressionWithTotals(string expression, string expectedExpression)
        {
            var count = 1;
            mockExpressionEvaluator.Setup(e => e.Evaluate(It.IsAny<string>())).Returns(() => count++);

            var newExpression = dice.ReplaceExpressionWithTotal(expression);
            Assert.That(newExpression, Is.EqualTo(expectedExpression));
        }
    }
}