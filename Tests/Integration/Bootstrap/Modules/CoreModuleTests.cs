using Albatross.Expression;
using Ninject;
using NUnit.Framework;
using RollGen.Domain;
using RollGen.Tests.Integration.Common;
using System;

namespace RollGen.Tests.Bootstrap.Modules
{
    [TestFixture]
    public class CoreModuleTests : IntegrationTests
    {
        [Test]
        public void DiceAreNotCreatedAsSingletons()
        {
            var dice1 = GetNewInstanceOf<Dice>();
            var dice2 = GetNewInstanceOf<Dice>();
            Assert.That(dice1, Is.Not.EqualTo(dice2));
        }

        [Test]
        public void RandomIsCreatedAsSingleton()
        {
            var random1 = GetNewInstanceOf<Random>();
            var random2 = GetNewInstanceOf<Random>();
            Assert.That(random1, Is.EqualTo(random2));
        }

        [Test]
        public void CannotInjectPartialRoll()
        {
            Assert.That(() => GetNewInstanceOf<PartialRoll>(), Throws.InstanceOf<ActivationException>());
        }

        [Test]
        public void ExpressionEvaluatorIsInjected()
        {
            var evaluator = GetNewInstanceOf<ExpressionEvaluator>();
            Assert.That(evaluator, Is.Not.Null);
            Assert.That(evaluator, Is.InstanceOf<AlbatrossExpressionEvaluator>());
        }

        [Test]
        public void PartialRollFactoryIsInjected()
        {
            var factory = GetNewInstanceOf<PartialRollFactory>();
            Assert.That(factory, Is.Not.Null);
            Assert.That(factory, Is.InstanceOf<RandomPartialRollFactory>());
        }

        [Test]
        public void AlbatrossParserIsInjected()
        {
            var parser = GetNewInstanceOf<IParser>();
            Assert.That(parser, Is.Not.Null);
            Assert.That(parser, Is.InstanceOf<Parser>());
        }
    }
}