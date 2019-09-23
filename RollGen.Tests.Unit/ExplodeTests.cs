using Albatross.Expression;
using Moq;
using NUnit.Framework;
using RollGen.Expressions;
using RollGen.PartialRolls;
using System;

namespace RollGen.Tests.Unit
{
    [TestFixture]
    public class ExplodeTests
    {
        readonly ExpressionEvaluator evaluator = new AlbatrossExpressionEvaluator(Factory.Instance.Create());
        readonly Mock<Random> random = new Mock<Random>();

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
            var seq = random.SetupSequence(r => r.Next(die));
            foreach (var roll in rolls)
            {
                seq.Returns(roll-1);
            }

            var partialRoll = new NumericPartialRoll(quantity, random.Object, evaluator);
            partialRoll.d(die).Explode();

            return partialRoll.AsSum();
        }
    }
}