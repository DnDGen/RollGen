using Ninject;
using NUnit.Framework;
using System;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class OperatorTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void StressPlus()
        {
            stressor.Stress(AssertPlus);
        }

        private void AssertPlus()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;
            var plus = Random.Next(int.MaxValue - quantity * die);

            var roll = Dice.Roll(quantity).d(die).Plus(plus);
            var min = quantity + plus;
            var max = quantity * die + plus;

            Assert.That(roll.AsSum(), Is.InRange(min, max), roll.ToString());
        }

        [Test]
        public void StressMinus()
        {
            stressor.Stress(AssertMinus);
        }

        private void AssertMinus()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;
            var minus = Random.Next();

            var roll = Dice.Roll(quantity).d(die).Minus(minus).AsSum();
            var min = quantity - minus;
            var max = quantity * die - minus;

            Assert.That(roll, Is.InRange(min, max));
        }

        [Test]
        public void StressTimes()
        {
            stressor.Stress(AssertTimes);
        }

        private void AssertTimes()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;
            var times = Random.Next(int.MaxValue / quantity / die);

            var roll = Dice.Roll(quantity).d(die).Times(times);
            var min = quantity * times;
            var max = quantity * die * times;

            Assert.That(roll.AsSum(), Is.InRange(min, max), roll.ToString());
        }

        [Test]
        public void StressDividedBy()
        {
            stressor.Stress(AssertDividedBy);
        }

        private void AssertDividedBy()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;
            var divisor = Random.Next();

            var roll = Dice.Roll(quantity).d(die).DividedBy(divisor);
            var min = quantity / divisor;
            var max = quantity * die / divisor;

            Assert.That(roll.AsSum(), Is.InRange(min, max), roll.ToString());
        }

        [Test]
        public void StressModulos()
        {
            stressor.Stress(AssertModulos);
        }

        private void AssertModulos()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var die = Random.Next(Limits.Die) + 1;
            var mod = Random.Next();

            var roll = Dice.Roll(quantity).d(die).Modulos(mod).AsSum();
            Assert.That(roll, Is.InRange(0, mod - 1));
        }
    }
}
