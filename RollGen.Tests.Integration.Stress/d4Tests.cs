using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d4Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 4; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d4();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        public override void FullRangeHit(int quantity)
        {
            AssertFullRangeHit(quantity);
        }

        [Test]
        public void RollWithLargestDieRollPossible()
        {
            Stress(AssertRollWithLargestDieRollPossible);
        }

        private void AssertRollWithLargestDieRollPossible()
        {
            var roll = Dice.Roll(Limits.Quantity).d4();
            Assert.That(roll, Is.InRange(Limits.Quantity, Limits.Quantity * 4));
        }
    }
}