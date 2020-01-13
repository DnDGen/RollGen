using Albatross.Expression;
using DnDGen.RollGen.Expressions;
using DnDGen.RollGen.PartialRolls;
using Ninject.Modules;
using System;

namespace DnDGen.RollGen.IoC.Modules
{
    internal class CoreModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Random>().ToSelf().InSingletonScope();
            Bind<Dice>().To<DomainDice>();
            Bind<PartialRollFactory>().To<DomainPartialRollFactory>();
            Bind<ExpressionEvaluator>().To<AlbatrossExpressionEvaluator>();
            Bind<IParser>().ToMethod(c => Factory.Instance.Create());
        }
    }
}