using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d12Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 12; }
        }

        protected override int GetRoll()
        {
            return Dice.Roll().d12();
        }

        [Test]
        public override void FullRangeHit()
        {
            AssertFullRangeHit();
        }
    }
}