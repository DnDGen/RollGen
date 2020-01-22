using Ninject;
using NUnit.Framework;
using System;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class RollHelperTests : StressTests
    {
        [Inject]
        public Random Random { get; set; }
        [Inject]
        public Dice Dice { get; set; }

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
            var upper = Random.Next(Limits.Die) + 1;
            var lower = Random.Next(upper);

            Assert.That(lower, Is.LessThan(upper));

            var roll = getRoll(lower, upper);

            Assert.That(roll, Is.Not.Empty.And.Matches("[0-9]d(100|20|12|10|8|6|4|3|2)"), roll);
            Assert.That(Dice.Roll(roll).AsPotentialMinimum(), Is.EqualTo(lower), roll);
            Assert.That(Dice.Roll(roll).AsPotentialMaximum(), Is.EqualTo(upper), roll);
        }
    }
}
