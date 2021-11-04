using DnDGen.RollGen.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DnDGen.RollGen.PartialRolls
{
    internal class Roll
    {
        public int Quantity { get; set; }
        public int Die { get; set; }
        public int AmountToKeep { get; set; }
        public bool Explode { get; set; }
        public readonly List<int> TransformToMax;

        public bool IsValid => QuantityValid && DieValid && KeepValid && ExplodeValid && TransformValid;
        private bool QuantityValid => Quantity > 0 && Quantity <= Limits.Quantity;
        private bool DieValid => Die > 0 && Die <= Limits.Die;
        private bool KeepValid => AmountToKeep > -1 && AmountToKeep <= Limits.Quantity;
        private bool ExplodeValid => !Explode || Die > 1;
        private bool TransformValid => !TransformToMax.Any() || TransformToMax.All(t => t > 0 && t <= Die);

        public Roll()
        {
            TransformToMax = new List<int>();
        }

        public Roll(string toParse)
        {
            toParse = toParse.Trim();
            Explode = toParse.Contains("!");
            toParse = toParse.Replace("!", string.Empty);

            var sections = new Dictionary<char, List<int>>();
            sections['q'] = new List<int>();
            sections['d'] = new List<int>();
            sections['t'] = new List<int>();
            sections['k'] = new List<int>();

            var key = 'q';
            var number = string.Empty;
            for (var i = 0; i < toParse.Length; i++)
            {
                if (char.IsWhiteSpace(toParse[i]))
                    continue;

                if (!char.IsDigit(toParse[i]) && !number.Any())
                {
                    key = toParse[i];
                    continue;
                }
                else if (char.IsDigit(toParse[i]))
                {
                    number += toParse[i];
                    continue;
                }

                if (!number.Any())
                    continue;

                sections[key].Add(Convert.ToInt32(number));

                number = string.Empty;
                key = toParse[i];
            }

            sections[key].Add(Convert.ToInt32(number));

            if (!sections['q'].Any())
                sections['q'].Add(1);

            Quantity = sections['q'][0];
            Die = sections['d'][0];

            if (sections['k'].Any())
                AmountToKeep = sections['k'][0];

            TransformToMax = sections['t'];
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

                if (Explode && roll == Die)
                {
                    --i; // We're rerolling this die, so Quantity actually stays the same.
                }

                if (TransformToMax.Contains(roll))
                {
                    roll = Die;
                }

                rolls.Add(roll);
            }

            if (AmountToKeep > 0 && AmountToKeep < rolls.Count())
                return rolls.OrderByDescending(r => r).Take(AmountToKeep);

            return rolls;
        }

        private void ValidateRoll()
        {
            if (IsValid)
                return;

            var message = $"{this} is not a valid roll.";

            if (!QuantityValid)
                message += $"\n\tQuantity: 0 < {Quantity} < {Limits.Quantity}";

            if (!DieValid)
                message += $"\n\tDie: 0 < {Die} < {Limits.Die}";

            if (!KeepValid)
                message += $"\n\tKeep: 0 <= {AmountToKeep} < {Limits.Quantity}";

            if (!ExplodeValid)
                message += $"\n\tExplode: Cannot explode die {Die}, must be > 1";

            if (!TransformValid)
                message += $"\n\tTransform: 0 < [{string.Join(',', TransformToMax)}] <= {Die}";

            throw new InvalidOperationException(message);
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

            var min = GetPotentialMinimum();
            var max = GetPotentialMaximum(false);
            var average = (min + max) / 2.0d;

            return average;
        }

        public int GetPotentialMinimum()
        {
            ValidateRoll();

            var quantity = GetEffectiveQuantity();
            var min = 1;

            while (TransformToMax.Contains(min) && min < Die)
            {
                min++;
            }

            return min * quantity;
        }

        private int GetEffectiveQuantity()
        {
            if (AmountToKeep > 0)
                return Math.Min(Quantity, AmountToKeep);

            return Quantity;
        }

        public int GetPotentialMaximum(bool includeExplode = true)
        {
            ValidateRoll();

            var quantity = GetEffectiveQuantity();
            var max = quantity * Die;

            //INFO: Since exploded dice can in theory be infinite, we will assume 10x multiplier,
            //which should cover 99.9% of use cases
            if (Explode && includeExplode)
                max *= 10;

            return max;
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

            if (Explode)
                output += "!";

            foreach (var transform in TransformToMax)
                output += $"t{transform}";

            if (AmountToKeep != 0)
                output += $"k{AmountToKeep}";

            return output;
        }
    }
}
