using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

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
            var plus = Random.Next(int.MaxValue - quantity) + Random.NextDouble();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(quantity) + 1;

            var roll = Dice.Roll(quantity).Plus(plus);

            AssertTotal(roll, quantity + plus, percentageThreshold, rollThreshold);
        }

        private void AssertTotal(PartialRoll roll, double total, double percentageThreshold, int rollThreshold)
        {
            Assert.That(roll.AsSum<double>(), Is.EqualTo(total), roll.ToString());
            Assert.That(roll.AsPotentialMinimum<double>(), Is.EqualTo(total));
            Assert.That(roll.AsPotentialMaximum<double>(false), Is.EqualTo(total));
            Assert.That(roll.AsPotentialMaximum<double>(), Is.EqualTo(total * 10));
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(total));
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.False, "Percentage");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True, "Roll");

            var rolls = roll.AsIndividualRolls<double>();

            Assert.That(rolls.Count(), Is.EqualTo(1));
            Assert.That(rolls, Has.All.EqualTo(total));
        }

        [Test]
        public void StressMinus()
        {
            stressor.Stress(AssertMinus);
        }

        private void AssertMinus()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var minus = Random.Next(1_000_000_000) + Random.NextDouble();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(1) + 1;

            var roll = Dice.Roll(quantity).Minus(minus);

            AssertTotal(roll, quantity - minus, percentageThreshold, rollThreshold);
        }

        [Test]
        public void StressTimes()
        {
            stressor.Stress(AssertTimes);
        }

        private void AssertTimes()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var times = Random.Next(int.MaxValue / quantity) + Random.NextDouble();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(quantity) + 1;

            var roll = Dice.Roll(quantity).Times(times);

            AssertTotal(roll, quantity * times, percentageThreshold, rollThreshold);
        }

        [Test]
        public void StressDividedBy()
        {
            stressor.Stress(AssertDividedBy);
        }

        private void AssertDividedBy()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var dividedBy = Random.Next(quantity) + Random.NextDouble();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(quantity) + 1;

            var roll = Dice.Roll(quantity).DividedBy(dividedBy);

            AssertTotal(roll, quantity / dividedBy, percentageThreshold, rollThreshold);
        }

        [Test]
        public void StressModulos()
        {
            stressor.Stress(AssertModulos);
        }

        private void AssertModulos()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var mod = Random.Next();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next(mod);

            var roll = Dice.Roll(quantity).Modulos(mod);

            AssertTotal(roll, quantity % mod, percentageThreshold, rollThreshold);
        }
    }
}
