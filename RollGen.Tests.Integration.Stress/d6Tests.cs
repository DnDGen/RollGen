using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d6Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 6; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d6();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        public override void FullRangeHit(int quantity)
        {
            AssertFullRangeHit(quantity);
        }
    }
}