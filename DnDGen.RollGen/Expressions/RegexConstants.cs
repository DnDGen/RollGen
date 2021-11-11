namespace DnDGen.RollGen.Expressions
{
    internal static class RegexConstants
    {
        public const string CommonRollRegexPattern = "d *\\d+(?: *("
            + "( *!)" //explode default
            + "|( *(e *\\d+))" //explode specific
            + "|( *(t *\\d+ *: *\\d+))" //transform specific
            + "|( *(t *\\d+))" //transform
            + "|( *(k *\\d+))" //keep
            + ")*)";
        public const string StrictRollPattern = "(?:(?:\\d* +)|(?:\\d+ *)|^)" + CommonRollRegexPattern;
        public const string ExpressionWithoutDieRollsPattern = "(?:[-+]?\\d*\\.?\\d+[%/\\+\\-\\*])+(?:[-+]?\\d*\\.?\\d+)";
        public const string LenientRollPattern = "\\d* *" + CommonRollRegexPattern;
        public const string BooleanExpressionPattern = "[<=>]";
    }
}
