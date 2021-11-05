using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class OperatorTests : StressTests
    {
        private Dice dice;
        private Random random;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
            random = GetNewInstanceOf<Random>();
        }

        [Test]
        public void StressPlus()
        {
            stressor.Stress(AssertPlus);
        }

        private void AssertPlus()
        {
            var quantity = random.Next(Limits.Quantity) + 1;
            var plus = random.Next(int.MaxValue - quantity) + random.NextDouble();

            var roll = dice.Roll(quantity).Plus(plus);

            AssertTotal(roll, quantity + plus);
        }

        private void AssertTotal(PartialRoll roll, double total)
        {
            Assert.That(roll.AsSum<double>(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMinimum<double>(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum<double>(false), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum<double>(), Is.EqualTo(total), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialAverage(), Is.EqualTo(total), roll.CurrentRollExpression);

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
            var quantity = random.Next(Limits.Quantity) + 1;
            var minus = random.Next(quantity * 2) + random.NextDouble();

            var roll = dice.Roll(quantity).Minus(minus);

            AssertTotal(roll, quantity - minus);
        }

        [Test]
        public void StressTimes()
        {
            stressor.Stress(AssertTimes);
        }

        private void AssertTimes()
        {
            var quantity = random.Next(Limits.Quantity) + 1;
            var times = random.Next(int.MaxValue / quantity) + 1 + random.NextDouble();

            var roll = dice.Roll(quantity).Times(times);

            AssertTotal(roll, quantity * times);
        }

        [Test]
        public void StressDividedBy()
        {
            stressor.Stress(AssertDividedBy);
        }

        private void AssertDividedBy()
        {
            var quantity = random.Next(Limits.Quantity) + 1;
            //HACK: Making sure divisor is >= 1, because when it is less, sometimes the division expression ends up in something Albatross doesn't like,
            //such as '3/9.12030227907016E-05' (it doesn't like the scientific notation)
            var dividedBy = random.Next(Limits.Quantity) + 1 + random.NextDouble();

            var roll = dice.Roll(quantity).DividedBy(dividedBy);

            AssertTotal(roll, quantity / dividedBy);
        }

        [Test]
        public void StressModulos()
        {
            stressor.Stress(AssertModulos);
        }

        private void AssertModulos()
        {
            var quantity = random.Next(Limits.Quantity) + 1;
            var mod = random.Next();

            var roll = dice.Roll(quantity).Modulos(mod);

            AssertTotal(roll, quantity % mod);
        }
    }
}
