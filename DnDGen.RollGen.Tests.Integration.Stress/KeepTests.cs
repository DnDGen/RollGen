using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class KeepTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void StressKeep()
        {
            stressor.Stress(AssertKeep);
        }

        private void AssertKeep()
        {
            var quantity = Random.Next(QuantityLimit) + 1;
            var die = Random.Next(DieLimit) + 1;
            var keep = Random.Next(quantity - 1) + 1;

            var rolls = Dice.Roll(quantity).d(die).Keeping(keep).AsIndividualRolls();

            Assert.That(keep, Is.LessThan(quantity));
            Assert.That(rolls.Count(), Is.EqualTo(keep));
            Assert.That(rolls, Has.All.InRange(1, die));
        }
    }
}
