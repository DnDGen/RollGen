using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
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
            return Dice.Roll(quantity).Percentile();
        }

        [TestCase(1)]
        [TestCase(2)]
        public override void FullRangeHit(int quantity)
        {
            AssertFullRangeHit(quantity);
        }
    }
}