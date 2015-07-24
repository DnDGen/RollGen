using Ninject.Modules;
using RollGen.Domain;
using System;

namespace RollGen.Bootstrap.Modules
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