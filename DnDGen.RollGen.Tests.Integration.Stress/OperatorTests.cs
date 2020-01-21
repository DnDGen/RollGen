﻿using Ninject;
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
            Assert.That(roll.AsSum<double>(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMinimum<double>(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum<double>(false), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum<double>(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True, $"Percentage ({percentageThreshold}): {roll.CurrentRollExpression}");
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True, $"Roll ({rollThreshold}): {roll.CurrentRollExpression}");

            var rolls = roll.AsIndividualRolls<double>();

            Assert.That(rolls.Count(), Is.EqualTo(1), roll.CurrentRollExpression);
            Assert.That(rolls, Has.All.EqualTo(total), roll.CurrentRollExpression);
        }

        [Test]
        public void StressMinus()
        {
            stressor.Stress(AssertMinus);
        }

        private void AssertMinus()
        {
            var quantity = Random.Next(Limits.Quantity) + 1;
            var minus = Random.Next(quantity * 2) + Random.NextDouble();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Convert.ToInt32(quantity - minus) - 1;

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
            var thresholdLimit = Convert.ToInt32(Math.Floor(quantity / dividedBy));
            var rollThreshold = Random.Next(thresholdLimit) + 1;

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
            var rollThreshold = Random.Next(quantity % mod);

            var roll = Dice.Roll(quantity).Modulos(mod);

            AssertTotal(roll, quantity % mod, percentageThreshold, rollThreshold);
        }
    }
}
