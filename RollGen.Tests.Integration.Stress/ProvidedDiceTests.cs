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

        public abstract void FullRangeHit(int quantity);

        protected void AssertFullRangeHit(int quantity)
        {
            var expectedCount = die * quantity - (quantity - 1);
            var rolls = Populate(new HashSet<int>(), () => GetRoll(quantity), expectedCount);

            Assert.That(rolls.Min(), Is.EqualTo(quantity));
            Assert.That(rolls.Max(), Is.EqualTo(die * quantity));
            Assert.That(rolls.Count, Is.EqualTo(expectedCount));
        }

        protected abstract int GetRoll(int quantity);
    }
}