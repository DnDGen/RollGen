using DnDGen.RollGen.Expressions;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace DnDGen.RollGen.Tests.Unit.Expressions
{
    [TestFixture]
    public class RegexConstantsTests
    {
        [TestCase(RegexConstants.CommonRollRegexPattern, "d *\\d+(?: *("
            + "(! *(t *\\d+)+ *(k *\\d+)?)" //etk
            + "|(! *(k *\\d+)? *(t *\\d+)*)" //ekt
            + "|((t *\\d+)+ *! *(k *\\d+)?)" //tek
            + "|((t *\\d+)+ *(k *\\d+)? *!?)" //tke
            + "|((k *\\d+)? *! *(t *\\d+)*)" //ket
            + "|((k *\\d+)? *(t *\\d+)* *!?)" //kte or nothing
            + "))")]
        [TestCase(RegexConstants.StrictRollPattern, "(?:(?:\\d* +)|(?:\\d+ *)|^)" + RegexConstants.CommonRollRegexPattern)]
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
        [TestCase("2d4!t1t2k3", true)]
        [TestCase(" 2 d 4 ! t 1 k 3 ", true)]
        [TestCase("4d3t2k1", true)]
        [TestCase("4d3k1t2", true)]
        [TestCase("4d3!t2k1", true)]
        [TestCase("4d3!k1t2", true)]
        [TestCase("4d3t2!k1", true)]
        [TestCase("4d3k1!t2", true)]
        [TestCase("4d3t2k1!", true)]
        [TestCase("4d3k1t2!", true)]
        public void StrictRollRegexMatches(string source, bool isMatch)
        {
            VerifyMatch(RegexConstants.StrictRollPattern, source, isMatch);
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
