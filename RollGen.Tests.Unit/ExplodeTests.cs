using Albatross.Expression;
using NUnit.Framework;
using RollGen.Expressions;
using RollGen.PartialRolls;
using System;
using System.Collections.Generic;

namespace RollGen.Tests.Unit
{
    // False random class that returns a given array of ints, throws if exceeds array size
    class MockRandom : Random
    {
        private readonly Queue<int> Rolls;

        public MockRandom(int[] rolls) => Rolls = new Queue<int>(rolls);

        public override int Next() => Rolls.Dequeue() - 1;
        public override int Next(int maxValue) => Next();
        public override int Next(int minValue, int maxValue) => Next();
    }

    [TestFixture]
    public class ExplodeTests
    {
        readonly ExpressionEvaluator evaluator = new AlbatrossExpressionEvaluator(Factory.Instance.Create());

        [TestCase(1, 1, new[]{1, 1}, ExpectedResult = 1)] // 1d1, shouldn't explode
        [TestCase(1, 6, new[]{1}, ExpectedResult = 1)] // Single, no Explode
        [TestCase(1, 6, new[]{6, 1}, ExpectedResult = 7)] // Single, Explode once
        [TestCase(1, 6, new[]{6, 6, 1}, ExpectedResult = 13)] // Single, Explode twice
        [TestCase(3, 6, new[]{3, 4, 2}, ExpectedResult = 9)] // Multiple, no Explode
        [TestCase(3, 6, new[]{1, 6, 2, 2}, ExpectedResult = 11)] // Multiple, Explode once
        [TestCase(3, 6, new[]{5, 6, 6, 1, 2}, ExpectedResult = 20)] // Multiple, Explode twice in a row
        [TestCase(3, 6, new[]{6, 1, 6, 4, 2}, ExpectedResult = 19)] // Multiple, Explode twice not in a row
        public int ExplodeTest(int quantity, int die, int[] rolls)
        {
            var random = new MockRandom(rolls);
            var partialRoll = new NumericPartialRoll(quantity, random, evaluator);
            partialRoll.d(die).Explode();

            return partialRoll.AsSum();
        }
    }
}