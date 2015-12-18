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

        public PartialRoll Roll(int quantity = 1)
        {
            return new RandomPartialRoll(quantity, random);
        }

        public int Roll(string roll)
        {
            var matches = rollRegex.Matches(roll);

            foreach (var match in matches)
            {
                var result = GetRoll(match.ToString());
                roll = roll.Replace(match.ToString(), result.ToString());
            }

            var rawValue = Parser.GetParser().Compile(roll).EvalValue(null);
            return Convert.ToInt32(rawValue);
        }

        private int GetRoll(string roll)
        {
            var sections = roll.Split('d');
            var quantity = Convert.ToInt32(sections[0]);
            var die = Convert.ToInt32(sections[1]);

            return Roll(quantity).d(die);
        }
    }
}