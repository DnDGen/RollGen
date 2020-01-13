using NUnit.Framework;

namespace DnDGen.RollGen.Tests.Integration.Stress
{
    [TestFixture]
    public class d10Tests : ProvidedDiceTests
    {
        protected override int die
        {
            get { return 10; }
        }

        protected override int GetRoll(int quantity)
        {
            return Dice.Roll(quantity).d10().AsSum();
        }

        [Test]
        public void StressD10()
        {
            stressor.Stress(AssertRoll);
        }
    }
}