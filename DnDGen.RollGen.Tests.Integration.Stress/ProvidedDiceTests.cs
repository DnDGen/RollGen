using Ninject;
using NUnit.Framework;
using System;

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

        protected abstract int GetRoll(int quantity);

        protected void AssertRoll()
        {
            var quantity = Random.Next(1000) + 1;
            var roll = GetRoll(quantity);
            Assert.That(roll, Is.InRange(quantity, die * quantity));
        }
    }
}