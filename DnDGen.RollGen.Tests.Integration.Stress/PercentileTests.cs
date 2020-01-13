using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class PercentileTests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 100; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).Percentile().AsSum();
        }

        [Test]
        public void StressPercentile()
        {
            stressor.Stress(AssertRoll);
        }
    }
}