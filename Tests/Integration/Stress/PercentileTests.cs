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

        protected override int GetRoll()
        {
            return Dice.Roll().Percentile();
        }

        [Test]
        public override void FullRangeHit()
        {
            AssertFullRangeHit();
        }
    }
}