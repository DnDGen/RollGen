using Ninject;

namespace RollGen.Domain.IoC
{
    public static class DiceFactory
    {
        private static IKernel kernel = new StandardKernel();
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
