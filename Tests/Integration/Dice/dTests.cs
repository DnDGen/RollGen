using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Rolls
{
    [TestFixture]
    public class dTests : DiceTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [TestCase(9266)]
        [TestCase(42)]
        [TestCase(7)]
        public void FullRangeHit(int maximum)
        {
            var rolls = new HashSet<int>();
            while (LoopShouldStillRun() && rolls.Count < maximum)
                rolls.Add(Dice.Roll().d(maximum));

            Assert.That(rolls.Min(), Is.EqualTo(1));
            Assert.That(rolls.Max(), Is.EqualTo(maximum));
            Assert.That(rolls.Count, Is.EqualTo(maximum));
        }
    }
}