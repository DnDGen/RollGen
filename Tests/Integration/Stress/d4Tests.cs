using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d4Tests : ProvidedDiceTests
    {
        protected override int maximum
        {
            get { return 4; }
        }

        protected override int GetRoll()
        {
            return Dice.Roll().d4();
        }

        [Test]
        public override void FullRangeHit()
        {
            AssertFullRangeHit();
        }
    }
}