using Albatross.Expression;
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
            Bind<Dice>().To<DomainDice>();
            Bind<PartialRollFactory>().To<RandomPartialRollFactory>();
            Bind<ExpressionEvaluator>().To<AlbatrossExpressionEvaluator>();
            Bind<IParser>().ToMethod(c => Parser.GetParser());
        }
    }
}