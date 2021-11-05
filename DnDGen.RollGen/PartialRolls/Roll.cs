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
        public readonly List<int> ExplodeOn;
        public readonly Dictionary<int, int> Transforms;

        public bool IsValid => QuantityValid && DieValid && KeepValid && ExplodeValid && TransformValid;
        private bool QuantityValid => Quantity > 0 && Quantity <= Limits.Quantity;
        private bool DieValid => Die > 0 && Die <= Limits.Die;
        private bool KeepValid => AmountToKeep > -1 && AmountToKeep <= Limits.Quantity;
        private bool Explode => ExplodeOn.Any();
        private bool ExplodeValid => !ExplodeOn.Any() || (DieValid && Enumerable.Range(1, Die).Except(ExplodeOn).Any());
        private bool TransformValid => !Transforms.Any() || Transforms.All(kvp => kvp.Key > 0 && kvp.Key <= Limits.Die);

        public Roll()
        {
            ExplodeOn = new List<int>();
            Transforms = new Dictionary<int, int>();
        }

        public Roll(string toParse)
        {
            var sections = new Dictionary<char, List<int>>();
            sections['q'] = new List<int>();
            sections['d'] = new List<int>();
            sections['e'] = new List<int>();
            sections['t'] = new List<int>();
            sections[':'] = new List<int>();
            sections['k'] = new List<int>();

            var key = 'q';
            var number = string.Empty;
            var explodeDefault = false;

            for (var i = 0; i < toParse.Length; i++)
            {
                if (char.IsWhiteSpace(toParse[i]))
                    continue;

                if (toParse[i] == '!')
                {
                    explodeDefault = true;
                    continue;
                }

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

                //INFO: This means we are using the "Default" transform to the max die value
                if (toParse[i] != ':' && key == 't')
                {
                    sections[':'].Add(sections['d'][0]);
                }

                sections[key].Add(Convert.ToInt32(number));

                number = string.Empty;
                key = toParse[i];
            }

            sections[key].Add(Convert.ToInt32(number));

            if (!sections['q'].Any())
                sections['q'].Add(1);

            Quantity = sections['q'][0];
            Die = sections['d'][0];

            if (explodeDefault)
                sections['e'].Add(Die);

            if (sections['k'].Any())
                AmountToKeep = sections['k'][0];

            ExplodeOn = sections['e'].Distinct().ToList();

            for (var i = 0; i < sections['t'].Count; i++)
            {
                Transforms[sections['t'][i]] = sections[':'][i];
            }
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

                if (ExplodeOn.Contains(roll))
                {
                    --i; // We're rerolling this die, so Quantity actually stays the same.
                }

                if (Transforms.ContainsKey(roll))
                {
                    roll = Transforms[roll];
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

            var message = $"{this} is not a valid roll";

            if (!QuantityValid)
                message += $"\n\tQuantity: 0 < {Quantity} < {Limits.Quantity}";

            if (!DieValid)
                message += $"\n\tDie: 0 < {Die} < {Limits.Die}";

            if (!KeepValid)
                message += $"\n\tKeep: 0 <= {AmountToKeep} < {Limits.Quantity}";

            if (!ExplodeValid)
                message += $"\n\tExplode: Must have at least 1 non-exploded roll";

            if (!TransformValid)
                message += $"\n\tTransform: 0 < [{string.Join(',', Transforms.Select(kvp => $"{kvp.Key}:{kvp.Value}"))}] <= {Limits.Die}";

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

            while ((Transforms.ContainsKey(min) || ExplodeOn.Contains(min)) && min < Die)
            {
                min++;
            }

            var minTransform = Transforms.Values.Min();

            return Math.Min(min, minTransform) * quantity;
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
            var maxTransform = Transforms.Values.Max();
            var max = quantity * Math.Max(Die, maxTransform);

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

            foreach (var explode in ExplodeOn)
                output += $"e{explode}";

            foreach (var kvp in Transforms)
                output += $"t{kvp.Key}:{kvp.Value}";

            if (AmountToKeep != 0)
                output += $"k{AmountToKeep}";

            return output;
        }
    }
}
