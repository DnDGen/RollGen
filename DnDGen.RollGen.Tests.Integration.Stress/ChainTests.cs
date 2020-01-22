using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ChainTests : StressTests
    {
        [Inject]
        public Dice Dice { get; set; }
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void StressNumericChain()
        {
            stressor.Stress(AssertNumericChain);
        }

        [Test]
        public void StressExpressionChain()
        {
            stressor.Stress(AssertExpressionChain);
        }

        [Test]
        public void StressRollChain()
        {
            stressor.Stress(AssertRollChain);
        }

        protected void AssertNumericChain()
        {
            var q = GetRandomNumber();
            var d = GetRandomNumber();
            var k = GetRandomNumber();
            var p = GetRandomNumber();
            var m = GetRandomNumber();
            var t = GetRandomNumber();
            var db = GetRandomNumber();
            var md = GetRandomNumber();
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next();

            var roll = GetRoll(q, d, k, p, m, t, db, md);

            AssertRoll(roll, q.ToString(), d.ToString(), k.ToString(), p.ToString(), m.ToString(), t.ToString(), db.ToString(), md.ToString(), percentageThreshold, rollThreshold);
        }

        private int GetRandomNumber()
        {
            return Random.Next(2) + 1;
        }

        private void AssertRoll(PartialRoll roll, string q, string d, string k, string p, string m, string t, string db, string md, double percentageThreshold, int rollThreshold)
        {
            var min = Math.Min(ComputeMinimum(q), ComputeMinimum(k)) * 3
                + ComputeMinimum(p)
                - ComputeMinimum(m)
                * ComputeMinimum(t)
                / ComputeMinimum(db)
                % ComputeMinimum(md);
            var max = ComputeMaximum(k) * 4
                + ComputeMaximum(k) * 10
                + ComputeMaximum(k) * 100
                + ComputeMaximum(p)
                - ComputeMaximum(m)
                * ComputeMaximum(t)
                / ComputeMaximum(db)
                % ComputeMaximum(md);
            var explodeMax = ComputeMaximum(k) * 4
                + ComputeMaximum(k) * 10
                + ComputeMaximum(k) * 100 * 10
                + ComputeMaximum(p)
                - ComputeMaximum(m)
                * ComputeMaximum(t)
                / ComputeMaximum(db)
                % ComputeMaximum(md);

            Assert.That(roll.AsSum<double>(), Is.InRange(min, max * 10), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMinimum<double>(), Is.EqualTo(min), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum<double>(false), Is.EqualTo(max), roll.CurrentRollExpression);
            Assert.That(roll.AsPotentialMaximum<double>(), Is.EqualTo(explodeMax), roll.CurrentRollExpression);
            //HACK: We are ignoring the average, since it will probably result in decimal rolls during evaluation

            Assert.That(roll.AsTrueOrFalse(percentageThreshold), Is.True.Or.False, roll.CurrentRollExpression);
            Assert.That(roll.AsTrueOrFalse(rollThreshold), Is.True.Or.False, roll.CurrentRollExpression);

            var rolls = roll.AsIndividualRolls();

            Assert.That(rolls.Count(), Is.EqualTo(1), roll.CurrentRollExpression);
            Assert.That(rolls, Has.All.InRange(min, explodeMax), roll.CurrentRollExpression);
        }

        private double ComputeMinimum(string expression)
        {
            return Dice.Roll(expression).AsPotentialMinimum();
        }

        private double ComputeMaximum(string expression)
        {
            return Dice.Roll(expression).AsPotentialMaximum();
        }

        protected void AssertExpressionChain()
        {
            var q = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var d = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var k = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var p = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var m = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var t = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var db = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var md = $"{GetRandomNumber()}d{GetRandomNumber()}";
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next();

            var roll = GetRoll(q, d, k, p, m, t, db, md);

            AssertRoll(roll, q.ToString(), d.ToString(), k.ToString(), p.ToString(), m.ToString(), t.ToString(), db.ToString(), md.ToString(), percentageThreshold, rollThreshold);
        }

        protected void AssertRollChain()
        {
            var q = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var d = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var k = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var p = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var m = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var t = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var db = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var md = Dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var percentageThreshold = Random.NextDouble();
            var rollThreshold = Random.Next();

            var roll = GetRoll(q, d, k, p, m, t, db, md);

            AssertRoll(roll,
                q.CurrentRollExpression,
                d.CurrentRollExpression,
                k.CurrentRollExpression,
                p.CurrentRollExpression,
                m.CurrentRollExpression,
                t.CurrentRollExpression,
                db.CurrentRollExpression,
                md.CurrentRollExpression,
                percentageThreshold,
                rollThreshold);
        }

        private PartialRoll GetRoll(int q, int d, int k, double p, double m, double t, double db, double md) => Dice.Roll(q)
            .d(d)
            .d2()
            .d3()
            .d4()
            .Keeping(k)
            .Plus(q)
            .d6()
            .d8()
            .d10()
            .Keeping(k)
            .Plus(q)
            .d12()
            .d20()
            .Percentile()
            .Explode()
            .Keeping(k)
            .Plus(p)
            .Minus(m)
            .Times(t)
            .DividedBy(db)
            .Modulos(md);
        private PartialRoll GetRoll(string q, string d, string k, string p, string m, string t, string db, string md) => Dice.Roll(q)
            .d(d)
            .d2()
            .d3()
            .d4()
            .Keeping(k)
            .Plus(q)
            .d6()
            .d8()
            .d10()
            .Keeping(k)
            .Plus(q)
            .d12()
            .d20()
            .Percentile()
            .Explode()
            .Keeping(k)
            .Plus(p)
            .Minus(m)
            .Times(t)
            .DividedBy(db)
            .Modulos(md);
        private PartialRoll GetRoll(PartialRoll q, PartialRoll d, PartialRoll k, PartialRoll p, PartialRoll m, PartialRoll t, PartialRoll db, PartialRoll md) => Dice.Roll(q)
            .d(d)
            .d2()
            .d3()
            .d4()
            .Keeping(k)
            .Plus(q)
            .d6()
            .d8()
            .d10()
            .Keeping(k)
            .Plus(q)
            .d12()
            .d20()
            .Percentile()
            .Explode()
            .Keeping(k)
            .Plus(p)
            .Minus(m)
            .Times(t)
            .DividedBy(db)
            .Modulos(md);
    }
}
