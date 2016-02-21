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

        public override string RollString(string roll)
        {
            var matches = rollRegex.Matches(roll);

            foreach (var match in matches)
            {
                var matchValue = match.ToString();
                var matchIndex = roll.IndexOf(matchValue);

                var rolls = GetIndividualRolls(matchValue);
                var sumOfRolls = string.Join(" + ", rolls);
                var sumOfRollsInParantheses = string.Format("({0})", sumOfRolls);

                roll = roll.Remove(matchIndex, matchValue.Length);
                roll = roll.Insert(matchIndex, sumOfRollsInParantheses);
            }

            return roll;
        }

        public override object CompileRaw(string rolled) => Parser.GetParser().Compile(rolled).EvalValue(null);

        private IEnumerable<int> GetIndividualRolls(string roll)
        {
            var sections = roll.Split('d');
            var quantity = 1;

            if (string.IsNullOrEmpty(sections[0]) == false)
                quantity = Convert.ToInt32(sections[0]);

            var die = Convert.ToInt32(sections[1]);

            return Roll(quantity).IndividualRolls(die);
        }
    }
}
