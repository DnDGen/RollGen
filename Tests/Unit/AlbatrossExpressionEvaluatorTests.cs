using Albatross.Expression;
using Moq;
using NUnit.Framework;
using RollGen.Domain;

namespace RollGen.Test.Unit
{
    [TestFixture]
    public class AlbatrossExpressionEvaluatorTests
    {
        private ExpressionEvaluator expressionEvaluator;
        private Mock<IParser> mockParser;

        [SetUp]
        public void Setup()
        {
            mockParser = new Mock<IParser>();
            expressionEvaluator = new AlbatrossExpressionEvaluator(mockParser.Object);

            mockParser.Setup(p => p.Compile("expression").EvalValue(null)).Returns(9266);
        }

        [Test]
        public void EvaluateExpression()
        {
            var result = expressionEvaluator.Evaluate("expression");
            Assert.That(result, Is.EqualTo(9266));
        }

        [Test]
        public void EvaluateMultipleExpressions()
        {
            mockParser.Setup(p => p.Compile("other expression").EvalValue(null)).Returns(902.1);
            expressionEvaluator.Evaluate("expression");

            var result = expressionEvaluator.Evaluate("other expression");
            Assert.That(result, Is.EqualTo(902.1));
        }
    }
}
