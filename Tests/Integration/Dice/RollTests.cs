using System;
using System.Collections.Generic;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Dice
{
    [TestFixture]
    public class RollTests : DiceTests
    {
        [Inject]
        public IDice Dice { get; set; }

        [TestCase("1d6", 1, 6)]
        [TestCase("2d6", 2, 12)]
        [TestCase("1d5", 1, 5)]
        [TestCase("2d5", 2, 10)]
        [TestCase("4d4*1000", 4000, 16000)]
        [TestCase("1d4-1", 0, 3)]
        [TestCase("1d4+1", 2, 5)]
        [TestCase("1d4+1d6", 2, 10)]
        [TestCase("1d100/2", 0, 50)]
        [TestCase("1d10*2-1d10/2+12-10", -1, 22)]
        [TestCase("1d10 * 2 - 1d10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1 d 10 * 2 - 1d10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1 d 10 * 2 - 1 d 10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1d2+1d2", 2, 4)]
        [TestCase("1-2+3d4", 2, 11)]
        [TestCase("1+2d3*4-5", 4, 20)]
        public void InRange(String roll, Int32 min, Int32 max)
        {
            while (LoopShouldStillRun())
            {
                var result = Dice.Roll(roll);
                Assert.That(result, Is.InRange<Int32>(min, max));
            }

            Assert.Pass("Iterations: {0}", iterations);
        }

        [TestCase("1d6", 1, 6)]
        [TestCase("2d6", 2, 12)]
        [TestCase("1d5", 1, 5)]
        [TestCase("2d5", 2, 10)]
        [TestCase("4d4*1000", 4000, 16000)]
        [TestCase("1d4-1", 0, 3)]
        [TestCase("1d4+1", 2, 5)]
        [TestCase("1d4+1d6", 2, 10)]
        [TestCase("1d100/2", 0, 50)]
        [TestCase("1d10*2-1d10/2+12-10", -1, 22)]
        [TestCase("1d10 * 2 - 1d10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1 d 10 * 2 - 1d10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1 d 10 * 2 - 1 d 10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1d2+1d2", 2, 4)]
        [TestCase("1-2+3d4", 2, 11)]
        [TestCase("1+2d3*4-5", 4, 20)]
        public void HitsMinAndMax(String roll, Int32 min, Int32 max)
        {
            var hitMin = false;
            var hitMax = false;

            while (LoopShouldStillRun())
            {
                var result = Dice.Roll(roll);

                hitMin |= result == min;
                hitMax |= result == max;
            }

            Assert.That(hitMin, Is.True, "Did not hit minimum");
            Assert.That(hitMax, Is.True, "Did not hit maximum");

            Assert.Pass("Iterations: {0}", iterations);
        }

        [TestCase("1d6", 1, 6)]
        [TestCase("2d6", 2, 12)]
        [TestCase("1d5", 1, 5)]
        [TestCase("2d5", 2, 10)]
        [TestCase("1d4-1", 0, 3)]
        [TestCase("1d4+1", 2, 5)]
        [TestCase("1d4+1d6", 2, 10)]
        [TestCase("1d100/2", 0, 50)]
        [TestCase("1d10*2-1d10/2+12-10", -1, 22)]
        [TestCase("1d10 * 2 - 1d10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1 d 10 * 2 - 1d10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1 d 10 * 2 - 1 d 10 / 2 + 12 - 10", -1, 22)]
        [TestCase("1d2+1d2", 2, 4)]
        [TestCase("1-2+3d4", 2, 11)]
        public void FullRangeHit(String roll, Int32 min, Int32 max)
        {
            var rolls = new HashSet<Int32>();
            while (LoopShouldStillRun())
                rolls.Add(Dice.Roll(roll));

            Assert.That(rolls.Count, Is.EqualTo(max - min + 1));
            Assert.Pass("Iterations: {0}", iterations);
        }
    }
}