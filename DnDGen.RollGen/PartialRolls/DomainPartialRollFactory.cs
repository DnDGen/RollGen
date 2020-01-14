using DnDGen.RollGen.Expressions;
using System;

namespace DnDGen.RollGen.PartialRolls
{
    internal class DomainPartialRollFactory : PartialRollFactory
    {
        private readonly Random random;
        private readonly ExpressionEvaluator expressionEvaluator;

        public DomainPartialRollFactory(Random random, ExpressionEvaluator expressionEvaluator)
        {
            this.random = random;
            this.expressionEvaluator = expressionEvaluator;
        }

        public PartialRoll Build(int quantity)
        {
            return new DomainPartialRoll(quantity, random, expressionEvaluator);
        }

        public PartialRoll Build(string quantity)
        {
            return new DomainPartialRoll(quantity, random, expressionEvaluator);
        }
    }
}
