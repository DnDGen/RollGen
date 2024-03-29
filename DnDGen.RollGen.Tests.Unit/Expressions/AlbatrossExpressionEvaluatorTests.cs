﻿using Albatross.Expression;
using Albatross.Expression.Tokens;
using DnDGen.RollGen.Expressions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DnDGen.RollGen.Tests.Unit.Expressions
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

            SetUpExpression(Expression, 9266);
        }

        private void SetUpExpression(string expression, object result)
        {
            var mockToken = new Mock<IToken>();
            var queue = new Queue<IToken>();
            var stack = new Stack<IToken>();

            mockParser.Setup(p => p.Tokenize(expression)).Returns(queue);
            mockParser.Setup(p => p.BuildStack(queue)).Returns(stack);
            mockParser.Setup(p => p.CreateTree(stack)).Returns(mockToken.Object);

            mockToken.Setup(t => t.EvalValue(null)).Returns(result);
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

            SetUpExpression("other expression", 902.1);

            result = expressionEvaluator.Evaluate<int>("other expression");
            Assert.That(result, Is.EqualTo(902));
        }

        [Test]
        public void EvaluateDouble()
        {
            SetUpExpression("other expression", 902.1);

            var result = expressionEvaluator.Evaluate<double>("other expression");
            Assert.That(result, Is.EqualTo(902.1));
        }

        [Test]
        public void IfDieRollIsInExpression_ThrowArgumentException()
        {
            Assert.That(() => expressionEvaluator.Evaluate<int>("expression with 3 d 4+2"),
                Throws.ArgumentException.With.Message.EqualTo("Cannot evaluate unrolled die roll 3 d 4"));
        }

        [Test]
        public void EvaluateTrueBooleanExpression()
        {
            SetUpExpression("boolean expression", bool.TrueString);

            var result = expressionEvaluator.Evaluate<bool>("boolean expression");
            Assert.That(result, Is.True);
        }

        [Test]
        public void EvaluateFalseBooleanExpression()
        {
            SetUpExpression("boolean expression", bool.FalseString);

            var result = expressionEvaluator.Evaluate<bool>("boolean expression");
            Assert.That(result, Is.False);
        }

        [Test]
        public void EvaluateExpressionThrowsException_WhenAlbatrossFailsToEvaluate()
        {
            var expression = "wrong expression";

            var mockToken = new Mock<IToken>();
            var queue = new Queue<IToken>();
            var stack = new Stack<IToken>();

            mockParser.Setup(p => p.Tokenize(expression)).Returns(queue);
            mockParser.Setup(p => p.BuildStack(queue)).Returns(stack);
            mockParser.Setup(p => p.CreateTree(stack)).Returns(mockToken.Object);

            var exception = new Exception("I failed");
            mockToken.Setup(t => t.EvalValue(null)).Throws(exception);

            Assert.That(() => expressionEvaluator.Evaluate<int>(expression),
                Throws.InvalidOperationException
                .With.Message.EqualTo($"Expression 'wrong expression' is invalid")
                .And.InnerException.EqualTo(exception));
        }

        [Test]
        public void IsValid_ReturnsTrue()
        {
            //mockParser.Setup(p => p.IsValidExpression(Expression)).Returns(true);

            var result = expressionEvaluator.IsValid(Expression);
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsValid_ReturnsFalse()
        {
            //mockParser.Setup(p => p.IsValidExpression(Expression)).Returns(false);

            var mockToken = new Mock<IToken>();
            var queue = new Queue<IToken>();
            var stack = new Stack<IToken>();

            mockParser.Setup(p => p.Tokenize(Expression)).Returns(queue);
            mockParser.Setup(p => p.BuildStack(queue)).Returns(stack);
            mockParser.Setup(p => p.CreateTree(stack)).Returns(mockToken.Object);

            var exception = new Exception("I failed");
            mockToken.Setup(t => t.EvalValue(null)).Throws(exception);

            var result = expressionEvaluator.IsValid(Expression);
            Assert.That(result, Is.False);
        }
    }
}
