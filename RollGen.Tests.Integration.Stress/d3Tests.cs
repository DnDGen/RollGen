using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d3Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 3; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d3().AsSum();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(10)]
        public override void FullRangeHit(int quantity)
        {
            AssertFullRangeHit(quantity);
        }

        [Test]
        public override void RollWithLargestDieRollPossible()
        {
            Stress(AssertRollWithLargestDieRollPossible);
        }
    }
}