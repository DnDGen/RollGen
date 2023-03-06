using NUnit.Framework;
using System;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class RollHelperTests : StressTests
    {
        private Dice dice;
        private Random random;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
            random = GetNewInstanceOf<Random>();
        }

        [Test]
        public void StressRollWithFewestDice()
        {
            stressor.Stress(() => AssertGetRoll((int l, int u) => RollHelper.GetRollWithFewestDice(l, u)));
        }

        [TestCase(false, false)]
        [TestCase(false, true)]
        [TestCase(true, false)]
        [TestCase(true, true)]
        public void StressRollWithMostEvenDistribution(bool multiplier, bool nonstadard)
        {
            stressor.Stress(() => AssertGetRoll((int l, int u) => RollHelper.GetRollWithMostEvenDistribution(l, u, multiplier, nonstadard)));
        }

        private void AssertGetRoll(Func<int, int, string> getRoll)
        {
            var upper = random.Next(Limits.Die) + 1;
            var lower = random.Next(upper);

            Assert.That(lower, Is.LessThan(upper));

            var roll = getRoll(lower, upper);

            Assert.That(dice.Roll(roll).IsValid(), Is.True, $"Min: {lower}; Max: {upper}; Roll: {roll}");
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower), $"Min: {lower}; Max: {upper}; Roll: {roll}");
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper), $"Min: {lower}; Max: {upper}; Roll: {roll}");
        }
    }
}
