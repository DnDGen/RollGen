using Ninject;
using NUnit.Framework;
using System;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class RollHelperTests : StressTests
    {
        [Inject]
        public Random Random { get; set; }

        [Test]
        public void StressBuildRoll()
        {
            stressor.Stress(AssertBuildRoll);
        }

        private void AssertBuildRoll()
        {
            var upper = Random.Next(100_000_000);
            var lower = Random.Next(upper);

            var roll = RollHelper.GetRoll(lower, upper);

            Assert.That(roll, Is.Not.Empty
                .And.Matches("[0-9]d(100|20|12|10|8|6|4|3|2)"));
        }
    }
}
