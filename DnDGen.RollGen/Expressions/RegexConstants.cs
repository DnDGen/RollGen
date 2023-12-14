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

        public static (bool IsMatch, string Match, int Index, int MatchCount) GetRepeatedRoll(string roll, string source)
        {
            var repeatedRollRegex = new Regex($"(^|[\\+\\(]+){roll}(\\+{roll})+([\\+\\-\\)]+|$)");
            var repeatedMatch = repeatedRollRegex.Match(source);
            var matchCount = 0;
            var trimmedMatch = string.Empty;
            var index = -1;

            if (repeatedMatch.Success)
            {
                matchCount = new Regex(roll).Matches(repeatedMatch.Value).Count;
                trimmedMatch = repeatedMatch.Value;
                index = repeatedMatch.Index;

                while (trimmedMatch.EndsWith('+') || trimmedMatch.EndsWith('-') || trimmedMatch.EndsWith(')'))
                    trimmedMatch = trimmedMatch[..^1];

                while (trimmedMatch.StartsWith('+') || trimmedMatch.StartsWith('-') || trimmedMatch.StartsWith('('))
                {
                    trimmedMatch = trimmedMatch[1..trimmedMatch.Length];
                    index++;
                }
            }

            return (repeatedMatch.Success, trimmedMatch, index, matchCount);
        }
    }
}
