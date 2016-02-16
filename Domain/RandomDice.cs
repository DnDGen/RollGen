using Albatross.Expression;
using System;
using System.Text.RegularExpressions;

namespace RollGen.Domain
{
    public class RandomDice : Dice
    {
        private readonly Random random;
        private Regex rollRegex;

        public RandomDice(Random random)
        {
            this.random = random;

            rollRegex = new Regex("\\d+d\\d+");
        }

        public override PartialRoll Roll(int quantity = 1)
        {
            return new RandomPartialRoll(quantity, random);
        }

        public override string RolledString(string roll)
        {
            var matches = rollRegex.Matches(roll);

            foreach (var match in matches)
            {
                var result = GetRoll(match.ToString());
                roll = roll.Replace(match.ToString(), result.ToString());
            }
            return roll;
        }

        public override int Compiled(string rolled) => Convert.ToInt32(Parser.GetParser().Compile(rolled).EvalValue(null));

        private int GetRoll(string roll)
        {
            var sections = roll.Split('d');
            var quantity = Convert.ToInt32(sections[0]);
            var die = Convert.ToInt32(sections[1]);

            return Roll(quantity).d(die);
        }
    }
}
