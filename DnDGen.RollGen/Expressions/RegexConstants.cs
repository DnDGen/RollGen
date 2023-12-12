using System.Text.RegularExpressions;

namespace DnDGen.RollGen.Expressions
{
    internal static class RegexConstants
    {
        public const string NumberPattern = "-?\\d+";
        public const string CommonRollRegexPattern = "d *" + NumberPattern + "(?: *("
            + "( *!)" //explode default
            + "|( *(e *" + NumberPattern + "))" //explode specific
            + "|( *(t *" + NumberPattern + " *: *" + NumberPattern + "))" //transform specific
            + "|( *(t *" + NumberPattern + "))" //transform
            + "|( *(k *" + NumberPattern + "))" //keep
            + ")*)";
        public const string StrictRollPattern = "(?:(?:\\d* +)|(?:(?<=\\D|^)" + NumberPattern + " *)|^)" + CommonRollRegexPattern;
        public const string ExpressionWithoutDieRollsPattern = "(?:[-+]?\\d*\\.?\\d+[%/\\+\\-\\*])+(?:[-+]?\\d*\\.?\\d+)";
        public const string LenientRollPattern = "\\d* *" + CommonRollRegexPattern;
        public const string BooleanExpressionPattern = "[<=>]";

        private static string RepeatedRollPattern(string roll) => $"{roll}(\\+{roll})+";

        public static (Match Match, int MatchCount) GetRepeatedRoll(string roll, string source)
        {
            var repeatedRollPattern = RepeatedRollPattern(roll);
            var repeatedRollRegex = new Regex(repeatedRollPattern);
            var repeatedMatch = repeatedRollRegex.Match(source);
            var matchCount = 0;

            if (repeatedMatch.Success)
            {
                matchCount = new Regex(roll).Matches(repeatedMatch.Value).Count;
            }

            return (repeatedMatch, matchCount);
        }
    }
}
