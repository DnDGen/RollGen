using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RollGen
{
    public abstract class Dice
    {
        protected Regex rollRegex;

        public Dice()
        {
            rollRegex = new Regex("\\d* *d *\\d+");
        }

        public abstract PartialRoll Roll(int quantity = 1);
        public abstract object Compute(string rolled);

        public int Roll(string roll)
        {
            var computableRoll = RollString(roll);
            return Compute<int>(computableRoll);
        }

        public T Compute<T>(string rolled)
        {
            var rawRoll = Compute(rolled);

            if (rawRoll is T)
                return (T)rawRoll;

            return (T)Convert.ChangeType(rawRoll, typeof(T));
        }

        public string RollString(string roll)
        {
            var matches = rollRegex.Matches(roll);

            foreach (var match in matches)
            {
                var matchValue = match.ToString();
                var matchIndex = roll.IndexOf(matchValue);

                var rolls = GetIndividualRolls(matchValue);
                var sumOfRolls = string.Join(" + ", rolls);
                var sumOfRollsInParentheses = string.Format("({0})", sumOfRolls);

                roll = roll.Remove(matchIndex, matchValue.Length);
                roll = roll.Insert(matchIndex, sumOfRollsInParentheses);
            }

            return roll;
        }

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