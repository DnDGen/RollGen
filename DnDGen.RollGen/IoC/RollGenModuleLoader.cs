using DnDGen.RollGen.IoC.Modules;
using Ninject;

namespace DnDGen.RollGen.IoC
{
    public class RollGenModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<CoreModule>();
        }
    }
}