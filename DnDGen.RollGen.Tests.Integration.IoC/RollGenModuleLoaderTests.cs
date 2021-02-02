using DnDGen.RollGen.IoC;
using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.IoC
{
    [TestFixture]
    public class RollGenModuleLoaderTests : IntegrationTests
    {
        [Test]
        public void ModuleLoaderCanBeRunTwice()
        {
            //INFO: First time was in the IntegrationTest one-time setup
            var rollGenLoader = new RollGenModuleLoader();
            rollGenLoader.LoadModules(kernel);

            var dice = GetNewInstanceOf<Dice>();
            Assert.That(dice, Is.Not.Null);
        }
    }
}
