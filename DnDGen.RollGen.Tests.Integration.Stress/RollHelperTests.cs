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

        [Test]
        public void StressRollWithMostEvenDistribution()
        {
            stressor.Stress(() => AssertGetRoll((int l, int u) => RollHelper.GetRollWithMostEvenDistribution(l, u)));
        }

        private void AssertGetRoll(Func<int, int, string> getRoll)
        {
            var upper = random.Next(Limits.Die) + 1;
            var lower = random.Next(upper);

            Assert.That(lower, Is.LessThan(upper));

            var roll = getRoll(lower, upper);

            Assert.That(roll, Is.Not.Empty.And.Matches("[0-9]d(100|20|12|10|8|6|4|3|2)"), roll);
            Assert.That(dice.Roll(roll).IsValid(), Is.True, roll);
            Assert.That(dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower), roll);
            Assert.That(dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper), roll);
        }
    }
}
