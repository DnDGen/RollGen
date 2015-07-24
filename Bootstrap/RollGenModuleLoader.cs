using Ninject;
using RollGen.Bootstrap.Modules;

namespace RollGen.Bootstrap
{
    public class RollGenModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<CoreModule>();
        }
    }
}