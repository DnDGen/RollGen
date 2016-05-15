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

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d12();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public override void FullRangeHit(int quantity)
        {
            AssertFullRangeHit(quantity);
        }
    }
}