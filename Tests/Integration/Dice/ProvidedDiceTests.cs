using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Dice
{
    [TestFixture]
    public abstract class ProvidedDiceTests : DiceTests
    {
        protected abstract Int32 maximum { get; }

        [Test]
        public void FullRangeHit()
        {
            var rolls = new HashSet<Int32>();
            while (LoopShouldStillRun() && rolls.Count < maximum)
                rolls.Add(GetRoll());

            Assert.That(rolls.Min(), Is.EqualTo(1));
            Assert.That(rolls.Max(), Is.EqualTo(maximum));
            Assert.That(rolls.Count, Is.EqualTo(maximum));
            Assert.Pass("Iterations: {0}", iterations);
        }

        protected abstract Int32 GetRoll();
    }
}