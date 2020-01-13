using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
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
            return Dice.Roll(quantity).d4().AsSum();
        }

        [Test]
        public void StressD4()
        {
            stressor.Stress(AssertRoll);
        }
    }
}