using D20Dice.Tests.Integration.Common;
using NUnit.Framework;

namespace D20Dice.Tests.Bootstrap.Modules
{
    [TestFixture]
    public class CoreModuleTests : IntegrationTests
    {
        [Test]
        public void DiceAreCreatedAsSingletons()
        {
            var dice1 = GetNewInstanceOf<IDice>();
            var dice2 = GetNewInstanceOf<IDice>();
            Assert.That(dice1, Is.EqualTo(dice2));
        }
    }
}