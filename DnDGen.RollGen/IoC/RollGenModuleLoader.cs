using DnDGen.RollGen.IoC.Modules;
using Ninject;
using System.Linq;

namespace DnDGen.RollGen.IoC
{
    public class RollGenModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            var modules = kernel.GetModules();

            if (!modules.Any(m => m is CoreModule))
                kernel.Load<CoreModule>();
        }
    }
}