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

        public static string RepeatedRollPattern(string roll) => $"{roll}(\\+{roll})+";
    }
}
