using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d20Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 20; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d20();
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
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
            var roll = Dice.Roll(Limits.Quantity).d20();
            Assert.That(roll, Is.InRange(Limits.Quantity, Limits.Quantity * 20));
        }
    }
}