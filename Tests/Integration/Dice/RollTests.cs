using System;
using System.Collections.Generic;
using System.Linq;
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
            var expectedTotal = max - min + 1;
            var rolls = new HashSet<Int32>();

            while (LoopShouldStillRun() && rolls.Count < expectedTotal)
                rolls.Add(Dice.Roll(roll));

            Assert.That(rolls.Count, Is.EqualTo(expectedTotal));
            Assert.Pass("Iterations: {0}", iterations);
        }
    }
}