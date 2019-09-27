namespace RollGen.Expressions
{
    internal static class RegexConstants
    {
        public const string CommonRollRegexPattern = "d *\\d+(?: *((! *(k *\\d+)?)|((k *\\d+)? *!?)))?";
        public const string StrictRollPattern = "(?:(?:\\d* +)|(?:\\d+ *)|^)" + CommonRollRegexPattern;
        public const string ExpressionWithoutDieRollsPattern = "(?:[-+]?\\d*\\.?\\d+[%/\\+\\-\\*])+(?:[-+]?\\d*\\.?\\d+)";
        public const string LenientRollPattern = "\\d* *" + CommonRollRegexPattern;
        public const string BooleanExpressionPattern = "[<=>]";
    }
}
