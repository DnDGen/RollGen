using Ninject;

namespace DnDGen.RollGen.IoC
{
    public static class DiceFactory
    {
        private static readonly IKernel kernel = new StandardKernel();
        private static bool modulesLoaded = false;

        public static Dice Create()
        {
            if (!modulesLoaded)
            {
                var rollGenModuleLoader = new RollGenModuleLoader();
                rollGenModuleLoader.LoadModules(kernel);
                modulesLoaded = true;
            }

            return kernel.Get<Dice>();
        }
    }
}
