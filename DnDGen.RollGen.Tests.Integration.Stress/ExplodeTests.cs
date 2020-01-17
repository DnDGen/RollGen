using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ExplodeTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void StressExplode()
        {
            stressor.Stress(AssertExplode);
        }

        private void AssertExplode()
        {
            var quantity = Random.Next(QuantityLimit) + 1;
            var die = Random.Next(DieLimit) + 2; //INFO: Can't allow d1, as explode fails on that

            var rolls = Dice.Roll(quantity).d(die).Explode().AsIndividualRolls();
            var maxCount = rolls.Count(r => r == die);

            Assert.That(rolls.Count(), Is.AtLeast(quantity)
                .And.EqualTo(quantity + maxCount));
            Assert.That(rolls, Has.All.InRange(1, die));
        }
    }
}
