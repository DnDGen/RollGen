using DnDGen.RollGen.Expressions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace DnDGen.RollGen.Tests.Unit.Expressions
{
    [TestFixture]
    public class RegexConstantsTests
    {
        [TestCase(RegexConstants.NumberPattern, "-?\\d+")]
        [TestCase(RegexConstants.CommonRollRegexPattern, "d *" + RegexConstants.NumberPattern + "(?: *("
            + "( *!)" //explode default
            + "|( *(e *" + RegexConstants.NumberPattern + "))" //explode specific
            + "|( *(t *" + RegexConstants.NumberPattern + " *: *" + RegexConstants.NumberPattern + "))" //transform specific
            + "|( *(t *" + RegexConstants.NumberPattern + "))" //transform
            + "|( *(k *" + RegexConstants.NumberPattern + "))" //keep
            + ")*)")]
        [TestCase(RegexConstants.StrictRollPattern, "(?:(?:\\d* +)|(?:(?<=\\D|^)" + RegexConstants.NumberPattern + " *)|^)" + RegexConstants.CommonRollRegexPattern)]
        [TestCase(RegexConstants.LenientRollPattern, "\\d* *" + RegexConstants.CommonRollRegexPattern)]
        [TestCase(RegexConstants.ExpressionWithoutDieRollsPattern, "(?:[-+]?\\d*\\.?\\d+[%/\\+\\-\\*])+(?:[-+]?\\d*\\.?\\d+)")]
        [TestCase(RegexConstants.BooleanExpressionPattern, "[<=>]")]
        public void Pattern(string constant, string value)
        {
            Assert.That(constant, Is.EqualTo(value));
        }

        [TestCase("1d2", true)]
        [TestCase(" 1 d 2 ", true)]
        [TestCase("d2", true)]
        [TestCase(" d 2 ", true)]
        [TestCase("1 and 2", false)]
        [TestCase("1d2k3", true)]
        [TestCase("1d2k3!", true)]
        [TestCase(" 1 d 2 k 3 ", true)]
        [TestCase(" 1 d 2 k 3 ! ", true)]
        [TestCase("d2k3", true)]
        [TestCase(" d 2 k 3 ", true)]
        [TestCase("This is not a match", false)]
        [TestCase("4d6k3", true)]
        [TestCase(" 4 d 6 k 3 ", true)]
        [TestCase("1d2!", true)]
        [TestCase(" 1 d 2 ! ", true)]
        [TestCase("1d2!k3", true)]
        [TestCase(" 1 d 2 ! k 3 ", true)]
        [TestCase("3d6t1", true)]
        [TestCase(" 3 d 6 t 1 ", true)]
        [TestCase("3d6t1t2", true)]
        [TestCase(" 3 d 6 t 1 t 2 ", true)]
        [TestCase(" 3 d 6 t 1 : 2 ", true)]
        [TestCase("3d6t1:2t3:4k5", true)]
        [TestCase(" 3 d 6 t 1 : 2 t 3 : 4 k 5 ", true)]
        [TestCase("3d6t1t3:4k5", true)]
        [TestCase(" 3 d 6 t 1 t 3 : 4 k 5 ", true)]
        [TestCase("3d6t1:2t3k5", true)]
        [TestCase(" 3 d 6 t 1 : 2 t 3 k 5 ", true)]
        [TestCase("2d4!t1t2k3", true)]
        [TestCase(" 2 d 4 ! t 1 k 3 ", true)]
        [TestCase("1d4e2", true)]
        [TestCase(" 1 d 4 e 2 ", true)]
        [TestCase("1d4e2!", true)]
        [TestCase(" 1 d 4 e 2 ! ", true)]
        [TestCase("1d4e2e3!", true)]
        [TestCase(" 1 d 4 e 2 e 3 ! ", true)]
        [TestCase("1d4e4", true)]
        [TestCase(" 1 d 4 e 4 ", true)]
        [TestCase("1d1", true)]
        [TestCase("-1d1", true)]
        [TestCase("1d-1", true)]
        [TestCase("-1d-1", true)]
        [TestCase("-1d-2!t-3:-4k-5", true)]
        [TestCase("--1d2", true, false)]
        [TestCase("1d-2", true)]
        [TestCase("1d--2", false)]
        [TestCase("--1d--2", false)]
        [TestCase("-23d-45", true)]
        [TestCase("0d0", true)]
        [TestCase("00d00", true)]
        [TestCase("not a roll", false)]
        [TestCase("1d2-3d4", true, false)]
        [TestCase("Roll this: -5d6", true, false)]
        //From README
        [TestCase("4d6", true)]
        [TestCase("92d66", true)]
        [TestCase("5+3d4*2", true, false)]
        [TestCase("((1d2)d5k1)d6", true, false)]
        [TestCase("4d6k3", true)]
        [TestCase("3d4k2", true)]
        [TestCase("5+3d4*3", true, false)]
        [TestCase("1d6+3", true, false)]
        [TestCase("1d8+1d2-1", true, false)]
        [TestCase("4d3-3", true, false)]
        [TestCase("4d6!", true)]
        [TestCase("3d4!", true)]
        [TestCase("3d4!k2", true)]
        [TestCase("3d4!e3", true)]
        [TestCase("3d4e1e2k2", true)]
        [TestCase("3d6t1", true)]
        [TestCase("3d6t1t5", true)]
        [TestCase("3d6!t1k2", true)]
        [TestCase("3d6t1:2", true)]
        [TestCase("4d3t2k1", true)]
        [TestCase("4d3k1t2", true)]
        [TestCase("4d3!t2k1", true)]
        [TestCase("4d3!k1t2", true)]
        [TestCase("4d3t2!k1", true)]
        [TestCase("4d3k1!t2", true)]
        [TestCase("4d3t2k1!", true)]
        [TestCase("4d3k1t2!", true)]
        public void StrictRollRegexMatches(string source, bool isMatch, bool verifyMatchContents = true)
        {
            VerifyMatch(RegexConstants.StrictRollPattern, source, isMatch, verifyMatchContents);
        }

        private void VerifyMatch(string pattern, string source, bool isMatch, bool verifyMatchContents = true)
        {
            var regex = new Regex(pattern);
            var match = regex.Match(source);
            Assert.That(match.Success, Is.EqualTo(isMatch));

            if (match.Success && verifyMatchContents)
                Assert.That(match.Value.Trim(), Is.EqualTo(source.Trim()));
        }

        [TestCase("1", false)]
        [TestCase("1d2", false)]
        [TestCase("1d2k3", false)]
        [TestCase("This is not a match", false)]
        [TestCase("1+2-3*4/5%6", true)]
        [TestCase(" 1 + 2 - 3 * 4 / 5 % 6 ", true, IgnoreReason = "Ignoring because spaces might indicate a sentence, and the expression evaluator shouldn't override that")]
        public void ExpressionWithoutDieRollsRegexMatches(string source, bool isMatch)
        {
            VerifyMatch(RegexConstants.ExpressionWithoutDieRollsPattern, source, isMatch);
        }

        [TestCase("1", false)]
        [TestCase("1d2", false)]
        [TestCase("1d2k3", false)]
        [TestCase("This is not a match", false)]
        [TestCase("1+2-3*4/5%6", false)]
        [TestCase("1d20 > 10", true)]
        [TestCase("1d20 < 2d10", true)]
        [TestCase("3d2 >= 2d3", true)]
        [TestCase("2d6 <= 3d4", true)]
        [TestCase("1d2 = 2", true)]
        [TestCase("1d100 > 0", true)]
        [TestCase("100 < 1 d 20", true)]
        [TestCase("100<1d20", true)]
        [TestCase("1d1 = 1", true)]
        [TestCase("9266 = 9266", true)]
        [TestCase("9266 = 90210", true)]
        [TestCase("9266=90210", true)]
        [TestCase("1d2 = 3", true)]
        public void BooleanExpressionRegexMatches(string source, bool isMatch)
        {
            VerifyMatch(RegexConstants.BooleanExpressionPattern, source, isMatch, false);
        }
    }
}
