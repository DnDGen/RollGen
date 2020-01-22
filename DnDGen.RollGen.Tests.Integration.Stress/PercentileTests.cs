using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class PercentileTests : ProvidedDiceTests
    {
        protected override int die => 100;
        protected override PartialRoll GetRoll(int quantity) => Dice.Roll(quantity).Percentile();

        [Test]
        public void StressPercentile()
        {
            stressor.Stress(AssertRoll);
        }
    }
}