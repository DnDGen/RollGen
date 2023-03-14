using NUnit.Framework;
using System;
using System.Diagnostics;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class RollHelperTests : StressTests
    {
        private Dice dice;
        private Random random;
        private Stopwatch stopwatch;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
            random = GetNewInstanceOf<Random>();
            stopwatch = new Stopwatch();
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
        public void StressRollWithMostEvenDistribution(bool multiplier, bool nonstandard)
        {
            stressor.Stress(() => AssertGetRoll((int l, int u) => RollHelper.GetRollWithMostEvenDistribution(l, u, multiplier, nonstandard)));
        }

        private void AssertGetRoll(Func<int, int, string> getRoll)
        {
            var upperLimit = random.Next(2) == 1 ? int.MaxValue : Limits.Die;
            var upper = random.Next(upperLimit) + 1;
            var lower = random.Next(upper);

            Assert.That(lower, Is.LessThan(upper));

            stopwatch.Start();
            var roll = getRoll(lower, upper);
            stopwatch.Stop();

            Assert.That(dice.Roll(roll).IsValid(), Is.True, $"Min: {lower}; Max: {upper}; Roll: {roll}");
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower), $"Min: {lower}; Max: {upper}; Roll: {roll}");
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper), $"Min: {lower}; Max: {upper}; Roll: {roll}");
            Assert.That(stopwatch.Elapsed, Is.LessThan(TimeSpan.FromSeconds(1)));
        }
    }
}
