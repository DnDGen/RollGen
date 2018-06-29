using Albatross.Expression;
using Ninject.Modules;
using RollGen.Expressions;
using RollGen.PartialRolls;
using System;

namespace RollGen.IoC.Modules
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