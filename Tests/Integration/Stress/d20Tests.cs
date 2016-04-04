using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d20Tests : ProvidedDiceTests
    {
        protected override int maximum
        {
            get { return 20; }
        }

        protected override int GetRoll()
        {
            return Dice.Roll().d20();
        }

        [Test]
        public override void FullRangeHit()
        {
            AssertFullRangeHit();
        }
    }
}