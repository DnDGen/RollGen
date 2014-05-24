using System;
using System.Collections.Generic;
using System.Linq;

namespace D20Dice
{
    public class CoreDice : IDice
    {
        private Random random;

        public CoreDice(Random random)
        {
            this.random = random;
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
            var computedString = ParseParantheses(roll);
            computedString = ParseDieRolls(computedString);
            computedString = ParseMultiplication(computedString);
            computedString = ParseAddition(computedString);

            return Convert.ToInt32(computedString);
        }

        private String ParseParantheses(String input)
        {
            var newString = FormatImplicitMultiplication(input);
            var parantheticalStatements = GetParantheticalStatements(newString);

            foreach (var statement in parantheticalStatements)
            {
                var unwrappedStatement = statement.Substring(1, statement.Length - 2);
                var computedStatement = Roll(unwrappedStatement);

                newString = newString.Replace(statement, computedStatement.ToString());
            }

            return newString;
        }

        private IEnumerable<String> GetParantheticalStatements(String input)
        {
            var parantheticalStatements = new List<String>();
            var openParanthesisIndex = 0;

            while (openParanthesisIndex > -1 && openParanthesisIndex < input.Length)
            {
                openParanthesisIndex = input.IndexOf('(', openParanthesisIndex);

                if (openParanthesisIndex > -1)
                {
                    var statementLength = GetParantheticalStatementLength(input, openParanthesisIndex);
                    var parantheticalStatement = input.Substring(openParanthesisIndex, statementLength);

                    parantheticalStatements.Add(parantheticalStatement);
                    openParanthesisIndex += statementLength;
                }
            }

            return parantheticalStatements;
        }

        private Int32 GetParantheticalStatementLength(String input, Int32 openParanthesisIndex)
        {
            var matching = 0;
            var closingIndex = 0;
            for (var i = openParanthesisIndex; i < input.Length; i++)
            {
                if (input[i] == '(')
                    matching++;
                else if (input[i] == ')')
                    matching--;

                if (matching == 0 && input[i] == ')')
                    closingIndex = i;
            }

            return closingIndex - openParanthesisIndex + 1;
        }

        private String FormatImplicitMultiplication(String input)
        {
            var newString = input;

            var paranthesisIndex = newString.Length - 1;
            while (paranthesisIndex > 0)
            {
                paranthesisIndex = newString.LastIndexOf(')', paranthesisIndex);

                if (paranthesisIndex > 0 && FollowedByNumber(newString, paranthesisIndex))
                    newString = newString.Insert(paranthesisIndex + 1, "*");

                paranthesisIndex--;
            }

            paranthesisIndex = newString.Length - 1;
            while (paranthesisIndex > 0)
            {
                paranthesisIndex = newString.LastIndexOf('(', paranthesisIndex);

                if (paranthesisIndex > 0 && PrecededByNumber(newString, paranthesisIndex))
                    newString = newString.Insert(paranthesisIndex, "*");

                paranthesisIndex--;
            }

            return newString;
        }

        private Boolean FollowedByNumber(String input, Int32 index)
        {
            if (index == input.Length - 1)
                return false;

            var numbers = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            return numbers.Contains(input[index + 1]);
        }

        private Boolean PrecededByNumber(String input, Int32 index)
        {
            if (index == 0)
                return false;

            var numbers = new[] { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            return numbers.Contains(input[index - 1]);
        }

        private String ParseDieRolls(String input)
        {
            var newString = input;
            var dieRolls = input.Split('+', '-', '*').Where(s => s.Contains('d'));

            foreach (var dieRoll in dieRolls)
            {
                var parsedRoll = dieRoll.Split('d');

                var quantity = Convert.ToInt32(parsedRoll[0]);
                for (var i = 1; i < parsedRoll.Length; i++)
                {
                    var die = Convert.ToInt32(parsedRoll[i]);
                    quantity = Roll(quantity, die);
                }

                var firstIndex = newString.IndexOf(dieRoll);
                newString = newString.Insert(firstIndex, "#");
                newString = newString.Replace("#" + dieRoll, quantity.ToString());
            }

            return newString;
        }

        private String ParseMultiplication(String input)
        {
            var newString = input;
            var products = input.Split('+', '-').Where(s => s.Contains('*')).Distinct();

            foreach (var productString in products)
            {
                var multiplicants = productString.Split('*');

                var product = 1;
                foreach (var multiplicant in multiplicants)
                    product *= Convert.ToInt32(multiplicant);

                newString = newString.Replace(productString, product.ToString());
            }

            return newString;
        }

        private String ParseAddition(String input)
        {
            var newString = FormatSubtraction(input);
            var addends = newString.Split('+');

            var sum = 0;
            foreach (var addend in addends)
                sum += Convert.ToInt32(addend);

            return sum.ToString();
        }

        private String FormatSubtraction(String input)
        {
            var newString = input;

            var minusIndex = newString.Length - 1;
            while (minusIndex > 0)
            {
                minusIndex = newString.LastIndexOf('-', minusIndex);

                if (minusIndex > 0 && newString[minusIndex - 1] != '+')
                    newString = newString.Insert(minusIndex, "+");

                minusIndex--;
            }

            return newString;
        }
    }
}