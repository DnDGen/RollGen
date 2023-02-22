using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration
{
    [TestFixture]
    public class DescribeTests : IntegrationTests
    {
        private Dice dice;

        [SetUp]
        public void Setup()
        {
            dice = GetNewInstanceOf<Dice>();
        }

        [TestCase("1", 1, "Good")]
        [TestCase("9266", 9266, "Good")]
        [TestCase("1d4", 1, "Bad")]
        [TestCase("1d4", 2, "Bad")]
        [TestCase("1d4", 3, "Good")]
        [TestCase("1d4", 4, "Good")]
        [TestCase("1d20", 1, "Bad")]
        [TestCase("1d20", 10, "Bad")]
        [TestCase("1d20", 11, "Good")]
        [TestCase("1d20", 20, "Good")]
        [TestCase("3d6", 3, "Bad")]
        [TestCase("3d6", 10, "Bad")]
        [TestCase("3d6", 11, "Good")]
        [TestCase("3d6", 12, "Good")]
        [TestCase("3d6", 18, "Good")]
        [TestCase("1d100", 1, "Bad")]
        [TestCase("1d100", 50, "Bad")]
        [TestCase("1d100", 51, "Good")]
        [TestCase("1d100", 100, "Good")]
        public void DescribeRoll_Default(string rollExpression, int roll, string expectedDescription)
        {
            var description = dice.Describe(rollExpression, roll);
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase("1", 1, "Succeed")]
        [TestCase("9266", 9266, "Succeed")]
        [TestCase("1d4", 1, "Failed")]
        [TestCase("1d4", 2, "Failed")]
        [TestCase("1d4", 3, "Succeed")]
        [TestCase("1d4", 4, "Succeed")]
        [TestCase("1d20", 1, "Failed")]
        [TestCase("1d20", 10, "Failed")]
        [TestCase("1d20", 11, "Succeed")]
        [TestCase("1d20", 20, "Succeed")]
        [TestCase("3d6", 3, "Failed")]
        [TestCase("3d6", 10, "Failed")]
        [TestCase("3d6", 11, "Succeed")]
        [TestCase("3d6", 12, "Succeed")]
        [TestCase("3d6", 18, "Succeed")]
        [TestCase("1d100", 1, "Failed")]
        [TestCase("1d100", 50, "Failed")]
        [TestCase("1d100", 51, "Succeed")]
        [TestCase("1d100", 100, "Succeed")]
        public void DescribeRoll_FailedSucceed(string rollExpression, int roll, string expectedDescription)
        {
            var description = dice.Describe(rollExpression, roll, "Failed", "Succeed");
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase("1", 1, "Average")]
        [TestCase("9266", 9266, "Average")]
        [TestCase("1d4", 1, "Low")]
        [TestCase("1d4", 2, "Average")]
        [TestCase("1d4", 3, "High")]
        [TestCase("1d4", 4, "High")]
        [TestCase("1d20", 1, "Low")]
        [TestCase("1d20", 7, "Low")]
        [TestCase("1d20", 8, "Average")]
        [TestCase("1d20", 10, "Average")]
        [TestCase("1d20", 11, "Average")]
        [TestCase("1d20", 13, "Average")]
        [TestCase("1d20", 14, "High")]
        [TestCase("1d20", 20, "High")]
        [TestCase("3d6", 3, "Low")]
        [TestCase("3d6", 7, "Low")]
        [TestCase("3d6", 8, "Average")]
        [TestCase("3d6", 10, "Average")]
        [TestCase("3d6", 12, "Average")]
        [TestCase("3d6", 13, "High")]
        [TestCase("3d6", 18, "High")]
        [TestCase("1d100", 1, "Low")]
        [TestCase("1d100", 33, "Low")]
        [TestCase("1d100", 34, "Average")]
        [TestCase("1d100", 66, "Average")]
        [TestCase("1d100", 67, "High")]
        [TestCase("1d100", 100, "High")]
        public void DescribeRoll_LowAverageHigh(string rollExpression, int roll, string expectedDescription)
        {
            var description = dice.Describe(rollExpression, roll, "Low", "Average", "High");
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase("1", 1, "Average")]
        [TestCase("9266", 42, "Very Low")]
        [TestCase("9266", 9266, "Average")]
        [TestCase("9266", 90210, "Very High")]
        [TestCase("1d4", 1, "Very Low")]
        [TestCase("1d4", 2, "Low")]
        [TestCase("1d4", 3, "High")]
        [TestCase("1d4", 4, "Very High")]
        [TestCase("1d20", 1, "Very Low")]
        [TestCase("1d20", 4, "Very Low")]
        [TestCase("1d20", 5, "Low")]
        [TestCase("1d20", 8, "Low")]
        [TestCase("1d20", 9, "Average")]
        [TestCase("1d20", 12, "Average")]
        [TestCase("1d20", 13, "High")]
        [TestCase("1d20", 16, "High")]
        [TestCase("1d20", 17, "Very High")]
        [TestCase("1d20", 20, "Very High")]
        [TestCase("3d6", 3, "Very Low")]
        [TestCase("3d6", 5, "Very Low")]
        [TestCase("3d6", 6, "Low")]
        [TestCase("3d6", 8, "Low")]
        [TestCase("3d6", 9, "Average")]
        [TestCase("3d6", 11, "Average")]
        [TestCase("3d6", 12, "High")]
        [TestCase("3d6", 14, "High")]
        [TestCase("3d6", 15, "Very High")]
        [TestCase("3d6", 18, "Very High")]
        [TestCase("1d100", 1, "Very Low")]
        [TestCase("1d100", 20, "Very Low")]
        [TestCase("1d100", 21, "Low")]
        [TestCase("1d100", 40, "Low")]
        [TestCase("1d100", 41, "Average")]
        [TestCase("1d100", 60, "Average")]
        [TestCase("1d100", 61, "High")]
        [TestCase("1d100", 80, "High")]
        [TestCase("1d100", 81, "Very High")]
        [TestCase("1d100", 100, "Very High")]
        public void DescribeRoll_VeryLowLowAverageHighVeryHigh(string rollExpression, int roll, string expectedDescription)
        {
            var description = dice.Describe(rollExpression, roll, "Very Low", "Low", "Average", "High", "Very High");
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase("1d20", 1, "Critical Failure")]
        [TestCase("1d20", 2, "OK")]
        [TestCase("1d20", 19, "OK")]
        [TestCase("1d20", 20, "Critical Success")]
        public void DescribeRoll_CritFailCrit(string rollExpression, int roll, string expectedDescription)
        {
            var descriptions = new[]
            {
                "Critical Failure",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "OK",
                "Critical Success",
            };

            var description = dice.Describe(rollExpression, roll, descriptions);
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase("1d20", 1, "Critical Failure")]
        [TestCase("1d20", 2, "Fail")]
        [TestCase("1d20", 13, "Fail")]
        [TestCase("1d20", 14, "Succeed")]
        [TestCase("1d20", 19, "Succeed")]
        [TestCase("1d20", 20, "Critical Success")]
        public void DescribeRoll_CritFailCritWithDC14(string rollExpression, int roll, string expectedDescription)
        {
            var descriptions = new[]
            {
                "Critical Failure",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Fail",
                "Succeed",
                "Succeed",
                "Succeed",
                "Succeed",
                "Succeed",
                "Succeed",
                "Critical Success",
            };

            var description = dice.Describe(rollExpression, roll, descriptions);
            Assert.That(description, Is.EqualTo(expectedDescription));
        }

        [TestCase("1d20", 1, "Critical Fail")]
        [TestCase("1d20", 3, "Critical Fail")]
        [TestCase("1d20", 4, "Great Fail")]
        [TestCase("1d20", 6, "Great Fail")]
        [TestCase("1d20", 7, "Fail")]
        [TestCase("1d20", 9, "Fail")]
        [TestCase("1d20", 10, "Standard")]
        [TestCase("1d20", 11, "Standard")]
        [TestCase("1d20", 12, "Success")]
        [TestCase("1d20", 14, "Success")]
        [TestCase("1d20", 15, "Great Success")]
        [TestCase("1d20", 17, "Great Success")]
        [TestCase("1d20", 18, "Critical Success")]
        [TestCase("1d20", 20, "Critical Success")]
        public void DescribeRoll_CritFailGreatFailFailStandardSuccessGreatSuccessCriticalSuccess(string rollExpression, int roll, string expectedDescription)
        {
            var descriptions = new[]
            {
                "Critical Fail",
                "Great Fail",
                "Fail",
                "Standard",
                "Success",
                "Great Success",
                "Critical Success",
            };

            var description = dice.Describe(rollExpression, roll, descriptions);
            Assert.That(description, Is.EqualTo(expectedDescription));
        }
    }
}
