using Ninject.Modules;

namespace D20Dice.Bootstrap.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDice>().To<CoreDice>().InSingletonScope();
        }
    }
}