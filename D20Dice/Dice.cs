using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Ciloci.Flee;

namespace D20Dice
{
    public class Dice : IDice
    {
        private const String DieRollRegexPattern = "\\d+ *d *\\d+";

        private Random random;
        private Regex regex;
        private ExpressionContext context;

        public Dice(Random random)
        {
            this.random = random;

            regex = new Regex(DieRollRegexPattern);
            context = new ExpressionContext(this);

            context.Options.OwnerMemberAccess = BindingFlags.Public | BindingFlags.NonPublic;
        }

        private Int32 Roll(Int32 quantity, Int32 die)
        {
            var roll = 0;

            while (quantity-- > 0)
                roll += random.Next(die) + 1;

            return roll;
        }

        public Int32 d2(Int32 quantity = 1)
        {
            return Roll(quantity, 2);
        }

        public Int32 d3(Int32 quantity = 1)
        {
            return Roll(quantity, 3);
        }

        public Int32 d4(Int32 quantity = 1)
        {
            return Roll(quantity, 4);
        }

        public Int32 d6(Int32 quantity = 1)
        {
            return Roll(quantity, 6);
        }

        public Int32 d8(Int32 quantity = 1)
        {
            return Roll(quantity, 8);
        }

        public Int32 d10(Int32 quantity = 1)
        {
            return Roll(quantity, 10);
        }

        public Int32 d12(Int32 quantity = 1)
        {
            return Roll(quantity, 12);
        }

        public Int32 d20(Int32 quantity = 1)
        {
            return Roll(quantity, 20);
        }

        public Int32 Percentile(Int32 quantity = 1)
        {
            return Roll(quantity, 100);
        }

        public Int32 Roll(String roll)
        {
            var formattedRoll = ConvertDieRollsToFunctionCalls(roll);
            var expression = context.CompileDynamic(formattedRoll);
            var answer = expression.Evaluate();

            return Convert.ToInt32(answer);
        }

        private String ConvertDieRollsToFunctionCalls(String roll)
        {
            var formattedRoll = roll;
            var matches = regex.Matches(formattedRoll);

            foreach (var match in matches)
            {
                var matchString = Convert.ToString(match);
                var arguments = matchString.Split('d');
                var replacement = String.Format("Roll({0},{1})", arguments[0], arguments[1]);
                formattedRoll = formattedRoll.Replace(matchString, replacement);
            }

            return formattedRoll;
        }
    }
}