using DnDGen.RollGen.Expressions;
using DnDGen.RollGen.PartialRolls;
using Moq;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Unit
{
    [TestFixture]
    public class DomainDiceTests
    {
        private Dice dice;
        private Mock<PartialRollFactory> mockPartialRollFactory;
        private Mock<PartialRoll> mockPartialRoll;

        [SetUp]
        public void Setup()
        {
            mockPartialRollFactory = new Mock<PartialRollFactory>();
            dice = new DomainDice(mockPartialRollFactory.Object);

            mockPartialRoll = new Mock<PartialRoll>();

            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<int>())).Returns(mockPartialRoll.Object);
            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<string>())).Returns(mockPartialRoll.Object);
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
        public void RollExpression()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("92d66")).Returns(mockPartialRollWithQuantity.Object);

            var partialRoll = dice.Roll("92d66");
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockPartialRollWithQuantity.Object));
        }

        [Test]
        public void RollAnotherRoll()
        {
            var otherRoll = new DomainPartialRoll("92d66", new Mock<Random>().Object, new Mock<ExpressionEvaluator>().Object);

            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("(92d66)")).Returns(mockPartialRollWithQuantity.Object);

            var partialRoll = dice.Roll(otherRoll);
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockPartialRollWithQuantity.Object));
        }

        [Test]
        public void TrimExpressionWithReplacedRolls()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("92    d  66")).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.AsIndividualRolls<int>()).Returns(new[] { 90210 });

            var expression = dice.ReplaceRollsWithSumExpression("  92    d  66   ");
            Assert.That(expression, Is.EqualTo("90210"));
        }

        [Test]
        public void HaveSummedSpacesInExpression()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("92    d  66")).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.AsIndividualRolls<int>()).Returns(new[] { 90210, 42 });

            var expression = dice.ReplaceRollsWithSumExpression("  92    d  66   ");
            Assert.That(expression, Is.EqualTo("(90210 + 42)"));
        }

        [Test]
        public void RollExpressionWithSpaces()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("  92    d  66   ")).Returns(mockPartialRollWithQuantity.Object);

            var partialRoll = dice.Roll("  92    d  66   ");
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockPartialRollWithQuantity.Object));
        }

        [Test]
        public void RollExpressionWithBonus()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("92d66+42")).Returns(mockPartialRollWithQuantity.Object);

            var partialRoll = dice.Roll("92d66+42");
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockPartialRollWithQuantity.Object));
        }

        [Test]
        public void RollExpressionWithMultiplier()
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("92d66*42")).Returns(mockPartialRollWithQuantity.Object);

            var partialRoll = dice.Roll("92d66*42");
            Assert.That(partialRoll, Is.InstanceOf<PartialRoll>());
            Assert.That(partialRoll, Is.EqualTo(mockPartialRollWithQuantity.Object));
        }

        [Test]
        public void ReplaceDieWithSumsInExpressionWithMultipleRolls()
        {
            var mockFirstPartialRoll = new Mock<PartialRoll>();
            var mockSecondPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("92d66")).Returns(mockFirstPartialRoll.Object);
            mockPartialRollFactory.Setup(f => f.Build("42d600")).Returns(mockSecondPartialRoll.Object);

            mockFirstPartialRoll.Setup(r => r.AsIndividualRolls<int>()).Returns(new[] { 9266, 90210 });
            mockSecondPartialRoll.Setup(r => r.AsIndividualRolls<int>()).Returns(new[] { 42, 600 });

            var expression = dice.ReplaceRollsWithSumExpression("92d66+42d600");
            Assert.That(expression, Is.EqualTo("(9266 + 90210)+(42 + 600)"));
        }

        [Test]
        public void ReplaceDieWithSumsInExpressionWithMultipleOfSameRoll()
        {
            var mockFirstPartialRoll = new Mock<PartialRoll>();
            var mockSecondPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.SetupSequence(f => f.Build("7d629")).Returns(mockFirstPartialRoll.Object).Returns(mockSecondPartialRoll.Object);

            mockFirstPartialRoll.Setup(r => r.AsIndividualRolls<int>()).Returns(new[] { 9266, 90210 });
            mockSecondPartialRoll.Setup(r => r.AsIndividualRolls<int>()).Returns(new[] { 42, 600 });

            var roll = dice.ReplaceRollsWithSumExpression("7d629%7d629");
            Assert.That(roll, Is.EqualTo("(9266 + 90210)%(42 + 600)"));
        }

        [TestCase("1d2", "1")]
        [TestCase("2d3", "1")]
        [TestCase("1+2d3", "1+1")]
        [TestCase("1d2+3", "1+3")]
        [TestCase("6d9k5", "1")]
        [TestCase("6d9k7", "1")]
        [TestCase("6d9k6", "1")]
        [TestCase("6d9k1", "1")]
        [TestCase("6d9k0", "1")]
        [TestCase("1d2+3d4", "1+(2 + 3)")]
        [TestCase("1+2d3-4d6*5", "1+1-(2 + 3)*5")]
        [TestCase("1+2d3-4d5*6", "1+1-(2 + 3)*6")]
        [TestCase("1+2d3-2d3/4", "1+1-(2 + 3)/4")]
        [TestCase("1+2d3k1-2d3k0/4", "1+1-(2 + 3)/4")]
        [TestCase("1+2d3-2d3*4/5", "1+1-(2 + 3)*4/5")]
        [TestCase("d2", "1")]
        [TestCase("I want to roll a d2.", "I want to roll a 1.")]
        [TestCase("I want to roll 1 d2.", "I want to roll 1.")]
        [TestCase("1+2", "1+2")]
        [TestCase("  1  d     2    ", "1")]
        [TestCase("  6  d     9      k    5  ", "1")]
        [TestCase("  6  d     9   !   k    5  ", "1")]
        [TestCase("  6  d     9     k    5  !", "1")]
        [TestCase("one d two", "one d two")]
        [TestCase("other things", "other things")]
        [TestCase("Contains 1d6+2 ghouls and 2d4 zombies", "Contains 1+2 ghouls and (2 + 3) zombies")]
        [TestCase("Contains 1d6+2 ghouls, 1d4+1 skeletons, and 2d4 zombies", "Contains 1+2 ghouls, (2 + 3)+1 skeletons, and (3 + 4 + 5) zombies")]
        [TestCase("Hydra (1d4+4 heads)", "Hydra (1+4 heads)")]
        [TestCase("Hydra (10+2 heads)", "Hydra (10+2 heads)")]
        [TestCase("6d6 fire damage", "1 fire damage")]
        [TestCase("I hit for 1d4", "I hit for 1")]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree",
            "I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree")]
        [TestCase("I am 1d100% confident", "I am 1% confident")]
        [TestCase("Fred2 has 4d16 health.", "Fred2 has 1 health.")]
        [TestCase("Fred2", "Fred2")]
        [TestCase("Fred2k", "Fred2k")]
        [TestCase("I have 2d3k copper pieces.", "I have 1k copper pieces.")]
        [TestCase("I have 7d8k3 copper pieces.", "I have 1 copper pieces.")]
        public void ReplaceRollsInExpressionWithSums(string roll, string rolled)
        {
            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<string>())).Returns(mockPartialRoll.Object);

            var count = 1;
            mockPartialRoll.Setup(r => r.AsIndividualRolls<int>()).Returns(() => Enumerable.Range(count, count++));

            var result = dice.ReplaceRollsWithSumExpression(roll);
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
            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<string>())).Returns(mockPartialRoll.Object);

            var count = 1;
            mockPartialRoll.Setup(r => r.AsSum<int>()).Returns(() => count++);

            var newExpression = dice.ReplaceWrappedExpressions<int>(roll);
            Assert.That(newExpression, Is.EqualTo(rolled));
        }

        [Test]
        public void ReplaceWrappedExpressionsWithInt()
        {
            var expression = "The druid attacks with his Flame Blade, dealing { 1d8 + min(4 / 2, 10)} damage.";

            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(" 1d8 + min(4 / 2, 10)")).Returns(mockPartialRoll.Object);
            mockPartialRoll.Setup(r => r.AsSum<int>()).Returns(9266);

            var result = dice.ReplaceWrappedExpressions<int>(expression);
            Assert.That(result, Is.EqualTo("The druid attacks with his Flame Blade, dealing 9266 damage."));
        }

        [Test, Ignore("Until generic partial rolls occur, this will be impossible")]
        public void ReplaceWrappedExpressionsWithDouble()
        {
            var expression = "Bark {1d1/2}";

            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("1d1/2")).Returns(mockPartialRoll.Object);
            mockPartialRoll.Setup(r => r.AsSum()).Returns(9266); //should be 0.5

            var result = dice.ReplaceWrappedExpressions<double>(expression);
            Assert.That(result, Is.EqualTo("Bark 0.5"));
        }

        [Test]
        public void ReplaceWrappedExpressionsWithTrue()
        {
            var expression = "Bark {1d1 > 2}";

            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("1d1 > 2")).Returns(mockPartialRoll.Object);
            mockPartialRoll.Setup(r => r.AsTrueOrFalse(.5)).Returns(true);

            var result = dice.ReplaceWrappedExpressions<bool>(expression);
            Assert.That(result, Is.EqualTo("Bark True"));
        }

        [Test]
        public void ReplaceWrappedExpressionsWithFalse()
        {
            var expression = "Bark {1d1 > 2}";

            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("1d1 > 2")).Returns(mockPartialRoll.Object);
            mockPartialRoll.Setup(r => r.AsTrueOrFalse(.5)).Returns(false);

            var result = dice.ReplaceWrappedExpressions<bool>(expression);
            Assert.That(result, Is.EqualTo("Bark False"));
        }

        [TestCase("Bark <1d4+4>", "Bark 9266", "<", ">")]
        [TestCase("Bark [[1d4+4]]", "Bark 9266", "[[", "]]")]
        [TestCase("Bark {1d4+4}", "Bark {1d4+4}", "[[", "]]")]
        public void ReplaceWrappedExpressionsAlternateWrap(string roll, string rolled, string openexpr, string closeexpr)
        {
            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("1d4+4")).Returns(mockPartialRoll.Object);
            mockPartialRoll.Setup(r => r.AsSum<int>()).Returns(9266);

            var result = dice.ReplaceWrappedExpressions<int>(roll, openexpr, closeexpr);
            Assert.That(result, Is.EqualTo(rolled));
        }

        [Test]
        public void ReplaceWrappedExpressionsNoEscape()
        {
            var expression = "Bark \\{1d4+4}";

            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build("1d4+4")).Returns(mockPartialRoll.Object);
            mockPartialRoll.Setup(r => r.AsSum<int>()).Returns(9266);

            var result = dice.ReplaceWrappedExpressions<int>(expression, openexprescape: null);
            Assert.That(result, Is.EqualTo("Bark \\9266"));
        }

        [TestCase("1d2", "1")]
        [TestCase("2d3", "1")]
        [TestCase("1+2d3", "2")]
        [TestCase("1d2+3", "2")]
        [TestCase("1d2+3d4", "3")]
        [TestCase("6d9k5", "1")]
        [TestCase("6d9k7", "1")]
        [TestCase("6d9k6", "1")]
        [TestCase("6d9k1", "1")]
        [TestCase("6d9k0", "1")]
        [TestCase("1+2d3-4d6*5", "3")]
        [TestCase("1+2d3-4d5*6", "3")]
        [TestCase("1+2d3-2d3/4", "3")]
        [TestCase("1+2d3-2d3*4/5", "3")]
        [TestCase("1+2d3k1-2d3k0/4", "3")]
        [TestCase("d2", "1")]
        [TestCase("I want to roll a d2.", "I want to roll a 1.")]
        [TestCase("I want to roll 1 d2.", "I want to roll 1.")]
        [TestCase("1+2", "1")]
        [TestCase("  1  d     2    ", "  1    ")]
        [TestCase("one d two", "one d two")]
        [TestCase("other things", "other things")]
        [TestCase("Contains 1d6+2 ghouls and 2d4 zombies", "Contains 3 ghouls and 2 zombies")]
        [TestCase("Contains 1d6+2 ghouls, 1d4+1 skeletons, and 2d4 zombies", "Contains 4 ghouls, 5 skeletons, and 3 zombies")]
        [TestCase("Hydra (1d4+4 heads)", "Hydra (2 heads)")]
        [TestCase("Hydra (10+2 heads)", "Hydra (1 heads)")]
        [TestCase("6d6 fire damage", "1 fire damage")]
        [TestCase("I hit for 1d4", "I hit for 1")]
        [TestCase("I hit for 1d4+1", "I hit for 2")]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree",
            "I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree")]
        [TestCase("I am 1d100% confident", "I am 1% confident")]
        [TestCase("Fred2 has 4d16 health.", "Fred2 has 1 health.")]
        [TestCase("Fred2", "Fred2")]
        [TestCase("Fred2k", "Fred2k")]
        [TestCase("I have 2d3k copper pieces.", "I have 1k copper pieces.")]
        [TestCase("I have 7d8k3 copper pieces.", "I have 1 copper pieces.")]
        [TestCase("I have 2d8! copper pieces.", "I have 1 copper pieces.")]
        public void ReplaceExpressionWithTotals(string expression, string expectedExpression)
        {
            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<string>())).Returns(mockPartialRoll.Object);

            var count = 1;
            mockPartialRoll.Setup(r => r.AsSum<int>()).Returns(() => count++);

            var newExpression = dice.ReplaceExpressionWithTotal(expression);
            Assert.That(newExpression, Is.EqualTo(expectedExpression));
        }

        [TestCase("1d2", "1")]
        [TestCase("2d3", "1")]
        [TestCase("2d3!", "1")]
        [TestCase("1+2d3", "2")]
        [TestCase("1d2+3", "2")]
        [TestCase("1d2+3d4", "3")]
        [TestCase("6d9k5", "1")]
        [TestCase("6d9k7", "1")]
        [TestCase("6d9k6", "1")]
        [TestCase("6d9k1", "1")]
        [TestCase("6d9k0", "1")]
        [TestCase("1+2d3-4d6*5", "3")]
        [TestCase("1+2d3-4d5*6", "3")]
        [TestCase("1+2d3-2d3/4", "3")]
        [TestCase("1+2d3-2d3*4/5", "3")]
        [TestCase("1+2d3k1-2d3k0/4", "3")]
        [TestCase("d2", "1")]
        [TestCase("I want to roll a d2.", "I want to roll a 1.")]
        [TestCase("I want to roll 1 d2.", "I want to roll 1.")]
        [TestCase("1+2", "1")]
        [TestCase("  1  d     2    ", "  1    ")]
        [TestCase("one d two", "one d two")]
        [TestCase("other things", "other things")]
        [TestCase("Contains 1d6+2 ghouls and 2d4 zombies", "Contains 4 ghouls an3 zombies")]
        [TestCase("Contains 1d6+2 ghouls, 1d4+1 skeletons, and 2d4 zombies", "Contains 5 ghouls, 6 skeletons, an4 zombies")]
        [TestCase("Hydra (1d4+4 heads)", "Hydra (2 heads)")]
        [TestCase("Hydra (10+2 heads)", "Hydra (1 heads)")]
        [TestCase("6d6 fire damage", "1 fire damage")]
        [TestCase("I hit for 1d4", "I hit for 1")]
        [TestCase("I hit for 1d4+1", "I hit for 2")]
        [TestCase("I found 5 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, and 1 partridge in a pear tree",
            "I foun1 golden rings, 4 calling birds, 3 french hens, 2 turtle doves, an2 partridge in a pear tree")]
        [TestCase("I am 1d100% confident", "I am 1% confident")]
        [TestCase("Fred2 has 4d16 health.", "Fre1 has 2 health.")]
        [TestCase("Fred2", "Fre1")]
        [TestCase("Fred2k", "Fre1k")]
        [TestCase("I have 2d3k copper pieces.", "I have 1k copper pieces.")]
        [TestCase("I have 7d8k3 copper pieces.", "I have 1 copper pieces.")]
        [TestCase("Gonna die, roll a 1d2d3d4d5d6!!", "Gonna die, roll a 5!")]
        [TestCase("Gonna die, roll a 1d2d3d4d5d6!!!", "Gonna die, roll a 5!!")]
        [TestCase("Gonna die, roll a 1 d 2 d 3 d 4 d 5 d 6!!", "Gonna die, roll a 5!")]
        [TestCase("Gonna die, roll a 1 d 2 d 3 d 4 d 5 d 6!!!", "Gonna die, roll a 5!!")]
        public void LenientReplaceExpressionWithTotals(string expression, string expectedExpression)
        {
            var mockPartialRoll = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(It.IsAny<string>())).Returns(mockPartialRoll.Object);

            var count = 1;
            mockPartialRoll.Setup(r => r.AsSum<int>()).Returns(() => count++);

            var newExpression = dice.ReplaceExpressionWithTotal(expression, true);
            Assert.That(newExpression, Is.EqualTo(expectedExpression));
        }

        [TestCase("1d20 > 10", true)]
        [TestCase("1d20 < 2d10", true)]
        [TestCase("3d2 >= 2d3", true)]
        [TestCase("2d6 <= 3d4", true)]
        [TestCase("1d2 = 2", true)]
        [TestCase("1d100 > 0", true)]
        [TestCase("100 < 1 d 20", false)]
        [TestCase("100<1d20", false)]
        [TestCase("1d1 = 1", true)]
        [TestCase("9266 = 9266", true)]
        [TestCase("9266 = 90210", false)]
        [TestCase("9266=90210", false)]
        [TestCase("1d2 = 3", false)]
        public void RollBooleanExpression(string expression, bool result)
        {
            var mockPartialRollWithQuantity = new Mock<PartialRoll>();
            mockPartialRollFactory.Setup(f => f.Build(expression)).Returns(mockPartialRollWithQuantity.Object);

            mockPartialRollWithQuantity.Setup(r => r.AsTrueOrFalse(.5)).Returns(result);

            var evaluatedResult = dice.Roll(expression).AsTrueOrFalse();
            Assert.That(evaluatedResult, Is.EqualTo(result));
        }
    }
}