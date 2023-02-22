using NUnit.Framework;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration
{
    [TestFixture]
    public class ReadmeTests : IntegrationTests
    {
        private Dice dice;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
        }

        [Test]
        [Repeat(100)]
        public void StandardRoll()
        {
            var roll = dice.Roll(4).d6().AsSum();
            Assert.That(roll, Is.InRange(4, 24));
        }

        [Test]
        [Repeat(100)]
        public void CustomRoll()
        {
            var roll = dice.Roll(92).d(66).AsSum();
            Assert.That(roll, Is.InRange(92, 92 * 66));
        }

        [Test]
        [Repeat(100)]
        public void ExpressionRoll()
        {
            var roll = dice.Roll("5+3d4*2").AsSum();
            Assert.That(roll, Is.InRange(11, 29));
        }

        [Test]
        [Repeat(100)]
        public void ChainedRolls()
        {
            var roll = dice.Roll().d2().d(5).Keeping(1).d6().AsSum();
            Assert.That(roll, Is.InRange(1, 30));
        }

        [Test]
        [Repeat(100)]
        public void IndividualRolls()
        {
            var rolls = dice.Roll(4).d6().AsIndividualRolls();
            Assert.That(rolls, Has.All.InRange(1, 6));
        }

        [Test]
        [Repeat(100)]
        public void ParsedRolls()
        {
            var rolls = dice.Roll("5+3d4*2").AsIndividualRolls();
            Assert.That(rolls, Has.All.InRange(11, 29));
            Assert.That(rolls.Count(), Is.EqualTo(1));
        }

        [Test]
        [Repeat(100)]
        public void KeptRolls()
        {
            var rolls = dice.Roll(4).d6().Keeping(3).AsIndividualRolls();
            Assert.That(rolls, Has.All.InRange(1, 6));
            Assert.That(rolls.Count(), Is.EqualTo(3));
        }

        [Test]
        [Repeat(100)]
        public void ExpressionKeptRolls()
        {
            var roll = dice.Roll("3d4k2").AsSum();
            Assert.That(roll, Is.InRange(2, 8));
        }

        [Test]
        public void AverageRoll()
        {
            var roll = dice.Roll(4).d6().AsPotentialAverage();
            Assert.That(roll, Is.EqualTo(14));
        }

        [Test]
        public void ExpressionAverageRoll()
        {
            var roll = dice.Roll("5+3d4*3").AsPotentialAverage();
            Assert.That(roll, Is.EqualTo(27.5));
        }

        [Test]
        public void MinimumRoll()
        {
            var roll = dice.Roll(4).d6().AsPotentialMinimum();
            Assert.That(roll, Is.EqualTo(4));
        }

        [Test]
        public void ExpressionMinimumRoll()
        {
            var roll = dice.Roll("5+3d4*3").AsPotentialMinimum();
            Assert.That(roll, Is.EqualTo(14));
        }

        [Test]
        public void MaximumRoll()
        {
            var roll = dice.Roll(4).d6().AsPotentialMaximum();
            Assert.That(roll, Is.EqualTo(24));
        }

        [Test]
        public void ExpressionMaximumRoll()
        {
            var roll = dice.Roll("5+3d4*3").AsPotentialMaximum();
            Assert.That(roll, Is.EqualTo(41));
        }

        [Test]
        [Repeat(100)]
        public void Success()
        {
            var roll = dice.Roll().Percentile().AsTrueOrFalse();
            Assert.That(roll, Is.True.Or.False);
        }

        [Test]
        [Repeat(100)]
        public void CustomPercentageSuccess()
        {
            var roll = dice.Roll().Percentile().AsTrueOrFalse(.9);
            Assert.That(roll, Is.True.Or.False);
        }

        [Test]
        [Repeat(100)]
        public void CustomRollSuccess()
        {
            var roll = dice.Roll().Percentile().AsTrueOrFalse(90);
            Assert.That(roll, Is.True.Or.False);
        }

        [Test]
        [Repeat(100)]
        public void ExpressionSuccess()
        {
            var roll = dice.Roll("5+3d4*2").AsTrueOrFalse();
            Assert.That(roll, Is.True.Or.False);
        }

        [Test]
        [Repeat(100)]
        public void ExplicitExpressionSuccess()
        {
            var roll = dice.Roll("2d6>=1d12").AsTrueOrFalse();
            Assert.That(roll, Is.True.Or.False);
        }

        [Test]
        public void ContainsRoll()
        {
            var containsRoll = dice.ContainsRoll("This contains a roll of 4d6k3 for rolling stats");
            Assert.That(containsRoll, Is.True);
        }

        [Test]
        [Repeat(100)]
        public void SummedSentence()
        {
            var summedSentence = dice.ReplaceRollsWithSumExpression("This contains a roll of 4d6k3 for rolling stats");
            Assert.That(summedSentence, Does.Match(@"This contains a roll of \([1-6] \+ [1-6] \+ [1-6]\) for rolling stats"));
        }

        [Test]
        [Repeat(100)]
        public void RolledSentence()
        {
            var rolledSentence = dice.ReplaceExpressionWithTotal("This contains a roll of 4d6k3 for rolling stats");
            Assert.That(rolledSentence, Does.Match(@"This contains a roll of ([3-9]|1[0-8]) for rolling stats"));
        }

        [Test]
        [Repeat(100)]
        public void RolledComplexSentence()
        {
            var rolledComplexSentence = dice.ReplaceWrappedExpressions<double>("Fireball does {min\\(4d6,10\\) + 0.5} damage");
            Assert.That(rolledComplexSentence, Does.Match(@"Fireball does (1[0-9]|2[0-4]).5 damage"));
        }

        [Test]
        public void OptimizedRoll()
        {
            var roll = RollHelper.GetRollWithMostEvenDistribution(4, 9);
            Assert.That(roll, Is.EqualTo("1d6+3"));
        }

        [Test]
        public void OptimizedRollWithMultipleDice()
        {
            var roll = RollHelper.GetRollWithMostEvenDistribution(1, 9);
            Assert.That(roll, Is.EqualTo("1d8+1d2-1"));
        }

        [Test]
        public void OptimizedRollWithFewestDice()
        {
            var roll = RollHelper.GetRollWithFewestDice(1, 9);
            Assert.That(roll, Is.EqualTo("4d3-3"));
        }

        [Test]
        [Repeat(100)]
        public void ExplodedRolls()
        {
            var explodedRolls = dice.Roll(4).d6().Explode().AsIndividualRolls();
            Assert.That(explodedRolls, Has.All.InRange(1, 6));
            Assert.That(explodedRolls.Count(), Is.AtLeast(4).And.EqualTo(4 + explodedRolls.Count(r => r == 6)));
        }

        [Test]
        [Repeat(100)]
        public void ExpressionExplodedRolls()
        {
            var expressionExplodedRolls = dice.Roll("3d4!").AsSum();
            Assert.That(expressionExplodedRolls, Is.InRange(3, 52)); //max+10*die
        }

        [Test]
        [Repeat(100)]
        public void ExpressionExplodedKeptRolls()
        {
            var expressionExplodedRolls = dice.Roll("3d4!k2").AsSum();
            Assert.That(expressionExplodedRolls, Is.InRange(2, 8));
        }

        [Test]
        [Repeat(100)]
        public void ExpressionExplodedMultipleRolls()
        {
            var expressionExplodedMultipleRolls = dice.Roll("3d4!e3").AsSum();
            Assert.That(expressionExplodedMultipleRolls, Is.InRange(3, 82)); //max+10*die
        }

        [Test]
        [Repeat(100)]
        public void ExpressionExplodedMultipleKeptRolls()
        {
            var expressionExplodedMultipleKeptRolls = dice.Roll("3d4e1e2k2").AsSum();
            Assert.That(expressionExplodedMultipleKeptRolls, Is.InRange(6, 8));
        }

        [Test]
        [Repeat(100)]
        public void TransformedRolls()
        {
            var transformedRolls = dice.Roll(3).d6().Transforming(1).AsIndividualRolls();
            Assert.That(transformedRolls, Has.All.InRange(2, 6));
            Assert.That(transformedRolls.Count(), Is.EqualTo(3));
        }

        [Test]
        [Repeat(100)]
        public void TransformedMultipleRolls()
        {
            var transformedMultipleRolls = dice.Roll("3d6t1t5").AsSum();
            Assert.That(transformedMultipleRolls, Is.InRange(6, 18));
        }

        [Test]
        [Repeat(100)]
        public void TransformedExplodedKeptRolls()
        {
            var transformedExplodedKeptRolls = dice.Roll("3d6!t1k2").AsSum();
            Assert.That(transformedExplodedKeptRolls, Is.InRange(4, 12));
        }

        [Test]
        [Repeat(100)]
        public void TransformedCustomRolls()
        {
            var transformedCustomRolls = dice.Roll("3d6t1:2").AsSum();
            Assert.That(transformedCustomRolls, Is.InRange(6, 18));
        }

        [TestCase(9, "Bad")]
        [TestCase(13, "Good")]
        public void DescribeRoll(int roll, string expectedDescription)
        {
            var description = dice.Describe("3d6", roll);
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase(6, "Bad")]
        [TestCase(12, "Average")]
        [TestCase(16, "Good")]
        public void DescribeRoll_CustomDescriptions(int roll, string expectedDescription)
        {
            var description = dice.Describe("3d6", roll, "Bad", "Average", "Good");
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [Test]
        [Repeat(100)]
        public void SingleIndividualRollForExpression()
        {
            var roll = dice.Roll("1d2+3d4").AsIndividualRolls();
            Assert.That(roll, Has.All.InRange(4, 14));
            Assert.That(roll.Count(), Is.EqualTo(1));
        }

        [Test]
        [Repeat(100)]
        public void RolledSentence_Strict()
        {
            var rolledSentence = dice.ReplaceExpressionWithTotal("1d2 ghouls and 2d4 zombies");
            Assert.That(rolledSentence, Does.Match(@"[1-2] ghouls and [2-8] zombies"));
        }

        [Test]
        [Repeat(100)]
        public void RolledSentence_Lenient()
        {
            var rolledSentence = dice.ReplaceExpressionWithTotal("1d2 ghouls and 2d4 zombies", true);
            Assert.That(rolledSentence, Does.Match(@"[1-2] ghouls an[1-8] zombies"));
        }

        [TestCase("4d3!t2k1")]
        [TestCase("4d3!k1t2")]
        [TestCase("4d3t2!k1")]
        [TestCase("4d3t2k1!")]
        [TestCase("4d3k1!t2")]
        [TestCase("4d3k1t2!")]
        [Repeat(100)]
        public void OrderOfOperations(string rollExpression)
        {
            var roll = dice.Roll(rollExpression).AsSum();
            Assert.That(roll, Is.InRange(1, 4));
        }

        [Test]
        [Repeat(100)]
        public void KeptRolls_AsSum()
        {
            var rolls = dice.Roll(4).d6().Keeping(3).AsSum();
            Assert.That(rolls, Is.InRange(3, 18));
        }
    }
}
