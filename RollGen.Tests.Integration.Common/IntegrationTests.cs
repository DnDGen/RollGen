using Ninject;
using NUnit.Framework;
using RollGen.IoC;

namespace RollGen.Tests.Integration.Common
{
    [TestFixture]
    public abstract class IntegrationTests
    {
        private readonly IKernel kernel;

        public IntegrationTests()
        {
            kernel = new StandardKernel();

            var rollGenModuleLoader = new RollGenModuleLoader();
            rollGenModuleLoader.LoadModules(kernel);

            kernel.Inject(this);
        }

        protected T GetNewInstanceOf<T>()
        {
            return kernel.Get<T>();
        }
    }
}