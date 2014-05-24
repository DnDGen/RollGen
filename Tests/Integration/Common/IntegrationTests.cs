using D20Dice.Bootstrap;
using Ninject;
using NUnit.Framework;

namespace D20Dice.Tests.Integration.Common
{
    [TestFixture]
    public abstract class IntegrationTests
    {
        private IKernel kernel;

        public IntegrationTests()
        {
            kernel = new StandardKernel();

            var d20DiceModuleLoader = new D20DiceModuleLoader();
            d20DiceModuleLoader.LoadModules(kernel);

            kernel.Inject(this);
        }

        protected T GetNewInstanceOf<T>()
        {
            return kernel.Get<T>();
        }
    }
}