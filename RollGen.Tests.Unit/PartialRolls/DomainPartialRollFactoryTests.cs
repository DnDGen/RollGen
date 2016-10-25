using Moq;
using NUnit.Framework;
using RollGen.Domain.Expressions;
using RollGen.Domain.PartialRolls;
using System;

namespace RollGen.Tests.Unit.PartialRolls
{
    [TestFixture]
    public class DomainPartialRollFactoryTests
    {
        private PartialRollFactory partialRollFactory;
        private Mock<Random> mockRandom;
        private Mock<ExpressionEvaluator> mockExpressionEvaluator;

        [SetUp]
        public void Setup()
        {
            mockRandom = new Mock<Random>();
            mockExpressionEvaluator = new Mock<ExpressionEvaluator>();
            partialRollFactory = new DomainPartialRollFactory(mockRandom.Object, mockExpressionEvaluator.Object);
        }

        [Test]
        public void ReturnNumericPartialRoll()
        {
            var partialRoll = partialRollFactory.Build(9266);
            Assert.That(partialRoll, Is.InstanceOf<NumericPartialRoll>());
        }

        [Test]
        public void ReturnExpressionPartialRoll()
        {
            var partialRoll = partialRollFactory.Build("roll expression");
            Assert.That(partialRoll, Is.InstanceOf<ExpressionPartialRoll>());
        }

        [Test]
        public void UseSameInstanceOfRandomForAllPartialRolls()
        {
            mockRandom.Setup(r => r.Next(9266)).Returns(90210);

            var firstPartialRoll = partialRollFactory.Build(42);
            var secondPartialRoll = partialRollFactory.Build(42);

            var firstRoll = firstPartialRoll.d(9266).AsSum();
            var secondRoll = secondPartialRoll.d(9266).AsSum();

            Assert.That(secondRoll, Is.EqualTo(firstRoll));
            Assert.That(firstRoll, Is.EqualTo(3788862));
        }

        [Test]
        public void UseSameInstanceOfRandomForAllPartialRollsBasedOnExpression()
        {
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>("roll expression")).Returns(9266);

            var firstPartialRoll = partialRollFactory.Build("roll expression");
            var secondPartialRoll = partialRollFactory.Build("roll expression");

            var firstRoll = firstPartialRoll.AsSum();
            var secondRoll = secondPartialRoll.AsSum();

            Assert.That(secondRoll, Is.EqualTo(firstRoll));
            Assert.That(firstRoll, Is.EqualTo(9266));
        }
    }
}
