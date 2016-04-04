using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class ProvidedDiceTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }

        protected abstract int maximum { get; }

        public abstract void FullRangeHit();

        protected void AssertFullRangeHit()
        {
            var rolls = new HashSet<int>();
            while (TestShouldKeepRunning() && rolls.Count < maximum)
                rolls.Add(GetRoll());

            Assert.That(rolls.Min(), Is.EqualTo(1));
            Assert.That(rolls.Max(), Is.EqualTo(maximum));
            Assert.That(rolls.Count, Is.EqualTo(maximum));
        }

        protected abstract int GetRoll();
    }
}