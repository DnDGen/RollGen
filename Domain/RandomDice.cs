using Albatross.Expression;
using System;
using System.Collections.Generic;
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

            rollRegex = new Regex("\\d* *d *\\d+");
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
                var m = match.ToString();
                int i = roll.IndexOf(m);
                roll = $"{roll.Substring(0, i)}({string.Join(" + ", GetRoll(m))}){roll.Substring(i+m.Length)}";
            }
            return roll;
        }

        public override object CompiledObj(string rolled) => Parser.GetParser().Compile(rolled).EvalValue(null);

        private IEnumerable<int> GetRoll(string roll)
        {
            var sections = roll.Split('d');
            var quantity_string = sections[0].Trim();
            var quantity = quantity_string.Length == 0 ? 1 : Convert.ToInt32(quantity_string);
            var die = Convert.ToInt32(sections[1].Trim());

            return Roll(quantity).multi_d(die);
        }
    }
}
