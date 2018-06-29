using Ninject;
using RollGen.IoC.Modules;

namespace RollGen.IoC
{
    public class RollGenModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<CoreModule>();
        }
    }
}