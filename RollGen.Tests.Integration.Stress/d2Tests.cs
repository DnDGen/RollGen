using NUnit.Framework;

namespace RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d2Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 2; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d2().AsSum();
        }

        [Test]
        public override void RollWithLargestDieRollPossible()
        {
            Stress(AssertRollWithLargestDieRollPossible);
        }
    }
}