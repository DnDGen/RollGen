namespace DnDGen.RollGen.Expressions
{
    internal static class RegexConstants
    {
        public const string CommonRollRegexPattern = "d *\\d+(?: *("
            + "(! *(t *\\d+)+ *(k *\\d+)?)" //etk
            + "|(! *(k *\\d+)? *(t *\\d+)*)" //ekt
            + "|((t *\\d+)+ *! *(k *\\d+)?)" //tek
            + "|((t *\\d+)+ *(k *\\d+)? *!?)" //tke
            + "|((k *\\d+)? *! *(t *\\d+)*)" //ket
            + "|((k *\\d+)? *(t *\\d+)* *!?)" //kte or nothing
            + "))";
        public const string StrictRollPattern = "(?:(?:\\d* +)|(?:\\d+ *)|^)" + CommonRollRegexPattern;
        public const string ExpressionWithoutDieRollsPattern = "(?:[-+]?\\d*\\.?\\d+[%/\\+\\-\\*])+(?:[-+]?\\d*\\.?\\d+)";
        public const string LenientRollPattern = "\\d* *" + CommonRollRegexPattern;
        public const string BooleanExpressionPattern = "[<=>]";
    }
}
