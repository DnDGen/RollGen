using Albatross.Expression;
using Ninject.Modules;
using RollGen.Domain.Expressions;
using RollGen.Domain.PartialRolls;
using System;

namespace RollGen.Domain.IoC.Modules
{
    internal class CoreModule : NinjectModule
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