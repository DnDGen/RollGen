using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d3Tests : ProvidedDiceTests
    {
        protected override int maximum
        {
            get { return 3; }
        }

        protected override int GetRoll()
        {
            return Dice.Roll().d3();
        }

        [Test]
        public override void FullRangeHit()
        {
            AssertFullRangeHit();
        }
    }
}