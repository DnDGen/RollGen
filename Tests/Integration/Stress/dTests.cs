using Ninject;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class dTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }

        [TestCase(9266)]
        [TestCase(42)]
        [TestCase(7)]
        public void FullRangeHit(int die)
        {
            var rolls = new HashSet<int>();
            GenerateOrFail(() => rolls.Add(Dice.Roll().d(die)), () => rolls.Count == die);

            Assert.That(rolls.Min(), Is.EqualTo(1));
            Assert.That(rolls.Max(), Is.EqualTo(die));
            Assert.That(rolls.Count, Is.EqualTo(die));
        }
    }
}