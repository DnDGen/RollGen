using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class ChainTests : StressTests
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
            var tr1 = GetRandomNumber(10);
            var tr2 = GetRandomNumber(10);
            var db = GetRandomNumber();
            var md = GetRandomNumber();
            var percentageThreshold = random.NextDouble();
            var rollThreshold = random.Next();

            var roll = GetRoll(q, d, k, p, m, t, db, md, tr1, tr2);

            AssertRoll(
                roll,
                q.ToString(),
                d.ToString(),
                k.ToString(),
                p.ToString(),
                m.ToString(),
                t.ToString(),
                db.ToString(),
                md.ToString(),
                percentageThreshold,
                rollThreshold,
                tr1.ToString(),
                tr2.ToString());
        }

        private int GetRandomNumber(int top = 2)
        {
            return random.Next(top) + 1;
        }

        private void AssertRoll(
            PartialRoll roll,
            string q,
            string d,
            string k,
            string p,
            string m,
            string t,
            string db,
            string md,
            double percentageThreshold,
            int rollThreshold,
            string tr1,
            string tr2)
        {
            var rollMin = GetRollMin(tr1, tr2);
            var min = Math.Min(ComputeMinimum(q), ComputeMinimum(k)) * 2
                + Math.Min(ComputeMinimum(q), ComputeMinimum(k)) * rollMin
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

            Assert.That(roll.IsValid(), Is.True, roll.CurrentRollExpression);
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

        private int GetRollMin(params string[] transforms)
        {
            var minimums = transforms.Select(ComputeMinimum);

            var minimum = 1;
            while (minimums.Contains(minimum))
                minimum++;

            return minimum;
        }

        private double ComputeMinimum(string expression)
        {
            return dice.Roll(expression).AsPotentialMinimum();
        }

        private double ComputeMaximum(string expression)
        {
            return dice.Roll(expression).AsPotentialMaximum();
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
            var tr1 = $"{GetRandomNumber()}d{GetRandomNumber(10)}";
            var tr2 = $"{GetRandomNumber()}d{GetRandomNumber(10)}";
            var percentageThreshold = random.NextDouble();
            var rollThreshold = random.Next();

            var roll = GetRoll(q, d, k, p, m, t, db, md, tr1, tr2);

            AssertRoll(
                roll,
                q.ToString(),
                d.ToString(),
                k.ToString(),
                p.ToString(),
                m.ToString(),
                t.ToString(),
                db.ToString(),
                md.ToString(),
                percentageThreshold,
                rollThreshold,
                tr1,
                tr2);
        }

        protected void AssertRollChain()
        {
            var q = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var d = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var k = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var p = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var m = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var t = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var db = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var md = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber()}");
            var tr1 = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber(10)}");
            var tr2 = dice.Roll($"{GetRandomNumber()}d{GetRandomNumber(10)}");
            var percentageThreshold = random.NextDouble();
            var rollThreshold = random.Next();

            var roll = GetRoll(q, d, k, p, m, t, db, md, tr1, tr2);

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
                rollThreshold,
                tr1.CurrentRollExpression,
                tr2.CurrentRollExpression);
        }

        private PartialRoll GetRoll(int q, int d, int k, double p, double m, double t, double db, double md, int tr1, int tr2) => dice.Roll(q)
            .d(d)
            .d2()
            .d3()
            .d4()
            .Keeping(k) //HACK: Keeping quantity from getting too high
            .Plus(q)
            .d6()
            .d8()
            .d10()
            .Keeping(k) //HACK: Keeping quantity from getting too high
            .Plus(q)
            .d12()
            .d20()
            .Percentile()
            .Explode()
            .Transforming(tr1)
            .Transforming(tr2)
            .Keeping(k)
            .Plus(p)
            .Minus(m)
            .Times(t)
            .DividedBy(db)
            .Modulos(md);
        private PartialRoll GetRoll(string q, string d, string k, string p, string m, string t, string db, string md, string tr1, string tr2) => dice.Roll(q)
            .d(d)
            .d2()
            .d3()
            .d4()
            .Keeping(k) //HACK: Keeping quantity from getting too high
            .Plus(q)
            .d6()
            .d8()
            .d10()
            .Keeping(k) //HACK: Keeping quantity from getting too high
            .Plus(q)
            .d12()
            .d20()
            .Percentile()
            .Explode()
            .Transforming(tr1)
            .Transforming(tr2)
            .Keeping(k)
            .Plus(p)
            .Minus(m)
            .Times(t)
            .DividedBy(db)
            .Modulos(md);
        private PartialRoll GetRoll(
                PartialRoll q,
                PartialRoll d,
                PartialRoll k,
                PartialRoll p,
                PartialRoll m,
                PartialRoll t,
                PartialRoll db,
                PartialRoll md,
                PartialRoll tr1,
                PartialRoll tr2) => dice.Roll(q)
            .d(d)
            .d2()
            .d3()
            .d4()
            .Keeping(k) //HACK: Keeping quantity from getting too high
            .Plus(q)
            .d6()
            .d8()
            .d10()
            .Keeping(k) //HACK: Keeping quantity from getting too high
            .Plus(q)
            .d12()
            .d20()
            .Percentile()
            .Explode()
            .Transforming(tr1)
            .Transforming(tr2)
            .Keeping(k)
            .Plus(p)
            .Minus(m)
            .Times(t)
            .DividedBy(db)
            .Modulos(md);
    }
}
