using System;
using Ninject.Modules;

namespace D20Dice.Bootstrap.Modules
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Random>().ToSelf().InSingletonScope();
            Bind<IDice>().To<Dice>();
        }
    }
}