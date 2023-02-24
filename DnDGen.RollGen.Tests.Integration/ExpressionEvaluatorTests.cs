using DnDGen.RollGen.Expressions;
using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration
{
    [TestFixture]
    public class ExpressionEvaluatorTests : IntegrationTests
    {
        private ExpressionEvaluator expressionEvaluator;

        [SetUp]
        public void Setup()
        {
            expressionEvaluator = GetNewInstanceOf<ExpressionEvaluator>();
        }

        [TestCase("-9266", -9266)]
        [TestCase("-2", -2)]
        [TestCase("-1", -1)]
        [TestCase("0", 0)]
        [TestCase("1", 1)]
        [TestCase("2", 2)]
        [TestCase("9266", 9266)]
        [TestCase("0", 0)]
        [TestCase("+1", 1)]
        [TestCase("+2", 2)]
        [TestCase("+9266", 9266)]
        [TestCase("9266.90210", 9266.9021)]
        [TestCase("9266+90210", 9266 + 90210)]
        [TestCase("9266-90210", 9266 - 90210)]
        [TestCase("9266%42", 9266 % 42)]
        [TestCase("9266/42", 9266 / 42d)]
        [TestCase("9266*42", 9266 * 42)]
        [TestCase("3^4", 81)]
        [TestCase("9266+90210-42", 9266 + 90210 - 42)]
        [TestCase("9266+90210-42*600", 9266 + 90210 - 42 * 600)]
        [TestCase("9266+(90210-42)*600", 9266 + (90210 - 42) * 600)]
        [TestCase("9266+90210-42*600/1337", 9266 + 90210 - 42 * 600 / 1337d)]
        [TestCase("9266+90210-42*600/1337%1336", 9266 + 90210 - 42 * 600 / 1337d % 1336)]
        [TestCase("avg(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", 9590.5)]
        [TestCase("coalesce(null, null, 9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", 9266)]
        [TestCase("max(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", 90210)]
        [TestCase("min(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", 42)]
        public void Evaluate_ReturnsValue(string expression, double expected)
        {
            var actual = expressionEvaluator.Evaluate<double>(expression);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("9266 < 90210", true)]
        [TestCase("9266 < 9266", false)]
        [TestCase("9266 < 42", false)]
        [TestCase("9266 <= 90210", true)]
        [TestCase("9266 <= 9266", true)]
        [TestCase("9266 <= 42", false)]
        [TestCase("9266 > 90210", false)]
        [TestCase("9266 > 9266", false)]
        [TestCase("9266 > 42", true)]
        [TestCase("9266 >= 90210", false)]
        [TestCase("9266 >= 9266", true)]
        [TestCase("9266 >= 42", true)]
        [TestCase("9266 <> 90210", true)]
        [TestCase("9266 <> 9266", false)]
        [TestCase("9266 <> 42", true)]
        [TestCase("9266 = 90210", false)]
        [TestCase("9266 = 9266", true)]
        [TestCase("9266 = 42", false)]
        [TestCase("9266 < 90210 and 42 > 600", false)]
        [TestCase("9266 < 90210 and 42 < 600", true)]
        [TestCase("9266 < 90210 or 42 > 600", true)]
        [TestCase("9266 < 90210 or 42 < 600", true)]
        [TestCase("9266 = 90210 or 42 > 600", false)]
        public void Evaluate_ReturnsBooleanValue(string expression, bool expected)
        {
            var actual = expressionEvaluator.Evaluate<bool>(expression);
            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("-9266", true)]
        [TestCase("-2", true)]
        [TestCase("-1", true)]
        [TestCase("0", true)]
        [TestCase("1", true)]
        [TestCase("2", true)]
        [TestCase("9266", true)]
        [TestCase("0", true)]
        [TestCase("+1", true)]
        [TestCase("+2", true)]
        [TestCase("+9266", true)]
        [TestCase("9266.90210", true)]
        [TestCase("9266+90210", true)]
        [TestCase("9266-90210", true)]
        [TestCase("9266%42", true)]
        [TestCase("9266/42", true)]
        [TestCase("9266*42", true)]
        [TestCase("3^4", true)]
        [TestCase("9266+90210-42", true)]
        [TestCase("9266+90210-42*600", true)]
        [TestCase("9266+(90210-42)*600", true)]
        [TestCase("9266+90210-42*600/1337", true)]
        [TestCase("9266+90210-42*600/1337%1336", true)]
        [TestCase("avg(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", true)]
        [TestCase("(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", false, Ignore = "Albatross interprets this as an array")]
        [TestCase("9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227", false)]
        [TestCase("coalesce(null, null, 9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", true)]
        [TestCase("max(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", true)]
        [TestCase("min(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", true)]
        [TestCase("bad(9266, 90210, 42, 600, 1337, 1336, 96, 783, 8245, 922, 2022, 227)", false)]
        [TestCase("9266 < 90210", true)]
        [TestCase("9266 < 9266", true)]
        [TestCase("9266 < 42", true)]
        [TestCase("9266 <= 90210", true)]
        [TestCase("9266 <= 9266", true)]
        [TestCase("9266 <= 42", true)]
        [TestCase("9266 =< 90210", false)]
        [TestCase("9266 =< 9266", false)]
        [TestCase("9266 =< 42", false)]
        [TestCase("9266 > 90210", true)]
        [TestCase("9266 > 9266", true)]
        [TestCase("9266 > 42", true)]
        [TestCase("9266 >= 90210", true)]
        [TestCase("9266 >= 9266", true)]
        [TestCase("9266 >= 42", true)]
        [TestCase("9266 => 90210", false)]
        [TestCase("9266 => 9266", false)]
        [TestCase("9266 => 42", false)]
        [TestCase("9266 <> 90210", true)]
        [TestCase("9266 <> 9266", true)]
        [TestCase("9266 <> 42", true)]
        [TestCase("9266 >< 90210", false)]
        [TestCase("9266 >< 9266", false)]
        [TestCase("9266 >< 42", false)]
        [TestCase("9266 = 90210", true)]
        [TestCase("9266 = 9266", true)]
        [TestCase("9266 = 42", true)]
        [TestCase("9266 == 90210", false)]
        [TestCase("9266 == 9266", false)]
        [TestCase("9266 == 42", false)]
        [TestCase("9266 < 90210 and 42 > 600", true)]
        [TestCase("9266 < 90210 and 42 < 600", true)]
        [TestCase("9266 < 90210 or 42 > 600", true)]
        [TestCase("9266 < 90210 or 42 < 600", true)]
        [TestCase("9266 = 90210 or 42 > 600", true)]
        [TestCase("1d6", false)]
        [TestCase("1d6+3", false)]
        [TestCase("1 d 6", false)]
        public void IsValid_ReturnsValidity(string expression, bool expected)
        {
            var actual = expressionEvaluator.IsValid(expression);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
