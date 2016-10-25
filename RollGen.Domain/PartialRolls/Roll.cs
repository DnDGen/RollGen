using RollGen.Domain.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace RollGen.Domain.PartialRolls
{
    internal class Roll
    {
        public int Quantity { get; set; }
        public int Die { get; set; }
        public int AmountToKeep { get; set; }

        public bool IsValid
        {
            get
            {
                return Quantity > 0
                    && Die > 0
                    && AmountToKeep > -1
                    && Quantity <= Limits.Quantity
                    && Die <= Limits.Die
                    && Die * (long)Quantity <= Limits.ProductOfQuantityAndDie;
            }
        }

        public Roll() { }

        public Roll(string toParse)
        {
            var sections = toParse.Trim().Split('d', 'k');
            Die = Convert.ToInt32(sections[1]);
            Quantity = 1;

            if (!string.IsNullOrEmpty(sections[0]))
                Quantity = Convert.ToInt32(sections[0]);

            if (sections.Length == 3 && !string.IsNullOrEmpty(sections[2]))
                AmountToKeep = Convert.ToInt32(sections[2]);
        }

        public static bool CanParse(string toParse)
        {
            if (string.IsNullOrWhiteSpace(toParse))
                return false;

            var trimmedSource = toParse.Trim();
            var strictRollRegex = new Regex(RegexConstants.StrictRollPattern);
            var rollMatch = strictRollRegex.Match(trimmedSource);

            return rollMatch.Success && rollMatch.Value == trimmedSource;
        }

        public IEnumerable<int> GetRolls(Random random)
        {
            ValidateRoll();

            var rolls = new List<int>(Quantity);

            for (var i = 0; i < Quantity; i++)
            {
                var roll = random.Next(Die) + 1;
                rolls.Add(roll);
            }

            if (AmountToKeep > 0 && AmountToKeep < rolls.Count())
                return rolls.OrderByDescending(r => r).Take(AmountToKeep);

            return rolls;
        }

        private void ValidateRoll()
        {
            if (!IsValid)
                throw new InvalidOperationException($"{this} is not a valid roll.  It might be too large for RollGen or involve values that are too low.");
        }

        public int GetSum(Random random)
        {
            ValidateRoll();

            var rolls = GetRolls(random);
            return rolls.Sum();
        }

        public double GetPotentialAverage()
        {
            ValidateRoll();

            var low = Quantity;

            if (AmountToKeep > 0)
                low = Math.Min(Quantity, AmountToKeep);

            var average = low * (Die + 1) / 2.0d;
            return average;
        }

        public bool GetTrueOrFalse(Random random)
        {
            ValidateRoll();

            var average = GetPotentialAverage();
            var sum = GetSum(random);

            return sum >= average;
        }

        public override string ToString()
        {
            var output = $"{Quantity}d{Die}";

            if (AmountToKeep > 0)
                output += $"k{AmountToKeep}";

            return output;
        }
    }
}
