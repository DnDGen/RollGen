using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
{
    [TestFixture]
    public abstract class ProvidedDiceTests : DiceTests
    {
        protected abstract Int32 maximum { get; }

        private const Int32 Minimum = 1;

        [Test]
        public void FullRangeHit()
        {
            var rolls = new HashSet<Int32>();
            while (LoopShouldStillRun() && rolls.Count < maximum)
                rolls.Add(GetRoll());

            Assert.That(rolls.Count, Is.EqualTo(maximum));
            Assert.Pass("Iterations: {0}", iterations);
        }

        protected abstract Int32 GetRoll();
    }
}