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
        public void InRange()
        {
            while (LoopShouldStillRun())
            {
                var result = GetRoll();
                Assert.That(result, Is.InRange<Int32>(Minimum, maximum));
            }

            Assert.Pass("Iterations: {0}", iterations);
        }

        protected abstract Int32 GetRoll();

        [Test]
        public void HitsMinAndMax()
        {
            var hitMin = false;
            var hitMax = false;

            while (LoopShouldStillRun())
            {
                var result = GetRoll();

                hitMin |= result == Minimum;
                hitMax |= result == maximum;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");

            Assert.Pass("Iterations: {0}", iterations);
        }

        [Test]
        public void FullRangeHit()
        {
            var rolls = new HashSet<Int32>();
            while (LoopShouldStillRun())
                rolls.Add(GetRoll());

            Assert.That(rolls.Count, Is.EqualTo(maximum));
            Assert.Pass("Iterations: {0}", iterations);
        }
    }
}