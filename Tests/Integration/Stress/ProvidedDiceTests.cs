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

        protected abstract int die { get; }

        public abstract void FullRangeHit();

        protected void AssertFullRangeHit()
        {
            var rolls = new HashSet<int>();
            GenerateOrFail(() => rolls.Add(GetRoll()), () => rolls.Count == die);

            Assert.That(rolls.Min(), Is.EqualTo(1));
            Assert.That(rolls.Max(), Is.EqualTo(die));
            Assert.That(rolls.Count, Is.EqualTo(die));
        }

        protected abstract int GetRoll();
    }
}