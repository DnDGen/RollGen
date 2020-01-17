using DnDGen.RollGen.Expressions;
using DnDGen.RollGen.PartialRolls;
using Moq;
using NUnit.Framework;
using System;

namespace DnDGen.RollGen.Tests.Unit.PartialRolls
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

            var count = 0;
            mockRandom.Setup(r => r.Next(It.IsAny<int>())).Returns((int max) => count++ % max);
            mockExpressionEvaluator.Setup(e => e.Evaluate<int>(It.IsAny<string>())).Returns((string s) => DefaultIntValue(s));
        }

        private int DefaultIntValue(string source)
        {
            if (int.TryParse(source, out var output))
                return output;

            throw new ArgumentException($"{source} was not configured to be evaluated");
        }

        [Test]
        public void ReturnPartialRollFromNumericQuantity()
        {
            var partialRoll = partialRollFactory.Build(9266);
            Assert.That(partialRoll, Is.InstanceOf<DomainPartialRoll>());
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("9266"));
        }

        [Test]
        public void ReturnPartialRollFromQuantityExpression()
        {
            var partialRoll = partialRollFactory.Build("roll expression");
            Assert.That(partialRoll, Is.InstanceOf<DomainPartialRoll>());
            Assert.That(partialRoll.CurrentRollExpression, Is.EqualTo("(roll expression)"));
        }

        [Test]
        public void UseSameInstanceOfRandomForAllPartialRolls()
        {
            mockRandom.SetupSequence(r => r.Next(9266))
                .Returns(1337)
                .Returns(600);

            var firstPartialRoll = partialRollFactory.Build(1);
            var secondPartialRoll = partialRollFactory.Build(1);

            var firstRoll = firstPartialRoll.d(9266).AsSum();
            var secondRoll = secondPartialRoll.d(9266).AsSum();

            Assert.That(firstRoll, Is.EqualTo(1338));
            Assert.That(secondRoll, Is.EqualTo(601));
        }

        [Test]
        public void UseSameInstanceOfRandomForAllPartialRollsBasedOnExpression()
        {
            mockRandom.SetupSequence(r => r.Next(1336))
                .Returns(1337)
                .Returns(600);

            var firstPartialRoll = partialRollFactory.Build("1d1336");
            var secondPartialRoll = partialRollFactory.Build("1d1336");

            var firstRoll = firstPartialRoll.AsSum();
            var secondRoll = secondPartialRoll.AsSum();

            Assert.That(firstRoll, Is.EqualTo(1338));
            Assert.That(secondRoll, Is.EqualTo(601));
        }
    }
}
