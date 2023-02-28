using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration
{
    [TestFixture]
    public class RollHelperTests : IntegrationTests
    {
        private Dice dice;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "2d3-1")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "2d4-1")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d20+3d6-3")]
        [TestCase(1, 36, "1d20+1d10+1d8-2")]
        [TestCase(1, 48, "2d20+1d10-2")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 9, "1d8+1d2-1")]
        [TestCase(2, 8, "2d4")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "1d20+1d10+1d8+2")]
        [TestCase(16, 50, "1d20+3d6+12")]
        [TestCase(100, 10_000, "100d100")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "7d100+3d20+1d12+2d4+424")]
        [TestCase(1336, 90210, "897d100+3d20+2d8+434")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithMostEvenDistribution(int lower, int upper, string expectedRoll)
        {
            var roll = RollHelper.GetRollWithMostEvenDistribution(lower, upper);
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "2d3-1")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "2d4-1")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "17d3-16")]
        [TestCase(1, 36, "5d8-4")]
        [TestCase(1, 48, "47d2-46")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 9, "4d3-3")]
        [TestCase(2, 8, "2d4")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "5d8")]
        [TestCase(16, 50, "17d3-1")]
        [TestCase(100, 10_000, "100d100")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "767d2-330")]
        [TestCase(1336, 90210, "897d100+71d2+368")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithFewestDice(int lower, int upper, string expectedRoll)
        {
            var roll = RollHelper.GetRollWithFewestDice(lower, upper);
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "2d3-1")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "2d4-1")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d20+3d6-3")]
        [TestCase(1, 36, "(1d12-1)*3+1d3")]
        [TestCase(1, 48, "(1d12-1)*4+1d4")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 9, "(1d3-1)*3+1d3")]
        [TestCase(2, 8, "2d4")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "(1d12-1)*3+1d3+4")]
        [TestCase(16, 50, "1d20+3d6+12")]
        [TestCase(100, 10_000, "100d100")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "(1d12-1)*64+(1d8-1)*8+1d8+436")]
        [TestCase(1336, 90210, "897d100+3d20+2d8+434")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithPerfectDistribution(int lower, int upper, string expectedRoll)
        {
            var roll = RollHelper.GetRollWithPerfectDistribution(lower, upper);
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
        }

        [TestCase(0, 0, "0")]
        [TestCase(1, 1, "1")]
        [TestCase(2, 2, "2")]
        [TestCase(9266, 9266, "9266")]
        [TestCase(0, 1, "1d2-1")]
        [TestCase(1, 2, "1d2")]
        [TestCase(1, 3, "1d3")]
        [TestCase(1, 4, "1d4")]
        [TestCase(1, 5, "1d5")]
        [TestCase(1, 6, "1d6")]
        [TestCase(1, 7, "1d7")]
        [TestCase(1, 8, "1d8")]
        [TestCase(1, 10, "1d10")]
        [TestCase(1, 12, "1d12")]
        [TestCase(1, 20, "1d20")]
        [TestCase(1, 35, "1d35")]
        [TestCase(1, 36, "(1d12-1)*3+1d3")]
        [TestCase(1, 48, "(1d12-1)*4+1d4")]
        [TestCase(1, 100, "1d100")]
        [TestCase(1, 9, "(1d3-1)*3+1d3")]
        [TestCase(2, 8, "1d7+1")]
        [TestCase(4, 9, "1d6+3")]
        [TestCase(5, 40, "(1d12-1)*3+1d3+4")]
        [TestCase(16, 50, "1d35+15")]
        [TestCase(100, 10_000, "1d9901+99")]
        [TestCase(101, 200, "1d100+100")]
        [TestCase(437, 1204, "(1d12-1)*64+(1d8-1)*8+1d8+436")]
        [TestCase(1336, 90210, "897d100+3d20+2d8+434")]
        [TestCase(10_000, 1_000_000, "10000d100")]
        public void RollWithPerfectDistribution_AllowNonstandard(int lower, int upper, string expectedRoll)
        {
            var roll = RollHelper.GetRollWithPerfectDistribution(lower, upper, true);
            Assert.That(roll, Is.EqualTo(expectedRoll));
            Assert.That(dice.Roll(roll).IsValid(), Is.True);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower));
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper));
        }
    }
}
