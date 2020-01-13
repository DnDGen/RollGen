using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class ProvidedDiceTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        protected abstract int die { get; }

        protected abstract PartialRoll GetRoll(int quantity);

        protected void AssertRollAsSum()
        {
            var quantity = Random.Next(1000) + 1;
            var roll = GetRoll(quantity);
            Assert.That(roll.AsSum(), Is.InRange(quantity, die * quantity));
        }

        protected void AssertRollAsIndividualRolls()
        {
            var quantity = Random.Next(1000) + 1;
            var roll = GetRoll(quantity);
            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(quantity));
            Assert.That(rolls, Has.All.InRange(1, die));
        }

        protected void AssertRollAsAverage()
        {
            var quantity = Random.Next(1000) + 1;
            var roll = GetRoll(quantity);
            var average = quantity * (die + 1) / 2.0d;
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(average));
        }

        protected void AssertRollAsMinimum()
        {
            var quantity = Random.Next(1000) + 1;
            var roll = GetRoll(quantity);
            Assert.That(roll.AsPotentialMinimum(), Is.EqualTo(quantity));
        }

        protected void AssertRollAsMaximum()
        {
            var quantity = Random.Next(1000) + 1;
            var roll = GetRoll(quantity);
            Assert.That(roll.AsPotentialMaximum(), Is.EqualTo(die * quantity));
        }
    }
}