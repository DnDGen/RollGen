using Ninject;
using RollGen.Domain.IoC.Modules;

namespace RollGen.Domain.IoC
{
    public class RollGenModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<CoreModule>();
        }
    }
}