using Albatross.Expression;
using Moq;
using NUnit.Framework;
using RollGen.Domain.Expressions;

namespace RollGen.Tests.Unit.Expressions
{
    [TestFixture]
    public class AlbatrossExpressionEvaluatorTests
    {
        private const string Expression = "expression";

        private ExpressionEvaluator expressionEvaluator;
        private Mock<IParser> mockParser;

        [SetUp]
        public void Setup()
        {
            mockParser = new Mock<IParser>();
            expressionEvaluator = new AlbatrossExpressionEvaluator(mockParser.Object);

            mockParser.Setup(p => p.Compile(Expression).EvalValue(null)).Returns(9266);
        }

        [Test]
        public void EvaluateExpression()
        {
            var result = expressionEvaluator.Evaluate<int>(Expression);
            Assert.That(result, Is.EqualTo(9266));
        }

        [Test]
        public void EvaluateMultipleExpressions()
        {
            var result = expressionEvaluator.Evaluate<int>(Expression);
            Assert.That(result, Is.EqualTo(9266));

            mockParser.Setup(p => p.Compile("other expression").EvalValue(null)).Returns(902.1);
            result = expressionEvaluator.Evaluate<int>("other expression");
            Assert.That(result, Is.EqualTo(902));
        }

        [Test]
        public void EvaluateDouble()
        {
            mockParser.Setup(p => p.Compile("other expression").EvalValue(null)).Returns(902.1);
            var result = expressionEvaluator.Evaluate<double>("other expression");
            Assert.That(result, Is.EqualTo(902.1));
        }

        [Test]
        public void IfDieRollIsInExpression_ThrowArgumentException()
        {
            Assert.That(() => expressionEvaluator.Evaluate<int>("expression with 3 d 4+2"), Throws.ArgumentException.With.Message.EqualTo("Cannot evaluate unrolled die roll 3 d 4"));
        }

        [Test]
        public void EvaluateTrueBooleanExpression()
        {
            mockParser.Setup(p => p.Compile("boolean expression").EvalValue(null)).Returns(bool.TrueString);
            var result = expressionEvaluator.Evaluate<bool>("boolean expression");
            Assert.That(result, Is.True);
        }

        [Test]
        public void EvaluateFalseBooleanExpression()
        {
            mockParser.Setup(p => p.Compile("boolean expression").EvalValue(null)).Returns(bool.FalseString);
            var result = expressionEvaluator.Evaluate<bool>("boolean expression");
            Assert.That(result, Is.False);
        }
    }
}
