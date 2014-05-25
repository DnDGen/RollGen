using System;
using D20Dice.Tests.Integration.Common;
using NUnit.Framework;

namespace D20Dice.Tests.Bootstrap.Modules
{
    [TestFixture]
    public class CoreModuleTests : IntegrationTests
    {
        [Test]
        public void DiceAreNotCreatedAsSingletons()
        {
            var dice1 = GetNewInstanceOf<IDice>();
            var dice2 = GetNewInstanceOf<IDice>();
            Assert.That(dice1, Is.Not.EqualTo(dice2));
        }

        [Test]
        public void RandomIsCreatedAsSingleton()
        {
            var random1 = GetNewInstanceOf<Random>();
            var random2 = GetNewInstanceOf<Random>();
            Assert.That(random1, Is.EqualTo(random2));
        }
    }
}