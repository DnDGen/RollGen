using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public abstract class ProvidedDiceTests : DiceTests
    {
        protected abstract int maximum { get; }

        [Test]
        public void FullRangeHit()
        {
            var rolls = new HashSet<int>();
            while (LoopShouldKeepRunning() && rolls.Count < maximum)
                rolls.Add(GetRoll());

            Assert.That(rolls.Min(), Is.EqualTo(1));
            Assert.That(rolls.Max(), Is.EqualTo(maximum));
            Assert.That(rolls.Count, Is.EqualTo(maximum));
        }

        protected abstract int GetRoll();
    }
}